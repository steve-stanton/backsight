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
using System.Data;

using Backsight.Environment;


namespace Backsight.Editor.Operations
{
    class NewRowTextOperation : NewTextOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewRowTextOperation"/> class.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal NewRowTextOperation(Session s, uint sequence)
            : base(s, sequence)
        {
        }

        /// <summary>
        /// Executes the new label operation.
        /// </summary>
        /// <param name="vtx">The position of the new label.</param>
        /// <param name="polygonId">The ID and entity type to assign to the new label.</param>
        /// <param name="row">The data to use for creating a row for the new label.</param>
        /// <param name="atemplate">The template to use in creating the RowTextGeometry
        /// for the new label.</param>
        /// <param name="pol">The polygon that the label falls inside. It should not already
        /// refer to a label. Not null.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the new label, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
        internal void Execute(IPosition vtx, IdHandle polygonId, DataRow row, ITemplate atemplate, Polygon pol,
                                double height, double width, double rotation)
        {
            if (pol == null)
                throw new ArgumentNullException();

            // Add the label.
            TextFeature text = MapModel.AddRowLabel(this, polygonId, vtx, row, atemplate, height, width, rotation);
            SetText(text);

            // Associate the polygon with the label, and vice versa.
            text.SetTopology(true);
            pol.ClaimLabel(text);

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
            return new Feature[0];
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewRowTextOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal NewRowTextOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            throw new NotImplementedException();
        }
    }
}
