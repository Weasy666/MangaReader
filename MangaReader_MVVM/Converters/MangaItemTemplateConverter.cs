﻿using MangaReader_MVVM.Services.SettingsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM.Converters
{
    public class MangaItemTemplateConverter : IValueConverter
    {
        public DataTemplate MangaItemWithDetailsTemplate { get; set; }
        public DataTemplate MangaItemWithoutDetailsTemplate { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var template = value as string;
            switch (template)
            {
                case "MangaItemWithDetails":
                    return MangaItemWithDetailsTemplate;
                case "MangaItemWithoutDetails":
                    return MangaItemWithoutDetailsTemplate;
                default:
                    return MangaItemWithDetailsTemplate;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
