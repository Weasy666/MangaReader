﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    public interface IManga
    {
        IMangaLibrary ParentLibrary { get;  }
        string Title { get; }
        string Id { get; }
        string Cover { get; }
        string Category { get; }
        string Author { get; }
        string Artist { get; }
        string Description { get; }
        int Hits { get; }
        DateTime Released { get; }
        DateTime LastUpdated { get; }
        string Status { get; }        
        int NumberOfChapters { get; }
        bool IsFavorit { get; }
        string FavoritAsSymbol { get; }

        Task<ObservableCollection<IChapter>> GetChaptersAsynch();
    }
}
