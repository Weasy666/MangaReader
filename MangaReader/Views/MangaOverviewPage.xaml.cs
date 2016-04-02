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
    public sealed partial class MangaOverviewPage : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;
        public Manga Manga { get; set; }

        public MangaOverviewPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameter = e.Parameter as Manga;
            if (parameter != null) Manga = parameter;

            base.OnNavigatedTo(e);
        }

        private void ChapterGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as Chapter;
            this.Frame.Navigate(
                typeof (ChapterPage),
                clickedItem,
                new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            StartupProgressRing.IsActive = true;
            LoadingGrid.Visibility = Visibility.Visible;

            Manga = await _rootPage.MangaManager.GetMangaInfoAsync(Manga);

            ChapterGridView.ItemsSource = null;
            ChapterGridView.ItemsSource = Manga.Chapters;

            //Category.Text = Manga.Category;
            //Status.Text = Manga.Status;
            NumberOfChapters.Text = Manga.NumberOfChapters.ToString();
            //LastUpdated.Text = Manga.LastUpdated.ToString();
            Description.Text = Manga.Description;

            StartupProgressRing.IsActive = false;
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Manga.Chapters = new ObservableCollection<Chapter>(Manga.Chapters.Reverse());
            ChapterGridView.ItemsSource = Manga.Chapters;
        }
    }
}
