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
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            return (bool)value ? "Ongoing" : "Finished";
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            return (string)value == "Ongoing";
        }
    }
}
