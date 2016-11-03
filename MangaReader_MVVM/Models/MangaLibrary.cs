using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    class MangaLibrary
    {
        public static readonly MangaLibrary instance = new MangaLibrary();
        public static MangaLibrary Instance { get; }
        private MangaLibrary() {}

        public IMangaSource _mangaSource = new MangaEdenSource();
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
        }

        public byte[] Icon => _mangaSource.Icon;
        public Task<ObservableCollection<IManga>> GetMangasAsync() => _mangaSource.GetMangasAsync();
        public Task<IManga> GetMangaAsync(Manga manga) => _mangaSource.GetMangaAsync(manga);
        public Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga) => _mangaSource.GetChaptersAsync(manga);
        public Task<IChapter> GetChapterAsync(Chapter chapter) => _mangaSource.GetChapterAsync(chapter);
        public Task<ObservableCollection<IManga>> SearchMangaAsync(string query) => _mangaSource.SearchMangaAsync(query);
    }
}
