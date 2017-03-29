using MangaReader_MVVM.Services.SettingsServices;
using System.Threading.Tasks;
using Template10.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaReader_MVVM
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }

            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            var source = SettingsService.Instance.UsedMangaSource;
            Services.MangaLibrary.Instance.MangaSource = source;
            await Services.MangaLibrary.Instance.GetMangasAsync();

            NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }

        //public override void OnLaunched(LaunchActivatedEventArgs args)
        //{
        //    SettingsPane.GetForCurrentView().CommandsRequested += DisplayPrivacyPolicy;
        //}

        //private void OnSuspending(object sender, SuspendingEventArgs e)
        //{
        //    SettingsPane.GetForCurrentView().CommandsRequested -= DisplayPrivacyPolicy;
        //}

        //private void DisplayPrivacyPolicy(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        //{
        //    SettingsCommand privacyPolicyCommand = new SettingsCommand("privacyPolicy", "Privacy Policy", (uiCommand) => { LaunchPrivacyPolicyUrl(); });
        //    args.Request.ApplicationCommands.Add(privacyPolicyCommand);
        //}

        //async void LaunchPrivacyPolicyUrl()
        //{
        //    Uri privacyPolicyUrl = new Uri("http://myapppolicy.com/app/wmangareader");
        //    var result = await Windows.System.Launcher.LaunchUriAsync(privacyPolicyUrl);
        //}
    }
}

