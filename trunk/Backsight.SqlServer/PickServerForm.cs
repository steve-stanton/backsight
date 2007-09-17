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
using System.Data;
using System.Windows.Forms;

using Microsoft.SqlServer.Management.Smo;

namespace Backsight.SqlServer
{
    public partial class PickServerForm : Form
    {
        private string m_ServerName;

        public PickServerForm()
        {
            InitializeComponent();
            m_ServerName = String.Empty;
        }

        internal string ServerName
        {
            get { return m_ServerName; }
        }

        private void PickServerForm_Shown(object sender, EventArgs e)
        {
            try
            {
                listBox.Items.Add("Loading server list...");
                listBox.Enabled = false;
                listBox.Refresh();

                this.Cursor = Cursors.WaitCursor;
                DataTable dt = SmoApplication.EnumAvailableSqlServers(false);
                listBox.Items.Clear();

                foreach (DataRow r in dt.Rows)
                {
                    string name = r["Name"].ToString();
                    listBox.Items.Add(name);
                }
            }

            finally
            {
                this.Cursor = Cursors.Default;
                listBox.Enabled = true;
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            object sel = listBox.SelectedItem;
            if (sel!=null)
            {
                m_ServerName = sel.ToString();
                this.DialogResult = DialogResult.OK;
                Close();
            }            
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            object sel = listBox.SelectedItem;
            if (sel==null)
            {
                MessageBox.Show("You must first select a server");
                return;
            }

            m_ServerName = sel.ToString();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
