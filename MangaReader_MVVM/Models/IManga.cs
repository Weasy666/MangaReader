using MangaReader_MVVM.Converters.JSON;
using Newtonsoft.Json;
using System;
using Template10.Controls;
using Template10.Mvvm;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public interface IManga : IBindable, IComparable<IManga>, IEquatable<IManga>
    {
        MangaSource MangaSource { get; }
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
        ObservableItemCollection<Chapter> Chapters { get; set; }
        int NumberOfChapters { get; set; }
        int ReadProgress { get; }
        bool IsFavorit { get; set; }

        void AddChapter(Chapter chapter);
        void RemoveChapter(Chapter chapter);
        ObservableItemCollection<Chapter> ReverseChapters();
    }
}
