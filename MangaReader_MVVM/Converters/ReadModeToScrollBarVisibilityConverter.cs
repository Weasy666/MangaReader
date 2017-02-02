using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MangaReader_MVVM.Converters
{
    public class ReadModeToScrollBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var readMode = (ReadMode)value;
            var param = parameter as string;
            if (readMode == ReadMode.HorizontalContinuous && param == "Horizontal")
                return ScrollBarVisibility.Auto;
            else if (readMode == ReadMode.VerticalContinuous && param == "Vertical")
                return ScrollBarVisibility.Auto;
            else
                return ScrollBarVisibility.Disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
