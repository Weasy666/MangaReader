using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public interface IManga : IComparable<IManga>
    {
        IMangaSource ParentLibrary { get; }
        string Title { get; set; }
        string Alias { get; set; }
        string Id { get; set; }
        BitmapImage Cover { get; set; }
        string Category { get; set; }
        string Author { get; set; }
        string Artist { get; set; }
        string Description { get; set; }
        int Hits { get; set; }
        DateTime Released { get; set; }
        DateTime LastUpdated { get; set; }
        bool Ongoing { get; set; }
        ObservableCollection<IChapter> Chapters { get; set; }
        int NumberOfChapters { get; set; }
        bool IsFavorit { get; set; }
    }
}
