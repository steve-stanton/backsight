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
        /// <summary>
        /// The domain that's being edited
        /// </summary>
        private readonly IEditDomainTable m_Edit;

        public DomainForm()
            : this(null)
        {
        }

        public DomainForm(IEditDomainTable edit)
        {
            InitializeComponent();

            m_Edit = edit;

            if (m_Edit==null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateDomainTable();
            }

            m_Edit.BeginEdit();
        }

        private void DomainForm_Shown(object sender, EventArgs e)
        {
            // Load the database tables in the current database (excluding all
            // Backsight system tables)
            string[] tableNames = new TableFactory().GetUserTables();
            tableNameComboBox.DataSource = PrependBlank(tableNames);

            alreadyAddedLabel.Visible = false;

            if (m_Edit.Id > 0)
            {
                tableNameComboBox.SelectedItem = m_Edit.TableName;
                shortValueColumnNameComboBox.SelectedItem = m_Edit.LookupColumnName;
                longValueColumnNameComboBox.SelectedItem = m_Edit.ValueColumnName;
            }
        }

        string[] PrependBlank(string[] array)
        {
            List<string> result = new List<string>(array.Length + 1);
            result.Add(String.Empty);
            result.AddRange(array);
            return result.ToArray();
        }

        private void tableNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            object o = tableNameComboBox.SelectedItem;
            if (o == null || o.ToString().Length==0)
            {
                shortValueColumnNameComboBox.SelectedIndex = 0;
                longValueColumnNameComboBox.SelectedIndex = 0;

                shortValueColumnNameComboBox.Enabled = false;
                longValueColumnNameComboBox.Enabled = false;
            }
            else
            {
                string tableName = o.ToString();
                string[] cols = GetColumnNames(tableName);

                shortValueColumnNameComboBox.DataSource = PrependBlank(cols);
                shortValueColumnNameComboBox.Enabled = true;
                if (cols.Length > 0)
                    shortValueColumnNameComboBox.SelectedIndex = 1;

                longValueColumnNameComboBox.DataSource = PrependBlank(cols);
                longValueColumnNameComboBox.Enabled = true;
                if (cols.Length > 1)
                    longValueColumnNameComboBox.SelectedIndex = 2;
            }
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

            // Ensure the lookup column name is defined.
            string lookupColumnName = shortValueColumnNameComboBox.SelectedItem.ToString();
            if (lookupColumnName.Length == 0)
            {
                MessageBox.Show("The name of the lookup column must be specified.");
                shortValueColumnNameComboBox.Focus();
                return;
            }

            m_Edit.TableName = tableName;
            m_Edit.LookupColumnName = lookupColumnName;
            m_Edit.ValueColumnName = longValueColumnNameComboBox.SelectedItem.ToString();
            m_Edit.FinishEdit();

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}