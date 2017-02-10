using MangaReader_MVVM.Models;
using System;
using Template10.Controls;

namespace MangaReader_MVVM.Services
{
    class DesignTimeService
    {
        public static ObservableItemCollection<Manga> GenerateMangaDummies(int number = 100, int offset = 0)
        {
            var mangaDummies = new ObservableItemCollection<Manga>();
            for (int i = 0 + offset; i < number + offset; i++)
                mangaDummies.Add(new Manga { Title = "Manga" + i,
                                             Id = i.ToString(),
                                             Cover = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/NewStoreLogo.scale-400.png")),
                                             Released = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                                             LastUpdated = DateTime.Now,
                                             Category = "SciFi"
                                           });
            return mangaDummies;
        }

        public static Manga GenerateMangaDetailDummy()
        {
            return new Manga { Title = "MangaDetail",
                               Cover = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/NewStoreLogo.scale-400.png")),
                               Released = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                               LastUpdated = DateTime.Now,
                               Category = "SciFi",
                               Description = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet."
                             };
        }

        public static ObservableItemCollection<Chapter> GenerateChapterDummies(int number = 100, int offset = 0)
        {
            var chapterDummies = new ObservableItemCollection<Chapter>();
            for (int i = 0 + offset; i < number + offset; i++)
                chapterDummies.Add(new Chapter { Title = "Chapter" + i,
                                                 Number = (i + 1),
                                                 Id = i.ToString(),
                                                 Released = DateTime.Now.Subtract(TimeSpan.FromDays(10))
                                               });
            return chapterDummies;
        }
        
        public static ObservableItemCollection<Page> GeneratePageDummies(int number = 25, int offset = 0)
        {
            var pageDummies = new ObservableItemCollection<Page>();
            for (int i = 0 + offset; i < number + offset; i++)
                pageDummies.Add(new Page { Number = i,
                                           Url = new Uri("ms-appx:///Assets/NewStoreLogo.scale-400.png")
                                         });
            return pageDummies;
        }
    }
}
