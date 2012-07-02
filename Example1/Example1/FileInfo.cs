// DotNetMetroWikiaAPI - Wikia API for C# Metro Style Applications
//
// Authors:
// Hazardius (2012-*) hazardiusam at gmail dot com
//
// Work based on:
// DotNetWikiBot Framework 2.101 - bot framework based on Microsoft .NET Framework 2.0 for wiki projects
// Distributed under the terms of the MIT (X11) license: http://www.opensource.org/licenses/mit-license.php
// Copyright (c) Iaroslav Vassiliev (2006-2012) codedriller@gmail.com
//
// Distributed under the terms of the license:
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
// http://www.gnu.org/copyleft/gpl.html

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

namespace DotNetMetroWikiaAPI
{
    static public partial class Api
    {
        public class FileInfo
        {
            /// <summary>Filename</summary>
            string filename;
            /// <summary>Date of publication.</summary>
            DateTime pubDate;
            /// <summary>Which User of Wikia.com publicated that file.</summary>
            string byUsername;

            /// <summary>Creates FileInfo Object.</summary>
            /// <param name="filename">Name of the file.</param>
            /// <param name="year">Year of publication.</param>
            /// <param name="month">Month of publication.</param>
            /// <param name="day">Day of publication.</param>
            /// <param name="hour">Hour of publication.</param>
            /// <param name="minute">Minute of publication.</param>
            /// <param name="second">Second of publication.</param>
            public FileInfo(string filename, int year, int month, int day, int hour,
                int minute, int second) : this(filename, year, month, day, hour, minute,
                second, "")
            { }

            /// <summary>Creates FileInfo Object.</summary>
            /// <param name="filename">Name of the file.</param>
            /// <param name="year">Year of publication.</param>
            /// <param name="month">Month of publication.</param>
            /// <param name="day">Day of publication.</param>
            /// <param name="hour">Hour of publication.</param>
            /// <param name="minute">Minute of publication.</param>
            /// <param name="second">Second of publication.</param>
            /// <param name="username">Username who publicated that file.</param>
            public FileInfo(string filename, int year, int month, int day, int hour,
                int minute, int second, string username)
            {
                this.filename = filename;
                pubDate = new DateTime(year, month, day, hour, minute, second);
                byUsername = username;
            }

            /// <summary>Creates FileInfo Object.</summary>
            /// <param name="filename">Name of the file.</param>
            /// <param name="date">Date of publication in format:
            /// Day of week, Year Month Day Hour:Minute:Second ...
            /// f.e. Mon, 2012 Jul 02 17:50:15 +0000</param>
            public FileInfo(string filename, string date)
                : this(filename, date, "")
            { }

            /// <summary>Creates FileInfo Object.</summary>
            /// <param name="filename">Name of the file.</param>
            /// <param name="date">Date of publication in format:
            /// Day of week, Year Month Day Hour:Minute:Second ...
            /// f.e. Mon, 2012 Jul 02 17:50:15 +0000</param>
            /// <param name="username">Username who publicated that file.</param>
            public FileInfo(string filename, string date, string username)
            {
                this.filename = filename;

                int day = int.Parse(date.Substring(5, 2));
                int month = 0;
                switch (date.Substring(8, 3))
                {
                    case "Jan": month = 1;
                        break;
                    case "Feb": month = 2;
                        break;
                    case "Mar": month = 3;
                        break;
                    case "Apr": month = 4;
                        break;
                    case "May": month = 5;
                        break;
                    case "Jun": month = 6;
                        break;
                    case "Jul": month = 7;
                        break;
                    case "Aug": month = 8;
                        break;
                    case "Sep": month = 9;
                        break;
                    case "Oct": month = 10;
                        break;
                    case "Nov": month = 11;
                        break;
                    case "Dec": month = 12;
                        break;
                }
                int year = int.Parse(date.Substring(12, 4));
                int hour = int.Parse(date.Substring(17, 2));
                int minute = int.Parse(date.Substring(20, 2));
                int second = int.Parse(date.Substring(23, 2));

                pubDate = new DateTime(year, month, day, hour, minute,
                    second);
                byUsername = username;
            }

            public string GetFilename()
            {
                return filename;
            }

            public void SetByUsername(string username)
            {
                byUsername = username;
            }

            public override string ToString()
            {
                return filename + "send by " + byUsername;
            }
        }
    }
}
