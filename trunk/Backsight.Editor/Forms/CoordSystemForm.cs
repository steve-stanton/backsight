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

using Backsight.Editor.Properties;
using Backsight.Editor.Observations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdCoordSystem"/>
    /// <summary>
    /// Dialog for displaying basic information for the coordinate system of the
    /// current map model.
    /// </summary>
    partial class CoordSystemForm : Form
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        internal CoordSystemForm()
        {
            InitializeComponent();
        }

        #endregion

        private void CoordSystemForm_Shown(object sender, EventArgs e)
        {
            CadastralMapModel map = CadastralMapModel.Current;
            ISpatialSystem sys = map.SpatialSystem;

            systemNameLabel.Text = sys.Name;
            epsgNumberLabel.Text = sys.EpsgNumber.ToString();

            // Display mean elevation and geoid separation in the current data entry units.
            EditingController ec = EditingController.Current;
            DistanceUnit eUnit = ec.EntryUnit;
            DistanceUnit meters = EditingController.GetUnits(DistanceUnitType.Meters);

            // The mean elevation & geoid separation fields are always editable (even
            // if the map contains stuff). The values are used to calculate the ground
            // area of polygons.

            Distance elev = new Distance(sys.MeanElevation.Meters, meters);
            meanElevationTextBox.Text = elev.Format(eUnit, true);

            Distance sep = new Distance(sys.GeoidSeparation.Meters, meters);
            geoidSeparationTextBox.Text = sep.Format(eUnit, true);
    	}

        private void okButton_Click(object sender, EventArgs e)
        {
            // Get the coordinate system in case we need to make changes.
            ISpatialSystem sys = CadastralMapModel.Current.SpatialSystem;

            // Mean elevation. If no units were specified, this
            // assumes the current data entry units.
            double meanElev = sys.MeanElevation.Meters;
            Distance elev;
            if (Distance.TryParse(meanElevationTextBox.Text, out elev))
                meanElev = elev.Meters;

            // Geoid separation
            double gSep = sys.GeoidSeparation.Meters;
            Distance sep;
            if (Distance.TryParse(geoidSeparationTextBox.Text, out sep))
                gSep = sep.Meters;

            if (Math.Abs(meanElev - sys.MeanElevation.Meters) > 0.0001 ||
                Math.Abs(gSep - sys.GeoidSeparation.Meters) > 0.0001)
            {
                sys.MeanElevation = new Length(meanElev);
                sys.GeoidSeparation = new Length(gSep);

                // Update application settings too (the remembered settings become the default
                // for any new maps subsequently created -- see CoordinateSystem constructor).

                // TODO: The following expects the user to save. Should really utilize an editing op
                // at this stage.

                Settings.Default.MeanElevation = meanElev;
                Settings.Default.GeoidSeparation = gSep;
                Settings.Default.Save();
            }

            DialogResult = DialogResult.OK;
            Close();
            return;
        }
    }
}