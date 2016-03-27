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
    public sealed partial class GridViewPage : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;
        private ObservableCollection<Manga> mangas; 
                             
        public GridViewPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!_rootPage.MangaManager.Loaded)
            {
                LoadingGrid.Visibility = Visibility.Visible;
                ProgressCircle.IsActive = true;

                await _rootPage.MangaManager.LoadRepositoryAsync();
                var mangaList = await _rootPage.MangaManager.GetListofMangasAsync();
                mangas = new ObservableCollection<Manga>(mangaList);

                MangaGridView.ItemsSource = mangas;

                LoadingGrid.Visibility = Visibility.Collapsed;
                ProgressCircle.IsActive = false;
            }
            else
            {
                LoadingGrid.Visibility = Visibility.Collapsed;
                ProgressCircle.IsActive = false;

                var mangaList = await _rootPage.MangaManager.GetListofMangasAsync();
                mangas = new ObservableCollection<Manga>(mangaList);

                MangaGridView.ItemsSource = mangas;
            }
            
            base.OnNavigatedTo(e);
        }

        private async void MangaGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as Manga;
            clickedItem = await _rootPage.MangaManager.GetMangaInfoAsync(clickedItem);
            this.Frame.Navigate(
                typeof(MangaOverviewPage),
                clickedItem,
                new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
    }
}
