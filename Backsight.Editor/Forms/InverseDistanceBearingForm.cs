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
using System.Windows.Forms;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Inverse distance and bearing calculator
    /// </summary>
    partial class InverseDistanceBearingForm : InverseDistanceForm
    {
        #region Constructors

        internal InverseDistanceBearingForm()
        {
            InitializeComponent();
        }

        #endregion

        internal override void ShowResult()
        {
	        base.ShowResult();

            // Now show the bearing too.
	        if (Point1!=null && Point2!=null)
            {
                double bearing = Geom.BearingInRadians(Point1, Point2);
                bearingTextBox.Text = RadianValue.AsString(bearing);
            }
        }
	}
}