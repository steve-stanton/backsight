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
using System.Diagnostics;

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// An edit that creates a line segment connecting a pair of pre-existing points.
    /// </summary>
    class NewSegmentOperation : NewLineOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewSegmentOperation"/> class.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal NewSegmentOperation(Session s, uint sequence)
            : base(s, sequence)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewSegmentOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal NewSegmentOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            // Nothing to do
        }

        /// <summary>
        /// Creates a new simple line segment.
        /// </summary>
        /// <param name="start">The point at the start of the new line.</param>
        /// <param name="end">The point at the end of the new line.</param>
        internal void Execute(PointFeature start, PointFeature end)
        {
            // Disallow an attempt to add a null line.
            if (start.Geometry.IsCoincident(end.Geometry))
                throw new Exception("NewLineOperation.Execute - Attempt to add null line.");

            // Add the new line with default entity type.
            CadastralMapModel mapModel = this.MapModel;
            LineFeature newLine = mapModel.AddLine(start, end, mapModel.DefaultLineType, this);
            base.SetNewLine(newLine);

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            // Nothing to do - the relevant info should have come out via the geometry object attached
            // to the created line feature
            Debug.Assert(base.Line.LineGeometry is SegmentGeometry);
        }
    }
}
