﻿using System;
using System.Diagnostics;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace MangaReader_MVVM.Models
{
    [DebuggerDisplay("Page: Number = {Number}")]
    public class Page : BindableBase, IPage
    {
        private Visibility _overlayVisibility = Visibility.Collapsed;
        public Visibility OverlayVisibility
        {
            get { return _overlayVisibility; }
            set { Set(ref _overlayVisibility, value); }
        }
        public int Number { get; set; }
        public Uri Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int CompareTo(IPage other) => other == null ? 1 : this.Number.CompareTo(other.Number);
    }
}
