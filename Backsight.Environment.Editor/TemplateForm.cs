// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
/// Dialog that lets the user define templates for text formatting
/// </summary>
public partial class TemplateForm : Form
{
    readonly ITemplate m_Item;

    /// <summary>
    /// The table (if any) that refers to the template (may be null while creating a new template).
    /// </summary>
    ITable? m_Table;
    
    internal TemplateForm(ITemplate? item)
    {
        InitializeComponent();

        m_Item = item ?? EnvironmentRepository.Current.CreateNewItem<ITemplate>();
        m_Table = null;
    }

    private void TemplateForm_Shown(object sender, EventArgs e)
    {
        // Load the schema combo (without the <none> item). Note that
        // this will end up calling tableComboBox_SelectedValueChanged,
        // which will set m_Table to be the first table in the array.
        ITable[] tables = EnvironmentRepository.Current.Tables.OrderBy(x => x.TableName).ToArray();
        tableComboBox.DataSource = tables;

        // The associated table will initially be undefined when creating a new template
        if (m_Item.Id == 0)
        {
            tableComboBox.SelectedItem = null;
            fieldsListBox.Enabled = false;
            formatTextBox.Enabled = false;
            nameTextBox.Focus();
            return;
        }

        // Define alternate title for the dialog window
        this.Text = "Update Text Template";

        // Display the name of the item.
        nameTextBox.Text = m_Item.Name;

        // Select the associated table. Then load the list of fields, and select
        // those that the template already uses.
        tableComboBox.SelectedItem = m_Item.Schema;
        Debug.Assert(m_Table == m_Item.Schema);
        ListFields();

        // Display the current format.
        formatTextBox.Text = m_Item.Format;

        // Set focus on the OK button.
        okButton.Focus();
    }

    void ListFields()
    {
        // The list should be enabled.
        fieldsListBox.Enabled = m_Table is not null;
        fieldsListBox.DataSource = null;

        if (m_Table is not null)
        {
            var columns = EnvironmentRepository.Current
                .QueryTableColumns(m_Table.TableName)
                .Select(x => x.Name)
                .ToArray();

            // You can only poke the columns into the ListBox via a BindingSource, don't know why, don't care
            BindingSource bs = new BindingSource();
            bs.DataSource = columns;
            fieldsListBox.DataSource = bs;
        }
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        // Ensure the template name is defined
        string name = nameTextBox.Text.Trim();
        if (name.Length == 0)
        {
            MessageBox.Show("A template name must be specified");
            nameTextBox.Focus();
            return;
        }

        // Ensure the schema is defined.
        if (m_Table is null)
        {
            MessageBox.Show("The template must be related to a table.");
            tableComboBox.Focus();
            return;
        }

        // Ensure the format is defined
        string fmt = formatTextBox.Text.TrimEnd();
        if (fmt.Length == 0)
        {
            MessageBox.Show("The text formatting instructions have not been specified");
            formatTextBox.Focus();
            return;
        }

        var repo = EnvironmentRepository.Current;
        var set = repo.GetSetter<ITemplate, ISetTemplate>(m_Item);
        
        set.Name = name;
        set.Format = fmt;
        set.Schema = m_Table;

        repo.SaveChanges(m_Item, set);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void fieldsListBox_DoubleClick(object sender, EventArgs e)
    {
        OnSelect();
    }

    private void selectButton_Click(object sender, EventArgs e)
    {
        OnSelect();
    }

    void OnSelect()
    {
        // Get the ID of the selected field.
        var col = fieldsListBox.SelectedItem?.ToString();
        if (col is null)
            return;

        // Append the column name to the format
        formatTextBox.Text += $"[{col}]";
    }

    private void tableComboBox_SelectedValueChanged(object sender, EventArgs e)
    {
        // Get the selected schema.
        m_Table = (ITable?)tableComboBox.SelectedItem;

        // And list the fields
        ListFields();

        // Clear out the current format and ensure the control is enabled.
        formatTextBox.Text = String.Empty;
        formatTextBox.Enabled = true;
    }
}