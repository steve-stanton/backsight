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
using System.ComponentModel;
using System.Drawing.Design;

using Backsight.Environment;
using Backsight.Editor.Database;
using Backsight.Forms;
using System.Diagnostics;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="19-FEB-2009"/>
    /// <summary>
    /// A simple dialog for entering the database attributes for something.
    /// </summary>
    public partial class AttributeDataForm : Form
    {
        #region Static

        static ITable s_LastTable;

        /// <summary>
        /// The last row that was entered via this dialog
        /// </summary>
        static object[] s_LastItems;

        #endregion

        #region Class data

        /// <summary>
        /// The table the attribute data is for
        /// </summary>
        readonly ITable m_Table;

        /// <summary>
        /// The ID that will be assigned to the new label
        /// </summary>
        readonly string m_Id;

        /// <summary>
        /// The data entered by the user
        /// </summary>
        DataRow m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">The table the attribute data is for</param>
        /// <param name="id">The ID that will be assigned to the new label</param>
        public AttributeDataForm(ITable t, string id)
        {
            InitializeComponent();

            if (t==null)
                throw new ArgumentNullException();

            m_Table = t;
            m_Id = id;
        }

        #endregion

        private void AttributeDataForm_Shown(object sender, EventArgs e)
        {
            try
            {
                this.Text = m_Table.TableName;
                DataRow data = DbUtil.CreateNewRow(m_Table);

                // Initialize items so they match the values of the last row we processed (if any)
                if (s_LastTable != null && s_LastTable.Id == m_Table.Id)
                {
                    Debug.Assert(s_LastItems != null);
                    Debug.Assert(s_LastItems.Length == data.Table.Columns.Count);
                    data.ItemArray = s_LastItems;
                }

                SetRow(data);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void SetRow(DataRow data)
        {
            m_Data = data;

            // Create type converters for any domains
            IColumnDomain[] cds = m_Table.ColumnDomains;
            ColumnDomainConverter[] cvs = new ColumnDomainConverter[cds.Length];

            for (int i=0; i<cds.Length; i++)
                cvs[i] = new ColumnDomainConverter(cds[i]);

            // Hmm, there isn't a PropertyGrid.DataSource property, so do it the
            // hard way (is there a better way?)

            DataTable table = data.Table;
            object[] items = data.ItemArray;
            AdhocPropertyList props = new AdhocPropertyList(items.Length);

            for (int i=0; i<items.Length; i++)
            {
                DataColumn dc = table.Columns[i];
                string columnName = dc.ColumnName;
                AdhocProperty item = new AdhocProperty(columnName, items[i]);

                // Disallow editing of the feature ID
                if (String.Compare(columnName, m_Table.IdColumnName, true) == 0)
                {
                    item.Value = m_Id;
                    item.Description = "The ID field cannot be edited";
                    item.ReadOnly = true;
                }
                else
                {
                    // If the column has a domain, define the corresponding converter
                    ColumnDomainConverter cv = Array.Find<ColumnDomainConverter>(cvs,
                        delegate(ColumnDomainConverter t) { return String.Compare(t.ColumnDomain.ColumnName, columnName, true)==0; });

                    if (cv != null)
                    {
                        item.Converter = cv;
                    }
                    else
                    {
                        // Define a converter that's consistent with the column datatype (drop through
                        // to a string converter)

                        Type t = dc.DataType;

                        if (t == typeof(int))
                            item.Converter = new Int32Converter();
                        else if (t == typeof(short))
                            item.Converter = new Int16Converter();
                        else if (t == typeof(double))
                            item.Converter = new DoubleConverter();
                        else if (t == typeof(float))
                            item.Converter = new SingleConverter();
                        else if (t == typeof(DateTime))
                            item.Converter = new DateTimeConverter();
                        else
                            item.Converter = new StringConverter();
                    }
                }

                props.Add(item);
            }

            propertyGrid.SelectedObject = props;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Data = null;
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Remember the row as-entered (and the table involved) - we'll
            // use it the data to initialize default values the next time this
            // dialog is displayed)
            AdhocPropertyList list = (propertyGrid.SelectedObject as AdhocPropertyList);
            if (list == null)
                throw new InvalidOperationException();

            foreach (AdhocProperty p in list)
                m_Data[p.Name] = p.Value;

            s_LastTable = m_Table;
            s_LastItems = m_Data.ItemArray;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// The data specified by the user (null if the user cancelled from
        /// the dialog)
        /// </summary>
        internal DataRow Data
        {
            get { return m_Data; }
        }
    }
}