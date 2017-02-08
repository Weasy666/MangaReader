using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using MangaReader_MVVM.Models;
using Windows.Storage.Pickers;
using Windows.Storage;
using Newtonsoft.Json;
using Windows.Storage.Provider;
using System.Linq;

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
        public ObservableCollection<IManga> Mangas => _mangaSource.Mangas;
        public ObservableCollection<IManga> Favorits => _mangaSource.Favorits;

        public Task<ObservableCollection<IManga>> GetMangasAsync(ReloadMode mode = ReloadMode.Local) => _mangaSource.GetMangasAsync(mode);
        public Task<IManga> GetMangaAsync(string mangaId) => _mangaSource.GetMangaAsync(mangaId);
        public Task<IChapter> GetChapterAsync(Chapter chapter) => _mangaSource.GetChapterAsync(chapter);
        public Task<ObservableCollection<IManga>> GetFavoritMangasAsync(ReloadMode mode = ReloadMode.Local) => _mangaSource.GetFavoritMangasAsync(mode);
        public Task<ObservableCollection<IManga>> GetLatestReleasesAsync(int numberOfPastDays = 7, ReloadMode mode = ReloadMode.Local) => _mangaSource.GetLatestReleasesAsync(numberOfPastDays, mode);
        public void AddFavorit(ObservableCollection<IManga> mangas) => _mangaSource.AddFavorit(mangas);
        public void AddFavorit(IManga manga, List<string> favorits = null) => _mangaSource.AddFavorit(manga, favorits);
        public void AddAsRead(string mangaId, ObservableCollection<IChapter> chapters) => _mangaSource.AddAsRead(mangaId, chapters);
        public void AddAsRead(string mangaId, IChapter chapter) => _mangaSource.AddAsRead(mangaId, chapter, true);
        public void RemoveAsRead(string mangaId, ObservableCollection<IChapter> chapters) => _mangaSource.RemoveAsRead(mangaId, chapters);
        public void RemoveAsRead(string mangaId, IChapter chapter) => _mangaSource.RemoveAsRead(mangaId, chapter, true);
        public ObservableCollection<IManga> SearchManga(string query) => _mangaSource.SearchManga(query);
        public Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga) => _mangaSource.GetChaptersAsync(manga);
    }
}
