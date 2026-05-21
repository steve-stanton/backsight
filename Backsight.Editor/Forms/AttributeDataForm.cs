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

using System.Drawing;
using System.Windows.Forms;
using Backsight.Environment;
using Backsight.Database;

namespace Backsight.Editor.Forms;

/// <written by="Steve Stanton" on="19-FEB-2009"/>
/// <summary>
/// A simple dialog for entering the database attributes for something.
/// </summary>
public partial class AttributeDataForm : Form
{
    #region Static

    /// <summary>
    /// The last record that was saved via this dialog.
    /// </summary>
    private static AttributeRecord? s_LastRecord;

    #endregion

    #region Class data

    /// <summary>
    /// The data entered by the user. 
    /// </summary>
    readonly AttributeRecord m_Record;
    
    /// <summary>
    /// Are we updating an existing row?
    /// </summary>
    readonly bool m_IsUpdate;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="AttributeDataForm"/> for a brand new row of attribute data
    /// </summary>
    /// <param name="t">The table the attribute data is for</param>
    /// <param name="id">The ID that will be assigned to the new label</param>
    public AttributeDataForm(ITable t, string id)
    {
        InitializeComponent();

        if (t==null)
            throw new ArgumentNullException();

        m_Record = EnvironmentRepository.Current.CreateNewRecord(t);
        m_Record.Id = id;
        m_IsUpdate = false;
    }

    /// <summary>
    /// Creates a new <see cref="AttributeDataForm"/> that shows an existing row of attribute data
    /// </summary>
    /// <param name="record">The row to display</param>
    public AttributeDataForm(AttributeRecord record)
    {
        InitializeComponent();

        m_Record = record;
        m_IsUpdate = true;
    }

    #endregion

    /// <summary>
    /// The data specified by the user (irrelevant if the user cancelled from the dialog).
    /// </summary>
    internal AttributeRecord Record => m_Record;
    
    private void AttributeDataForm_Shown(object sender, EventArgs e)
    {
        try
        {
            this.Text = m_Record.Table.TableName;
            updateLabel.Visible = m_IsUpdate;

            if (!m_IsUpdate)
            {
                string id = m_Record.Id;
                
                // Initialize items so they match the values of the last row we processed (if any).
                // Otherwise assign default values that are indicative of the data type.
                if (s_LastRecord?.Table.Id == m_Record.Table.Id)
                    m_Record.Assign(s_LastRecord);
                else
                    m_Record.AssignDefaultValues();
                
                m_Record.Id = id;
            }

            SetGrid();
            grid.Focus();
        }

        catch (Exception ex)
        {
            MessageBox.Show(ex.StackTrace, ex.Message);
        }
    }

    IColumnDomain? FindColumnDomain(string columnName)
    {
        IColumnDomain[] cds = m_Record.Table.ColumnDomains;
        return cds.FirstOrDefault(x =>
            String.Compare(x.ColumnName, columnName, StringComparison.OrdinalIgnoreCase) == 0);
    }

    private void SetGrid()
    {
        grid.Enabled = false;

        grid.RowCount = m_Record.Columns.Length;
        int rowIndex = 0;

        foreach (var c in m_Record.Columns)
        {
            DataGridViewRow row = grid.Rows[rowIndex];
            row.Tag = c;
            row.Cells["dgcColumnName"].Value = c.Name;

            DataGridViewCell cell = row.Cells["dgcValue"];
            cell.ValueType = c.DataType;
            cell.Value = m_Record.Content.GetValueOrDefault(c.Name);
            
            // Disallow editing of the feature ID
            if (String.Compare(c.Name, m_Record.Table.IdColumnName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                cell.Value = m_Record.Id;
                cell.ReadOnly = true;
                DataGridViewCellStyle readStyle = new DataGridViewCellStyle(grid.DefaultCellStyle);
                readStyle.BackColor = Color.LightGray;
                cell.Style = readStyle;
            }

            rowIndex++;
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
        this.DialogResult = DialogResult.Cancel;
        Close();
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in grid.Rows)
        {
            var c = (ColumnInfo)row.Tag;
            DataGridViewCell valueCell = row.Cells["dgcValue"];
            m_Record.Content[c.Name] = valueCell.Value;
        }

        // Remember the row as-entered (and the table involved) - we'll
        // use it the data to initialize default values the next time this
        // dialog is displayed)
        s_LastRecord = m_Record;

        DialogResult = DialogResult.OK;
        Close();
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
        var dc = (ColumnInfo)row.Tag;
        dataTypeLabel.Text = dc.DataType.Name;

        if (!dc.Nullable)
            dataTypeLabel.Text += " not null";

        // Show any domain values
        IColumnDomain? cd = FindColumnDomain(dc.Name);
        domainValuesLabel.Enabled = cd is not null;

        if (cd is not null)
        {
            // Note the currently defined value
            string currentValue = row.Cells["dgcValue"].FormattedValue.ToString();
            DataGridViewCell currentCell = null;

            IDomainTable domainTable = cd.Domain;
            string[] lookups = domainTable.GetLookupValues();
            domainGrid.RowCount = lookups.Length;
            for (int i=0; i<lookups.Length; i++)
            {
                string shortValue = lookups[i];
                DataGridViewRow r = domainGrid.Rows[i];
                r.Tag = shortValue;
                r.Cells["dgcShortValue"].Value = shortValue;
                r.Cells["dgcLongValue"].Value = domainTable.Lookup(shortValue);

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