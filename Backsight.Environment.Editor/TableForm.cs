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

using System.Diagnostics;
using System.Windows.Forms;
using Backsight.Database;

namespace Backsight.Environment.Editor;

/// <summary>
/// Dialog that lets the user associate a database table with the Backsight
/// editing environment.
/// </summary>
public partial class TableForm : Form
{
    /// <summary>
    /// The table association the user is editing
    /// </summary>
    readonly ITable m_Item;

    /// <summary>
    /// The tables already associated with Backsight
    /// </summary>
    ITable[] m_Tables;

    /// <summary>
    /// The name of the table that the user has selected (relevant only when adding
    /// a new table association).
    /// </summary>
    private string m_TableName;
    
    internal TableForm(ITable? item)
    {
        InitializeComponent();

        m_Item = item ?? EnvironmentRepository.Current.CreateNewItem<ITable>();
        m_TableName = m_Item.TableName;
    }

    private void TableForm_Load(object sender, EventArgs e)
    {
        // Load attribute tables that have already been associated with Backsight
        var repo = EnvironmentRepository.Current;
        m_Tables = repo.Tables.OrderBy(x => x.TableName).ToArray();

        // If we're adding a new table, list the database tables. Otherwise
        // skip to the page that lists column names
        if (String.IsNullOrEmpty(m_Item.TableName))
        {
            LoadTableList();
        }
        else
        {
            wizard.Pages.Remove(tablesPage);
            wizard.NextTo(columnsPage);
        }

        // Display available domains
        domainsListBox.DataSource = repo.DomainTables.OrderBy(x => x.TableName).ToArray();
    }

    void LoadTableList()
    {
        var repo = EnvironmentRepository.Current;
        string[] tableNames = GetUserTables();
        var exclude = new List<string>();

        if (excludeDomainTablesCheckBox.Checked)
            exclude.AddRange(repo.DomainTables.Select(x => x.TableName));

        if (excludeAlreadyAddedCheckBox.Checked)
            exclude.AddRange(repo.Tables.Select(x => x.TableName));

        if (exclude.Count > 0)
            tableNames = tableNames.Except(exclude).ToArray();

        tableList.Items.Clear();
        tableList.Items.AddRange(tableNames);
    }

    private string[] GetUserTables()
    {
        var systemTables = new HashSet<string>()
        {
            "ColumnDomains",
            "DomainTables",
            "EntityTypeSchemas",
            "EntityTypes",
            "Domains",
            "Fonts",
            "IdGroups",
            "Layers",
            "Properties",
            "Schemas",
            "SchemaTemplates",
            "SysId",
            "Templates",
            "Themes",
            "Zones"
        };
        
        return EnvironmentRepository.Current
            .QueryTableNames()
            .Except(systemTables)
            .OrderBy(x => x)
            .ToArray();
    }
    
    private void tablesPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
    {
        string? s = tableList.SelectedItem?.ToString();
        if (s is null)
        {
            MessageBox.Show("You must first select a table");
            e.Page = tablesPage;
        }
        else
        {
            if (m_Tables.Any(x => x.TableName == s))
            {
                MessageBox.Show("The selected table has already been recorded as a data source for Backsight");
                e.Page = tablesPage;
            }
            else
            {
                m_TableName = s;
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
        if (String.IsNullOrEmpty(m_TableName))
        {
            MessageBox.Show("The associated attribute table has not been defined");
            return;
        }
        
        // Ensure ID column has been defined
        string idColumnName = idColumnComboBox.SelectedItem?.ToString() ?? String.Empty;
        if (idColumnName.Length == 0)
        {
            MessageBox.Show("You must specify the name of the column that holds the feature ID");
            idColumnComboBox.Focus();
            e.Page = columnsPage;
            return;
        }
        
        // Save the table association if we're dealing with a new item (if we're doing an update,
        // the table selection page should have been skipped because these fields should never change)
        var repo = EnvironmentRepository.Current;

        if (m_Item.Id == 0)
        {
            var set = repo.GetSetter<ITable, ISetTable>(m_Item);
            set.TableName = m_TableName;
            set.IdColumnName = idColumnName;
            repo.SaveChanges(m_Item, set);
            Debug.Assert(m_Item.Id != 0);
        }
        
        // Figure out whether any new column domains need to be inserted
        var oldColumnDomains = new List<IColumnDomain>(m_Item.ColumnDomains);
        var newColumnDomains = new List<IColumnDomain>();

        foreach (DataGridViewRow row in columnsGrid.Rows)
        {
            string? columnName = row.Cells["dgcColumnName"].FormattedValue?.ToString();

            if (row.Cells["dgcDomain"].Value is IDomainTable dt)
            {
                Debug.Assert(columnName is not null);

                bool wasExisting = oldColumnDomains.RemoveAll(x => x.ColumnName == columnName && x.Domain == dt) != 0; 

                if (!wasExisting)
                    newColumnDomains.Add(repo.CreateColumnDomain(m_Item, columnName, dt));
            }
        }
        
        // Remove any column domains that haven't been accounted for
        foreach (IColumnDomain cd in oldColumnDomains)
            repo.DeleteColumnDomain(cd);
        
        // And save any new ones (we need to do this after removing the old ones, because any change
        // to an associated domain would otherwise lead to a unique constraint violation)
        foreach (IColumnDomain cd in newColumnDomains)
            repo.SaveColumnDomain(cd);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void wizard_CloseFromCancel(object sender, System.ComponentModel.CancelEventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }

    private void columnsPage_ShowFromNext(object sender, EventArgs e)
    {
        columnsGrid.Rows.Clear();
        idColumnComboBox.Items.Clear();

        var columns = EnvironmentRepository.Current.QueryTableColumns(m_TableName).ToArray();

        // Get any domains already associated with the table
        IColumnDomain[] curDomains = m_Item.ColumnDomains;

        columnsGrid.RowCount = columns.Length;

        for (int i=0; i<columnsGrid.RowCount; i++)
        {
            ColumnInfo c = columns[i];
            idColumnComboBox.Items.Add(c.Name);

            DataGridViewRow row = columnsGrid.Rows[i];
            row.Cells["dgcColumnName"].Value = c.Name;

            string dataType = c.DataType.Name;
            if (!c.Nullable)
                dataType += " not null";

            row.Cells["dgcDataType"].Value = dataType;

            // Display any domain previously associated with the column
            IColumnDomain? cd = curDomains.FirstOrDefault(x => x.ColumnName == c.Name);
            if (cd is not null)
                row.Cells["dgcDomain"].Value = cd.Domain;

            row.Tag = c;
        }

        // Nothing initially selected
        columnsGrid.CurrentCell = null;

        // If we have a simple primary key, assume it's the feature ID column
        if (String.IsNullOrEmpty(m_Item.IdColumnName))
        {
            ColumnInfo? pk = GetSimplePrimaryKeyColumn(columns);
            if (pk is not null)
                idColumnComboBox.SelectedItem = pk.Name;
        }
        else
        {
            idColumnComboBox.SelectedItem = m_Item.IdColumnName;
        }
    }

    /// <summary>
    /// Attempts to locate a simple primary key for a table (a key where the
    /// index consists of just one column)
    /// </summary>
    /// <param name="columns">The columns to consider</param>
    /// <returns>The column that defines the primary key (null if the table does
    /// not have a primary key, or it consists of more than one column)</returns>
    private static ColumnInfo? GetSimplePrimaryKeyColumn(ColumnInfo[] columns)
    {
        var pk = columns.Where(x => x.PrimaryKey).ToArray();
        if (pk.Length != 1)
            return null;

        return pk[0];
    }

    private void domainsListBox_DoubleClick(object sender, EventArgs e)
    {
        IDomainTable? dt = GetSelectedDomainTable();
        DataGridViewRow? c = GetSelectedColumn();
        if (dt is not null && c is not null)
            c.Cells["dgcDomain"].Value = dt;
    }

    IDomainTable? GetSelectedDomainTable()
    {
        return domainsListBox.SelectedItem as IDomainTable;
    }

    DataGridViewRow? GetSelectedColumn()
    {
        DataGridViewSelectedRowCollection sel = columnsGrid.SelectedRows;
        if (sel is null || sel.Count == 0)
            return null;
        else
            return sel[0];
    }

    private void setDomainLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        IDomainTable? dt = GetSelectedDomainTable();
        DataGridViewRow? c = GetSelectedColumn();

        if (dt is null)
            MessageBox.Show("You must select the domain table you want to assign");
        else if (c is null)
            MessageBox.Show("You must select the database column the domain should apply to");
        else
            c.Cells["dgcDomain"].Value = dt;
    }

    private void clearDomainLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        DataGridViewRow? c = GetSelectedColumn();
        if (c is null)
            MessageBox.Show("You must first select a database column");
        else
            c.Cells["dgcDomain"].Value = null;
    }
}