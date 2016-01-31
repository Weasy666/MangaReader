using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MangaReader.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MangaReader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GridViewPage : Page
    {
        public List<Manga> Mangas = new List<Manga>
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
        public GridViewPage()
        {
            this.InitializeComponent();
        }
    }
}
