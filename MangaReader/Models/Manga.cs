using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MangaReader.Models
{
    public class Manga : IComparable<Manga>
    {
        public string Title { get; set; }
        public string Alais { get; set; }
        public string Id { get; set; }
        public string MangaCover { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public string Description { get; set; }
        public int Hits { get; set; }
        public string Released { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Status { get; set; }
        public ObservableCollection<Chapter> Chapters { get; set; }
        public int NumberOfChapters { get; set; }
        public bool IsFavorit { get; set; }

        public int CompareTo(Manga comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : string.Compare(this.Title, comparePart.Title, StringComparison.Ordinal);
        }
    }
    public class Chapter : IComparable<Chapter>
    {
        public string Number { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public ObservableCollection<MangaPage> Pages { get; set; }
        public int NumberOfPages => 0; //Pages.Count;
        public DateTime Released { get; set; }
        public string ChapterCover { get; set; }
        public bool ReadOnce { get; set; }

        public int CompareTo(Chapter comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : string.Compare(this.Number, comparePart.Number, StringComparison.Ordinal);
        }
    }
    public class MangaPage : IComparable<MangaPage>
    {
        public string Number { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int CompareTo(MangaPage comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : string.Compare(this.Number, comparePart.Number, StringComparison.Ordinal);
        }
    }
}
