using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
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

        private MangaEdenSource()
        {
            RootUri = new Uri("http://www.mangaeden.com/api/");
            MangasListPage = new Uri(RootUri, $"/list/{Language}/");
            MangaDetails = new Uri(RootUri, "/manga/{0}/");
            MangaChapterPages = new Uri(RootUri, "/chapter/{0}");
        }

        public async Task<ObservableCollection<IManga>> GetMangasAsync()
        {
            throw new NotImplementedException();
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
