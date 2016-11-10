using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public class Page : IPage, IComparable<Page>
    {
        public int Number { get; set; }
        public BitmapImage Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int CompareTo(Page comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : this.Number.CompareTo(comparePart.Number);
        }
    }
}
