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
    /// Flag bits for the <c>Feature</c> class
    /// </summary>
    [Flags]
    enum FeatureFlag : ushort
    {
        /// <summary>
        /// Feature is being rolled back (user is undoing the operation that
        /// created a feature)
        /// </summary>
        Undoing=0x0001,

        /// <summary>
        /// Feature has been superseded. This flag gets set if a feature should no longer
        /// appear on the current editing layer.
        /// </summary>
        Inactive=0x0002,

        /// <summary>
        /// Feature has moved in rollforward
        /// </summary>
        Moved=0x0010,

        /// <summary>
        /// Is this feature topological?
        /// </summary>
        Topol=0x0080,

        /// <summary>
        /// Does line represent a staggered property face?
        /// </summary>
        Void=0x0200,

        /// <summary>
        /// Does the feature's ID come from a foreign source (i.e. import)?
        /// </summary>
        ForeignId=0x0400,

        /// <summary>
        /// Is the feature locked (i.e. should not be edited).
        /// </summary>
        Locked=0x0800,

        /// <summary>
        /// Topology completely defined
        /// </summary>
        Built=0x1000,

        /// <summary>
        /// Line object marked as trimmed
        /// </summary>
        Trim=0x2000,

        /// <summary>
        /// Is the feature spatially indexed?
        /// </summary>
        Indexed=0x4000,
    };
}
