﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
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

        public BitmapImage Icon { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon-mangaeden.png"));
        public MangaSource Name { get; } = MangaSource.MangaEden;
        private int Language { get; } = 0;
        public Uri RootUri { get; }
        public Uri MangasListPage { get; }
        public Uri MangaDetails { get; }
        public Uri MangaChapterPages { get; }

        public MangaEdenSource()
        {
            RootUri = new Uri("http://www.mangaeden.com/api/");
            MangasListPage = new Uri($"list/{Language}/", UriKind.Relative);
            MangaDetails = new Uri("manga/", UriKind.Relative);
            MangaChapterPages = new Uri("chapter/", UriKind.Relative);
        }

        public async Task<ObservableCollection<IManga>> GetMangasAsync(ReloadMode mode = ReloadMode.Default)
        {
            if (!Mangas.Any() || mode == ReloadMode.FromSource)
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

        public async Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays, ReloadMode mode)
        {
            if (!Mangas.Any() || mode == ReloadMode.FromSource)
            {
                await GetMangasAsync(ReloadMode.Default);
            }
            var latestReleases = Mangas.Where(manga => manga.LastUpdated.AddDays(numberOfPastDays) >= DateTime.Today).ToList();
            latestReleases.Sort((y, x) => y == null ? 1 : DateTime.Compare(x.LastUpdated, y.LastUpdated));

            return new ObservableCollection<IManga>(latestReleases);
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
            }
        }
        
        public async Task<ObservableCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode = ReloadMode.Default)
        {
            if (!Mangas.Any() || mode == ReloadMode.FromSource)
            {
                await GetMangasAsync(ReloadMode.Default);
                Favorits = new ObservableCollection<IManga>(Mangas.Where(manga => manga.IsFavorit).ToList());
            }
            else if (_favorits == null || !Favorits.Any())
            {
                Favorits = new ObservableCollection<IManga>(Mangas.Where(manga => manga.IsFavorit).ToList());
            }

            return Favorits;
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
                    favorits?.Remove(favorit.Id);
                    Favorits.Remove(favorit);

                    if (favorit.IsFavorit)
                        Favorits.AddSorted(favorit);
                }
                else
                {
                    favorits.Add(favorit.Id);
                    Favorits.AddSorted(favorit);
                    //var toSort = Favorits.ToList();
                    //toSort.Sort();
                    //Favorits = new ObservableCollection<IManga>(toSort);
                }

                await FileHelper.WriteFileAsync<List<string>>("favorits_" + this.Name, favorits, StorageStrategies.Roaming);
            }
        }

        public async void AddAsRead(string mangaId, IChapter chapter)
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
                await FileHelper.WriteFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, _readStatus, StorageStrategies.Roaming);
            }
        }

        public async void RemoveAsRead(string mangaId, IChapter chapter)
        {
            if (mangaId != null && chapter != null)
            {
                chapter.IsRead = !chapter.IsRead;
                if (await FileHelper.FileExistsAsync("readStatus_" + this.Name, StorageStrategies.Roaming))
                {
                    var mangasWithStatus = await FileHelper.ReadFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, StorageStrategies.Roaming);
                    if (mangasWithStatus.ContainsKey(mangaId))
                    {
                        var mangaChaptersWithStatus = mangasWithStatus[mangaId];
                        var chapterWithStatus = mangaChaptersWithStatus.FirstOrDefault(c => c == chapter.Id);
                        if (chapterWithStatus != null && chapterWithStatus.Any())
                        {
                            mangaChaptersWithStatus.Remove(chapter.Id);
                            mangasWithStatus[mangaId] = mangaChaptersWithStatus;
                        }
                    }
                    await FileHelper.WriteFileAsync<Dictionary<string, List<string>>>("readStatus_" + this.Name, mangasWithStatus, StorageStrategies.Roaming);
                }
            }
        }

        public async Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga)
        {
            throw new NotImplementedException();
        }

        public async Task<IChapter> GetChapterAsync(Chapter chapter)
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

        public async Task<ObservableCollection<IManga>> SearchMangaAsync(string query)
        {
            throw new NotImplementedException();
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
            if (_readStatus.ContainsKey(mangaId))
            {
                var chapterWithStatus = _readStatus[mangaId].FirstOrDefault(c => c == chapter.Id);
                if (chapterWithStatus != null)
                    chapter.IsRead = true;
            }
        }
    }
}
