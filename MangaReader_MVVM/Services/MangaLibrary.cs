using MangaReader_MVVM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                        throw new NotImplementedException("MangaFox Source is not implemented right now");
                        break;
                    case MangaSource.MangaReader:
                        throw new NotImplementedException("MangaReader Source is not implemented right now");
                        break;
                    default:
                        break;
                }
            }
            get => _mangaSource.Name;
        }

        public async Task<bool> ExportFavoritesAsync()
        {
            if (Favorits != null && Favorits.Any())
            {
                FileSavePicker savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.Downloads
                };

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("JSON", new List<string>() { ".json" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "Favorits";

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    foreach (var manga in Favorits)
                    {
                        var serializedManga = JsonConvert.SerializeObject(manga);
                        await FileIO.AppendTextAsync(file, serializedManga + "\n");
                    }

                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    return (status == FileUpdateStatus.Complete);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Uri RootUri => _mangaSource.RootUri;
        public BitmapImage Icon => _mangaSource.Icon;
        public ObservableItemCollection<Manga> Mangas => _mangaSource.Mangas;
        public ObservableItemCollection<Manga> Favorits => _mangaSource.Favorits;

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
        public Task<ObservableItemCollection<Chapter>> GetChaptersAsync(Manga manga) => _mangaSource.GetChaptersAsync(manga);
    }
}
