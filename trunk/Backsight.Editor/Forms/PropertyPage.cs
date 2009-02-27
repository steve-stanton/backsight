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

using Backsight.Forms;
using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="10-FEB-2009"/>
    /// <summary>
    /// A TabPage that contains a PropertyGrid
    /// </summary>
    partial class PropertyPage : TabPage
    {
        #region Class data

        /// <summary>
        /// The database row that's being displayed
        /// </summary>
        Row m_Row;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPage"/> class.
        /// </summary>
        /// <param name="row">The database row that's being displayed (not null)</param>
        /// <exception cref="ArgumentNullException">If the supplied row is null</exception>
        internal PropertyPage(Row row)
            : base()
        {
            InitializeComponent();
            SetRow(row);
        }

        #endregion

        /// <summary>
        /// The database row that's being displayed
        /// </summary>
        internal Row DisplayedRow
        {
            get { return m_Row; }
        }

        /// <summary>
        /// Refreshes the display of row attributes (e.g. after the user has edited
        /// the data)
        /// </summary>
        internal void RefreshRow()
        {
            SetRow(m_Row);
        }

        /// <summary>
        /// Display the attributes of a specific row on this property page
        /// </summary>
        /// <param name="row">The row of interest (not null)</param>
        /// <exception cref="ArgumentNullException">If the supplied row is null</exception>
        internal void SetRow(Row row)
        {
            if (row == null)
                throw new ArgumentNullException();

            m_Row = row;

            // Set the text on the tab
            this.Text = row.Table.TableName;

            // We'll display any expanded domain values if we can
            IColumnDomain[] cds = row.Table.ColumnDomains;

            // Hmm, there isn't a PropertyGrid.DataSource property, so do it the
            // hard way (is there a better way?)

            DataRow data = row.Data;
            DataTable table = data.Table;
            object[] items = data.ItemArray;
            AdhocPropertyList props = new AdhocPropertyList(items.Length);

            for (int i=0; i<items.Length; i++)
            {
                DataColumn dc = table.Columns[i];
                string columnName = dc.ColumnName;
                AdhocProperty item = new AdhocProperty(columnName, items[i]);
                item.ReadOnly = true;

                // If the column is associated with a domain, lookup the expanded value and
                // record as the item's description

                IColumnDomain cd = Array.Find<IColumnDomain>(cds, delegate(IColumnDomain t)
                    { return String.Compare(t.ColumnName, columnName, true)==0; });

                if (cd != null)
                {
                    string shortValue = items[i].ToString();
                    string longValue = cd.Domain.Lookup(shortValue);
                    item.Description = longValue;
                }

                props.Add(item);
            }

            propertyGrid.SelectedObject = props;
        }
    }
}
