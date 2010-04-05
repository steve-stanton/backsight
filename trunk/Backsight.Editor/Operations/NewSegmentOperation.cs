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
    /// An edit that creates a line segment connecting a pair of pre-existing points.
    /// </summary>
    class NewSegmentOperation : NewLineOperation
    {
        internal NewSegmentOperation(Session s)
            : base(s)
        {
        }

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal NewSegmentOperation(Session s, NewSegmentData t)
            : base(s, t)
        {
        }

        /// <summary>
        /// Creates a new simple line segment.
        /// </summary>
        /// <param name="start">The point at the start of the new line.</param>
        /// <param name="end">The point at the end of the new line.</param>
        /// <returns>True if new line added ok.</returns>
        internal bool Execute(PointFeature start, PointFeature end)
        {
            // Disallow an attempt to add a null line.
            if (start.Geometry.IsCoincident(end.Geometry))
                throw new Exception("NewLineOperation.Execute - Attempt to add null line.");

            // Add the new line with default entity type.
            CadastralMapModel map = CadastralMapModel.Current;
            LineFeature newLine = map.AddLine(start, end, map.DefaultLineType, this);
            base.SetNewLine(newLine);

            // Peform standard completion steps
            Complete();
            return true;
        }

        /// <summary>
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// <returns>The serializable version of this edit</returns>
        internal override OperationData GetSerializableEdit()
        {
            return new NewSegmentData(this);
            //NewSegmentData t = new NewSegmentData();
            //base.SetSerializableEdit(t);
            //t.Line = (SegmentData)base.Line.GetSerializableLine();
            //return t;
        }
    }
}
