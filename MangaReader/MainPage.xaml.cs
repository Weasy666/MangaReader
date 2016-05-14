using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml.Automation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MangaReader.Controls;
using MangaReader.Models;
using MangaReader.Views;

namespace MangaReader
{
    /// <summary>
    /// The "chrome" layer of the app that provides top-level navigation with
    /// proper keyboarding navigation.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Declare the top level nav items
        #region <private readonly List<NavMenuItem> _navlist = new List<NavMenuItem>;>
        private readonly List<NavMenuItem> _navlist = new List<NavMenuItem>(
            new[]
            {
                new NavMenuItem()
                {
                    Arguments = "Latest Releases",
                    Symbol = Symbol.Sync,
                    Label = "Latest Releases",
                    DestPage = typeof(GridViewPage)
                },
                new NavMenuItem()
                {
                    Arguments = "Favorite",
                    Symbol = Symbol.Favorite,
                    Label = "Favorite",
                    DestPage = typeof(CommandBarPage)
                },
                new NavMenuItem()
                {
                    Arguments = "Manga Library",
                    Symbol = Symbol.Library,
                    Label = "Manga Library",
                    DestPage = typeof(SemanticPage)
                },
            });
        #endregion
        #region <private readonly List<NavMenuItem> _setlist = new List<NavMenuItem>;>
        private readonly List<NavMenuItem> _setlist = new List<NavMenuItem>(
            new[]
            {
                new NavMenuItem()
                {
                    Arguments = "Settings",
                    Symbol = Symbol.Setting,
                    Label = "Settings",
                    DestPage = typeof(SettingsPage)
                },
            });
        #endregion
        public static MainPage Current;
        public  MangaManager MangaManager { get; set; }
        private static ObservableCollection<Manga> _mangas;
        public ObservableCollection<Manga> Mangas
        {
            get
            {
                if (MangaManager.Loaded)
                {
                    return _mangas ?? (_mangas = MangaManager.GetListofMangasAsync().Result);
                }
                else
                    return new ObservableCollection<Manga>();
            }
            set { _mangas = value; }
        }
        private IEnumerable<Manga> _suggestions;

        /// <summary>
        /// Initializes a new instance of the AppShell, sets the static 'Current' reference,
        /// adds callbacks for Back requests and changes in the SplitView's DisplayMode, and
        /// provide the nav menu list with the data to display.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            
            this.Loaded += (sender, args) =>
            {
                // This is a static public property that allows downstream pages to get a handle to the MainPage instance
                // in order to call methods that are in this class.
                Current = this;

                this.TogglePaneButton.Focus(FocusState.Programmatic);
            };

            this.RootSplitView.RegisterPropertyChangedCallback(
                SplitView.DisplayModeProperty,
                (s, a) =>
                {
                    // Ensure that we update the reported size of the TogglePaneButton when the SplitView's
                    // DisplayMode changes.
                    this.CheckTogglePaneButtonSizeChanged();
                });

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

            NavMenuList.ItemsSource = _navlist;
            SettingsMenuList.ItemsSource = _setlist;

            MangaManager = new MangaManager {Source = 0};
            LoadManga();

        }

         private async void LoadManga()
         {
            await MangaManager.LoadRepositoryAsync();
         }

        public Frame AppFrame => this.RootFrame;

        /// <summary>
        /// Default keyboard focus movement for any unhandled keyboarding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            FocusNavigationDirection direction = FocusNavigationDirection.None;
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Left:
                case Windows.System.VirtualKey.GamepadDPadLeft:
                case Windows.System.VirtualKey.GamepadLeftThumbstickLeft:
                case Windows.System.VirtualKey.NavigationLeft:
                    direction = FocusNavigationDirection.Left;
                    break;
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.GamepadDPadRight:
                case Windows.System.VirtualKey.GamepadLeftThumbstickRight:
                case Windows.System.VirtualKey.NavigationRight:
                    direction = FocusNavigationDirection.Right;
                    break;

                case Windows.System.VirtualKey.Up:
                case Windows.System.VirtualKey.GamepadDPadUp:
                case Windows.System.VirtualKey.GamepadLeftThumbstickUp:
                case Windows.System.VirtualKey.NavigationUp:
                    direction = FocusNavigationDirection.Up;
                    break;

                case Windows.System.VirtualKey.Down:
                case Windows.System.VirtualKey.GamepadDPadDown:
                case Windows.System.VirtualKey.GamepadLeftThumbstickDown:
                case Windows.System.VirtualKey.NavigationDown:
                    direction = FocusNavigationDirection.Down;
                    break;
            }

            if (direction != FocusNavigationDirection.None)
            {
                var control = FocusManager.FindNextFocusableElement(direction) as Control;
                if (control != null)
                {
                    control.Focus(FocusState.Programmatic);
                    e.Handled = true;
                }
            }
        }

        #region BackRequested Handlers

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled && this.AppFrame.CanGoBack)
            {
                e.Handled = true;
                this.AppFrame.GoBack();
            }
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Navigate to the Page for the selected <paramref name="listViewItem"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="listViewItem"></param>
        private void NavMenuList_ItemInvoked(object sender, ListViewItem listViewItem)
        {
            var item = (NavMenuItem)((NavMenuListView)sender).ItemFromContainer(listViewItem);

            var item1 = (from p in this._navlist where p.DestPage == item.DestPage select p).SingleOrDefault() ??
                        (from p in this._setlist where p.DestPage == item.DestPage select p).SingleOrDefault();

            if (item != null)
            {
                var container = (ListViewItem)NavMenuList.ContainerFromItem(item1) ??
                                (ListViewItem)SettingsMenuList.ContainerFromItem(item1);

                // While updating the selection state of the item prevent it from taking keyboard focus.  If a
                // user is invoking the back button via the keyboard causing the selected nav menu item to change
                // then focus will remain on the back button.
                if (container != null) container.IsTabStop = false;
                NavMenuList.SetSelectedItem(container);
                SettingsMenuList.SetSelectedItem(container);
                if (container != null) container.IsTabStop = true;

                // Navigate to selected Page
                if (item.DestPage != null &&
                    item.DestPage != this.AppFrame.CurrentSourcePageType)
                {
                    this.AppFrame.Navigate(item.DestPage, item.Arguments);
                }
            }
        }

        /// <summary>
        /// Ensures the nav menu reflects reality when navigation is triggered outside of
        /// the nav menu buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                var item = (from p in this._navlist where p.DestPage == e.SourcePageType select p).SingleOrDefault() ??
                           (from p in this._setlist where p.DestPage == e.SourcePageType select p).SingleOrDefault();
                if (item == null && this.AppFrame.BackStackDepth > 0)
                {
                    // In cases where a page drills into sub-pages then we'll highlight the most recent
                    // navigation menu item that appears in the BackStack
                    foreach (var entry in this.AppFrame.BackStack.Reverse())
                    {
                        item = (from p in this._navlist where p.DestPage == entry.SourcePageType select p).SingleOrDefault() ??
                               (from p in this._setlist where p.DestPage == e.SourcePageType select p).SingleOrDefault();
                        if (item != null)
                            break;
                    }
                }

                var container = (ListViewItem) NavMenuList.ContainerFromItem(item) ??
                                (ListViewItem) SettingsMenuList.ContainerFromItem(item);

                // While updating the selection state of the item prevent it from taking keyboard focus.  If a
                // user is invoking the back button via the keyboard causing the selected nav menu item to change
                // then focus will remain on the back button.
                if (container != null) container.IsTabStop = false;
                NavMenuList.SetSelectedItem(container);
                SettingsMenuList.SetSelectedItem(container);
                if (container != null) container.IsTabStop = true;
            }
        }

        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            // After a successful navigation set keyboard focus to the loaded page
            if (e.Content is Page && e.Content != null)
            {
                var control = (Page)e.Content;
                control.Loaded += Page_Loaded;
            }

            // Update the Back button depending on whether we can go Back.
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((Page)sender).Focus(FocusState.Programmatic);
            ((Page)sender).Loaded -= Page_Loaded;
            this.CheckTogglePaneButtonSizeChanged();
        }

        #endregion

        public Rect TogglePaneButtonRect
        {
            get;
            private set;
        }

        /// <summary>
        /// An event to notify listeners when the hamburger button may occlude other content in the app.
        /// The custom "PageHeader" user control is using this.
        /// </summary>
        public event TypedEventHandler<MainPage, Rect> TogglePaneButtonRectChanged;

        /// <summary>
        /// Callback when the SplitView's Pane is toggled open or close.  When the Pane is not visible
        /// then the floating hamburger may be occluding other content in the app unless it is aware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {
            this.CheckTogglePaneButtonSizeChanged();
        }

        /// <summary>
        /// Check for the conditions where the navigation pane does not occupy the space under the floating
        /// hamburger button and trigger the event.
        /// </summary>
        private void CheckTogglePaneButtonSizeChanged()
        {
            if (this.RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
                this.RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                var transform = this.TogglePaneButton.TransformToVisual(this);
                var rect = transform.TransformBounds(new Rect(0, 0, this.TogglePaneButton.ActualWidth, this.TogglePaneButton.ActualHeight));
                this.TogglePaneButtonRect = rect;
            }
            else
            {
                this.TogglePaneButtonRect = new Rect();
            }

            var handler = this.TogglePaneButtonRectChanged;
            if (handler != null)
            {
                // handler(this, this.TogglePaneButtonRect);
                handler.DynamicInvoke(this, this.TogglePaneButtonRect);
            }
        }

        /// <summary>
        /// Enable accessibility on each nav menu item by setting the AutomationProperties.Name on each container
        /// using the associated Label of each item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavMenuItemContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue && args.Item != null && args.Item is NavMenuItem)
            {
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, ((NavMenuItem)args.Item).Label);
            }
            else
            {
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty);
            }
        }

        private void SearchAllManga_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            _suggestions = Mangas.Where(p => p.Title.StartsWith(sender.Text) && sender.Text != string.Empty);
            SearchAllManga.ItemsSource = _suggestions;
        }

        private void SearchAllManga_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.Text = "";
            var suggestionChosen = args.ChosenSuggestion;
            if (suggestionChosen != null)
            {
                this.AppFrame.Navigate(typeof (MangaOverviewPage), suggestionChosen as Manga);
            }
            else
            {
                var results = sender.Items;
                this.AppFrame.Navigate(typeof (GridViewPage), results);
            }
        }

        private void SearchAllManga_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = "";
        }
    }
}