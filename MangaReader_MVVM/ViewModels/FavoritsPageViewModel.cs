using MangaReader_MVVM.Models;
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
using Template10.Utils;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace MangaReader_MVVM.ViewModels
{
    public class FavoritsPageViewModel : ViewModelBase
    {
        private MangaLibrary _library = MangaLibrary.Instance;
        private SettingsService _settings = SettingsService.Instance;

        private ScrollViewer zoomedInGridViewScrollViewer = null;
        private ScrollViewer zoomedOutGridViewScrollViewer = null;

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
                //FavoritsGroups = new GroupedObservableCollection<char, Manga>(m => m.Title[0], Favorits);
            }
        }

        public ObservableItemCollection<Manga> Favorits => _library.Favorits;
        public ObservableItemCollection<MangaGroup> FavoritsGroups
        {
            get
            {
                var groups = new ObservableItemCollection<MangaGroup> { new MangaGroup() { Key = '&'.ToString() },
                                                                        new MangaGroup() { Key = '#'.ToString() } };

                for (int i = 'A'; i <= 'Z'; i++)
                {
                    groups.Add(new MangaGroup() { Key = ((char)i).ToString() });
                }

                var query = from manga in Favorits
                            group manga by manga.Title.ToUpper()[0] into grp
                            orderby grp.Key
                            select new { GroupName = grp.Key, Items = grp };

                foreach (var grp in query)
                {
                    MangaGroup group = groups.First(g => g.Key == Helpers.CategorizeAlphabetically(grp.GroupName).ToString());
                    foreach (var item in grp.Items)
                    {
                        group.AddSorted(item);
                    }
                }

                return groups;
            }
        }

        //public GroupedObservableCollection<string, Manga> FavoritsGroups { get; internal set; }

        private void Favorits_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            //FavoritsGroups.ReplaceWith(new GroupedObservableCollection<string, Manga>(c => c.Title[0].ToString(), Favorits), new GenericCompare<Manga>(m => m.Id));
            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    var added = e.NewItems;

            //    if (FavoritsCVS.IsSourceGrouped)
            //    {
            //        foreach (Manga toAdd in added)
            //        {
            //            (FavoritsCVS.View.CollectionGroups.Where(g => (g as MangaGroup).Key == toAdd.Title.ToUpper()[0]).First() as MangaGroup).AddSorted(toAdd);
            //        }
            //    }
            //}
            //if (e.Action == NotifyCollectionChangedAction.Remove)
            //{
            //    var removed = e.OldItems;

            //    if (FavoritsCVS.IsSourceGrouped)
            //    {
            //        foreach (Manga toRemove in removed)
            //        {
            //            (FavoritsCVS.View.CollectionGroups.Where(g => (g as MangaGroup).Key == toRemove.Title.ToUpper()[0]).First() as MangaGroup).Remove(toRemove);
            //        }
            //    }
            //}
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (mode == NavigationMode.Back || mode == NavigationMode.Forward)
            {
                if (suspensionState.ContainsKey("ZoomedInGridViewVerticalOffset"))
                {
                    zoomedInGridViewScrollViewer.ChangeView(null, Double.Parse(suspensionState["ZoomedInGridViewVerticalOffset"].ToString()), null);
                }
                if (suspensionState.ContainsKey("ZoomedOutGridViewVerticalOffset"))
                {
                    zoomedOutGridViewScrollViewer.ChangeView(null, Double.Parse(suspensionState["ZoomedOutGridViewVerticalOffset"].ToString()), null);
                }
            }

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            suspensionState["ZoomedInGridViewVerticalOffset"] = zoomedInGridViewScrollViewer.VerticalOffset;
            suspensionState["ZoomedOutGridViewVerticalOffset"] = zoomedOutGridViewScrollViewer.VerticalOffset;

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

        //private DelegateCommand _exportCommand;
        //public DelegateCommand ExportCommand
        //    => _exportCommand ?? (_exportCommand = new DelegateCommand(async () =>
        //    {
        //        await _library.ExportMangaStatusAsync();
        //    }, () => Favorits.Any()));

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

        public void ZoomedInGridView_Loaded(object sender, RoutedEventArgs e)
        {
            var gridView = sender as GridView;
            zoomedInGridViewScrollViewer = gridView.FirstChild<ScrollViewer>();
        }

        public void ZoomedOutGridView_Loaded(object sender, RoutedEventArgs e)
        {
            var gridView = sender as GridView;
            zoomedOutGridViewScrollViewer = gridView.FirstChild<ScrollViewer>();
        }
    }
}
