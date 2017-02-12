using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services;
using MangaReader_MVVM.Services.SettingsServices;
using MangaReader_MVVM.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MangaReader_MVVM.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private MangaLibrary _library = MangaLibrary.Instance;
        private SettingsService _settings = SettingsService.Instance;

        public MangaItemTemplate MangaGridLayout => SettingsService.Instance.MangaGridLayout;
        private void Settings_Changed(object sender, PropertyChangedEventArgs e)
        {
            //if (nameof(_settings.MangaGridLayout).Equals(e.PropertyName))
            //    base.RaisePropertyChanged(nameof(MangaGridLayout));
            //else if (nameof(_settings.NumberOfRecentMangas).Equals(e.PropertyName))
            //    base.RaisePropertyChanged(nameof(LastRead));
        }

        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {

            }
            else
            {
                _settings.PropertyChanged += Settings_Changed;
            }
        }

        public ObservableItemCollection<Manga> LastRead => _library.LastRead;

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {

            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {

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
    }
}

