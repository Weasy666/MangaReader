using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM.Converters
{
    public class ReadModeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var readMode = (ReadMode)value;
            return (int)readMode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var integer = (int)value;
            return (ReadMode)integer;
        }
    }
}
