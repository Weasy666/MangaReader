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
        public ObservableCollection<IChapter> _chapters { get; set; } = new ObservableCollection<IChapter>();
        public ObservableCollection<IChapter> Chapters
        {
            get
            {
                return _chapters;
            }
            set
            {
                _chapters = value;
                base.RaisePropertyChanged();
            }
        }
        public int NumberOfChapters { get; set; }
        private bool _isFavorit = false;
        public bool IsFavorit
        {
            get
            {
                return _isFavorit;
            }
            set
            {
                _isFavorit = value;
                base.RaisePropertyChanged();
            }
        }

        public void AddChapter(IChapter chapter)
        {
            chapter.ParentManga = this;
            this.Chapters.Add(chapter);
        }

        public void RemoveChapter(IChapter chapter)
        {
            this.Chapters.Remove(chapter);
            chapter.ParentManga = null;
        }

        public void ReverseChapters()
        {
            this.Chapters = new ObservableCollection<IChapter>(Chapters.Reverse());
        }

        public int CompareTo(IManga comparePart) => comparePart == null ? 1 : Utils.CompareNatural.Compare(this.Title, comparePart.Title);
    }
}
