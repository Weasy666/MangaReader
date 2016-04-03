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
        private Manga Mangas { get; set; }
        private ObservableCollection<MangaPage> _pages;

        public ChapterPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameter = e.Parameter as List<object>;
            var chapter = parameter?[1] as Chapter;
            Mangas = parameter?[0] as Manga;

            StartupProgressRing.IsActive = true;
            _pages = null;

            if (chapter != null) _pages = await _rootPage.MangaManager.LoadPagesAsync(chapter);
            PageView.ItemsSource = _pages;

            StartupProgressRing.IsActive = false;

            base.OnNavigatedTo(e);
        }

        private void Page_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            TopBar.IsOpen = !TopBar.IsOpen;
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
