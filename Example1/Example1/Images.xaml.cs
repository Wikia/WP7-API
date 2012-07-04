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
using RestSharp;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text;

namespace Example1
{
    public partial class Images : PhoneApplicationPage
    {
        /// <summary>Is user logged into the wikia.com .</summary>
        static public bool isLogged = false;
        static private Dictionary<DotNetMetroWikiaAPI.Api.FileInfo, WriteableBitmap>
            TenImages = new Dictionary<DotNetMetroWikiaAPI.Api.FileInfo,
                WriteableBitmap>();
        /// <summary>Consist names and prefixes of 10 remembered wikis.</summary>
        static private Dictionary<string, string> TenWikias = new Dictionary<string,
            string>();

        public Images()
        {
            InitializeComponent();

            GetTenWikisFromFile();

            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(Images_Loaded);
        }

        /// <summary>Loads dictionary of Ten Wikias into the application. If it's first
        /// time run - creates file with default ones.</summary>
        private void GetTenWikisFromFile()
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

            string path = "Resources" + System.IO.Path.DirectorySeparatorChar
                + "TenWikis.dat";

            if (isf.FileExists(path))
            {
                try
                {
                    using (IsolatedStorageFileStream rawStream = isf.OpenFile(path,
                        System.IO.FileMode.Open))
                    {
                        StreamReader reader = new StreamReader(rawStream, Encoding.UTF8);

                        for (int i = 0; reader.EndOfStream; i++)
                        {
                            string name = reader.ReadLine();
                            string prefix = reader.ReadLine();
                            string stars = reader.ReadLine();
                            if (stars.Equals("**********"))
                            {
                                TenWikias.Add(name, prefix);
                            }
                        }

                        reader.Close();
                    }
                }
                catch
                {
                    // TODO: Do something with that Exception.
                }
            }
            else
            {
                TenWikias.Add("glee wiki", "glee");
                TenWikias.Add("BATTLEFIELD WIKI", "battlefield");
                TenWikias.Add("animepedia", "anime");
                TenWikias.Add("Logopedia", "logos");
                TenWikias.Add("Academic Jobs", "academicjobs");
                TenWikias.Add("Borderlands Wiki", "borderlands");
                TenWikias.Add("Solar Cookers World", "solarcooking");
                TenWikias.Add("Snow White and the Huntsman Wiki",
                    "snowwhiteandthehuntsman");
                TenWikias.Add("The One Wiki to Rule Them All", "lotr");
                TenWikias.Add("Dragon Age Wiki", "dragonage");

                using (isf)
                {
                    using (IsolatedStorageFileStream rawStream = isf.CreateFile(path))
                    {
                        StreamWriter writer = new StreamWriter(rawStream);

                        foreach (KeyValuePair<string, string> pair in TenWikias)
                        {
                            writer.WriteLine(pair.Key, Encoding.UTF8);
                            writer.WriteLine(pair.Value, Encoding.UTF8);
                            writer.WriteLine("**********", Encoding.UTF8);
                        }

                        writer.Close();
                    }
                }
            }
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

        private void test2(WriteableBitmap downloadedImage)
        {
            backImageTEST.Source = downloadedImage;
        }

        private void test(List<DotNetMetroWikiaAPI.Api.FileInfo> lista)
        {
            searchWikiBox.Text = lista.ElementAt(0).ToString();
            /// TODO: Need to have working imagecrop on MW1.19 !!!
            lista.ElementAt(0).SetAddressOfFile("http://www.fizyka.umk.pl/~jkob/gallery4home/gallery4home/original_img_5430_jpeg.jpeg");
            DotNetMetroWikiaAPI.Api.DownloadImage(test2, lista.ElementAt(0));
        }

        private void ListBoxItem_Tap(object sender, GestureEventArgs e)
        {
            DotNetMetroWikiaAPI.Api.GetNewFilesListFromWiki(test, "glee", 10);
        }

        private void Grid_DoubleTap(object sender, GestureEventArgs e)
        {
            ImageProcessing.saveTopImagesAsTiles((Grid)sender, "/Images/1.jpg");
        }

        private void LogOut(IRestResponse e, string sendData)
        {
            isLogged = false;
        }

        private void PhoneApplicationPage_LostFocus(object sender, RoutedEventArgs e)
        {
            DotNetMetroWikiaAPI.Api.LogOut(new Action<IRestResponse, string>(LogOut));
        }

        private void PhoneApplicationPage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!isLogged)
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}