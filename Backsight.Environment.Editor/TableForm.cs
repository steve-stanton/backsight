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

using Backsight.SqlServer;

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Dialog that lets the user associate a database table with the Backsight
    /// editing environment.
    /// </summary>
    public partial class TableForm : Form
    {
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

        private void TableForm_Shown(object sender, EventArgs e)
        {
            // Load tables that have already been associated with Backsight
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            m_Tables = ec.Tables;

            // Load the database tables in the current database (excluding all
            // Backsight system tables)
            string[] tableNames = new TableFactory().GetUserTables();
            tableComboBox.DataSource = tableNames;

            alreadyAddedLabel.Visible = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string t = tableComboBox.Text.Trim();
            if (t.Length==0)
            {
                MessageBox.Show("You must first select a table name");
                tableComboBox.Focus();
                return;
            }

            m_Edit.TableName = t;
            m_Edit.FinishEdit();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void tableComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            alreadyAddedLabel.Visible = false;

            if (m_Tables==null)
                return;

            object o = tableComboBox.SelectedItem;
            if (o==null)
                return;

            string s = o.ToString();
            ITable addedTable = Array.Find<ITable>(m_Tables, delegate(ITable t)
                                    { return t.TableName==s; });
            alreadyAddedLabel.Visible = (addedTable!=null);
        }
    }
}