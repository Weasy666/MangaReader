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

namespace MangaReader_MVVM.ViewModels
{
    public class MangaDetailsPageViewModel : ViewModelBase
    {
        public MangaDetailsPageViewModel()
        {
            //if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Manga = DesignTimeService.GenerateMangaDetailDummy();
                Chapters = DesignTimeService.GenerateChapterDummies();
            }
        }

        private IManga _manga;
        public IManga Manga { get { return _manga; } set { Set(ref _manga, value); } }

        private ObservableCollection<IChapter> _chapters;
        public ObservableCollection<IChapter> Chapters { get { return _chapters; } set { Set(ref _chapters, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //var repository = MangaHereRepository.Instance;
            //Mangas = await repository.GetSeriesAsync();
            if (mode == NavigationMode.New)
            {
                //this here is only for testing purposes
                var test = Manga as Manga;
                test.Id = parameter as string;
                test.Title = test.Title + parameter as string;
                Manga = test;
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
        {
            get
            {
                if (_sortGridCommand == null)
                {
                    _sortGridCommand = new DelegateCommand(() =>
                    {
                        Chapters = new ObservableCollection<IChapter>(Chapters.Reverse());
                    }, () => Chapters.Any());
                }
                return _sortGridCommand;
            }
        }

        public void ChapterClicked(object sender, RoutedEventArgs args)
        {
            //NavigationService.Navigate(typeof(Views.ChapterPage), 1);
        }
    }
}
