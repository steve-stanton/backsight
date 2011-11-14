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
using System.Collections.Generic;

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// An edit that creates a circular arc.
    /// </summary>
    class NewArcOperation : NewLineOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewArcOperation"/> class.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal NewArcOperation(Session s, uint sequence)
            : base(s, sequence)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewArcOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal NewArcOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            // Let the base class do it
            // ArcFeature arc = editDeserializer.ReadPersistent<ArcFeature>("Line");
            // SetNewLine(arc);
        }

        /// <summary>
        /// Creates a new circular arc.
        /// </summary>
        /// <param name="start">The point at the start of the new arc.</param>
        /// <param name="end">The point at the end of the new arc.</param>
        /// <param name="circle">The circle that the new arc should sit on.</param>
        /// <param name="isShortArc">True if the new arc refers to the short arc. False
        /// if it's a long arc (i.e. greater than half the circumference of the circle).</param>
        internal void Execute(PointFeature start, PointFeature end, Circle circle, bool isShortArc)
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
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>
        /// The referenced features (never null, but may be an empty array).
        /// </returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>(base.GetRequiredFeatures());
            ArcFeature arc = (ArcFeature)this.Line;
            PointFeature center = arc.Circle.CenterPoint;
            if (center.Creator != this)
                result.Add(center);

            return result.ToArray();
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            // Let the base class do it
            // ArcFeature arc = (ArcFeature)base.Line;
            // editSerializer.WritePersistent<ArcFeature>("Line", arc);
        }
    }
}
