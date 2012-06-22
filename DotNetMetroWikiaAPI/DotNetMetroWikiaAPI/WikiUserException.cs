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
    public class WikiUserException : System.Exception
    {
        /// <summary>Just overriding default constructor.</summary>
        /// <returns>Returns Exception object.</returns>
        public WikiUserException() { }
        /// <summary>Just overriding constructor.</summary>
        /// <returns>Returns Exception object.</returns>
        public WikiUserException(string message)
            : base(message) { /*Console.Beep(); Console.ForegroundColor = ConsoleColor.Red;*/ }
        /// <summary>Just overriding constructor.</summary>
        /// <returns>Returns Exception object.</returns>
        public WikiUserException(string message, System.Exception inner)
            : base(message, inner) { }
        /// <summary>Destructor is invoked automatically when exception object becomes
        /// inaccessible.</summary>
        ~WikiUserException() { }
    }
}
