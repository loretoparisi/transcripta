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
using System.Linq;
using System.Text;

namespace transcripa
{
    /// <summary>
    /// A romanization methodology for a string in a language that does not use the Latin alphabet.
    /// </summary>
    public partial class Romanization
    {
        /// <summary>
        /// A romanization methodology for a string in a language that does not use the Latin alphabet.
        /// <param name="name">The name of the methodology.</param>
        /// </summary>
        public Romanization(string name)
        {
            Name = name;
            Transliterations = new List<Transcription>();
        }

        /// <summary>
        /// Romanizes a string according to the rules of the methodology.
        /// </summary>
        /// <param name="input">The string to romanize.</param>
        /// <returns>The romanized string.</returns>
        public string Romanize(string input)
        {
            StringBuilder builder = new StringBuilder();
            input = " " + input + " ";
            int inputLength = input.Length;

            for (int i = 1; i < inputLength - 1; i++)
            {
                bool hasMatch = false;
                foreach (Transcription transliteration in Transliterations)
                {
                    Transcription.TranscriptionMatch match = transliteration.IsMatch(input, i);
                    if (match != null)
                    {
                        hasMatch = true;
                        builder.Append(match.Replacement);
                        i += match.Length - 1;
                        break;
                    }
                }

                if (!hasMatch) builder.Append(input[i]);
            }

            return builder.ToString();
        }

        #region Properties
        /// <summary>
        /// The name of the romanization methodology
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of all transliterations available under the romanization method.
        /// </summary>
        public List<Transcription> Transliterations { get; set; }
        #endregion
    }
}
