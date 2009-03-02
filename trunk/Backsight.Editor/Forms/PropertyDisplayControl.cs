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
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Editor.Database;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Delegate for handling the <c>ControlClosed</c> event (raised when the
    /// user cicks on the Close button)
    /// </summary>
    public delegate void OnControlClosed();

    /// <summary>
    /// Display of properties associated with a spatial feature
    /// </summary>
    public partial class PropertyDisplayControl : UserControl
    {
        public event OnControlClosed ControlClosed;

        /// <summary>
        /// The name of the tab that was previously on top (only pages that
        /// are instances of <see cref="PropertyPage"/> are considered - the
        /// standard properties page doesn't count).
        /// </summary>
        string m_LastPageName;

        public PropertyDisplayControl()
        {
            InitializeComponent();
            m_LastPageName = String.Empty;
            editButton.Visible = false;
        }

        internal void SetSelectedObject(ISpatialObject so)
        {
            try
            {
                editButton.Visible = false;
                string oldLastPage = m_LastPageName;

                // Grab the attributes we'll be displaying
                List<Row> rows = GetRows(so);

                // Require a page for each attribute, plus 1 for standard properties.
                // If we have an excess number of property pages, get rid of the redundant ones
                while (tabControl.TabCount > (1+rows.Count))
                    tabControl.TabPages.RemoveAt(1);

                // Reuse any remaining PropertyPages, add any additional pages
                int toReuse = (tabControl.TabCount - 1);
                foreach (Row r in rows)
                {
                    if (toReuse > 0)
                    {
                        PropertyPage pp = (tabControl.TabPages[toReuse] as PropertyPage);
                        Debug.Assert(pp!=null);
                        pp.SetRow(r);
                        toReuse--;
                    }
                    else
                    {
                        tabControl.TabPages.Add(new PropertyPage(r));
                    }
                }

                propertyGrid1.SelectedObject = so;

                // If a non-standard tab was previously on top, and we still have
                // a page with the same tab text, ensure it's on top. Failing
                // that, go for the first tab that shows any database attributes
                // (since that info will likely have more relevance to the user).
                if (!SelectPage(oldLastPage))
                {
                    if (tabControl.TabCount > 1)
                        tabControl.SelectedIndex = 1;
                }
            }

            catch
            {
                propertyGrid1.SelectedObject = null;
            }

            propertyGrid1.Refresh();
        }

        /// <summary>
        /// Obtains any miscellaneous attribute data for a specific spatial object
        /// </summary>
        /// <param name="so">The object of interest</param>
        /// <returns>Any attributes for the spatial object (never null, but may be an empty list)</returns>
        List<Row> GetRows(ISpatialObject so)
        {
            List<Row> result = new List<Row>();

            Feature f = (so as Feature);
            if (f == null && so is Polygon)
                f = (so as Polygon).Label;

            if (f != null && f.Id != null)
            {
                FeatureId fid = f.Id;
                IPossibleList<Row> rows = fid.Rows;
                if (rows != null)
                {
                    foreach (Row r in rows)
                        result.Add(r);
                }
            }

            return result;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            ControlClosed();
        }

        bool SelectPage(string pageName)
        {
            if (String.IsNullOrEmpty(pageName))
                return false;

            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Text == pageName)
                {
                    tabControl.SelectedTab = page;
                    return true;
                }
            }

            return false;
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            TabPage page = tabControl.SelectedTab;
            bool isAttributes = (page is PropertyPage);
            if (isAttributes)
                m_LastPageName = page.Text;

            // You can only edit database attributes (not spatial properties)
            editButton.Visible = isAttributes;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            // Do nothing if the topmost tab page doesn't relate to a database row (the
            // editing button should have been invisible)
            PropertyPage pp = (tabControl.SelectedTab as PropertyPage);
            if (pp == null)
                return;

            Row r = pp.DisplayedRow;
            if (AttributeData.Update(r))
                pp.RefreshRow();
        }
    }
}
