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
    public class Manga : Template10.Mvvm.ViewModelBase, IManga
    {
        public IMangaSource ParentLibrary { get; internal set; }
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

        private ObservableCollection<IChapter> _chapters = new ObservableCollection<IChapter>();
        public ObservableCollection<IChapter> Chapters
        {
            get { return _chapters; }
            set { Set(ref _chapters, value); }
        }
        public int NumberOfChapters { get; set; }
        private int _readProgress;
        public int ReadProgress
        {
            get => _readProgress;
            set { Set(ref _readProgress, value); }
        }
        private bool _isFavorit = false;
        public bool IsFavorit
        {
            get { return _isFavorit; }
            set { Set(ref _isFavorit, value); }
        }

        public void AddChapter(IChapter chapter)
        {            
            if (!Chapters.Contains(chapter))
            {
                chapter.ParentManga = this;
                Chapters.Add(chapter);
            }            
        }

        public void RemoveChapter(IChapter chapter)
        {
            chapter.ParentManga = null;
            Chapters.Remove(chapter);
        }

        public ObservableCollection<IChapter> ReverseChapters() => new ObservableCollection<IChapter>(Chapters.Reverse());
        public int CompareTo(IManga other) => other == null ? 1 : Utils.CompareNatural.Compare(Title, other.Title);
        public bool Equals(IManga other) => Id == other.Id;
    }
}
