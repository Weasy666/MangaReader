using MangaReader_MVVM.Converters.JSON;
using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services.FileService;
using MangaReader_MVVM.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
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
        private Dictionary<string, Manga> _storedData = new Dictionary<string, Manga>();

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
        }

        public async Task<ObservableItemCollection<Manga>> GetMangasAsync(ReloadMode mode = ReloadMode.Local)
        {
            if (!Mangas.Any() || mode == ReloadMode.Server)
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

            LoadAndMergeStoredData();

            return Mangas;
        }        

        public async Task<Manga> GetMangaAsync(string mangaId)
        {
            var manga = Mangas.Where(m => m.Id == mangaId).First();
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
            return manga;
        }

        public async Task<Chapter> GetChapterAsync(Chapter chapter)
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
                await FileHelper.WriteFileAsync<Dictionary<string, Manga>>(Name + "_mangasStatus", _storedData);
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
                    if (storedManga.ReadProgress > 0 || favorit.IsFavorit)
                    {
                        storedManga.IsFavorit = favorit.IsFavorit;
                    }
                    else
                    {
                        _storedData.Remove(favorit.Id);
                    }
                }
                else
                {
                    _storedData[favorit.Id] = favorit;
                }

                Favorits.Remove(favorit);
                if (favorit.IsFavorit)
                {
                    Favorits.AddSorted(favorit);
                }

                if (IsSingle)
                {
                    await FileHelper.WriteFileAsync<Dictionary<string, Manga>>(Name + "_mangasStatus", _storedData);
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
                await FileHelper.WriteFileAsync<Dictionary<string, Manga>>(Name + "_mangasStatus", _storedData);
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
                    storedManga.Chapters.Where(c => c.Id == chapter.Id).First().IsRead = true;
                }
                else
                {
                    _storedData[chapter.ParentManga.Id] = chapter.ParentManga;
                }

                if (IsSingle)
                {
                    await FileHelper.WriteFileAsync<Dictionary<string, Manga>>(Name + "_mangasStatus", _storedData);
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
                await FileHelper.WriteFileAsync<Dictionary<string, Manga>>(Name + "_mangasStatus", _storedData);
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
                    if (storedManga.ReadProgress > 0 || storedManga.IsFavorit)
                    {
                        storedManga.Chapters.Where(c => c.Id == chapter.Id).First().IsRead = false;
                    }
                    else
                    {
                        _storedData.Remove(chapter.ParentManga.Id);
                    }

                    if (IsSingle)
                    {
                        await FileHelper.WriteFileAsync<Dictionary<string, Manga>>(Name + "_mangasStatus", _storedData);
                    }
                }                
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
            foreach (var chapter in details.Chapters)
            {
                manga.AddChapter(chapter);
            }
        }

        private async void LoadAndMergeStoredData()
        {
            if (await FileHelper.FileExistsAsync(this.Name + "_mangasStatus"))
            {
                _storedData = await FileHelper.ReadFileAsync<Dictionary<string, Manga>>(Name + "_mangasStatus");

                for (int i = 0; i < Mangas.Count; i++)
                {
                    var manga = Mangas[i];
                    if (_storedData.ContainsKey(manga.Id))
                    {
                        var storedManga = _storedData[manga.Id];
                        if (storedManga != null)
                        {
                            Mangas[i] = storedManga;
                        }
                    }
                }
            }
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
                savePicker.SuggestedFileName = DateTime.Now.ToUniversalTime().ToString() + "_MangaReader_Backup";

                StorageFile file = await savePicker.PickSaveFileAsync();
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
                    return (status == FileUpdateStatus.Complete);
                }
                else
                {
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
                SuggestedStartLocation = PickerLocationId.Downloads
            };
            
            openPicker.FileTypeFilter.Add(".json");
            openPicker.CommitButtonText = "Import and overwrite";
            
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                _storedData = JsonConvert.DeserializeObject<Dictionary<string, Manga>>(await FileIO.ReadTextAsync(file));
                var result = await FileHelper.WriteFileAsync<Dictionary<string, Manga>>(Name + "_mangasStatus", _storedData);
                
                LoadAndMergeStoredData();

                var tempCollection = new ObservableItemCollection<Manga>(Mangas.Where(m => m.IsFavorit));
                foreach(var temp in tempCollection)
                {
                    if (!Favorits.Any(m => m.Id == temp.Id))
                    {
                       Favorits.AddSorted(temp);
                    }
                }

                return result;
            }
            else
            {
                return false;
            }
        }
    }
}
