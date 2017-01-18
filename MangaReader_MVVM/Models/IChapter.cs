﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MangaReader_MVVM.Models
{
    public interface IChapter : IComparable<Chapter>
    {
        IManga ParentManga { get; set; }
        int Number { get; set; }
        string Title { get; set; }
        string Id { get; set; }
        DateTime Released { get; set; }
        bool IsRead { get; set; }
        ObservableCollection<IPage> Pages { get; set; }
    }
}
