using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM.Converters
{
    public class BoolToMangaStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "Ongoing" : "Finished";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (string)value == "Ongoing";
        }
    }
}
