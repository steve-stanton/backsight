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

using Backsight.Data.BacksightDataSetTableAdapters;
using Backsight.Data;

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Dialog for displaying and editing general environment properties.
    /// </summary>
    public partial class PropertyListForm : Form
    {
        /// <summary>
        /// The data displayed in the property grid.
        /// </summary>
        BacksightDataSet.PropertyDataTable m_Data;

        public PropertyListForm()
        {
            InitializeComponent();

            m_Data = null;
        }

        private void PropertyListForm_Shown(object sender, EventArgs e)
        {
            PropertyTableAdapter ta = AdapterFactory.Create<PropertyTableAdapter>();
            m_Data = ta.GetData();

            // If we don't have mandatory properties, add them now with blank values
            string[] props = PropertyNaming.MandatoryProperties;
            foreach (string p in props)
            {
                if (m_Data.FindByName(p)==null)
                {
                    BacksightDataSet.PropertyRow row = m_Data.NewPropertyRow();
                    row.Name = p;
                    row.Value = row.Description = String.Empty;
                    m_Data.AddPropertyRow(row);
                }
            }

            propertyGrid.AutoGenerateColumns = false;
            propertyGrid.DataSource = m_Data;
            propertyGrid.CurrentCell = null;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            PropertyForm dial = new PropertyForm(String.Empty, String.Empty);
            if (dial.ShowDialog() == DialogResult.OK)
            {
                BacksightDataSet.PropertyRow row = m_Data.NewPropertyRow();
                row.Name = dial.PropertyName;
                row.Value = dial.PropertyValue;
                row.Description = String.Empty;
                m_Data.AddPropertyRow(row);
            }
            dial.Dispose();            
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow sel = GetSelectedRow();
            if (sel==null)
            {
                MessageBox.Show("You must first select the property to update");
                return;
            }

            UpdateProperty(sel);
        }

        private void propertyGrid_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewRow sel = GetSelectedRow();
            if (sel!=null)
                UpdateProperty(sel);
        }

        void UpdateProperty(DataGridViewRow row)
        {
            BacksightDataSet.PropertyRow dbRow = GetDatabaseRow(row);
            if (dbRow!=null)
            {
                PropertyForm dial = new PropertyForm(dbRow.Name, dbRow.Value);
                if (dial.ShowDialog() == DialogResult.OK)
                    dbRow.Value = dial.PropertyValue;
                dial.Dispose();
            }
        }

        BacksightDataSet.PropertyRow GetDatabaseRow(DataGridViewRow gridRow)
        {
            string name = gridRow.Cells[0].Value.ToString();
            return m_Data.FindByName(name);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = GetSelectedRow();
            if (row==null)
            {
                MessageBox.Show("You must first select the property to delete");
                return;
            }

            BacksightDataSet.PropertyRow dbRow = GetDatabaseRow(row);
            if (dbRow!=null)
                dbRow.Delete();
        }

        DataGridViewRow GetSelectedRow()
        {
            DataGridViewSelectedRowCollection sel = propertyGrid.SelectedRows;
            if (sel.Count==0)
                return null;
            else
                return sel[0];
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (m_Data!=null)
            {
                PropertyTableAdapter ta = AdapterFactory.Create<PropertyTableAdapter>();
                ta.Update(m_Data);
            }

            Close();
        }
    }
}