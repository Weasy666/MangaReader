using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class GridViewPage : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;
        private ObservableCollection<Manga> _mangas; 
                             
        public GridViewPage()
        {
            this.InitializeComponent();
        }

        private void MangaGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as Manga;

            this.Frame.Navigate(
                typeof(MangaOverviewPage),
                clickedItem,
                new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
        private async void AppBarReloadButton_Click(object sender, RoutedEventArgs e)
        {
            _rootPage.MangaManager.Loaded = false;
            await LoadGridContent();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGridContent();
        }

        private async Task LoadGridContent()
        {
            if (!_rootPage.MangaManager.Loaded)
            {
                LoadingGrid.Visibility = Visibility.Visible;
                StartupProgressRing.IsActive = true;

                await _rootPage.MangaManager.LoadRepositoryAsync();
                _mangas = await _rootPage.MangaManager.GetListofMangasAsync();

                MangaGridView.ItemsSource = _mangas;

                StartupProgressRing.IsActive = false;
                LoadingGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                StartupProgressRing.IsActive = false;
                LoadingGrid.Visibility = Visibility.Collapsed;

                _mangas = await _rootPage.MangaManager.GetListofMangasAsync();

                MangaGridView.ItemsSource = _mangas;
            }
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            var icon = sender as Button;
            var manga = icon.DataContext as Manga;
            manga.IsFavorit = !manga.IsFavorit;
            icon.DataContext = manga;
            icon.Content = icon.Content.ToString() == "\uE1CF" ? "\uE1CE" : "\uE1CF";
        }
    }
}
