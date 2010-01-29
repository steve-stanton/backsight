// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
    /// <summary>
    /// A display that contains a <c>ListBox</c> that displays the
    /// names of environment items.
    /// </summary>
    partial class SimpleListControl : UserControl, IDisplayControl
    {
        #region Class data

        /// <summary>
        /// The object that provides data for this display.
        /// </summary>
        readonly ISimpleListData m_DataProvider;

        #endregion

        #region Constructors

        //public SimpleListControl()
        //{
        //    InitializeComponent();
        //}

        internal SimpleListControl(ISimpleListData dataProvider)
        {
            InitializeComponent();
            m_DataProvider = dataProvider;
        }

        #endregion

        #region IDisplayControl Members

        /// <summary>
        /// Prompts the user for information for a new environment item.
        /// </summary>
        public virtual void NewItem()
        {
            UpdateItem(null);
        }

        /// <summary>
        /// Updates the currently selected environment item.
        /// </summary>
        public virtual void UpdateSelectedItem()
        {
            IEnvironmentItem item = (IEnvironmentItem)listBox.SelectedItem;
            if (item == null)
                MessageBox.Show("You must first select an item from the list");
            else
                UpdateItem(item);
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            IEnvironmentItem item = (IEnvironmentItem)listBox.SelectedItem;
            if (item != null)
                UpdateItem(item);
        }

        void UpdateItem(IEnvironmentItem item)
        {
            // Some pages don't support the update function

            try
            {
                using (Form dial = m_DataProvider.GetEntryDialog(item))
                {
                    if (dial.ShowDialog() == DialogResult.OK)
                        RefreshList();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Removes the currently selected environment item.
        /// </summary>
        public virtual void DeleteSelectedItem()
        {
            IEnvironmentItem item = (IEnvironmentItem)listBox.SelectedItem;
            if (item == null)
            {
                MessageBox.Show("You must first select an item from the list");
                return;
            }

            // Deletions should be disallowed if the environment has been "published"

            if (item is IEditControl)
            {
                (item as IEditControl).Delete();
                RefreshList();
            }
            else
                throw new NotSupportedException();
        }

        public virtual void RefreshList()
        {
            IEnvironmentItem[] items = m_DataProvider.GetEnvironmentItems();
            listBox.Items.Clear();
            listBox.Items.AddRange(items);

            // If the first item is blank, remove it (all "real" items should have
            // a defined name, blanks refer to rows that exist only to accommodate
            // foreign key constraints)
            if (items.Length > 0 && listBox.Items[0].ToString().Length == 0)
                listBox.Items.RemoveAt(0);
        }

        #endregion
    }
}
