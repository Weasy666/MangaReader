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
        private ObservableCollection<Manga> _mangas; 
                             
        public GridViewPage()
        {
            this.InitializeComponent();
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

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_rootPage.MangaManager.Loaded)
            {
                StartupProgressRing.IsActive = true;

                await _rootPage.MangaManager.LoadRepositoryAsync();
                var mangaList = await _rootPage.MangaManager.GetListofMangasAsync();
                _mangas = new ObservableCollection<Manga>(mangaList);

                MangaGridView.ItemsSource = _mangas;
                
                StartupProgressRing.IsActive = false;
            }
            else
            {
                StartupProgressRing.IsActive = false;

                var mangaList = await _rootPage.MangaManager.GetListofMangasAsync();
                _mangas = new ObservableCollection<Manga>(mangaList);

                MangaGridView.ItemsSource = _mangas;
            }
        }
    }
}
