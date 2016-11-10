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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace transcripa
{
    /// <summary>
    /// A class containing information on each language and the available transliterations.
    /// </summary>
    public partial class LanguageManager
    {
        private const string xmlPath = "Data/Languages.xml";
        private const string xmlSchemaPath = "Data/Languages.xsd";
        private XmlDocument xml = new XmlDocument();
        private Dictionary<string, Language> languages = new Dictionary<string, Language>();
        private string name = "";

        /// <summary>
        /// A class containing information on each language and the available transliterations.
        /// </summary>
        public LanguageManager()
        {
            XmlNodeList matches;
            xml.LoadWithSchema(xmlPath, xmlSchemaPath, Validation.ValidationEventHandler,
                "transcripa", !Properties.Settings.Default.HideErrors);
            matches = xml.SelectNodes("/Languages/Language");
            foreach (XmlNode match in matches)
            {
                string iso = match.GetAttribute("ISO");
                string name = match.GetAttribute("Name");

                languages.Add(name, new Language(iso, name));
            }
        }

        /// <summary>
        /// Loads the language from the XML.
        /// </summary>
        /// <param name="name">The name of the language to load.</param>
        /// <returns>The current object.</returns>
        public void Load(string name)
        {
            this.name = name;
            this.CurrentLanguage.Load(ref xml);
        }

        #region Properties
        /// <summary>
        /// The path to the transcriptions XML file.
        /// </summary>
        public string XmlPath
        {
            get { return xmlPath; }
        }

        /// <summary>
        /// An string array consisting of the names of languages available for transliteration.
        /// </summary>
        public string[] Languages
        {
            get {
                string[] keys = new string[languages.Keys.Count];
                languages.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        /// <summary>
        /// The number of languages available for transliteration.
        /// </summary>
        public int Length
        {
            get { return languages.Count; }
        }

        /// <summary>
        /// An accessor for the current language.
        /// </summary>
        public Language CurrentLanguage
        {
            get { return languages[name]; }
        }

        /// <summary>
        /// The name of the current language being transliterated.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion
    }
}
