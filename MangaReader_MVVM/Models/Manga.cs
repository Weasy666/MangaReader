using MangaReader_MVVM.Utils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using Template10.Controls;
using Template10.Mvvm;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    [DebuggerDisplay("Manga: {Title} | ID = {Id}")]
    public class Manga : BindableBase, IManga, IEquatable<Manga>
    {
        public MangaSource MangaSource { get; internal set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Id { get; set; }
        [JsonIgnore]
        public BitmapImage Cover { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        [JsonIgnore]
        public int Hits { get; set; }
        public DateTime Released { get; set; }
        [JsonIgnore]
        public DateTime LastUpdated { get; set; }
        [JsonIgnore]
        public bool Ongoing { get; set; }
        
        private ObservableItemCollection<Chapter> _chapters = new ObservableItemCollection<Chapter>();
        public ObservableItemCollection<Chapter> Chapters
        {
            get { return _chapters; }
            set { Set(ref _chapters, value); base.RaisePropertyChanged(nameof(ReadProgress)); }
        }
        public int NumberOfChapters { get; set; }
        public int ReadProgress => Chapters.Count(c => c.IsRead == true);

        private bool _isFavorit = false;
        public bool IsFavorit
        {
            get { return _isFavorit; }
            set { Set(ref _isFavorit, value); }
        }

        public void AddChapter(Chapter chapter)
        {
            chapter.ParentManga = this;

            if (!Chapters.Contains(chapter))
            {
                Chapters.AddSorted(chapter);
            }
        }

        public bool RemoveChapter(Chapter chapter)
        {
            chapter.ParentManga = null;
            return Chapters.Remove(chapter);
        }
        public ObservableItemCollection<Chapter> ReverseChapters() => new ObservableItemCollection<Chapter>(Chapters.Reverse());
        public int CompareTo(IManga other) => other == null ? 1 : Utils.CompareNatural.Compare(Title, other.Title);
        public bool Equals(Manga other) => Id.Equals(other.Id);
    }
}
