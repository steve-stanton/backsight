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
using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    class NewMiscTextOperation : NewTextOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewMiscTextOperation"/> class.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal NewMiscTextOperation(Session s, uint sequence)
            : base(s, sequence)
        {
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
    }
}
