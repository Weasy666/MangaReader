using Newtonsoft.Json;
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
using Template10.Services.SettingsService;
using MangaReader_MVVM.Models;

namespace MangaReader_MVVM.Services
{
    class MangaEdenSource : IMangaSource
    {
        private static readonly ISettingsHelper _helper = new SettingsHelper();

        private ObservableCollection<IManga> _mangas;
        private ObservableCollection<IManga> _favorits;

        public BitmapImage Icon { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon-mangaeden.png"));
        public string Name { get; } = Utils.MangaSource.MangaEden.ToString();
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

        public async Task<ObservableCollection<IManga>> GetMangasAsync(Utils.ReloadMode mode)
        {
            if (_mangas == null || !_mangas.Any() || mode == Utils.ReloadMode.FromSource)
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

                            _mangas = JsonConvert.DeserializeObject<ObservableCollection<IManga>>(result, settings);
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

            return _mangas;
        }

        public async Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays, Utils.ReloadMode mode)
        {
            if (_mangas == null || !_mangas.Any() || mode == Utils.ReloadMode.FromSource)
            {
                await GetMangasAsync(Utils.ReloadMode.Default);
            }
            var latestReleases = _mangas.Where(manga => manga.LastUpdated.AddDays(numberOfPastDays) >= DateTime.Today).ToList();
            latestReleases.Sort((y, x) => y == null ? 1 : DateTime.Compare(x.LastUpdated, y.LastUpdated));

            return new ObservableCollection<IManga>(latestReleases);
        }

        public async Task<IManga> GetMangaAsync(Manga manga)
        {
            using (var httpClient = new HttpClient { BaseAddress = RootUri })
            {
                try
                {
                    var mangaDetailsUri = new Uri(RootUri, MangaDetails);
                    var response = await httpClient.GetAsync(new Uri(mangaDetailsUri, manga.Id));
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.Converters.Add(new MangaEdenMangaDetailsConverter());
                        
                        var details = JsonConvert.DeserializeObject<IManga>(result, settings);

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

        //TODO somehow the added chapters are not linked to the manga in the _mangas list
        private void MergeMangaWithDetails(IManga manga, IManga details)
        {
            manga.Alias = details.Alias;
            manga.Artist = details.Artist;
            manga.Author = details.Author;
            foreach (var chapter in details.Chapters)
            {
                manga.AddChapter(chapter);
            }
            manga.Description = details.Description;
            manga.NumberOfChapters = details.NumberOfChapters;
            manga.Released = details.Released;
        }

        //TODO private method for loading and merging favorits with existing _mangas Collection
        public async Task<ObservableCollection<IManga>> GetFavoritMangasAsync(Utils.ReloadMode mode)
        {
            if (_mangas == null || !_mangas.Any() || mode == Utils.ReloadMode.FromSource)
            {
                await GetMangasAsync(Utils.ReloadMode.Default);
                _favorits = new ObservableCollection<IManga>(_mangas.Where(manga => manga.IsFavorit).ToList());
            }

            if (_favorits == null || !_favorits.Any())
            {
                _favorits = new ObservableCollection<IManga>(_mangas.Where(manga => manga.IsFavorit).ToList());
            }

            return _favorits;
        }

        public async void AddFavorit(IManga favorit)
        {
            if (favorit != null)
            {
                favorit.IsFavorit = !favorit.IsFavorit;
                var favorits = _helper.Read("favorits_" + this.Name, new List<string>(), SettingsStrategies.Roam);
                if (favorits.Any() && !favorit.IsFavorit)
                {
                    favorits.Remove(favorit.Id);                    
                }
                else
                {
                    favorits.Add(favorit.Id);
                }
                _helper.Write("favorits_" + this.Name, favorits, SettingsStrategies.Roam);
            }
        }

        public async void AddFavorit(ObservableCollection<IManga> newFavorits)
        {
            if (newFavorits != null && newFavorits.Any())
            {
                foreach (var fav in newFavorits)
                {
                    AddFavorit(fav);
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

        private void LoadAndMergeFavorits()
        {
            var favorits = _helper.Read("favorits_" + this.Name, new List<string>(), SettingsStrategies.Roam);
            foreach (var id in favorits)
            {
                var manga = _mangas.FirstOrDefault(m => m.Id == id);
                if (manga != null)
                    manga.IsFavorit = true;
            }
        }
    }
}
