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

using System.Windows.Forms;
using Backsight.Database;

namespace Backsight.Environment.Editor;

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
        // Load the database tables in the current database (excluding all Backsight system tables)
        var repo = EnvironmentRepository.Current;
        string[] tableNames = repo.GetUserTables();

        // Grab the currently defined domain tables
        IDomainTable[] currentDomains = repo.DomainTables.ToArray();

        var domains = new List<string>();

        // Include only those tables that have the required columns.
        // Exclude tables that have already been defined as domain tables.

        foreach (string t in tableNames)
        {
            var isDomainTable = currentDomains.Any(x => String.Compare(x.TableName, t, StringComparison.OrdinalIgnoreCase) == 0);
            
            if (!isDomainTable)
            {
                string[] cols = repo.QueryTableColumns(t).Select(x => x.Name).ToArray();

                if (cols.Any(x => String.Compare(x, "ShortValue", StringComparison.OrdinalIgnoreCase) == 0) &&
                    cols.Any(x => String.Compare(x, "LongValue", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    domains.Add(t);
                }
            }
        }

        if (domains.Count == 0)
        {
            MessageBox.Show("No suitable domain tables found in the current database");
            DialogResult = DialogResult.Cancel;
            Close();
        }
        else
        {
            tableNameComboBox.DataSource = domains;
        }
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        // Ensure the table name is defined
        string? tableName = tableNameComboBox.SelectedItem?.ToString();
        if (String.IsNullOrWhiteSpace(tableName))
        {
            MessageBox.Show("The name of the domain table must be specified");
            tableNameComboBox.Focus();
            return;
        }

        var repo = EnvironmentRepository.Current;
        var item = repo.CreateNewItem<IDomainTable>();
        var set = repo.GetSetter<IDomainTable, ISetDomainTable>(item);
        set.TableName = tableName;
        repo.SaveChanges(item, set);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}