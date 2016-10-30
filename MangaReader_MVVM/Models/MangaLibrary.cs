using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    class MangaLibrary : IMangaLibrary
    {

        public byte[] Icon { get; private set; }
        public Uri MangaIndexPage { get; private set; }
        public ObservableCollection<IManga> Mangas { get; private set; }
        public string Name { get; private set; }
        public Uri RootUri { get; private set; }
        private static MangaLibrary _instance;

        public static MangaLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MangaLibrary();
                }
                return _instance;
            }
        }



        public Task<ObservableCollection<IChapter>> GetChaptersAsync(Manga manga)
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<IManga>> GetMangasAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<IManga>> SearchMangaAsync(string query)
        {
            throw new NotImplementedException();
        }
    }
}
