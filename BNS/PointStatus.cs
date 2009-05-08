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

namespace BNS
{
    enum PointStatus
    {
        Unknown=0x0,
        None=0x100,
        ReadyToShoot=0x200,
        Shot=0x800,
        Laid=0x1000,
        BagDropped=0x4000,
        Killed=0x8000,
        Theoretical=0x10000,
        Planned=0x20000,
        Completed=0x40000,
        PickedUp=0x80000,
        Serviced=0x100000,

        /// <summary>
        /// An unplanned bag drop that has been subsquently lifted. This status
        /// value should only be assigned only if the feature's PointCode property
        /// equals <c>PointCodes.UnplannedBagDrop</c>.
        /// </summary>
        UnplannedBagDropLifted=0x400000,

        /// <summary>
        /// A drill point that has been shot but encountered a problem. Requires a
        /// troubleshoot crew to revisit the point to resolve the problem.
        /// </summary>
        Shot_Unsuccessful=0x800000,

        /// <summary>
        /// A point that has been drilled, but not yet loaded with dynamite
        /// </summary>
        ReadyToLoad=0x1000000,
    }
}
