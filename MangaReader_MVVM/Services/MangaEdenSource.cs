using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Template10.Services.FileService;
using MangaReader_MVVM.Models;
using MangaReader_MVVM.Utils;
using Template10.Mvvm;

namespace MangaReader_MVVM.Services
{
    class MangaEdenSource : ViewModelBase, IMangaSource
    {
        public BitmapImage Icon { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon-mangaeden.png"));
        public MangaSource Name { get; } = MangaSource.MangaEden;
        private int Language { get; } = 0;
        public Uri RootUri { get; }
        public Uri MangasListPage { get; }
        public Uri MangaDetails { get; }
        public Uri MangaChapterPages { get; }

        private ObservableCollection<IManga> _mangas;
        public ObservableCollection<IManga> Mangas
        {
            get { return _mangas = _mangas ?? new ObservableCollection<IManga>(); }
            internal set { Set(ref _mangas, value); }
        }

        private ObservableCollection<IManga> _favorits;
        public ObservableCollection<IManga> Favorits
        {
            get { return _favorits = _favorits ?? GetFavoritMangasAsync().Result; }
            internal set { Set(ref _favorits, value); }
        }

        private Dictionary<string, List<string>> _readStatus;

        public MangaEdenSource()
        {
            RootUri = new Uri("http://www.mangaeden.com/api/");
            MangasListPage = new Uri($"list/{Language}/", UriKind.Relative);
            MangaDetails = new Uri("manga/", UriKind.Relative);
            MangaChapterPages = new Uri("chapter/", UriKind.Relative);
            LoadReadStatus();
        }

        private async void LoadReadStatus()
        {
            _readStatus = await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, StorageStrategies.Roaming);
        }

        public async Task<ObservableCollection<IManga>> GetMangasAsync(ReloadMode mode = ReloadMode.Local)
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
                            settings.Converters.Add(new MangaEdenMangasListConverter());

                            Mangas = JsonConvert.DeserializeObject<ObservableCollection<IManga>>(result, settings);
                        }
                    }
                    catch (Exception e)
                    {
                        var dialog = new MessageDialog(e.Message);
                        await dialog.ShowAsync();
                    }
                }
            }

            LoadAndMergeFavorits();

            return Mangas;
        }        

        public async Task<IManga> GetMangaAsync(string mangaId)
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
                        
                        var details = JsonConvert.DeserializeObject<IManga>(result, settings);
                        
                        MergeMangaWithDetails(manga, details);
                        int i = Mangas.IndexOf(manga);
                        Mangas[i] = manga;
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

        public async Task<IChapter> GetChapterAsync(IChapter chapter)
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

                        var pages = JsonConvert.DeserializeObject<ObservableCollection<IPage>>(result, settings);

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

        public async Task<ObservableCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode = ReloadMode.Local)
        {
            if (!Mangas.Any() || mode == ReloadMode.Server)
            {
                await GetMangasAsync(ReloadMode.Local);
                Favorits = new ObservableCollection<IManga>(Mangas.Where(manga => manga.IsFavorit).ToList());
            }
            else if (_favorits == null || !Favorits.Any())
            {
                Favorits = new ObservableCollection<IManga>(Mangas.Where(manga => manga.IsFavorit).ToList());
            }

            return Favorits;
        }

        public async Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays, ReloadMode mode)
        {
            if (!Mangas.Any() || mode == ReloadMode.Server)
            {
                await GetMangasAsync(ReloadMode.Local);
            }
            var latestReleases = Mangas.Where(manga => manga.LastUpdated.AddDays(numberOfPastDays) >= DateTime.Today).ToList();
            latestReleases.Sort((y, x) => y == null ? 1 : DateTime.Compare(x.LastUpdated, y.LastUpdated));

            return new ObservableCollection<IManga>(latestReleases);
        }

        public async void AddFavorit(ObservableCollection<IManga> newFavorits)
        {
            if (newFavorits != null && newFavorits.Any())
            {
                var favorits = await FileHelper.ReadFileAsync<List<string>>("favorits_" + this.Name, StorageStrategies.Roaming) ?? new List<string>();
                foreach (var fav in newFavorits)
                {
                    AddFavorit(fav, favorits);
                }
            }
        }

        public async void AddFavorit(IManga favorit, List<string> favorits)
        {
            if (favorit != null)
            { 
                favorit.IsFavorit = !favorit.IsFavorit;
                favorits = favorits ?? await FileHelper.ReadFileAsync<List<string>>("favorits_" + this.Name, StorageStrategies.Roaming) ?? new List<string>();

                if (Favorits.Contains(favorit))
                {
                    favorits.Remove(favorit.Id);
                    Favorits.Remove(favorit);

                    if (favorit.IsFavorit)
                    {
                        favorits.Add(favorit.Id);
                        Favorits.AddSorted(favorit);
                    }
                }
                else
                {
                    favorits.Add(favorit.Id);
                    Favorits.AddSorted(favorit);
                }

                await FileHelper.WriteFileAsync<List<string>>("favorits_" + this.Name, favorits, StorageStrategies.Roaming);
            }
        }

        public async void AddAsRead(string mangaId, ObservableCollection<IChapter> chapters)
        {
            if (chapters != null && chapters.Any())
            {
                foreach (var chapter in chapters)
                {
                    AddAsRead(mangaId, chapter, false);
                }
                await FileHelper.WriteFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, _readStatus, StorageStrategies.Roaming);
            }
        }

        public async void AddAsRead(string mangaId, IChapter chapter, bool single = true)
        {
            if (mangaId != null && chapter != null)
            {
                chapter.IsRead = true;
                _readStatus = _readStatus ?? await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, StorageStrategies.Roaming) ?? new Dictionary<string, List<string>>();
                if (_readStatus.ContainsKey(mangaId))
                {
                    var mangaChaptersWithStatus = _readStatus[mangaId];
                    var chapterWithStatus = mangaChaptersWithStatus.FirstOrDefault(c => c == chapter.Id);
                    if (chapterWithStatus != null && chapterWithStatus.Any())
                    {
                        //mangaChaptersWithStatus.Remove(chapter.Id);
                    }                        
                    else
                    {
                        mangaChaptersWithStatus.Add(chapter.Id);
                        _readStatus[mangaId] = mangaChaptersWithStatus;   
                        // evtl noch das chapter in _mangas und _favorits liste ersetzen                    
                    }                    
                }
                else
                {
                    _readStatus[mangaId] = new List<string>() { chapter.Id };
                }
                if (single)
                {
                    await FileHelper.WriteFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, _readStatus, StorageStrategies.Roaming);
                }
            }
        }

        public async void RemoveAsRead(string mangaId, ObservableCollection<IChapter> chapters)
        {
            if (chapters != null && chapters.Any())
            {
                foreach (var chapter in chapters)
                {
                    RemoveAsRead(mangaId, chapter, false);
                }
                if (await FileHelper.FileExistsAsync("readStatus_" + this.Name, StorageStrategies.Roaming))
                {
                    await FileHelper.WriteFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, _readStatus, StorageStrategies.Roaming);
                }
            }
        }

        public async void RemoveAsRead(string mangaId, IChapter chapter, bool single = true)
        {
            if (mangaId != null && chapter != null)
            {
                chapter.IsRead = false;
                _readStatus = _readStatus ?? await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, StorageStrategies.Roaming);
                if (_readStatus != null && _readStatus.ContainsKey(mangaId))
                {
                    _readStatus[mangaId]?.Remove(chapter.Id);

                    if (single)
                    {
                        await FileHelper.WriteFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, _readStatus, StorageStrategies.Roaming);
                    }
                }
            }
        }

        public ObservableCollection<IManga> SearchManga(string query)
        {
            return new ObservableCollection<IManga>(Mangas.Where(manga => manga.Title.ToLower().Contains(query.ToLower()) && query != string.Empty));
        }

        public async Task<ObservableCollection<IChapter>> GetChaptersAsync(IManga manga)
        {
            throw new NotImplementedException();
        }

        private void MergeMangaWithDetails(IManga manga, IManga details)
        {
            manga.Alias = details.Alias;
            manga.Artist = details.Artist;
            manga.Author = details.Author;
            manga.Description = details.Description;
            manga.NumberOfChapters = details.NumberOfChapters;
            manga.Released = details.Released;
            foreach (var chapter in details.Chapters)
            {
                LoadAndMergeReadStatus(manga.Id, chapter);
                manga.AddChapter(chapter);
                manga.RaisePropertyChanged(nameof(manga.ReadProgress));
            }
        }

        private async void LoadAndMergeFavorits()
        {
            if (await FileHelper.FileExistsAsync("favorits_" + this.Name, StorageStrategies.Roaming))
            {
                var favorits = await FileHelper.ReadFileAsync<List<string>>("favorits_" + this.Name, StorageStrategies.Roaming);
                foreach (var id in favorits)
                {
                    var manga = Mangas.FirstOrDefault(m => m.Id == id);
                    if (manga != null)
                        manga.IsFavorit = true;
                }
            }
        }

        private async void LoadAndMergeReadStatus(string mangaId, IChapter chapter)
        {
            _readStatus = _readStatus ?? await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, StorageStrategies.Roaming);

            if (_readStatus != null && _readStatus.ContainsKey(mangaId))
            {
                var chapterWithStatus = _readStatus[mangaId].FirstOrDefault(c => c == chapter.Id);
                if (chapterWithStatus != null)
                {
                    chapter.IsRead = true;

                    //var manga =_mangas.First(m => m.Id == mangaId);
                    //manga.ReadProgress = manga.Chapters.Count(c => c.IsRead == true);
                    //_favorits.First(m => m.Id == mangaId).ReadProgress = manga.ReadProgress;
                }
            }
        }
    }
}
