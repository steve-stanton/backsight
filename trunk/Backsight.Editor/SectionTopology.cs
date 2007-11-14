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
using System.Diagnostics;
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-OCT-2007" />
    /// <summary>
    /// Topology for a section of line.
    /// Base class for <see cref="SectionDivider"/> and <see cref="SectionOverlap"/>.
    /// </summary>
    /// <seealso cref="LineTopology"/>
    /// <seealso cref="SectionTopologyList"/>
    [Serializable]
    abstract class SectionTopology : ISection, IDivider
    {
        #region Class data

        /// <summary>
        /// The line this topological section coincides with. The geometry for this feature
        /// may be an instance of <see cref="SectionGeometry"/> (consequently, the geometry
        /// for this <c>SectionTopology</c> object may be a section on a section).
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// The start position for the topological section.
        /// </summary>
        readonly ITerminal m_From;

        /// <summary>
        /// The end position for the topological section.
        /// </summary>
        readonly ITerminal m_To;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SectionTopology</c>
        /// </summary>
        /// <param name="line">The line this topological section partially coincides with.</param>
        /// <param name="from">The start position for the topological section.</param>
        /// <param name="to">The end position for the topological section.</param>
        protected SectionTopology(LineFeature line, ITerminal from, ITerminal to)
        {
            m_Line = line;
            m_From = from;
            m_To = to;
        }

        #endregion

        /// <summary>
        /// The polygon ring to the left of this section of line
        /// </summary>
        abstract public Ring Left { get; set; } // IDivider

        /// <summary>
        /// The polygon ring to the right of this section of line
        /// </summary>
        abstract public Ring Right { get; set; } // IDivider

        /// <summary>
        /// The line the section partially coincides with.
        /// </summary>
        public LineFeature Line // IDivider
        {
            get { return m_Line; }
        }

        /// <summary>
        /// The start position for the section.
        /// </summary>
        public ITerminal From // ISection
        {
            get { return m_From; }
        }

        /// <summary>
        /// The end position for the section.
        /// </summary>
        public ITerminal To // ISection
        {
            get { return m_To; }
        }

        /// <summary>
        /// The geometry of the section of the line feature associated with this topology.
        /// </summary>
        public LineGeometry LineGeometry // IIntersectable, IDivider
        {
            get
            {
                // Note that the geometry associated with the boundary line may be an instance
                // of SectionGeometry (in that case, we need to return a section on a section).
                return m_Line.LineGeometry.SectionBase.Section(this);
            }
        }
        /// <summary>
        /// Implements <see cref="IDivider"/> method by returning <c>false</c>,
        /// indicating that this topology is not involved in any sort of overlap.
        /// The <see cref="SectionOverlap"/> class overrides.
        /// </summary>
        public virtual bool IsOverlap
        {
            get { return false; }
        }

        /// <summary>
        /// Override returns a string for use in debugging.
        /// </summary>
        /// <returns>A sting indicating the internal ID of the line involved, plus the two terminals.</returns>
        public override string ToString()
        {
            return String.Format("Section on line {0} from {1}-{2}", m_Line.DataId, m_From.ToString(), m_To.ToString());
        }
    }
}
