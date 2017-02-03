using System;
using Windows.UI.Xaml.Data;
using MangaReader_MVVM.Services.SettingsServices;

namespace MangaReader_MVVM.Converters
{
    public class ReadDirectionToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var readMode = (ReadDirection)value;        
            return (int)readMode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var integer = (int)value;
            return (ReadDirection)integer;
        }
    }
}
