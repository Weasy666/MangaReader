using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM.Converters
{
    public class ReadModeToOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var readMode = (ReadMode)value;
            switch (readMode)
            {
                case ReadMode.HorizontalContinuous:
                    return Orientation.Horizontal;
                case ReadMode.HorizontalSingle:
                    return Orientation.Horizontal;
                case ReadMode.VerticalContinuous:
                    return Orientation.Vertical;
                case ReadMode.VerticalSingle:
                    return Orientation.Vertical;
                default:
                    return Orientation.Horizontal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
