using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    public interface IMangaSource
    {
        byte[] Icon { get; }
        string Name { get; }       
        Uri RootUri { get; }
        Uri MangaIndexPage { get; }

        Task<ObservableCollection<IManga>> GetMangasAsync();
        Task<IManga> GetMangaAsync(Manga manga);
        Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga);
        Task<IChapter> GetChapterAsync(Chapter chapter);
        Task<ObservableCollection<IManga>> SearchMangaAsync(string query);        
    }
}
