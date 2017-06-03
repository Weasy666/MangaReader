using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services;
using MangaReader_MVVM.Services.SettingsServices;
using MangaReader_MVVM.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Template10.Utils;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MangaReader_MVVM.ViewModels
{
    public class SearchResultPageViewModel : ViewModelBase
    {
        public MangaLibrary _library = MangaLibrary.Instance;
        public SettingsService _settings = SettingsService.Instance;

        private ScrollViewer gridViewScrollViewer = null;
        public string PageHeaderText { get; set; } = "Search results for \"{0}\"";

        public MangaItemTemplate MangaGridLayout => SettingsService.Instance.MangaGridLayout;
        private void Settings_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (nameof(_settings.MangaGridLayout).Equals(e.PropertyName))
                base.RaisePropertyChanged(nameof(MangaGridLayout));
        }

        public SearchResultPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Mangas = DesignTimeService.GenerateMangaDummies();
            }
            else
            {
                _settings.PropertyChanged += Settings_Changed;
            }
        }

        private ObservableItemCollection<Manga> _mangas = new ObservableItemCollection<Manga>();
        public ObservableItemCollection<Manga> Mangas { get { return _mangas; } set { Set(ref _mangas, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var query = parameter as string;
            PageHeaderText = String.Format(PageHeaderText, query);

            Mangas = _library.SearchManga(query);

            if (suspensionState.ContainsKey("GridViewVerticalOffset") && (mode == NavigationMode.Back || mode == NavigationMode.Forward))
            {
                var test = Double.Parse(suspensionState["GridViewVerticalOffset"].ToString());
                gridViewScrollViewer.ChangeView(null, Double.Parse(suspensionState["GridViewVerticalOffset"].ToString()), null);
            }

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            suspensionState["GridViewVerticalOffset"] = gridViewScrollViewer.VerticalOffset;

            if (suspending)
            {
                suspensionState[nameof(Mangas)] = Mangas;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        private DelegateCommand<Manga> _favoritCommand;
        public DelegateCommand<Manga> FavoritCommand
            => _favoritCommand ?? (_favoritCommand = new DelegateCommand<Manga>((manga) =>
            {
                _library.AddFavorit(manga);
            }));

        public async void MangaClickedAsync(object sender, ItemClickEventArgs e)
        {
            var clickedManga = e.ClickedItem as Manga;
            if (clickedManga != null)
            {
                NavigationService.Navigate(typeof(MangaDetailsPage), clickedManga);
            }
            else
            {
                //TODO change to PopupService
                var dialog = new MessageDialog("This Manga doesn't exist");
                await dialog.ShowAsync();
            }
        }

        public void GridView_Loaded(object sender, RoutedEventArgs e)
        {
            var gridView = sender as GridView;
            gridViewScrollViewer = gridView.FirstChild<ScrollViewer>();
        }
    }
}
