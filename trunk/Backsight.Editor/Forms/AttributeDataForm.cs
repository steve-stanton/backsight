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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Backsight.Environment;
using Backsight.Data;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="19-FEB-2009"/>
    /// <summary>
    /// A simple dialog for entering the database attributes for something.
    /// </summary>
    public partial class AttributeDataForm : Form
    {
        #region Static

        static ITable s_LastTable;

        /// <summary>
        /// The last row that was entered via this dialog
        /// </summary>
        static object[] s_LastItems;

        #endregion

        #region Class data
        
        /// <summary>
        /// The database holding the attribute data (not null).
        /// </summary>
        readonly IDataServer m_DataServer;

        /// <summary>
        /// The table the attribute data is for
        /// </summary>
        readonly ITable m_Table;

        /// <summary>
        /// The ID that will be assigned to the new label
        /// </summary>
        readonly string m_Id;

        /// <summary>
        /// The data entered by the user. 
        /// </summary>
        DataRow m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="AttributeDataForm"/> for a brand new row of attribute data
        /// </summary>
        /// <param name="t">The table the attribute data is for</param>
        /// <param name="id">The ID that will be assigned to the new label</param>
        /// <exception cref="InvalidOperationException">If no database is available</exception>
        public AttributeDataForm(ITable t, string id)
        {
            InitializeComponent();

            if (t==null)
                throw new ArgumentNullException();

            m_Table = t;
            m_Id = id;
            m_Data = null;
            m_DataServer = EditingController.Current.DataServer;

            if (m_DataServer == null)
                throw new InvalidOperationException("No database available");
        }

        /// <summary>
        /// Creates a new <see cref="AttributeDataForm"/> that shows an existing row of attribute data
        /// </summary>
        /// <param name="t">The table the attribute data is for</param>
        /// <param name="row">The row to display</param>
        public AttributeDataForm(ITable t, DataRow row)
        {
            InitializeComponent();

            if (t==null || row==null)
                throw new ArgumentNullException();

            m_Table = t;
            m_Id = row[t.IdColumnName].ToString();
            m_Data = row;
        }

        #endregion

        private void AttributeDataForm_Shown(object sender, EventArgs e)
        {
            try
            {
                this.Text = m_Table.TableName;
                updateLabel.Visible = (m_Data != null);

                if (m_Data == null)
                {
                    DataRow data = DbUtil.CreateNewRow(m_Table);

                    // Initialize items so they match the values of the last row we processed (if any).
                    // Otherwise assign default values that are indicative of the data type.
                    if (s_LastTable != null && s_LastTable.Id == m_Table.Id)
                    {
                        Debug.Assert(s_LastItems != null);
                        Debug.Assert(s_LastItems.Length == data.Table.Columns.Count);
                        data.ItemArray = s_LastItems;
                    }
                    else
                    {
                        AssignDefaultValues(data);
                    }

                    SetGrid(data);
                }
                else
                {
                    SetGrid(m_Data);
                }

                grid.Focus();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
        }

        /// <summary>
        /// Assigns default values to fields in a new row of attribute data. Numeric fields are
        /// assigned a value of 0 (even though this could conceivably break check constraints).
        /// Fields associated with a domain table are assigned the first lookup value (whatever
        /// that is). Any other [var]char field that is not nullable will be assigned a blank value.
        /// </summary>
        /// <param name="data">The data to assign defaults to</param>
        void AssignDefaultValues(DataRow data)
        {
            DataTable dt = data.Table;
            IColumnDomain[] cds = m_Table.ColumnDomains;

            foreach (DataColumn dc in dt.Columns)
            {
                Type t = dc.DataType;
                if (t==typeof(int) || t==typeof(short) || t==typeof(byte))
                    data[dc] = 0;
                else if (t==typeof(double) || t==typeof(float))
                    data[dc] = 0.0;
                else if (t==typeof(string))
                {
                    IColumnDomain cd = FindColumnDomain(dc.ColumnName);

                    if (cd != null)
                    {
                        string[] vals = cd.Domain.GetLookupValues(m_DataServer.ConnectionString);
                        if (vals.Length > 0)
                            data[dc] = vals[0];
                    }
                    else if (!dc.AllowDBNull)
                    {
                        // Default to a blank string is the field isn't nullable                        
                        data[dc] = String.Empty;
                    }
                }
            }
        }

        IColumnDomain FindColumnDomain(string columnName)
        {
            IColumnDomain[] cds = m_Table.ColumnDomains;
            return Array.Find<IColumnDomain>(cds, t => String.Compare(t.ColumnName, columnName, true)==0);
        }

        void SetGrid(DataRow data)
        {
            grid.Enabled = false;
            m_Data = data;
            DataTable table = data.Table;
            object[] items = data.ItemArray;

            grid.RowCount = items.Length;

            for (int i=0; i<items.Length; i++)
            {
                DataGridViewRow row = grid.Rows[i];
                DataColumn dc = table.Columns[i];
                row.Tag = dc;
                row.Cells["dgcColumnName"].Value = dc.ColumnName;

                DataGridViewCell cell = row.Cells["dgcValue"];
                cell.ValueType = dc.DataType;
                cell.Value = items[i];

                if (dc.DataType == typeof(string))
                {
                    DataGridViewTextBoxCell textCell = (DataGridViewTextBoxCell)cell;
                    textCell.MaxInputLength = dc.MaxLength;
                }

                // Disallow editing of the feature ID
                if (String.Compare(dc.ColumnName, m_Table.IdColumnName, true) == 0)
                {
                    cell.Value = m_Id;
                    cell.ReadOnly = true;
                    DataGridViewCellStyle readStyle = new DataGridViewCellStyle(grid.DefaultCellStyle);
                    readStyle.BackColor = Color.LightGray;
                    cell.Style = readStyle;
                }
            }

            // Enable the grid now (if the first editable cell selected below relates to
            // a domain table, we want to domain values to show up)
            grid.Enabled = true;

            // Select the first editable cell (the first cell is frequently the ID, which
            // the user is not expected to edit).
            //SelectFirstEditableValue();
            grid.CurrentCell = grid.Rows[0].Cells["dgcValue"];
        }

        /*
        void SelectFirstEditableValue()
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                DataGridViewCell cell = row.Cells["dgcValue"];
                if (!cell.ReadOnly)
                {
                    grid.CurrentCell = cell;
                    return;
                }
            }
        }
        */

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Data = null;
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                DataGridViewCell valueCell = row.Cells["dgcValue"];

                // If we're dealing with a varchar column, ensure the entered value isn't too long (this
                // should have been covered via the MaxInputLength assigned to the grid cell, but we'll
                // check again just in case)
                DataColumn dc = (DataColumn)row.Tag;
                if (dc.DataType == typeof(string))
                {
                    string s = valueCell.FormattedValue.ToString();
                    if (s.Length > dc.MaxLength)
                    {
                        string msg = String.Format("Value for {0} is too long (maximum is {1} characters)",
                                                dc.ColumnName, dc.MaxLength);
                        MessageBox.Show(msg, "Text too long");
                        grid.CurrentCell = row.Cells["dgcValue"];
                        return;
                    }
                }

                m_Data[dc] = valueCell.Value;
            }

            // Remember the row as-entered (and the table involved) - we'll
            // use it the data to initialize default values the next time this
            // dialog is displayed)
            s_LastTable = m_Table;
            s_LastItems = m_Data.ItemArray;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// The data specified by the user (null if the user cancelled from
        /// the dialog)
        /// </summary>
        internal DataRow Data
        {
            get { return m_Data; }
        }

        DataGridViewRow GetSelectedGridRow()
        {
            DataGridViewSelectedCellCollection sel = grid.SelectedCells;
            if (sel==null || sel.Count==0)
                return null;

            DataGridViewCell cell = sel[0];
            return grid.Rows[cell.RowIndex];
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            domainGrid.Tag = null;
            domainGrid.Rows.Clear();
            domainGrid.Enabled = false;

            if (!grid.Enabled)
                return;

            DataGridViewRow row = GetSelectedGridRow();
            if (row == null)
                return;

            // Show the data type
            DataColumn dc = (DataColumn)row.Tag;
            dataTypeLabel.Text = dc.DataType.Name;

            if (!dc.AllowDBNull)
                dataTypeLabel.Text += " not null";

            if (dc.DataType == typeof(string))
            {
                if (dc.MaxLength == 1)
                    dataTypeLabel.Text += " (1 character)";
                else
                    dataTypeLabel.Text += String.Format(" (up to {0} characters)", dc.MaxLength);
            }

            // Show any domain values
            IColumnDomain cd = FindColumnDomain(dc.ColumnName);
            domainValuesLabel.Enabled = (cd != null);

            if (cd != null)
            {
                // Note the currently defined value
                string currentValue = row.Cells["dgcValue"].FormattedValue.ToString();
                DataGridViewCell currentCell = null;

                IDomainTable domainTable = cd.Domain;
                string[] lookups = domainTable.GetLookupValues(m_DataServer.ConnectionString);
                domainGrid.RowCount = lookups.Length;
                for (int i=0; i<lookups.Length; i++)
                {
                    string shortValue = lookups[i];
                    DataGridViewRow r = domainGrid.Rows[i];
                    r.Tag = shortValue;
                    r.Cells["dgcShortValue"].Value = shortValue;
                    r.Cells["dgcLongValue"].Value = domainTable.Lookup(m_DataServer.ConnectionString, shortValue);

                    // If we have just defined the current data value, remember the cell so
                    // that we can set it once the grid has been loaded.
                    if (shortValue == currentValue)
                        currentCell = r.Cells["dgcShortValue"];
                }

                domainGrid.CurrentCell = currentCell;
                domainGrid.Enabled = true;
                domainGrid.Tag = domainTable;
            }
        }

        private void domainGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (!domainGrid.Enabled)
                return;

            DataGridViewRow row = GetSelectedGridRow();
            if (row == null)
                return;

            DataGridViewSelectedRowCollection sel = domainGrid.SelectedRows;
            if (sel==null || sel.Count==0)
                return;

            string shortValue = sel[0].Tag.ToString();
            row.Cells["dgcValue"].Value = shortValue;
        }

        private void grid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Don't enter readonly cells
            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            DataGridViewCell cell = grid.Rows[rowIndex].Cells[colIndex];
            if (cell.ReadOnly)
                SendKeys.Send("{Tab}");
        }
    }
}