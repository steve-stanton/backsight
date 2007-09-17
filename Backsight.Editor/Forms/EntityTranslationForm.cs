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
    /// <summary>
    /// Dialog used to obtain the entity type that corresponds to an externally defined alias.
    /// </summary>
    public partial class EntityTranslationForm : Form
    {
        #region Class data

        /// <summary>
        /// The feature code that needs to be translated.
        /// </summary>
        readonly string m_Alias;

        /// <summary>
        /// The spatial type(s) the translation relates to.
        /// </summary>
        readonly SpatialType m_Type;

        /// <summary>
        /// The selected entity type. On return, this will normally be not null.
        /// The only exception is a situation where there were no suitable entity
        /// types for the user to select.
        /// </summary>
        IEntity m_Result;

        #endregion

        #region Constructors

        internal EntityTranslationForm(string alias, SpatialType type)
        {
            InitializeComponent();

            m_Alias = alias;
            m_Type = type;
            m_Result = null;
        }

        #endregion

        /// <summary>
        /// The selected entity type (may be null if there are no suitable entity
        /// types to select).
        /// </summary>
        internal IEntity Result
        {
            get { return m_Result; }
        }

        private void EntityTranslationForm_Shown(object sender, EventArgs e)
        {
            unknownTextBox.Text = m_Alias;

            IEntity[] ents = EnvironmentContainer.EntityTypes(m_Type);
            listBox.Items.AddRange(ents);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex<0 && listBox.Items.Count>0)
            {
                MessageBox.Show("You must first select an entity type from the list.");
                return;
            }

            m_Result = (IEntity)listBox.SelectedItem;
            Close();
        }

        private void EntityTranslationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_Result==null && listBox.Items.Count>0)
            {
                MessageBox.Show("You must select an entity type from the list.");
                e.Cancel = true;
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            m_Result = (listBox.SelectedItem as IEntity);
            if (m_Result!=null)
                Close();
        }
    }
}
