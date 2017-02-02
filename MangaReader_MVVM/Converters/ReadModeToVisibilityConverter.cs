using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM.Converters
{
    public class ReadModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var readMode = (ReadMode)value;
            var param = parameter as string;
            if (readMode == ReadMode.HorizontalContinuous && param == "GridView")
                return Visibility.Visible;
            else if (readMode == ReadMode.HorizontalSingle && param == "FlipView")
                return Visibility.Visible;
            else if (readMode == ReadMode.VerticalContinuous && param == "GridView")
                return Visibility.Visible;
            else if (readMode == ReadMode.VerticalSingle && param == "FlipView")
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
