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
        MainPage _rootPage = MainPage.Current;
        ApplicationDataContainer _localSettings = MainPage.LocalSettings;

        public SettingsPage()
        {
            this.InitializeComponent();

            if (_localSettings.Containers.ContainsKey("cThemeToggle"))
            {
                var composite = _localSettings.Values["cThemeToggle"] as ApplicationDataCompositeValue;
                ThemeToggle = composite["cThemeToggle"] as ToggleSwitch;
            }
            //else
            //    _localSettings.CreateContainer("ThemeToggle", ApplicationDataCreateDisposition.Always);
        }

        private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch.RequestedTheme == ElementTheme.Light || (toggleSwitch.RequestedTheme == ElementTheme.Default && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop"))
                _rootPage.RequestedTheme = ElementTheme.Dark;
            else if(toggleSwitch.RequestedTheme == ElementTheme.Dark || (toggleSwitch.RequestedTheme == ElementTheme.Default && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile"))
                _rootPage.RequestedTheme = ElementTheme.Light;
            ApplicationDataCompositeValue themeToggle = new ApplicationDataCompositeValue
            {
                ["cThemeToggle"] = toggleSwitch
            };
            _localSettings.Values["cThemeToggle"] = themeToggle;

            //rootPage.RequestedTheme = (rootPage.RequestedTheme == ElementTheme.Light || rootPage.RequestedTheme == ElementTheme.Default) ? ElementTheme.Dark : ElementTheme.Light;
            //if (_rootPage.RequestedTheme == ElementTheme.Light || _rootPage.RequestedTheme == ElementTheme.Default)
            //{
            //    _rootPage.RequestedTheme = ElementTheme.Dark;
            //    localSettings.Values["ThemeToggle"] = "Dark";
            //    localSettings.Values["ThemeToggleValue"] = "on";
            //}
            //else
            //{
            //    _rootPage.RequestedTheme = ElementTheme.Light;
            //    localSettings.Values["ThemeToggle"] = "Light";
            //    localSettings.Values["ThemeToggleValue"] = "off";
            //}
        }
    }
}