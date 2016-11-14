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

namespace MangaReader_MVVM.ViewModels
{
    public class MangaDetailsPageViewModel : ViewModelBase
    {
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
                Manga = new Manga();
                Manga.Chapters = new ObservableCollection<IChapter>();
            }
        }

        private IManga _manga;
        public IManga Manga { get { return _manga; } set { Set(ref _manga, value); } }
        //public ObservableCollection<IChapter> Chapters => Manga.Chapters;

        private ObservableCollection<IChapter> _chapters;
        public ObservableCollection<IChapter> Chapters
        {
            get
            {
                if (Manga.Chapters != _chapters)
                    _chapters = Manga.Chapters;
                return _chapters;
            }
            set
            {
                Manga.Chapters = value;
                Set(ref _chapters, value);
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var manga = parameter as Manga;
            if (mode == NavigationMode.New && manga != null)
            {
                Manga = await  MangaLibrary.Instance.GetMangaAsync(manga);
                Chapters = Manga.Chapters;
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

        private DelegateCommand _sortGridCommand;
        public DelegateCommand SortGridCommand
            => _sortGridCommand ?? (_sortGridCommand = new DelegateCommand(() =>
            {
                Chapters = new ObservableCollection<IChapter>(Chapters.Reverse());
            }, () => Chapters != null || Chapters.Any()));

        public async Task ChapterClickedAsync(object sender, ItemClickEventArgs e)
        {
            var clickedChapter = e.ClickedItem as Chapter;
            if (clickedChapter != null)
                NavigationService.Navigate(typeof(Views.ChapterPage), clickedChapter);
            else
            {
                //TODO
                var dialog = new MessageDialog("This Chapter doesn't exist");
                await dialog.ShowAsync();
            }
        }
    }
}
