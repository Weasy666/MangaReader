using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace MangaReader.Models
{
    public class MangaManager
    {
        //Repositories of all the different Manga Sources
        private MangaEdenRepository MangaEden { get; set; }
        /// <summary>
        /// 0 = MangaEden, 1 = MangaFox, 2 = MangaReader
        /// </summary>
        public MangaSources Source { get; set; }
        public bool Loaded { get; set; }
        
        /// <summary>
        /// Load all Manga from chosen online source
        /// </summary>
        public async Task LoadRepositoryAsync()
        {
            switch (Source)
            {
                case MangaSources.MangaEden:
                    MangaEden = new MangaEdenRepository();
                    await MangaEden.LoadManga();
                    Loaded = true;
                    break;
                case MangaSources.MangaFox:
                    break;
                case MangaSources.MangaReader:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Source), Source, null);
            }
        }

        /// <summary>
        /// Load Favorit Status from RoamingSettings and add it to the given MangaList
        /// </summary>
        private static ObservableCollection<Manga> AddFavorits(ObservableCollection<Manga> mangas)
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            var roamingFavorits = roamingSettings.Containers["Favorits"].Values.ToList();

            if (roamingFavorits != null)
            {
                foreach (KeyValuePair<string, object> fav in roamingFavorits)
                {
                    var favorit = fav.Key;
                    var manga = mangas.FirstOrDefault(m => m.Title == favorit);
                    if (manga != null)
                        manga.IsFavorit = true;
                }
            }
            return mangas;
        }

        /// <summary>
        /// Get an ObservableCollection of already loaded Manga
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<Manga>> GetListofMangasAsync()
        {
            switch (Source)
            {
                case MangaSources.MangaEden:
                    ObservableCollection<Manga> ret;
                    if (MangaEden != null)
                        ret = MangaEden.GetListOfManga();
                    else
                    {
                        await LoadRepositoryAsync();
                        ret = MangaEden.GetListOfManga();
                    }
                    return AddFavorits(ret);
                default:
                    throw new ArgumentOutOfRangeException(nameof(Source), Source, null);
            }
        }

        /// <summary>
        /// Get an ObservableCollection of the latest releases of already loaded Manga
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<Manga>> GetListofLatestReleasesAsync(double numberOfPastDays = 5)
        {
            switch (Source)
            {
                case MangaSources.MangaEden:
                    ObservableCollection<Manga> ret;
                    if (MangaEden != null)
                        ret = MangaEden.GetListOfLatestReleases(numberOfPastDays);
                    else
                    {
                        await LoadRepositoryAsync();
                        ret = MangaEden.GetListOfLatestReleases(numberOfPastDays);
                    }
                    return AddFavorits(ret);
                default:
                    throw new ArgumentOutOfRangeException(nameof(Source), Source, null);
            }
        }

        /// <summary>
        /// Get the Info for a Manga
        /// </summary>
        /// <param name="manga">Manga whichs Info will be loaded</param>
        /// <returns></returns>
        public async Task<Manga> GetMangaInfoAsync(Manga manga)
        {
            switch (Source)
            {
                case MangaSources.MangaEden:
                    return await MangaEden.LoadInfosAsync(manga);
                default:
                    throw new ArgumentOutOfRangeException(nameof(Source), Source, null);
            }
        }

        /// <summary>
        /// Get the Pages for a Chapter
        /// </summary>
        /// <param name="chapter">Chapter whichs Pages will be loaded</param>
        /// <returns></returns>
        public async Task<ObservableCollection<MangaPage>> LoadPagesAsync(Chapter chapter)
        {
            switch (Source)
            {
                case MangaSources.MangaEden:
                    return await MangaEden.LoadPagesAsync(chapter);
                default:
                    throw new ArgumentOutOfRangeException(nameof(Source), Source, null);
            }
        }
    }

    public enum MangaSources
    {
        MangaEden,
        MangaFox,
        MangaReader
    }
}
