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

namespace MangaReader_MVVM.ViewModels
{
    public class MangasPageViewModel : ViewModelBase
    {
        public MangasPageViewModel()
        {
            //if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Mangas = DesignTimeValues.GenerateMangaDummies();
            }
        }

        private ObservableCollection<IManga> _mangas;
        public ObservableCollection<IManga> Mangas { get { return _mangas; } set { Set(ref _mangas, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //var repository = MangaHereRepository.Instance;
            //Mangas = await repository.GetSeriesAsync();
            
            //Mangas = (suspensionState.ContainsKey(nameof(Mangas))) ? suspensionState[nameof(Mangas)] : new ObservableCollection<Models.Manga>(parameter);
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Mangas)] = Mangas;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        private DelegateCommand _reloadGridCommand;

        public DelegateCommand ReloadGridCommand
        {
            get
            {
                if (_reloadGridCommand == null)
                {
                    _reloadGridCommand = new DelegateCommand(() =>
                    {
                        Mangas = DesignTimeValues.GenerateMangaDummies(100, 100);
                    }, () => Mangas.Any());

                }

                return _reloadGridCommand;

            }
        }
    }

    public static class DesignTimeValues
    {
        public static ObservableCollection<IManga> GenerateMangaDummies(int number = 100, int offset = 0)
        {
            var mangaDummies = new ObservableCollection<IManga>();
            for (int i = 0 + offset; i < number + offset; i++)
                mangaDummies.Add(new Manga { Title = "Manga" + i, Cover = @"Assets\NewStoreLogo.scale-400.png", Released = DateTime.Now.Subtract(TimeSpan.FromDays(10)), LastUpdated = DateTime.Now, Category = "SciFi" });
            return mangaDummies;
        }
    }
}
