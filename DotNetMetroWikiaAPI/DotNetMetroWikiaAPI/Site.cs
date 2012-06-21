// DotNetMetroWikiaAPI - Wikia API for C# Metro Style Applications
// Distributed under the terms of the license:
// TODO : NAME THE USED LICENCE
// Authors:
// Hazardius (2012-*) hazardiusam at gmail dot com
//
// Work based on:
// DotNetWikiBot Framework 2.101 - bot framework based on Microsoft .NET Framework 2.0 for wiki projects
// Distributed under the terms of the MIT (X11) license: http://www.opensource.org/licenses/mit-license.php
// Copyright (c) Iaroslav Vassiliev (2006-2012) codedriller@gmail.com
//
// Note: Most time it's just codedriller's code made to work on Windows Phone.

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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;

namespace DotNetMetroWikiaAPI
{
	/// <summary>Class defines wiki site object.</summary>
    public class Site
    {
        /// <summary>Wiki site URL.</summary>
        public string site;
        /// <summary>User's account to login with.</summary>
        public string userName;
        /// <summary>User's password to login with.</summary>
        private string userPass;
        /// <summary>Default domain for LDAP authentication. Additional information can
        /// be found at http://www.mediawiki.org/wiki/Extension:LDAP_Authentication.</summary>
        public string userDomain = "";
        /// <summary>Site title.</summary>
        public string name;
        /// <summary>MediaWiki version as string.</summary>
        public string generator;
        /// <summary>MediaWiki version as number.</summary>
        public float version;
        /// <summary>MediaWiki version as Version object.</summary>
        public Version ver;
        /// <summary>Rule of page title capitalization.</summary>
        public string capitalization;
        /// <summary>Short relative path to wiki pages (if such alias is set on the server).
        /// See "http://www.mediawiki.org/wiki/Manual:Short URL" for details.</summary>
        public string wikiPath;		// = "/wiki/";
        /// <summary>Relative path to "index.php" file on server.</summary>
        public string indexPath;	// = "/w/";
        /// <summary>User's watchlist. Should be loaded manually with FillFromWatchList function,
        /// if it is necessary.</summary>
        public PageList watchList;
        /// <summary>MediaWiki interface messages. Should be loaded manually with
        /// GetMediaWikiMessagesEx function, if it is necessary.</summary>
        public PageList messages;
        /// <summary>Regular expression to find redirection target.</summary>
        public Regex redirectRE;
        /// <summary>Regular expression to find links to pages in list in HTML source.</summary>
        public static Regex linkToPageRE1 =
            new Regex("<li><a href=\"[^\"]*?\" (?:class=\"mw-redirect\" )?title=\"([^\"]+?)\">");
        /// <summary>Alternative regular expression to find links to pages in HTML source.</summary>
        public static Regex linkToPageRE2 =
            new Regex("<a href=\"[^\"]*?\" title=\"([^\"]+?)\">\\1</a>");
        /// <summary>Alternative regular expression to find links to pages (mostly image and file
        /// pages) in HTML source.</summary>
        public Regex linkToPageRE3;
        /// <summary>Regular expression to find links to subcategories in HTML source
        /// of category page on sites that use "CategoryTree" MediaWiki extension.</summary>
        public static Regex linkToSubCategoryRE =
            new Regex(">([^<]+)</a></div>\\s*<div class=\"CategoryTreeChildren\"");
        /// <summary>Regular expression to find links to image pages in galleries
        /// in HTML source.</summary>
        public static Regex linkToImageRE =
            new Regex("<div class=\"gallerytext\">\n<a href=\"[^\"]*?\" title=\"([^\"]+?)\">");
        /// <summary>Regular expression to find titles in markup.</summary>
        public static Regex pageTitleTagRE = new Regex("<title>(.+?)</title>");
        /// <summary>Regular expression to find internal wiki links in markup.</summary>
        public static Regex wikiLinkRE = new Regex(@"\[\[(.+?)(\|.+?)?]]");
        /// <summary>Regular expression to find wiki category links.</summary>
        public Regex wikiCategoryRE;
        /// <summary>Regular expression to find wiki templates in markup.</summary>
        public static Regex wikiTemplateRE = new Regex(@"(?s)\{\{(.+?)((\|.*?)*?)}}");
        /// <summary>Regular expression to find embedded images and files in wiki markup.</summary>
        public Regex wikiImageRE;
        /// <summary>Regular expression to find links to sister wiki projects in markup.</summary>
        public static Regex sisterWikiLinkRE;
        /// <summary>Regular expression to find interwiki links in wiki markup.</summary>
        public static Regex iwikiLinkRE;
        /// <summary>Regular expression to find displayed interwiki links in wiki markup,
        /// like "[[:de:...]]".</summary>
        public static Regex iwikiDispLinkRE;
        /// <summary>Regular expression to find external web links in wiki markup.</summary>
        public static Regex webLinkRE =
            new Regex("(https?|t?ftp|news|nntp|telnet|irc|gopher)://([^\\s'\"<>]+)");
        /// <summary>Regular expression to find sections of text, that are explicitly
        /// marked as non-wiki with special tag.</summary>
        public static Regex noWikiMarkupRE = new Regex("(?is)<nowiki>(.*?)</nowiki>");
        /// <summary>A template for disambiguation page. If some unusual template is used in your
        /// wiki for disambiguation, then it must be set in this variable. Use "|" as a delimiter
        /// when enumerating several templates here.</summary>
        public string disambigStr;
        /// <summary>Regular expression to extract language code from site URL.</summary>
        public static Regex siteLangRE = new Regex(@"https?://(.*?)\.(.+?\..+)");
        /// <summary>Regular expression to extract edit session time attribute.</summary>
        public static Regex editSessionTimeRE1 =
            new Regex("value=\"([^\"]*?)\" name=['\"]wpEdittime['\"]");
        /// <summary>Regular expression to extract edit session time attribute.</summary>
        public static Regex editSessionTimeRE3 = new Regex(" touched=\"(.+?)\"");
        /// <summary>Regular expression to extract edit session token attribute.</summary>
        public static Regex editSessionTokenRE1 =
            new Regex("value=\"([^\"]*?)\" name=['\"]wpEditToken['\"]");
        /// <summary>Regular expression to extract edit session token attribute.</summary>
        public static Regex editSessionTokenRE2 =
            new Regex("name=['\"]wpEditToken['\"](?: type=\"hidden\")? value=\"([^\"]*?)\"");
        /// <summary>Regular expression to extract edit session token attribute.</summary>
        public static Regex editSessionTokenRE3 = new Regex(" edittoken=\"(.+?)\"");
        /// <summary>Site cookies.</summary>
        public CookieContainer cookies = new CookieContainer();
        /// <summary>XML name table for parsing XHTML documents from wiki site.</summary>
        public NameTable xhtmlNameTable = new NameTable();
        /// <summary>XML namespace URI of wiki site's XHTML version.</summary>
        public string xhtmlNSUri = "http://www.w3.org/1999/xhtml";
        /// <summary>XML namespace manager for parsing XHTML documents from wiki site.</summary>
        public XmlNamespaceManager xmlNS;
        /// <summary>Local namespaces.</summary>
        public Dictionary<string, string> namespaces = new Dictionary<string, string>();
        /// <summary>Default namespaces.</summary>
        public static Dictionary<string, string> wikiNSpaces = new Dictionary<string, string>();
        /// <summary>List of Wikimedia Foundation sites and according prefixes.</summary>
        public static Dictionary<string, string> WMSites = new Dictionary<string, string>();
        /// <summary>Built-in variables of MediaWiki software, used in brackets {{...}}.
        /// To be distinguished from templates.
        /// (see http://meta.wikimedia.org/wiki/Help:Magic_words).</summary>
        public static string[] mediaWikiVars;
        /// <summary>Built-in parser functions (and similar prefixes) of MediaWiki software, used
        /// like first ... in {{...:...}}. To be distinguished from templates.
        /// (see http://meta.wikimedia.org/wiki/Help:Magic_words).</summary>
        public static string[] parserFunctions;
        /// <summary>Built-in template modifiers of MediaWiki software
        /// (see http://meta.wikimedia.org/wiki/Help:Magic_words).</summary>
        public static string[] templateModifiers;
        /// <summary>Interwiki links sorting order, based on local language by first word.
        /// See http://meta.wikimedia.org/wiki/Interwiki_sorting_order for details.</summary>
        public static string[] iwikiLinksOrderByLocalFW;
        /// <summary>Interwiki links sorting order, based on local language.
        /// See http://meta.wikimedia.org/wiki/Interwiki_sorting_order for details.</summary>
        public static string[] iwikiLinksOrderByLocal;
        /// <summary>Interwiki links sorting order, based on latin alphabet by first word.
        /// See http://meta.wikimedia.org/wiki/Interwiki_sorting_order for details.</summary>
        public static string[] iwikiLinksOrderByLatinFW;
        /// <summary>Wikimedia Foundation sites and prefixes in one regex-escaped string
        /// with "|" as separator.</summary>
        public static string WMSitesStr;
        /// <summary>ISO 639-1 language codes, used as prefixes to identify Wikimedia
        /// Foundation sites, gathered in one regex-escaped string with "|" as separator.</summary>
        public static string WMLangsStr;
        /// <summary>Availability of "api.php" MediaWiki extension (bot interface).</summary>
        public bool botQuery;
        /// <summary>Versions of "api.php" MediaWiki extension (bot interface) modules.</summary>
        public Dictionary<string, string> botQueryVersions = new Dictionary<string, string>();
        /// <summary>Set of lists of pages, produced by bot interface.</summary>
        public static Dictionary<string, string> botQueryLists = new Dictionary<string, string>();
        /// <summary>Set of lists of parsed data, produced by bot interface.</summary>
        public static Dictionary<string, string> botQueryProps = new Dictionary<string, string>();
        /// <summary>Site language.</summary>
        public string language;
        /// <summary>Site language text direction.</summary>
        public string langDirection;
        /// <summary>Site's neutral (language) culture.</summary>
        public CultureInfo langCulture;
        /// <summary>Randomly chosen regional (non-neutral) culture for site's language.</summary>
        public CultureInfo regCulture;
        /// <summary>Windows Phone Isolated Storage</summary>
        private IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        /// <summary>This constructor is used to generate most Site objects.</summary>
        /// <param name="site">Wiki site's URI. It must point to the main page of the wiki, e.g.
        /// "http://en.wikipedia.org" or "http://127.0.0.1:80/w/index.php?title=Main_page".</param>
        /// <param name="userName">User name to log in.</param>
        /// <param name="userPass">Password.</param>
        /// <returns>Returns Site object.</returns>
        public Site(string site, string userName, string userPass)
            : this(site, userName, userPass, "") { }

        /// <summary>This constructor is used for LDAP authentication. Additional information can
        /// be found at "http://www.mediawiki.org/wiki/Extension:LDAP_Authentication".</summary>
        /// <param name="site">Wiki site's URI. It must point to the main page of the wiki, e.g.
        /// "http://en.wikipedia.org" or "http://127.0.0.1:80/w/index.php?title=Main_page".</param>
        /// <param name="userName">User name to log in.</param>
        /// <param name="userPass">Password.</param>
        /// <param name="userDomain">Domain for LDAP authentication.</param>
        /// <returns>Returns Site object.</returns>
        public Site(string site, string userName, string userPass, string userDomain)
        {
            this.site = site;
            this.userName = userName;
            this.userPass = userPass;
            this.userDomain = userDomain;
            Initialize();
        }

        /// <summary>This constructor uses default site, userName and password. The site URL and
        /// account data can be stored in UTF8-encoded "Defaults.dat" file in bot's "Cache"
        /// subdirectory.</summary>
        /// <returns>Returns Site object.</returns>
        public Site()
        {
            if (isf.FileExists("Cache" + System.IO.Path.DirectorySeparatorChar + "Defaults.dat") == true)
            {
                string[] lines = ReadAllLines(
                    "Cache" + System.IO.Path.DirectorySeparatorChar + "Defaults.dat", Encoding.UTF8);
                if (lines.GetUpperBound(0) >= 2)
                {
                    this.site = lines[0];
                    this.userName = lines[1];
                    this.userPass = lines[2];
                    if (lines.GetUpperBound(0) >= 3)
                        this.userDomain = lines[3];
                    else
                        this.userDomain = "";
                }
                else
                    throw new WikiUserException(
                        User.Msg("\"\\Cache\\Defaults.dat\" file is invalid."));
            }
            else
                throw new WikiUserException(User.Msg("\"\\Cache\\Defaults.dat\" file not found."));
            Initialize();
        }

        /// <summary>This function is getting file content into table of text lines.</summary>
        /// <param name="path">Path to the file(in the Insolated Storage)</param>
        /// <param name="encType">Type of encoding the text use</param>
        /// <returns>Table of text lines</returns>
        internal string[] ReadAllLines(string path, Encoding encType)
        {
            List<string> tempList = new List<string>();

            try
            {
                using (IsolatedStorageFileStream rawStream = isf.OpenFile("Cache" + System.IO.Path.DirectorySeparatorChar + "Defaults.dat", System.IO.FileMode.Open))
                {
                    StreamReader reader = new StreamReader(rawStream, encType);

                    for (int i = 0; reader.EndOfStream; i++)
                    {
                        tempList[i] = reader.ReadLine();
                    }

                    reader.Close();
                }
            }
            catch
            {
                // TODO: Do something with that Exception.
            }

            return tempList.ToArray();
        }

        /// <summary>This internal function establishes connection to site and loads general site
        /// info by the use of other functions. Function is called from the constructors.</summary>
        public void Initialize()
        {
            xmlNS = new XmlNamespaceManager(xhtmlNameTable);
            if (site.Contains("sourceforge"))
            {
                site = site.Replace("https://", "http://");
                GetPaths();
                xmlNS.AddNamespace("ns", xhtmlNSUri);
                LoadDefaults();
                LogInSourceForge();
                site = site.Replace("http://", "https://");
            }
            else if (site.Contains("wikia.com"))
            {
                GetPaths();
                xmlNS.AddNamespace("ns", xhtmlNSUri);
                LoadDefaults();
                LogInViaApi();
            }
            else
            {
                GetPaths();
                xmlNS.AddNamespace("ns", xhtmlNSUri);
                LoadDefaults();
                LogIn();
            }
            GetInfo();
            if (!User.isRunningOnMono)
                User.DisableCanonicalizingUriAsFilePath();	// .NET bug evasion
        }
    }
}
