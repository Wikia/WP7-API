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
using RestSharp;

namespace DotNetMetroWikiaAPI
{
    static public partial class Api{
        private class QueryMaker
        {
            Delegate callback;
            Site site;
            object[] args;

            /// <summary>Creates a query and gets respose.</summary>
            /// <param name="callback">Delegate of method you want to use. It must have two
            /// more argument fields at the beginning for string queryResponse and
            /// string sendData.</param>
            /// <param name="query">Query you want to use.</param>
            /// <param name="site">Site you want to use.</param>
            /// <param name="args">Delegate's arguments.</param>
            public QueryMaker(Delegate callback, string query, Site site, params object[] args)
            {
                this.callback = callback;
                this.site = site;
                this.args = args;

                RunQuery(query);
            }

            /// <summary>Post another things(not exactly query).</summary>
            /// <param name="site">Site you want to use.</param>
            /// <param name="request">What do you need from api.</param>
            /// <param name="callback">Delegate of method you want to use. It must have
            /// two more argument fields at the beginning for IRestResponse queryResponse and
            /// string sendData.</param>
            public QueryMaker(Delegate callback, string request, Site site)
            {
                this.callback = callback;
                this.site = site;

                RunRequest(request);
            }

            private void callbackRequestWrapper(IRestResponse e, string sendData, params object[] nullArgs)
            {
                callback.DynamicInvoke(e, sendData);
            }

            private void callbackQueryWrapper(IRestResponse e, string sendData, params object[] nullArgs)
            {
                callback.DynamicInvoke(e.Content, sendData, args);
            }

            private void RunRequest(string request)
            {
                site.PostDataAndGetResultHTM(site.site + "/api.php", request, callbackRequestWrapper);
            }

            private void RunQuery(string query)
            {
                site.PostDataAndGetResultHTM(site.site + "/api.php", "action=query&" + query, callbackQueryWrapper);
            }
        }
    }
}
