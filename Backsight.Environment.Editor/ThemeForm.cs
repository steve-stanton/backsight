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
using System.Diagnostics;
using Backsight.Database;

namespace Backsight.Environment.Editor;

public partial class ThemeForm : Form
{
    private readonly ITheme m_Item;

    /// <summary>
    /// The initial set of layers for the theme (base layer first)
    /// </summary>
    private readonly ILayer[] m_OriginalLayers;

    internal ThemeForm(ITheme? item)
    {
        InitializeComponent();

        m_Item = item ?? EnvironmentRepository.Current.CreateNewItem<ITheme>();
        m_OriginalLayers = m_Item.Layers;
    }

    private void ThemeForm_Shown(object sender, EventArgs e)
    {
        nameTextBox.Text = m_Item.Name;

        // Put the base layer at the end of the list
        foreach (ILayer layer in m_OriginalLayers)
            listBox.Items.Insert(0, layer);
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        if (!ValidateEdit())
            return;

        var repo = EnvironmentRepository.Current;

        // Save any changes to the theme name
        var name = nameTextBox.Text.Trim();
        if (name != m_Item.Name)
        {
            var setTheme = repo.GetSetter<ITheme, ISetTheme>(m_Item);
            setTheme.Name = name;
            repo.SaveChanges(m_Item, setTheme);
        }
        
        // Save any changes to the associated layers

        // If any layers have been removed from the list, ensure they no longer refer to the edited theme
        foreach (ILayer layer in m_OriginalLayers.Where(x => GetListLayer(x) is null))
        {
            var setLayer = repo.GetSetter<ILayer, ISetLayer>(layer);
            setLayer.Theme = null;
            repo.SaveChanges(layer, setLayer);
        }

        // Ensure all displayed layers refer to this theme, and have the same sequence as the display
        ILayer[] layers = GetListedLayers();
        int themeSequence = 0;

        foreach (ILayer layer in layers)
        {
            var setLayer = repo.GetSetter<ILayer, ISetLayer>(layer);
            setLayer.Theme = m_Item;
            themeSequence++;
            setLayer.ThemeSequence = themeSequence;
            repo.SaveChanges(layer, setLayer);
        }
        
        DialogResult = DialogResult.OK;
        Close();
    }

    bool ValidateEdit()
    {
        string name = nameTextBox.Text.Trim();
        if (name.Length==0)
        {
            MessageBox.Show("A name must be supplied for the theme");
            nameTextBox.Focus();
            return false;
        }
        
        return true;
    }

    ILayer? GetListLayer(ILayer layer)
    {
        foreach (object o in listBox.Items)
        {
            ILayer listLayer = (ILayer)o;
            if (listLayer.Id == layer.Id)
                return listLayer;
        }

        return null;
    }

    /// <summary>
    /// Returns listed layers, starting with the base layer
    /// </summary>
    /// <returns></returns>
    ILayer[] GetListedLayers()
    {
        ILayer[] result = new ILayer[listBox.Items.Count];
        int index = result.Length-1;

        foreach (object o in listBox.Items)
        {
            result[index] = (ILayer)o;
            index--;
        }

        Debug.Assert(index==-1);
        return result;
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void addButton_Click(object sender, EventArgs e)
    {
        var dial = new AddLayerToThemeForm();
        if (dial.ShowDialog() == DialogResult.OK)
        {
            ILayer? layer = dial.SelectedLayer;
            Debug.Assert(layer is not null);
            int selIndex = listBox.SelectedIndex; // -1 if nothing selected
            int insIndex = selIndex<0 ? 0 : selIndex;
            listBox.Items.Insert(insIndex, layer);
            listBox.SelectedItem = layer;
        }
        dial.Dispose();
    }

    private void removeButton_Click(object sender, EventArgs e)
    {
        int index = listBox.SelectedIndex;
        if (index < 0)
        {
            MessageBox.Show("You must first select the layer you want to remove");
            return;
        }

        listBox.Items.RemoveAt(index);
    }

    private void upButton_Click(object sender, EventArgs e)
    {
        MoveLayer(true);
    }

    private void downButton_Click(object sender, EventArgs e)
    {
        MoveLayer(false);
    }

    void MoveLayer(bool isUp)
    {
        int index = listBox.SelectedIndex;
        if (index < 0)
        {
            MessageBox.Show("You must first select the layer you want to shift");
            return;
        }

        object sel = listBox.Items[index];

        if (isUp && index>0)
        {
            listBox.Items.RemoveAt(index);
            listBox.Items.Insert(index-1, sel);
        }
        else if (!isUp && (index+1)<listBox.Items.Count)
        {
            listBox.Items.RemoveAt(index);
            listBox.Items.Insert(index+1, sel);
        }

        listBox.SelectedItem = sel;
    }
}