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
        Task<IManga> GetMangaAsync(Manga manga);
        Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga);
        Task<IChapter> GetChapterAsync(Chapter chapter);
        Task<ObservableCollection<IManga>> SearchMangaAsync(string query);        
    }
}
