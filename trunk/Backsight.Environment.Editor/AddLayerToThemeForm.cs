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

namespace Backsight.Environment.Editor
{
    public partial class AddLayerToThemeForm : Form
    {
        /// <summary>
        /// The selected layer (null if user cancelled)
        /// </summary>
        private ILayer m_Layer;

        public AddLayerToThemeForm()
        {
            InitializeComponent();
            m_Layer = null;
        }

        internal ILayer SelectedLayer
        {
            get { return m_Layer; }
        }

        private void AddLayerToThemeForm_Shown(object sender, EventArgs e)
        {
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            ILayer[] layers = ec.Layers;
            listBox.Items.AddRange(layers);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ILayer sel = (ILayer)listBox.SelectedItem;
            if (sel==null)
            {
                MessageBox.Show("You must first select the layer you want to use");
                return;
            }

            m_Layer = sel;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            ILayer sel = (ILayer)listBox.SelectedItem;
            if (sel!=null)
            {
                m_Layer = sel;
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
