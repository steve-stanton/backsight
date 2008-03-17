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
using System.Drawing;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdGetPoint" />
    /// <summary>
    /// Dialog for getting the user to specify a point (either by entering the
    /// ID, or pointing at the map). Not as general-purpose as it sounds, since
    /// it is exclusively used by the <see cref="PathUI"/> class.
    /// </summary>
    partial class GetPointForm : Form
    {
        #region Class data

        /// <summary>
        /// The parent command.
        /// </summary>
        readonly PathUI m_Parent;

        /// <summary>
        /// The title for the window.
        /// </summary>
        readonly string m_Title;

        /// <summary>
        /// The color hint for the point.
        /// </summary>
        readonly Color m_Color;

        /// <summary>
        /// The selected point.
        /// </summary>
        PointFeature m_Point;

        /// <summary>
        /// Was the point selected by pointing at the map?
        /// </summary>
        bool m_IsPointed;

        /// <summary>
        /// Should the "back" button be enabled?
        /// </summary>
        bool m_IsBackEnabled;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>GetPointForm</c>
        /// </summary>
        /// <param name="cmd">The parent command.</param>
        /// <param name="title">The title for the window.</param>
        /// <param name="col">The color hint for the point.</param>
        /// <param name="enableBack">Should the "back" button be enabled?</param>
        internal GetPointForm(PathUI cmd, string title, Color col, bool enableBack)
        {
            InitializeComponent();

            m_Parent = cmd;
            m_Point = null;
            m_Color = col;
            m_Title = title;
            m_IsPointed = false;
            m_IsBackEnabled = enableBack;
        }

        #endregion

        /// <summary>
        /// The selected point.
        /// </summary>
        internal PointFeature Point
        {
            get { return m_Point; }
        }

        private void GetPointForm_Shown(object sender, EventArgs e)
        {
            // Display at top left corner of the screen.
            this.Location = new Point(0, 0);

            // Display the desired title.
            this.Text = m_Title;

            // Disable the "back" button if necessary.
            backButton.Enabled = m_IsBackEnabled;

            colorButton.BackColor = m_Color;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            m_Parent.OnPointBack();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Parent.OnPointCancel();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Confirm that we have a point!
            if (m_Point==null)
            {
                MessageBox.Show("You must select a point.");
                pointTextBox.Focus();
                return;
            }

            m_Parent.OnPointNext();
        }

        private void pointTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the field is now empty, allow the user to type in an ID.
            if (pointTextBox.Text.Trim().Length==0)
            {
                SetNormalColor();
                m_IsPointed = false;
                m_Point = null;
            }
        }

        private void pointTextBox_Leave(object sender, EventArgs e)
        {
            // Just return if the user specified the point by pointing.
            if (m_IsPointed)
                return;

            // Return if the field is empty.
            string str = pointTextBox.Text.Trim();
            if (str.Length==0)
                return;

            // Parse the ID value.
            uint idnum;
            if (!UInt32.TryParse(str, out idnum))
            {
                MessageBox.Show("Invalid point ID");
                pointTextBox.Focus();
                return;
            }

            // Ask the map to locate the specified point.
            CadastralMapModel map = CadastralMapModel.Current;
            m_Point = new FindPointByIdQuery(map.Index, idnum.ToString()).Result;
            if (m_Point==null)
            {
                MessageBox.Show("No point with specified ID.");
                pointTextBox.Focus();
                return;
            }

            // Tell the parent dialog we're done.
            m_Parent.OnPointNext();
        }

        /// <summary>
        /// Ensures the currently selected point (if any) is drawn in its normal color.
        /// </summary>
        void SetNormalColor()
        {
            if (m_Point!=null)
                m_Parent.ErasePainting();
        }

        /// <summary>
        /// Sets the color for the currently selected point. 
        /// </summary>
        void SetColor()
        {
            // Draw the point in the correct color.
            if (m_Point!=null)
                m_Point.Draw(m_Parent.ActiveDisplay, m_Color);
        }

        /// <summary>
        /// Performs processing upon selection of a new point (indicated by pointing at the map)
        /// </summary>
        /// <param name="point">The point that has been selected</param>
        /// <param name="movenext">Should the dialog automatically move on to the next point (default was true)</param>
        internal void OnSelectPoint(PointFeature point, bool movenext)
        {
            // Return if point is not defined.
            if (point==null)
                return;

            // Ensure that any previously selected point reverts to its normal color.
            SetNormalColor();

            // Remember the new point.
            m_Point = point;
            m_IsPointed = true;

            // Set the color of the point.
            SetColor();

            // Display the point's ID.
            pointTextBox.Text = point.FormattedKey;

            // Tell the command that's running this dialog to move on.
            if (movenext)
                m_Parent.OnPointNext();
        }

        /// <summary>
        /// Does any painting that this dialog does.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal void Render(ISpatialDisplay display)
        {
            SetColor();
        }
    }
}