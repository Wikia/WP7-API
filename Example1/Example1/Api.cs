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

namespace DotNetMetroWikiaAPI
{
    static public partial class Api
    {
        static Site usedWiki = null;
        static bool isTempDelFree = true;
        static Delegate tempDel = null;

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
            if ((usedWiki != null) && (isTempDelFree))
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
            if ((usedWiki != null) && (isTempDelFree))
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
            if ((usedWiki != null) && (isTempDelFree))
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
            if ((usedWiki != null)&&(isTempDelFree))
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
            if ((usedWiki != null) && (isTempDelFree))
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
            if ((usedWiki != null) && (isTempDelFree))
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
            if ((usedWiki != null) && (isTempDelFree))
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
    }
}
