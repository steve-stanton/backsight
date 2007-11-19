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
//using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="18-NOV-1999" was="CePolygonCheck" />
    /// <summary>
    /// The result of a check on a polygon ring.
    /// </summary>
    class RingCheck : CheckItem
    {
        #region Static

        /// <summary>
        /// Performs checks for a polygon ring.
        /// </summary>
        /// <param name="ring">The ring to check.</param>
        /// <returns>The problem(s) that were found.</returns>
        internal static CheckType CheckRing(Ring ring)
        {
            CheckType types = CheckType.Null;

            // Is it really small? Use a hard-coded size of 0.01
	        // square metres (a 10cm square).
            double area = Math.Abs(ring.Area);
            if (area < 0.01)
                types |= CheckType.SmallPolygon;

            if (ring is Island)
            {
                // Does the polygon have an enclosing polygon?
                Island island = (ring as Island);
                if (island.Container==null)
                    types |= CheckType.NotEnclosed;
            }
            else
            {
        		// Does the polygon have a label?
                Polygon pol = (Polygon)ring;
                if (pol.LabelCount==0)
                    types |= CheckType.NoLabel;
            }

            return types;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The ring the check relates to (never null).
        /// </summary>
        readonly Ring m_Ring;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>RingCheck</c> that relates to the specified polygon ring.
        /// </summary>
        /// <param name="ring">The polygon ring the check relates to (not null).</param>
        /// <param name="types">The type(s) of check this item corresponds to</param>
        /// <exception cref="ArgumentNullException">If <paramref name="ring"/> is null</exception>
        internal RingCheck(Ring ring, CheckType types)
            : base(types)
        {
            if (ring==null)
                throw new ArgumentNullException();

            m_Ring = ring;
        }

        #endregion

        internal override void Paint(ISpatialDisplay display)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void PaintOut(ISpatialDisplay display, CheckType newTypes)
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
