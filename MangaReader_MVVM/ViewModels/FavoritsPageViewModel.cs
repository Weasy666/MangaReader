﻿using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services;
using MangaReader_MVVM.Services.SettingsServices;
using MangaReader_MVVM.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace MangaReader_MVVM.ViewModels
{
    public class FavoritsPageViewModel : ViewModelBase
    {
        private MangaLibrary _library = MangaLibrary.Instance;
        private SettingsService _settings = SettingsService.Instance;

        public MangaItemTemplate MangaGridLayout => _settings.MangaGridLayout;
        private void Settings_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (nameof(_settings.MangaGridLayout).Equals(e.PropertyName))
                base.RaisePropertyChanged(nameof(MangaGridLayout));
        }

        public FavoritsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                //Favorits = DesignTimeService.GenerateMangaDummies();
            }
            else
            {
                _settings.PropertyChanged += Settings_Changed;
                Favorits.CollectionChanged += Favorits_Changed;
            }
        }

        public ObservableItemCollection<Manga> Favorits => _library.Favorits; 
        public ObservableItemCollection<MangaGroup> FavoritsGroups
        {
            get
            {
                var groups = new ObservableItemCollection<MangaGroup> { new MangaGroup() { Initial = '&' },
                                                                        new MangaGroup() { Initial = '#' } };

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
        private void Favorits_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
              
        }

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

        private DelegateCommand _exportCommand;
        public DelegateCommand ExportCommand
            => _exportCommand ?? (_exportCommand = new DelegateCommand(async () =>
            {
                await _library.ExportFavoritesAsync();
            }, () => Favorits.Any()));

        private DelegateCommand _reloadGridCommand;
        public DelegateCommand ReloadGridCommand
            => _reloadGridCommand ?? (_reloadGridCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, "Picking up the freshly printed books...");
                await _library.GetMangasAsync(ReloadMode.Server);
                //base.RaisePropertyChanged(nameof(Favorits));
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
