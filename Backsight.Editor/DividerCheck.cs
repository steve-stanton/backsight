/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="18-NOV-1999" was="CeArcCheck" />
    /// <summary>
    /// The result of a check on a polygon ring divider.
    /// </summary>
    class DividerCheck : CheckItem
    {
        #region Static

        /// <summary>
        /// Checks if a divider is a dangle, floating, or a bridge.
        /// </summary>
        /// <param name="d">The divider to check</param>
        /// <returns>Bit mask of the check(s) the divider failed.</returns>
        internal static CheckType CheckNeighbours(IDivider d)
        {
            Debug.Assert(!d.IsOverlap);

	        // Return if the divider has different polygons on both sides.
            if (d.Left != d.Right)
                return CheckType.Null;

            bool sDangle = Topology.IsStartDangle(d);
            bool eDangle = Topology.IsEndDangle(d);

            if (sDangle && eDangle)
                return CheckType.Floating;

            if (sDangle || eDangle)
                return CheckType.Dangle;

            return CheckType.Bridge;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The divider the check relates to (never null).
        /// </summary>
        readonly IDivider m_Divider; // m_pArc

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DividerCheck</c> that relates to the specified divider.
        /// </summary>
        /// <param name="divider">The divider the check relates to (not null).</param>
        /// <param name="types">The type(s) of check this item corresponds to</param>
        /// <exception cref="ArgumentNullException">If <paramref name="divider"/> is null</exception>
        internal DividerCheck(IDivider divider, CheckType types)
            : base(types)
        {
            if (divider==null)
                throw new ArgumentNullException();

            m_Divider = divider;
        }

        #endregion

        internal override void Paint()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void PaintOut(CheckType newTypes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override CheckType ReCheck()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override string Explanation
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override IPosition Position
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override void Select()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
