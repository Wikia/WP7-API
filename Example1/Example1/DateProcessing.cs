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

namespace Example1
{
    static public class DateProcessing
    {
        static public string HowLongAgo(DateTime date)
        {
            DateTime now = DateTime.Now;
            int days = 0, hours = 0, minutes;
            minutes = now.Minute - date.Minute;
            if (minutes < 0)
            {
                minutes += 60;
                hours -= 1;
            }
            hours += now.Hour - date.Hour;
            if (hours < 0)
            {
                hours += 24;
                days -= 1;
            }
            days += now.DayOfYear - date.DayOfYear;
            if (days < 0)
            {
                int years = now.Year - date.Year;
                days += years * 365;
                /// TODO: Take into account leap years.
            }
            return days + "d, " + hours + "h, " + minutes + "m";
        }

    }
}
