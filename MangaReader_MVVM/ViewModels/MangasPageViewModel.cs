﻿using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using MangaScrapeLib.Repositories;

namespace MangaReader_MVVM.ViewModels
{
    public class MangasPageViewModel : ViewModelBase
    {
        public MangasPageViewModel()
        {

        }

        private MangaScrapeLib.Models.ISeries[] _mangas;
        public MangaScrapeLib.Models.ISeries[] Mangas { get { return _mangas; } set { Set(ref _mangas, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var repository = MangaHereRepository.Instance;
            Mangas = await repository.GetSeriesAsync();
            
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
    }
}