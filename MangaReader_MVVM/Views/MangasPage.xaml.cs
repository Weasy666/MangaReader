﻿using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MangaReader_MVVM.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MangasPage : Page
    {
        public MangasPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
    }
}
