using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM.Converters
{
    public class DateTimeToPrettyDateTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dateTime = ((DateTime)value).ToLocalTime();
            var now = DateTime.Now;

            if (dateTime.Date == now.Date)
            {
                return "Today at " + dateTime.ToString("T");
            }
            else if (dateTime.Date == now.Date.AddDays(-1))
            {
                return "Yesterday";
            }
            else
            {
                return dateTime.ToString("d. MMM yyyy");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
