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
using System.Drawing;
using System.Windows.Forms;

namespace Backsight.Forms
{
    partial class MagnifyForm : Form
    {
        #region Class data

        /// <summary>
        /// The tool that created this form.
        /// </summary>
        private readonly MagnifyTool m_Tool;

        #endregion

        #region Constructors

        internal MagnifyForm(MagnifyTool tool, double initialScale)
        {
            if (tool==null)
                throw new ArgumentNullException();

            InitializeComponent();

            // Don't let focus come to this form => doesn't react on mouseclicks
            //this.SetStyle(ControlStyles.Selectable, false);
            //this.SetStyle(ControlStyles.UserMouse, true);

            m_Tool = tool;
            ShowScale(initialScale);
        }

        #endregion

        private void MagnifyForm_MouseMove(object sender, MouseEventArgs e)
        {
        }

        internal void Draw(IPosition center, double scale)
        {
            this.magnifyMapControl.SetCenterAndScale(center, scale, false);
        }

        private void MagnifyForm_Shown(object sender, EventArgs e)
        {
            this.magnifyMapControl.ReplaceMapModel(null);
        }

        internal double ZoomIn(double factor)
        {
            MapControl mc = this.magnifyMapControl;
            double oldScale = mc.MapScale;
            double newScale = oldScale - (oldScale*factor);
            mc.SetCenterAndScale(mc.Center, newScale, true);

            //this.magnifyMapControl.ZoomIn(factor);
            return ShowMapScale();
        }

        internal double ZoomOut(double factor)
        {
            this.magnifyMapControl.ZoomOut(factor);
            return ShowMapScale();
        }

        /// <summary>
        /// Zooms in or out
        /// </summary>
        /// <param name="delta">Value obtained from <c>MouseEventArgs.Delta</c> property (any -ve value
        /// to zoomin 5%, else zoomout 5%)</param>
        /// <returns>The resultant map scale</returns>
        new internal double MouseWheel(int delta)
        {
            if (delta < 0)
                return ZoomIn(0.05);
            else
                return ZoomOut(0.05);
        }

        double ShowMapScale()
        {            
            double scale = this.magnifyMapControl.MapScale;
            ShowScale(scale);
            return scale;
        }

        void ShowScale(double scale)
        {
            if (scale>1)
                scaleLabel.Text = String.Format("1:{0:0}", scale);
            else
                scaleLabel.Text = String.Format("{0:0}:1", 1.0/scale);
        }

        private void ctxZoomInMenuItem_Click(object sender, EventArgs e)
        {
            IPosition p = m_Tool.RightClickPosition;
            m_Tool.MouseMove(p, MouseButtons.Left);
            m_Tool.ZoomIn(0.1);
        }

        private void ctxZoomOutMenuItem_Click(object sender, EventArgs e)
        {
            IPosition p = m_Tool.RightClickPosition;
            m_Tool.MouseMove(p, MouseButtons.Left);
            m_Tool.ZoomOut(0.1);
        }

        private void ctxDrawHereMenuItem_Click(object sender, EventArgs e)
        {
            IPosition p = this.magnifyMapControl.Center;
            m_Tool.MouseDown(p, MouseButtons.Left);
        }

        private void ctxCancelMenuItem_Click(object sender, EventArgs e)
        {
            m_Tool.Escape();
        }

        private void ctxScaleMenuItem_Click(object sender, EventArgs e)
        {
            MapControl mc = this.magnifyMapControl;
            ScaleForm dial = new ScaleForm(mc.MapScale);
            if (dial.ShowDialog() == DialogResult.OK)
                mc.SetCenterAndScale(m_Tool.RightClickPosition, dial.MapScale, false);

            dial.Dispose();
            //this.magnifyMapControl.DrawScale();
        }
    }
}