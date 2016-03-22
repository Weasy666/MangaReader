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
            LoadingGrid.Visibility = Visibility.Visible;
            ProgressCircle.IsActive = true;

            await _rootPage.MangaManager.LoadRepository();
            var test = await _rootPage.MangaManager.GetListofMangas();
            mangas = new ObservableCollection<Manga>(test);

            MangaGridView.DataContext = mangas;

            LoadingGrid.Visibility = Visibility.Collapsed;
            ProgressCircle.IsActive = false;

            base.OnNavigatedTo(e);
        }

        private void MangaGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(
                typeof(MangaOverviewPage),
                e.ClickedItem,
                new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
    }
}
