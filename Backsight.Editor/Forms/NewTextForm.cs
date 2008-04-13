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

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog that lets the user specify an item of miscellaneous text.
    /// </summary>
    partial class NewTextForm : Form
    {
        #region Class data

        /// <summary>
        /// The label being updated (if it's an update).
        /// </summary>
        readonly TextFeature m_Label;

        /// <summary>
        /// The text specified by the user
        /// </summary>
        string m_Text;

        /// <summary>
        /// The entity type for the text.
        /// </summary>
        IEntity m_Entity;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewTextForm</c>
        /// </summary>
        /// <param name="label">Optional label that's being updated (null if
        /// specifying a new item of text) [default was null]</param>
        internal NewTextForm(TextFeature label)
        {
            InitializeComponent();

            m_Text = String.Empty;
            m_Entity = null;
            m_Label = label;

            // If we're doing an update, grab initial values.
            if (m_Label!=null)
            {
                m_Text = m_Label.TextGeometry.Text;
                m_Entity = m_Label.EntityType;
            }
        }

        #endregion

        /// <summary>
        /// The text specified by the user
        /// </summary>
        internal string EnteredText // was GetText()
        {
            get { return m_Text; }
        }

        /// <summary>
        /// The entity type for the text.
        /// </summary>
        internal IEntity EntityType
        {
            get { return m_Entity; }
        }

        private void NewTextForm_Shown(object sender, EventArgs e)
        {
            // If we already have an initial string, display it.
            textTextBox.Text = m_Text;

            // Load up entity combo box with the default type for misc text.
            entityTypeComboBox.Load(SpatialType.Text);

            // If we are doing an update, scroll to the entity type we
            // already have, and alter the window title.
            if (m_Entity!=null)
            {
                entityTypeComboBox.SelectEntity(m_Entity);
                this.Text = "Update Miscellaneous Text";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // The entity type must be defined.
            m_Entity = entityTypeComboBox.SelectedEntityType;
            if (m_Entity==null)
            {
                MessageBox.Show("The text type hasn't been specified.");
                entityTypeComboBox.Focus();
                return;
            }

            // The text must be defined (obviously)
            m_Text = textTextBox.Text.Trim();
            if (m_Text.Length==0)
            {
                MessageBox.Show("You haven't specified any text");
                textTextBox.Focus();
                return;
            }

            // If the specified entity type is not the default for misc
            // text, make it the default.
            CadastralMapModel map = CadastralMapModel.Current;
            IEntity curDef = map.DefaultTextType;
            if (curDef==null || curDef.Id!=m_Entity.Id)
                map.DefaultTextType = m_Entity;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}