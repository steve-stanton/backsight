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
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using Backsight.Forms;
using Backsight.ShapeViewer.Properties;

namespace Backsight.ShapeViewer
{
    public partial class MainForm : Form
    {
        private readonly UserActionList m_Actions;
        private readonly ViewController m_Controller;

        public MainForm()
        {
            // Define the controller for the application
            m_Controller = new ViewController(this);

            InitializeComponent();

            // All user interface actions should be handled via the UserActionList technique.
            // Those that deal with map navigation get routed to the map control, the rest
            // will get routed to methods in this class.

            m_Actions = new UserActionList();

            // File menu...
            AddAction(fileOpenMenuItem, IsFileOpenEnabled, FileOpen);
            AddAction(fileExitMenuItem, IsFileExitEnabled, FileExit);

            // View menu...
            AddAction(mnuViewAutoSelect, null, AutoSelect);
            AddAction(DisplayToolId.Overview,       new ToolStripItem[] { mnuViewOverview, ctxViewOverview });
            AddAction(DisplayToolId.ZoomIn,         new ToolStripItem[] { mnuViewZoomIn, ctxViewZoomIn });
            AddAction(DisplayToolId.ZoomOut,        new ToolStripItem[] { mnuViewZoomOut, ctxViewZoomOut });
            AddAction(DisplayToolId.ZoomRectangle,  new ToolStripItem[] { mnuViewZoomRectangle, ctxViewZoomRectangle });
            AddAction(DisplayToolId.DrawScale,      new ToolStripItem[] { mnuViewDrawScale, ctxViewDrawScale });
            AddAction(DisplayToolId.Magnify,        new ToolStripItem[] { mnuViewMagnify, ctxViewMagnify });
            AddAction(DisplayToolId.NewCentre,      new ToolStripItem[] { mnuViewNewCenter, ctxViewNewCenter });
            AddAction(DisplayToolId.Pan,            new ToolStripItem[] { mnuViewPan, ctxViewPan });
            AddAction(DisplayToolId.Previous,       new ToolStripItem[] { mnuViewPrevious, ctxViewPrevious });
            AddAction(DisplayToolId.Next,           new ToolStripItem[] { mnuViewNext, ctxViewNext });
            AddAction(mnuViewStatusBar, IsViewStatusBarEnabled, ViewStatusBar);

            // Update UI in idle time
            Application.Idle += OnIdle;
        }

        void AddAction(DisplayToolId toolId, ToolStripItem[] items)
        {
            m_Actions.Add(new MapControlAction(mapControl, toolId, items));
        }

        void AddAction( ToolStripMenuItem menuItem
                      , UserAction.IsActionEnabled isActionEnabled
                      , UserAction.DoAction doAction)
        {
            ToolStripItem[] item = new ToolStripItem[] { menuItem };
            m_Actions.Add(new UserAction(item, isActionEnabled, doAction));
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Don't define the model until the screen gets shown for the first time. Otherwise
            // the map control may end up saving an incorrect screen image.

            string fileName = Settings.Default.LastMap;
            if (File.Exists(fileName))
            {
                ISpatialData data = new ShapeFile(fileName);
                m_Controller.ShapeFileName = fileName;
                m_Controller.SetMapModel(new SimpleMapModel(data), null);
            }

            // All properties are readonly, and by default they will show up in
            // a pale grey that's difficult to see. Change it to black so the
            // info can be seen without squinting (the fact that the fields can't
            // be edited will be apparent when the user tries to do it). Actually,
            // changing it to black seems to be a special case, where the text
            // continues to comes out grey. So change it to nearly black.
            propertyGrid.ViewForeColor = Color.FromArgb(1, 0, 0); //SystemColors.ControlText; // Color.Black;

            // Ensure the map control has focus, so that things like the ESC key will be recognized
            //mapControl.Focus();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Idle -= OnIdle;
        }

        private void OnIdle(object sender, EventArgs e)
        {
            m_Actions.Update();
            double mapScale = mapControl.MapScale;
            mapScaleLabel.Text = (Double.IsNaN(mapScale) ? "Scale undefined" : String.Format("1:{0:0}", mapScale));

            string name = m_Controller.ShapeFileName;
            this.Text = (String.IsNullOrEmpty(name) ? "(Untitled map)" : name);

            mnuViewStatusBar.Checked = statusStrip.Visible;
        }

        #region File menu

        private bool IsFileOpenEnabled()
        {
            return true;
        }

        private void FileOpen(IUserAction action)
        {
            OpenFileDialog dial = new OpenFileDialog();
            String fileName = (dial.ShowDialog()==DialogResult.OK ? dial.FileName : null);
            dial.Dispose();

            if (fileName==null)
                return;

            ISpatialData data = new ShapeFile(fileName);
            m_Controller.ShapeFileName = fileName;
            m_Controller.SetMapModel(new SimpleMapModel(data), null);
            Settings.Default.LastMap = fileName;
            Settings.Default.Save();
        }

        private bool IsFileExitEnabled()
        {
            return true;
        }

        private void FileExit(IUserAction action)
        {
            Close();
        }

        #endregion

        #region View menu

        private void AutoSelect(IUserAction action)
        {
            mnuViewAutoSelect.Checked = !mnuViewAutoSelect.Checked;
        }

        // Most of the View menu is handled by the map display control

        private bool IsViewStatusBarEnabled()
        {
            return true;
        }

        private void ViewStatusBar(IUserAction action)
        {
            statusStrip.Visible = !statusStrip.Visible;
        }

        #endregion

        // Called by ViewController.MouseMove
        internal void OnMouseMove(ISpatialDisplay sender, IPosition p, System.Windows.Forms.MouseButtons b)
        {
            positionLabel.Text = String.Format("{0:0.000}E, {1:0.000}N", p.X, p.Y);

            if (mnuViewAutoSelect.Checked)
                m_Controller.Select(sender, p, SpatialType.Feature);
        }

        internal void SetSelection(object o)
        {
            if (o!=null)
                infoLabel.Visible = false;

            try
            {
                propertyGrid.SelectedObject = o;
            }

            catch
            {
                propertyGrid.SelectedObject = null;
            }
        }

        /// <summary>
        /// Creates a context menu that contains all display tools
        /// </summary>
        /// <param name="s">The current spatial selection (not used)</param>
        /// <returns>A context menu corresponding to all display tools</returns>
        internal ContextMenuStrip CreateContextMenu(ISpatialSelection s)
        {
            return contextMenu;
        }   
    }
}
