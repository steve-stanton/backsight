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

namespace Backsight
{
	/// <written by="Steve Stanton" on="01-SEP-2006" />
    /// <summary>
    /// An ordinate (or dimension) that's stored in microns.
    /// </summary>
    public struct MicronValue : ILength
    {
        #region Data

        /// <summary>
        /// The value in microns.
        /// </summary>
        /// <remarks>
        /// Take care if you modify this struct to allow value changes. Since the
        /// <c>ILength</c> interface is implemented, you may fail to appreciate when
        /// it is being passed around by value versus by reference (from what I can
        /// see, a struct will be passed by reference if it was declared to be an
        /// instance of the interface type).
        /// </remarks>
        readonly long m_Value;

        #endregion

        #region Constructors

        public MicronValue(long value)
        {
            m_Value = value;
        }

        public MicronValue(double meters)
        {
            m_Value = Length.ToMicrons(meters);
        }

        /// <summary>
        /// Creates a new <c>MicronValue</c> during deserialization from XML.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public MicronValue(XmlContentReader reader)
        {
            m_Value = reader.ReadLong("Value");
        }

        #endregion

        public long Microns
        {
            get { return m_Value; }
        }

        public double Meters
        {
            get { return Length.ToMeters(m_Value); }
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
            writer.WriteLong("Value", m_Value);
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
