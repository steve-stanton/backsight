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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;

namespace Backsight.Environment.Editor
{
    public partial class IdGroupForm : Form
    {
        private readonly IEditIdGroup m_Edit;

        /// <summary>
        /// The entity types that are currently associated with the ID group (null
        /// if the user hasn't yet displayed entity types).
        /// </summary>
        private IEntity[] m_EntityTypes;

        internal IdGroupForm() : this(null)
        {
        }

        internal IdGroupForm(IEditIdGroup edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit==null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateIdGroup();
            }

            m_Edit.BeginEdit();
        }

        private void IdGroupForm_Shown(object sender, EventArgs e)
        {
            groupNameTextBox.Text = m_Edit.Name;
            minTextBox.Text = m_Edit.LowestId.ToString();
            maxTextBox.Text = m_Edit.HighestId.ToString();
            packetSizeTextBox.Text = m_Edit.PacketSize.ToString();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        bool ValidateEdit()
        {
            string name = groupNameTextBox.Text.Trim();
            if (name.Length==0)
            {
                MessageBox.Show("A name must be supplied for the ID group");
                groupNameTextBox.Focus();
                return false;
            }

            int minId = GetInt(minTextBox);
            int maxId = GetInt(maxTextBox);

            if (minId>maxId)
            {
                MessageBox.Show("Low end of range is bigger than the max");
                minTextBox.Focus();
                return false;
            }

            int psize = GetInt(packetSizeTextBox);
            if (psize > (maxId-minId+1))
            {
                MessageBox.Show("Packet size exceeds the number of IDs in the group");
                packetSizeTextBox.Focus();
                return false;
            }

            m_Edit.Name = name;
            m_Edit.LowestId = minId;
            m_Edit.HighestId = maxId;
            m_Edit.PacketSize = psize;

            return true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateEdit())
                return;

            // If the list of associated entity types has been displayed, see whether
            // any changes need to be made.

            if (m_EntityTypes!=null)
            {
                IEntity[] currentTypes = m_Edit.EntityTypes;
                IEntity[] newTypes = m_EntityTypes;

                // If any entity types have been de-selected, make sure they don't refer to
                // the ID group
                foreach (IEntity ent in currentTypes)
                {
                    if (Array.Find<IEntity>(newTypes, delegate(IEntity r)
                        { return (ent.Id==r.Id); })==null)
                        (ent as IEditEntity).IdGroup = null;
                }

                foreach (IEntity ent in newTypes)
                {
                    (ent as IEditEntity).IdGroup = m_Edit;
                }
            }

            m_Edit.FinishEdit();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void formatButton_Click(object sender, EventArgs e)
        {
            // Ensure currently displayed values have been pushed into the ID group
            if (!ValidateEdit())
                return;

            IdFormatForm dial = new IdFormatForm(m_Edit);
            if (dial.ShowDialog() == DialogResult.OK)
            {
                m_Edit.HasCheckDigit = dial.HasCheckDigit;
                m_Edit.KeyFormat = dial.KeyFormat;
            }
            dial.Dispose();
        }

        IEntity[] GetEntityTypes()
        {
            if (m_EntityTypes==null)
                m_EntityTypes = m_Edit.EntityTypes;

            return m_EntityTypes;
        }

        private void entitiesButton_Click(object sender, EventArgs e)
        {
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            IEntity[] entities = ec.EntityTypes;
            IEntity[] selection = GetEntityTypes();
            ChecklistForm<IEntity> dial =
                new ChecklistForm<IEntity>(entities, selection);

            if (dial.ShowDialog() == DialogResult.OK)
                m_EntityTypes = dial.Selection;

            dial.Dispose();
        }

        int GetInt(TextBox tb)
        {
            string s = tb.Text.Trim();
            return (s.Length==0 ? 0 : Int32.Parse(s));
        }
    }
}
