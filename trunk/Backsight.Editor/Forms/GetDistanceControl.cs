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
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for getting the user to specify a distance (for use with
    /// Intersection edits).
    /// </summary>
    partial class GetDistanceControl : UserControl
    {
        #region Class data

        // Data for operation ...

        /// <summary>
        /// The point the distance was observed from.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// Observed distance (either m_Distance or m_OffsetPoint).
        /// </summary>
        Observation m_ObservedDistance;

        /// <summary>
        /// Type of line (if any).
        /// </summary>
        IEntity m_LineType;

        // View-related ...

        /// <summary>
        /// Currently displayed circle.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Point used to specify distance.
        /// </summary>
        PointFeature m_DistancePoint;

        /// <summary>
        /// Entered distance value (if specified that way).
        /// </summary>
        Distance m_Distance;

        /// <summary>
        /// The offset point (if specified that way).
        /// </summary>
        OffsetPoint m_OffsetPoint;

        #endregion

        #region Constructors

        internal GetDistanceControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
