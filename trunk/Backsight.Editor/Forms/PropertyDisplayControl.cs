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

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Display of properties associated with a spatial feature
    /// </summary>
    public partial class PropertyDisplayControl : UserControl
    {
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
        }

        internal void SetSelectedObject(ISpatialObject so)
        {
            try
            {
                string oldLastPage = m_LastPageName;

                // Create pages for any database attributes
                List<PropertyPage> pages = new List<PropertyPage>();
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
                            pages.Add(new PropertyPage(r));
                    }
                }

                // Retain just the first tab page
                while (tabControl.TabCount > 1)
                    tabControl.TabPages.RemoveAt(1);

                // Add back any pages for the new database rows
                tabControl.TabPages.AddRange(pages.ToArray());

                propertyGrid1.SelectedObject = so;

                // If a non-standard tab was previously on top, and we still have
                // a page with the same tab text, bring it to the front. Failing
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

        private void closeButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("close");
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
            if (page is PropertyPage)
                m_LastPageName = page.Text;
        }
    }
}
