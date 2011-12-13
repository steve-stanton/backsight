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

using System.Diagnostics;
using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    class NewMiscTextOperation : NewTextOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewMiscTextOperation"/> class.
        /// </summary>
        internal NewMiscTextOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewMiscTextOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal NewMiscTextOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            // Nothing to do
        }

        /// <summary>
        /// Executes this operation. This version is suitable for adding miscellaneous
        /// non-topological trim.
        /// </summary>
        /// <param name="trim">The text of the label.</param>
        /// <param name="ent">The entity type to assign to the new label (default was null)</param>
        /// <param name="position">The reference position for the label.</param>
        /// <param name="ght">The height of the new label, in meters on the ground.</param>
        /// <param name="gwd">The width of the new label, in meters on the ground.</param>
        /// <param name="rot">The clockwise rotation of the text, in radians from the horizontal.</param>
        internal void Execute(string trim, IEntity ent, IPosition position, double ght, double gwd, double rot)
        {
            // Add the label.
            TextFeature text = MapModel.AddMiscText(this, trim, ent, position, ght, gwd, rot);
            SetText(text);

            // The trim is always non-topological.
            text.SetTopology(false);

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
            base.WriteData(editSerializer);

            // Nothing to do - the relevant info should have come out via the geometry object attached
            // to the created text feature
            Debug.Assert(base.Text.TextGeometry is MiscTextGeometry);
        }
    }
}
