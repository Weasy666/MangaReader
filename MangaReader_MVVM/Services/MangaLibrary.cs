using MangaReader_MVVM.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Services
{
    public class MangaLibrary
    {
        private static readonly MangaLibrary instance = new MangaLibrary();
        public static MangaLibrary Instance { get { return instance; } }
        private MangaLibrary() {}

        private IMangaSource _mangaSource = new MangaEdenSource();
        public MangaSource MangaSource
        {
            set
            {
                switch (value)
                {
                    case MangaSource.MangaEden:
                        _mangaSource = new MangaEdenSource();
                        break;
                    case MangaSource.MangaFox:
                        _mangaSource = new MangaFoxSource();
                        break;
                    //case MangaSource.MangaReader:
                    //    throw new NotImplementedException("MangaReader Source is not implemented right now");
                    //    break;
                    default:
                        break;
                }
            }
            get => _mangaSource.Name;
        }        

        public Uri RootUri => _mangaSource.RootUri;
        public BitmapImage Icon => _mangaSource.Icon;
        public ObservableItemCollection<Manga> Mangas => _mangaSource.Mangas;
        public ObservableItemCollection<Manga> Favorits => _mangaSource.Favorits;
        public ObservableItemCollection<Manga> LastRead => _mangaSource.LastRead;
        public ObservableCollection<string> Categories => _mangaSource.Categories;

        public Task<ObservableItemCollection<Manga>> GetMangasAsync(ReloadMode mode = ReloadMode.Local) => _mangaSource.GetMangasAsync(mode);
        public Task<Manga> GetMangaAsync(string mangaId) => _mangaSource.GetMangaAsync(mangaId);
        public Task<Chapter> GetChapterAsync(Chapter chapter) => _mangaSource.GetChapterAsync(chapter);
        //public Task<ObservableItemCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode = ReloadMode.Local) => _mangaSource.GetFavoritMangasAsync(mode);
        public Task<ObservableItemCollection<Manga>> GetLatestReleasesAsync(int numberOfPastDays = 7, ReloadMode mode = ReloadMode.Local) => _mangaSource.GetLatestReleasesAsync(numberOfPastDays, mode);
        public void AddFavorit(ObservableItemCollection<Manga> mangas) => _mangaSource.AddFavorit(mangas);
        public void AddFavorit(Manga manga) => _mangaSource.AddFavorit(manga);
        public void AddAsRead(ObservableItemCollection<Chapter> chapters) => _mangaSource.AddAsRead(chapters);
        public void AddAsRead(Chapter chapter) => _mangaSource.AddAsRead(chapter);
        public void RemoveAsRead(ObservableItemCollection<Chapter> chapters) => _mangaSource.RemoveAsRead(chapters);
        public void RemoveAsRead(Chapter chapter) => _mangaSource.RemoveAsRead(chapter);
        public ObservableItemCollection<Manga> SearchManga(string query) => _mangaSource.SearchManga(query);
        public ObservableItemCollection<Manga> FilterMangaByCategory(IEnumerable filters) => _mangaSource.FilterMangaByCategory(filters);
        public Task<ObservableItemCollection<Chapter>> GetChaptersAsync(Manga manga) => _mangaSource.GetChaptersAsync(manga);

        public Task<bool> LoadAndMergeStoredDataAsync() => _mangaSource.LoadAndMergeStoredDataAsync();
        public Task<bool> SaveMangaStatusAsync(CreationCollisionOption option = CreationCollisionOption.ReplaceExisting) => _mangaSource.SaveMangaStatusAsync(option);
        public Task<bool> ExportMangaStatusAsync() => _mangaSource.ExportMangaStatusAsync();
        public Task<bool> ImportMangaStatusAsync() => _mangaSource.ImportMangaStatusAsync();
    }
}
