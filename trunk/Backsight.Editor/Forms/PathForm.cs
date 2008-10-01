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
using System.Collections.Generic;
using System.Drawing;

using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdPath"/>
    /// <summary>
    /// Dialog for a new connection path.
    /// </summary>
    partial class PathForm : Form
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        readonly PathUI m_Command;

        /// <summary>
        /// The start of the path.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The end of the path.
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The control that last had the focus.
        /// </summary>
        Control m_Focus;

        /// <summary>
        /// True if user pointer to obtain the start of the path.
        /// </summary>
        bool m_FromPointed;

        /// <summary>
        /// True if user pointer to obtain the end of the path.
        /// </summary>
        bool m_ToPointed;

        /// <summary>
        /// Parsed path items
        /// </summary>
        PathItem[] m_Items;

        /// <summary>
        /// The path created from the items.
        /// </summary>
        PathData m_PathData;

        /// <summary>
        /// True if the path has been drawn.
        /// </summary>
        bool m_DrawPath;

        /// <summary>
        /// Current dialog for closure info.
        /// </summary>
        AdjustmentForm m_Adjustment;

        /// <summary>
        /// The current data entry units
        /// </summary>
        DistanceUnit m_Units;

        #endregion

        #region Constructors

        internal PathForm(PathUI command, PointFeature from, PointFeature to)
        {
            InitializeComponent();

            m_Command = command;
            m_From = from;
            m_To = to;

            SetZeroValues();
        }
    
        #endregion

        void SetZeroValues()
        {
	        m_Focus			= null;
	        m_FromPointed	= false;
	        m_ToPointed		= false;
	        m_Items			= null;
	        m_PathData		= null;
	        m_DrawPath		= false;
	        m_Adjustment	= null;
            m_Units         = null;
        }

        private void PathForm_Shown(object sender, EventArgs e)
        {
            // Display at top left corner of the screen.
            this.Location = new Point(0, 0);

            // If we are recalling an old operation, fill in the
            // data entry string.
            PathOperation op = null;
            if (m_Command != null)
                op = (m_Command.Recall as PathOperation);
            ShowInput(op);

            // Display the current default units.
            m_Units = EditingController.Current.EntryUnit;
            if (m_Units == null)
                defaultUnitsLabel.Text = String.Empty;
            else
                defaultUnitsLabel.Text = String.Format("{0}... ", m_Units.Abbreviation);

            // Disable the "Preview" & "OK" buttons. They are only valid
            // after the from- and to-points have been entered, and after
            // something has been entered for the path.
            this.NoPreview();

            // If we already have a from and a to point, fill them
            // in and start in the data entry field.
            if (m_From!=null && m_To!=null)
            {
                m_Focus = fromTextBox;
                OnSelectPoint(m_From);

                m_Focus = toTextBox;
                OnSelectPoint(m_To);
            }
        }

        /// <summary>
        /// Saves the path. This method is called from <see cref="AdjustmentForm"/>
        /// when the user clicks the "OK" button.
        /// </summary>
        internal void Save()
        {
            // There SHOULD be a path to save.
            if (m_PathData==null)
            {
                MessageBox.Show("PathForm.Save -- nothing to save");
                return;
            }

            // Ensure adjustment dialog has been destroyed.
            OnDestroyAdj();

            // Execute the edit
            PathOperation op = null;

            try
            {
                string str = GetEnteredPath();
                op = new PathOperation(m_From, m_To, str);
                //op = new PathOperation(m_PathData);
                op.Execute();
                Finish();
            }

            catch (Exception ex)
            {
                Session.WorkingSession.Remove(op);
                MessageBox.Show(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If we are showing adjustment results, get rid of them
            if (m_Adjustment!=null)
            {
                m_Adjustment.Dispose();
                m_Adjustment = null;
            }

            // Tell the command that's running this dialog that we're done
            if (m_Command!=null)
                m_Command.DialAbort(this);
        }

        /// <summary>
        /// Tells the command that's running this dialog that we're finished
        /// </summary>
        void Finish()
        {
            if (m_Command!=null)
                m_Command.DialFinish(this);
        }

        /// <summary>
        /// Performs processing upon selection of a new point (indicated by pointing at the map)
        /// </summary>
        /// <param name="point">The point that has been selected</param>
        internal void OnSelectPoint(PointFeature point)
        {
            // Return if point is not defined.
            if (point==null)
                return;

            SetColor(point, null);

            // Handle the pointing, depending on what field we were last in.
            if (Object.ReferenceEquals(m_Focus, fromTextBox))
            {
                // Ensure that any previously selected backsight reverts
                // to its normal color and remember the new point.
                if (!Object.ReferenceEquals(point, m_From))
                {
                    SetNormalColor(m_From);
                    m_From = point;
                }

                // Remember that the user pointed.
                m_FromPointed = true;

                // Display any point key
                fromTextBox.Text = GetPointString(m_From);

                // See if preview button can be enabled.
                CheckPreview();

                // Move focus to the to-point.
                toTextBox.Focus();
            }
            else if (Object.ReferenceEquals(m_Focus, toTextBox))
            {
                // Save the to point, and remember that the user pointed.
                if (!Object.ReferenceEquals(point, m_To))
                {
                    SetNormalColor(m_To);
                    m_To = point;
                }

                m_ToPointed = true;

                // Display any point key
                toTextBox.Text = GetPointString(m_To);

                // See if preview button can be enabled.
                CheckPreview();

                // Move focus to the data field, making sure that nothing
                // is initially selected.
                GotoPath();
            }
        }

        /// <summary>
        /// Ensures a point is drawn in its normal color.
        /// </summary>
        /// <param name="point">The point that needs to be drawn normally</param>
        void SetNormalColor(PointFeature point)
        {
            // Redraw in idle time
            if (point != null)
                m_Command.ErasePainting();
        }

        /// <summary>
        /// Sets color for a point. 
        /// </summary>
        /// <param name="point">The point to draw.</param>
        /// <param name="c">The field that the point relates to. The default is
        /// the field that currently has the focus.</param>
        void SetColor(PointFeature point, Control c)
        {
            // Return if point not specified.
            if (point==null)
                return;

            ISpatialDisplay display = m_Command.ActiveDisplay;
            Control field = (c == null ? m_Focus : c);

            if (Object.ReferenceEquals(field, fromTextBox))
                point.Draw(display, Color.DarkBlue);
            else if (Object.ReferenceEquals(field, toTextBox))
                point.Draw(display, Color.LightBlue);
        }

        /// <summary>
        /// Does any painting that this dialog does.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal void Render(ISpatialDisplay display)
        {
            // Draw any currently selected points & any direction.
            SetColor(m_From, fromTextBox);
            SetColor(m_To, toTextBox);

            if (m_PathData!=null && m_DrawPath)
                m_PathData.Render(display);
        }

        private void fromTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = fromTextBox;
        }

        private void toTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = toTextBox;
        }

        private void distanceButton_Click(object sender, EventArgs e)
        {
            // Display distance dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.

            DistanceForm dial = new DistanceForm();
            if (dial.ShowDialog() == DialogResult.OK)
                ReplaceSel(dial.Format());

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        private void fromTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the field is now empty, allow the user to type in an ID.
            if (fromTextBox.Text.Trim().Length == 0)
            {
                SetNormalColor(m_From);
                m_FromPointed = false;
                m_From = null;
                NoPreview();
            }
        }

        private void fromTextBox_Leave(object sender, EventArgs e)
        {
            // Just return if the user specified the field by pointing.
            if (m_FromPointed)
                return;

            // Return if the field is empty.
            string str = fromTextBox.Text.Trim();
            if (str.Length == 0)
                return;

            // Parse the ID value.
            uint idnum;
            if (!UInt32.TryParse(str, out idnum))
            {
                MessageBox.Show("Invalid point ID");
                fromTextBox.Focus();
                return;
            }

            // Ask the map to locate the specified point.
            CadastralMapModel map = CadastralMapModel.Current;
            m_From = new FindPointByIdQuery(map.Index, idnum.ToString()).Result;
            if (m_From == null)
            {
                MessageBox.Show("No point with specified key.");
                fromTextBox.Focus();
                return;
            }

            // Display the point in the correct color.
            SetColor(m_From, fromTextBox);
            CheckPreview();
        }

        private void toTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the field is now empty, allow the user to type in an ID.
            if (toTextBox.Text.Trim().Length == 0)
            {
                SetNormalColor(m_To);
                m_To = null;
                m_ToPointed = false;
                NoPreview();
            }
        }

        private void toTextBox_Leave(object sender, EventArgs e)
        {
            // Just return if the user specified the field by pointing.
            if (m_ToPointed)
                return;

            // Return if the field is empty.
            string str = toTextBox.Text.Trim();
            if (str.Length == 0)
                return;

            // Parse the ID value.
            uint idnum;
            if (!UInt32.TryParse(str, out idnum))
            {
                MessageBox.Show("Invalid point ID");
                fromTextBox.Focus();
                return;
            }

            // Ask the map to locate the specified point.
            CadastralMapModel map = CadastralMapModel.Current;
            m_To = new FindPointByIdQuery(map.Index, idnum.ToString()).Result;
            if (m_To == null)
            {
                MessageBox.Show("No point with specified key.");
                toTextBox.Focus();
                return;
            }

            // Display the point in the correct color.
            SetColor(m_To, toTextBox);
            CheckPreview();
        }

        private void angleButton_Click(object sender, EventArgs e)
        {
            // Display angle dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.

            AngleForm dial = new AngleForm();
            if (dial.ShowDialog() == DialogResult.OK)
                ReplaceSel(dial.Format());

            dial.Dispose();

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        private void culDeSacButton_Click(object sender, EventArgs e)
        {
            // Display cul-de-sac dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.
            CulDeSacForm dial = new CulDeSacForm();
            if (dial.ShowDialog() == DialogResult.OK)
                ReplaceSel(dial.Format());

            dial.Dispose();

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        void ReplaceSel(string s)
        {
            // This is meant to do the same as the MFC CEdit::ReplaceSel method...
            Clipboard.SetText(s);
            pathTextBox.Paste();
        }

        private void curveButton_Click(object sender, EventArgs e)
        {
            // Display curve dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.

            ArcForm dial = new ArcForm();
            if (dial.ShowDialog() == DialogResult.OK)
                ReplaceSel(dial.Format());

            dial.Dispose();

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            // If the entered path parses ok, do the adjustment
            if (ParsePath())
                Adjust();
        }

        private void endCurveButton_Click(object sender, EventArgs e)
        {
            // Stick an /EC at the end of the edit box.
            ReplaceSel(") ");
        }

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            // If we previously had an adjustment dialog, cancel it.
            // This should also end up calling this->OnDestroyAdj

            if (m_Adjustment != null)
            {
                m_Adjustment.Dispose();
                m_Adjustment = null;
            }

            // Ignore any path data that remains.
            m_PathData = null;

            // The preview button should be enabled only if there is
            // something defined for the path.
            CheckPreview();
        }

        void CheckPreview()
        {
            previewButton.Enabled = (m_To != null &&
                                     m_From != null &&
                                     pathTextBox.Text.Trim().Length > 0);
        }

        void NoPreview()
        {
            previewButton.Enabled = false;
        }

        /// <summary>
        /// Sets focus to the path field, ensuring that nothing is
        /// selected when the focus gets there.
        /// </summary>
        void GotoPath()
        {
            // Set selection to the character after any current selection.
            int nSel = pathTextBox.SelectionLength;
            if (nSel>0)
            {
                int istart = pathTextBox.SelectionStart;
                pathTextBox.SelectionLength = 0;
                pathTextBox.SelectionStart = istart + nSel;
            }

            // Move the focus.
            pathTextBox.Focus();

            // If there's only one line in the path control, scroll
            // to the end of the line.
            if (pathTextBox.Lines.Length == 1)
            {
                pathTextBox.SelectionLength = 0;
                pathTextBox.SelectionStart = pathTextBox.Lines[0].Length;
            }
        }

        bool ParsePath()
        {
            // How many characters have we got (this may include extra junk
            // on each line of the control).
            int nchars = pathTextBox.Text.Length;
            if (nchars < 0)
                return false;

            string str = GetEnteredPath();
            return ParseString(str);
        }

        /// <summary>
        /// Grabs the entered path (without any embedded newlines) - not sure
        /// if they're there or not
        /// </summary>
        /// <returns>The connection path, as entered by the user</returns>
        string GetEnteredPath()
        {
            string str = pathTextBox.Text;
            return str.Replace(System.Environment.NewLine, " ").Trim();
        }

        /// <summary>
        /// Parses the string that represents the path description.
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <returns>True if the supplied string is understandable</returns>
        bool ParseString(string str)
        {
            try
            {
                m_Items = PathParser.GetPathItems(str);
                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Adjusts a validated path.
        /// </summary>
        void Adjust()
        {
            // Confirm that the from-point & to-point are both defined (this
            // should have been checked beforehand).
            if (m_From==null || m_To==null)
            {
                MessageBox.Show("Terminal points have not been defined");
                return;
            }

            // Get rid of any path previously created.
            m_PathData = null;

            try
            {
                // Create new path data
                PathData pd = new PathData(m_From, m_To);
                pd.Create(m_Items);
                m_PathData = pd;

                // Adjust the path

                double de;				// Misclosure in eastings
                double dn;				// Misclosure in northings
                double prec;			// Precision
                double length;			// Total observed length
                double rotation;		// Rotation for adjustment
                double sfac;			// Adjustment scaling factor

                m_PathData.Adjust(out dn, out de, out prec, out length, out rotation, out sfac);

                // Draw the path with the adjustment info
                m_DrawPath = true;
                m_PathData.Render(m_Command.ActiveDisplay);

                // Display the numeric results.
                m_Adjustment = new AdjustmentForm(dn, de, prec, length, this);
                m_Adjustment.Show();

                // Disable the preview button (it will be re-enabled when
                // the adjustment dialog is destroyed).
                previewButton.Enabled = false;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Takes action when the adjustment result dialog has been cancelled (user
        /// has clicked the "Reject" button). At the time of call, the dialog
        /// is already closed, though a reference to the instance is still
        /// held by this class.
        /// </summary>
        internal void OnDestroyAdj()
        {
            // Remember not to draw the path.
            m_DrawPath = false;

            // Get rid of the dialog object.
            if (m_Adjustment!=null)
            {
                m_Adjustment.Dispose();
                m_Adjustment = null;
            }

            // Re-enable the Preview button.
            previewButton.Enabled = true;

            // Ensure any preview stuff gets erased
            m_Command.ErasePainting();
        }

        /// <summary>
        /// Fills the data entry field with the stuff that was entered for
        /// a specific connection path.
        /// </summary>
        /// <param name="op">The operation to show.</param>
        void ShowInput(PathOperation op)
        {
            // Return if the operation has not been specified.
            if (op==null)
                return;

            // Get the data entry string.
            string str = op.GetString();

            // Display the data entry string (this takes care of word wrap).
            pathTextBox.Text = str;
        }

        /// <summary>
        /// Obtains the ID string for a selected point
        /// </summary>
        /// <param name="p">The point of interest</param>
        /// <returns>The ID string to display</returns>
        internal static string GetPointString(PointFeature p)
        {
            string s = p.FormattedKey;
            return (s.Length == 0 ? p.DataId : s);
        }
    }
}