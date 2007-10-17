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

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Inverse distance calculator for circular arcs (with bearings to the center of
    /// the circle)
    /// </summary>
    partial class InverseArcDistanceBearingForm : InverseArcDistanceForm
    {
        #region Constructors

        internal InverseArcDistanceBearingForm()
        {
            InitializeComponent();
        }

        #endregion

        internal override void ShowResult()
        {
            base.ShowResult();

            // Now show the bearings too. Don't bother if we've
            // only got 1 point, since that may involve more than
            // one circle (also true when we have 2 points, but in
            // that case, we use the first circle arbitrarily).

            if (Point1!=null && Point2!=null)
            {
                // It's conceivable that the two points share more than
                // one common circle. For now, just pick off the first
                // common circle and use that.
                Circle circle = FirstCommonCircle;
                if (circle==null)
                    return;

                // Get the bearing from the center to both points
                IAngle bear1 = Geom.Bearing(Point1, circle.Center);
                IAngle bear2 = Geom.Bearing(Point2, circle.Center);

                bearing1TextBox.Text = RadianValue.AsString(bear1.Radians);
                bearing2TextBox.Text = RadianValue.AsString(bear2.Radians);
            }
            else
            {
                bearing1TextBox.Text = bearing2TextBox.Text = "<no bearing>";
            }
        }
    }
}