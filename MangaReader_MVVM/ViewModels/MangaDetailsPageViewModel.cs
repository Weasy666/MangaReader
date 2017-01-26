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

namespace MangaReader_MVVM.ViewModels
{
    public class MangaDetailsPageViewModel : ViewModelBase
    {
        public MangaLibrary _library = MangaLibrary.Instance;
        public MangaDetailsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Manga = DesignTimeService.GenerateMangaDetailDummy();
                Manga.Chapters = DesignTimeService.GenerateChapterDummies();
            }
        }

        private IManga _manga = new Manga();
        public IManga Manga { get { return _manga; } set { Set(ref _manga, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var manga = parameter as Manga;
            
            if (mode == NavigationMode.New && manga != null)
            {
                Manga = await _library.GetMangaAsync(manga.Id);
                Chapters = Manga.Chapters;
            }
            if (mode == NavigationMode.Back)
            {
                //base.RaisePropertyChanged("Manga");
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

        private ObservableCollection<IChapter> _chapters = new ObservableCollection<IChapter>();
        public ObservableCollection<IChapter> Chapters { get { return _chapters; } set { Set(ref _chapters, value); } }

        private DelegateCommand _sortGridCommand;
        public DelegateCommand SortGridCommand
            => _sortGridCommand ?? (_sortGridCommand = new DelegateCommand(() =>
            {
                Chapters = new ObservableCollection<IChapter>(Chapters.Reverse());
            }, () => Manga.Chapters != null || Manga.Chapters.Any()));

        private DelegateCommand _multiSelectCommand;
        public DelegateCommand MultiSelectCommand
            => _multiSelectCommand ?? (_multiSelectCommand = new DelegateCommand(() =>
            {
                
            }, () => Manga.Chapters != null && Manga.Chapters.Count > 1));

        private DelegateCommand _favoritCommand;
        public DelegateCommand FavoritCommand
            => _favoritCommand ?? (_favoritCommand = new DelegateCommand(() =>
            {
                _library.AddFavorit(Manga);
                //base.RaisePropertyChanged("Mangas");
            }));

        private DelegateCommand _downloadCommand;
        public DelegateCommand DownloadCommand
            => _downloadCommand ?? (_downloadCommand = new DelegateCommand(() =>
            {
                //TODO implement download function for offline reading
            }));

        public async Task ChapterClickedAsync(object sender, ItemClickEventArgs e)
        {
            var clickedChapter = e.ClickedItem as Chapter;
            if (clickedChapter != null)
            {
                _library.AddAsRead(Manga.Id, clickedChapter);
                var parameters = clickedChapter.Id;
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
