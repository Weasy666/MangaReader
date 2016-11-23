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
using Windows.UI.Xaml.Input;

namespace MangaReader_MVVM.ViewModels
{
    public class ChapterPageViewModel : ViewModelBase
    {
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
                Chapter = new Chapter()
                {
                    Pages = new ObservableCollection<IPage>()
                };
            }
        }

        public int _selectedChapterIndex = -1;
        public int SelectedChapterIndex { get { return _selectedChapterIndex; } set { Set(ref _selectedChapterIndex, value); } }

        public Visibility _pageOverlayVisibility = Visibility.Collapsed;

        private IChapter _chapter;
        public IChapter Chapter { get { return _chapter; } set { Set(ref _chapter, value); } }
        //public ObservableCollection<IPage> Pages => Chapter.Pages;

        private ObservableCollection<IPage> _pages;
        public ObservableCollection<IPage> Pages
        {
            get
            {
                if (Chapter.Pages != _pages)
                    _pages = Chapter.Pages;
                return _pages;
            }
            set
            {
                Chapter.Pages = value;
                Set(ref _pages, value);
            }
        }

        //private ObservableCollection<IChapter> _chapters;
        public ObservableCollection<IChapter> Chapters { get { return Chapter.ParentManga.Chapters; } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var chapter = parameter as Chapter;
            if (mode == NavigationMode.New && chapter != null)
            {
                Pages.Clear();
                Chapter = await MangaLibrary.Instance.GetChapterAsync(chapter);
                SelectedChapterIndex = chapter.ParentManga.Chapters.IndexOf(chapter);
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

        //private DelegateCommand _sortGridCommand;
        //public DelegateCommand SortGridCommand
        //{
        //    get
        //    {
        //        if (_sortGridCommand == null)
        //        {
        //            _sortGridCommand = new DelegateCommand(() =>
        //            {
        //                Chapter.Pages = new ObservableCollection<IPage>(Pages.Reverse());
        //            }, () => Pages.Any());
        //        }
        //        return _sortGridCommand;
        //    }
        //}
        private DelegateCommand _favoritCommand;
        public DelegateCommand FavoritCommand
            => _favoritCommand ?? (_favoritCommand = new DelegateCommand(() =>
            {
                var test = MangaLibrary.Instance.GetMangasAsync();
                MangaLibrary.Instance.AddFavorit(Chapter.ParentManga);
            }, () => Chapter.ParentManga != null));

        public void Page_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var chaptersPageHeader = grid.Children[0] as Template10.Controls.PageHeader;
                chaptersPageHeader.IsOpen = !chaptersPageHeader.IsOpen;

                _pageOverlayVisibility = _pageOverlayVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                foreach (var page in Pages)
                {
                    page.OverlayVisibility = _pageOverlayVisibility;
                }
            }
        }

        public async Task ChapterClickedAsync(object sender, TappedRoutedEventArgs args)
        {
            var gridView = (sender as ScrollViewer).Content as GridView;
            var clickedChapter = gridView.SelectedItem as Chapter;

            if (clickedChapter != null)
            {
                if (clickedChapter != Chapter)
                {
                    Chapter = await MangaLibrary.Instance.GetChapterAsync(clickedChapter);
                    Pages = new ObservableCollection<IPage>(Chapter.Pages);
                    //NavigationService.Navigate(typeof(Views.ChapterPage), clickedChapter);
                }                
            }
            else
            {
                //TODO
                var dialog = new MessageDialog("This Chapter doesn't exist");
                await dialog.ShowAsync();
            }
        }
    }
}
