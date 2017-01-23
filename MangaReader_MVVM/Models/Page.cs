using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    public class Page : Template10.Mvvm.ViewModelBase, IPage
    {
        private Visibility _overlayVisibility = Visibility.Collapsed;
        public Visibility OverlayVisibility
        {
            get { return _overlayVisibility; }
            set { Set(ref _overlayVisibility, value); }
        }
        public int Number { get; set; }
        public Uri Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int CompareTo(IPage comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : this.Number.CompareTo(comparePart.Number);
        }
    }
}
