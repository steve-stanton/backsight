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
using System.Collections.ObjectModel;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for listing the editing sessions (and individual edits). Also
    /// lets the user undo edits.
    /// </summary>
    public partial class HistoryForm : Form
    {
        #region Class data

        /// <summary>
        /// The data for the session grid
        /// </summary>
        BindingSource m_Binding;

        /// <summary>
        /// Have any edits been rolled back?
        /// </summary>
        bool m_IsEdited;

        #endregion

        #region Constructors

        public HistoryForm()
        {
            InitializeComponent();
            m_IsEdited = false;
            m_Binding = null;
        }

        #endregion

        private void HistoryForm_Shown(object sender, EventArgs e)
        {
            LoadSessionList();
        }

        void LoadSessionList()
        {
            m_Binding = new BindingSource();
            ReadOnlyCollection<Session> s = CadastralMapModel.Current.Sessions;

            // Display last session at top
            Session[] r = new Session[s.Count];
            for (int i=r.Length-1, j=0; i>=0; i--, j++)
                r[j] = s[i];

            m_Binding.DataSource = r;
            grid.AutoGenerateColumns = false;
            grid.DataSource = m_Binding;
        }

        /// <summary>
        /// Rolls back the last known edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rollbackButton_Click(object sender, EventArgs e)
        {
            uint rtype = CadastralMapModel.Current.Rollback(false);
            if (rtype!=0)
            {
                m_IsEdited = true;
                LoadSessionList();
                EditingController.Current.RefreshAllDisplays();
            }
            else
                MessageBox.Show("Nothing to rollback.");
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
            return (m_Binding[row.Index] as Session);
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

        private void HistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If we rolled back anything, save the map model
            if (m_IsEdited)
                CadastralMapModel.Current.Write();
        }
    }
}