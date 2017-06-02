using MangaReader_MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using System.Collections;
using System.Collections.ObjectModel;
using Template10.Controls;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using MangaReader_MVVM.Services.SettingsServices;
using System.ComponentModel;
using MangaReader_MVVM.Utils;
using Newtonsoft.Json;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using MangaReader_MVVM.Services.FileService;
using Template10.Services.NetworkAvailableService;
using System.Net.Http;
using Windows.UI.Popups;
using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;
using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;

namespace MangaReader_MVVM.Services
{
    class MangaFoxSource : BindableBase, IMangaSource
    {
        public BitmapImage Icon => new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon-mangaeden.png"));
        public MangaSource Name => MangaSource.MangaFox;
        public Uri RootUri { get; }
        public Uri MangasListPage { get; }
        public Uri MangaDetails { get; }
        public Uri MangaChapterPages { get; }

        private SettingsService _settings = SettingsService.Instance;

        private ObservableItemCollection<Manga> _mangas;
        public ObservableItemCollection<Manga> Mangas
        {
            get { return _mangas; }
            internal set { Set(ref _mangas, value); }
        }

        private ObservableItemCollection<Manga> _favorits;
        public ObservableItemCollection<Manga> Favorits
        {
            get => _favorits = _favorits ?? new ObservableItemCollection<Manga>(Mangas.Where(m => m.IsFavorit));
            internal set { Set(ref _favorits, value); }
        }

        private ObservableItemCollection<Manga> _lastRead;
        public ObservableItemCollection<Manga> LastRead
        {
            get => _lastRead = _lastRead ?? new ObservableItemCollection<Manga>();
            internal set { Set(ref _lastRead, value); }
        }

        private Dictionary<string, List<string>> _storedData = new Dictionary<string, List<string>>();

        private ObservableCollection<string> _categories;
        public ObservableCollection<string> Categories
        {
            get => _categories = _categories ?? GetCategories();
            internal set { Set(ref _categories, value); }
        }

        //private AdvancedCollectionView _favorits;
        //public AdvancedCollectionView Favorits
        //{
        //    get { return _favorits = _favorits ?? GetFavoritMangasAsync().Result; }
        //    internal set { Set(ref _favorits, value); }
        //}

        public MangaFoxSource()
        {
            RootUri = new Uri("http://mangafox.me/");
            MangasListPage = new Uri($"directory/{0}.htm?az", UriKind.Relative);
            MangaDetails = new Uri("manga/", UriKind.Relative);
            Mangas = new ObservableItemCollection<Manga>();
            _settings.PropertyChanged += Settings_Changed;
        }

        private Dictionary<string, string> MangasRefs { get; } = new Dictionary<string, string>();
        private Dictionary<string, string> CurrentMangaChaptersRef { get; } = new Dictionary<string, string>();

        public async Task<ObservableItemCollection<Manga>> GetMangasAsync(ReloadMode mode = ReloadMode.Local)
        {
            //try
            {
                if (!Mangas.Any() || mode == ReloadMode.Server)
                {
                    var mangaList = new List<IElement>();
                    await GetPagedMangas(mangaList, 1);
                }

                await LoadAndMergeStoredDataAsync();
                LoadLastReadAsync();
            }
            //catch (HttpRequestException e)
            //{
            //    MessageDialog dialog = new MessageDialog(e.Message);
            //    dialog.Commands.Add(new UICommand { Label = "Retry", Id = 0 });
            //    dialog.Commands.Add(new UICommand { Label = "Exit", Id = 1 });
            //    dialog.DefaultCommandIndex = 0;
            //    dialog.CancelCommandIndex = 1;

            //    var result = await dialog.ShowAsync();
            //    if ((int)result.Id == 0)
            //    {
            //        Mangas = await GetMangasAsync(mode);
            //    }
            //    else
            //    {
            //        App.Current.Exit();
            //    }
            //}
            //catch (Exception e)
            //{
            //    var dialog = new MessageDialog(e.Message);
            //    await dialog.ShowAsync();
            //}
            return Mangas;
        }

        private async Task<List<IElement>> GetPagedMangas(List<IElement> mangaList, int currentPage)
        {
            var networkService = new NetworkAvailableService();
            if (await networkService.IsInternetAvailable())
            {
                using (var httpClient = new HttpClient { BaseAddress = RootUri })
                {
                    var response = await httpClient.GetAsync(new Uri($"directory/{currentPage}.htm?az", UriKind.Relative));
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var parser = new HtmlParser();
                        var document = parser.Parse(result);

                        var divContent = document.All.OfType<IHtmlDivElement>().Where(m => m.Id == "content").FirstOrDefault();
                        mangaList.AddRange(divContent.QuerySelector("ul.list")?.Children);// as IHtmlUnorderedListElement;

                        var ulNav = divContent.QuerySelector("div#nav").FirstElementChild;

                        var spanNextParent = ulNav.QuerySelector("span.next").ParentElement as IHtmlSpanElement;

                        bool HasNextPage = spanNextParent == null || spanNextParent.ClassName != "disable";

                        if (HasNextPage)
                        {
                            await GetPagedMangas(mangaList, currentPage + 1);
                        }

                        //var groupedMangaList = document.QuerySelector("div.manga_list");
                        //var mangaList = groupedMangaList.QuerySelectorAll("a.series_preview").OfType<IHtmlAnchorElement>();

                        //foreach (var manga in mangaList)
                        //{
                        //    MangasRefs[manga.Relation] = manga.PathName;
                        //    Mangas.Add(new Manga
                        //    {
                        //        MangaSource = Name,
                        //        Title = manga.Text,
                        //        Id = manga.Relation,
                        //        IsFavorit = false
                        //    });
                        //    //await GetMangaAsync(manga.Relation);
                        //}
                    }
                    else
                    {
                        throw new HttpRequestException(Name + ": " + response.ReasonPhrase);
                    }
                }
            }
            else
            {
                throw new HttpRequestException("No Internet connection available. Check your connection and try again.");
            }

            return mangaList;
        }

        public async Task<Manga> GetMangaAsync(string mangaId)
        {
            var manga = Mangas.Where(m => m.Id == mangaId).First();
            //try
            {
                var networkService = new NetworkAvailableService();
                if (await networkService.IsInternetAvailable())
                {
                    using (var httpClient = new HttpClient { BaseAddress = RootUri })
                    {
                        var mangaDetailsUri = new Uri(RootUri, MangaDetails);
                        var response = await httpClient.GetAsync(new Uri(mangaDetailsUri, MangasRefs[mangaId]));
                        var result = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            var parser = new HtmlParser();
                            var document = parser.Parse(result);
                            //page sections
                            var leftDiv = document.All.OfType<IHtmlDivElement>().Where(m => m.ClassName == "left").First();
                            var titleDiv = leftDiv.Children.OfType<IHtmlDivElement>().Where(m => m.Id == "title").First();
                            var seriesInfoDiv = leftDiv.Children.OfType<IHtmlDivElement>().Where(m => m.Id == "series_info").First();
                            var chaptersDiv = leftDiv.Children.OfType<IHtmlDivElement>().Where(m => m.Id == "chapters").First();
                            var chaptersList = chaptersDiv.QuerySelectorAll("li");
                            //titleDiv
                            var alias = titleDiv.Children.OfType<IHtmlHeadingElement>().Where(m => m.LocalName == "h3").FirstOrDefault()?.TextContent ?? manga.Title;
                            var description = titleDiv.QuerySelector("p.summary")?.TextContent ?? "N/A";
                            var table = titleDiv.QuerySelectorAll("td");
                            var released = new DateTime();
                            if (DateTime.TryParseExact(table[0].TextContent.Trim(), "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out released))
                                released = released.ToLocalTime();
                            var author = table[1].TextContent.Trim();
                            var artist = table[2].TextContent.Trim();
                            var category = table[3].TextContent.Trim();
                            //seriesInfoDiv
                            var cover = new BitmapImage(new Uri((seriesInfoDiv.QuerySelector("div.cover").FirstElementChild as IHtmlImageElement).Source));
                            var data = seriesInfoDiv.QuerySelectorAll("div.data");
                            var ongoing = !data[0].TextContent.Contains("Completed");
                            var hits = Int32.Parse(Regex.Match(data[1].TextContent, "\\d+.\\d+\\s").Value.Trim().Replace(",", ""));
                            //chaptersDiv
                            IElement chapDate = chaptersList != null && chaptersList.Any() ? chaptersList.ElementAt(0)?.QuerySelector("span.date") : null;
                            var lastUpdated = ConvertToDateTime(chapDate);



                            //creation of manga
                            var details = new Manga
                            {
                                MangaSource = this.Name,
                                Alias = alias,
                                Cover = cover,
                                Category = category,
                                Author = author,
                                Artist = artist,
                                Description = description,
                                Hits = hits,
                                Released = released,
                                LastUpdated = lastUpdated,
                                Ongoing = ongoing,
                                NumberOfChapters = chaptersList.Length,
                            };

                            var chapters = new List<Chapter>();
                            CurrentMangaChaptersRef.Clear();
                            foreach (var chapter in chaptersList)
                            {
                                var chapterId = chapter.QuerySelector("a.tips").TextContent;

                                var chapNumber = Int32.Parse(Regex.Match(chapter.QuerySelector("a.tips").TextContent, "\\d+$").Value);
                                var chapTitle = chapter.QuerySelector("span.title") != null ? chapter.QuerySelector("span.title").TextContent : chapter.QuerySelector("a.tips").TextContent;
                                var chapReleased = ConvertToDateTime(chapter.QuerySelector("span.date"));

                                new Chapter
                                {
                                    Number = chapNumber,
                                    Title = chapTitle,
                                    Id = chapterId,
                                    Released = chapReleased,
                                    IsRead = false
                                };
                                //add chapter Hrefs to CurrentMangaChaptersRef                                
                                CurrentMangaChaptersRef[chapterId] = (chapter.QuerySelector("a.tips") as IHtmlAnchorElement).Href;
                            }
                            chapters.Sort();
                            manga.Chapters = new ObservableItemCollection<Chapter>(chapters);


                            MergeMangaWithDetails(manga, details);
                        }
                        else
                        {
                            throw new HttpRequestException(Name + ": " + response.ReasonPhrase);
                        }
                    }
                }
                else
                {
                    throw new HttpRequestException("No Internet connection available. Check your connection and try again.");
                }
            }
            //catch (HttpRequestException e)
            //{
            //    MessageDialog dialog = new MessageDialog(e.Message);
            //    dialog.Commands.Add(new UICommand { Label = "Retry", Id = 0 });
            //    dialog.Commands.Add(new UICommand { Label = "Exit", Id = 1 });
            //    dialog.DefaultCommandIndex = 0;
            //    dialog.CancelCommandIndex = 1;

            //    var result = await dialog.ShowAsync();
            //    if ((int)result.Id == 0)
            //    {
            //        manga = await GetMangaAsync(mangaId);
            //    }
            //    else
            //    {
            //        App.Current.Exit();
            //    }
            //}
            //catch (Exception e)
            //{
            //    var dialog = new MessageDialog(e.Message);
            //    await dialog.ShowAsync();
            //}

            return manga;
        }

        public async Task<Chapter> GetChapterAsync(Chapter chapter)
        {
            //try
            {
                var networkService = new NetworkAvailableService();
                if (await networkService.IsInternetAvailable())
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(new Uri(CurrentMangaChaptersRef[chapter.Id]));
                        var result = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            var parser = new HtmlParser();
                            var document = parser.Parse(result);

                            //var pages = JsonConvert.DeserializeObject<ObservableItemCollection<Models.Page>>(result, settings);

                            //chapter.Pages = pages;
                        }
                        else
                        {
                            throw new HttpRequestException(Name + ": " + response.ReasonPhrase);
                        }
                    }
                }
                else
                {
                    throw new HttpRequestException("No Internet connection available. Check your connection and try again.");
                }
            }
            //catch (HttpRequestException e)
            //{
            //    MessageDialog dialog = new MessageDialog(e.Message);
            //    dialog.Commands.Add(new UICommand { Label = "Retry", Id = 0 });
            //    dialog.Commands.Add(new UICommand { Label = "Exit", Id = 1 });
            //    dialog.DefaultCommandIndex = 0;
            //    dialog.CancelCommandIndex = 1;

            //    var result = await dialog.ShowAsync();
            //    if ((int)result.Id == 0)
            //    {
            //        chapter = await GetChapterAsync(chapter);
            //    }
            //    else
            //    {
            //        App.Current.Exit();
            //    }
            //}
            //catch (Exception e)
            //{
            //    var dialog = new MessageDialog(e.Message);
            //    await dialog.ShowAsync();
            //}

            return chapter;
        }

        public async Task<ObservableItemCollection<Manga>> GetLatestReleasesAsync(int numberOfPastDays, ReloadMode mode)
        {
            if ((!Mangas.Any() && mode == ReloadMode.Local) || mode == ReloadMode.Server)
            {
                await GetMangasAsync(mode);
            }

            var latestReleases = new ObservableItemCollection<Manga>(Mangas.Where(manga => manga.LastUpdated.AddDays(numberOfPastDays) >= DateTime.Today));
            latestReleases.SortAscending((y, x) => y == null ? 1 : DateTime.Compare(x.LastUpdated, y.LastUpdated));

            return latestReleases;
        }

        public async void AddFavorit(ObservableItemCollection<Manga> newFavorits)
        {
            if (newFavorits != null && newFavorits.Any())
            {
                foreach (var fav in newFavorits)
                {
                    AddFavorit(fav, false);
                }
                await SaveMangaStatusAsync();
            }
        }

        public async void AddFavorit(Manga favorit, bool IsSingle = true)
        {
            if (favorit != null)
            {
                favorit.IsFavorit = !favorit.IsFavorit;

                if (_storedData.ContainsKey(favorit.Id))
                {
                    var storedManga = _storedData[favorit.Id];
                    if (!storedManga.Any() && !favorit.IsFavorit)
                    {
                        _storedData.Remove(favorit.Id);
                    }
                    else
                    {
                        storedManga[0] = favorit.IsFavorit.ToString();
                    }
                }
                else
                {
                    _storedData[favorit.Id] = new List<string> { favorit.IsFavorit.ToString() };

                    foreach (var chapter in favorit.Chapters)
                    {
                        if (chapter.IsRead)
                        {
                            _storedData[favorit.Id].Add(chapter.Id);
                        }
                    }
                }

                Favorits.Remove(favorit);
                if (favorit.IsFavorit)
                {
                    Favorits.AddSorted(favorit);
                }

                if (IsSingle)
                {
                    await SaveMangaStatusAsync();
                }
            }
        }

        public async void AddAsRead(ObservableItemCollection<Chapter> chapters)
        {
            if (chapters != null && chapters.Any())
            {
                foreach (var chapter in chapters)
                {
                    AddAsRead(chapter, false);
                }
                await SaveMangaStatusAsync();
                await FileHelper.WriteFileAsync<ObservableItemCollection<Manga>>(Name + "_lastRead", LastRead);
            }
        }

        public async void AddAsRead(Chapter chapter, bool IsSingle = true)
        {
            if (chapter.ParentManga.Id != null && chapter != null)
            {
                chapter.IsRead = true;

                if (_storedData.ContainsKey(chapter.ParentManga.Id))
                {
                    var storedManga = _storedData[chapter.ParentManga.Id];
                    if (!storedManga.Contains(chapter.Id))
                    {
                        storedManga.Add(chapter.Id);
                    }
                }
                else
                {
                    _storedData[chapter.ParentManga.Id] = new List<string> { "False", chapter.Id };
                }

                AddAsLastRead(chapter);

                if (IsSingle)
                {
                    await SaveMangaStatusAsync();
                    await FileHelper.WriteFileAsync<ObservableItemCollection<Manga>>(Name + "_lastRead", LastRead);
                }
            }
        }

        public async void RemoveAsRead(ObservableItemCollection<Chapter> chapters)
        {
            if (chapters != null && chapters.Any())
            {
                foreach (var chapter in chapters)
                {
                    RemoveAsRead(chapter, false);
                }
                await SaveMangaStatusAsync();
            }
        }

        public async void RemoveAsRead(Chapter chapter, bool IsSingle = true)
        {
            if (chapter.ParentManga.Id != null && chapter != null)
            {
                chapter.IsRead = false;

                if (_storedData.ContainsKey(chapter.ParentManga.Id))
                {
                    var storedManga = _storedData[chapter.ParentManga.Id];
                    if (storedManga.Contains(chapter.Id))
                    {
                        storedManga.Remove(chapter.Id);
                    }

                    if (IsSingle)
                    {
                        await SaveMangaStatusAsync();
                    }
                }
            }
        }

        private void AddAsLastRead(Chapter chapter)
        {
            if (LastRead.Contains(chapter.ParentManga))
            {
                LastRead.Remove(chapter.ParentManga);
            }

            LastRead.Insert(0, chapter.ParentManga);
            UpdateLastRead();
        }

        public void UpdateLastRead()
        {
            while (LastRead.Count > _settings.NumberOfRecentMangas)
            {
                LastRead.RemoveAt(LastRead.Count - 1);
            }
        }

        public ObservableItemCollection<Manga> SearchManga(string query)
        {
            return new ObservableItemCollection<Manga>(Mangas.Where(manga => manga.Title.ToLower().Contains(query.ToLower()) && query != string.Empty));
        }

        public ObservableItemCollection<Manga> FilterMangaByCategory(IEnumerable filters)
        {
            var retval = Mangas;

            if (filters != null)
            {
                var temp = retval.AsEnumerable();
                foreach (string filter in filters)
                {
                    temp = temp.Where(m => m.Category.Contains(filter));
                }
                retval = new ObservableItemCollection<Manga>(temp);
            }

            return retval;
        }

        public async Task<ObservableItemCollection<Chapter>> GetChaptersAsync(Manga manga)
        {
            throw new NotImplementedException();
        }

        private ObservableCollection<string> GetCategories()
        {
            var hashSet = new HashSet<string>();
            if (Mangas != null && Mangas.Any())
            {
                foreach (var manga in Mangas)
                {
                    var categories = manga.Category.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToArray();
                    hashSet.UnionWith(categories);
                }
            }
            var retval = new ObservableCollection<string>(hashSet);
            retval.SortAscending((y, x) => y.CompareTo(x));
            return retval;
        }

        private DateTime ConvertToDateTime(IElement element)
        {
            var retval = new DateTime();
            if (element != null && element.TextContent.Contains("Today"))
            {
                retval = DateTime.Today;
            }
            else if (element != null && element.TextContent.Contains("Yesterday"))
            {
                retval = DateTime.Today.AddDays(-1);
            }
            else if (element != null)
            {
                Convert.ToDateTime(element.TextContent).ToLocalTime();
            }
            return retval;
        }

        private void MergeMangaWithDetails(Manga manga, Manga details)
        {
            manga.Alias = details.Alias;
            manga.Artist = details.Artist;
            manga.Author = details.Author;
            manga.Description = details.Description;
            manga.NumberOfChapters = details.NumberOfChapters;
            manga.Released = details.Released;
            List<string> storedManga = null;
            if (_storedData.ContainsKey(manga.Id))
            {
                storedManga = _storedData[manga.Id];
            }
            foreach (var chapter in details.Chapters)
            {
                if (storedManga != null)
                    chapter.IsRead = storedManga.Contains(chapter.Id);
                manga.AddChapter(chapter);
            }
        }

        public async Task<bool> LoadAndMergeStoredDataAsync()
        {
            bool retval = false;
            if (await FileHelper.FileExistsAsync(this.Name + "_mangasStatus", _settings.StorageStrategy))
            {
                if (_settings.StorageStrategy == StorageStrategies.OneDrive)
                {
                    var oneDriveContent = await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>(Name + "_mangasStatus", _settings.StorageStrategy);
                    var localContent = await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>(Name + "_mangasStatus");

                    if (oneDriveContent.Any() && !localContent.Any())
                    {
                        localContent = oneDriveContent;
                        _storedData = oneDriveContent;
                        await SaveMangaStatusAsync();
                    }
                    else if (!oneDriveContent.Any() && localContent.Any())
                    {
                        oneDriveContent = localContent;
                        _storedData = localContent;
                        await SaveMangaStatusAsync();
                    }
                    else if (oneDriveContent.Any() && localContent.Any())
                    {
                        for (int i = 0; i < localContent.Count; i++)
                        {
                            var pair = localContent.ElementAt(i);
                            if (oneDriveContent.ContainsKey(pair.Key))
                            {
                                var oneDriveValue = oneDriveContent[pair.Key];
                                oneDriveValue.Union(pair.Value);
                                oneDriveContent[pair.Key] = oneDriveValue;
                            }
                            else
                            {
                                oneDriveContent[pair.Key] = pair.Value;
                            }
                        }
                        localContent = oneDriveContent;
                        _storedData = oneDriveContent;
                        await SaveMangaStatusAsync();
                    }
                }
                else
                {
                    _storedData = await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>(Name + "_mangasStatus", _settings.StorageStrategy);
                }

                if (_storedData != null && _storedData.Any())
                {
                    for (int i = 0; i < Mangas.Count; i++)
                    {
                        var manga = Mangas[i];
                        if (_storedData.ContainsKey(manga.Id))
                        {
                            var storedManga = _storedData[manga.Id];

                            if (storedManga.Count == 0)
                                storedManga.Add("True");

                            Mangas[i].IsFavorit = bool.TryParse(storedManga[0], out bool isFavorit) ? isFavorit : false;
                        }
                    }
                    retval = true;
                    Favorits = new ObservableItemCollection<Manga>(Mangas.Where(m => m.IsFavorit));
                }
            }
            return retval;
        }

        private async void LoadLastReadAsync()
        {
            if (await FileHelper.FileExistsAsync(this.Name + "_lastRead"))
            {
                var lastRead = await FileHelper.ReadFileAsync<ObservableItemCollection<Manga>>(Name + "_lastRead");
                LastRead.Clear();
                if (Mangas != null && Mangas.Any())
                {
                    foreach (var manga in lastRead)
                    {
                        LastRead.Add(Mangas.Where(m => m.Id == manga.Id).First());
                    }
                }
            }
        }

        public async Task<bool> SaveMangaStatusAsync(CreationCollisionOption option = CreationCollisionOption.ReplaceExisting)
        {
            var retval = await FileHelper.WriteFileAsync<Dictionary<string, List<string>>>(Name + "_mangasStatus", _storedData, _settings.StorageStrategy, option);
            if (_settings.StorageStrategy == StorageStrategies.OneDrive)
            {
                _settings.LastSynced = DateTime.Now;
            }
            return retval;
        }

        public async Task<bool> ExportMangaStatusAsync()
        {
            if (_storedData != null && _storedData.Any())
            {
                FileSavePicker savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.Downloads
                };

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("JSON", new List<string>() { ".json" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = DateTime.Now.ToString("s") + "_MangaReader_Backup_" + this.Name;

                StorageFile file = await savePicker.PickSaveFileAsync();
                Views.Busy.SetBusy(true, "Printing your favorite Mangas...");
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);

                    // write to file
                    var serializedMangas = JsonConvert.SerializeObject(_storedData, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.Objects,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                    });

                    await FileIO.WriteTextAsync(file, serializedMangas);

                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                    Views.Busy.SetBusy(false);
                    return (status == FileUpdateStatus.Complete);
                }
                else
                {
                    Views.Busy.SetBusy(false);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ImportMangaStatusAsync()
        {
            FileOpenPicker openPicker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
                CommitButtonText = "Import"
            };

            openPicker.FileTypeFilter.Add(".json");

            StorageFile file = await openPicker.PickSingleFileAsync();
            Views.Busy.SetBusy(true, "Importing your favorite Mangas...");
            if (file != null)
            {
                var _storedData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(await FileIO.ReadTextAsync(file));

                var result = await SaveMangaStatusAsync();

                await LoadAndMergeStoredDataAsync();

                var tempCollection = new ObservableItemCollection<Manga>(Mangas.Where(m => m.IsFavorit));
                foreach (var temp in tempCollection)
                {
                    if (!Favorits.Contains(temp))
                    {
                        Favorits.AddSorted(temp);
                    }
                }

                Views.Busy.SetBusy(false);
                return result;
            }
            else
            {
                Views.Busy.SetBusy(false);
                return false;
            }
        }

        private void Settings_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (nameof(_settings.NumberOfRecentMangas).Equals(e.PropertyName))
                UpdateLastRead();
        }
    }
}