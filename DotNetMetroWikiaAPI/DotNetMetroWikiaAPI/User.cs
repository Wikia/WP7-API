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
using System.Reflection;

namespace DotNetMetroWikiaAPI
{
    public class User
    {
        /// <summary>Desired user's messages language (ISO 639-1 language code).
        /// If not set explicitly, the language will be detected automatically.</summary>
        /// <example><code>User.userMessagesLang = "fr";</code></example>
        public static string userMessagesLang = null;
        /// <summary>Title and description of web agent.</summary>
        public static readonly string userVer = "DotNetMetroWikiaAPI";
        /// <summary>Array, containing localized DotNetMetroWikiaAPI interface messages.</summary>
        public static Dictionary<string, string> messages =
            new Dictionary<string, string>();
        /// <summary>Content type for HTTP header of web client.</summary>
        public static readonly string webContentType = "application/x-www-form-urlencoded";
        /// <summary>If true, assembly is running on Mono framework. If false,
        /// it is running on original Microsoft .NET Framework. This variable is set
        /// automatically, just get it's value, don't change it.</summary>
        public static readonly bool isRunningOnMono = (Type.GetType("Mono.Runtime") != null);
        /// <summary>Initial state of HttpWebRequestElement.UseUnsafeHeaderParsing boolean
        /// configuration setting. 0 means true, 1 means false, 2 means unchanged.</summary>
        public static int unsafeHttpHeaderParsingUsed = 2;
        /// <summary>Number of times to retry user action in case of temporary connection failure or
        /// some other common net problems.</summary>
        public static int retryTimes = 3;

        /// <summary>The function gets localized (translated) form of the specified user
        /// interface message.</summary>
        /// <param name="message">Message itself, placeholders for substituted parameters are
        /// denoted in curly brackets like {0}, {1}, {2} and so on.</param>
        /// <returns>Returns localized form of the specified user interface message,
        /// or English form if localized form was not found.</returns>
        public static string Msg(string message)
        {
            if (userMessagesLang == "en")
                return message;
            try
            {
                return messages[message];
            }
            catch (KeyNotFoundException)
            {
                return message;
            }
        }

        /// <summary>This internal function switches unsafe HTTP headers parsing on or off.
        /// This is needed to ignore unimportant HTTP protocol violations,
        /// committed by misconfigured web servers.</summary>
        public static void SwitchUnsafeHttpHeaderParsing(bool enabled)
        {
            if (unsafeHttpHeaderParsingUsed == 2)
            {
                unsafeHttpHeaderParsingUsed = 1;
            }
            if (unsafeHttpHeaderParsingUsed == 1)
            {
                unsafeHttpHeaderParsingUsed = 0;
            }
            if (unsafeHttpHeaderParsingUsed == 0)
            {
                unsafeHttpHeaderParsingUsed = 2;
            }
            //System.Configuration.Configuration config =
            //    System.Configuration.ConfigurationManager.OpenExeConfiguration(
            //        System.Configuration.ConfigurationUserLevel.None);
            //System.Net.Configuration.SettingsSection section =
            //    (System.Net.Configuration.SettingsSection)config.GetSection("system.net/settings");
            //if (unsafeHttpHeaderParsingUsed == 2)
            //    unsafeHttpHeaderParsingUsed = section.HttpWebRequest.UseUnsafeHeaderParsing ? 1 : 0;
            //section.HttpWebRequest.UseUnsafeHeaderParsing = enabled;
            //config.Save();
            //System.Configuration.ConfigurationManager.RefreshSection("system.net/settings");
        }

        /// <summary>This auxiliary function counts the occurrences of specified string
        /// in specified text. This count is often needed, but strangely there is no
        /// such function in .NET Framework's String class.</summary>
        /// <param name="text">String to look in.</param>
        /// <param name="str">String to look for.</param>
        /// <param name="ignoreCase">Pass "true" if you need case-insensitive search.
        /// But remember that case-sensitive search is faster.</param>
        /// <returns>Returns the number of found occurrences.</returns>
        /// <example><code>int m = CountMatches("Bot Bot bot", "Bot", false); // =2</code></example>
        public static int CountMatches(string text, string str, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException("str");
            int matches = 0;
            int position = 0;
            StringComparison rule = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            while ((position = text.IndexOf(str, position, rule)) != -1)
            {
                matches++;
                position++;
            }
            return matches;
        }
    }
}
