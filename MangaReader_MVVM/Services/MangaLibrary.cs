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
    public class MangaLibrary
    {
        private static readonly MangaLibrary instance = new MangaLibrary();
        public static MangaLibrary Instance { get { return instance; } }
        private MangaLibrary() {}

        private IMangaSource _mangaSource = new MangaEdenSource();
        public MangaSource MangaSource
        {
            set
            {
                switch (value)
                {
                    case MangaSource.MangaEden:
                        _mangaSource = new MangaEdenSource();
                        break;
                    case MangaSource.MangaFox:
                        throw new NotImplementedException("MangaFox Source is not implemented right now");
                        break;
                    case MangaSource.MangaReader:
                        throw new NotImplementedException("MangaReader Source is not implemented right now");
                        break;
                    default:
                        break;
                }
            }
            get => _mangaSource.Name;
        }

        public Uri RootUri => _mangaSource.RootUri;
        public BitmapImage Icon => _mangaSource.Icon;
        public Task<ObservableCollection<IManga>> GetMangasAsync(ReloadMode mode = ReloadMode.Default) => _mangaSource.GetMangasAsync(mode);
        public Task<ObservableCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode = ReloadMode.Default) => _mangaSource.GetFavoritMangasAsync(mode);
        public Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays = 7, ReloadMode mode = ReloadMode.Default) => _mangaSource.GetLatestReleasesAsync(numberOfPastDays, mode);
        public void AddFavorit(IManga manga, List<string> favorits = null) => _mangaSource.AddFavorit(manga, favorits);
        public void AddFavorit(ObservableCollection<IManga> mangas) => _mangaSource.AddFavorit(mangas);
        public void AddAsRead(string mangaId, IChapter chapter) => _mangaSource.AddAsRead(mangaId, chapter);
        public void RemoveAsRead(string mangaId, IChapter chapter) => _mangaSource.RemoveAsRead(mangaId, chapter);
        public Task<IManga> GetMangaAsync(string mangaId) => _mangaSource.GetMangaAsync(mangaId);
        public Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga) => _mangaSource.GetChaptersAsync(manga);
        public Task<IChapter> GetChapterAsync(Chapter chapter) => _mangaSource.GetChapterAsync(chapter);
        public Task<ObservableCollection<IManga>> SearchMangaAsync(string query) => _mangaSource.SearchMangaAsync(query);
    }
}
