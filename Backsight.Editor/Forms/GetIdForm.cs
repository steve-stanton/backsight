// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
    /// <summary>
    /// Dialog for obtaining a feature ID from the user.
    /// </summary>
    /// <seealso cref="GetKeyForm"/>
    partial class GetIdForm : Form
    {
        #region Class data

        /// <summary>
        /// The desired entity type.
        /// </summary>
        readonly IEntity m_Entity;

        /// <summary>
        /// The reserved ID.
        /// </summary>
        readonly IdHandle m_IdHandle;

        #endregion

        #region Constructors

        internal GetIdForm(IEntity ent, IdHandle idh)
        {
            InitializeComponent();

            m_Entity = ent;
            m_IdHandle = idh;
        }

        #endregion

        private void GetIdForm_Shown(object sender, EventArgs e)
        {
            // Load the ID combo and reserve the first available ID (extending
            // the allocation if necessary).
            int nid = IdHelper.LoadIdCombo(pointIdComboBox, m_Entity, m_IdHandle, true);

            // If nothing could be reserved, get out now.
            if (nid == 0)
            {
                string errmsg = String.Format("Cannot obtain any IDs for '{0}'", m_Entity.Name);
                MessageBox.Show(errmsg);
                Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            EditingController ec = EditingController.Current;
            ec.IsAutoNumber = autoNumberCheckBox.Checked;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void pointIdComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            IdHelper.OnChangeSelectedId(pointIdComboBox, m_IdHandle);
        }
    }
}