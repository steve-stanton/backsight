// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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

using Backsight.Editor.Observations;


namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdDist"/>
    /// <summary>
    /// Simple dialog for entering a distance.
    /// </summary>
    partial class DistForm : Form
    {
        #region Class data

        /// <summary>
        /// The entered distance
        /// </summary>
        Distance m_Dist;

        /// <summary>
        /// Should the "want line too" check box be shown?
        /// </summary>
        bool m_ShowWantLine;

        /// <summary>
        /// Does the user want a line?
        /// </summary>
        bool m_WantLine;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DistForm"/> class.
        /// </summary>
        internal DistForm()
        {
            InitializeComponent();

            m_Dist = null;
            m_ShowWantLine = true;
            m_WantLine = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistForm"/> class.
        /// </summary>
        /// <param name="dist">The initial distance to display.</param>
        /// <param name="showWantLine">Should the "want line too" check box be shown?</param>
        internal DistForm(Distance dist, bool showWantLine)
            : this()
        {
            m_Dist = dist;
            m_ShowWantLine = showWantLine;
        }

        #endregion

        private void DistForm_Shown(object sender, EventArgs e)
        {
            if (m_Dist != null)
            {
                distTextBox.Text = m_Dist.Format();
                m_Dist = null;
            }

            // Hide the "want line" checkbox if requested. Otherwise
            // check it by default.

            if (m_ShowWantLine)
                wantLineCheckBox.Checked = true;
            else
                wantLineCheckBox.Visible = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string dstr = distTextBox.Text.Trim();
            if (dstr.Length == 0)
            {
                MessageBox.Show("You have not entered a distance.");
                return;
            }

            Distance dist;
            if (!Distance.TryParse(dstr, out dist))
            {
                MessageBox.Show("Don't understand entered distance.");
                return;
            }

            m_Dist = dist;

            if (m_ShowWantLine)
                m_WantLine = wantLineCheckBox.Checked;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// The entered distance (null if the user cancelled the dialog).
        /// </summary>
        internal Distance Distance
        {
            get { return m_Dist; }
        }

        /// <summary>
        /// Does the user want a line?
        /// </summary>
        internal bool WantLine
        {
            get { return m_WantLine; }
        }
    }
}
