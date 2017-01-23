﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public interface IPage : IComparable<IPage>
    {
        Visibility OverlayVisibility { get; set; }
        int Number { get; set; }
        Uri Url { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }
}
