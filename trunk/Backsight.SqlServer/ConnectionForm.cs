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
using System.Data.SqlClient;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

using Backsight.Data;

namespace Backsight.SqlServer
{
    public partial class ConnectionForm : Form
    {
        private Database m_Database;

        public ConnectionForm()
        {
            InitializeComponent();
            m_Database = null;
        }

        /// <summary>
        /// The selected database (null if nothing selected)
        /// </summary>
        public Database Database
        {
            get { return m_Database; }
        }

        private void ConnectionForm_Shown(object sender, EventArgs e)
        {
            string s = GlobalUserSetting.LastConnection;
            if (String.IsNullOrEmpty(s))
            {
                // For the time being, assume localhost is valid
                serverTextBox.Text = "localhost";
                userNameTextBox.Text =
                String.Format("{0}\\{1}", System.Environment.UserDomainName, System.Environment.UserName);
                windowsAuthenticationRadioButton.Checked = true;
            }
            else
            {
                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(s);
                serverTextBox.Text = sb.DataSource;
                windowsAuthenticationRadioButton.Checked = sb.IntegratedSecurity;
                userNameTextBox.Text = sb.UserID;
                passwordTextBox.Text = sb.Password;
                dbTextBox.Text = sb.InitialCatalog;
            }

            ProcessWindowsAuthentication();
        }

        private void browseServerButton_Click(object sender, EventArgs e)
        {
            PickServerForm dial = new PickServerForm();
            if (dial.ShowDialog() == DialogResult.OK)
                serverTextBox.Text = dial.ServerName;

            dial.Dispose();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            // Attempt to connect to the database (even if we obtained one via
            // the PickDatabaseForm, it's possible the user has altered the displayed
            // text subsequent to that)
            try
            {
                ServerConnection sc = GetConnection();
                if (sc==null)
                    return;

                string dbName = dbTextBox.Text;
                m_Database = new Server(sc).Databases[dbName];
                if (m_Database==null)
                    throw new Exception(String.Format("No database called '{0}'", dbName));

                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
                sb.DataSource = serverTextBox.Text;
                sb.IntegratedSecurity = windowsAuthenticationRadioButton.Checked;
                sb.UserID = userNameTextBox.Text;
                sb.Password = passwordTextBox.Text;
                sb.InitialCatalog = dbName;
                GlobalUserSetting.LastConnection = sb.ToString();

                this.DialogResult = DialogResult.OK;
                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void dbBrowseButton_Click(object sender, EventArgs e)
        {
            ServerConnection sc = GetConnection();
            if (sc==null)
                return;

            PickDatabaseForm dial = new PickDatabaseForm(sc);
            if (dial.ShowDialog() == DialogResult.OK)
                dbTextBox.Text = dial.Database.Name;

            dial.Dispose();
            sc.Disconnect();
        }

        ServerConnection GetConnection()
        {
            string serverName = serverTextBox.Text.Trim();
            if (String.IsNullOrEmpty(serverName))
            {
                MessageBox.Show("You must first specify the name of a server");
                serverTextBox.Focus();
                return null;
            }

            Cursor currentCursor = Cursor.Current;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ServerConnection result = new ServerConnection();
                result.ServerInstance = serverName;
                result.ConnectTimeout = 5;

                result.LoginSecure = windowsAuthenticationRadioButton.Checked;
                if (!result.LoginSecure)
                {
                    result.Login = userNameTextBox.Text;
                    result.Password = passwordTextBox.Text;
                }

                result.Connect();
                return result;
            }

            catch (Exception e)
            {
                this.Cursor = currentCursor;
                MessageBox.Show(e.Message);
            }

            finally
            {
                this.Cursor = currentCursor;
            }

            return null;
        }

        private void windowsAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ProcessWindowsAuthentication();
        }

        private void ProcessWindowsAuthentication()
        {
            userNameTextBox.Enabled = passwordTextBox.Enabled = (!windowsAuthenticationRadioButton.Checked);
        }
    }
}
