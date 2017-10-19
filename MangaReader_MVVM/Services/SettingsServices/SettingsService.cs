using System;
using System.Globalization;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Utils;
using Windows.Globalization;
using Windows.UI.Xaml;

namespace MangaReader_MVVM.Services.SettingsServices
{
    public class SettingsService : Template10.Mvvm.BindableBase
    {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                //Views.Shell.HamburgerMenu.RefreshStyles(value.ToElementTheme());
            }
        }

        public CultureInfo AppLanguage
        {
            get
            {
                var culture = CultureInfo.CurrentCulture;
                var value = _helper.Read<string>(nameof(AppLanguage), culture.Name);
                return new CultureInfo(value);
            }
            set
            {
                _helper.Write(nameof(AppLanguage), value.Name);
                ApplicationLanguages.PrimaryLanguageOverride = value.Name;
                Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();
                Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public MangaItemTemplate MangaGridLayout
        {
            get
            {
                var template = MangaItemTemplate.CoverWithDetails;
                var value = _helper.Read<string>(nameof(MangaGridLayout), template.ToString());
                return Enum.TryParse<MangaItemTemplate>(value, out template) ? template : MangaItemTemplate.CoverWithDetails;
            }
            set { _helper.Write(nameof(MangaGridLayout), value.ToString()); base.RaisePropertyChanged(nameof(MangaGridLayout)); }
        }

        public MangaSource UsedMangaSource
        {
            get
            {
                var source = MangaSource.MangaEden;
                var value = _helper.Read<string>(nameof(UsedMangaSource), source.ToString());
                return Enum.TryParse<MangaSource>(value, out source) ? source : MangaSource.MangaEden;
            }
            set
            {
                _helper.Write(nameof(UsedMangaSource), value.ToString());
                MangaLibrary.Instance.MangaSource = value;
            }
        }

        public StorageStrategies StorageStrategy
        {
            get
            {
                var source = StorageStrategies.Local;
                var value = _helper.Read<string>(nameof(StorageStrategy), source.ToString());
                return Enum.TryParse<StorageStrategies>(value, out source) ? source : StorageStrategies.Local;
            }
            set { _helper.Write(nameof(StorageStrategy), value.ToString()); base.RaisePropertyChanged(nameof(StorageStrategy)); }
        }

        public DateTime LastSynced
        {
            get { return _helper.Read<DateTime>(nameof(LastSynced), new DateTime()); }
            set { _helper.Write(nameof(LastSynced), value); }
        }

        public int DaysOfLatestReleases
        {
            get { return _helper.Read<int>(nameof(DaysOfLatestReleases), 7); }
            set { _helper.Write(nameof(DaysOfLatestReleases), value); }
        }

        public bool IsGroupedFavoritsGrid
        {
            get { return _helper.Read<bool>(nameof(IsGroupedFavoritsGrid), false); }
            set { _helper.Write(nameof(IsGroupedFavoritsGrid), value); }
        }

        public ReadMode ReadMode
        {
            get
            {
                var readMode = ReadMode.HorizontalContinuous;
                var value = _helper.Read<string>(nameof(ReadMode), readMode.ToString());
                return Enum.TryParse<ReadMode>(value, out readMode) ? readMode : ReadMode.HorizontalContinuous;
            }
            set { _helper.Write(nameof(ReadMode), value.ToString()); base.RaisePropertyChanged(nameof(ReadMode)); }
        }

        public ReadDirection ReadDirection
        {
            get
            {
                var readDirection = ReadDirection.LeftToRight;
                var value = _helper.Read<string>(nameof(ReadDirection), readDirection.ToString());
                return Enum.TryParse<ReadDirection>(value, out readDirection) ? readDirection : ReadDirection.LeftToRight;
            }
            set { _helper.Write(nameof(ReadDirection), value.ToString()); base.RaisePropertyChanged(nameof(ReadDirection)); }
        }

        public int NumberOfRecentMangas
        {
            get { return _helper.Read<int>(nameof(NumberOfRecentMangas), 8); }
            set { _helper.Write(nameof(NumberOfRecentMangas), value); base.RaisePropertyChanged(nameof(NumberOfRecentMangas)); }
        }
    }
}

