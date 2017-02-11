using System;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public interface IMangaSource : IBindable
    {
        BitmapImage Icon { get; }
        MangaSource Name { get; }       
        Uri RootUri { get; }
        Uri MangasListPage { get; }
        ObservableItemCollection<Manga> Mangas { get; }
        ObservableItemCollection<Manga> Favorits { get; }

        Task<ObservableItemCollection<Manga>> GetMangasAsync(ReloadMode mode);
        Task<Manga> GetMangaAsync(string mangaId);
        Task<Chapter> GetChapterAsync(Chapter chapter);
        //Task<ObservableItemCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode);
        Task<ObservableItemCollection<Manga>> GetLatestReleasesAsync(int numberOfPastDays, ReloadMode mode);
        void AddFavorit(ObservableItemCollection<Manga> mangas);
        void AddFavorit(Manga manga, bool isSingle = true);
        void AddAsRead(ObservableItemCollection<Chapter> chapters);
        void AddAsRead(Chapter chapter, bool isSingle = true);
        void RemoveAsRead(ObservableItemCollection<Chapter> chapters);
        void RemoveAsRead(Chapter chapter, bool isSingle = true);
        ObservableItemCollection<Manga> SearchManga(string query);
        Task<ObservableItemCollection<Chapter>> GetChaptersAsync(Manga manga);

        Task<bool> ExportMangaStatusAsync();
        Task<bool> ImportMangaStatusAsync();
    }
}
