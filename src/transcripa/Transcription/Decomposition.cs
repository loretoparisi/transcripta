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
    /// A character decomposition formula for easier romanization of the form:
    /// [... + offset + SUMPRODUCT(prev * prevfactor)] % modulus / divisor + intercept
    /// </summary>
    public class Decomposition : IComparable<Decomposition>
    {
        /// <summary>
        /// Builds a character decomposition formula for easier romanization of the form: 
        /// [... + offset + SUMPRODUCT(prev * prevfactor)] % modulus / divisor + intercept
        /// </summary>
        /// <param name="offset">The offset to apply in the equation.</param>
        /// <param name="modulus">The modulus to apply in the equation.</param>
        /// <param name="divisor">The divisor to apply in the equation.</param>
        /// <param name="intercept">The intercept to apply in the equation.</param>
        /// <param name="rangeMin">The minimum Unicode value of a character to decompose.</param>
        /// <param name="rangeMax">The maximum Unicode value of a character to decompose.</param>
        /// <param name="order">The order to use when sorting all decompositions and returning the final result.</param>
        public Decomposition(int offset, int modulus, int divisor, int intercept, int order, int rangeMin, int rangeMax)
        {
            Offset = offset;
            Modulus = modulus;
            Divisor = divisor;
            Intercept = intercept;
            Order = order;
            RangeMin = rangeMin;
            RangeMax = rangeMax;
            PrevFactors = new Dictionary<int, int>();
        }

        /// <summary>
        /// Decomposes the provided character into a constituent characters.
        /// </summary>
        /// <param name="input">The input character to decompose.</param>
        /// <returns>A constituent character.</returns>
        public int Decompose(char input, List<int> previous)
        {
            int codepoint = (int)input;

            if (codepoint >= RangeMin && codepoint <= RangeMax)
            {
                int previousLength = previous.Count;

                for (int i = 0; i < previousLength; i++)
                {
                    if (PrevFactors.ContainsKey(i))
                    {
                        codepoint += previous[i] * PrevFactors[i];
                    }
                }

                codepoint += Offset;
                return ((Modulus == 1 ? codepoint : codepoint % Modulus) / Divisor + Intercept);
            }
            else return codepoint;
        }

        #region Properties
        /// <summary>
        /// The offset to apply in the equation.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The modulus to apply in the equation.
        /// </summary>
        public int Modulus { get; set; }

        /// <summary>
        /// The divisor to apply in the equation.
        /// </summary>
        public int Divisor { get; set; }

        /// <summary>
        /// The intercept to apply in the equation.
        /// </summary>
        public int Intercept { get; set; }

        /// <summary>
        /// The order to use when sorting all decompositions and returning the final result.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The minimum Unicode value of a character to decompose.
        /// </summary>
        public int RangeMin { get; set; }

        /// <summary>
        /// The maximum Unicode value of a character to decompose.
        /// </summary>
        public int RangeMax { get; set; }

        /// <summary>
        /// A list of factors to apply when taking the sum product of previous results
        /// </summary>
        public Dictionary<int, int> PrevFactors { get; set; }
        #endregion

        #region Comparison methods
        /// <summary>
        /// Compares this instance with a specified Decomposition object and indicates whether this instance
        /// precedes, follows, or appears in the same position in the sort order as the specified Decomposition object.
        /// </summary>
        /// <param name="other">An object that evaluates to a Decomposition object.</param>
        /// <returns>The comparison result.</returns>
        public int CompareTo(Decomposition other)
        {
            return other.Order.CompareTo(this.Order);
        }
        #endregion
    }
}