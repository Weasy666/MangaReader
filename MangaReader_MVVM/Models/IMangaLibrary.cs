using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    public interface IMangaLibrary
    {
        byte[] Icon { get; }
        Uri MangaIndexPage { get; }
        string Name { get; }
        Uri RootUri { get; }
        ObservableCollection<IManga> Mangas { get; }

        Task<ObservableCollection<IManga>> GetMangasAsync();
        Task<ObservableCollection<IManga>> SearchMangaAsync(string query);
        Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga);
    }
}
