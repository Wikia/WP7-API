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
using System.Windows.Media.Imaging;
using System.Text;
using Microsoft.Phone;

namespace DotNetMetroWikiaAPI
{
    static public partial class Api
    {
        static Site usedWiki = null;
        static bool isTempDelFree = true;
        static Delegate tempDel = null;
        static int tempInt = -1;
        static string tempAddress = null;

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
            Delegate temp = tempDel;
            tempDel = null;
            isTempDelFree = true;
            temp.DynamicInvoke(listOfWikis);
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
            Delegate temp = tempDel;
            tempDel = null;
            isTempDelFree = true;
            temp.DynamicInvoke(number);
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

        private static void ReturnNewFilesListWrapper(string content, object[] args)
        {
            int counter = (int)args[1];
            List<FileInfo> files = (List<FileInfo>)args[0];
            counter++;
            XmlReader reader = XmlReader.Create(new StringReader(content));
            if (reader.ReadToDescendant("page"))
            {
                files[counter-1].SetFileID(int.Parse(HttpUtility.HtmlDecode(reader
                    .GetAttribute(0))));
                if (reader.ReadToDescendant("ii"))
                    files[counter - 1].SetByUsername(HttpUtility.HtmlDecode(reader
                        .GetAttribute(1)));
            }
            reader.Close();
            if ( counter < tempInt )
            {
                usedWiki.GetPageHTM(tempAddress + "/api.php?action=query&titles=File:"
                    + files[counter].GetFilename() + "&prop=imageinfo&format=xml",
                    ReturnNewFilesListWrapper, files, counter);
            } else {
                Delegate temp = tempDel;
                tempDel = null;
                tempInt = -1;
                isTempDelFree = true;
                temp.DynamicInvoke(files);
            }
        }

        private static void GetNewFilesListWrapper(string pageCode, params object[] args)
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
                    filename = HttpUtility.HtmlDecode(reader
                        .ReadElementContentAsString());
                    reader.ReadToFollowing("pubDate");
                    date = HttpUtility.HtmlDecode(reader.ReadElementContentAsString());
                    files.Add(new FileInfo(filename, date));
                    counter++;
                } while ((reader.ReadToFollowing("title")) && (counter < tempInt));
                tempInt = counter;
                reader.Close();
                usedWiki.GetPageHTM(tempAddress + "/api.php?action=query&titles=File:"
                    + files[0].GetFilename() + "&prop=imageinfo&format=xml",
                    ReturnNewFilesListWrapper, files, 0);

                /// TODO: Cross recurrent to download address of file.
            }
            else
            {
                reader.Close();
                tempDel = null;
                tempInt = -1;
                isTempDelFree = true;
                throw new WebException("Response from the server isn't valid.");
            }
        }

        /// <summary>Get list of New Files from choosen wiki. Returned list will have
        /// only as many entries, as many will rss of Special:NewFiles have.</summary>
        /// <param name="callback">Method using</param>
        /// <param name="wikiname">Prefix used by choosen wiki.</param>
        public static void GetNewFilesListFromWiki(Action<List<FileInfo>> callback,
            string wikiname)
        {
            GetNewFilesListFromWiki(callback, wikiname, 100);
        }

        /// <summary>Get list of New Files from choosen wiki. Returned list will have
        /// only as many entries, as many will rss of Special:NewFiles have.</summary>
        /// <param name="callback">Method using</param>
        /// <param name="wikiname">Prefix used by choosen wiki.</param>
        /// <param name="quantity">How many new files you want to get.</param>
        public static void GetNewFilesListFromWiki(Action<List<FileInfo>> callback,
            string wikiname, int quantity)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            tempDel = callback;
            isTempDelFree = false;
            tempInt = quantity;
            int beginning = usedWiki.site.IndexOf(".wikia");
            tempAddress = "http://" + wikiname + usedWiki.site.Substring(beginning);
            usedWiki.GetPageHTM(tempAddress + "/wiki/Special:NewFiles?feed=rss",
                GetNewFilesListWrapper);
        }

        private static void DownloadImageWrapper(IRestResponse response, params object[] args)
        {
            byte[] imageData = response.RawBytes;
            using (Stream ms = new MemoryStream(imageData))
            {
                WriteableBitmap wbImg = PictureDecoder.DecodeJpeg(ms);
                

                Delegate temp = tempDel;
                tempDel = null;
                isTempDelFree = true;
                temp.DynamicInvoke(wbImg, ((FileInfo)args[0]));
            };

            tempDel = null;
            isTempDelFree = true;
        }

        /// <summary>Download whole image and returns it as a WritableBitmap to the
        /// callback function.</summary>
        /// <param name="callback">Function which will be called after downloading the
        /// picture.</param>
        /// <param name="file">FileInfo of the Picture.</param>
        public static void DownloadImage(Action<WriteableBitmap, FileInfo> callback, FileInfo file)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            tempDel = callback;
            isTempDelFree = false;
            if (file.isImage())
            {
                usedWiki.GetPageHTM(file.GetAddressOfFile(), DownloadImageWrapper, file);
            }
            else
            {
                tempDel = null;
                isTempDelFree = true;
            }
        }

        private static void ReturnAddressOfTheFileWrapper(IRestResponse e, params object[] args)
        {
            string content = e.Content;
            FileInfo target = (FileInfo)args[0];

            XmlReader reader = XmlReader.Create(new StringReader(content));
            if (reader.ReadToFollowing("image"))
            {
                target.SetAddressOfFile(reader.GetAttribute(0));
            }
            else
            {
                reader.Close();
                tempDel = null;
                isTempDelFree = true;
            
                throw new WikiUserException("Wrong xml file received.");
            }
            reader.Close();

            Delegate temp = tempDel;
            tempDel = null;
            isTempDelFree = true;
            temp.DynamicInvoke();
        }

        public static void GetAddressOfTheFile(Action callback, FileInfo file, string wikiname)
        {
            while (!isTempDelFree) { Thread.Sleep(2000); };
            tempDel = callback;
            isTempDelFree = false;

            int beginning = usedWiki.site.IndexOf(".wikia");

            usedWiki.GetPageHTM("http://" + wikiname + usedWiki.site.Substring(beginning)
                + "api.php?action=imageserving&wisId=" + file.GetFileID()
                + "&format=xml", ReturnAddressOfTheFileWrapper, file);

        }
    }
}
