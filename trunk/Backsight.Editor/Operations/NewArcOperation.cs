// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
using Backsight.Editor.Xml;

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// An edit that creates a circular arc.
    /// </summary>
    class NewArcOperation : NewLineOperation
    {
        internal NewArcOperation(Session s)
            : base(s)
        {
        }

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal NewArcOperation(Session s, NewArcData t)
            : base(s, t)
        {
        }


        /// <summary>
        /// Creates a new circular arc.
        /// </summary>
        /// <param name="start">The point at the start of the new arc.</param>
        /// <param name="end">The point at the end of the new arc.</param>
        /// <param name="circle">The circle that the new arc should sit on.</param>
        /// <param name="isShortArc">True if the new arc refers to the short arc. False
        /// if it's a long arc (i.e. greater than half the circumference of the circle).</param>
        /// <returns>True if new arc added ok.</returns>
        internal bool Execute(PointFeature start, PointFeature end, Circle circle, bool isShortArc)
        {
            // Disallow an attempt to add a null line.
            if (start.Geometry.IsCoincident(end.Geometry))
                throw new Exception("NewLineOperation.Execute - Attempt to add null line.");

            // Figure out whether the arc should go clockwise or not.
            IPointGeometry centre = circle.Center;

            // Get the clockwise angle from the start to the end.
            Turn sturn = new Turn(centre, start);
            double angle = sturn.GetAngleInRadians(end);

            // Figure out which direction the curve should go, depending
            // on whether the user wants the short arc or the long one.
            bool iscw;
            if (angle < Constants.PI)
                iscw = isShortArc;
            else
                iscw = !isShortArc;

            // Add the new arc with default line entity type (this will
            // cross-reference the circle to the arc that gets created).
            CadastralMapModel map = CadastralMapModel.Current;
            LineFeature newLine = map.AddCircularArc(circle, start, end, iscw, map.DefaultLineType, this);
            base.SetNewLine(newLine);

            // Peform standard completion steps
            Complete();
            return true;
        }
    }
}
