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
using System.Text.RegularExpressions;

namespace transcripa
{
    /// <summary>
    /// A combination of letters and the conditions for their transliteration.
    /// </summary>
    public partial class Transcription
    {
        private List<TranscriptionException> exceptions = new List<TranscriptionException>();
        private const RegexOptions regexOptions = RegexOptions.IgnoreCase;
        private Regex originalRegex;
        private Regex prefixRegex;
        private Regex suffixRegex;
        private string replacement;

        /// <summary>
        /// A combination of letters and the conditions for their transliteration.
        /// </summary>
        /// <param name="original">The combination that will be replaced.</param>
        /// <param name="replacement">A string representing what the combination will be replaced with.</param>
        public Transcription(string original, string replacement)
        {
            this.originalRegex = new Regex("^(?:" + original + ")", regexOptions);
            this.replacement = replacement;
        }

        /// <summary>
        /// A combination of letters and the conditions for their transliteration.
        /// </summary>
        /// <param name="original">The combination that will be replaced.</param>
        /// <param name="replacement">A string representing what the combination will be replaced with.</param>
        /// <param name="prefix">The regular expression pattern for checking the prefix. Defaults to blank.</param>
        /// <param name="suffix">The regular expression pattern for checking the prefix. Defaults to blank.</param>
        public Transcription(string original, string replacement, string prefix, string suffix) :
            this(original, replacement)
        {
            this.originalRegex = new Regex("^(?:" + original + ")", regexOptions);
            this.replacement = replacement;
            
            // Keep prefix and suffix regex as null to make it easier to ignore in the Transcribe function
            if (prefix != "")
            {
                this.prefixRegex = new Regex("(?:" + prefix + ")$", regexOptions);
            }
            if (suffix != "")
            {
                this.suffixRegex = new Regex("^(?:" + suffix + ")", regexOptions);
            }
        }

        public void AddException(string original, string replacement, string prefix, string suffix)
        {
            exceptions.Add(new TranscriptionException(original, replacement, prefix, suffix));
        }

        /// <summary>
        /// Returns the length of the match on the raw string, returning 0 if no match is found.
        /// </summary>
        /// <param name="raw">The string to attempt to transcribe.</param>
        /// <param name="startIndex">The beginning index of the string from which to build a substring</param>
        /// <returns></returns>
        public TranscriptionMatch IsMatch(string raw, int startIndex)
        {
            string original = raw.Substring(startIndex);
            Match match = originalRegex.Match(original);
            int matchLength = match.Length;
            if (matchLength != 0)
            {
                string prefix = raw.Substring(0, startIndex);
                if (prefixRegex == null || prefixRegex.IsMatch(prefix))
                {
                    string suffix = raw.Substring(startIndex + matchLength);
                    if (suffixRegex == null || suffixRegex.IsMatch(suffix))
                    {
                        foreach (TranscriptionException exception in exceptions)
                        {
                            if (exception.IsMatch(original, prefix, suffix))
                            {
                                return new TranscriptionMatch(exception.Replacement, match.Length);
                            }
                        }
                        return new TranscriptionMatch(replacement, match.Length);
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }

        #region Properties
        /// <summary>
        /// A regular expression representing the combination that will be searched for and replaced.
        /// </summary>
        public Regex Original
        {
            get { return originalRegex; }
        }

        /// <summary>
        /// A string representing what the combination will be replaced with.
        /// </summary>
        public string Replacement {
            get { return replacement; }
            set { replacement = value; }
        }

        /// <summary>
        /// A regular expression representing the prefix for which to query.
        /// </summary>
        public Regex Prefix
        {
            get { return prefixRegex; }
        }

        /// <summary>
        /// A regular expression representing the suffix for which to query.
        /// </summary>
        public Regex Suffix
        {
            get { return suffixRegex; }
        }    


        /// <summary>
        /// The options
        /// </summary>
        public static RegexOptions RegexOptions
        {
            get { return regexOptions; }
        }
        #endregion
    }
}