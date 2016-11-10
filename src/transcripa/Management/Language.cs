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
using System.Xml;

namespace transcripa
{
    public partial class LanguageManager
    {
        public partial class Language
        {
            private string iso;
            private string name;
            private List<Transcription> transcriptions = new List<Transcription>();
            private List<Romanization> romanizations = new List<Romanization>();
            private List<Decomposition> decompositions = new List<Decomposition>();
            private int romanizationIndex = -1;
            private bool loaded = false;

            #region XML parsing XPaths
            private const string languageXPath = "/Languages/Language[@Name=\"{0}\"]";

            private const string transcriptionXPath = "IPA/Transcription";
            private const string transcriptionExXPath = "Exception";

            private const string romanizationXPath = "Romanization";
            private const string transliterationXPath = "Transliteration";
            private const string transliterationExXPath = "Exception";

            private const string decompositionXPath = "Decomposition/Constituent";
            private const string prevFactorsXPath = "PrevFactor";
            #endregion

            /// <summary>
            /// Constructs a language of the specified ISO code and full name.
            /// </summary>
            /// <param name="iso">The two-letter ISO code for the language.</param>
            /// <param name="name">The full English name of the language.</param>
            public Language(string iso, string name)
            {
                this.iso = iso;
                this.name = name;
            }

            /// <summary>
            /// Loads a language from the provided XML document reference.
            /// </summary>
            /// <param name="xml">A reference to the transliterations XML document.</param>
            public void Load(ref XmlDocument xml)
            {
                if (!loaded)
                {
                    XmlNode languageNode=xml.SelectSingleNode(string.Format(languageXPath,name));
                    XmlNodeList ipaMatches = languageNode.SelectNodes(transcriptionXPath);
                    XmlNodeList romanizationMatches = languageNode.SelectNodes(romanizationXPath);
                    XmlNodeList decompositionMatches = languageNode.SelectNodes(decompositionXPath);

                    foreach (XmlNode m in ipaMatches)
                    {
                        // A little lenient, in that it allows for all or none of these attributes to exist
                        XmlNodeList transcriptionExMatches = m.SelectNodes(transcriptionExXPath);
                        string original = m.GetAttribute("Original");
                        string replacement = m.GetAttribute("Replacement");
                        string prefix = m.GetAttribute("Prefix");
                        string suffix = m.GetAttribute("Suffix");
                        Transcription t = new Transcription(original, replacement, prefix, suffix);

                        foreach (XmlNode n in transcriptionExMatches)
                        {
                            string exOriginal = n.GetAttribute("Original");
                            string exReplacement = n.GetAttribute("Replacement");
                            string exPrefix = n.GetAttribute("Prefix");
                            string exSuffix = n.GetAttribute("Suffix");
                            t.AddException(exOriginal, exReplacement, exPrefix, exSuffix);
                        }

                        transcriptions.Add(t);
                    }

                    foreach (XmlNode m in romanizationMatches)
                    {
                        XmlNodeList transliterationMatches = m.SelectNodes(transliterationXPath);
                        string romanizationName = m.GetAttribute("Name");
                        Romanization r = new Romanization(romanizationName);

                        foreach (XmlNode n in transliterationMatches)
                        {
                            XmlNodeList transliterationExMatches = n.SelectNodes(transliterationExXPath);
                            string original = n.GetAttribute("Original");
                            string replacement = n.GetAttribute("Replacement");
                            string prefix = n.GetAttribute("Prefix");
                            string suffix = n.GetAttribute("Suffix");
                            Transcription t= new Transcription(original, replacement,prefix,suffix);

                            foreach (XmlNode o in transliterationExMatches)
                            {
                                string exOriginal = o.GetAttribute("Original");
                                string exReplacement = o.GetAttribute("Replacement");
                                string exPrefix = o.GetAttribute("Prefix");
                                string exSuffix = o.GetAttribute("Suffix");
                                t.AddException(exOriginal, exReplacement, exPrefix, exSuffix);
                            }
                            r.Transliterations.Add(t);
                        }

                        romanizations.Add(r);
                    }

                    foreach (XmlNode m in decompositionMatches)
                    {
                        XmlNodeList prevFactorsMatches = m.SelectNodes(prevFactorsXPath);

                        int offset = int.Parse(m.GetAttribute("Offset"));
                        int modulus = int.Parse(m.GetAttribute("Modulus"));
                        int divisor = int.Parse(m.GetAttribute("Divisor"));
                        int intercept = int.Parse(m.GetAttribute("Intercept"));
                        int order = int.Parse(m.GetAttribute("Order"));
                        int rangeMin = int.Parse(m.GetAttribute("RangeMin"));
                        int rangeMax = int.Parse(m.GetAttribute("RangeMax"));

                        Decomposition d = new Decomposition(offset, modulus, divisor, intercept, order, rangeMin, rangeMax);

                        foreach (XmlNode n in prevFactorsMatches)
                        {
                            int index = int.Parse(n.GetAttribute("Index"));
                            int multiplyBy = int.Parse(n.GetAttribute("MultiplyBy"));

                            d.PrevFactors.Add(index, multiplyBy);
                        }

                        decompositions.Add(d);
                    }
                }
            }

            /// <summary>
            /// Decomposes a string according to the decomposition rules for the language.
            /// </summary>
            /// <param name="input">The string to decompose.</param>
            /// <returns>The string decomposed into its constituent characters.</returns>
            public string Decompose(string input)
            {
                if (decompositions.Count == 0)
                {
                    return input;
                }
                else
                {
                    StringBuilder builder = new StringBuilder();

                    foreach (char letter in input)
                    {
                        List<int> results = new List<int>();
                        int codepoint = (int)letter;

                        foreach (Decomposition decomposition in decompositions)
                        {
                            if (codepoint >= decomposition.RangeMin && codepoint <= decomposition.RangeMax)
                            {
                                results.Add(decomposition.Decompose(letter, results));
                            }
                        }
                        if (results.Count == 0)
                        {
                            builder.Append(letter);
                        }
                        else
                        {
                            results.Sort();
                            foreach (int result in results)
                            {
                                builder.Append(char.ConvertFromUtf32(result));
                            }
                        }
                    }
                    return builder.ToString();
                }
            }

            #region Overrides
            /// <summary>
            /// Romanizes a string according to the decomposition rules for the language.
            /// </summary>
            /// <param name="input">The string to romanize.</param>
            /// <returns>The romanized string.</returns>
            public string Romanize(string input)
            {
                return Romanize(input, true);
            }

            /// <summary>
            /// Transcribes a string according to the rules for the language.
            /// </summary>
            /// <param name="input">The string to transcribe.</param>
            /// <returns>The IPA transcription.</returns>
            public string Transcribe(string input)
            {
                return Transcribe(input, true, true);
            }

            /// <summary>
            /// Transcribes a string according to the rules for the language.
            /// </summary>
            /// <param name="input">The string to transcribe.</param>
            /// <param name="isDecomposed">Whether the string has already been decomposed. Defaults to true.</param>
            /// <returns>The IPA transcription.</returns>
            public string Transcribe(string input, bool isDecomposed)
            {
                return Transcribe(input, isDecomposed, true);
            }
            #endregion

            /// <summary>
            /// Romanizes a string according to the decomposition rules for the language.
            /// </summary>
            /// <param name="input">The string to romanize.</param>
            /// <param name="isDecomposed">Whether the string has already been decomposed. Defaults to true.</param>
            /// <returns>The romanized string.</returns>
            public string Romanize(string input, bool isDecomposed)
            {
                if (romanizations.Count == 0 || romanizationIndex == -1)
                {
                    return input;
                }
                else
                {
                    if (!isDecomposed)
                    {
                        input = Decompose(input);
                    }

                    return Romanization.Romanize(input);
                }
            }

            /// <summary>
            /// Transcribes a string according to the rules for the language.
            /// </summary>
            /// <param name="input">The string to transcribe.</param>
            /// <param name="isDecomposed">Whether the string has already been decomposed. Defaults to true.</param>
            /// <param name="isRomanized">Whether the string has already been romanized. Defaults to true.</param>
            /// <returns>The IPA transcription.</returns>
            public string Transcribe(string input, bool isDecomposed, bool isRomanized)
            {
                if (!isDecomposed)
                {
                    input = Decompose(input);
                }
                if (!isRomanized)
                {
                    input = Romanize(input);
                }

                StringBuilder builder = new StringBuilder();
                input = " " + input + " ";
                int inputLength = input.Length;

                for (int i = 1; i < inputLength - 1; i++)
                {
                    bool hasMatch = false;
                    foreach (Transcription transcription in transcriptions)
                    {
                        Transcription.TranscriptionMatch match = transcription.IsMatch(input, i);
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

                return builder.ToString().Trim();
            }

            #region Properties
            /// <summary>
            /// The two-letter ISO code for the language.
            /// </summary>
            public string IsoCode
            {
                get { return iso; }
                set { iso = value; }
            }

            /// <summary>
            /// The full English name of the language.
            /// </summary>
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            /// <summary>
            /// Whether the transliterations for the language have been loaded or not.
            /// </summary>
            public bool Loaded
            {
                get { return loaded; }
            }

            /// <summary>
            /// The current romanization system.
            /// </summary>
            public Romanization Romanization
            {
                get { return romanizations[romanizationIndex]; }
            }

            /// <summary>
            /// The index of the current romanization system.
            /// </summary>
            public int RomanizationIndex
            {
                get { return romanizationIndex; }
                set { romanizationIndex = value; }
            }

            /// <summary>
            /// The name of the current romanization system.
            /// </summary>
            public string RomanizationName
            {
                get
                {
                    return Romanization.Name;
                }
                set
                {
                    int romanizationLength = romanizations.Count;

                    for (int i = 0; i < romanizationLength; i++)
                    {
                        if (value == romanizations[i].Name)
                        {
                            romanizationIndex = i;
                            break;
                        }
                    }
                }
            }

            /// <summary>
            /// A list of the romanization systems available for this language.
            /// </summary>
            public List<string> RomanizationNames
            {
                get
                {
                    List<string> rs = new List<string>();
                    foreach (Romanization r in romanizations)
                    {
                        rs.Add(r.Name);
                    }
                    return rs;
                }
            }
            #endregion
        }
    }
}
