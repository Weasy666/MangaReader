﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public interface IMangaSource
    {
        BitmapImage Icon { get; }
        string Name { get; }       
        Uri RootUri { get; }
        Uri MangasListPage { get; }

        Task<ObservableCollection<IManga>> GetMangasAsync(Utils.ReloadMode mode);
        Task<ObservableCollection<IManga>> GetFavoritMangasAsync(Utils.ReloadMode mode);
        Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays, Utils.ReloadMode mode);
        void AddFavorit(IManga manga);
        void AddFavorit(ObservableCollection<IManga> mangas);
        Task<IManga> GetMangaAsync(Manga manga);
        Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga);
        Task<IChapter> GetChapterAsync(Chapter chapter);
        Task<ObservableCollection<IManga>> SearchMangaAsync(string query);        
    }
}
