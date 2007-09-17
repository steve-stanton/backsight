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
    /// <written by="Steve Stanton" on="05-JUL-2007" />
    /// <summary>
    /// A boundary between polygons, coinciding with a section of a line.
    /// </summary>
    [Serializable]
    class BoundarySection : Boundary, ISection
    {
        #region Class data

        /// <summary>
        /// The position at the start of this boundary (coincident with the
        /// line feature associated with this boundary).
        /// </summary>
        readonly ITerminal m_Start;

        /// <summary>
        /// The position at the end of this boundary (coincident with the
        /// line feature associated with this boundary).
        /// </summary>
        readonly ITerminal m_End;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new polygon <c>BoundarySection</c> that coincides with a
        /// section of the specified line.
        /// </summary>
        /// <param name="line">The line the boundary partially coincides with.</param>
        /// <param name="start">The start of the section</param>
        /// <param name="end">The end of the section</param>
        internal BoundarySection(LineFeature line, ITerminal start, ITerminal end)
            : base(line)
        {
            m_Start = start;
            m_End = end;
        }

        #endregion

        #region ISection Members

        /// <summary>
        /// The position of the start of this boundary.
        /// </summary>
        public ITerminal From
        {
            get { return m_Start; }
        }

        /// <summary>
        /// The position of the end of this boundary.
        /// </summary>
        public ITerminal To
        {
            get { return m_End; }
        }

        #endregion

        /// <summary>
        /// The geometry for this boundary, corresponding to a section of the geometry associated
        /// with the boundary line.
        /// </summary>
        /// <returns>The geometry for this boundary section</returns>
        internal override LineGeometry GetLineGeometry()
        {
            // Note that the geometry associated with the boundary line may be an instance
            // of SectionGeometry (in that case, we need to return a section on a section).
            return Line.LineGeometry.SectionBase.Section(this);
        }
    }
}
