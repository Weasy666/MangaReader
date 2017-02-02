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
using Windows.UI.Xaml.Data;
using MangaReader_MVVM.Services.SettingsServices;
using System.ComponentModel;
using MangaReader_MVVM.Utils;

namespace MangaReader_MVVM.ViewModels
{
    public class FavoritsPageViewModel : ViewModelBase
    {
        public MangaLibrary _library = MangaLibrary.Instance;
        public SettingsService _settings = SettingsService.Instance;
        public FavoritsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Favorits = DesignTimeService.GenerateMangaDummies();
            }
            else
            {
                //Mangas = _library.GetFavoritMangasAsync().Result;
            }
            
        }

        private ObservableCollection<IManga> _favorits;
        public ObservableCollection<IManga> Favorits { get { return _favorits = _favorits ?? _library.Favorits; } set { Set(ref _favorits, value); base.RaisePropertyChanged(nameof(FavoritsGroups)); } }
        public ObservableCollection<MangaGroup> FavoritsGroups
        {
            get
            {
                var groups = new ObservableCollection<MangaGroup>();
                groups.Add(new MangaGroup() { Initial = '&' });
                groups.Add(new MangaGroup() { Initial = '#' });
                for (int i = 'A'; i <= 'Z'; i++)
                {
                    groups.Add(new MangaGroup() { Initial = (char)i });
                }
                
                var query = from manga in Favorits
                            group manga by manga.Title.ToUpper()[0] into grp
                            orderby grp.Key
                            select new { GroupName = grp.Key, Items = grp };

                foreach (var grp in query)
                {
                    MangaGroup group = groups.First(g => g.Initial == Helpers.CategorizeAlphabetically(grp.GroupName));
                    foreach (var item in grp.Items)
                    {
                        group.AddSorted(item);
                    }
                }

                return groups;
            }
        }        

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (mode == NavigationMode.New)
            {
                //Mangas = await _library.GetFavoritMangasAsync();
            }
            else
            {
                //Favorits = await _library.GetFavoritMangasAsync();
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Favorits)] = Favorits;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public bool? IsGridGrouped
        {
            get => _settings.IsGroupedFavoritsGrid;
            set { _settings.IsGroupedFavoritsGrid = (bool)value; SemanticZoomCanChangeView = (bool)value; }
        }

        public bool SemanticZoomCanChangeView
        {
            get => _settings.IsGroupedFavoritsGrid;
            set { base.RaisePropertyChanged(nameof(SemanticZoomCanChangeView)); }
        }

        private CollectionViewSource _favoritsCVS;
        public CollectionViewSource FavoritsCVS
        {
            get { return _favoritsCVS = _favoritsCVS ?? new CollectionViewSource() { IsSourceGrouped = (bool)IsGridGrouped, Source = (bool)IsGridGrouped ? (object)FavoritsGroups : (object)Favorits }; }
            set { Set(ref _favoritsCVS, value); }
        }

        private DelegateCommand _reloadGridCommand;
        public DelegateCommand ReloadGridCommand
            => _reloadGridCommand ?? (_reloadGridCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, "Picking up the freshly printed books...");
                Favorits = await _library.GetFavoritMangasAsync(ReloadMode.FromSource);
                Views.Busy.SetBusy(false);
            }, () => Favorits.Any()));
        
        private DelegateCommand<object> _groupGridCommand;
        public DelegateCommand<object> GroupGridCommand
            => _groupGridCommand ?? (_groupGridCommand = new DelegateCommand<object>((param) =>
            {
                var source = new CollectionViewSource() { IsSourceGrouped = (bool)IsGridGrouped };

                if ((bool)IsGridGrouped)
                    source.Source = FavoritsGroups;
                else
                    source.Source = Favorits;

                FavoritsCVS = source;
            }));

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
