using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM.Converters
{
    public class BoolToFavoritSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "\uE1CF" : "\uE1CE";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (string)value == "\uE1CF";
        }
    }
}
