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
using Backsight.Environment;

namespace Backsight.Editor.Forms;

/// <written by="Steve Stanton" was="CdAttributes"/>
/// <summary>
/// Dialog for showing the structure of the Backsight environment.
/// </summary>
partial class EnvironmentStructureForm : Form
{
    internal EnvironmentStructureForm()
    {
        InitializeComponent();
    }

    private void EnvironmentStructureForm_Shown(object sender, EventArgs e)
    {
        // Hide the color label, since it only applies when an entity type is selected
        colorLabel.Visible = false;

        var repo = EnvironmentRepository.Current;
        TreeNode root = new TreeNode(repo.Name);
        root.ImageKey = root.SelectedImageKey = "AttributeContainer";
        tree.Nodes.Add(root);

        // Themes and layers
        foreach (ITheme theme in repo.Themes.Where(x => x.Id != 0))
        {
            TreeNode themeNode = new TreeNode(theme.Name);
            themeNode.ImageKey = themeNode.SelectedImageKey = "Theme";
            themeNode.Tag = theme;

            foreach (ILayer layer in theme.Layers)
            {
                TreeNode layerNode = new TreeNode(layer.Name);
                layerNode.ImageKey = layerNode.SelectedImageKey = "Layer";
                layerNode.Tag = layer;
                themeNode.Nodes.Add(layerNode);
            }

            root.Nodes.Add(themeNode);
        }

        // Now any layers that aren't associated with a theme
        // TODO: The database structure suggests that every layer must be associated with a theme, so what's this for?
        /*
        foreach (ILayer layer in repo.Layers)
        {
            if (layer.Theme == null)
            {
                TreeNode layerNode = new TreeNode(layer.Name);
                layerNode.ImageKey = layerNode.SelectedImageKey = "Layer";
                layerNode.Tag = layer;
                root.Nodes.Add(layerNode);
            }
        }
        */

        // Entity types
        foreach (IEntity ent in repo.EntityTypes.Where(x => x.Id != 0))
        {
            TreeNode entNode = new TreeNode(ent.Name);
            entNode.ImageKey = entNode.SelectedImageKey = "Body";
            entNode.Tag = ent;
            root.Nodes.Add(entNode);
        }
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void tree_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Display the color of any selected entity type

        TreeNode node = e.Node;
        IEntity ent = (node.Tag as IEntity);
        colorLabel.Visible = (ent != null);

        if (ent !=null)
            colorLabel.BackColor = EntityUtil.GetColor(ent);
    }
}