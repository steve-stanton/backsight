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
    partial class GetEntityForm : Form
    {
        private readonly ILayer m_Layer;
        private readonly SpatialType m_Type;
        private IEntity m_SelectedEntity;

        internal GetEntityForm(ILayer layer, SpatialType t)
        {
            if (layer==null)
                throw new ArgumentNullException("Map layer must be specified");

            InitializeComponent();

            m_Layer = layer;
            m_Type = t;
            m_SelectedEntity = null;
        }

        internal IEntity SelectedEntity
        {
            get { return m_SelectedEntity; }
        }

        private void GetEntityForm_Shown(object sender, EventArgs e)
        {
            // Display appropriate form title
            if (m_Type == SpatialType.Point)
                this.Text = "Entity type for points";
            else if (m_Type == SpatialType.Line)
                this.Text = "Entity type for lines";
            else if (m_Type == SpatialType.Polygon)
                this.Text = "Entity type for area labels";
            else if (m_Type == SpatialType.Text)
                this.Text = "Entity type for text";
            else
                this.Text = "Specify entity type";

            // Load the entity types for the spatial data type
            IEntity[] ents = EnvironmentContainer.EntityTypes(m_Type, m_Layer);
            listBox.Items.AddRange(ents);

            // Remove first item if it's blank
            if (listBox.Items.Count > 0 && listBox.Items[0].ToString().Length==0)
                listBox.Items.RemoveAt(0);

            // Select the current default for the layer
            IEntity defEnt = GetListDefault();
            listBox.SelectedItem = defEnt;

            // The polygon boundary checkbox should only be visible for lines
            // (and should be checked if the default entity type is topological).
            polBoundaryCheckBox.Visible = (m_Type == SpatialType.Line);
            if (polBoundaryCheckBox.Visible && defEnt!=null)
                polBoundaryCheckBox.Checked = defEnt.IsPolygonBoundaryValid;
        }

        /// <summary>
        /// Returns the list item that corresponds to the entity type that is the
        /// default for the layer of interest.
        /// </summary>
        /// <returns></returns>
        IEntity GetListDefault()
        {
            // Get the object from the layer
            IEntity e = GetLayerDefault();

            // The object the layer knows about should only be a facade. What we
            // want is the corresponding object from the env container (since that's
            // what's displayed in the list).
            foreach (object o in listBox.Items)
            {
                IEntity listEnt = (IEntity)o;
                if (listEnt.Id == e.Id)
                    return listEnt;
            }

            return null;
        }

        /// <summary>
        /// Returns the default entity type for the layer of interest.
        /// </summary>
        /// <returns></returns>
        IEntity GetLayerDefault()
        {
            if (m_Layer==null)
                return null;

            if (m_Type == SpatialType.Point)
                return m_Layer.DefaultPointType;

            if (m_Type == SpatialType.Line)
                return m_Layer.DefaultLineType;

            if (m_Type == SpatialType.Text)
                return m_Layer.DefaultTextType;

            if (m_Type == SpatialType.Polygon)
                return m_Layer.DefaultPolygonType;

            return null;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_SelectedEntity = (IEntity)listBox.SelectedItem;
            if (m_SelectedEntity==null)
            {
                MessageBox.Show("You must first select the entity type you want");
                listBox.Focus();
                return;
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            m_SelectedEntity = (IEntity)listBox.SelectedItem;
            if (m_SelectedEntity!=null)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (polBoundaryCheckBox.Visible)
            {
                IEntity ent = (IEntity)listBox.SelectedItem;
                if (ent==null)
                    return;

                polBoundaryCheckBox.Checked = ent.IsPolygonBoundaryValid;
            }
        }
    }
}
