using System;
using System.Collections.Generic;
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
        

        public MangaManager()
        {

        }

        /// <summary>
        /// Load Mangas from chosen Source
        /// </summary>
        public async Task LoadRepository()
        {
            switch (Source)
            {
                case MangaSources.MangaEden:
                    MangaEden = new MangaEdenRepository();
                    await MangaEden.LoadManga();
                    break;
                case MangaSources.MangaFox:
                    break;
                case MangaSources.MangaReader:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Source), Source, null);
            }
        }

        public async Task<List<Manga>> GetListofMangas()
        {
            switch (Source)
            {
                case MangaSources.MangaEden:
                    List<Manga> ret;
                    if (MangaEden != null)
                        ret = MangaEden.GetManga();
                    else
                    {
                        await LoadRepository();
                        ret = MangaEden.GetManga();
                    }
                    return ret;
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
