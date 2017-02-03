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
using MangaReader_MVVM.Services.SettingsServices;
using Windows.UI.Xaml.Media;
using MangaReader_MVVM.Utils;
using System.Collections;

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
                Manga = new Manga()
                {
                    
                };
                Chapter = new Chapter()
                {
                    Pages = new ObservableCollection<IPage>()
                };
            }
        }

        public int _selectedChapterIndex = -1;
        public int SelectedChapterIndex { get { return _selectedChapterIndex; } set { Set(ref _selectedChapterIndex, value); } }

        public List<string> ReadModeList => Enum.GetNames(typeof(ReadMode)).ToList();
        public ReadMode ReadMode
        {
            get => _settings.ReadMode;
            set { _settings.ReadMode = value; base.RaisePropertyChanged(nameof(ReadMode)); }
        }

        public List<string> ReadDirectionList
        {
            get
            {
                var list = new List<string>();
                if(ReadMode == ReadMode.HorizontalContinuous || ReadMode == ReadMode.HorizontalSingle)
                {
                    list.Add(ReadDirection.LeftToRight.ToString());
                    list.Add(ReadDirection.RightToLeft.ToString());
                }
                else if(ReadMode == ReadMode.VerticalContinuous || ReadMode == ReadMode.VerticalSingle)
                {
                    list.Add(ReadDirection.TopToBottom.ToString());
                    list.Add(ReadDirection.BottomToTop.ToString());
                }
                return list;
            }
        }
        public ReadDirection ReadDirection
        {
            get => _settings.ReadDirection;
            set
            {
                _settings.ReadDirection = value;

                if (value == ReadDirection.LeftToRight || value == ReadDirection.TopToBottom)
                    Pages.SortAscending((x, y) => x.Number.CompareTo(y.Number));
                else if(value == ReadDirection.RightToLeft || value == ReadDirection.BottomToTop)
                    Pages.SortDescending((x, y) => x.Number.CompareTo(y.Number));

                base.RaisePropertyChanged(nameof(ReadDirection));
                base.RaisePropertyChanged(nameof(ReadDirectionList));
            }
        }

        private IManga _manga;
        public IManga Manga { get { return _manga; } set { Set(ref _manga, value); } }

        public ObservableCollection<IChapter> Chapters => Manga.Chapters;
        private IChapter _chapter;
        public IChapter Chapter { get { return _chapter; } set { Set(ref _chapter, value); } }
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
        
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var chapter = parameter as Chapter;

            if (mode == NavigationMode.New && parameter != null)
            {
                Pages.Clear();
                Manga = await _library.GetMangaAsync(chapter.ParentManga.Id);
                
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
            if(comboBox.Name == "ReadMode")
            {
                if((ReadMode == ReadMode.HorizontalContinuous || ReadMode == ReadMode.HorizontalSingle) &&
                   (ReadDirection != ReadDirection.LeftToRight || ReadDirection != ReadDirection.RightToLeft))
                {
                    ReadDirection = ReadDirection.LeftToRight;
                }
                else if((ReadMode == ReadMode.VerticalContinuous || ReadMode == ReadMode.VerticalSingle) &&
                        (ReadDirection != ReadDirection.TopToBottom || ReadDirection != ReadDirection.BottomToTop))
                {
                    ReadDirection = ReadDirection.TopToBottom;
                }
            }
        }

        public async Task ChapterClickedAsync(object sender, TappedRoutedEventArgs args)
        {
            var listView = sender as ListView;
            var clickedChapter = listView.SelectedItem as Chapter;

            if (clickedChapter != null)
            {
                if (clickedChapter != Chapter)
                {
                    Chapter = await MangaLibrary.Instance.GetChapterAsync(clickedChapter);
                    SelectedChapterIndex = listView.SelectedIndex;
                    MangaLibrary.Instance.AddAsRead(clickedChapter.ParentManga.Id, clickedChapter);
                    Pages = new ObservableCollection<IPage>(Chapter.Pages);
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
