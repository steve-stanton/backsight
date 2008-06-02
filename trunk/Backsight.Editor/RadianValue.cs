/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="01-SEP-2006" />
    /// <summary>
    /// An angle expressed in radians
    /// </summary>
    public struct RadianValue : IAngle
    {
        #region Statics

        /// <summary>
        /// Performs same function as modf function in C++
        /// </summary>
        /// <param name="a">The value to process</param>
        /// <param name="intPart">The integer portion (same sign as a)</param>
        /// <returns>The remainder (same sign as a)</returns>
        private static double Modf(double a, out double intPart)
        {
            double v = Math.Abs(a);
            int i = (int)v;
            intPart = (double)i;
            double rem = v - intPart;

            if (a<0)
            {
                intPart = -intPart;
                rem = -rem;
            }

            return rem;
        }

        /// <summary>
        /// Converts a value in radians into a string that represents the value in
        /// D-M-S format. This is the same as <c>AsString</c>, except that the string 
        /// will be abbreviated where possible. Thus, something like 23-00-00.000
        /// will come back as 23-0
        /// </summary>
        /// <param name="radianValue">The value in radians.</param>
        /// <returns>The formatted string</returns>
        public static string AsShortString(double radianValue)
        {
            string res;

            // Convert to decimal degrees (possibly signed).
            double sdeg = radianValue * MathConstants.RADTODEG;

            // Get the degrees, minutes, and seconds, all unsigned.
	        double deg, mins, secs, rem;
            rem = Modf(Math.Abs(sdeg), out deg);
            rem = Modf(rem*60.0, out mins);
            secs = rem*60.0;

            // Make sure we don't have max-values (i.e. 60's)
	        uint ideg  = (uint)deg;
	        uint imins = (uint)mins;

            if (Math.Abs(secs-60.0)<0.001) // 3 decimals formatted below
            {
		        secs -= 60.0;
		        secs = 0.0;
		        imins++;
	        }

	        if ( imins>=60 )
            {
		        imins = 0;
		        ideg++;
	        }

            // Create the return string, making sure that the sign is there.
	        if ( sdeg<0.0 )
                res = String.Format("-{0}-{1}", ideg, imins);
	        else
                res = String.Format("{0}-{1}", ideg, imins);

            // Append seconds if they'll show.
            if (secs>=0.001)
                res += String.Format("-{0:F3}", secs);

        	return res;
        }

        /// <summary>
        /// Converts a value in radians into a string that represents the value in D-M-S format.
        /// </summary>
        /// <param name="radianValue">The value in radians.</param>
        /// <returns>The formatted string</returns>
        public static string AsString(double radianValue)
        {
            // Convert to decimal degrees (possibly signed).
            double sdeg = radianValue * MathConstants.RADTODEG;

            // Get the degrees, minutes, and seconds, all unsigned.
	        double deg, mins, secs, rem;
            rem = Modf(Math.Abs(sdeg), out deg);
            rem = Modf(rem*60.0, out mins);
            secs = rem*60.0;

            // Make sure we don't have max-values (i.e. 60's)
	        uint ideg  = (uint)deg;
	        uint imins = (uint)mins;

            if (Math.Abs(secs-60.0)<0.001) // 3 decimals formatted below
            {
		        secs -= 60.0;
		        secs = 0.0;
		        imins++;
	        }

	        if ( imins>=60 )
            {
		        imins = 0;
		        ideg++;
	        }

            // Create the return string, making sure that the sign is there.
            string res = String.Format("{0:D}-{1:D2}-{2:F3}", ideg, imins, secs);
            if (sdeg<0.0)
                res = "-" + res;

            return res;
        }

        /// <summary>
        /// Attempt to convert an angle string from DMS format into radians.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <param name="rad">The result in radians.</param>
        /// <returns>True if string parsed OK. False if the string is no good (in that
        /// case, the radian value returned will be defined, but incorrect).</returns>
        public static bool TryParse(string str, out double rad)
        {
            rad = 0.0;

            try
            {
                // Get the number of characters up to the last character
                // that is not white space. Return if it's all white space.
                string s = str.TrimEnd(null);
                if (s.Length==0)
                    return false;

                // Extract digits for the degrees and convert to radians.
                int startIndex = 0;
                string ss = GetNumericSubstring(s, startIndex);
                double deg = Double.Parse(ss);
                rad = deg * MathConstants.DEGTORAD;

                //	Point to the character after the degrees (if any).
                startIndex = ss.Length;
                if (startIndex>=s.Length)
                    return true;

                // If next character is a hyphen, skip it.
                if (s[startIndex] == '-')
                {
                    startIndex++;
                    if (startIndex>=s.Length)
                        return true;
                }

                // Get the minutes. Return if we got a negated values (indicating
                // that there was an extra hyphen).
                ss = GetNumericSubstring(s, startIndex);
                double mins = Double.Parse(ss);
                if (mins<0.0)
                    return false;

                // Add or subtract the minutes, in radians.
                if (rad<0.0)
                    rad -= ((mins/60.0) * MathConstants.DEGTORAD);
                else
                    rad += ((mins/60.0) * MathConstants.DEGTORAD);

                // Point to the character after the minutes (if any).
                startIndex += ss.Length;
                if (startIndex>=s.Length)
                    return true;

                // If next character is a hyphen, skip it.
                if (s[startIndex] == '-')
                {
                    startIndex++;
                    if (startIndex>=s.Length)
                        return true;
                }

                // Get the seconds and confirm it isn't negated.
                ss = GetNumericSubstring(s, startIndex);
                double secs = Double.Parse(ss);
                if (secs<0.0)
                    return false;

                // Add or subtract the seconds, in radians.
                if (rad<0.0)
                    rad -= ((secs/3600.0) * MathConstants.DEGTORAD);
                else
                    rad += ((secs/3600.0) * MathConstants.DEGTORAD);

                // Parsing was successful so long as we got to the end of the supplied string.
                if (startIndex + ss.Length == s.Length)
                    return true;
            }

            catch { }
            return false;
        }

        /// <summary>
        /// Returns a portion of a string that contains numeric characters
        /// </summary>
        /// <param name="s">The string containing the numeric substring</param>
        /// <param name="startIndex">The array index of the first character
        /// to examine (>=0)"/></param>
        /// <returns>The numeric string starting at the supplied index (a
        /// blank string if the character at that position is not a number,
        /// a period, or a minus sign).</returns>
        /// <exception cref="IndexOutOfRangeException">If <c>startIndex</c>
        /// doesn't refer to a character within the string</exception>
        /// <remarks>This may not handle i18n (e.g. decimal places may
        /// actually be commas).</remarks>
        static string GetNumericSubstring(string s, int startIndex)
        {
            if (startIndex<0 || startIndex>=s.Length)
                throw new IndexOutOfRangeException();

            int nChar = 0;

            for (int i=startIndex; i<s.Length; i++)
            {
                char c = s[i];

                // Allow '-' character only at the start
                if ((c=='-' && i==startIndex) || c=='.' || Char.IsNumber(c))
                    nChar++;
                else
                    break;
            }

            if (nChar==0)
                return String.Empty;

            return s.Substring(startIndex, nChar);
        }

        #endregion

        #region Data

        /// <summary>
        /// The radian value.
        /// </summary>
        /// <remarks>
        /// Take care if you modify this struct to allow value changes. Since the
        /// <c>IAngle</c> interface is implemented, you may fail to appreciate when
        /// it is being passed around by value versus by reference (from what I can
        /// see, a struct will be passed by reference if it was declared to be an
        /// instance of the interface type).
        /// </remarks>
        readonly double m_Value;

        #endregion

        #region Constructors

        public RadianValue(double value)
        {
            m_Value = value;
        }

        /// <summary>
        /// Creates a new <c>RadianValue</c> during deserialization from XML.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public RadianValue(XmlContentReader reader)
        {
            string s = reader.ReadString("Value");
            if (!TryParse(s, out m_Value))
                throw new Exception("RadianValue: Cannot parse " + s);
        }

        #endregion

        /// <summary>
        /// The angle in radians
        /// </summary>
        public double Value
        {
            get { return m_Value; }
        }

        /// <summary>
        /// Override returns the result of <c>AsString</c> (a D-M-S string)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return AsString(m_Value);
        }

        public string AsShortString()
        {
            return AsShortString(m_Value);
        }

        #region IAngle Members

        /// <summary>
        /// The angle in radians.
        /// </summary>
        public double Radians
        {
            get { return m_Value; }
        }

        /// <summary>
        /// The angle in decimal degrees.
        /// </summary>
        public double Degrees
        {
            get { return (m_Value * MathConstants.RADTODEG); }
        }

        #endregion

        /// <summary>
        /// Converts the supplied value from radians to degrees
        /// </summary>
        /// <param name="radianValue">The value in radians to convert</param>
        /// <returns>The corresponding value in decimal degrees</returns>
        public static double ToDegrees(double radianValue)
        {
            return (radianValue * MathConstants.RADTODEG);
        }

        #region IXmlContent Members

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public void WriteContent(XmlContentWriter writer)
        {
            writer.WriteString("Value", AsString(m_Value));
        }

        /// <summary>
        /// Loads the content of this class using the supplied reader.
        /// </summary>
        /// <param name="reader">The reading tool.</param>
        /// <exception cref="InvalidOperationException">Always thrown. This struct involves
        /// readonly members, so the constructor that accepts am <c>XmlContentReader</c> should
        /// be used.</exception>
        public void ReadContent(XmlContentReader reader)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
