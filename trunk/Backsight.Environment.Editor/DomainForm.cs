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
using System.Data;

using Smo=Microsoft.SqlServer.Management.Smo;

using Backsight.SqlServer;

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Dialog for specifying field domains. This is currently restricted to domains
    /// where the lookup values are explicitly listed in some database table.
    /// </summary>
    public partial class DomainForm : Form
    {
        public DomainForm()
        {
            InitializeComponent();
        }

        private void DomainForm_Shown(object sender, EventArgs e)
        {
            // Load the database tables in the current database (excluding all
            // Backsight system tables)
            string[] tableNames = new TableFactory().GetUserTables();

            // Grab the currently defined domain tables
            IDomainTable[] currentDomains = EnvironmentContainer.Current.DomainTables;

            List<string> domains = new List<string>();
            domains.Add(String.Empty);

            // Include only those tables that have the required columns.
            // Exclude tables that have already been defined as domain tables.

            foreach (string t in tableNames)
            {
                if (!Array.Exists<IDomainTable>(currentDomains, delegate(IDomainTable dt)
                                { return String.Compare(dt.TableName, t, true) == 0; }))
                {
                    string[] cols = GetColumnNames(t);

                    if (Array.Exists(cols, delegate(string a) { return String.Compare(a, "ShortValue", true) == 0; }) &&
                        Array.Exists(cols, delegate(string a) { return String.Compare(a, "LongValue", true) == 0; }))
                        domains.Add(t);
                }
            }

            tableNameComboBox.DataSource = domains;
        }

        string[] GetColumnNames(string tableName)
        {
            TableFactory tf = new TableFactory();
            Smo.Table t = tf.FindTableByName(tableName);

            if (t == null)
                return new string[0];

            List<string> result = new List<string>(t.Columns.Count);

            foreach (Smo.Column c in t.Columns)
                result.Add(c.Name);

            return result.ToArray();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Ensure the table name is defined
            string tableName = tableNameComboBox.SelectedItem.ToString();
            if (tableName.Length == 0)
            {
                MessageBox.Show("The name of the domain table must be specified");
                tableNameComboBox.Focus();
                return;
            }

            IEnvironmentFactory f = EnvironmentContainer.Factory;
            IEditDomainTable dt = f.CreateDomainTable();
            dt.BeginEdit();
            dt.TableName = tableName;
            dt.FinishEdit();

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