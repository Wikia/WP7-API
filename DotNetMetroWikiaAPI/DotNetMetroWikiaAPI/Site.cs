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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using RestSharp;

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
        public string wikiPath = "/wiki/";		// = "/wiki/";
        /// <summary>Relative path to "index.php" file on server.</summary>
        public string indexPath = "/";	// = "/w/";
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
        /// <summary>Availability of "api.php" MediaWiki extension (user interface).</summary>
        public bool userQuery;
        /// <summary>Versions of "api.php" MediaWiki extension (user interface) modules.</summary>
        public Dictionary<string, string> userQueryVersions = new Dictionary<string, string>();
        /// <summary>Set of lists of pages, produced by user interface.</summary>
        public static Dictionary<string, string> userQueryLists = new Dictionary<string, string>();
        /// <summary>Set of lists of parsed data, produced by user interface.</summary>
        public static Dictionary<string, string> userQueryProps = new Dictionary<string, string>();
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
        /// <summary>Action which will be raised after succes in log in.</summary>
        private Action loggedIn;
        /// <summary>Action which will be raised after fail in log in.</summary>
        private Action problemInLogin;

        /// <summary>This constructor is used to generate most Site objects.</summary>
        /// <param name="site">Wiki site's URI. It must point to the main page of the wiki, e.g.
        /// "http://en.wikipedia.org" or "http://127.0.0.1:80/w/index.php?title=Main_page".</param>
        /// <param name="userName">User name to log in.</param>
        /// <param name="userPass">Password.</param>
        /// <returns>Returns Site object.</returns>
        public Site(string site, string userName, string userPass, Action loggedIn, Action problem)
            : this(site, userName, userPass, "", loggedIn, problem) { }

        /// <summary>This constructor is used for LDAP authentication. Additional information can
        /// be found at "http://www.mediawiki.org/wiki/Extension:LDAP_Authentication".</summary>
        /// <param name="site">Wiki site's URI. It must point to the main page of the wiki, e.g.
        /// "http://en.wikipedia.org" or "http://127.0.0.1:80/w/index.php?title=Main_page".</param>
        /// <param name="userName">User name to log in.</param>
        /// <param name="userPass">Password.</param>
        /// <param name="userDomain">Domain for LDAP authentication.</param>
        /// <returns>Returns Site object.</returns>
        public Site(string site, string userName, string userPass, string userDomain, Action loggedIn, Action problem)
        {
            this.site = site;
            this.userName = userName;
            this.userPass = userPass;
            this.userDomain = userDomain;
            this.loggedIn = loggedIn;
            problemInLogin = problem;
            Initialize();
        }

        /// <summary>This constructor uses default site, userName and password. The site URL and
        /// account data can be stored in UTF8-encoded "Defaults.dat" file in user's "Cache"
        /// subdirectory.</summary>
        /// <returns>Returns Site object.</returns>
        public Site(Action loggedIn)
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
            this.loggedIn = loggedIn;
            Initialize();
        }

        /// <summary>This internal function establishes connection to site and loads general site
        /// info by the use of other functions. Function is called from the constructors.</summary>
        public void Initialize()
        {
            xmlNS = new XmlNamespaceManager(xhtmlNameTable);
            bool isWikia;
            if (site.Contains("wikia.com"))
                isWikia = true;
            else
                isWikia = false;
            GetPaths(isWikia);
        }

        private static void asyncHelper(IAsyncResult asyncReceive)
        {
            return;
        }

        /// <summary>Gets path to "index.php", short path to pages (if present), and then
        /// saves paths to file.</summary>
        public void GetPaths(bool isWikia)
        {
            if (!site.StartsWith("http"))
                site = "http://" + site;
            if (User.CountMatches(site, "/", false) == 3 && site.EndsWith("/"))
                site = site.Substring(0, site.Length - 1);
            string filePathName = "Cache" + System.IO.Path.DirectorySeparatorChar +
                HttpUtility.UrlEncode(site.Replace("://", ".").Replace("/", ".")) + ".dat";

            if (isf.FileExists(filePathName) == true)
            {
                string[] lines = ReadAllLines(filePathName, Encoding.UTF8);
                if (lines.GetUpperBound(0) >= 4)
                {
                    wikiPath = lines[0];
                    indexPath = lines[1];
                    xhtmlNSUri = lines[2];
                    language = lines[3];
                    langDirection = lines[4];
                    if (lines.GetUpperBound(0) >= 5)
                        site = lines[5];
                    return;
                }
            }
            Console.WriteLine(User.Msg("Logging in..."));

            for (int errorCounter = 0; true; errorCounter++)
            {
                try
                {
                    var client = new RestClient(site);
                    var request = new RestRequest("index.php");

                    client.ExecuteAsync(request, (responce) => {
                        GetPaths2(responce, isWikia, filePathName);
                    });

                    break;
                }
                catch (WebException e)
                {
                    string message = e.Message;
                    if (Regex.IsMatch(message, ": \\(50[02349]\\) "))
                    {		// Remote problem
                        if (errorCounter > User.retryTimes)
                            throw;
                        Console.Error.WriteLine(message + " " + User.Msg("Retrying in 60 seconds."));
                        Thread.Sleep(60000);
                    }
                    else
                    {
                        Console.Error.WriteLine(User.Msg("Can't access the site.") + " " + message);
                        throw;
                    }
                }
            }
        }

        private void GetPaths2(IRestResponse e, bool isWikia, string filePathName)
        {
            string data = e.Content;
            string temp = e.ResponseUri.AbsoluteUri;
            if (User.CountMatches(temp, "/", false) > 2)
            {
                int lastSlash = temp.LastIndexOf('/');
                temp = temp.Substring(0, lastSlash);
            }
            site = temp;
            Regex wikiPathRE = new Regex("(?i)" + Regex.Escape(site) + "(/.+?/).+");
            Regex indexPathRE1 = new Regex("(?i)" + Regex.Escape(site) +
                "(/.+?/)index\\.php(\\?|/)");
            Regex indexPathRE2 = new Regex("(?i)href=\"(/[^\"\\s<>?]*?)index\\.php(\\?|/)");
            Regex indexPathRE3 = new Regex("(?i)wgScript=\"(/[^\"\\s<>?]*?)index\\.php");
            Regex xhtmlNSUriRE = new Regex("(?i)<html[^>]*( xmlns=\"(?'xmlns'[^\"]+)\")[^>]*>");
            Regex languageRE = new Regex("(?i)<html[^>]*( lang=\"(?'lang'[^\"]+)\")[^>]*>");
            Regex langDirectionRE = new Regex("(?i)<html[^>]*( dir=\"(?'dir'[^\"]+)\")[^>]*>");
            string mainPageUri = site;
            if (mainPageUri.Contains("/index.php?"))
                indexPath = indexPathRE1.Match(mainPageUri).Groups[1].ToString();
            else
                wikiPath = wikiPathRE.Match(mainPageUri).Groups[1].ToString();
            if (string.IsNullOrEmpty(indexPath) && string.IsNullOrEmpty(wikiPath) &&
                mainPageUri[mainPageUri.Length - 1] != '/' &&
                User.CountMatches(mainPageUri, "/", false) == 3)
                wikiPath = "/";
            string src = data;
            if (!site.Contains("wikia.com"))
                indexPath = indexPathRE2.Match(src).Groups[1].ToString();
            else
                indexPath = indexPathRE3.Match(src).Groups[1].ToString();
            xhtmlNSUri = xhtmlNSUriRE.Match(src).Groups["xmlns"].ToString();
            if (string.IsNullOrEmpty(xhtmlNSUri))
                xhtmlNSUri = "http://www.w3.org/1999/xhtml";
            language = languageRE.Match(src).Groups["lang"].ToString();
            langDirection = langDirectionRE.Match(src).Groups["dir"].ToString();
            if (!isf.DirectoryExists("Cache"))
                isf.CreateDirectory("Cache");
            WriteAllText(filePathName, wikiPath + "\r\n" + indexPath + "\r\n" + xhtmlNSUri +
                "\r\n" + language + "\r\n" + langDirection + "\r\n" + site, Encoding.UTF8);

            xmlNS.AddNamespace("ns", xhtmlNSUri);
            LoadDefaults(isWikia);
        }

        /// <summary>Retrieves metadata and local namespace names from site.</summary>
        public void GetInfo()
        {
            try
            {
                langCulture = new CultureInfo(language);
            }
            catch (Exception)
            {
                langCulture = new CultureInfo("");
            }
            if (langCulture.Equals(CultureInfo.CurrentUICulture.Parent))
                regCulture = CultureInfo.CurrentUICulture;
            else
            {
                try
                {
                    regCulture = (new CultureInfo(language));
                }
                catch (Exception)
                {
                    if (regCulture == null)
                        regCulture = CultureInfo.InvariantCulture;
                }
            }

            var client = new RestClient(site);
            var request = new RestRequest("index.php" + "?title=Special:Export/" +
                DateTime.Now.Ticks.ToString("x"));

            client.ExecuteAsync(request, (responce) =>
            {
                GetInfo2(responce);
            });
        }

        private void GetInfo2(IRestResponse e)
        {
            string src = e.Content;
            XmlReader reader = XmlReader.Create(new StringReader(src));
            //XmlTextReader reader = new XmlTextReader(new StringReader(src));
            //reader.Settings.IgnoreWhitespace = true;
            reader.ReadToFollowing("sitename");
            name = reader.ReadElementContentAsString();
            reader.ReadToFollowing("generator");
            generator = reader.ReadElementContentAsString();
            ver = new Version(Regex.Replace(generator, @"[^\d\.]", ""));
            float.TryParse(ver.ToString(), NumberStyles.AllowDecimalPoint,
                new CultureInfo("en-US"), out version);
            reader.ReadToFollowing("case");
            capitalization = reader.ReadElementContentAsString();
            namespaces.Clear();
            while (reader.ReadToFollowing("namespace"))
                namespaces.Add(reader.GetAttribute("key"),
                    HttpUtility.HtmlDecode(reader.ReadElementContentAsString()));
            reader.Close();
            namespaces.Remove("0");
            foreach (KeyValuePair<string, string> ns in namespaces)
            {
                if (!wikiNSpaces.ContainsKey(ns.Key) ||
                    ns.Key.ToString() == "4" || ns.Key.ToString() == "5")
                    wikiNSpaces[ns.Key] = ns.Value;
            }
            if (ver >= new Version(1, 14))
            {
                wikiNSpaces["6"] = "File";
                wikiNSpaces["7"] = "File talk";
            }
            wikiCategoryRE = new Regex(@"\[\[(?i)(((" + Regex.Escape(wikiNSpaces["14"].ToString()) +
                "|" + Regex.Escape(namespaces["14"].ToString()) + @"):(.+?))(\|(.+?))?)]]");
            wikiImageRE = new Regex(@"\[\[(?i)((File|Image" +
                "|" + Regex.Escape(namespaces["6"].ToString()) + @"):(.+?))(\|(.+?))*?]]");
            string namespacesStr = "";
            foreach (KeyValuePair<string, string> ns in namespaces)
                namespacesStr += Regex.Escape(ns.Value.ToString()) + "|";
            namespacesStr = namespacesStr.Replace("||", "|").Trim("|".ToCharArray());
            linkToPageRE3 = new Regex("<a href=\"[^\"]*?\" title=\"(" +
                Regex.Escape(namespaces["6"].ToString()) + ":[^\"]+?)\">");
            string redirectTag = "REDIRECT";
            switch (language)
            {		// Revised 2010-07-02 (MediaWiki 1.15.4)
                case "af": redirectTag += "|aanstuur"; break;
                case "ar": redirectTag += "|تحويل"; break;
                case "arz": redirectTag += "|تحويل|تحويل#"; break;
                case "be": redirectTag += "|перанакіраваньне"; break;
                case "be-x-old": redirectTag += "|перанакіраваньне"; break;
                case "bg": redirectTag += "|пренасочване|виж"; break;
                case "br": redirectTag += "|adkas"; break;
                case "bs": redirectTag += "|preusmjeri"; break;
                case "cs": redirectTag += "|přesměruj"; break;
                case "cu": redirectTag += "|прѣнаправлєниѥ"; break;
                case "cy": redirectTag += "|ail-cyfeirio|ailgyfeirio"; break;
                case "de": redirectTag += "|weiterleitung"; break;
                case "el": redirectTag += "|ανακατευθυνση"; break;
                case "eo": redirectTag += "|alidirektu"; break;
                case "es": redirectTag += "|redireccíon"; break;
                case "et": redirectTag += "|suuna"; break;
                case "eu": redirectTag += "|birzuzendu"; break;
                case "fa": redirectTag += "|تغییرمسیر"; break;
                case "fi": redirectTag += "|uudelleenohjaus|ohjaus"; break;
                case "fr": redirectTag += "|redirection"; break;
                case "ga": redirectTag += "|athsheoladh"; break;
                case "gl": redirectTag += "|redirección"; break;
                case "he": redirectTag += "|הפניה"; break;
                case "hr": redirectTag += "|preusmjeri"; break;
                case "hu": redirectTag += "|átirányítás"; break;
                case "hy": redirectTag += "|վերահղում"; break;
                case "id": redirectTag += "|alih"; break;
                case "is": redirectTag += "|tilvísun"; break;
                case "it": redirectTag += "|redirezione"; break;
                case "ja": redirectTag += "|転送|リダイレクト|転送|リダイレクト"; break;
                case "ka": redirectTag += "|გადამისამართება"; break;
                case "kk": redirectTag += "|ايداۋ|айдау|aýdaw"; break;
                case "km": redirectTag += "|បញ្ជូនបន្ត|ប្ដូរទីតាំងទៅ #ប្តូរទីតាំងទៅ"
                    + "|ប្ដូរទីតាំង|ប្តូរទីតាំង|ប្ដូរចំណងជើង"; break;
                case "ko": redirectTag += "|넘겨주기"; break;
                case "ksh": redirectTag += "|ömleidung"; break;
                case "lt": redirectTag += "|peradresavimas"; break;
                case "mk": redirectTag += "|пренасочување|види"; break;
                case "ml": redirectTag += "|аґ¤аґїаґ°аґїаґљаµЌаґљаµЃаґµаґїаґџаµЃаґ•" +
                    "|аґ¤аґїаґ°аґїаґљаµЌаґљаµЃаґµаґїаґџаґІаµЌвЂЌ"; break;
                case "mr": redirectTag += "|а¤ЄаҐЃа¤Ёа¤°аҐЌа¤Ёа¤їа¤°аҐЌа¤¦аҐ‡а¤¶а¤Ё"; break;
                case "mt": redirectTag += "|rindirizza"; break;
                case "mwl": redirectTag += "|ancaminar"; break;
                case "nds": redirectTag += "|wiederleiden"; break;
                case "nds-nl": redirectTag += "|deurverwiezing|doorverwijzing"; break;
                case "nl": redirectTag += "|doorverwijzing"; break;
                case "nn": redirectTag += "|omdiriger"; break;
                case "oc": redirectTag += "|redireccion"; break;
                case "pl": redirectTag += "|patrz|przekieruj|tam"; break;
                case "pt": redirectTag += "|redirecionamento"; break;
                case "ro": redirectTag += "|redirecteaza"; break;
                case "ru": redirectTag += "|перенаправление|перенапр"; break;
                case "sa": redirectTag += "|а¤ЄаҐЃа¤Ёа¤°аҐЌа¤Ёа¤їа¤¦аҐ‡а¤¶а¤Ё"; break;
                case "sd": redirectTag += "|چوريو"; break;
                case "si": redirectTag += "|а¶єа·…а·’а¶єа·ња¶ёа·”а·Ђ"; break;
                case "sk": redirectTag += "|presmeruj"; break;
                case "sl": redirectTag += "|preusmeritev"; break;
                case "sq": redirectTag += "|ridrejto"; break;
                case "sr": redirectTag += "|преусмери|preusmeri"; break;
                case "srn": redirectTag += "|doorverwijzing"; break;
                case "sv": redirectTag += "|omdirigering"; break;
                case "ta": redirectTag += "|а®µа®ґа®їа®®а®ѕа®±аЇЌа®±аЇЃ"; break;
                case "te": redirectTag += "|а°¦а°ѕа°°а°їа°®а°ѕа°°а±Ќа°Єа±Ѓ"; break;
                case "tr": redirectTag += "|yönlendİrme"; break;
                case "tt": redirectTag += "перенаправление|перенапр|yünältü"; break;
                case "uk": redirectTag += "|перенаправлення|перенаправление|перенапр"; break;
                case "vi": redirectTag += "|đổi|đổi"; break;
                case "vro": redirectTag += "|saadaq|suuna"; break;
                case "yi": redirectTag += "|ווייטערפירן|#הפניה"; break;
                default: redirectTag = "REDIRECT"; break;
            }
            redirectRE = new Regex(@"(?i)^#(?:" + redirectTag + @")\s*:?\s*\[\[(.+?)(\|.+)?]]",
                RegexOptions.Compiled);
            Console.WriteLine(User.Msg("Site: {0} ({1})"), name, generator);
            string userQueryUriStr = "api.php?version";
            try
            {
                var client = new RestClient(site);
                var request = new RestRequest(userQueryUriStr);

                client.ExecuteAsync(request, (responce) =>
                {
                    GetInfo3(responce, userQueryUriStr);
                });
            }
            catch (WebException)
            {
                userQuery = false;
            }
        }

        private void GetInfo3(IRestResponse e, string userQueryUriStr)
        {
            string respStr = e.Content;
            if (respStr.Contains("<title>MediaWiki API</title>"))
            {
                userQuery = true;
                Regex userQueryVersionsRE = new Regex(@"(?i)<b><i>\$" +
                    @"Id: (\S+) (\d+) (.+?) \$</i></b>");
                foreach (Match m in userQueryVersionsRE.Matches(respStr))
                    userQueryVersions[m.Groups[1].ToString()] = m.Groups[2].ToString();
                if (!userQueryVersions.ContainsKey("ApiMain.php") && ver > new Version(1, 17))
                {
                    // if versioning system is broken
                    userQueryVersions["ApiQueryCategoryMembers.php"] = "104449";
                    userQueryVersions["ApiQueryRevisions.php"] = "104449";
                }
            }

            if ((userQuery == false || !userQueryVersions.ContainsKey("ApiQueryCategoryMembers.php"))
                && ver < new Version(1, 16))
            {
                userQueryUriStr = site + indexPath + "query.php";
                try
                {
                    var client = new RestClient(site);
                    var request = new RestRequest(userQueryUriStr);

                    client.ExecuteAsync(request, (responce) =>
                    {
                        GetInfo4(responce);
                    });
                }
                catch (WebException)
                {
                    return;
                }
            }

            loggedIn();
        }

        private void GetInfo4(IRestResponse e)
        {
            string respStr = e.Content;
            if (respStr.Contains("<title>MediaWiki Query Interface</title>"))
            {
                userQuery = true;
                userQueryVersions["query.php"] = "Unknown";
            }

            loggedIn();
        }

        /// <summary>Loads default English namespace names for site.</summary>
        public void LoadDefaults(bool isWikia)
        {
            if (wikiNSpaces.Count != 0 && WMSites.Count != 0)
                return;

            string[] wikiNSNames = { "Media", "Special", "", "Talk", "User", "User talk", name,
				name + " talk", "Image", "Image talk", "MediaWiki", "MediaWiki talk", "Template",
				"Template talk", "Help", "Help talk", "Category", "Category talk" };
            for (int i = -2, j = 0; i < 16; i++, j++)
                wikiNSpaces.Add(i.ToString(), wikiNSNames[j]);
            wikiNSpaces.Remove("0");

            WMSites.Add("w", "wikipedia"); WMSites.Add("wikt", "wiktionary");
            WMSites.Add("b", "wikibooks"); WMSites.Add("n", "wikinews");
            WMSites.Add("q", "wikiquote"); WMSites.Add("s", "wikisource");
            foreach (KeyValuePair<string, string> s in WMSites)
                WMSitesStr += s.Key + "|" + s.Value + "|";

            // Revised 2010-07-02
            mediaWikiVars = new string[] { "currentmonth","currentmonthname","currentmonthnamegen",
				"currentmonthabbrev","currentday2","currentdayname","currentyear","currenttime",
				"currenthour","localmonth","localmonthname","localmonthnamegen","localmonthabbrev",
				"localday","localday2","localdayname","localyear","localtime","localhour",
				"numberofarticles","numberoffiles","sitename","server","servername","scriptpath",
				"pagename","pagenamee","fullpagename","fullpagenamee","namespace","namespacee",
				"currentweek","currentdow","localweek","localdow","revisionid","revisionday",
				"revisionday2","revisionmonth","revisionyear","revisiontimestamp","subpagename",
				"subpagenamee","talkspace","talkspacee","subjectspace","dirmark","directionmark",
				"subjectspacee","talkpagename","talkpagenamee","subjectpagename","subjectpagenamee",
				"numberofusers","rawsuffix","newsectionlink","numberofpages","currentversion",
				"basepagename","basepagenamee","urlencode","currenttimestamp","localtimestamp",
				"directionmark","language","contentlanguage","pagesinnamespace","numberofadmins",
				"currentday","numberofarticles:r","numberofpages:r","magicnumber",
				"numberoffiles:r", "numberofusers:r", "numberofadmins:r", "numberofactiveusers",
				"numberofactiveusers:r" };
            parserFunctions = new string[] { "ns:", "localurl:", "localurle:", "urlencode:",
				"anchorencode:", "fullurl:", "fullurle:",  "grammar:", "plural:", "lc:", "lcfirst:",
				"uc:", "ucfirst:", "formatnum:", "padleft:", "padright:", "#language:",
				"displaytitle:", "defaultsort:", "#if:", "#if:", "#switch:", "#ifexpr:",
				"numberingroup:", "pagesinns:", "pagesincat:", "pagesincategory:", "pagesize:",
				"gender:", "filepath:", "#special:", "#tag:" };
            templateModifiers = new string[] { ":", "int:", "msg:", "msgnw:", "raw:", "subst:" };
            // Revised 2010-07-02
            iwikiLinksOrderByLocalFW = new string[] {
				"ace", "af", "ak", "als", "am", "ang", "ab", "ar", "an", "arc",
				"roa-rup", "frp", "as", "ast", "gn", "av", "ay", "az", "id", "ms",
				"bm", "bn", "zh-min-nan", "nan", "map-bms", "jv", "su", "ba", "be",
				"be-x-old", "bh", "bcl", "bi", "bar", "bo", "bs", "br", "bug", "bg",
				"bxr", "ca", "ceb", "cv", "cs", "ch", "cbk-zam", "ny", "sn", "tum",
				"cho", "co", "cy", "da", "dk", "pdc", "de", "dv", "nv", "dsb", "na",
				"dz", "mh", "et", "el", "eml", "en", "myv", "es", "eo", "ext", "eu",
				"ee", "fa", "hif", "fo", "fr", "fy", "ff", "fur", "ga", "gv", "sm",
				"gd", "gl", "gan", "ki", "glk", "gu", "got", "hak", "xal", "ko",
				"ha", "haw", "hy", "hi", "ho", "hsb", "hr", "io", "ig", "ilo",
				"bpy", "ia", "ie", "iu", "ik", "os", "xh", "zu", "is", "it", "he",
				"kl", "kn", "kr", "pam", "ka", "ks", "csb", "kk", "kw", "rw", "ky",
				"rn", "sw", "kv", "kg", "ht", "ku", "kj", "lad", "lbe", "lo", "la",
				"lv", "to", "lb", "lt", "lij", "li", "ln", "jbo", "lg", "lmo", "hu",
				"mk", "mg", "ml", "krc", "mt", "mi", "mr", "arz", "mzn", "cdo",
				"mwl", "mdf", "mo", "mn", "mus", "my", "nah", "fj", "nl", "nds-nl",
				"cr", "ne", "new", "ja", "nap", "ce", "pih", "no", "nb", "nn",
				"nrm", "nov", "ii", "oc", "mhr", "or", "om", "ng", "hz", "uz", "pa",
				"pi", "pag", "pnb", "pap", "ps", "km", "pcd", "pms", "nds", "pl",
				"pnt", "pt", "aa", "kaa", "crh", "ty", "ksh", "ro", "rmy", "rm",
				"qu", "ru", "sah", "se", "sa", "sg", "sc", "sco", "stq", "st", "tn",
				"sq", "scn", "si", "simple", "sd", "ss", "sk", "sl", "cu", "szl",
				"so", "ckb", "srn", "sr", "sh", "fi", "sv", "tl", "ta", "kab",
				"roa-tara", "tt", "te", "tet", "th", "vi", "ti", "tg", "tpi",
				"tokipona", "tp", "chr", "chy", "ve", "tr", "tk", "tw", "udm", "uk",
				"ur", "ug", "za", "vec", "vo", "fiu-vro", "wa", "zh-classical",
				"vls", "war", "wo", "wuu", "ts", "yi", "yo", "zh-yue", "diq", "zea",
				"bat-smg", "zh", "zh-tw", "zh-cn"
			};
            iwikiLinksOrderByLocal = new string[] {
				"ace", "af", "ak", "als", "am", "ang", "ab", "ar", "an", "arc",
				"roa-rup", "frp", "as", "ast", "gn", "av", "ay", "az", "bm", "bn",
				"zh-min-nan", "nan", "map-bms", "ba", "be", "be-x-old", "bh", "bcl",
				"bi", "bar", "bo", "bs", "br", "bg", "bxr", "ca", "cv", "ceb", "cs",
				"ch", "cbk-zam", "ny", "sn", "tum", "cho", "co", "cy", "da", "dk",
				"pdc", "de", "dv", "nv", "dsb", "dz", "mh", "et", "el", "eml", "en",
				"myv", "es", "eo", "ext", "eu", "ee", "fa", "hif", "fo", "fr", "fy",
				"ff", "fur", "ga", "gv", "gd", "gl", "gan", "ki", "glk", "gu",
				"got", "hak", "xal", "ko", "ha", "haw", "hy", "hi", "ho", "hsb",
				"hr", "io", "ig", "ilo", "bpy", "id", "ia", "ie", "iu", "ik", "os",
				"xh", "zu", "is", "it", "he", "jv", "kl", "kn", "kr", "pam", "krc",
				"ka", "ks", "csb", "kk", "kw", "rw", "ky", "rn", "sw", "kv", "kg",
				"ht", "ku", "kj", "lad", "lbe", "lo", "la", "lv", "lb", "lt", "lij",
				"li", "ln", "jbo", "lg", "lmo", "hu", "mk", "mg", "ml", "mt", "mi",
				"mr", "arz", "mzn", "ms", "cdo", "mwl", "mdf", "mo", "mn", "mus",
				"my", "nah", "na", "fj", "nl", "nds-nl", "cr", "ne", "new", "ja",
				"nap", "ce", "pih", "no", "nb", "nn", "nrm", "nov", "ii", "oc",
				"mhr", "or", "om", "ng", "hz", "uz", "pa", "pi", "pag", "pnb",
				"pap", "ps", "km", "pcd", "pms", "tpi", "nds", "pl", "tokipona",
				"tp", "pnt", "pt", "aa", "kaa", "crh", "ty", "ksh", "ro", "rmy",
				"rm", "qu", "ru", "sah", "se", "sm", "sa", "sg", "sc", "sco", "stq",
				"st", "tn", "sq", "scn", "si", "simple", "sd", "ss", "sk", "cu",
				"sl", "szl", "so", "ckb", "srn", "sr", "sh", "su", "fi", "sv", "tl",
				"ta", "kab", "roa-tara", "tt", "te", "tet", "th", "ti", "tg", "to",
				"chr", "chy", "ve", "tr", "tk", "tw", "udm", "bug", "uk", "ur",
				"ug", "za", "vec", "vi", "vo", "fiu-vro", "wa", "zh-classical",
				"vls", "war", "wo", "wuu", "ts", "yi", "yo", "zh-yue", "diq", "zea",
				"bat-smg", "zh", "zh-tw", "zh-cn"
			};
            iwikiLinksOrderByLatinFW = new string[] {
				"ace", "af", "ak", "als", "am", "ang", "ab", "ar", "an", "arc",
				"roa-rup", "frp", "arz", "as", "ast", "gn", "av", "ay", "az", "id",
				"ms", "bg", "bm", "zh-min-nan", "nan", "map-bms", "jv", "su", "ba",
				"be", "be-x-old", "bh", "bcl", "bi", "bn", "bo", "bar", "bs", "bpy",
				"br", "bug", "bxr", "ca", "ceb", "ch", "cbk-zam", "sn", "tum", "ny",
				"cho", "chr", "co", "cy", "cv", "cs", "da", "dk", "pdc", "de", "nv",
				"dsb", "na", "dv", "dz", "mh", "et", "el", "eml", "en", "myv", "es",
				"eo", "ext", "eu", "ee", "fa", "hif", "fo", "fr", "fy", "ff", "fur",
				"ga", "gv", "sm", "gd", "gl", "gan", "ki", "glk", "got", "gu", "ha",
				"hak", "xal", "haw", "he", "hi", "ho", "hsb", "hr", "hy", "io",
				"ig", "ii", "ilo", "ia", "ie", "iu", "ik", "os", "xh", "zu", "is",
				"it", "ja", "ka", "kl", "kr", "pam", "krc", "csb", "kk", "kw", "rw",
				"ky", "rn", "sw", "km", "kn", "ko", "kv", "kg", "ht", "ks", "ku",
				"kj", "lad", "lbe", "la", "lv", "to", "lb", "lt", "lij", "li", "ln",
				"lo", "jbo", "lg", "lmo", "hu", "mk", "mg", "mt", "mi", "cdo",
				"mwl", "ml", "mdf", "mo", "mn", "mr", "mus", "my", "mzn", "nah",
				"fj", "ne", "nl", "nds-nl", "cr", "new", "nap", "ce", "pih", "no",
				"nb", "nn", "nrm", "nov", "oc", "mhr", "or", "om", "ng", "hz", "uz",
				"pa", "pag", "pap", "pi", "pcd", "pms", "nds", "pnb", "pl", "pt",
				"pnt", "ps", "aa", "kaa", "crh", "ty", "ksh", "ro", "rmy", "rm",
				"qu", "ru", "sa", "sah", "se", "sg", "sc", "sco", "sd", "stq", "st",
				"tn", "sq", "si", "scn", "simple", "ss", "sk", "sl", "cu", "szl",
				"so", "ckb", "srn", "sr", "sh", "fi", "sv", "ta", "tl", "kab",
				"roa-tara", "tt", "te", "tet", "th", "ti", "vi", "tg", "tokipona",
				"tp", "tpi", "chy", "ve", "tr", "tk", "tw", "udm", "uk", "ur", "ug",
				"za", "vec", "vo", "fiu-vro", "wa", "vls", "war", "wo", "wuu", "ts",
				"yi", "yo", "diq", "zea", "zh", "zh-tw", "zh-cn", "zh-classical",
				"zh-yue", "bat-smg"
			};
            userQueryLists.Add("allpages", "ap"); userQueryLists.Add("alllinks", "al");
            userQueryLists.Add("allusers", "au"); userQueryLists.Add("backlinks", "bl");
            userQueryLists.Add("categorymembers", "cm"); userQueryLists.Add("embeddedin", "ei");
            userQueryLists.Add("imageusage", "iu"); userQueryLists.Add("logevents", "le");
            userQueryLists.Add("recentchanges", "rc"); userQueryLists.Add("usercontribs", "uc");
            userQueryLists.Add("watchlist", "wl"); userQueryLists.Add("exturlusage", "eu");
            userQueryProps.Add("info", "in"); userQueryProps.Add("revisions", "rv");
            userQueryProps.Add("links", "pl"); userQueryProps.Add("langlinks", "ll");
            userQueryProps.Add("images", "im"); userQueryProps.Add("imageinfo", "ii");
            userQueryProps.Add("templates", "tl"); userQueryProps.Add("categories", "cl");
            userQueryProps.Add("extlinks", "el"); userQueryLists.Add("search", "sr");

            if (isWikia)
                LogInViaApi();
            else
                LogIn();
        }

        /// <summary>Logs in via api.php and retrieves cookies.</summary>
        public void LogInViaApi()
        {
            string postData = string.Format("lgname={0}&lgpassword={1}&lgdomain={2}",
                HttpUtility.UrlEncode(userName), HttpUtility.UrlEncode(userPass),
                HttpUtility.UrlEncode(userDomain));
            PostDataAndGetResultHTM(site + indexPath +
                "api.php?action=login&format=xml", postData, true, false, LogInViaApi2);
        }

        /// <summary>Logs in and retrieves cookies.</summary>
        public void LogIn()
        {
            PostDataAndGetResultHTM(site + indexPath +
                "index.php?title=Special:Userlogin", "", true, true, LogIn2);
        }

        /// <summary>This internal function gets the hypertext markup (HTM) of wiki-page.</summary>
        /// <param name="pageURL">Absolute or relative URL of page to get.</param>
        /// <returns>Returns HTM source code.</returns>
        public void GetPageHTM(string pageURL, Action<IRestResponse, string, object[]> callback)
        {
            PostDataAndGetResultHTM(pageURL, "", false, true, callback);
        }

        /// <summary>This internal function posts specified string to requested resource
        /// and gets the result hypertext markup (HTM).</summary>
        /// <param name="pageURL">Absolute or relative URL of page to get.</param>
        /// <param name="postData">String to post to site with web request.</param>
        /// <returns>Returns code of hypertext markup (HTM).</returns>
        public void PostDataAndGetResultHTM(string pageURL, string postData, Action<IRestResponse, string, object[]> callback)
        {
            PostDataAndGetResultHTM(pageURL, postData, false, true, callback);
        }

        /// <summary>This internal function posts specified string to requested resource
        /// and gets the result hypertext markup (HTM).</summary>
        /// <param name="pageURL">Absolute or relative URL of page to get.</param>
        /// <param name="postData">String to post to site with web request.</param>
        /// <param name="getCookies">If set to true, gets cookies from web response and
        /// saves it in site.cookies container.</param>
        /// <param name="allowRedirect">Allow auto-redirection of web request by server.</param>
        /// <returns>Returns code of hypertext markup (HTM).</returns>
        public void PostDataAndGetResultHTM(string pageURL, string postData, bool getCookies,
            bool allowRedirect, Action<IRestResponse, string, object[]> callback)
        {
            if (string.IsNullOrEmpty(pageURL))
                throw new WikiUserException(User.Msg("No URL specified."));
            if (pageURL.Contains("?"))
            {
                int inqm = pageURL.IndexOf('?');
                string addedParams = pageURL.Substring(inqm + 1);

                int apiPhpIn = pageURL.IndexOf("api.php");
                pageURL = pageURL.Substring(0, apiPhpIn);

                string[] newParams = addedParams.Split('&');
                foreach (string str in newParams)
                {
                    if (!postData.Contains(str))
                        postData += "&" + str;
                }
            }
            var client = new RestClient(pageURL);

            client.CookieContainer = cookies;

            var request = new RestRequest("api.php", Method.POST);
            string[] postDataTable = postData.Split('&');
            foreach (string pair in postDataTable)
            {
                int eqm = pair.IndexOf('=');
                request.AddParameter(pair.Substring(0, eqm), pair.Substring(eqm + 1));
            }
            request.AddHeader("ContentType", "application/x-www-form-urlencoded");

            for (int errorCounter = 0; true; errorCounter++)
            {
                try
                {
                    client.ExecuteAsync(request, (responce) =>
                    {
                        foreach (RestSharp.RestResponseCookie buiscuit in responce.Cookies)
                        {
                            cookies.Add(new Uri(pageURL), new Cookie(buiscuit.Name, buiscuit.Value, buiscuit.Path, buiscuit.Domain));
                        }
                        callback(responce, postData, null);
                    });
                    break;
                }
                catch (WebException e)
                {
                    string message = e.Message;
                    if (Regex.IsMatch(message, ": \\(50[02349]\\) "))
                    {		// Remote problem
                        if (errorCounter > User.retryTimes)
                            throw;
                        Console.Error.WriteLine(message + " " + User.Msg("Retrying in 60 seconds."));
                        Thread.Sleep(5000);
                    }
                    else
                        throw;
                }
            }
        }

        private void LogInViaApi2(IRestResponse e, string postData, params object[] args)
        {
            string respStr = e.Content;
            if (respStr.Contains("result=\"Success\""))
            {
                Console.WriteLine(User.Msg("Logged in as {0}."), userName);
                return;
            }

            int tokenPos = respStr.IndexOf("token=\"");
            if (tokenPos < 1)
            {
                wikiNSpaces = new Dictionary<string, string>();
                WMSites = new Dictionary<string, string>();
                userQueryLists = new Dictionary<string, string>();
                userQueryProps = new Dictionary<string, string>();
                problemInLogin();
                return;
            }
            string loginToken = respStr.Substring(tokenPos + 7, 32);
            postData += "&lgtoken=" + HttpUtility.UrlEncode(loginToken);
            PostDataAndGetResultHTM(site + indexPath +
                "api.php?action=login&format=xml", postData, true, false, LogInViaApi3);
        }

        private void LogInViaApi3(IRestResponse e, string postData, params object[] args)
        {
            string respStr = e.Content;

            if (!respStr.Contains("result=\"Success\""))
            {
                wikiNSpaces = new Dictionary<string, string>();
                WMSites = new Dictionary<string, string>();
                userQueryLists = new Dictionary<string, string>();
                userQueryProps = new Dictionary<string, string>();
                problemInLogin();
                return;
            }
            Console.WriteLine(User.Msg("Logged in as {0}."), userName);
            GetInfo();
        }

        private void LogIn2(IRestResponse e, string notUsed, params object[] args)
        {
            string loginPageSrc = e.Content;
            string loginToken = "";
            int loginTokenPos = loginPageSrc.IndexOf(
                "<input type=\"hidden\" name=\"wpLoginToken\" value=\"");
            if (loginTokenPos != -1)
                loginToken = loginPageSrc.Substring(loginTokenPos + 48, 32);

            string postData = string.Format("wpName={0}&wpPassword={1}&wpDomain={2}" +
                "&wpLoginToken={3}&wpRemember=1&wpLoginattempt=Log+in",
                HttpUtility.UrlEncode(userName), HttpUtility.UrlEncode(userPass),
                HttpUtility.UrlEncode(userDomain), HttpUtility.UrlEncode(loginToken));
            PostDataAndGetResultHTM(site + indexPath +
                "index.php?title=Special:Userlogin&action=submitlogin&type=login",
                postData, true, false, LogIn3);
        }

        private void LogIn3(IRestResponse e, string postData, params object[] args)
        {
            string respStr = e.Content;

            if (respStr.Contains("<div class=\"errorbox\">"))
            {
                wikiNSpaces = new Dictionary<string, string>();
                WMSites = new Dictionary<string, string>();
                userQueryLists = new Dictionary<string, string>();
                userQueryProps = new Dictionary<string, string>();
                problemInLogin();
                return;
            }
            Console.WriteLine(User.Msg("Logged in as {0}."), userName);
            GetInfo();
        }

        // Another methods added to make it work on WP7

        /// <summary>Opens a text file, reads all lines of the file, and then closes the
        /// file.</summary>
        /// <param name="path">The file(in the Insolated Storage) to open for reading.</param>
        /// <param name="encType">Type encoding used in the file.</param>
        /// <returns>A string array containing all lines of the file.</returns>
        private string[] ReadAllLines(string path, Encoding encType)
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

        /// <summary>Creates a new file, write the contents to the file, and then closes
        /// the file. If the target file already exists, it is overwritten</summary>
        /// <param name="path">The file(in the Insolated Storage) to write to.</param>
        /// <param name="contents">The string to write to the file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        private void WriteAllText(string path, string contents, Encoding encoding)
        {
            List<string> tempList = new List<string>();

            using (isf)
            {
                if (isf.FileExists(path))
                    isf.DeleteFile(path);

                using (IsolatedStorageFileStream rawStream = isf.CreateFile(path))
                {
                    StreamWriter writer = new StreamWriter(rawStream);

                    //Probably would work with that encoding.
                    writer.WriteLine(contents, encoding);

                    writer.Close();
                }
            }
        }
    }
}
