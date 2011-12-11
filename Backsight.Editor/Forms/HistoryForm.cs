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
using System.Diagnostics;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for listing the editing sessions (and individual edits). Also
    /// lets the user undo edits.
    /// </summary>
    public partial class HistoryForm : Form
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        public HistoryForm()
        {
            InitializeComponent();
        }

        #endregion

        private void HistoryForm_Shown(object sender, EventArgs e)
        {
            LoadSessionList();
        }

        void LoadSessionList()
        {
            ISession[] sa = CadastralMapModel.Current.Sessions;
            grid.ColumnCount = 4;
            grid.RowCount = sa.Length;
            int rowIndex = grid.RowCount;
            for (int i = 0; i < sa.Length; i++)
            {
                rowIndex--;
                DataGridViewRow row = grid.Rows[rowIndex];
                row.Cells["StartTime"].Value = sa[i].StartTime;
                row.Cells["EndTime"].Value = sa[i].EndTime;
                row.Cells["EditCount"].Value = sa[i].OperationCount;
                row.Tag = sa[i];
            }

            grid.CurrentCell = null;
        }

        /// <summary>
        /// Rolls back the last known edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rollbackButton_Click(object sender, EventArgs e)
        {
            // If the working session contains anything, rollback that
            Session ws = Session.WorkingSession;
            bool done = false;

            if (ws.OperationCount > 0)
                done = CadastralMapModel.Current.Rollback(ws);
            else
            {
                ISession[] sa = CadastralMapModel.Current.Sessions;
                Debug.Assert(sa.Length>0);

                if (sa.Length == 1)
                {
                    MessageBox.Show("Nothing to undo");
                    Debug.Assert(sa[0] == ws);
                    return;
                }

                // Get the session that precedes the working session
                ISession s = sa[sa.Length-2];
                done = CadastralMapModel.Current.Rollback(s);
            }

            if (done)
            {
                LoadSessionList();
                EditingController.Current.RefreshAllDisplays();
            }
        }

        private void detailsButton_Click(object sender, EventArgs e)
        {
            Session s = GetSelectedSession();
            if (s==null)
            {
                MessageBox.Show("Select the session you want details for");
                return;
            }

            ShowSession(s);
        }

        private void grid_DoubleClick(object sender, EventArgs e)
        {
            Session s = GetSelectedSession();
            if (s!=null)
                ShowSession(s);
        }

        Session GetSelectedSession()
        {
            DataGridViewSelectedRowCollection sel = grid.SelectedRows;
            if (sel.Count==0)
                return null;

            DataGridViewRow row = sel[0];
            return (Session)row.Tag;
        }

        void ShowSession(Session s)
        {
            SessionForm dial = new SessionForm(s);
            dial.ShowDialog();
            dial.Dispose();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}