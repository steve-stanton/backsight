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
    /// Dialog for displaying the coordinate system of the current map model. This provides
    /// very basic information since it was originally written only to cover requirements
    /// in Manitoba (meaning we're dealing only with UTM on NAD83). In the longer term, it
    /// is expected that support for coordinate systems will involve other open source
    /// projects (likely Proj.NET).
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
            ICoordinateSystem sys = map.CoordinateSystem;

            // Map projection & ellipsoid (currently fixed)
            projectionLabel.Text = sys.Projection;
            ellipsoidLabel.Text = sys.Ellipsoid;

            // UTM zone
            zoneUpDown.Value = sys.Zone;
            zoneUpDown.Enabled = map.IsEmpty;

            // Display mean elevation and geoid separation in the current data entry units.
            EditingController ec = EditingController.Current;
            DistanceUnit eUnit = ec.EntryUnit;
            DistanceUnit meters = EditingController.GetUnits(DistanceUnitType.Meters);

            // The mean elevation & geoid separation fields are always editable (even
            // if the map contains stuff). The values are used to calculate the ground
            // area of polygons.

            Distance elev = new Distance(sys.MeanElevation, meters);
            meanElevationTextBox.Text = elev.Format(eUnit, true);

            Distance sep = new Distance(sys.GeoidSeparation, meters);
            geoidSeparationTextBox.Text = sep.Format(eUnit, true);
    	}

        private void okButton_Click(object sender, EventArgs e)
        {
            // Get the coordinate system in case we need to make changes.
            CoordinateSystem sys = (CoordinateSystem)CadastralMapModel.Current.CoordinateSystem;

            // Pick up the UTM zone number
            int zone = (int)zoneUpDown.Value;
            if (zone<1 || zone>60)
            {
                MessageBox.Show("Zone is out of range [1,60]");
                return;
            }

            // Mean elevation. If no units were specified, this
            // assumes the current data entry units.
            double meanElev = sys.MeanElevation;
            Distance elev;
            if (Distance.TryParse(meanElevationTextBox.Text, out elev))
                meanElev = elev.Meters;

            // Geoid separation
            double gSep = sys.GeoidSeparation;
            Distance sep;
            if (Distance.TryParse(geoidSeparationTextBox.Text, out sep))
                gSep = sep.Meters;

            if (zone != sys.Zone ||
                Math.Abs(meanElev - sys.MeanElevation) > 0.0001 ||
                Math.Abs(gSep - sys.GeoidSeparation) > 0.0001)
            {
                sys.Zone = (byte)zone;
                sys.MeanElevation = meanElev;
                sys.GeoidSeparation = gSep;

                // Update application settings too (the remembered settings become the default
                // for any new maps subsequently created -- see CoordinateSystem constructor).

                // TODO: The following expects the user to save. Should really utilize an editing op
                // at this stage.

                Settings.Default.Zone = sys.Zone;
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