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

namespace Backsight.Forms
{
    /// <summary>
    /// Obtains scale for drawing a map
    /// </summary>
    public partial class ScaleForm : Form
    {
        private double m_Scale;
        public double MapScale { get { return m_Scale; } }

        public ScaleForm(double scale)
        {
            InitializeComponent();
            m_Scale = scale;
        }

        private void ScaleForm_Load(object sender, EventArgs e)
        {
            scaleTextBox.Text = String.Format("{0:0}", m_Scale);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                double scale = Convert.ToDouble(scaleTextBox.Text.Trim());
                if (scale < Double.Epsilon)
                    throw new Exception("Scale must be greater than zero");

                m_Scale = scale;
                this.DialogResult = DialogResult.OK;
                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}