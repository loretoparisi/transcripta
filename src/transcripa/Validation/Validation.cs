using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Schema;

namespace transcripa
{
    /// <summary>
    /// A static class containing XML validation routines
    /// </summary>
    public static class Validation
    {
        private const string caption = "transcripa";

        /// <summary>
        /// A handler for XML document validation.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">An object containing information on the error.</param>
        public static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (!Properties.Settings.Default.HideErrors)
            {
                bool isWarning = (e.Severity == XmlSeverityType.Warning);

                string message = string.Format("XML Validation {0}: {1}\n\nLine {2}, column {3} in {4}\n\n" +
                    "This may have no effect on you, but to ensure the program functions properly, it is recommended " +
                    "that you follow the XSD schema that came with the Languages and Accents XML documents.",
                    (isWarning ? "Warning" : "Error"),
                    e.Message, e.Exception.LineNumber, e.Exception.LinePosition,
                    e.Exception.SourceUri.Replace("file:///", ""));

                MessageBox.Show(message, caption, MessageBoxButton.OK,
                    (isWarning ? MessageBoxImage.Warning : MessageBoxImage.Error));
            }
        }
    }
}