using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings;

        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
                _sourcesWithIcons = LoadMangaSourceIcons();
            }
        }

        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(nameof(UseLightThemeButton)); }
        }

        public bool UseDetailedMangaItem
        {
            get { return _settings.MangaGridLayout.Equals("MangaItemWithDetails"); }
            set { _settings.MangaGridLayout = value ? "MangaItemWithDetails" : "MangaItemWithoutDetails"; base.RaisePropertyChanged(nameof(UseDetailedMangaItem)); }
        }

        ObservableCollection<BitmapImage> _sourcesWithIcons;
        public ObservableCollection<BitmapImage> SourcesWithIcons => _sourcesWithIcons;
        public int SelectedMangaSource => SourcesWithIcons.IndexOf(SourcesWithIcons.Where(source => source.UriSource.ToString().Contains(_settings.UsedMangaSource.ToString().ToLower())).First());
        private ObservableCollection<BitmapImage> LoadMangaSourceIcons()
        {
            var sources = Enum.GetValues(typeof(MangaSource)).Cast<MangaSource>().ToList();
            var sourcesWithIcons = new ObservableCollection<BitmapImage>();
            foreach (var source in sources)
            {
                sourcesWithIcons.Add(new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon-" + source.ToString().ToLower() + ".png")));
            }
            return sourcesWithIcons;
        }

        public int DaysOfLatestReleases
        {
            get { return _settings.DaysOfLatestReleases; }
            set { _settings.DaysOfLatestReleases = value; base.RaisePropertyChanged(nameof(DaysOfLatestReleases)); }
        }
        
        //public bool IsGroupedFavoritsGrid
        //{
        //    get { return _settings.IsGroupedFavoritsGrid; }
        //    set { _settings.IsGroupedFavoritsGrid = value; }
        //}
    }

    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public Uri RateMe => new Uri("http://aka.ms/template10");
    }
}

