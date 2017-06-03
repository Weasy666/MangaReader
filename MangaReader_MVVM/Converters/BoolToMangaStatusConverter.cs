using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM.Converters
{
    public class BoolToMangaStatusConverter : IValueConverter
    {
        private string ongoing = ResourceLoader.GetForViewIndependentUse().GetString("BoolToMangaStatusConverter_Value");
        private string finished = ResourceLoader.GetForViewIndependentUse().GetString("BoolToMangaStatusConverter_Otherwise");

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? ongoing : finished;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (string)value == ongoing;
        }
    }
}
