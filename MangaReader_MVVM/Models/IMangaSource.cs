using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Windows.Storage;
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
        ObservableItemCollection<Manga> LastRead { get; }
        ObservableCollection<string> Categories { get; }

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
        ObservableItemCollection<Manga> FilterMangaByCategory(IEnumerable filters);
        Task<ObservableItemCollection<Chapter>> GetChaptersAsync(Manga manga);

        Task<bool> LoadAndMergeStoredDataAsync();
        Task<bool> SaveMangaStatusAsync(CreationCollisionOption option);
        Task<bool> ExportMangaStatusAsync();
        Task<bool> ImportMangaStatusAsync();
    }
}
