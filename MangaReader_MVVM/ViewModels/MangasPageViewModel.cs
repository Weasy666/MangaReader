﻿using Template10.Mvvm;
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

namespace MangaReader_MVVM.ViewModels
{
    public class MangasPageViewModel : ViewModelBase
    {
        public MangasPageViewModel()
        {
            //if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
                Mangas = DesignTimeService.GenerateMangaDummies();
            }
        }

        private ObservableCollection<IManga> _mangas;
        public ObservableCollection<IManga> Mangas { get { return _mangas; } set { Set(ref _mangas, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //var repository = MangaHereRepository.Instance;
            //Mangas = await repository.GetSeriesAsync();
            
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
                        Mangas = DesignTimeService.GenerateMangaDummies(100, 100);
                    }, () => Mangas.Any());

                }

                return _reloadGridCommand;
            }
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
                        Mangas = new ObservableCollection<IManga>(Mangas.Reverse());
                    }, () => Mangas.Any());

                }

                return _sortGridCommand;
            }
        }

        private DelegateCommand _itemClickedGridCommand;
        public DelegateCommand ItemClickedGridCommand
        {
            get
            {
                if (_itemClickedGridCommand == null)
                {
                    _itemClickedGridCommand = new DelegateCommand(() =>
                    {
                        Mangas = new ObservableCollection<IManga>(Mangas.Reverse());
                    }, () => Mangas.Any());

                }

                return _itemClickedGridCommand;
            }
        }
    }
}
