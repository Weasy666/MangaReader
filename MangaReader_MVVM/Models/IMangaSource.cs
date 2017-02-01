using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        Task<ObservableCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode);
        Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays, ReloadMode mode);
        void AddFavorit(IManga manga, List<string> favorits);
        void AddFavorit(ObservableCollection<IManga> mangas);
        void AddAsRead(string mangaId, IChapter chapter, bool single);
        void AddAsRead(string mangaId, ObservableCollection<IChapter> chapters);
        void RemoveAsRead(string mangaId, IChapter chapter, bool single);
        void RemoveAsRead(string mangaId, ObservableCollection<IChapter> chapters);
        Task<IManga> GetMangaAsync(string mangaId);
        Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga);
        Task<IChapter> GetChapterAsync(Chapter chapter);
        Task<ObservableCollection<IManga>> SearchMangaAsync(string query);        
    }
}
