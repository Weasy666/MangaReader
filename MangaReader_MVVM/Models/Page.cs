﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    public class Page : IPage, IComparable<Page>
    {
        public string Number { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int CompareTo(Page comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : CompareNatural.Compare(this.Number, comparePart.Number);
        }
    }
}
