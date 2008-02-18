/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
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

            IEnvironmentContainer ec = EnvironmentContainer.Current;

            TreeNode root = new TreeNode(ec.Name);
            root.ImageKey = root.SelectedImageKey = "AttributeContainer";
            tree.Nodes.Add(root);

            // Themes and layers

            ITheme[] themes = ec.Themes;
            if (themes.Length > 0)
            {
                for (int i=0; i<themes.Length; i++)
                {
                    // Ignore the blank theme
                    ITheme theme = themes[i];
                    if (theme.Id==0)
                        continue;

                    TreeNode themeNode = new TreeNode(theme.Name);
                    themeNode.ImageKey = themeNode.SelectedImageKey = "Theme";
                    themeNode.Tag = theme;

                    ILayer[] layers = theme.Layers;
                    for (int j=0; j<layers.Length; j++)
                    {
                        ILayer layer = layers[j];
                        TreeNode layerNode = new TreeNode(layer.Name);
                        layerNode.ImageKey = layerNode.SelectedImageKey = "Layer";
                        layerNode.Tag = layer;
                        themeNode.Nodes.Add(layerNode);
                    }

                    root.Nodes.Add(themeNode);
                }
            }

            // Now any layers that aren't associated with a theme
            ILayer[] allLayers = ec.Layers;
            foreach (ILayer layer in allLayers)
            {
                if (layer.Theme == null)
                {
                    TreeNode layerNode = new TreeNode(layer.Name);
                    layerNode.ImageKey = layerNode.SelectedImageKey = "Layer";
                    layerNode.Tag = layer;
                    root.Nodes.Add(layerNode);
                }
            }

            // Entity types
            IEntity[] ents = ec.EntityTypes;
            foreach (IEntity ent in ents)
            {
                if (ent.Id != 0)
                {
                    TreeNode entNode = new TreeNode(ent.Name);
                    entNode.ImageKey = entNode.SelectedImageKey = "Body";
                    entNode.Tag = ent;
                    root.Nodes.Add(entNode);
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // To display the colour of the selected entity type

            TreeNode node = e.Node;
            IEntity ent = (node.Tag as IEntity);
            colorLabel.Visible = (ent != null);

            // TODO ... get the colour to show
        }
    }
}