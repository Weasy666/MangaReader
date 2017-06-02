using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();

        public SettingsPageViewModel()
        {
            LoadMarkDownFile();
        }

        public string MarkDownText { get; set; }

        private async void LoadMarkDownFile()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync("ToDo.md");

            MarkDownText = await FileIO.ReadTextAsync(file);
            base.RaisePropertyChanged(nameof(MarkDownText));
        }
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
            get { return _settings.MangaGridLayout.Equals(MangaItemTemplate.CoverWithDetails); }
            set { _settings.MangaGridLayout = value ? MangaItemTemplate.CoverWithDetails : MangaItemTemplate.CoverOnly; base.RaisePropertyChanged(nameof(UseDetailedMangaItem)); }
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

        public async void MangaSourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is BitmapImage image)
                {
                    var uri = image.UriSource.Segments.Last();
                    var IsParsed = Enum.TryParse<MangaSource>(uri.Substring(5).Remove(uri.Length - 5 - 4), true, out MangaSource source);

                    if (IsParsed && source != _settings.UsedMangaSource)
                    {
                        _settings.UsedMangaSource = source;
                        Views.Busy.SetBusy(true, "Picking up the freshly printed books...");
                        Services.MangaLibrary.Instance.MangaSource = source;
                        await Services.MangaLibrary.Instance.GetMangasAsync();
                        Views.Busy.SetBusy(false);
                    }
                }
            }
        }

        public int DaysOfLatestReleases
        {
            get { return _settings.DaysOfLatestReleases; }
            set { _settings.DaysOfLatestReleases = value; base.RaisePropertyChanged(nameof(DaysOfLatestReleases)); }
        }

        public int NumberOfRecentMangas
        {
            get { return _settings.NumberOfRecentMangas; }
            set { _settings.NumberOfRecentMangas = value; base.RaisePropertyChanged(nameof(NumberOfRecentMangas)); }
        }

        public bool UseOneDriveSync
        {
            get { return _settings.StorageStrategy.Equals(StorageStrategies.OneDrive); }
            set
            {
                _settings.StorageStrategy = value ? StorageStrategies.OneDrive : StorageStrategies.Local;
                base.RaisePropertyChanged(nameof(UseOneDriveSync));
                InvokeLibrarySync();
                if(!value)
                {
                    OneDriveSyncTime = new DateTime();
                }
                RaisePropertyChanged(nameof(OneDriveSyncTime));
            }
        }

        private async void InvokeLibrarySync()
        {
            IsSyncing = true;
            await Services.MangaLibrary.Instance.LoadAndMergeStoredDataAsync();
            await Services.MangaLibrary.Instance.SaveMangaStatusAsync(CreationCollisionOption.OpenIfExists);
            RaisePropertyChanged(nameof(OneDriveSyncTime));
            IsSyncing = false;
        }

        private bool _isSyncing;
        public bool IsSyncing
        {
            get => _isSyncing;
            set { Set(ref _isSyncing, value); }
        }

        public DateTime OneDriveSyncTime
        {
            get { return _settings.LastSynced; }
            set { _settings.LastSynced = value; RaisePropertyChanged(nameof(OneDriveSyncTime)); }
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

