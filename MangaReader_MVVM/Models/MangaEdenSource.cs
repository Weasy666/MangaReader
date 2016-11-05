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

namespace MangaReader_MVVM.Models
{
    class MangaEdenSource : IMangaSource
    {
        private ObservableCollection<IManga> _mangas;

        public BitmapImage Icon { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon-mangaeden.png"));
        public string Name { get; } = MangaSource.MangaEden.ToString();
        private int Language { get; } = 0;
        public Uri RootUri { get; }
        public Uri MangasListPage { get; }
        public Uri MangaDetails { get; }
        public Uri MangaChapterPages { get; }

        public MangaEdenSource()
        {
            RootUri = new Uri("http://www.mangaeden.com/api/");
            MangasListPage = new Uri($"list/{Language}/", UriKind.Relative);
            MangaDetails = new Uri("manga/{0}/", UriKind.Relative);
            MangaChapterPages = new Uri("chapter/{0}", UriKind.Relative);
        }

        public async Task<ObservableCollection<IManga>> GetMangasAsync()
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
                        settings.Converters.Add(new MangaConverter());

                        var manga = JsonConvert.DeserializeObject<Manga[]>(result, settings);

                        //var test = await JsonConvert.DeserializeObjectAsync(result) as JObject;
                        //var test2 = test["manga"].First.ToList();
                    }
                }
                catch (Exception e)
                {
                    var dialog = new MessageDialog(e.Message);
                    await dialog.ShowAsync();
                }
            }
            var back = new ObservableCollection<IManga>();
            return new ObservableCollection<IManga>(back.Where(m => m.Artist == "a").ToList());
        }

        public async Task<IManga> GetMangaAsync(Manga manga)
        {
            throw new NotImplementedException();
        }

        public async Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga)
        {
            throw new NotImplementedException();
        }

        public async Task<IChapter> GetChapterAsync(Chapter chapter)
        {
            throw new NotImplementedException();
        }

        public async Task<ObservableCollection<IManga>> SearchMangaAsync(string query)
        {
            throw new NotImplementedException();
        }
    }
}
