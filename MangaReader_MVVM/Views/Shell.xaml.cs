using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services;
using Microsoft.Toolkit.Uwp.Services.OneDrive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace MangaReader_MVVM.Views
{
    public sealed partial class Shell : Windows.UI.Xaml.Controls.Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        private MangaLibrary _library => MangaLibrary.Instance;
        public ObservableItemCollection<Manga> Mangas => _library.Mangas;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }

        private void SearchAllManga_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchAutoSuggestBox.ItemsSource = _library.SearchManga(sender.Text);
        }

        private void SearchAllManga_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {            
            var suggestionChosen = args.ChosenSuggestion;
            if (suggestionChosen != null)
            {
                HamburgerMenu.NavigationService.Navigate(typeof(MangaDetailsPage), suggestionChosen as Manga);
            }
            else
            {
                HamburgerMenu.NavigationService.Navigate(typeof(SearchResultPage), sender.Text);
            }
            sender.Text = "";
        }
        
        private void MyHamburgerMenu_IsOpenChanged(object sender, Template10.Common.ChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                SearchAutoSuggestBoxIcon.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SearchAutoSuggestBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                SearchAutoSuggestBoxIcon.Visibility = Windows.UI.Xaml.Visibility.Visible;
                SearchAutoSuggestBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void MyHamburgerMenu_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (HamburgerMenu.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                SearchAutoSuggestBoxIcon.Visibility = Windows.UI.Xaml.Visibility.Visible;
                SearchAutoSuggestBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }            
        }

        private void SearchAllManga_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = "";
        }

        private void SearchAutoSuggestBox_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            (sender as AutoSuggestBox).Text = "";
        }

        private async void DataBackup_Tapped(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Create or Load a Backup of your Manga Status. \nWhen you load a Backup your actual status will be overwriten. ");
            dialog.Title = "Manga Status Backup";
            dialog.Commands.Add(new UICommand { Label = "Save", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Load", Id = 1 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            var isSuccess = false;

            if ((int)result.Id == 0)
            {
                isSuccess = await _library.ExportMangaStatusAsync();
                dialog.Commands.Clear();
                dialog.Title = "";
                dialog.Content = isSuccess ? "Successfully created the Backup file." : "An error occured while creating the Backup file, or you canceled the operation.";
                await dialog.ShowAsync();
            }
            else if ((int)result.Id == 1)
            {
                isSuccess = await _library.ImportMangaStatusAsync();
                dialog.Commands.Clear();
                dialog.Title = "";
                dialog.Content = isSuccess ? "Successfully loaded the Backup file." : "An error occured while loading the Backup file, or you canceled the operation.";
                await dialog.ShowAsync();
            }
        }
    }
}

