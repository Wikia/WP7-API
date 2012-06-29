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
    public class QueryMaker
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

        private void callbackWrapper(IRestResponse e, string sendData, params object[] nullArgs)
        {
            callback.DynamicInvoke(e.Content, sendData, args);
        }

        private void RunQuery(string query)
        {
            site.PostDataAndGetResultHTM(site.site + "/api.php", "action=query&" + query, callbackWrapper);
        }
    }
}
