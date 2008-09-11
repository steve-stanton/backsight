// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Diagnostics;

namespace Ntx
{
    /// <summary>
    /// Miscellaneous utility functions for working with NTX.
    /// </summary>
    unsafe static class Util
    {
        /// <summary>
        /// Converts 4-byte integer array into character string.
        /// </summary>
        /// <param name="ibuf">Integer array to convert</param>
        /// <param name="nbyte">The number of bytes to convert (expected to be
        /// a multiple of 4)</param>
        /// <returns></returns>
        internal static string I4ch(int* ibuf, int nbyte)
        {
            Debug.Assert((nbyte%4) == 0);

            char[] res = new char[nbyte];
            for (int i=0, j=0; i<nbyte/4; i++, j+=4)
            {
                int val = ibuf[i];
#if (_SUN)
                res[j+3] = (char)(val & 0x000000FF);
                res[j+2] = (char)((val & 0x0000FF00) >> 8);
                res[j+1] = (char)((val & 0x00FF0000) >> 16);
                res[j]   = (char)((val & 0xFF000000) >> 24);
#else
                res[j]   = (char)(val & 0x000000FF);
                res[j+1] = (char)((val & 0x0000FF00) >> 8);
                res[j+2] = (char)((val & 0x00FF0000) >> 16);
                res[j+3] = (char)((val & 0xFF000000) >> 24);
#endif
            }

            // Force any embedded nulls into blanks.

            for (int i=0; i<nbyte; i++)
            {
                if (res[i]=='\0')
                    res[i] = ' ';
            }

            // Strip off trailing white space
            string s = new String(res);
            return s.TrimEnd(null);
        }

        /// <summary>
        /// Converts a string into integer format.
        /// </summary>
        /// <param name="ibuf">The string in integer format</param>
        /// <param name="numword">Number of words</param>
        /// <param name="sbuf">The string to convert</param>
        /*
        static void Chi4(int* ibuf, uint numword, string sbuf)
        {
        }
    */

        /// <summary>
        /// Reverses the bytes in a word
        /// </summary>
        /// <param name="word">The 32-bit integer containing the bytes to reverse</param>
        /// <returns>Integer holding the reversed bytes.</returns>
        static uint ReverseWord(uint word)
        {
            uint res = 0;
            res |= ((word & 0x000000FF) << 24);
            res |= ((word & 0x0000FF00) << 8);
            res |= ((word & 0x00FF0000) >> 8);
            res |= ((word & 0xFF000000) >> 24);
            return res;
            /*
	        CHARS*  src;            // Pointer to source byte
	        CHARS*  dst;            // Pointer to resultant byte
	        CHARS   res[4];         // The resultant bytes
	        size_t  n;              // Loop counter

	        for ( src=(CHARS*)&word, dst=&res[3], n=0; n<4; *dst--=*src++, n++ );
	        return *((INT4*)res);
             */
        }

        /// <summary>
        /// Converts two 4-byte integers into a double
        /// </summary>
        /// <param name="ibuf">The integer buffer to convert</param>
        /// <returns>The corresponding floating point number</returns>
        static double I4double(int* ibuf)
        {
        	return *((double*)ibuf); // obviously;)
        }

    }
}
