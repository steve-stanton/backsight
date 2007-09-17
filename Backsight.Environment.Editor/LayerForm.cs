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
    public partial class LayerForm : Form
    {
        private readonly IEditLayer m_Edit;

        internal LayerForm() : this(null)
        {
        }

        internal LayerForm(IEditLayer edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit==null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateLayer();
            }

            m_Edit.BeginEdit();
        }

        private void LayerForm_Shown(object sender, EventArgs e)
        {
            nameTextBox.Text = m_Edit.Name;

            IEnvironmentContainer ec = EnvironmentContainer.Current;
            IEntity[] all = ec.EntityTypes;
            pointComboBox.Items.AddRange(EnvironmentContainer.Filter(all, SpatialType.Point));
            lineComboBox.Items.AddRange(EnvironmentContainer.Filter(all, SpatialType.Line));
            textComboBox.Items.AddRange(EnvironmentContainer.Filter(all, SpatialType.Text));
            polygonComboBox.Items.AddRange(EnvironmentContainer.Filter(all, SpatialType.Polygon));

            if (m_Edit.DefaultPointType!=null)
                pointComboBox.SelectedItem = m_Edit.DefaultPointType;

            if (m_Edit.DefaultLineType!=null)
                lineComboBox.SelectedItem = m_Edit.DefaultLineType;

            if (m_Edit.DefaultTextType!=null)
                textComboBox.SelectedItem = m_Edit.DefaultTextType;

            if (m_Edit.DefaultPolygonType!=null)
                polygonComboBox.SelectedItem = m_Edit.DefaultPolygonType;

            ITheme theme = m_Edit.Theme;
            if (theme!=null)
            {
                themeLabel.Visible = true;
                themeTextBox.Visible = true;
                themeTextBox.Text = theme.Name;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateEdit())
            {
                m_Edit.FinishEdit();
                this.DialogResult = DialogResult.OK;
                Close();
            }
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

            m_Edit.Name = name;
            m_Edit.DefaultPointType = (IEntity)pointComboBox.SelectedItem;
            m_Edit.DefaultLineType = (IEntity)lineComboBox.SelectedItem;
            m_Edit.DefaultTextType = (IEntity)textComboBox.SelectedItem;
            m_Edit.DefaultPolygonType = (IEntity)polygonComboBox.SelectedItem;

            return true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
