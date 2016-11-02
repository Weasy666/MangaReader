﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public interface IPage
    {
        string Number { get; set; }
        BitmapImage Url { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }
}
