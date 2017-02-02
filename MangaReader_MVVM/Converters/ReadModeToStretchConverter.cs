using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MangaReader_MVVM.Converters
{
    public class ReadModeToStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var readMode = (ReadMode)value;
            switch (readMode)
            {
                case ReadMode.HorizontalContinuous:
                    return Stretch.Uniform;
                case ReadMode.HorizontalSingle:
                    return Stretch.Uniform;
                case ReadMode.VerticalContinuous:
                    return Stretch.UniformToFill;
                case ReadMode.VerticalSingle:
                    return Stretch.Uniform;
                default:
                    return Stretch.Uniform;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
