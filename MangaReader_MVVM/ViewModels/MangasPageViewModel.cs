using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using MangaReader_MVVM.Services.SettingsServices;
using System.ComponentModel;

namespace MangaReader_MVVM.ViewModels
{
    public class MangasPageViewModel : ViewModelBase
    {
        public MangaLibrary _library = MangaLibrary.Instance;
        public SettingsService _settings = SettingsService.Instance;

        public MangaItemTemplate MangaGridLayout => SettingsService.Instance.MangaGridLayout;
        private void Settings_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (nameof(_settings.MangaGridLayout).Equals(e.PropertyName))
                base.RaisePropertyChanged(nameof(MangaGridLayout));
        }

        public MangasPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Mangas = DesignTimeService.GenerateMangaDummies();
            }
            else
            {
                Mangas = _library.Mangas;
                _settings.PropertyChanged += Settings_Changed;
            }
        }

        private ObservableCollection<IManga> _mangas = new ObservableCollection<IManga>();
        public ObservableCollection<IManga> Mangas { get { return _mangas; } set { Set(ref _mangas, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (mode == NavigationMode.New)
            {

            }
            else
            {
                
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
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

        private DelegateCommand _reloadGridCommand;
        public DelegateCommand ReloadGridCommand
            => _reloadGridCommand ?? (_reloadGridCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, "Picking up the freshly printed books...");
                Mangas = await _library.GetMangasAsync(ReloadMode.Server);
                Views.Busy.SetBusy(false);
            }, () => Mangas.Any()));

        private DelegateCommand _sortGridCommand;
        public DelegateCommand SortGridCommand
            => _sortGridCommand ?? (_sortGridCommand = new DelegateCommand(() =>
            {
                Mangas = new ObservableCollection<IManga>(Mangas.Reverse());
            }, () => Mangas.Any()));

        private DelegateCommand<IManga> _favoritCommand;
        public DelegateCommand<IManga> FavoritCommand
            => _favoritCommand ?? (_favoritCommand = new DelegateCommand<IManga>((manga) =>
            {
                _library.AddFavorit(manga);
            }));

        public async void MangaClickedAsync(object sender, ItemClickEventArgs e)
        {
            var clickedManga = e.ClickedItem as Manga;
            if (clickedManga != null)
            {
                NavigationService.Navigate(typeof(Views.MangaDetailsPage), clickedManga);
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
