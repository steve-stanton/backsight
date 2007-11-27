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
using System.Collections.Generic;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="10-FEB-1998" was="CdGetControl" />
    /// <summary>
    /// Dialog for getting a list of control points
    /// </summary>
    public partial class GetControlForm : Form
    {
        #region Class data

        /// <summary>
        /// Control ranges (one for each line in the dialog).
        /// </summary>
        List<ControlRange> m_Ranges;

        /// <summary>
        /// True if the map initially has an undefined extent.
        /// </summary>
        bool m_NewMap;

    	// uint m_CurrRange; // used in methods for CCF file input

        #endregion

        #region Constructors

        public GetControlForm()
        {
            InitializeComponent();
        }

        #endregion

        private void GetControlForm_Shown(object sender, EventArgs e)
        {

        }

        private void browseButton_Click(object sender, EventArgs e)
        {

        }

        private void getDataButton_Click(object sender, EventArgs e)
        {

        }

        private void addToMapButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }
    }
}