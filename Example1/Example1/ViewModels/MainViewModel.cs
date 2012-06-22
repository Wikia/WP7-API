using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;


namespace WikiaApiExample1
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.Items.Add(new ItemViewModel()
            {
                Odd = "/Images/1.jpg",
                OddAvatar = "/Images/1av.jpg",
                OddUsername = "Windows",
                OddDate = "1",
                Even = "/Images/2.jpg",
                EvenAvatar = "/Images/2av.jpg",
                EvenUsername = "Adam",
                EvenDate = "2"
            });
            this.Items.Add(new ItemViewModel()
            {
                Odd = "/Images/3.jpg",
                OddAvatar = "/Images/3av.jpg",
                OddUsername = "Linux",
                OddDate = "3",
                Even = "/Images/4.jpg",
                EvenAvatar = "/Images/4av.jpg",
                EvenUsername = "Barney",
                EvenDate = "4"
            });
            this.Items.Add(new ItemViewModel()
            {
                Odd = "/Images/5.jpg",
                OddAvatar = "/Images/5av.jpg",
                OddUsername = "BeOS",
                OddDate = "5",
                Even = "/Images/6.jpg",
                EvenAvatar = "/Images/6av.jpg",
                EvenUsername = "Ceslav",
                EvenDate = "6"
            });
            this.Items.Add(new ItemViewModel()
            {
                Odd = "/Images/7.jpg",
                OddAvatar = "/Images/7av.jpg",
                OddUsername = "MacOS",
                OddDate = "7",
                Even = "/Images/8.jpg",
                EvenAvatar = "/Images/8av.jpg",
                EvenUsername = "Diana",
                EvenDate = "8"
            });
            this.Items.Add(new ItemViewModel()
            {
                Odd = "/Images/9.jpg",
                OddAvatar = "/Images/9av.jpg",
                OddUsername = "Android",
                OddDate = "9",
                Even = "/Images/10.jpg",
                EvenAvatar = "/Images/10av.jpg",
                EvenUsername = "Edward",
                EvenDate = "10"
            });

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}