using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        

        public MangaManager()
        {

        }

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
        /// Get a ObservableCollection of already loaded Manga
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
                    return ret;
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
