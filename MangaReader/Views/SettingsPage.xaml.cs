using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        MainPage rootPage = MainPage.Current;
        ApplicationDataContainer localSettings;

        public SettingsPage()
        {
            this.InitializeComponent();

            //NOT WORKING
            localSettings = ApplicationData.Current.LocalSettings;

            //ThemeToggle.IsOn = localSettings.Values["ThemeToggleValue"] as string == "on";

            //if (localSettings.Containers.ContainsKey("ThemeToggle"))
            //    ThemeToggle = localSettings.Values["ThemeToggle"] as ToggleSwitch;
            //else if (localSettings.Containers.ContainsKey("Settings"))
            //    localSettings = localSettings.CreateContainer("Settings", ApplicationDataCreateDisposition.Always);
        }

        private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            //var requestedTheme = rootPage.RequestedTheme;
            //if(requestedTheme == ElementTheme.Light)

            //rootPage.RequestedTheme = (rootPage.RequestedTheme == ElementTheme.Light || rootPage.RequestedTheme == ElementTheme.Default) ? ElementTheme.Dark : ElementTheme.Light;
            if (rootPage.RequestedTheme == ElementTheme.Light || rootPage.RequestedTheme == ElementTheme.Default)
            {
                rootPage.RequestedTheme = ElementTheme.Dark;
                localSettings.Values["ThemeToggle"] = "Dark";
                localSettings.Values["ThemeToggleValue"] = "on";
            }
            else
            {
                rootPage.RequestedTheme = ElementTheme.Light;
                localSettings.Values["ThemeToggle"] = "Light";
                localSettings.Values["ThemeToggleValue"] = "off";
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}