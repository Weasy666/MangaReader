using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MangaReader_Toolkit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            hamburgerMenuControl.ItemsSource = MenuItem.GetMainItems();
            hamburgerMenuControl.OptionsItemsSource = MenuItem.GetOptionsItems();
        }

        private void OnMenuItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as MenuItem;
            contentFrame.Navigate(menuItem.PageType);
        }
    }

    public class MenuItem
    {
        public Symbol Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }

        public static List<MenuItem> GetMainItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Accept, Name = "MenuItem1", PageType = typeof(Views.BlankPage1) });
            items.Add(new MenuItem() { Icon = Symbol.Send, Name = "MenuItem2", PageType = typeof(Views.BlankPage1) });
            items.Add(new MenuItem() { Icon = Symbol.Shop, Name = "MenuItem3", PageType = typeof(Views.BlankPage1) });
            return items;
        }

        public static List<MenuItem> GetOptionsItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Setting, Name = "OptionItem1", PageType = typeof(Views.BlankPage1) });
            return items;
        }
    }
}
