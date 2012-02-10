// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
    /// <written by="Steve Stanton" on="21-APR-2009" was="CdOpList" />
    /// <summary>
    /// Dialog for selecting a specific line for update (from a list of
    /// predecessor lines).
    /// </summary>
    /// <seealso cref="PickEditForm"/>
    partial class PickPredecessorForm : Form
    {
        #region Class data

        /// <summary>
        /// The lines to select from
        /// </summary>
        readonly LineFeature[] m_Lines;

        /// <summary>
        /// The selected line
        /// </summary>
        LineFeature m_Selection;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PickPredecessorForm"/> class.
        /// </summary>
        /// <param name="lines">The predecessors to display</param>
        /// <param name="canSelect">Can the user select a specific line?</param>
        internal PickPredecessorForm(LineFeature[] lines, bool canSelect)
        {
            InitializeComponent();
            okButton.Visible = canSelect;
            m_Lines = lines;
        }

        #endregion

        private void PickPredecessorForm_Shown(object sender, EventArgs e)
        {
            grid.RowCount = m_Lines.Length;

            for (int i=0; i<m_Lines.Length; i++)
            {
                LineFeature line = m_Lines[i];
                Operation op = line.Creator;
                DataGridViewRow row = grid.Rows[i];

                row.Tag = line;
                row.Cells["imageColumn"].Value = GetImage(op, line);
                row.Cells["opIdColumn"].Value = op.InternalId;
                row.Cells["operationColumn"].Value = op.Name;
                row.Cells["createdColumn"].Value = op.Session.StartTime.ToShortDateString();
                row.Cells["editorColumn"].Value = op.Session.User;

                // If the line was created by a connection path, display the precision.
                if (op is PathOperation)
                {
                    PathOperation path = (PathOperation)op;
                    row.Cells["precisionColumn"].Value = path.GetPrecision();
                }
            }

            grid.CurrentCell = null;
        }

        Image GetImage(Operation op, LineFeature line)
        {
            // If the creating op can't be updated, use a no-entry sign.
            if (!(op is IRevisable))
                return smallImageList.Images["NoEntry"];

            // If the line is not a circular curve, use the line icon.
            ArcFeature arc = (line as ArcFeature);
            if (arc==null)
                return smallImageList.Images["Line"];

            // This leaves us with either a circle, or gray circle.
            if (arc.Circle.Creator == op)
                return smallImageList.Images["Circle"];
            else
                return smallImageList.Images["GrayCircle"];
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
                MessageBox.Show("You have not selected anything yet.");
            else
                CloseOnSelectedLine();
        }

        LineFeature GetSel()
        {
            DataGridViewSelectedRowCollection sel = grid.SelectedRows;
            if (sel.Count == 0)
                return null;

            LineFeature line = (LineFeature)sel[0].Tag;
            if (line.Creator is IRevisable)
                return line;

            MessageBox.Show("Can't update this sort of edit.");
            return null;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            m_Selection = null;
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CloseOnSelectedLine();
        }

        void CloseOnSelectedLine()
        {
            m_Selection = GetSel();

            if (m_Selection != null)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// The selected line (null if user cancelled)
        /// </summary>
        internal LineFeature SelectedLine
        {
            get { return m_Selection; }
        }
    }
}