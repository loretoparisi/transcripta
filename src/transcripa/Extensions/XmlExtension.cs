/* 
 * transcripa
 * http://code.google.com/p/transcripa/
 * 
 * Copyright 2011, Bryan McKelvey
 * Licensed under the MIT license
 * http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Windows;
using System.Xml;
using System.Xml.Schema;

namespace transcripa
{
    public static partial class Extensions
    {
        #region LoadWithSchema overrides
        /// <summary>
        /// Loads the XML and XSD documents from the specified URL and sets up validation.
        /// </summary>
        /// <param name="xmlDoc">The XML document object for which to load the specified XML and XSD documents.</param>
        /// <param name="filename">URL for the file containing the XML document to load.</param>
        /// <param name="schemaUri">The URL that specifies the schema to load.</param>
        /// <param name="handler">The event handler if the XML document fails to validate.</param>
        /// <param name="failureCaption">The caption to show if loading the documents fails. Defaults to "XML Error".</param>
        public static void LoadWithSchema(this XmlDocument xmlDoc, string filename, string schemaUri,
            ValidationEventHandler handler)
        {
            LoadWithSchema(xmlDoc, filename, schemaUri, handler, "XML Error", true);
        }

        /// <summary>
        /// Loads the XML and XSD documents from the specified URL and sets up validation.
        /// </summary>
        /// <param name="xmlDoc">The XML document object for which to load the specified XML and XSD documents.</param>
        /// <param name="filename">URL for the file containing the XML document to load.</param>
        /// <param name="schemaUri">The URL that specifies the schema to load.</param>
        /// <param name="handler">The event handler if the XML document fails to validate.</param>
        /// <param name="failureCaption">The caption to show if loading the documents fails. Defaults to "XML Error".</param>
        public static void LoadWithSchema(this XmlDocument xmlDoc, string filename, string schemaUri,
            ValidationEventHandler handler, string failureCaption)
        {
            LoadWithSchema(xmlDoc, filename, schemaUri, handler, failureCaption, true);
        }
        #endregion

        /// <summary>
        /// Loads the XML and XSD documents from the specified URL and sets up validation.
        /// </summary>
        /// <param name="xmlDoc">The XML document object for which to load the specified XML and XSD documents.</param>
        /// <param name="filename">URL for the file containing the XML document to load.</param>
        /// <param name="schemaUri">The URL that specifies the schema to load.</param>
        /// <param name="handler">The event handler if the XML document fails to validate.</param>
        /// <param name="failureCaption">The caption to show if loading the documents fails. Defaults to "XML Error".</param>
        /// <param name="alertOnSchemaLoadFailure">Whether to alert the user if the schema fails to load. Defaults to true.</param>
        public static void LoadWithSchema(this XmlDocument xmlDoc, string filename, string schemaUri,
            ValidationEventHandler handler, string failureCaption, bool alertOnSchemaLoadFailure)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            try
            {
                settings.Schemas.Add(null, schemaUri);
            }
            catch (Exception e)
            {
                if (alertOnSchemaLoadFailure)
                {
                    MessageBox.Show(string.Format("Warning: {0}", e.Message), failureCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += handler;

            try
            {
                XmlReader reader = XmlReader.Create(filename, settings);
                xmlDoc.Load(reader);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Error: {0}", e.Message), failureCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Determines whether the current node has an attribute with the specified name.
        /// </summary>
        /// <param name="node">The node in which to search for an attribute.</param>
        /// <param name="name">The name of the attribute to find.</param>
        /// <returns>Whether the current node has an attribute with the specified name.</returns>
        public static bool HasAttribute(this XmlNode node, string name)
        {
            XmlNode attributeNode = node.Attributes[name];
            return (attributeNode != null);
        }

        /// <summary>
        /// Gets the value of an attribute of the specified name from the current node,
        /// returning a blank string if no such attribute exists.
        /// </summary>
        /// <param name="node">The node in which to search for an attribute.</param>
        /// <param name="name">The name of the attribute to find.</param>
        /// <returns>The attribute value.</returns>
        public static string GetAttribute(this XmlNode node, string name)
        {
            return (node.HasAttribute(name) ? node.Attributes[name].Value : "");
        }
    }
}
