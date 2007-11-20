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
using System.Collections.Generic;
using System.Diagnostics;
using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-OCT-2007" />
    /// <summary>
    /// Topology that relates to a complete line.
    /// Base class for <see cref="LineDivider"/> and <see cref="LineOverlap"/>.
    /// </summary>
    /// <seealso cref="SectionTopology"/>
    [Serializable]
    abstract class LineTopology : Topology, IDivider
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>LineTopology</c> that relates to a complete line.
        /// Base class for <see cref="LineDivider"/> and <see cref="LineOverlap"/>.
        /// </summary>
        /// <param name="line">The line the topology relates to.</param>
        /// <seealso cref="SectionTopology"/>
        protected LineTopology(LineFeature line)
            : base(line)
        {
        }

        #endregion

        /// <summary>
        /// The polygon ring to the left of the line.
        /// </summary>
        abstract public Ring Left { get; set; } // IDivider

        /// <summary>
        /// The polygon ring to the right of the line.
        /// </summary>
        abstract public Ring Right { get; set; } // IDivider

        /// <summary>
        /// The geometry of the line that is associated with this topology.
        /// </summary>
        public LineGeometry LineGeometry // IIntersectable, IDivider
        {
            get { return Line.LineGeometry; }
        }

        /// <summary>
        /// The position of the start of this divider (coincides with the start
        /// of the associated line)
        /// </summary>
        public ITerminal From
        {
            get { return Line.StartPoint; }
        }

        /// <summary>
        /// The position of the end of this divider (coincides with the end
        /// of the associated line)
        /// </summary>
        public ITerminal To
        {
            get { return Line.EndPoint; }
        }

        /// <summary>
        /// The divider at the start of the associated line
        /// </summary>
        internal override IDivider FirstDivider
        {
            get { return this; }
        }

        /// <summary>
        /// The divider at the end of the associated line.
        /// </summary>
        internal override IDivider LastDivider
        {
            get { return this; }
        }

        /// <summary>
        /// Returns an enumerator that identifies this instance as the one and only divider
        /// in this topology.
        /// </summary>
        /// <returns>This</returns>
        public override IEnumerator<IDivider> GetEnumerator()
        {
            yield return this;
        }

        public override string ToString()
        {
            return String.Format("Line={0} L={1} R={2}",
                                    Line.DataId,
                                    (Left==null ? "n/a" : Left.ToString()),
                                    (Right==null ? "n/a" : Right.ToString()));
        }

        /// <summary>
        /// Implements <see cref="IDivider"/> method by returning <c>false</c>,
        /// indicating that this topology is not involved in any sort of overlap.
        /// The <see cref="LineOverlap"/> class overrides.
        /// </summary>
        public virtual bool IsOverlap
        {
            get { return false; }
        }

        /// <summary>
        /// Implements <see cref="IDivider"/> method by returning <c>true</c>,
        /// indicating that this topology is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return true; }
        }

        /// <summary>
        /// Performs any processing when the line associated with this topology
        /// is being de-activated.  This should mark adjacent polygons for deletion, and
        /// remove line references from any intersections.
        /// </summary>
        internal override void OnLineDeactivation()
        {
            // Mark adjacent polygons for deletion
            Topology.MarkPolygons(this);

            // Don't need to do anything about intersections, since LineTopology relates
            // to a complete line, and complete lines must terminate on concrete PointFeature
            // objects.
        }
    }
}
