﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MangaReader_MVVM.Models
{
    public class Chapter : IChapter, IComparable<Chapter>
    {
        public IManga ParentManga { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }        
        public int NumberOfPages => 0; //Pages.Count;
        public DateTime Released { get; set; }
        public bool IsRead { get; set; }
        public ObservableCollection<IPage> Pages { get; set; }

        public int CompareTo(Chapter comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : this.Number.CompareTo(comparePart.Number);
        }
    }
}
