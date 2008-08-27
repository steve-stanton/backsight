/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// The database content that corresponds to an instance of <see cref="StraightLeg"/>
    /// </summary>
    class StraightLegContent : LegContent
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, for serialization.
        /// </summary>
        public StraightLegContent()
            : base(new StraightLeg())
        {
        }

        internal StraightLegContent(StraightLeg leg)
            : base(leg)
        {
        }

        #endregion

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            (base.Leg as StraightLeg).WriteLegContent(writer);
            base.WriteContent(writer);
        }

        /// <summary>
        /// Loads the content of this class.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            (base.Leg as StraightLeg).ReadLegContent(reader);
            base.ReadContent(reader);
        }

    }
}
