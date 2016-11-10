/* 
 * transcripa
 * http://code.google.com/p/transcripa/
 * 
 * Copyright 2011, Bryan McKelvey
 * Licensed under the MIT license
 * http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace transcripa
{
    /// <summary>
    /// A list of accent marks to replace when typing another language.
    /// </summary>
    public partial class Accents
    {
        private const string xmlPath = "Data/Accents.xml";
        private const string xmlSchemaPath = "Data/Accents.xsd";
        private const string xpath = "/Letters/Letter/Accent";
        private XmlDocument xml = new XmlDocument();
        private Dictionary<string, string> accents = new Dictionary<string, string>();
        private int maxLength;

        /// <summary>
        /// A list of accent marks to replace when typing another language.
        /// </summary>
        public Accents()
        {
            XmlNodeList matches;
            xml.LoadWithSchema(xmlPath, xmlSchemaPath, Validation.ValidationEventHandler,
                "transcripa", !Properties.Settings.Default.HideErrors);

            matches = xml.SelectNodes(xpath);

            foreach (XmlNode match in matches)
            {
                string oldValue = match.GetAttribute("Old");
                string newValue = match.GetAttribute("New");
                accents.Add(oldValue, newValue);
                if (oldValue.Length > maxLength)
                {
                    maxLength = oldValue.Length;
                }
            }
        }

        /// <summary>
        /// Applies the available accents.
        /// </summary>
        /// <param name="input">The string to check for an accented equivalent.</param>
        /// <returns>The parsed string with any available accents added.</returns>
        public string Apply(string input)
        {
            string inputLower = input.ToLower();
            return (accents.ContainsKey(inputLower) ? accents[inputLower] : input);
        }

        #region Properties
        /// <summary>
        /// The length of the longest old value string.
        /// </summary>
        public int MaxLength
        {
            get { return maxLength; }
        }

        /// <summary>
        /// The path to the accents XML file.
        /// </summary>
        public string XmlPath
        {
            get { return xmlPath; }
        }

        /// <summary>
        /// The XPath expression used to enumerate the accent mark replacements.
        /// </summary>
        public string XPath
        {
            get { return xpath; }
        }
        #endregion Properties
    }
}
