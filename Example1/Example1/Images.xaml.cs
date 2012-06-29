using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Example1
{
    public partial class Images : PhoneApplicationPage
    {
        public Images()
        {
            InitializeComponent();

            DotNetMetroWikiaAPI.Api.GetListOfWikis(new Action<List<string>>(test), 966, 976, true, "en");
            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(Images_Loaded);
        }

        private void test(List<string> lista)
        {
            searchWikiBox.Text = lista.ElementAt(0);
        }

        // Load data for the ViewModel Items
        private void Images_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Pivot)sender).SelectedIndex == 1)
            {
                this.ApplicationBar.IsVisible = true;

                ShellTile PrimaryTile = ShellTile.ActiveTiles.First();

                if (PrimaryTile != null)
                {
                    StandardTileData tile = new StandardTileData();

                    tile.BackgroundImage = new Uri("/Images/1.jpg", UriKind.Relative);
                    //tile.Title = "Test";
                    tile.BackBackgroundImage = new Uri("/Images/2.jpg", UriKind.Relative);
                    //tile.BackTitle = "App";
                    //tile.BackContent = "Content";

                    PrimaryTile.Update(tile);
                }
            }
            else
            {
                this.ApplicationBar.IsVisible = false;
            }
        }

        private void ListBoxItem_Tap(object sender, GestureEventArgs e)
        {

        }

        private void Grid_DoubleTap(object sender, GestureEventArgs e)
        {
            ImageProcessing.saveGridColumn((Grid)sender, 0, "/Images/1.jpg");
        }
    }
}