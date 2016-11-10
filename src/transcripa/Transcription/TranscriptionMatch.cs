using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace transcripa
{
    public partial class Transcription
    {
        /// <summary>
        /// An object for holding data related to a match in transcription.
        /// </summary>
        public class TranscriptionMatch
        {
            public TranscriptionMatch(string replacement, int length)
            {
                Length = length;
                Replacement = replacement;
            }

            #region Properties
            public int Length { get; set; }
            public string Replacement { get; set; }
            #endregion
        }
    }
}
