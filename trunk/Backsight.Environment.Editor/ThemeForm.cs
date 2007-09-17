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
using System.Diagnostics;

namespace Backsight.Environment.Editor
{
    public partial class ThemeForm : Form
    {
        private readonly IEditTheme m_Edit;

        /// <summary>
        /// The original set of layers for the theme (base layer first)
        /// </summary>
        private readonly ILayer[] m_Layers;

        internal ThemeForm() : this(null)
        {
        }

        internal ThemeForm(IEditTheme edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit==null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateTheme();
                m_Layers = new ILayer[0];
            }
            else
                m_Layers = m_Edit.Layers;

            m_Edit.BeginEdit();
        }

        private void ThemeForm_Shown(object sender, EventArgs e)
        {
            nameTextBox.Text = m_Edit.Name;

            // Put the base layer at the end of the list
            foreach (ILayer layer in m_Layers)
                listBox.Items.Insert(0, layer);
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
                MessageBox.Show("A name must be supplied for the theme");
                nameTextBox.Focus();
                return false;
            }

            // If any layers have been removed from the list, ensure they no
            // longer refer to the edited theme
            foreach (ILayer layer in m_Layers)
            {
                ILayer listedLayer = GetListLayer(layer);
                if (listedLayer==null)
                    (layer as IEditLayer).Theme = null;
            }

            // Ensure all displayed layers refer to this theme, and have the same
            // sequence as the display
            ILayer[] layers = GetListedLayers();
            int themeSequence = 0;

            foreach (ILayer layer in layers)
            {
                IEditLayer ed = (IEditLayer)layer;
                ed.Theme = m_Edit;
                themeSequence++;
                ed.ThemeSequence = themeSequence;
            }

            m_Edit.Name = name;

            return true;
        }

        ILayer GetListLayer(ILayer layer)
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
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddLayerToThemeForm dial = new AddLayerToThemeForm();
            if (dial.ShowDialog()==DialogResult.OK)
            {
                ILayer layer = dial.SelectedLayer;
                int selIndex = listBox.SelectedIndex; // -1 if nothing selected
                int insIndex = (selIndex<0 ? 0 : selIndex);
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
            Move(true);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            Move(false);
        }

        void Move(bool isUp)
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
}
