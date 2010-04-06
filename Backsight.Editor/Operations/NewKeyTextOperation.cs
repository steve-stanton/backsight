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

namespace Backsight.Editor.Operations
{
    class NewKeyTextOperation : NewTextOperation
    {
        /// <summary>
        /// Executes the new label operation.
        /// </summary>
        /// <param name="vtx">The position of the new label.</param>
        /// <param name="polygonId">The ID and entity type to assign to the new label.</param>
        /// <param name="pol">The polygon that the label falls inside. It should not already refer to a label.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the text, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
        internal void Execute(IPosition vtx, IdHandle polygonId, Polygon pol,
                                double height, double width, double rotation)
        {
            // Add the label.
            CadastralMapModel map = MapModel;
            TextFeature text = map.AddKeyLabel(this, polygonId, vtx, height, width, rotation);
            SetText(text);

            // Associate the polygon with the label, and vice versa.
            text.SetTopology(true);
            pol.ClaimLabel(text);

            Complete();
        }
    }
}
