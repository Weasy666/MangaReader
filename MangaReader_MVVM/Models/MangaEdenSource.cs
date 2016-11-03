using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    class MangaEdenSource : IMangaSource
    {
        private ObservableCollection<IManga> _mangas;

        public Byte[] Icon { get; } = ;

        public Uri MangaIndexPage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public String Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Uri RootUri
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<IChapter> GetChapterAsync(Chapter chapter)
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga)
        {
            throw new NotImplementedException();
        }

        public Task<IManga> GetMangaAsync(Manga manga)
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<IManga>> GetMangasAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<IManga>> SearchMangaAsync(String query)
        {
            throw new NotImplementedException();
        }
    }
}
