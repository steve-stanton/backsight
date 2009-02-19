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
using System.Data;

using Backsight.Environment;
using Backsight.Editor.Database;
using Backsight.Forms;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// A simple dialog for entering the database attributes for something.
    /// </summary>
    public partial class AttributeDataForm : Form
    {
        #region Class data

        readonly ITable m_Table;

        #endregion

        #region Constructors

        public AttributeDataForm(ITable t)
        {
            InitializeComponent();

            if (t==null)
                throw new ArgumentNullException();

            m_Table = t;
        }

        #endregion

        private void AttributeDataForm_Shown(object sender, EventArgs e)
        {
            try
            {
                this.Text = m_Table.TableName;
                DataRow data = DbUtil.CreateNewRow(m_Table);
                SetRow(data);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void SetRow(DataRow data)
        {
            // Hmm, there isn't a PropertyGrid.DataSource property, so do it the
            // hard way (is there a better way?)

            DataTable table = data.Table;
            object[] items = data.ItemArray;
            AdhocPropertyList props = new AdhocPropertyList(items.Length);

            for (int i=0; i<items.Length; i++)
            {
                DataColumn dc = table.Columns[i];
                AdhocProperty item = new AdhocProperty(dc.ColumnName, items[i]);
                props.Add(item);
            }

            propertyGrid.SelectedObject = props;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}