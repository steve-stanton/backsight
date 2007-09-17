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

namespace Backsight.Editor.Forms
{
    public partial class GetCircleForm : Form
    {
        class CircleItem
        {
            readonly Circle m_Circle;

            internal CircleItem(Circle c)
            {
                m_Circle = c;
            }

            internal Circle Circle { get { return m_Circle; } }

            public override string ToString()
            {
                return String.Format("Radius={0} meters", m_Circle.Radius.Meters);
            }
        }

        private readonly List<Circle> m_Circles;
        private Circle m_Select;

        internal GetCircleForm(List<Circle> circles)
        {
            InitializeComponent();
            m_Circles = circles;
            m_Select = null;
        }

        internal Circle Circle { get { return m_Select; } }

        private void GetCircleForm_Shown(object sender, EventArgs e)
        {
            foreach (Circle c in m_Circles)
            {
                listBox.Items.Add(new CircleItem(c));
            }

            if (listBox.Items.Count>0)
            {
        		// Select the first circle in the list.
                listBox.SelectedIndex = 0;

                // That doesn't cause a call to OnSelchangeList, so force it.
		        //OnSelchangeList();
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            okButton_Click(sender, e);        
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
        	// Erase the circle and redraw the curves (current theme).
            if (m_Select!=null)
            {
                CadastralEditController.Current.ActiveDisplay.PaintNow();
                m_Select = null;

                /*
		m_pSelect->Erase();
		CeLayerList curlayer;
		m_pSelect->DrawCurves(curlayer);
                 */
            }

            this.DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
        	// Ensure something is selected.
            if (m_Select==null)
                m_Select = GetSel();

        	if (m_Select==null)
            {
		        MessageBox.Show("You have not selected a circle.");
		        return;
	        }

	        // Erase the circle and redraw the curves (current theme only).
            CadastralEditController.Current.ActiveDisplay.PaintNow();
            /*
	CeLayerList curlayer;
	m_pSelect->Erase();
	m_pSelect->DrawCurves(curlayer);
             */

            this.DialogResult = DialogResult.OK;
        }

        Circle GetSel()
        {
            if (listBox.SelectedItem==null)
            {
		        MessageBox.Show("Nothing is currently selected.");
		        return null;
            }

            CircleItem item = (CircleItem)listBox.SelectedItem;
            return item.Circle;
        }

        private void listBox_SelectedValueChanged(object sender, EventArgs e)
        {
        	// If we previously had a selected circle, erase it.
            ISpatialDisplay display = CadastralEditController.Current.ActiveDisplay;
            if (m_Select!=null)
                display.PaintNow();
            /*
	// If we previously had a selected circle, erase it. Then
	// redraw any attached curved (erasing the highlighting
	// may have left attached curves looking in poor shape).

	if ( m_pSelect ) {
		m_pSelect->Erase();
		CeLayerList curlayer;
		m_pSelect->DrawCurves(curlayer);
	}
             */

	        // Get the new selection.
	        m_Select = GetSel();
	        if (m_Select==null)
                return;

        	// ... and highlight it.
            DottedStyle style = new DottedStyle();
            m_Select.Render(display, style);
        }
    }
}
