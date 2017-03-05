using MangaReader_MVVM.Converters.JSON;
using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services.FileService;
using MangaReader_MVVM.Services.SettingsServices;
using MangaReader_MVVM.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Template10.Services.NetworkAvailableService;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Services
{
    class MangaEdenSource : BindableBase, IMangaSource
    {
        public BitmapImage Icon { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon-mangaeden.png"));
        public MangaSource Name { get; } = MangaSource.MangaEden;
        private int Language { get; } = 0;
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

        //private AdvancedCollectionView _favorits;
        //public AdvancedCollectionView Favorits
        //{
        //    get { return _favorits = _favorits ?? GetFavoritMangasAsync().Result; }
        //    internal set { Set(ref _favorits, value); }
        //}

        //private Dictionary<string, List<string>> _readStatus;

        public MangaEdenSource()
        {
            RootUri = new Uri("http://www.mangaeden.com/api/");
            MangasListPage = new Uri($"list/{Language}/", UriKind.Relative);
            MangaDetails = new Uri("manga/", UriKind.Relative);
            MangaChapterPages = new Uri("chapter/", UriKind.Relative);
            Mangas = new ObservableItemCollection<Manga>();            
            _settings.PropertyChanged += Settings_Changed;
        }

        public async Task<ObservableItemCollection<Manga>> GetMangasAsync(ReloadMode mode = ReloadMode.Local)
        {
            if (!Mangas.Any() || mode == ReloadMode.Server)
            {
                var networkService = new NetworkAvailableService();
                if (await networkService.IsInternetAvailable())
                {
                    using (var httpClient = new HttpClient { BaseAddress = RootUri })
                    {
                        try
                        {
                            var response = await httpClient.GetAsync(MangasListPage);
                            var result = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                JsonSerializerSettings settings = new JsonSerializerSettings();
                                settings.Converters.Add(new MangaEdenMangasListConverter(this.Name));

                                Mangas = JsonConvert.DeserializeObject<ObservableItemCollection<Manga>>(result, settings);
                            }
                        }
                        catch (Exception e)
                        {
                            var dialog = new MessageDialog(e.Message);
                            await dialog.ShowAsync();
                        }
                    }
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("No Internet connection available. Check your connection and try again.");
                    //dialog.Title = "Manga Status Backup";
                    dialog.Commands.Add(new UICommand { Label = "Retry", Id = 0 });
                    dialog.Commands.Add(new UICommand { Label = "Exit", Id = 1 });
                    dialog.DefaultCommandIndex = 0;
                    dialog.CancelCommandIndex = 1;

                    var result = await dialog.ShowAsync();
                    if((int)result.Id == 0)
                    {
                        Mangas = await GetMangasAsync(mode);
                    }
                    else
                    {
                        App.Current.Exit();
                    }
                }
            }

            await LoadAndMergeStoredData();
            LoadLastRead();

            return Mangas;
        }        

        public async Task<Manga> GetMangaAsync(string mangaId)
        {
            var manga = Mangas.Where(m => m.Id == mangaId).First();
            var networkService = new NetworkAvailableService();
            if (await networkService.IsInternetAvailable())
            {
                using (var httpClient = new HttpClient { BaseAddress = RootUri })
                {
                    try
                    {
                        var mangaDetailsUri = new Uri(RootUri, MangaDetails);
                        var response = await httpClient.GetAsync(new Uri(mangaDetailsUri, mangaId));
                        var result = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            JsonSerializerSettings settings = new JsonSerializerSettings();
                            settings.Converters.Add(new MangaEdenMangaDetailsConverter());

                            var details = JsonConvert.DeserializeObject<Manga>(result, settings);

                            MergeMangaWithDetails(manga, details);
                        }
                    }
                    catch (Exception e)
                    {
                        var dialog = new MessageDialog(e.Message);
                        await dialog.ShowAsync();
                    }
                }
            }
            else
            {
                MessageDialog dialog = new MessageDialog("No Internet connection available. Check your connection and try again.");
                //dialog.Title = "Manga Status Backup";
                dialog.Commands.Add(new UICommand { Label = "Retry", Id = 0 });
                dialog.Commands.Add(new UICommand { Label = "Exit", Id = 1 });
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                var result = await dialog.ShowAsync();
                if ((int)result.Id == 0)
                {
                    manga = await GetMangaAsync(mangaId);
                }
                else
                {
                    App.Current.Exit();
                }
            }

            return manga;
        }

        public async Task<Chapter> GetChapterAsync(Chapter chapter)
        {
            var networkService = new NetworkAvailableService();
            if (await networkService.IsInternetAvailable())
            {
                using (var httpClient = new HttpClient { BaseAddress = RootUri })
                {
                    try
                    {
                        var chapterUri = new Uri(RootUri, MangaChapterPages);
                        var response = await httpClient.GetAsync(new Uri(chapterUri, chapter.Id));
                        var result = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            JsonSerializerSettings settings = new JsonSerializerSettings();
                            settings.Converters.Add(new MangaEdenChapterPagesConverter());

                            var pages = JsonConvert.DeserializeObject<ObservableItemCollection<Models.Page>>(result, settings);

                            chapter.Pages = pages;
                        }
                    }
                    catch (Exception e)
                    {
                        var dialog = new MessageDialog(e.Message);
                        await dialog.ShowAsync();
                    }
                }
            }
            else
            {
                MessageDialog dialog = new MessageDialog("No Internet connection available. Check your connection and try again.");
                //dialog.Title = "Manga Status Backup";
                dialog.Commands.Add(new UICommand { Label = "Retry", Id = 0 });
                dialog.Commands.Add(new UICommand { Label = "Exit", Id = 1 });
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                var result = await dialog.ShowAsync();
                if ((int)result.Id == 0)
                {
                    chapter = await GetChapterAsync(chapter);
                }
                else
                {
                    App.Current.Exit();
                }
            }
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

        public async Task<ObservableItemCollection<Chapter>> GetChaptersAsync(Manga manga)
        {
            throw new NotImplementedException();
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

        private async Task<bool> LoadAndMergeStoredData()
        {
            bool retval = false;
            if (await FileHelper.FileExistsAsync(this.Name + "_mangasStatus"))
            {
                _storedData = await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>(Name + "_mangasStatus", _settings.StorageStrategy);

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
                }
            }
            return retval;
        }

        private async void LoadLastRead()
        {
            if (await FileHelper.FileExistsAsync(this.Name + "_lastRead"))
            {
                var lastRead = await FileHelper.ReadFileAsync<ObservableItemCollection<Manga>>(Name + "_lastRead");
                LastRead.Clear();
                foreach (var manga in lastRead)
                {
                    LastRead.Add(Mangas.Where(m => m.Id == manga.Id).First());
                }
            }
        }

        public async Task<bool> SaveMangaStatusAsync(CreationCollisionOption option = CreationCollisionOption.ReplaceExisting)
        {
            if(_settings.StorageStrategy == StorageStrategies.OneDrive)
            {
                _settings.LastSynced = DateTime.Now;
            }
            return await FileHelper.WriteFileAsync<Dictionary<string, List<string>>>(Name + "_mangasStatus", _storedData, _settings.StorageStrategy, option);
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
                    var serializedMangas  = JsonConvert.SerializeObject(_storedData, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
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

                LoadAndMergeStoredData();

                var tempCollection = new ObservableItemCollection<Manga>(Mangas.Where(m => m.IsFavorit));
                foreach(var temp in tempCollection)
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
