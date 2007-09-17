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
	/// <written by="Steve Stanton" on="12-MAR-1998" />
    /// <todo>This class needs to be re-designed, since the data members are defined to
    /// cater to the old C++ implementation.</todo>
    /// <summary>
    /// The key for spatial features. This is part of a feature's ID.
    /// </summary>
    [Serializable]
    public class Key
    {
        #region Statics

        /// <summary>
        /// Gets the check digit for a numeric key. 
        /// </summary>
        /// <param name="num">The numeric key</param>
        /// <returns></returns>
        public static uint GetCheckDigit(uint num)
        {
            uint val = num;
            uint total;			// The total for one iteration

            for (; val>9; val=total)
            {
                for (total=0; val!=0; val /= 10)
                {
                    total += (val % 10);
                }
            }

            return val;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The integer key value (used only if m_String is null).
        /// </summary>
        uint m_Num;

        /// <summary>
        /// The character key value (null if this is a numeric key).
        /// </summary>
        string m_String;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>Key</c> based on an existing feature ID.
        /// </summary>
        /// <param name="id">The ID object containing key. The ID may actually contain
        /// a key that is undefined.</param>
        Key (FeatureId id)
        {
            Key k = id.Key;

        	m_Num = k.NumericValue;
	        m_String = k.StringValue;
        }

        /// <summary>
        /// Creates a numeric <c>Key</c>
        /// </summary>
        /// <param name="key">The value for the key (C++ default=0)</param>
        internal Key (uint key)
        {
            m_Num = key;
            m_String = null;
        }

        /// <summary>
        /// Creates a new <c>Key</c> by parsing the supplied string.
        /// </summary>
        /// <param name="str">The key string</param>
        /// <param name="tryNumeric">Should we try to convert the key string into a numeric?
        /// Default=TRUE. For example, if you pass in the string "123456789", we could store
        /// this as a 4-byte integer key as opposed to a 9-character string. Specify FALSE
        /// if you need to preserve stuff like leading "0" chars. Strings that start with a
        /// "0" character will not be converted to numeric, since leading zeroes may mean
        /// something.
        /// </param>
        internal Key (string str, bool tryNumeric)
        {
            if (tryNumeric && str[0]!='0' )
            {
		        // Try to parse an unsigned integer value.
                uint val;
                if (UInt32.TryParse(str, out val))
                {
                    m_Num = val;
                    m_String = null;
                    return;
                }
	        }

        	// So it's gotta be a character key ...
            m_String = str;
            m_Num = 0;
        }

        #endregion

        bool IsNumeric
        {
            get { return (m_String==null); }
        }

        uint CharCount
        {
            get { return (m_String==null ? 0 : (uint)m_String.Length); }
        }

        /// <summary>
        /// Checks whether this key matches the key of a feature. 
        /// </summary>
        /// <param name="feat">The feature to check (may be null).</param>
        /// <returns>TRUE if keys match. FALSE if the supplied feature is null,
        /// or the feature does not have an ID.Or the keys don't match (obviously).
        /// </returns>
        bool IsSameKeyAs (Feature feat)
        {
            // No match if undefined feature
	        if (feat==null)
                return false;

            // No match if this key is undefined.
	        if (!this.IsDefined)
                return false;

            // Get the ID of the feature.
	        FeatureId id = feat.Id;

            // No match if the feature does not have an ID (even if this
            // key is not defined, it's still not a match).
	        if (id==null)
                return false;

            // Check whether the keys match.
	        return this.IsEqual(id.Key);
        }

        public string StringValue
        {
            get { return m_String; }
        }

        public uint NumericValue
        {
            get { return m_Num; }
        }

        bool IsEqual(Key that)
        {
            if (this.StringValue!=null && that.StringValue!=null)
                return this.StringValue.Equals(that.StringValue);
            else if (this.StringValue==null && that.StringValue==null)
                return this.NumericValue.Equals(that.NumericValue);
            else
                return false;
        }

        /// <summary>
        /// Return a formatted string representing the key.
        /// </summary>
        /// <returns></returns>
        internal string Format()
        {
            if (m_String!=null)
                return m_String;
            else
                return String.Format("{0}", m_Num);
        }

        /// <summary>
        /// Checks whether this key is defined.
        /// </summary>
        internal bool IsDefined
        {
            get { return (m_Num>0 || !String.IsNullOrEmpty(m_String)); }
        }

        /// <summary>
        /// Checks whether this key matches a string.
        /// </summary>
        /// <param name="keystr">The string to compare with.</param>
        /// <returns>TRUE if we have a match.</returns>
        bool IsMatch(string keystr)
        {
        	// If this key is a string, just do a string comparison.
	        if (m_String!=null)
                return m_String.Equals(keystr);

            // Try to convert specified key string into a numeric.
            uint val;
            if (UInt32.TryParse(keystr, out val))
                return m_Num.Equals(val);
            else
                return false;
        }

        /// <summary>
        /// Logs this key as part of a map comparison
        /// </summary>
        /// <param name="cc">The comparison tool</param>
        internal void Log(Comparison cc)
        {
            string s = String.Format("<key>\"{0}\"</key>\n", this.Format());
            cc.Write(s);
        }
    }
}
