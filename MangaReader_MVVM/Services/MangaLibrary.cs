using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using MangaReader_MVVM.Models;

namespace MangaReader_MVVM.Services
{
    class MangaLibrary
    {
        private static readonly MangaLibrary instance = new MangaLibrary();
        public static MangaLibrary Instance { get { return instance; } }
        private MangaLibrary() {}

        private IMangaSource _mangaSource = new MangaEdenSource();
        public Utils.MangaSource MangaSource
        {
            set
            {
                switch (value)
                {
                    case Utils.MangaSource.MangaEden:
                        _mangaSource = new MangaEdenSource();
                        break;
                    case Utils.MangaSource.MangaFox:
                        throw new NotImplementedException("MangaFox Source is not implemented right now");
                        break;
                    case Utils.MangaSource.MangaReader:
                        throw new NotImplementedException("MangaReader Source is not implemented right now");
                        break;
                    default:
                        break;
                }
            }
        }

        public Uri RootUri => _mangaSource.RootUri;
        public BitmapImage Icon => _mangaSource.Icon;
        public Task<ObservableCollection<IManga>> GetMangasAsync(Utils.ReloadMode mode = Utils.ReloadMode.Default) => _mangaSource.GetMangasAsync(mode);
        public Task<ObservableCollection<IManga>> GetFavoritMangasAsync(Utils.ReloadMode mode = Utils.ReloadMode.Default) => _mangaSource.GetFavoritMangasAsync(mode);
        public Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays = 7, Utils.ReloadMode mode = Utils.ReloadMode.Default) => _mangaSource.GetLatestReleasesAsync(numberOfPastDays, mode);
        public void AddFavorit(IManga manga) { _mangaSource.AddFavorit(manga); }
        public void AddFavorit(ObservableCollection<IManga> mangas) { _mangaSource.AddFavorit(mangas); }
        public Task<IManga> GetMangaAsync(Manga manga) => _mangaSource.GetMangaAsync(manga);
        public Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga) => _mangaSource.GetChaptersAsync(manga);
        public Task<IChapter> GetChapterAsync(Chapter chapter) => _mangaSource.GetChapterAsync(chapter);
        public Task<ObservableCollection<IManga>> SearchMangaAsync(string query) => _mangaSource.SearchMangaAsync(query);
    }
}
