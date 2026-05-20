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

using System.Windows.Forms;
using Backsight.Database;

namespace Backsight.Environment.Editor;

public partial class EntityForm : Form
{
    /// <summary>
    /// The entity type that's being edited
    /// </summary>
    private readonly IEntity m_Item;

    /// <summary>
    /// The tables (if any) that are associated with the entity type (null if
    /// the list hasn't been edited).
    /// </summary>
    private ITable[]? m_DefaultTables;

    /// <summary>
    /// Creates a new <c>EntityForm</c> for the specified entity type.
    /// </summary>
    /// <param name="item">The entity type to update (specify null to create a new entity type).</param>
    internal EntityForm(IEntity? item)
    {
        InitializeComponent();

        m_Item = item ?? EnvironmentRepository.Current.CreateNewItem<IEntity>();
        m_DefaultTables = null;
    }

    private void EntityForm_Shown(object sender, EventArgs e)
    {
        var repo = EnvironmentRepository.Current;
        idGroupComboBox.Items.AddRange(repo.IdGroups.Cast<object>().ToArray());
        layerComboBox.Items.AddRange(repo.Layers.Cast<object>().ToArray());
        fontComboBox.Items.AddRange(repo.Fonts.Cast<object>().ToArray());

        IIdGroup? g = m_Item.IdGroup;
        if (g is not null)
            idGroupComboBox.SelectedItem = g;

        ILayer? layer = m_Item.Layer;
        if (layer is not null)
            layerComboBox.SelectedItem = layer;

        IFont? font = m_Item.Font;
        if (font is not null)
            fontComboBox.SelectedItem = font;

        entityNameTextBox.Text = m_Item.Name;

        pointCheckbox.Checked = m_Item.IsPointValid;
        lineCheckbox.Checked = m_Item.IsLineValid;
        boundaryCheckbox.Checked = m_Item.IsPolygonBoundaryValid;
        textCheckbox.Checked = m_Item.IsTextValid;
        labelCheckbox.Checked = m_Item.IsPolygonValid;

        labelCheckbox.Enabled = textCheckbox.Checked;
        boundaryCheckbox.Enabled = lineCheckbox.Checked;
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        if (entityNameTextBox.Text.Trim().Length==0)
        {
            MessageBox.Show("The name for an entity type must be specified");
            entityNameTextBox.Focus();
            return;
        }
        
        var repo = EnvironmentRepository.Current;
        var set = repo.GetSetter<IEntity, ISetEntity>(m_Item);

        set.Name = entityNameTextBox.Text;
        set.IsPointValid = pointCheckbox.Checked;
        set.IsLineValid = lineCheckbox.Checked;
        set.IsLineAutoTrimmed = false;
        set.IsPolygonBoundaryValid = boundaryCheckbox.Checked;
        set.IsTextValid = textCheckbox.Checked;
        set.IsPolygonValid = labelCheckbox.Checked;
        set.IdGroup = (IIdGroup?)idGroupComboBox.SelectedItem;
        set.Layer = (ILayer?)layerComboBox.SelectedItem;
        set.Font = (IFont?)fontComboBox.SelectedItem;
        
        repo.SaveChanges(m_Item, set);
        
        // Update the associated tables separately (needs the entity type ID when dealing with a new instance)
        if (m_DefaultTables is not null)
            repo.SaveAssociatedTables(m_Item, m_DefaultTables);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void textCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        labelCheckbox.Enabled = textCheckbox.Checked;
        if (!labelCheckbox.Enabled)
            labelCheckbox.Checked = false;

        fontLabel.Enabled = textCheckbox.Checked;
        fontComboBox.Enabled = textCheckbox.Checked;

        if (!fontComboBox.Enabled)
            fontComboBox.SelectedItem = null;
    }

    private void lineCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        boundaryCheckbox.Enabled = lineCheckbox.Checked;
        if (!boundaryCheckbox.Enabled)
            boundaryCheckbox.Checked = false;
    }

    private void tablesButton_Click(object sender, EventArgs e)
    {
        var repo = EnvironmentRepository.Current;
        
        // If this is the first time the button has been clicked, load up current associations
        if (m_DefaultTables is null)
            m_DefaultTables = repo.FindAssociatedTables(m_Item).ToArray();

        // Grab the complete table list
        ITable[] tables = repo.Tables.ToArray();

        ChecklistForm<ITable> dial = new ChecklistForm<ITable>(tables, m_DefaultTables);
        if (dial.ShowDialog() == DialogResult.OK)
            m_DefaultTables = dial.Selection;

        dial.Dispose();
    }
}