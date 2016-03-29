using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MangaReader.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MangaReader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChapterPage : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;
        private ObservableCollection<MangaPage> _pages;

        public ChapterPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameter = e.Parameter as Chapter;

            StartupProgressRing.IsActive = true;

            if (parameter != null) _pages = await _rootPage.MangaManager.LoadPagesAsync(parameter);
            pageView.ItemsSource = _pages;

            StartupProgressRing.IsActive = false;

            base.OnNavigatedTo(e);
        }
    }
}
