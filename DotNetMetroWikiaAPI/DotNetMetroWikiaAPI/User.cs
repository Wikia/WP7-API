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
using System.Collections.Generic;

namespace DotNetMetroWikiaAPI
{
    public class User
    {
        /// <summary>Desired user's messages language (ISO 639-1 language code).
        /// If not set explicitly, the language will be detected automatically.</summary>
        /// <example><code>User.userMessagesLang = "fr";</code></example>
        public static string userMessagesLang = null;

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
    }
}
