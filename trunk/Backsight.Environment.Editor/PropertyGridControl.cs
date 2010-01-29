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

using Backsight.Data;

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Display of miscellaneous properties
    /// </summary>
    public partial class PropertyGridControl : UserControl, IDisplayControl
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridControl"/> class.
        /// </summary>
        public PropertyGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region IDisplayControl Members

        /// <summary>
        /// Prompts the user for information for a new property item.
        /// </summary>
        public void NewItem()
        {
            using (PropertyForm dial = new PropertyForm(String.Empty, String.Empty))
            {
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    IEnvironmentFactory f = EnvironmentContainer.Factory;
                    IEditProperty newProp = f.CreateProperty();
                    newProp.BeginEdit();
                    newProp.Name = dial.PropertyName;
                    newProp.Value = dial.PropertyValue;
                    newProp.FinishEdit();

                    RefreshList();
                }
            }
        }

        /// <summary>
        /// Updates the currently selected property item.
        /// </summary>
        public void UpdateSelectedItem()
        {
            DataGridViewRow sel = GetSelectedRow();
            if (sel == null)
            {
                MessageBox.Show("You must first select the property to update");
                return;
            }

            UpdateProperty(sel);
        }

        /// <summary>
        /// Removes the currently selected property item.
        /// </summary>
        public void DeleteSelectedItem()
        {
            DataGridViewRow row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("You must first select the property to delete");
                return;
            }

            IEditProperty p = (IEditProperty)row.Tag;
            p.Delete();
            RefreshList();
        }

        /// <summary>
        /// Refresh the list of property items.
        /// </summary>
        public void RefreshList()
        {
            IProperty[] data = EnvironmentContainer.Current.Properties;

            // If we don't have mandatory properties, add them now with blank values
            if (AddMandatoryProperties(data))
                data = EnvironmentContainer.Current.Properties;

            propertyGrid.RowCount = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                DataGridViewRow row = propertyGrid.Rows[i];
                row.Tag = data[i];
                row.Cells["nameColumn"].Value = data[i].Name;
                row.Cells["valueColumn"].Value = data[i].Value;
            }

            propertyGrid.CurrentCell = null;
        }

        /// <summary>
        /// Ensures all mandatory properties have been added
        /// </summary>
        /// <returns>True if any properties were added.</returns>
        bool AddMandatoryProperties(IProperty[] data)
        {
            bool result = false;
            string[] props = PropertyNaming.MandatoryProperties;

            foreach (string p in props)
            {
                if (!Array.Exists<IProperty>(data, delegate(IProperty t) { return t.Name == p; }))
                {
                    IEnvironmentFactory f = EnvironmentContainer.Factory;
                    IEditProperty newProp = f.CreateProperty();
                    newProp.BeginEdit();
                    newProp.Name = p;
                    newProp.Value = String.Empty;
                    newProp.FinishEdit();

                    result = true;
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Handles double-click on the grid by passing the selected row to <see cref="UpdateProperty"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void propertyGrid_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewRow sel = GetSelectedRow();
            if (sel != null)
                UpdateProperty(sel);
        }

        /// <summary>
        /// Asks the user to specify a revised value for the property item attached
        /// to the supplied row.
        /// </summary>
        /// <param name="row">The row associated with the property item to update.</param>
        void UpdateProperty(DataGridViewRow row)
        {
            IEditProperty p = (IEditProperty)row.Tag;

            using (PropertyForm dial = new PropertyForm(p.Name, p.Value))
            {
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    p.BeginEdit();
                    p.Value = dial.PropertyValue;
                    p.FinishEdit();

                    RefreshList();
                }
            }
        }

        /// <summary>
        /// Gets the currently selected row.
        /// </summary>
        /// <returns>The selected row (null if nothing is selected)</returns>
        DataGridViewRow GetSelectedRow()
        {
            DataGridViewSelectedRowCollection sel = propertyGrid.SelectedRows;
            if (sel.Count == 0)
                return null;
            else
                return sel[0];
        }
    }
}
