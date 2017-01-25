﻿using Template10.Mvvm;
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
        public MangaLibrary _library = MangaLibrary.Instance;
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
        public ObservableCollection<IChapter> Chapters { get { return Manga.Chapters; } }

        private IManga _manga;
        public IManga Manga { get { return _manga; } set { Set(ref _manga, value); } }

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
                    //NavigationService.Navigate(typeof(Views.ChapterPage), clickedChapter);
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
