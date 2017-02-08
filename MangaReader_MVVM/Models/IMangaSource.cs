using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public interface IMangaSource
    {
        BitmapImage Icon { get; }
        MangaSource Name { get; }       
        Uri RootUri { get; }
        Uri MangasListPage { get; }
        ObservableCollection<IManga> Mangas { get; }
        ObservableCollection<IManga> Favorits { get; }

        Task<ObservableCollection<IManga>> GetMangasAsync(ReloadMode mode);
        Task<IManga> GetMangaAsync(string mangaId);
        Task<IChapter> GetChapterAsync(IChapter chapter);
        Task<ObservableCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode);
        Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays, ReloadMode mode);
        void AddFavorit(ObservableCollection<IManga> mangas);
        void AddFavorit(IManga manga, List<string> favorits);
        void AddAsRead(string mangaId, ObservableCollection<IChapter> chapters);
        void AddAsRead(string mangaId, IChapter chapter, bool single);
        void RemoveAsRead(string mangaId, ObservableCollection<IChapter> chapters);
        void RemoveAsRead(string mangaId, IChapter chapter, bool single);
        ObservableCollection<IManga> SearchManga(string query);
        Task<ObservableCollection<IChapter>> GetChaptersAsync(IManga manga);       
    }
}
