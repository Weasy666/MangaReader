using System;
using System.Collections.Generic;
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
        public string ID { get; set; }
        public string ImgCover { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public string Description { get; set; }
        public int Hits { get; set; }
        public string Released { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Status { get; set; }
        public List<Chapter> Chapters { get; set; }
        public int NumberOfChapters { get; set; }

        public int CompareTo(Manga comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : string.Compare(this.Title, comparePart.Title, StringComparison.Ordinal);
        }
    }
    public class Chapter
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public MangaPage[] Pages { get; set; }
        public int NumberOfPages => Pages.Length;
        public DateTime Released { get; set; }
    }
    public class MangaPage
    {
        public int Number { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
