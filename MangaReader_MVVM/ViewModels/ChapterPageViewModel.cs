using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services;
using MangaReader_MVVM.Services.SettingsServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Mvvm;
using Template10.Utils;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace MangaReader_MVVM.ViewModels
{
    public class ChapterPageViewModel : ViewModelBase
    {
        public MangaLibrary _library = MangaLibrary.Instance;
        public SettingsService _settings = SettingsService.Instance;

        public ChapterPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Chapter = DesignTimeService.GenerateChapterDummies(1).FirstOrDefault();
                Chapter.Pages = DesignTimeService.GeneratePageDummies();
                Chapter.ParentManga = DesignTimeService.GenerateMangaDetailDummy();
                Chapter.ParentManga.Chapters = DesignTimeService.GenerateChapterDummies();
            }
            else
            {
                Manga = new Manga();
                Chapter = new Chapter()
                {
                    Pages = new ObservableItemCollection<Models.Page>()
                };
            }
        }

        public int _selectedChapterIndex = -1;
        public int SelectedChapterIndex { get { return _selectedChapterIndex; } set { Set(ref _selectedChapterIndex, value); } }

        public List<string> ReadModeList => Enum.GetNames(typeof(ReadMode)).ToList();
        public ReadMode ReadMode
        {
            get { return _settings.ReadMode; }
            set { if (value >= 0) { _settings.ReadMode = value; base.RaisePropertyChanged(nameof(ReadMode)); } }
        }

        //public List<string> ReadDirectionList
        //{
        //    get
        //    {
        //        var list = new List<string>();
        //        if(ReadMode == ReadMode.HorizontalContinuous || ReadMode == ReadMode.HorizontalSingle)
        //        {
        //            list.Add(ReadDirection.LeftToRight.ToString());
        //            list.Add(ReadDirection.RightToLeft.ToString());
        //        }
        //        else if(ReadMode == ReadMode.VerticalContinuous || ReadMode == ReadMode.VerticalSingle)
        //        {
        //            list.Add("TopToBottom");
        //            list.Add("BottomToTop");
        //        }
        //        return list;
        //    }
        //}
        //public ReadDirection ReadDirection
        //{
        //    get => _settings.ReadDirection;
        //    set
        //    {
        //        _settings.ReadDirection = value;

        //        if (value == ReadDirection.LeftToRight)
        //            Pages.SortAscending((x, y) => x.Number.CompareTo(y.Number));
        //        else if(value == ReadDirection.RightToLeft)
        //            Pages.SortDescending((x, y) => x.Number.CompareTo(y.Number));

        //        base.RaisePropertyChanged(nameof(ReadDirection));
        //        base.RaisePropertyChanged(nameof(ReadDirectionList));
        //    }
        //}

        private Manga _manga;
        public Manga Manga { get { return _manga; } set { Set(ref _manga, value); } }

        private Chapter _chapter;
        public Chapter Chapter { get { return _chapter; } set { Set(ref _chapter, value); RaisePropertyChanged(nameof(Pages)); } }

        public ObservableItemCollection<Models.Page> Pages => Chapter.Pages;
        
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var chapter = parameter as Chapter;

            if (mode == NavigationMode.New && parameter != null)
            {
                Manga = chapter.ParentManga;
                
                var chapterIndex = Manga.Chapters.IndexOf(chapter);
                Chapter = await _library.GetChapterAsync(chapter);
                SelectedChapterIndex = chapterIndex;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Chapter)] = Chapter;
            }
            await Task.CompletedTask;
        }
        
        private DelegateCommand _favoritCommand;
        public DelegateCommand FavoritCommand
            => _favoritCommand ?? (_favoritCommand = new DelegateCommand(() =>
            {
                MangaLibrary.Instance.AddFavorit(Manga);
            }, () => Manga != null));

        public void Page_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var chaptersPageHeader = grid.FindName("ChaptersBar") as Template10.Controls.PageHeader;
                chaptersPageHeader.IsOpen = !chaptersPageHeader.IsOpen;

                var pageOverlayVisibility = chaptersPageHeader.IsOpen ? Visibility.Visible : Visibility.Collapsed;
                foreach (var page in Pages)
                {
                    page.OverlayVisibility = pageOverlayVisibility;
                }
            }
        }

        public void ReadMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.Parent is StackPanel stackPanel)
            {
                var flyout = stackPanel.Parent as FlyoutPresenter;
                var popup = flyout.Parent as Windows.UI.Xaml.Controls.Primitives.Popup;
                popup.IsOpen = false;
            }
        }

        //public void ReadDirection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = sender as ComboBox;
        //    if (comboBox.Parent is StackPanel stackPanel)
        //    {
        //        var flyout = stackPanel.Parent as FlyoutPresenter;
        //        var popup = flyout.Parent as Windows.UI.Xaml.Controls.Primitives.Popup;
        //        popup.IsOpen = false;
        //    }
        //}

        public async Task ChapterClickedAsync(object sender, TappedRoutedEventArgs args)
        {
            var listView = sender as ListView;

            if (listView.SelectedItem is Chapter clickedChapter)
            {
                if (clickedChapter != Chapter)
                {
                    Chapter = await MangaLibrary.Instance.GetChapterAsync(clickedChapter);
                    SelectedChapterIndex = listView.SelectedIndex;
                    MangaLibrary.Instance.AddAsRead(clickedChapter);
                }
            }
            else
            {
                //TODO change to PopupService
                var dialog = new MessageDialog("This Chapter doesn't exist");
                await dialog.ShowAsync();
            }
        }

        public void ScrollToSelectedItem(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView.SelectedItem != null)
            {
                listView.ScrollIntoView(listView.SelectedItem, ScrollIntoViewAlignment.Leading);
            }
        }
    }
}
