using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MangaReader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;
        private bool _loaded;
        
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;

            if (_loaded)
            {
                if (_rootPage.RequestedTheme == ElementTheme.Light ||
                    (_rootPage.RequestedTheme == ElementTheme.Default &&
                     AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop"))
                {
                    _rootPage.RequestedTheme = ElementTheme.Dark;
                }
                else if (_rootPage.RequestedTheme == ElementTheme.Dark ||
                         (_rootPage.RequestedTheme == ElementTheme.Default &&
                          AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile"))
                {
                    _rootPage.RequestedTheme = ElementTheme.Light;
                }

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["ThemeToggle"] = toggleSwitch?.IsOn;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("ThemeToggle"))
            {
                ThemeToggle.IsOn = (bool)localSettings.Values["ThemeToggle"];
            }
            _loaded = true;
        }
    }
}