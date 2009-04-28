// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
using Backsight.Editor.Operations;
using Backsight.Environment;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Serializable version of the <see cref="CircularLeg"/> class.
    /// </summary>
    public partial class CircularLegType : LegType
    {
        /// <summary>
        /// Loads this leg as part of an editing operation.
        /// </summary>
        /// <param name="op">The editing operation creating the leg</param>
        /// <param name="startPoint">The point (if any) at the start of this leg (may be
        /// null if the preceding leg ended with the "omit point" option)</param>
        /// <param name="lineType">The entity type to assign to any lines created
        /// along the length of this leg</param>
        /// <returns>The leg that was loaded</returns>
        internal override Leg LoadLeg(PathOperation op, PointFeature startPoint, IEntity lineType)
        {
            return new CircularLeg(op, this, startPoint, lineType);
        }
    }
}
