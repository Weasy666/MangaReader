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
        public Manga Manga { get; set; }
        private ObservableCollection<MangaPage> _pages;
        private Collection<Grid> children;

        public ChapterPage()
        {
            this.InitializeComponent();
            children = new Collection<Grid>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameter = e.Parameter as List<object>;
            var chapter = parameter?[1] as Chapter;
            Manga = parameter?[0] as Manga;

            ChapterView.ItemsSource = Manga.Chapters;

            StartupProgressRing.IsActive = true;

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
            
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            var grid = image.Parent as Grid;
            var progressRing = grid.Children[1] as ProgressRing;
            progressRing.IsActive = false;

            children.Add(grid);
        }

        private async void ScrollViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var textBlock = e.OriginalSource as TextBlock;
            var chapter = textBlock.DataContext as Chapter;

            StartupProgressRing.IsActive = true;

            if (chapter != null) _pages = await _rootPage.MangaManager.LoadPagesAsync(chapter);
            PageView.ItemsSource = _pages;

            StartupProgressRing.IsActive = false;
        }

        private void TopBar_Opening(object sender, object e)
        {
            foreach (var c in children)
            {
                c.Children[2].Visibility = Visibility.Visible;
            }
        }

        private void TopBar_Closing(object sender, object e)
        {
            foreach (var c in children)
            {
                c.Children[2].Visibility = Visibility.Collapsed;
            }
        }
    }
}
