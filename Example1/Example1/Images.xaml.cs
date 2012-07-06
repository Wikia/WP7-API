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
using System.Collections.ObjectModel;
using System.Threading;

namespace Example1
{
    public partial class Images : PhoneApplicationPage
    {
        /// <summary>Is user logged into the wikia.com .</summary>
        static public bool isLogged = false;
        private Dictionary<DotNetMetroWikiaAPI.Api.FileInfo, WriteableBitmap>
            TenImages = new Dictionary<DotNetMetroWikiaAPI.Api.FileInfo,
                WriteableBitmap>();
        private List<DotNetMetroWikiaAPI.Api.FileInfo> ListOfFiles = new
            List<DotNetMetroWikiaAPI.Api.FileInfo>();
        /// <summary>Consist names and prefixes of 10 remembered wikis.</summary>
        public ObservableCollection<PairOfNames> TenWikias { get; set; }
        private string prefix = "";
        private int tempCounter = 0;

        public Images()
        {
            InitializeComponent();

            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(Images_Loaded);
        }

        /// <summary>Loads dictionary of Ten Wikias into the application. If it's first
        /// time run - creates file with default ones.</summary>
        private void GetTenWikisFromFile()
        {
            TenWikias = new ObservableCollection<PairOfNames>();

            Wikis.ItemsSource = TenWikias;

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

                        for (int i = 0; ((i<10)&&(!reader.EndOfStream)); i++)
                        {
                            string name = reader.ReadLine();
                            string prefix = reader.ReadLine();
                            string stars = reader.ReadLine();
                            if (stars.Equals("**********"))
                            {
                                TenWikias.Add(new PairOfNames());
                                TenWikias[i].NameOfWiki = name;
                                TenWikias[i].PrefixOfWiki = prefix;
                            }
                        }

                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    searchWikiBox.Text = e.Message;
                    // TODO: Do something with that Exception.
                }
            }
            else
            {
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias.Add(new PairOfNames());
                TenWikias[0].NameOfWiki = "glee wiki";
                TenWikias[0].PrefixOfWiki = "glee";
                TenWikias[1].NameOfWiki = "BATTLEFIELD WIKI";
                TenWikias[1].PrefixOfWiki = "battlefield";
                TenWikias[2].NameOfWiki = "animepedia";
                TenWikias[2].PrefixOfWiki = "anime";
                TenWikias[3].NameOfWiki = "Logopedia";
                TenWikias[3].PrefixOfWiki = "logos";
                TenWikias[4].NameOfWiki = "Academic Jobs";
                TenWikias[4].PrefixOfWiki = "academicjobs";
                TenWikias[5].NameOfWiki = "Borderlands Wiki";
                TenWikias[5].PrefixOfWiki = "borderlands";
                TenWikias[6].NameOfWiki = "Solar Cookers World";
                TenWikias[6].PrefixOfWiki = "solarcooking";
                TenWikias[7].NameOfWiki = "Snow White and the Huntsman Wiki";
                TenWikias[7].PrefixOfWiki = "snowwhiteandthehuntsman";
                TenWikias[8].NameOfWiki = "The One Wiki to Rule Them All";
                TenWikias[8].PrefixOfWiki = "lotr";
                TenWikias[9].NameOfWiki = "CALL OF DUTY";
                TenWikias[9].PrefixOfWiki = "callofduty";

                using (isf)
                {
                    if (!isf.DirectoryExists("Resources"))
                    {
                        isf.CreateDirectory("Resources");
                    }

                    using (IsolatedStorageFileStream rawStream = isf.CreateFile(path))
                    {
                        StreamWriter writer = new StreamWriter(rawStream);

                        foreach (PairOfNames pair in TenWikias)
                        {
                            writer.WriteLine(pair.NameOfWiki, Encoding.UTF8);
                            writer.WriteLine(pair.PrefixOfWiki, Encoding.UTF8);
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

            GetTenWikisFromFile();
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

        private void test3(WriteableBitmap downloadedImage, DotNetMetroWikiaAPI.Api.FileInfo info)
        {
            TenImages.Add(info, downloadedImage);
            backImageTEST.Source = downloadedImage;
        }

        private void CheckNow()
        {
            tempCounter++;
            if (tempCounter == ListOfFiles.Count)
            {
                foreach (DotNetMetroWikiaAPI.Api.FileInfo fi in ListOfFiles)
                {
                    DotNetMetroWikiaAPI.Api.DownloadImage(test3, ListOfFiles
                        .ElementAt(0));
                }
            }
        }

        private async void test(List<DotNetMetroWikiaAPI.Api.FileInfo> lista)
        {
            ListOfFiles = lista;
            searchWikiBox.Text = ListOfFiles.ElementAt(0).ToString();
            tempCounter = 0;
            foreach (DotNetMetroWikiaAPI.Api.FileInfo fi in ListOfFiles)
            {
                DotNetMetroWikiaAPI.Api.GetAddressOfTheFile
                    (new Action(CheckNow), fi, prefix);
            }
        }

        private void ListBoxItem_Tap(object sender, GestureEventArgs e)
        {
            string name = ((TextBlock)((StackPanel)sender).Children.ElementAt(0)).Text;
            foreach (PairOfNames pon in TenWikias)
            {
                if (pon.NameOfWiki == name)
                {
                    prefix = pon.PrefixOfWiki;
                    break;
                }
            }
            DotNetMetroWikiaAPI.Api.GetNewFilesListFromWiki(test, prefix, 10);
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