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

namespace MangaReader_MVVM.ViewModels
{
    public class FavoritsPageViewModel : ViewModelBase
    {
        public MangaLibrary _library = MangaLibrary.Instance;
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
        public ObservableCollection<IManga> Favorits { get { return _favorits = _favorits ?? _library.Favorits; } set { Set(ref _favorits, value); } }
        public ObservableCollection<MangaGroups> FavoritsGroups
        {
            get
            {
                var groups = new ObservableCollection<MangaGroups>();

                var query = from item in Favorits
                            group item by item.Title.ToUpper()[0] into g
                            orderby g.Key
                            select new { GroupName = g.Key, Items = g };

                foreach (var g in query)
                {
                    MangaGroups info = new MangaGroups();
                    info.Initial = g.GroupName;
                    foreach (var item in g.Items)
                    {
                        info.Add(item);
                    }
                    groups.Add(info);
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

        private bool _isGridGrouped;
        public bool IsGridGrouped
        {
            get => _isGridGrouped;
            set { Set(ref _isGridGrouped, value); }
        }
        private CollectionViewSource _favoritsCVS;
        public CollectionViewSource FavoritsCVS
        {
            get { return _favoritsCVS = _favoritsCVS ?? new CollectionViewSource() { IsSourceGrouped = IsGridGrouped, Source = IsGridGrouped ? (object)FavoritsGroups : (object)Favorits }; }
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
                IsGridGrouped = !IsGridGrouped;
                var gridView = param as GridView;
                var source = new CollectionViewSource() { IsSourceGrouped = IsGridGrouped };

                if (IsGridGrouped)
                    source.Source = FavoritsGroups;
                else
                    source.Source = Favorits;

                FavoritsCVS = source;

                //gridView.InitializeViewChange();
            }));

        //private DelegateCommand _sortGridCommand;
        //public DelegateCommand SortGridCommand
        //    => _sortGridCommand ?? (_sortGridCommand = new DelegateCommand(() =>
        //    {
        //        Favorits = new ObservableCollection<IManga>(Favorits.Reverse());
        //    }, () => Favorits.Any()));

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
