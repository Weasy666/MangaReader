using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public class Manga : IManga, IComparable<Manga>
    {
        public IMangaLibrary ParentLibrary { get; internal set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Id { get; set; }
        public BitmapImage Cover { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public string Description { get; set; }
        public int Hits { get; set; }
        public DateTime Released { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool Ongoing { get; set; }
        public int NumberOfChapters { get; set; }
        public bool IsFavorit { get; set; }
        public string FavoritAsSymbol => IsFavorit ? "\uE1CF" : "\uE1CE";

        public Task<ObservableCollection<IChapter>> GetChaptersAsynch()
        {
            return ParentLibrary.GetChaptersAsync(this);
        }

        public int CompareTo(Manga comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : CompareNatural.Compare(this.Title, comparePart.Title);
        }
    }
}
