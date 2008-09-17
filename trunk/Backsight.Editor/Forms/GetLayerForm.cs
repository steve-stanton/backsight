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

using System;
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    public partial class GetLayerForm : Form
    {
        private ILayer m_SelectedLayer;

        public GetLayerForm()
        {
            InitializeComponent();
            m_SelectedLayer = null;
        }

        public GetLayerForm(ILayer layer) : this()
        {
            m_SelectedLayer = layer;
        }

        internal ILayer SelectedLayer
        {
            get { return m_SelectedLayer; }
        }

        private void GetLayerForm_Shown(object sender, EventArgs e)
        {
            // Load the layers
            ILayer[] layers = EnvironmentContainer.Current.Layers;
            listBox.Items.AddRange(layers);

            // Remove first item if it's blank
            if (listBox.Items.Count > 0 && listBox.Items[0].ToString().Length==0)
                listBox.Items.RemoveAt(0);

            if (m_SelectedLayer!=null)
                listBox.SelectedItem = Array.Find<ILayer>(layers,
                    delegate(ILayer layer) { return layer.Id==m_SelectedLayer.Id; });
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_SelectedLayer = (ILayer)listBox.SelectedItem;
            if (m_SelectedLayer==null)
            {
                MessageBox.Show("You must first select the map layer");
                listBox.Focus();
                return;
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            m_SelectedLayer = (ILayer)listBox.SelectedItem;
            if (m_SelectedLayer!=null)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
