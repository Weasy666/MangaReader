using MangaReader_MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Services
{
    class DesignTimeService
    {
        public static ObservableCollection<IManga> GenerateMangaDummies(int number = 100, int offset = 0)
        {
            var mangaDummies = new ObservableCollection<IManga>();
            for (int i = 0 + offset; i < number + offset; i++)
                mangaDummies.Add(new Manga { Title = "Manga" + i, Cover = @"Assets\NewStoreLogo.scale-400.png", Released = DateTime.Now.Subtract(TimeSpan.FromDays(10)), LastUpdated = DateTime.Now, Category = "SciFi" });
            return mangaDummies;
        }
    }
}
