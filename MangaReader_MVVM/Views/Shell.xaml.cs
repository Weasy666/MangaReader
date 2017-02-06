using MangaReader_MVVM.Models;
using MangaReader_MVVM.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;

namespace MangaReader_MVVM.Views
{
    public sealed partial class Shell : Windows.UI.Xaml.Controls.Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        public ObservableCollection<IManga> Mangas => MangaLibrary.Instance.Mangas;
        private IEnumerable<IManga> _suggestions;

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
            _suggestions = Mangas.Where(p => p.Title.ToLower().Contains(sender.Text.ToLower()) && sender.Text != string.Empty);
            SearchAutoSuggestBox.ItemsSource = _suggestions;
        }

        private void SearchAllManga_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {            
            var suggestionChosen = args.ChosenSuggestion;
            if (suggestionChosen != null)
            {
                HamburgerMenu.NavigationService.Navigate(typeof(MangaDetailsPage), suggestionChosen as IManga);
            }
            else
            {
                var Results = sender.Items.ToList().Cast<IManga>().ToList();
                HamburgerMenu.NavigationService.Navigate(typeof(SearchResultPage), new List<object> { sender.Text, Results });
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
    }
}

