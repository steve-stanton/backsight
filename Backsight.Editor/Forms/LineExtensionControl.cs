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
using System.Drawing;

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Geometry;
using Backsight.Forms;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for the <see cref="LineExtensionUI"/>
    /// </summary>
    public partial class LineExtensionControl : UserControl
    {
        #region Class data

        /// <summary>
        /// The command running this dialog.
        /// </summary>
        readonly CommandUI m_Cmd;

        /// <summary>
        /// The line that is being extended.
        /// </summary>
        LineFeature m_ExtendLine;

        /// <summary>
        /// True if extending from the end of the line. False from the start.
        /// </summary>
        bool m_IsExtendFromEnd;

        /// <summary>
        /// The length of the extension.
        /// </summary>
        Distance m_Length;

        /// <summary>
        /// The ID (+ entity type) of the extension point.
        /// </summary>
        IdHandle m_PointId;

        /// <summary>
        /// True if a line should be added too.
        /// </summary>
        bool m_WantLine;

        #endregion

        #region Constructors

        internal LineExtensionControl(LineExtensionUI cmd, LineFeature extendLine, Operation recall)
        {
            InitializeComponent();

            Zero();
            m_Cmd = cmd;
            m_ExtendLine = extendLine;

            LineExtensionOperation op = (recall as LineExtensionOperation);
	        if (op!=null)
            {
		        m_IsExtendFromEnd = op.IsExtendFromEnd;
		        m_Length = new Distance(op.Length);
                m_WantLine = (op.NewLine!=null);
            }
        }

        internal LineExtensionControl(UpdateUI updcmd)
        {
            InitializeComponent();
            Zero();
        }

        #endregion

        void Zero()
        {
        	m_ExtendLine = null;
	        m_IsExtendFromEnd = true;
	        m_Length = new Distance();
	        m_PointId = new IdHandle();
	        m_WantLine = true;
        }

        /// <summary>
        /// The observed length of the extension.
        /// </summary>
        internal Distance Length
        {
            get { return m_Length; }
        }

        /// <summary>
        /// Is the extension from the end of selected line?
        /// </summary>
        internal bool IsExtendFromEnd
        {
            get { return m_IsExtendFromEnd; }
        }

        /// <summary>
        /// Should a line be added too?
        /// </summary>
        internal bool WantLine
        {
            get { return m_WantLine; }
        }

        /// <summary>
        /// The ID (+ entity type) of the extension point.
        /// </summary>
        internal IdHandle PointId
        {
            get { return m_PointId; }
        }

        private void LineExtensionControl_Load(object sender, EventArgs e)
        {
            // Initialize combo box with a list of all point entity types
            // for the currently active layer.
            IEntity ent = pointTypeComboBox.Load(SpatialType.Point);

            // If we are doing an update select the previously defined
            // entity type, and show the key that was assigned to the point.
            if (!InitUpdate())
            {
                // Otherwise ...

                // Load the ID combo (reserving the first available ID).
                IdHelper.LoadIdCombo(idComboBox, ent, m_PointId, true);

                // If we are auto-numbering, disable the combo.
                if (m_Cmd.Controller.IsAutoNumber)
                    idComboBox.Enabled = false;

                // We may be recalling an old operation.
                if (m_Length.IsDefined)
                    lengthTextBox.Text = m_Length.Format();
            }

            wantLineCheckBox.Checked = m_WantLine;

            // De-select the line we're extending (if it's highlighted, and we're extending
            // on a circle construction line, it can be difficult to see stuff we draw here).
            m_Cmd.Controller.ClearSelection();
        }

        private void lengthTextBox_TextChanged(object sender, EventArgs e)
        {
            m_Length = new Distance(lengthTextBox.Text);
            m_Cmd.ErasePainting();
        }

        private void otherEndButton_Click(object sender, EventArgs e)
        {
            // Toggle the end we're extending from.
            m_IsExtendFromEnd = !m_IsExtendFromEnd;

            // Set the focus to something useful (keeping the focus on
            // the "other end" button is kind of useless).
            lengthTextBox.Focus();

            m_Cmd.ErasePainting();
        }

        private void pointTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Get the new point type (it might be null while the control is loading)
            IEntity ent = pointTypeComboBox.SelectedEntityType;
            if (ent==null)
                return;

            // If the current ID does not apply to the new point type,
            // reload the ID combo (reserving a different ID).
            if (!m_PointId.IsValidFor(ent))
                IdHelper.LoadIdCombo(idComboBox, ent, m_PointId, true);
            else
                m_PointId.Entity = ent;
        }

        private void wantLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_WantLine = wantLineCheckBox.Checked;
            m_Cmd.ErasePainting();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (m_ExtendLine!=null && m_Length.IsDefined)
                m_Cmd.DialFinish(this);
            else
            {
                if (m_ExtendLine==null)
                    MessageBox.Show("The line to extend has not been specified.");
                else
                    MessageBox.Show("The length of the extension has not been specified.");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        internal void Draw()
        {
            ISpatialDisplay display = m_Cmd.ActiveDisplay;

            // Draw the line we're extending in a special colour (any highlighting it
            // originally had should have been removed during LineExtensionControl_Load)
            if (m_ExtendLine!=null)
                m_ExtendLine.Draw(display, Color.DarkBlue);

            // If we're doing an update, draw the original extension in grey.
            LineExtensionOperation pop = UpdateOp;
            if (pop!=null)
            {
                LineFeature origLine = pop.NewLine;
                if (origLine!=null)
                    origLine.Draw(display, Color.Gray);

                PointFeature origPoint = pop.NewPoint;
                if (origPoint!=null)
                    origPoint.Draw(display, Color.Gray);
            }

            // Calculate the start and end points of the extension, initially
            // assuming that it's a straight line extension.
            IPosition start, end;
            if (LineExtensionUI.Calculate(m_ExtendLine, m_IsExtendFromEnd, m_Length, out start, out end))
            {
                // Draw the straight extension line
                IDrawStyle style = (m_WantLine ? new DrawStyle(Color.Magenta) : new DottedStyle(Color.Magenta));
                LineSegmentGeometry seg = new LineSegmentGeometry(start, end);
                seg.Render(display, style);
            }
            else
            {
                // Perhaps it's a circular arc ...

                IPosition center;
                bool iscw;

                if (LineExtensionUI.Calculate(m_ExtendLine, m_IsExtendFromEnd, m_Length,
                            out start, out end, out center, out iscw))
                {
                    // And draw the curve.
                    IDrawStyle style = (m_WantLine ? new DrawStyle(Color.Magenta) : new DottedStyle(Color.Magenta));
                    IPointGeometry c = PointGeometry.Create(center);
                    CircularArcGeometry arc = new CircularArcGeometry(c, start, end, iscw);
                    arc.Render(display, style);
                }
                else if (m_ExtendLine!=null)
                {
                    // Get the position we're extending from.
                    end = (m_IsExtendFromEnd ? m_ExtendLine.EndPoint : m_ExtendLine.StartPoint);
                }
            }

            // If we actually got something, draw the end point.
            if (end!=null)
            {
                IDrawStyle style = m_Cmd.Controller.DrawStyle;
                style.FillColor = Color.Magenta;
                style.Render(display, end);
            }
        }

        private void idComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            IdHelper.OnChangeSelectedId(idComboBox, m_PointId);
        }

        bool InitUpdate()
        {
            // Get the creating op.
            LineExtensionOperation pop = this.UpdateOp;
            if (pop==null)
                return false;

            Form parent = ParentForm;
            parent.Text = "Update Line Extension";

            m_ExtendLine = pop.ExtendedLine;
            m_IsExtendFromEnd = pop.IsExtendFromEnd;
            m_Length = new Distance(pop.Length);

            PointFeature point = pop.NewPoint;
            m_PointId = new IdHandle(point);
            IEntity ent = (point==null ? null : point.EntityType);

            // Was an extension line added?
            m_WantLine = (pop.NewLine!=null);

            // Scroll the entity combo to the previously defined
            // entity type for the extension point.
            if (ent!=null)
                pointTypeComboBox.SelectEntity(ent);

            // Disable the entity combo, as well as the check box that
            // says whether a line should be added. All the user can
            // update is the length of the extension, and the end of
            // the line to extend from.
            pointTypeComboBox.Enabled = false;
            wantLineCheckBox.Enabled = false;

            // Display the original observed length.
            lengthTextBox.Text = m_Length.Format();

            // Display the point key (if any) and disable it.
            idComboBox.Text = m_PointId.FormattedKey;
            idComboBox.Enabled = false;

            return true;
        }

        bool IsUpdate
        {
            get { return (m_Cmd is UpdateUI); }
        }

        LineExtensionOperation UpdateOp
        {
            get
            {
                UpdateUI up = (m_Cmd as UpdateUI);
                return (up==null ? null : (LineExtensionOperation)up.GetOp());
            }
        }
    }
}
