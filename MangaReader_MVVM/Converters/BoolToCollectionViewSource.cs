using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using MangaReader_MVVM.Models;

namespace MangaReader_MVVM.Converters
{
    public class BoolToCollectionViewSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
