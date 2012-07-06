using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Example1
{
    public class PairOfNames : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _NameOfWiki="";
        public string NameOfWiki
        {
            get
            {
                return _NameOfWiki;
            }
            set
            {
                _NameOfWiki = value;
                NotifyPropertyChanged("NameOfWiki");
            }
        }

        private string _PrefixOfWiki="";
        public string PrefixOfWiki
        {
            get
            {
                return _PrefixOfWiki;
            }
            set
            {
                _PrefixOfWiki = value;
                NotifyPropertyChanged("PrefixOfWiki");
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void NotifyPropertyChanged(String name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
