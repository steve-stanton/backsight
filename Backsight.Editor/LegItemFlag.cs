// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <summary>
    /// Flag bits that apply to each distance along a <see cref="Leg"/>
    /// in a connection path.
    /// </summary>
    /// <seealso cref="PathOperation"/>
    [Flags]
    enum LegItemFlag : byte
    {
        /// <summary>
        /// No flag
        /// </summary>
        Null = 0x00,

        /// <summary>
        /// Miss-connect (no line)
        /// </summary>
        MissConnect = 0x01,

        /// <summary>
        /// Omit point (no line, no point)
        /// </summary>
        OmitPoint = 0x02,

        /// <summary>
        /// Angle at start of a straight leg is a deflection.
        /// This switch will be set ONLY for the first span in a straight leg.
        /// </summary>
        //Deflection = 0x04,

        /// <summary>
        /// Miss-connect should be replaced with a new line upon rollforward.
        /// </summary>
        NewLine = 0x08,

        /// <summary>
        /// Leg is staggered, and this is the first face. 
        /// This switch will be set ONLY for the first span in a leg.
        /// </summary>
        //Face1 = 0x10,

        /// <summary>
        /// Leg is staggered, and this is the second face (the leg follows
        /// the first face in the enclosing <c>PathOperation</c> object).
        /// This switch will be set ONLY for the first span in a leg.
        /// </summary>
        //Face2 = 0x20,
    }
}
