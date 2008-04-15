/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
    /// <summary>
    /// Dialog that lets the user define templates for text formatting
    /// </summary>
    public partial class TemplateForm : Form
    {
        readonly IEditTemplate m_Edit;

        internal TemplateForm() : this(null)
        {
        }


        internal TemplateForm(IEditTemplate edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit == null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateTemplate();
            }

            m_Edit.BeginEdit();
        }

        private void TemplateForm_Shown(object sender, EventArgs e)
        {
            // Load tables that have already been associated with Backsight
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            //m_Templates = ec.Templates;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            //m_Edit.TableName = t;
            m_Edit.FinishEdit();
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}