using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MangaReader_MVVM.Services
{
    public class MangaItemTemplateSelector : DataTemplateSelector
    {
        Services.SettingsServices.SettingsService _settings;
        public DataTemplate MangaItemWithDetailsTemplate { get; set; }
        public DataTemplate MangaItemWithoutDetailsTemplate { get; set; }

        public MangaItemTemplateSelector()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
            }
        }

        // http://www.wpftutorial.net/datatemplates.html
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (_settings.MangaGridLayout)
            {
                case "MangaItemWithDetails":
                    return MangaItemWithDetailsTemplate;
                case "MangaItemWithoutDetails":
                    return MangaItemWithoutDetailsTemplate;
                default:
                    return base.SelectTemplateCore(item, container);
            }
        }
    }
}
