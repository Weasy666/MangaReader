using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace MangaReader_MVVM.Services.SettingsServices
{
    public class SettingsService : Template10.Mvvm.ViewModelBase
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
                Views.Shell.HamburgerMenu.RefreshStyles(value);
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

        public string MangaGridLayout
        {
            get { return _helper.Read<string>(nameof(MangaGridLayout), "MangaItemWithDetails"); }
            set
            {
                _helper.Write(nameof(MangaGridLayout), value);
                TestSelector = new MangaItemTemplateSelector()
                {
                    MangaItemWithDetailsTemplate = TestSelector.MangaItemWithDetailsTemplate,
                    MangaItemWithoutDetailsTemplate = TestSelector.MangaItemWithoutDetailsTemplate
                };
            }
        }

        private MangaItemTemplateSelector _testSelector = new MangaItemTemplateSelector()
        {
            MangaItemWithDetailsTemplate = XamlUtils.GetResource<DataTemplate>("MangaItemWithDetailsTemplate", null),
            MangaItemWithoutDetailsTemplate = XamlUtils.GetResource<DataTemplate>("MangaItemWithoutDetailsTemplate", null)
        };
        public MangaItemTemplateSelector TestSelector
        {
            get => _testSelector;
            set { _testSelector = value; base.RaisePropertyChanged(); }
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

        public int DaysOfLatestReleases
        {
            get { return _helper.Read<int>(nameof(DaysOfLatestReleases), 7); }
            set { _helper.Write(nameof(DaysOfLatestReleases), value); }
        }
    }
}

