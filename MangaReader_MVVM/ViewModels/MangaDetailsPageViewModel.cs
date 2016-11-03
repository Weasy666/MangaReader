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
                Manga.Id = parameter as string;
                Manga.Title = Manga.Title + parameter as string;
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

        public async Task ChapterClickedAsync(object sender, ItemClickEventArgs e)
        {
            var clickedChapter = e.ClickedItem as Chapter;
            if (clickedChapter != null)
                NavigationService.Navigate(typeof(Views.ChapterPage), clickedChapter.Id);
            else
            {
                //TODO
                var dialog = new MessageDialog("This Manga doesn't exist");
                await dialog.ShowAsync();
            }
        }
    }
}
