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
        private ApplicationDataContainer Theme;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            
            MainPage.Current.RequestedTheme = (MainPage.Current.RequestedTheme == ElementTheme.Light || MainPage.Current.RequestedTheme == ElementTheme.Default) ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}