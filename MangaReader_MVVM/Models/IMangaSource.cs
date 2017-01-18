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
        string Name { get; }       
        Uri RootUri { get; }
        Uri MangasListPage { get; }

        Task<ObservableCollection<IManga>> GetMangasAsync(ReloadMode mode);
        Task<ObservableCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode);
        Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays, ReloadMode mode);
        void AddFavorit(IManga manga, List<string> favorits);
        void AddFavorit(ObservableCollection<IManga> mangas);
        Task<IManga> GetMangaAsync(Manga manga);
        Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga);
        Task<IChapter> GetChapterAsync(Chapter chapter);
        Task<ObservableCollection<IManga>> SearchMangaAsync(string query);        
    }
}
