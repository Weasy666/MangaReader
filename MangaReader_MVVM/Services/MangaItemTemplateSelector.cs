using MangaReader_MVVM.Services.SettingsServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MangaReader_MVVM.Services
{
    public class MangaItemTemplateSelector : DataTemplateSelector
    {
        SettingsService _settings;
        public DataTemplate MangaItemWithDetailsTemplate { get; set; }
        public DataTemplate MangaItemWithoutDetailsTemplate { get; set; }

        public MangaItemTemplateSelector()
        {

        }

        // http://www.wpftutorial.net/datatemplates.html
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (SettingsService.Instance.MangaGridLayout)
            {
                case MangaItemTemplate.CoverWithDetails:
                    return MangaItemWithDetailsTemplate;
                case MangaItemTemplate.CoverOnly:
                    return MangaItemWithoutDetailsTemplate;
                default:
                    return MangaItemWithDetailsTemplate;
            }
        }
    }
}
