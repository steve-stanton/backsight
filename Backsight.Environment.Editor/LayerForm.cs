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

public partial class LayerForm : Form
{
    private readonly ILayer m_Item;

    internal LayerForm() : this(null)
    {
    }

    internal LayerForm(ILayer? item)
    {
        InitializeComponent();

        m_Item = item ?? EnvironmentRepository.Current.CreateNewItem<ILayer>();
    }

    private void LayerForm_Shown(object sender, EventArgs e)
    {
        nameTextBox.Text = m_Item.Name;
        
        IEntity[] all = EnvironmentRepository.Current.EntityTypes.OrderBy(x => x.Name).ToArray();
        pointComboBox.Items.AddRange(all.Where(x => x.IsPointValid).ToArray<object>());
        lineComboBox.Items.AddRange(all.Where(x => x.IsLineValid).ToArray<object>());
        textComboBox.Items.AddRange(all.Where(x => x.IsTextValid).ToArray<object>());
        polygonComboBox.Items.AddRange(all.Where(x => x.IsPolygonValid).ToArray<object>());

        if (m_Item.DefaultPointType is not null)
            pointComboBox.SelectedItem = m_Item.DefaultPointType;

        if (m_Item.DefaultLineType is not null)
            lineComboBox.SelectedItem = m_Item.DefaultLineType;

        if (m_Item.DefaultTextType is not null)
            textComboBox.SelectedItem = m_Item.DefaultTextType;

        if (m_Item.DefaultPolygonType is not null)
            polygonComboBox.SelectedItem = m_Item.DefaultPolygonType;

        ITheme theme = m_Item.Theme;
        if (theme is not null)
        {
            themeLabel.Visible = true;
            themeTextBox.Visible = true;
            themeTextBox.Text = theme.Name;
        }
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        if (!ValidateEdit())
            return;

        var repo = EnvironmentRepository.Current;
        var set = repo.GetSetter<ILayer, ISetLayer>(m_Item);
        
        set.Name = nameTextBox.Text.Trim();
        set.DefaultPointType = (IEntity)pointComboBox.SelectedItem;
        set.DefaultLineType = (IEntity)lineComboBox.SelectedItem;
        set.DefaultTextType = (IEntity)textComboBox.SelectedItem;
        set.DefaultPolygonType = (IEntity)polygonComboBox.SelectedItem;
        
        repo.SaveChanges(m_Item, set);
        
        DialogResult = DialogResult.OK;
        Close();
    }

    bool ValidateEdit()
    {
        string name = nameTextBox.Text.Trim();
        if (name.Length==0)
        {
            MessageBox.Show("A name must be supplied for the layer");
            nameTextBox.Focus();
            return false;
        }

        return true;
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}