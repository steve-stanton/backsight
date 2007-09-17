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

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Backsight.SqlServer
{
    public partial class PickDatabaseForm : Form
    {
        private readonly ServerConnection m_Server;
        private Database m_Database;

        public PickDatabaseForm(ServerConnection sc)
        {
            InitializeComponent();
            m_Server = sc;
            m_Database = null;
        }

        internal Database Database
        {
            get { return m_Database; }
        }

        private void PickDatabaseForm_Shown(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (!m_Server.IsOpen)
                    m_Server.Connect();

                listBox.Items.Add("Loading database list...");
                listBox.Enabled = false;
                listBox.Refresh();

                Server s = new Server(m_Server);
                DatabaseCollection dc = s.Databases;
                listBox.Items.Clear();

                foreach (Database d in dc)
                    listBox.Items.Add(d);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                m_Database = (Database)sel;
                this.DialogResult = DialogResult.OK;
                Close();
            }            
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            object sel = listBox.SelectedItem;
            if (sel==null)
            {
                MessageBox.Show("You must first select a database");
                return;
            }

            m_Database = (Database)sel;
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
