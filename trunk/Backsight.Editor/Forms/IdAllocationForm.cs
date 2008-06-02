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
    public partial class IdAllocationForm : Form
    {
        #region Class data

        /// <summary>
        /// The map's ID manager.
        /// </summary>
        IdManager m_IdMan;

        /// <summary>
        /// True if a change has been made.
        /// </summary>
        bool m_IsChange;

        #endregion

        #region Constructors

        internal IdAllocationForm()
        {
            InitializeComponent();
            m_IdMan = IdManager.Current;
	        m_IsChange = false;
        }

        #endregion

        private void IdAllocationForm_Shown(object sender, EventArgs e)
        {
            // If there is no ID manager, end the dialog now!
            if (m_IdMan==null)
            {
                MessageBox.Show("Database connection for ID management could not be established.");
                this.DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            // Refresh the list.
            RefreshList();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // If we saved anything, save the file too (so that it's in
            // sync with the database).
            /*
            if (m_IsChange)
            {
                EditingController c = EditingController.Current;
                c.AutoSave();
            }
            */
            Close();
        }

        private void getButton_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection sel = grid.SelectedRows;

            if (sel==null || sel.Count==0)
            {
                if (MessageBox.Show("Do you want an allocation for every group?", "No selection", MessageBoxButtons.YesNo)
                        == DialogResult.No)
                {
                    MessageBox.Show("Then you must first select the group(s) you want.");
                    return;
                }

                // Go through each group, getting an allocation for each one
                // ... even if it already has one?
                m_IdMan.GetAllocation();
            }
            else
            {
                foreach (DataGridViewRow row in sel)
                {
                    IdGroup g = (IdGroup)row.Cells["dgcGroupName"].Value;
                    m_IdMan.GetAllocation(g, false); // no announcement
                }
            }

	        RefreshList();
	        SetChanged();
        }

        private void releaseButton_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection sel = grid.SelectedRows;

            if (sel==null || sel.Count==0)
            {
                if (MessageBox.Show("Return unused portions of every allocation?", "No selection", MessageBoxButtons.YesNo)
                        == DialogResult.No)
                {
                    MessageBox.Show("Then you must first select the group(s) you want.");
                    return;
                }

                // Go through each group, releasing the allocation of every range.
                m_IdMan.Release();
            }
            else
            {
                foreach (DataGridViewRow row in sel)
                {
                    IdRange r = (IdRange)row.Cells["dgcRange"].Value;

                    // The range will be null if the user selected a group that didn't have
                    // any allocated ranges
                    if (r!=null)
                    {
                        IdGroup g = (IdGroup)row.Cells["dgcGroupName"].Value;
                        g.Release(r.Min, r.Max);
                    }
                }
            }

            RefreshList();
            SetChanged();
        }

        private void grid_DoubleClick(object sender, EventArgs e)
        {
	        // Return if more than one thing is selected (how we got a
	        // double-click message is therefore a bit odd).
            DataGridViewSelectedRowCollection sel = grid.SelectedRows;
            if (sel==null || sel.Count!=1)
                return;

            // Make an allocation for the selected group.
            IdGroup g = (IdGroup)sel[0].Cells["dgcGroupName"].Value;
            m_IdMan.GetAllocation(g, false); // no announcement
            RefreshList();
            SetChanged();
        }

        void RefreshList()
        {
            grid.Rows.Clear();

            foreach (IdGroup group in m_IdMan.IdGroups)
            {
                // Get the ID ranges that are already associated with the group (if any).

                int rowIndex = grid.Rows.Add();
                DataGridViewRow row = grid.Rows[rowIndex];
                row.Cells["dgcGroupName"].Value = group;

                List<IdRange> ranges = group.IdRanges;
                if (ranges!=null && ranges.Count>0)
                {
                    foreach (IdRange range in group.IdRanges)
                    {                        
                        row.Cells["dgcAllocation"].Value = String.Format("{0}-{1}", range.Min, range.Max);

                        string s;
                        if (range.IsReleased)
                            s = String.Format("{0} (released)", range.NumUsed);
                        else
                            s = String.Format("{0} of {1}", range.NumUsed, range.Size);

                        row.Cells["dgcNumUsed"].Value = s;
                        row.Cells["dgcRange"].Value = range;
                    }
                }
            }

            // Ensure nothing is initially selected
            grid.CurrentCell = null;
        }

        void SetChanged()
        {
            if (!m_IsChange)
            {
                okButton.Text = "&Save";
                m_IsChange = true;
            }
        }
    }
}
