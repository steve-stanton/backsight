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
using System.Collections.Generic;

using Smo=Microsoft.SqlServer.Management.Smo;

using Backsight.SqlServer;

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Dialog that lets the user associate a database table with the Backsight
    /// editing environment.
    /// </summary>
    public partial class TableForm : Form
    {
        /// <summary>
        /// The table association the user is editing
        /// </summary>
        readonly IEditTable m_Edit;

        /// <summary>
        /// The tables already associated with Backsight
        /// </summary>
        ITable[] m_Tables;

        internal TableForm() : this(null)
        {
        }

        internal TableForm(IEditTable edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit == null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateTableAssociation();
            }

            m_Edit.BeginEdit();
        }

        private void TableForm_Load(object sender, EventArgs e)
        {
            // Load tables that have already been associated with Backsight
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            m_Tables = ec.Tables;

            // If we're adding a new table, list the database tables. Otherwise
            // skip to the page that lists column names
            if (String.IsNullOrEmpty(m_Edit.TableName))
                LoadTableList();
            else
            {
                wizard.Pages.Remove(tablesPage);
                wizard.NextTo(columnsPage);
            }

            // Display available domains
            domainsListBox.DataSource = ec.DomainTables;
        }

        void LoadTableList()
        {
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            string[] tableNames = new TableFactory().GetUserTables();
            List<string> exclude = new List<string>();

            if (excludeDomainTablesCheckBox.Checked)
            {
                IDomainTable[] domainTables = ec.DomainTables;
                foreach (IDomainTable t in domainTables)
                    exclude.Add(t.TableName);
            }

            if (excludeAlreadyAddedCheckBox.Checked)
            {
                ITable[] tables = ec.Tables;
                foreach (ITable t in tables)
                    exclude.Add(t.TableName);
            }

            if (exclude.Count > 0)
            {
                tableNames = Array.FindAll<string>(tableNames, delegate(string s)
                                                    { return !exclude.Contains(s); });
            }

            tableList.Items.Clear();
            tableList.Items.AddRange(tableNames);
        }

        private void tablesPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
        {
            object o = tableList.SelectedItem;
            if (o == null)
            {
                MessageBox.Show("You must first select a table");
                e.Page = tablesPage;
            }
            else
            {
                string s = o.ToString();
                if (Array.Exists<ITable>(m_Tables, delegate(ITable t) { return t.TableName==s; }))
                {
                    MessageBox.Show("The selected table has already been recorded as a data source for Backsight");
                    e.Page = tablesPage;
                }
                else
                {
                    m_Edit.TableName = s;
                }
            }
        }

        private void excludeDomainTablesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadTableList();
        }

        private void excludeAlreadyAddedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadTableList();
        }

        private void columnsPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
        {
            // Ensure ID column has been defined
            string idColumnName = (idColumnComboBox.SelectedItem == null ? String.Empty : idColumnComboBox.SelectedItem.ToString());
            if (idColumnName.Length == 0)
            {
                MessageBox.Show("You must specify the name of the column that holds the feature ID");
                idColumnComboBox.Focus();
                e.Page = columnsPage;
                return;
            }

            m_Edit.IdColumnName = idColumnName;

            // Ensure column domains are up to date.
            // For the time being, we do NOT establish or remove foreign keys in the
            // database - if that is considered desirable, bear in mind that the changes
            // being saved here may ultimately be discarded by the user (on exit from the
            // application).

            List<IColumnDomain> cds = new List<IColumnDomain>();
            IEnvironmentFactory factory = EnvironmentContainer.Factory;

            foreach (DataGridViewRow row in columnsGrid.Rows)
            {
                IDomainTable dt = (row.Cells["dgcDomain"].Value as IDomainTable);

                if (dt != null)
                {
                    IEditColumnDomain cd = factory.CreateColumnDomain();
                    cd.ParentTable = m_Edit;
                    cd.ColumnName = row.Cells["dgcColumnName"].FormattedValue.ToString();
                    cd.Domain = dt;

                    cds.Add(cd);
                }
            }

            m_Edit.ColumnDomains = cds.ToArray();
            m_Edit.FinishEdit();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void wizard_CloseFromCancel(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
        }

        private void columnsPage_ShowFromNext(object sender, EventArgs e)
        {
            columnsGrid.Rows.Clear();
            idColumnComboBox.Items.Clear();

            TableFactory tf = new TableFactory();
            string tableName = m_Edit.TableName;
            Smo.Table t = tf.FindTableByName(tableName);

            if (t == null)
                return;

            // Get any domains already associated with the table
            IColumnDomain[] curDomains = m_Edit.ColumnDomains;

            columnsGrid.RowCount = t.Columns.Count;

            for (int i=0; i<columnsGrid.RowCount; i++)
            {
                Smo.Column c = t.Columns[i];
                idColumnComboBox.Items.Add(c.Name);

                DataGridViewRow row = columnsGrid.Rows[i];
                row.Cells["dgcColumnName"].Value = c.Name;

                Smo.DataType dt = c.DataType;
                string dataType = dt.SqlDataType.ToString().ToLower();

                if (dt.SqlDataType == Smo.SqlDataType.Char ||
                    dt.SqlDataType == Smo.SqlDataType.NChar ||
                    dt.SqlDataType == Smo.SqlDataType.VarChar ||
                    dt.SqlDataType == Smo.SqlDataType.NVarChar)
                    dataType += String.Format("({0})", dt.MaximumLength);

                if (!c.Nullable)
                    dataType += " not null";

                row.Cells["dgcDataType"].Value = dataType;

                // Display any domain previously associated with the column
                IColumnDomain cd = Array.Find<IColumnDomain>(curDomains,
                    delegate(IColumnDomain tcd) { return tcd.ColumnName == c.Name; });
                if (cd != null)
                    row.Cells["dgcDomain"].Value = cd.Domain;

                row.Tag = c;
            }

            // Nothing initially selected
            columnsGrid.CurrentCell = null;

            // If we have a simple primary key, assume it's the feature ID column
            if (String.IsNullOrEmpty(m_Edit.IdColumnName))
            {
                Smo.Column pk = TableFactory.GetSimplePrimaryKeyColumn(t);
                if (pk != null)
                    idColumnComboBox.SelectedItem = pk.Name;
            }
            else
            {
                idColumnComboBox.SelectedItem = m_Edit.IdColumnName;
            }
        }

        private void domainsListBox_DoubleClick(object sender, EventArgs e)
        {
            IDomainTable dt = GetSelectedDomainTable();
            DataGridViewRow c = GetSelectedColumn();
            if (dt !=null && c != null)
                c.Cells["dgcDomain"].Value = dt;
        }

        IDomainTable GetSelectedDomainTable()
        {
            return (domainsListBox.SelectedItem as IDomainTable);
        }

        DataGridViewRow GetSelectedColumn()
        {
            DataGridViewSelectedRowCollection sel = columnsGrid.SelectedRows;
            if (sel == null || sel.Count == 0)
                return null;
            else
                return sel[0];
        }

        private void setDomainLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IDomainTable dt = GetSelectedDomainTable();
            DataGridViewRow c = GetSelectedColumn();

            if (dt == null)
                MessageBox.Show("You must select the domain table you want to assign");
            else if (c == null)
                MessageBox.Show("You must select the database column the domain should apply to");
            else
                c.Cells["dgcDomain"].Value = dt;
        }

        private void clearDomainLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataGridViewRow c = GetSelectedColumn();
            if (c == null)
                MessageBox.Show("You must first select a database column");
            else
                c.Cells["dgcDomain"].Value = null;
        }
    }
}