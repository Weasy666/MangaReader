using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MangaReader_MVVM.Converters
{
    public class ReadModeToIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var readMode = (ReadMode)value;
            var param = parameter as string;
            if (readMode == ReadMode.HorizontalContinuous && param == "GridView")
                return true;
            else if (readMode == ReadMode.HorizontalSingle && param == "FlipView")
                return true;
            else if (readMode == ReadMode.VerticalContinuous && param == "GridView")
                return true;
            else if (readMode == ReadMode.VerticalSingle && param == "FlipView")
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
