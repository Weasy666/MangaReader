using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using MangaScrapeLib.Repositories;
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
        }

        private IChapter _chapter = new Chapter();
        public IChapter Chapter { get { return _chapter; } set { Set(ref _chapter, value); } }
        public ObservableCollection<IPage> Pages => Chapter.Pages;

        //private IManga _manga;
        //public IManga Manga { get { return _manga; } set { Set(ref _manga, value); } }

        //private ObservableCollection<IChapter> _chapters;
        public ObservableCollection<IChapter> Chapters { get { return Chapter.ParentManga.Chapters; } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var chapter = parameter as Chapter;
            if (mode == NavigationMode.New && chapter != null)
            {
                Chapter = await MangaLibrary.Instance.GetChapterAsync(chapter);
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

        private DelegateCommand _sortGridCommand;
        public DelegateCommand SortGridCommand
        {
            get
            {
                if (_sortGridCommand == null)
                {
                    _sortGridCommand = new DelegateCommand(() =>
                    {
                        Chapter.Pages = new ObservableCollection<IPage>(Pages.Reverse());
                    }, () => Pages.Any());
                }
                return _sortGridCommand;
            }
        }

        public async Task ChapterClickedAsync(object sender, TappedRoutedEventArgs args)
        {
            var test = args;
            //var clickedChapter = args. .ClickedItem as Chapter;
            //if (clickedChapter != null)
            //    NavigationService.Navigate(typeof(Views.ChapterPage), clickedChapter.Id);
            //else
            //{
            //    //TODO
            //    var dialog = new MessageDialog("This Manga doesn't exist");
            //    await dialog.ShowAsync();
            //}
        }
    }
}
