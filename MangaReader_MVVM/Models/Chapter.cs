﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MangaReader_MVVM.Models
{
    [DebuggerDisplay("{Title} | Number = {Number} | ID = {Id}")]
    public class Chapter : Template10.Mvvm.ViewModelBase, IChapter
    {
        public IManga ParentManga { get; set; }
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
        public ObservableCollection<IPage> Pages { get; set; } = new ObservableCollection<IPage>();
        public int CompareTo(IChapter other) => other == null ? 1 : Number.CompareTo(other.Number);
        public bool Equals(IChapter other) => Id == other.Id;        
    }
}
