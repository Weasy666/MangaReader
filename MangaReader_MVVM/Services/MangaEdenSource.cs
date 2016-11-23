using Microsoft.Toolkit.Uwp;
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
using MangaReader_MVVM.Models;

namespace MangaReader_MVVM.Services
{
    class MangaEdenSource : IMangaSource
    {
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
            return _mangas;
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

        private void MergeMangaWithDetails(IManga manga, IManga details)
        {
            manga.Alias = details.Alias;
            manga.Artist = details.Artist;
            manga.Author = details.Author;
            manga.Chapters = details.Chapters;
            manga.Description = details.Description;
            manga.NumberOfChapters = details.NumberOfChapters;
            manga.Released = details.Released;

            foreach (var chapter in manga.Chapters)
                chapter.ParentManga = manga;
        }

        //TODO private method for loading and merging favorits with existing _mangas Collection
        public async Task<ObservableCollection<IManga>> GetFavoritMangasAsync(Utils.ReloadMode mode)
        {
            if (_mangas != null && _mangas.Any() && (_favorits == null || !_favorits.Any() || mode == Utils.ReloadMode.FromSource))
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
                if (_favorits == null)
                    _favorits = new ObservableCollection<IManga>();
                _favorits.Add(favorit);
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
    }
}
