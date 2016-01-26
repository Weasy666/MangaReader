using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader.Controls
{
    class CategorySemanticView
    {
        public List<MangaCategory> Items { get; set; }

        public CategorySemanticView()
        {
            var mangas = new List<Manga>
                             {
                                 new Manga {Title = "Chio-chan no Tsuugakuro", Category = "C", Image = "http://c.mfcdn.net/store/manga/15545/cover.jpg?1453835461"},
                                 new Manga {Title = "Tsurezure Children", Category = "T", Image = "http://c.mfcdn.net/store/manga/16485/cover.jpg?1453844402"},
                                 new Manga {Title = "Inugamihime ni Kuchizuke", Category = "I", Image = "http://c.mfcdn.net/store/manga/13870/cover.jpg?1453833661"},

                                 new Manga {Title = "Keijo!!!!!!!!", Category = "K", Image = "http://c.mfcdn.net/store/manga/13435/cover.jpg?1453828202"},
                                 new Manga {Title = "Hatsukoi Zombie", Category = "H", Image = "http://c.mfcdn.net/store/manga/18279/cover.jpg?1453824722"},
                                 new Manga {Title = "Hinamatsuri", Category = "H", Image = "http://c.mfcdn.net/store/manga/11760/cover.jpg?1453824662"},

                                 new Manga {Title = "Asuka@Future.Come", Category = "A", Image = "http://c.mfcdn.net/store/manga/16213/cover.jpg?1453817461"},
                                 new Manga {Title = "Golden Kamui", Category = "G", Image = "http://c.mfcdn.net/store/manga/17215/cover.jpg?1453817402"},
                                 new Manga {Title = "Tantei Gakuen Q", Category = "T", Image = "http://c.mfcdn.net/store/manga/813/cover.jpg?1453815722"},
                             };

            var mangasByCategories = mangas.GroupBy(x => x.Category)
                .Select(x => new MangaCategory { Title = x.Key, Items = x.ToList() });

            Items = mangasByCategories.ToList();
        }
    }

    public class Manga
    {
        public string Title { get; set; }
        public List<Chapter> Chapters { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public string Source { get; set; }
    }

    public class Chapter
    {
        public int Number { get; set; }
        public string Source { get; set; }
    }

    public class MangaCategory
    {
        public string Title { get; set; }
        public List<Manga> Items { get; set; }
    }
}
