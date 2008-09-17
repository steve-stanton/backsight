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
    /// <summary>
    /// Dialog for the <see cref="AttachPointUI"/>
    /// </summary>
    public partial class AttachPointForm : Form
    {
        #region Constants

        /// <summary>
        /// Registry key indicating whether the command should auto-repeat. The value of
        /// the entry should be 1 or 0 (for true or false).
        /// </summary>
        const string REPEAT_KEY = "AttachPointRepeat";

        /// <summary>
        /// The format of the registry key for default point type (the substitution refers
        /// to the ID of the relevant map layer, while the value is the ID of the corresponding
        /// entity type).
        /// </summary>
        const string DEFAULT_KEY_FORMAT = "AttachPointType.{0}";

        #endregion

        #region Class data

        /// <summary>
        /// The command that displayed this dialog
        /// </summary>
        readonly AttachPointUI m_Cmd;

        /// <summary>
        /// The entity type to assign to new points 
        /// </summary>
        IEntity m_PointType;

        /// <summary>
        /// Should the command be automatically repeated? 
        /// </summary>
        bool m_Repeat;

        #endregion

        internal AttachPointForm(AttachPointUI cmd)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_Repeat = false;
            m_PointType = null;
        }

        internal IEntity PointType
        {
            get { return m_PointType; }
        }

        internal bool ShouldRepeat
        {
            get { return m_Repeat; }
        }

        private void AttachPointForm_Shown(object sender, EventArgs e)
        {
            // Load the entity combo box with a list for point features.
            m_PointType = entityTypeComboBox.Load(SpatialType.Point);

            // The option to make the selected type the default for
            // this command is ALWAYS set by default
            defaultCheckBox.Checked = true;

            // If there is a default entity for this command (on the
            // current editing layer), select that instead & disable
            // the corresponding checkbox
            int entId = ReadDefaultPointEntityTypeId();
            if (entId > 0)
            {
                IEntity ent = EnvironmentContainer.FindEntityById(entId);
                if (ent!=null)
                {
                    entityTypeComboBox.SelectEntity(ent);
                    defaultCheckBox.Enabled = false;
                    m_PointType = ent;
                }
            }

            // Check auto-repeat option (default is to repeat)
            int repeat = GlobalUserSetting.ReadInt(REPEAT_KEY, 1);
            m_Repeat = (repeat!=0);
            repeatCheckBox.Checked = m_Repeat;
        }

        int ReadDefaultPointEntityTypeId()
        {
            string key = GetDefaultPointEntityTypeIdKey();
            return GlobalUserSetting.ReadInt(key, 0);
        }

        void WriteDefaultPointEntityTypeId(IEntity e)
        {
            string key = GetDefaultPointEntityTypeIdKey();
            GlobalUserSetting.WriteInt(key, e.Id);
        }

        string GetDefaultPointEntityTypeIdKey()
        {
            ILayer layer = m_Cmd.ActiveLayer;
            return String.Format(DEFAULT_KEY_FORMAT, layer.Id);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Get the selected point type.
            m_PointType = entityTypeComboBox.SelectedEntityType;

            // Ensure the currently selected repeat option is saved in the registry
            if (defaultCheckBox.Checked)
                WriteDefaultPointEntityTypeId(m_PointType);

            // Ensure the auto-repeat option is saved too
            m_Repeat = repeatCheckBox.Checked;
            GlobalUserSetting.WriteInt(REPEAT_KEY, (m_Repeat ? 1 : 0));

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void entityComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Ensure the option to make it the default is enabled
            defaultCheckBox.Enabled = true;
        }
    }
}