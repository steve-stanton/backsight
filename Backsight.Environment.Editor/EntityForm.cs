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

namespace Backsight.Environment.Editor
{
    public partial class EntityForm : Form
    {
        private readonly IEditEntity m_Edit;

        internal EntityForm() : this(null)
        {
        }

        internal EntityForm(IEditEntity edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit==null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateEntity();
            }

            m_Edit.BeginEdit();
        }

        private void EntityForm_Shown(object sender, EventArgs e)
        {
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            idGroupComboBox.Items.AddRange(ec.IdGroups);
            layerComboBox.Items.AddRange(ec.Layers);

            IIdGroup g = m_Edit.IdGroup;
            if (g!=null)
                idGroupComboBox.SelectedItem = g;

            ILayer layer = m_Edit.Layer;
            if (layer!=null)
                layerComboBox.SelectedItem = layer;

            entityNameTextBox.Text = m_Edit.Name;
            pointCheckbox.Checked = m_Edit.IsPointValid;
            lineCheckbox.Checked = m_Edit.IsLineValid;
            boundaryCheckbox.Checked = m_Edit.IsPolygonBoundaryValid;
            textCheckbox.Checked = m_Edit.IsTextValid;
            labelCheckbox.Checked = m_Edit.IsPolygonValid;

            labelCheckbox.Enabled = textCheckbox.Checked;
            boundaryCheckbox.Enabled = lineCheckbox.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
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

            m_Edit.Name = entityNameTextBox.Text;
            m_Edit.IsPointValid = pointCheckbox.Checked;
            m_Edit.IsLineValid = lineCheckbox.Checked;
            m_Edit.IsPolygonBoundaryValid = boundaryCheckbox.Checked;
            m_Edit.IsTextValid = textCheckbox.Checked;
            m_Edit.IsPolygonValid = labelCheckbox.Checked;
            m_Edit.IdGroup = (IIdGroup)idGroupComboBox.SelectedItem;
            m_Edit.Layer = (ILayer)layerComboBox.SelectedItem;

            m_Edit.FinishEdit();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void textCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            labelCheckbox.Enabled = textCheckbox.Checked;
            if (!labelCheckbox.Enabled)
                labelCheckbox.Checked = false;
        }

        private void lineCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            boundaryCheckbox.Enabled = lineCheckbox.Checked;
            if (!boundaryCheckbox.Enabled)
                boundaryCheckbox.Checked = false;
        }
    }
}
