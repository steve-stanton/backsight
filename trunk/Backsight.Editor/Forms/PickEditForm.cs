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

using Backsight.Editor.Operations;


namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for selecting a specific edit from the edits that have been
    /// performed during the current editing. For use when recalling an edit
    /// using the <c>Edit - Recall</c> command.
    /// </summary>
    /// <seealso cref="SessionForm"/>
    partial class PickEditForm : Form
    {
        #region Class data

        /// <summary>
        /// The session of interest
        /// </summary>
        readonly ISession m_Session;

        /// <summary>
        /// The currently select edit
        /// </summary>
        Operation m_SelectedEdit;

        #endregion

        #region Constructors

        internal PickEditForm(ISession s)
        {
            InitializeComponent();

            if (s==null)
                throw new ArgumentNullException();

            m_Session = s;
            m_SelectedEdit = null;
        }

        #endregion

        Operation[] GetOperations()
        {
            Operation[] result = m_Session.Edits;

            // Return in reverse order
            for (int i = 0, j = result.Length - 1; i < j; i++, j--)
            {
                Operation temp = result[i];
                result[i] = result[j];
                result[j] = temp;
            }

            return result;
        }

        private void PickEditForm_Shown(object sender, EventArgs e)
        {
            // Load the list of operations that were performed in the session (in reverse order).
            Operation[] ops = GetOperations();

            grid.RowCount = ops.Length;
            for (int i = 0; i < ops.Length; i++)
            {
                Operation op = ops[i];
                DataGridViewRow row = grid.Rows[i];

                row.Tag = op;
                row.Cells["EditSequence"].Value = op.EditSequence;
                row.Cells["EditType"].Value = op.Name;
                row.Cells["FeatureCount"].Value = op.FeatureCount;
                row.Cells["Recallable"].Value = (op is IRecallable ? "yes" : "no");
            }

            // The first edit in the grid is selected by default. If it's
            // recallable, ensure the OK button reflects this and focus there (so
            // that hitting the Return key will close the dialog).
            if (ops.Length > 0)
            {
                okButton.Enabled = (ops[0] is IRecallable);
                if (okButton.Enabled)
                    okButton.Focus();
            }
        }

        Operation GetSelectedOperation()
        {
            DataGridViewSelectedRowCollection sel = grid.SelectedRows;
            if (sel.Count==0)
                return null;

            DataGridViewRow row = sel[0];
            return (row.Tag as Operation);
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Operation op = GetSelectedOperation();

            if (IsAcceptableEdit(op))
            {
                m_SelectedEdit = op;
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        bool IsAcceptableEdit(Operation op)
        {
            if (op==null)
                return false;

            if (op is IRecallable)
                return true;

            MessageBox.Show("Specified edit does not have a recall facility.");
            return false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Operation op = GetSelectedOperation();

            if (op == null)
                MessageBox.Show("You must first select an edit");
            else if (IsAcceptableEdit(op))
            {
                m_SelectedEdit = op;
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// The currently select edit
        /// </summary>
        internal Operation SelectedEdit
        {
            get { return m_SelectedEdit; }
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            Operation op = GetSelectedOperation();
            okButton.Enabled = (op is IRecallable);
        }
    }
}