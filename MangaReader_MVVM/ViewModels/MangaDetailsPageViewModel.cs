using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MangaReader_MVVM.ViewModels
{
    public class MangaDetailsPageViewModel : ViewModelBase
    {
        private bool viewModelInitialised;
        public MangaLibrary _library = MangaLibrary.Instance;


        public MangaDetailsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Manga = DesignTimeService.GenerateMangaDetailDummy();
                Manga.Chapters = DesignTimeService.GenerateChapterDummies();
            }
            else
            {
                
            }
        }

        private Manga _manga = new Manga();
        public Manga Manga { get { return _manga; } set { Set(ref _manga, value); } }

        private ObservableItemCollection<Chapter> _chapters = new ObservableItemCollection<Chapter>();
        public ObservableItemCollection<Chapter> Chapters { get { return _chapters; } set { Set(ref _chapters, value); } }

        public int ReadProgress => Manga.ReadProgress;

        public void PageGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            var page = grid.Parent as Windows.UI.Xaml.Controls.Page;
            var group = page.FindName("AdaptiveVisualStateGroup") as VisualStateGroup;
            var state = group.States.First(x => x.Name == "VisualStateNormal");
            var normalStateTrigger = state.StateTriggers.First() as AdaptiveTrigger;
            state = group.States.First(x => x.Name == "VisualStateWide");
            var wideStateTrigger = state.StateTriggers.First() as AdaptiveTrigger;

            if (page.ActualWidth < normalStateTrigger.MinWindowWidth)
            {
                VisualStateManager.GoToState(page, "VisualStateNarrow", false);
            }
            else if (page.ActualWidth < wideStateTrigger.MinWindowWidth)
            {
                VisualStateManager.GoToState(page, "VisualStateNormal", false);
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var manga = parameter as Manga;
            
            if ((mode == NavigationMode.New || mode == NavigationMode.Refresh || viewModelInitialised == false) && manga != null)
            {
                Manga = (suspensionState.ContainsKey(nameof(Manga))) ? suspensionState[nameof(Manga)] as Manga : await _library.GetMangaAsync(manga.Id);
                Chapters = Manga.Chapters;

                viewModelInitialised = true;
            }

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Manga)] = Manga;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        private DelegateCommand _sortGridCommand;
        public DelegateCommand SortGridCommand
            => _sortGridCommand ?? (_sortGridCommand = new DelegateCommand(() =>
            {
                Chapters = new ObservableItemCollection<Chapter>(Chapters.Reverse());
            }, () => Manga.Chapters != null || Manga.Chapters.Any()));

        private bool _isMultiSelect;
        public bool IsMultiSelect
        {
            get { return _isMultiSelect; }
            set { Set(ref _isMultiSelect, value); }
        }

        private DelegateCommand<object> _multiSelectCommand;
        public DelegateCommand<object> MultiSelectCommand
            => _multiSelectCommand ?? (_multiSelectCommand = new DelegateCommand<object>((param) =>
            {
                var chapterGridView = param as GridView;

                chapterGridView.SelectionMode = chapterGridView.SelectionMode == ListViewSelectionMode.None ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;
                IsMultiSelect = !IsMultiSelect;
                chapterGridView.IsItemClickEnabled = !chapterGridView.IsItemClickEnabled;
            }, (param) => Manga.Chapters.Any() && Manga.Chapters.Count > 1));

        private DelegateCommand _favoritCommand;
        public DelegateCommand FavoritCommand
            => _favoritCommand ?? (_favoritCommand = new DelegateCommand(() =>
            {
                _library.AddFavorit(Manga);
            }));

        private DelegateCommand _downloadCommand;
        public DelegateCommand DownloadCommand
            => _downloadCommand ?? (_downloadCommand = new DelegateCommand(() =>
            {
                //TODO implement download function for offline reading
            }));

        private bool? _multiSelectButtonIsChecked = false;
        public bool? MultiSelectButtonIsChecked
        {
            get { return _multiSelectButtonIsChecked; }
            set { Set(ref _multiSelectButtonIsChecked, value); }
        }

        private DelegateCommand<object> _selectAllCommand;
        public DelegateCommand<object> SelectAllCommand
            => _selectAllCommand ?? (_selectAllCommand = new DelegateCommand<object>((param) =>
            {
                var chapterGridView = param as GridView;
                chapterGridView.SelectAll();
            }));

        private DelegateCommand<object> _markAsUnReadCommand;
        public DelegateCommand<object> MarkAsUnReadCommand
            => _markAsUnReadCommand ?? (_markAsUnReadCommand = new DelegateCommand<object>((param) =>
            {
                var chapterGridView = param as GridView;
                var chapters = new ObservableItemCollection<Chapter>(chapterGridView.SelectedItems.Cast<Chapter>());
                _library.RemoveAsRead(chapters);

                MultiSelectButtonIsChecked = false;
                chapterGridView.SelectionMode = ListViewSelectionMode.None;
                IsMultiSelect = false;
                chapterGridView.IsItemClickEnabled = true;
                base.RaisePropertyChanged(nameof(ReadProgress));
            }));

        private DelegateCommand<object> _markAsReadCommand;
        public DelegateCommand<object> MarkAsReadCommand
            => _markAsReadCommand ?? (_markAsReadCommand = new DelegateCommand<object>((param) =>
            {
                var chapterGridView = param as GridView;
                var chapters = new ObservableItemCollection<Chapter>(chapterGridView.SelectedItems.Cast<Chapter>());
                _library.AddAsRead(chapters);

                MultiSelectButtonIsChecked = false;
                chapterGridView.SelectionMode = ListViewSelectionMode.None;
                IsMultiSelect = false;
                chapterGridView.IsItemClickEnabled = true;
                base.RaisePropertyChanged(nameof(ReadProgress));
            }));

        private DelegateCommand<object> _cancelCommand;
        public DelegateCommand<object> CancelCommand
            => _cancelCommand ?? (_cancelCommand = new DelegateCommand<object>((param) =>
            {
                var chapterGridView = param as GridView;

                MultiSelectButtonIsChecked = false;
                chapterGridView.SelectionMode = ListViewSelectionMode.None;
                IsMultiSelect = false;
                chapterGridView.IsItemClickEnabled = true;
            }));

        public async Task ChapterClickedAsync(object sender, ItemClickEventArgs e)
        {
            var clickedChapter = e.ClickedItem as Chapter;
            if (clickedChapter != null)
            {
                _library.AddAsRead(clickedChapter);                
                NavigationService.Navigate(typeof(Views.ChapterPage), clickedChapter);
            }
            else
            {
                //TODO change to PopupService
                var dialog = new MessageDialog("This Chapter doesn't exist");
                await dialog.ShowAsync();
            }
        }
    }
}
