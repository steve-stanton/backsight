/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
    /// Dialog for the <see cref="NewCircleUI"/>
    /// </summary>
    partial class NewCircleForm : Form
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        readonly CommandUI m_Cmd;

        /// <summary>
        /// The circle (if any) that is being updated.
        /// </summary>
        LineFeature m_Update;

        /// <summary>
        /// The control that has the focus.
        /// </summary>
        Control m_Focus;

        /// <summary>
        /// A previous operation that was recalled (always null if doing
        /// an update).
        /// </summary>
        //NewCircleOperation m_Recall;

        // For the operation ...

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        PointFeature m_Center;

        /// <summary>
        /// Observed distance (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/>).
        /// </summary>
        Observation m_Radius; // was m_pRadius

        // Preview related ...

        /// <summary>
        /// Currently displayed circle.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Point used to specify distance.
        /// </summary>
        PointFeature m_RadiusPoint;

        /// <summary>
        /// Entered distance value (if specified that way).
        /// </summary>
        Distance m_RadiusDistance; // was m_Radius

        #endregion

        internal NewCircleForm(NewCircleUI cmd)
        {
            InitializeComponent();

            m_Cmd = cmd;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            //m_Cmd.DialFinish(this);
        }
    }
}