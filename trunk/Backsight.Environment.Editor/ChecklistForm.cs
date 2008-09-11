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
using System.ComponentModel;
using System.Windows.Forms;

namespace Backsight.Environment.Editor
{
    internal partial class ChecklistForm<T> : Form where T : IEnvironmentItem
    {
        private readonly T[] m_Items;
        private T[] m_Selection;

        internal ChecklistForm (T[] items, T[] initialSelection)
        {
            InitializeComponent();
            m_Items = items;
            m_Selection = initialSelection;
        }

        internal T[] Selection { get { return m_Selection; } }

        private void ChecklistForm_Load(object sender, EventArgs e)
        {
            // Display all known items
            foreach (T t in m_Items)
            {
                if (t.Id!=0)
                    listBox.Items.Add(t);
            }

            if (m_Items.Length == 1)
                countLabel.Text = "1 item listed";
            else
                countLabel.Text = m_Items.Length+" items listed";

            // Set check marks against those that are initially selected
            foreach(T t in m_Selection)
            {
                int index = listBox.Items.IndexOf(t);
                if (index >= 0)
                    listBox.SetItemChecked(index, true);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Array.Clear(m_Selection, 0, m_Selection.Length);
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Remember the items now selected
            CheckedListBox.CheckedItemCollection sel = listBox.CheckedItems;
            m_Selection = new T[sel.Count];

            for (int i=0; i<m_Selection.Length; i++)
                m_Selection[i] = (T)sel[i];

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
