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

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Display of properties associated with a spatial feature
    /// </summary>
    public partial class PropertyDisplayControl : UserControl
    {
        /// <summary>
        /// The name of the tab that was previously on top
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

                // Retain just the first tab page
                while (tabControl.TabCount > 1)
                    tabControl.TabPages.RemoveAt(1);

                propertyGrid1.SelectedObject = so;

                // Display any database attributes
                Feature f = (so as Feature);
                if (f==null && so is Polygon)
                    f = (so as Polygon).Label;

                if (f!=null && f.Id!=null)
                {
                    FeatureId fid = f.Id;
                    IPossibleList<Row> rows = fid.Rows;
                    if (rows != null)
                    {
                        foreach (Row r in rows)
                        {
                            TabPage page = new PropertyPage(r);
                            tabControl.TabPages.Add(page);
                        }
                    }
                }

                // If a non-standard tab was previously on top, and we still have
                // a page at that location, bring it to the front
                SelectPage(oldLastPage);
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
            if (page == null)
                m_LastPageName = String.Empty;
            else
                m_LastPageName = page.Text;
        }
    }
}
