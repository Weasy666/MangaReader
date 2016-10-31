using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public interface IManga
    {
        IMangaLibrary ParentLibrary { get;  }
        string Title { get; }
        string Id { get; }
        BitmapImage Cover { get; }
        string Category { get; }
        string Author { get; }
        string Artist { get; }
        string Description { get; }
        int Hits { get; }
        DateTime Released { get; }
        DateTime LastUpdated { get; }
        bool Ongoing { get; }        
        int NumberOfChapters { get; }
        bool IsFavorit { get; }
        string FavoritAsSymbol { get; }

        Task<ObservableCollection<IChapter>> GetChaptersAsynch();
    }
}
