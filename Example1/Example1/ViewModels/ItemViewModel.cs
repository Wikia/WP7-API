using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WikiaApiExample1
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _odd;
        public string Odd
        {
            get
            {
                return _odd;
            }
            set
            {
                if (value != _odd)
                {
                    _odd = value;
                    NotifyPropertyChanged("Odd");
                }
            }
        }

        private string _oddAvatar;
        public string OddAvatar
        {
            get
            {
                return _oddAvatar;
            }
            set
            {
                if (value != _oddAvatar)
                {
                    _oddAvatar = value;
                    NotifyPropertyChanged("OddAvatar");
                }
            }
        }

        private string _oddUsername;
        public string OddUsername
        {
            get
            {
                return _oddUsername;
            }
            set
            {
                if (value != _oddUsername)
                {
                    _oddUsername = value;
                    NotifyPropertyChanged("OddUsername");
                }
            }
        }

        private string _oddDate;
        public string OddDate
        {
            get
            {
                return _oddDate;
            }
            set
            {
                if (value != _oddDate)
                {
                    _oddDate = value;
                    NotifyPropertyChanged("OddDate");
                }
            }
        }

        public string OddText
        {
            get
            {
                //TODO: method returning e.g. "2 days ago" in place of _oddDate
                return _oddUsername + "-" + _oddDate;
            }
        }

        private string _even;
        public string Even
        {
            get
            {
                return _even;
            }
            set
            {
                if (value != _even)
                {
                    _even = value;
                    NotifyPropertyChanged("Even");
                }
            }
        }

        private string _evenAvatar;
        public string EvenAvatar
        {
            get
            {
                return _evenAvatar;
            }
            set
            {
                if (value != _evenAvatar)
                {
                    _evenAvatar = value;
                    NotifyPropertyChanged("EvenAvatar");
                }
            }
        }

        private string _evenUsername;
        public string EvenUsername
        {
            get
            {
                return _evenUsername;
            }
            set
            {
                if (value != _evenUsername)
                {
                    _evenUsername = value;
                    NotifyPropertyChanged("EvenUsername");
                }
            }
        }

        private string _evenDate;
        public string EvenDate
        {
            get
            {
                return _evenDate;
            }
            set
            {
                if (value != _evenDate)
                {
                    _evenDate = value;
                    NotifyPropertyChanged("EvenDate");
                }
            }
        }

        public string EvenText
        {
            get
            {
                //TODO: method returning e.g. "2 days ago" in place of _oddDate
                return _evenUsername + "-" + _evenDate;
            }
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