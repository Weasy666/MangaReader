using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    interface IMangaLibrary
    {
        byte[] Icon { get; }
        Uri MangaIndexPage { get; }
        string Name { get; }
        Uri RootUri { get; }
        

        Task<IManga[]> GetMangasAsync();
        Task<IManga[]> SearchMangaAsync(string query);
    }
}
