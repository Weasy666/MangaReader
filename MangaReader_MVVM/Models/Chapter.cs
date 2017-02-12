using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Template10.Controls;
using Template10.Mvvm;

namespace MangaReader_MVVM.Models
{
    [DebuggerDisplay("Chapter: {Title} | Number = {Number} | ID = {Id}")]
    public class Chapter : BindableBase, IChapter, IEquatable<IChapter>
    {
        public Manga ParentManga { get; set; }
        public float Number { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }        
        public int NumberOfPages => Pages?.Count ?? 0;
        public DateTime Released { get; set; }
        private bool _isRead = false;
        public bool IsRead
        {
            get { return _isRead; }
            set { Set(ref _isRead, value); }
        }

        public ObservableItemCollection<Page> Pages { get; set; } = new ObservableItemCollection<Page>();
        public int CompareTo(IChapter other) => other == null ? 1 : Number.CompareTo(other.Number);
        public bool Equals(IChapter other) => Id.Equals(other.Id);
    }
}
