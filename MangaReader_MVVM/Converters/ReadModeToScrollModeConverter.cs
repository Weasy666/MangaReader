using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MangaReader_MVVM.Converters
{
    public class ReadModeToScrollModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var readMode = (ReadMode)value;
            var param = parameter as string;
            if (readMode == ReadMode.HorizontalContinuous && param == "Horizontal")
                return ScrollMode.Auto;
            else if (readMode == ReadMode.VerticalContinuous && param == "Vertical")
                return ScrollMode.Auto;
            else
                return ScrollMode.Disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
