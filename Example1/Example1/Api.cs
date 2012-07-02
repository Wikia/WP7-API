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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using RestSharp;
using System.Threading;

namespace DotNetMetroWikiaAPI
{
    static public partial class Api
    {
        static Site usedWiki = null;
        static bool isTempDelFree = true;
        static Delegate tempDel = null;
        static int tempInt = -1;

        /// <summary>Method to log in to the www.wikia.com api.</summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="successInLogin">What should happen when user will be logged in.
        /// </param>
        /// <param name="failInLogin">What should happen if something's wrong.</param>
        public static void LogIn(string username, string password, Action successInLogin,
            Action failInLogin)
        {
            LogIn("http://www.wikia.com", username, password, successInLogin,
                failInLogin);
        }

        /// <summary>Method to log in to the api of choosen wikia page.</summary>
        /// <param name="address">Address of wikia page.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="successInLogin">What should happen when user will be logged in.
        /// </param>
        /// <param name="failInLogin">What should happen if something's wrong.</param>
        public static void LogIn(string address, string username, string password,
            Action successInLogin, Action failInLogin)
        {
            usedWiki = new Site(address, username, password, successInLogin,
                failInLogin);
        }

        /// <summary>Method to log out from the api of wikia.</summary>
        /// <param name="callback">What should happen when user will be logged out.
        /// Using only args: IRestResponce queryResponce and string sendData.</param>
        public static void LogOut(Delegate callback)
        {
            new QueryMaker(callback, "action=logout", usedWiki);
            usedWiki.ResetDictionaries();
            usedWiki = null;
        }

        /// <summary>Method to get response for a query. It's recommended to use other
        /// methods from this API to get what You need.</summary>
        /// <param name="callback">Delegate of function to run after receiving query
        /// response. Using only args: IRestResponce queryResponce and string sendData.
        /// </param>
        /// <param name="query">Query.</param>
        public static void SendQuery(Delegate callback, string query)
        {
            new QueryMaker(callback, query, usedWiki, null);
        }

        /// <summary>Method to get response for a query. It's recommended to use other
        /// methods from this API to get what You need.</summary>
        /// <param name="callback">Delegate of function to run after receiving query
        /// response.</param>
        /// <param name="query">Query.</param>
        /// <param name="args">Arguments except IRestResponse queryResponse and string
        /// sendData which MUST be first two args of delegate.</param>
        public static void SendQuery(Delegate callback, string query,
            params object[] args)
        {
            new QueryMaker(callback, query, usedWiki, args);
        }

        private static void SendBackListOfWikis(string response, string postData,
            object[] args)
        {
            List<string> listOfWikis = new List<string>();
            XmlReader reader = XmlReader.Create(new StringReader(response));
            if (reader.ReadToDescendant("variable"))
                do
                {
                    if (reader.NodeType == XmlNodeType.Text)
                        listOfWikis.Add(HttpUtility.HtmlDecode(reader.Value));
                } while (reader.Read());
            reader.Close();
            tempDel.DynamicInvoke(listOfWikis);
            tempDel = null;
            isTempDelFree = true;
        }

        private static void SendBackNumberOfWikis(string response, string postData,
            object[] args)
        {
            int number = 0;
            XmlReader reader = XmlReader.Create(new StringReader(response));
            if (reader.ReadToDescendant("variable"))
                do
                {
                    if (reader.NodeType == XmlNodeType.Element
                        && reader.Name == "variable")
                        number++;
                } while (reader.Read());
            reader.Close();
            tempDel.DynamicInvoke(number);
            tempDel = null;
            isTempDelFree = true;
        }

        /// <summary>Get list of all wikis.</summary>
        /// <param name="callback">Method using the list.</param>
        /// <param name="from">Beginning of interval.</param>
        /// <param name="to">End of interval.</param>
        public static void GetListOfWikis(Delegate callback, int from, int to)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            if (usedWiki != null)
            {
                isTempDelFree = false;
                tempDel = callback;
                SendQuery(new Action<string, string, object[]>(SendBackListOfWikis),
                    "list=wkdomains&wkfrom=" + from + "&wkto=" + to + "&format=xml");
            }
        }

        /// <summary>Get list of all wikis.</summary>
        /// <param name="callback">Method using the list.</param>
        /// <param name="from">Beginning of interval.</param>
        /// <param name="to">End of interval.</param>
        /// <param name="onlyActive">Return only active wikis?</param>
        public static void GetListOfWikis(Delegate callback, int from, int to,
            bool onlyActive)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            if (usedWiki != null)
            {
                isTempDelFree = false;
                tempDel = callback;
                int active = 0;
                if (onlyActive)
                    active = 1;
                SendQuery(new Action<string, string, object[]>(SendBackListOfWikis),
                    "list=wkdomains&wkfrom=" + from + "&wkactive=" + active + "&wkto="
                    + to + "&format=xml");
            }
        }

        /// <summary>Get list of all wikis.</summary>
        /// <param name="callback">Method using the list.</param>
        /// <param name="from">Beginning of interval.</param>
        /// <param name="to">End of interval.</param>
        /// <param name="wikiLang">Return only wikis of selected language.</param>
        public static void GetListOfWikis(Delegate callback, int from, int to,
            string wikiLang)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            if (usedWiki != null)
            {
                isTempDelFree = false;
                tempDel = callback;
                SendQuery(new Action<string, string, object[]>(SendBackListOfWikis),
                    "list=wkdomains&wkfrom=" + from + "&wkto=" + to + "&wklang="
                    + wikiLang + "&format=xml");
            }
        }

        /// <summary>Get list of all wikis.</summary>
        /// <param name="callback">Method using the list.</param>
        /// <param name="from">Beginning of interval.</param>
        /// <param name="to">End of interval.</param>
        /// <param name="onlyActive">Return only active wikis?</param>
        /// <param name="wikiLang">Return only wikis of selected language.</param>
        public static void GetListOfWikis(Delegate callback, int from, int to,
            bool onlyActive, string wikiLang)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            if (usedWiki != null)
            {
                isTempDelFree = false;
                tempDel = callback;
                int active = 0;
                if (onlyActive)
                    active = 1;
                SendQuery(new Action<string, string, object[]>(SendBackListOfWikis),
                    "list=wkdomains&wkfrom=" + from + "&wkactive=" + active + "&wkto="
                    + to + "&wklang=" + wikiLang + "&format=xml");
            }
        }

        /// <summary>Get number of all wikis in interval.</summary>
        /// <param name="callback">Method using the link.</param>
        /// <param name="from">Beginning of interval.</param>
        /// <param name="to">End of interval.</param>
        public static void GetNumberOfWikis(Delegate callback, int from, int to)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            if (usedWiki != null)
            {
                isTempDelFree = false;
                tempDel = callback;
                SendQuery(new Action<string, string, object[]>(SendBackNumberOfWikis),
                        "list=wkdomains&wkfrom=" + from + "&wkto=" + to + "&wkcountonly="
                        + 1 + "&format=xml");
            }
        }

        /// <summary>Get number of all wikis in interval.</summary>
        /// <param name="callback">Method using the link.</param>
        /// <param name="from">Beginning of interval.</param>
        /// <param name="to">End of interval.</param>
        /// <param name="wikiLang">Count only wikis of selected language.</param>
        public static void GetNumberOfWikis(Delegate callback, int from, int to,
            string wikiLang)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            if (usedWiki != null)
            {
                isTempDelFree = false;
                tempDel = callback;
                SendQuery(new Action<string, string, object[]>(SendBackNumberOfWikis),
                        "list=wkdomains&wkfrom=" + from + "&wkto=" + to + "&wkcountonly="
                        + 1 + "&wklang=" + wikiLang + "&format=xml");
            }
        }

        /// <summary>Get number of all wikis in interval.</summary>
        /// <param name="callback">Method using the link.</param>
        /// <param name="from">Beginning of interval.</param>
        /// <param name="to">End of interval.</param>
        /// <param name="onlyActive">Count only active wikis?</param>
        public static void GetNumberOfWikis(Delegate callback, int from, int to,
            bool onlyActive)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            if (usedWiki != null)
            {
                isTempDelFree = false;
                tempDel = callback;
                int active = 0;
                if (onlyActive)
                    active = 1;
                SendQuery(new Action<string, string, object[]>(SendBackNumberOfWikis),
                        "list=wkdomains&wkfrom=" + from + "&wkactive=" + active
                        + "&wkto=" + to + "&wkcountonly=" + 1 + "&format=xml");
            }
        }

        private static void ReturnNewFilesListWrapper(IRestResponse e, string postData, List<FileInfo> files)
        {
            tempInt--;
            if (tempInt > 0 )
            {
                /// TODO: Parse XML with Image data to get Username
                /// TODO: Find a way to nicely do counting of Images.
                SendQuery(new Action<IRestResponse, string, List<FileInfo>>
                    (ReturnNewFilesListWrapper), "generator=images&titles="
                    + files[0].GetFilename(), files);
            } else {
                tempDel.DynamicInvoke(files);
                tempDel = null;
                tempInt = -1;
                isTempDelFree = true;
            }
        }

        private static void GetNewFilesListWrapper(string pageCode)
        {
            List<FileInfo> files = new List<FileInfo>();

            XmlReader reader = XmlReader.Create(new StringReader(pageCode));
            if (reader.ReadToFollowing("item"))
            {
                string filename = "", date = "";
                int counter = 0;
                reader.ReadToFollowing("title");
                do
                {
                    filename = HttpUtility.HtmlDecode(reader.ReadElementContentAsString());
                    reader.ReadToFollowing("pubDate");
                    date = HttpUtility.HtmlDecode(reader.ReadElementContentAsString());
                    files.Add(new FileInfo(filename, date));
                    counter++;
                } while ((reader.ReadToFollowing("title")) && (counter < tempInt));
                tempInt = counter;
                reader.Close();
                SendQuery(new Action<IRestResponse, string, List<FileInfo>>
                    (ReturnNewFilesListWrapper), "titles=File:"
                    + files[0].GetFilename() + "&prop=imageinfo", files);
            }
            else
            {
                reader.Close();
                throw new WebException("Response from the server isn't valid.");
            }
        }

        /// <summary>Get list of up to 18 New Files from choosen wiki.</summary>
        /// <param name="callback">Method using</param>
        /// <param name="wikiname">Prefix used by choosen wiki.</param>
        public static void GetNewFilesListFromWiki(Action<List<FileInfo>> callback,
            string wikiname, int quantity)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            tempDel = callback;
            isTempDelFree = false;
            tempInt = quantity;
            int beginning = usedWiki.site.IndexOf(".wikia");
            usedWiki.GetPageHTM("http://" + wikiname + usedWiki.site
                .Substring(beginning) + "/wiki/Special:NewFiles?feed=rss", GetNewFilesListWrapper);
        }
    }
}
