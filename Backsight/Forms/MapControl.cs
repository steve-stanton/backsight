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
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>
    /// Control for displaying a map.
    /// </summary>
    public partial class MapControl : UserControl, ISpatialDisplay, IDisposable
    {
        #region Class data

        /// <summary>
        /// Buffer for drawing of the map
        /// </summary>
        private BufferedGraphics m_Display = null;

        /// <summary>
        /// Buffer for holding the map display without any transient stuff (e.g. highlighting)
        /// </summary>
        private BufferedGraphics m_SavedDisplay = null;

        /// <summary>
        /// Is the parent form in a resize loop?
        /// </summary>
        private bool m_IsResizing = false;

        /// <summary>
        /// The parent form (container for this control). While this should be accessible via the
        /// <c>ParentForm</c> property, it's noted here to indicate whether resize handlers have
        /// been added to the parent form (a null value means the resize handlers still need to be added).
        /// </summary>
        private Form m_Parent = null;

        /// <summary>
        /// Multiplier for converting ground units into display units
        /// </summary>
        private double m_GroundToDisplay;

        /// <summary>
        /// The scale of the current draw (Double.NaN if no maps have been asssociated with
        /// this control)
        /// </summary>
        private double m_MapScale = Double.NaN;

        /// <summary>
        /// The ground extent covered by the map panel
        /// </summary>
        private Window m_MapPanelExtent = null;

        /// <summary>
        /// The value of <c>m_MapPanelExtent</c> when an overview is drawn
        /// </summary>
        private Window m_OverviewExtent = null;

        /// <summary>
        /// The current display navigation tool (null if nothing has been started)
        /// </summary>
        private ISpatialDisplayTool m_Tool = null;

        /// <summary>
        /// Info about previous draw extents
        /// </summary>
        private List<DrawInfo> m_Extents = new List<DrawInfo>();

        /// <summary>
        /// Index of the draw that's currently on screen ((-1 if there is no draw info).
        /// Refers to an element in <c>m_Extents</c>
        /// </summary>
        private int m_CurrentExtentIndex = -1;

        #endregion

        #region Constructors

        public MapControl()
        {
            InitializeComponent();

            // The MouseWheel didn't show up in the IDE, so do it now
            this.mapPanel.MouseWheel += new MouseEventHandler(mapPanel_MouseWheel);

            // Help says panel KeyPress events aren't meaningful to application code,
            // but if I need to handle it so that the control can react to the ESC key
            // (the MouseMove handler ensures focus is with mapPanel so that mouse
            // wheel events can be picked up, but taking that approach means the control
            // can no longer pick up KeyPress events).
            this.mapPanel.KeyPress += new KeyPressEventHandler(mapPanel_KeyPress);

            // Same deal for intercepting other keystrokes
            this.mapPanel.KeyDown += new KeyEventHandler(mapPanel_KeyDown);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes scrollbars from this control
        /// </summary>
        protected void RemoveScrollBars(Size mapPanelSize)
        {
            this.Controls.Remove(tableLayoutPanel);
            mapPanel.Size = mapPanelSize;
            this.Controls.Add(mapPanel);
        }

        public void ReplaceMapModel(IWindow initialDrawExtent)
        {
            ResetContent();

            if (initialDrawExtent==null || initialDrawExtent.IsEmpty)
                OnOverview();
            else
            {
                // The overview should get defined ok so long as the current map model
                // has a defined extent.
                bool ok = SetOverviewExtent();

                // If the map model doesn't have any extent, use the extent provided
                if (!ok)
                    ok = SetOverviewExtent(initialDrawExtent);

                Debug.Assert(ok);
                SetNewWindow(initialDrawExtent, true);
            }
        }

        /// <summary>
        /// Initializes scrollbars for a new draw. 
        /// </summary>
        internal virtual void SetScrollBars()
        {
            IWindow mapExtent = this.MapModel.Extent;
            bool isAllMap = (mapExtent==null || mapExtent.IsEmpty || mapExtent.IsEnclosedBy(m_MapPanelExtent));
            if (isAllMap)
            {
                // Just disable rather than make invisible, since making them invisible will
                // change the extent of the map panel.
                hScrollBar.Enabled = false;
                vScrollBar.Enabled = false;
            }
            else
            {
                // Roundoff to the nearest meter on the ground (you'll have to zoom in quite
                // a way before this is a problem with a projected coordinate system, though
                // lat-long data could be problematic)

                // Horizontal scrollbar...

                hScrollBar.Minimum = (int)m_OverviewExtent.Min.X;
                hScrollBar.Maximum = (int)m_OverviewExtent.Max.X;
                hScrollBar.LargeChange = (int)m_MapPanelExtent.Width;
                hScrollBar.SmallChange = hScrollBar.LargeChange/10;
                hScrollBar.Enabled = true;

                int val = (int)m_MapPanelExtent.Min.X;

                if (val<hScrollBar.Minimum)
                    val = hScrollBar.Minimum;
                else if (val>hScrollBar.Maximum)
                    val = hScrollBar.Maximum;

                hScrollBar.Value = val;
                
                // Vertical scrollbar...

                vScrollBar.Minimum = (int)m_OverviewExtent.Min.Y;
                vScrollBar.Maximum = (int)m_OverviewExtent.Max.Y;
                vScrollBar.LargeChange = (int)m_MapPanelExtent.Height;
                vScrollBar.SmallChange = hScrollBar.LargeChange/10;
                vScrollBar.Enabled = true;

                // Vertical scrollbar is a bit strange, since Y origin is at the top
                val = (int)(m_OverviewExtent.Min.Y + m_OverviewExtent.Max.Y - m_MapPanelExtent.Max.Y);

                if (val<vScrollBar.Minimum)
                    val = vScrollBar.Minimum;
                else if (val>vScrollBar.Maximum)
                    val = vScrollBar.Maximum;

                vScrollBar.Value = val;
            }
        }

        /// <summary>
        /// Redraws the map from the model.
        /// </summary>
        public void Redraw()
        {
            Draw(false);
        }

        /// <summary>
        /// Draws from the map model and refreshes the display
        /// </summary>
        /// <param name="addToHistory">Should the map panel extent be added to the view history?</param>
        internal void Draw(bool addToHistory)
        {
            Rectangle rect = new Rectangle(0, 0, mapPanel.Width, mapPanel.Height);
            Brush b = new SolidBrush(mapPanel.BackColor);
            //m_Display.Graphics.FillRectangle(Brushes.AntiqueWhite, rect);
            m_Display.Graphics.FillRectangle(b, rect);

            ISpatialController controller = SpatialController.Current;
            ISpatialModel mapModel = controller.MapModel;
            if (mapModel==null)
                return;

            SetScrollBars();
            //Rectangle rect = new Rectangle(0, 0, mapPanel.Width, mapPanel.Height);
            //m_Display.Graphics.FillRectangle(Brushes.White, rect);
            mapModel.Render(this, controller.DrawStyle);

            // Paint the map panel
            using (Graphics g = mapPanel.CreateGraphics())
            {
                m_Display.Render(g);

                // Save the display without any highlighting
                m_Display.Render(m_SavedDisplay.Graphics);
                //CopyMapPanelToSavedDisplay();

                // Any selection needs to be drawn too, but after the above
                ISpatialSelection ss = controller.Selection;
                if (ss.Count > 0)
                {
                    IDrawStyle style = controller.HighlightStyle;
                    foreach(ISpatialObject item in ss.Items)
                        item.Render(this, style);

                    m_Display.Render(g);
                }
            }

            if (addToHistory)
                AddExtent();
        }

        private ISpatialModel MapModel
        {
            get { return SpatialController.Current.MapModel; }
        }

        /// <summary>
        /// Sets the extent covered by an overview display, based on the extent of
        /// the current map model. The extent of the map panel will be defined to match.
        /// </summary>
        /// <returns>True if overview extent defined. False if the map model doesn't
        /// have any extent (e.g. empty map)</returns>
        bool SetOverviewExtent()
        {
            // Initialize with the window of the map(s)
            IEditWindow window = new Window(this.MapModel.Extent);
            if (window.IsEmpty)
                return false;

            return SetOverviewExtent(window);
        }

        /// <summary>
        /// Sets the extent covered by an overview display, based on the extent of
        /// the supplied window. The extent of the map panel will be defined to match.
        /// </summary>
        /// <param name="w">The window to use for initializing the overview extent</param>
        /// <returns>True if overview extent defined. False if the supplied window is null or empty.</returns>
        bool SetOverviewExtent(IWindow w)
        {
            if (w==null || w.IsEmpty)
                return false;

            Window window = new Window(w);

            //	If the map window is only ONE unit (i.e. we have just added
            //	our first point), define the window so that a point will draw
            //	at a size of 1mm in the centre of the control. Otherwise expand
            //	the map window by 20%

            double dE = window.Width;
            double dN = window.Height;

            if (dE < Double.Epsilon && dN < Double.Epsilon)
            {
                // Treat the SW corner as the centre
                double xc = window.Min.X;
                double yc = window.Min.Y;

                // Get the height of point symbols (in meters on the ground), and
                // figure out what draw scale would make a point symbol draw with a
                // height of 1mm (e.g. 2000 if the size is really 2 meters).
                double pth = 2.0;
                double scale = Math.Max(pth/0.001, 1.0);

                // Get the extent of the client area, in meters.
                double width;
                double height;
        		GetMapPanelScreenExtent(out width, out height);

                // Figure out the extent of the overview window at this scale.
                double dx = width*scale*0.5;
		        double dy = height*scale*0.5;
                IPosition sw = new Position(xc-dx, yc-dy);
                IPosition ne = new Position(xc+dx, yc+dy);
                window = new Window(sw,ne);
            }
            else
            {
                window.Expand(0.2);
            }

            // Figure out the scale and fit the window to the client area.
            // This defines the client extent, window, and scale.
            SetMapPanelMetrics(window);

            // Remember the extent of the overview.
            m_OverviewExtent = new Window(m_MapPanelExtent);
            return true;
        }

        /// <summary>
        /// Gets the size of the map panel, in meters (on the screen)
        /// </summary>
        /// <param name="width">The width of the map panel</param>
        /// <param name="height">The height of the map panel</param>
        private void GetMapPanelScreenExtent(out double width, out double height)
        {
            // Get the dimensions of this control, in pixels
            Size size = MapSize;
            double npixx = (double)size.Width;
            double npixy = (double)size.Height;

            // Figure out what that is in meters (I assume a "dot" == "pixel")
            using (Graphics g = mapPanel.CreateGraphics())
            {
                double dpix = (double)g.DpiX;
                double dpiy = (double)g.DpiY;

                const double inchesToMeters = 0.0254;
                width = (npixx/dpix) * inchesToMeters;
                height = (npixy/dpiy) * inchesToMeters;
            }
        }

        private Size MapSize
        {
            get { return mapPanel.Size; }
        }

        /// <summary>
        /// Defines the ground extent of the map panel, it's scale, and the factor used
        /// to convert between ground and display units.
        /// <param name="window">The window that must fit within the extent of the
        /// map panel</param>
        /// </summary>
        void SetMapPanelMetrics(IWindow window)
        {
            if (window.IsEmpty)
                return;

            // Get the physical dimensions of this control (in meters).
            double width, height;
            GetMapPanelScreenExtent(out width, out height);

            // Get the ground coverage in meters.
            double dE = window.Width;
            double dN = window.Height;

            // Figure out scale denominators in X and Y
            double scalex = dE/width;
            double scaley = dN/height;

            // Pick the smaller of the two scales (e.g. 1:500 as opposed to 1:480).
            m_MapScale = Math.Max(scalex, scaley);

            // Define the extent of the client area in ground units
            dE = width * m_MapScale;
            dN = height * m_MapScale;

            // Figure out the factor for converting ground units into display units
            width = (double)MapSize.Width;
            m_GroundToDisplay = width/dE;

            // Figure out the ground extent of the control (the centre of the map coverage,
            // less half the ground coverage of the control).
            IPosition c = window.Center;
            m_MapPanelExtent = new Window(c.X - 0.5*dE, c.Y - 0.5*dN, c.X + 0.5*dE, c.Y + 0.5*dN);
        }

        /// <summary>
        /// Draws the specified spatial extent
        /// </summary>
        /// <param name="win">The window to draw</param>
        public void DrawWindow(IWindow win)
        {
            SetNewWindow(win, true);
        }

        /// <summary>
        /// Initializes for a redraw. Prior to call, the overview extent of the map
        /// must be defined via a call to <c>SetOverviewExtent</c>.
        /// </summary>
        /// <param name="win">The ground window for the new draw.</param>
        /// <param name="addToHistory">Should the newly defined extent be added to the view history</param>
        public void SetNewWindow(IWindow win, bool addToHistory)
        {
            // Adjust the window so that it fits the client area.
            SetMapPanelMetrics(win);
            Draw(addToHistory);
        }

        new public void Dispose()
        {
            //MessageBox.Show("un-registering map control");
            SpatialController.Current.Unregister(this);
            DropBufferedGraphics();
            base.Dispose();
        }

        void DropBufferedGraphics()
        {
            if (m_Display!=null)
            {
                m_Display.Dispose();
                m_Display = null;
            }

            if (m_SavedDisplay!=null)
            {
                m_SavedDisplay.Dispose();
                m_SavedDisplay = null;
            }
        }

        void InitializeBufferedGraphics()
        {
            BufferedGraphicsContext ctx = BufferedGraphicsManager.Current;
            Rectangle rect = new Rectangle(0, 0, mapPanel.Width, mapPanel.Height);

            using (Graphics g = mapPanel.CreateGraphics())
            {
                m_Display = ctx.Allocate(g, rect);
                m_SavedDisplay = ctx.Allocate(g, rect);
                m_Display.Graphics.FillRectangle(Brushes.Gray, rect);
            }
        }

        void ResizeBufferedGraphics()
        {
            DropBufferedGraphics();
            InitializeBufferedGraphics();

            //if (!this.MapModel.IsEmpty && m_MapPanelExtent!=null)
            if (this.IsInitialized)
            {
                double scale = this.MapScale;
                IPosition center = m_MapPanelExtent.Center;
                SetOverviewExtent();
                SetCenterAndScale(center, scale, false);
            }
        }

        /*
        void CopyMapPanelToSavedDisplay()
        {
            CopyMapPanelToBuffer(m_SavedDisplay);
        }
        */

        /*
        void CopyMapPanelToBuffer(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            Rectangle r = mapPanel.Bounds;
            Point upperLeftDst = mapPanel.Location;
            Point upperLeftSrc = mapPanel.PointToScreen(upperLeftDst);
            g.CopyFromScreen(upperLeftSrc, upperLeftDst, mapPanel.Size);
        }
        */

        void SetParentResizeHandlers()
        {
            if (m_Parent!=null)
                return;

            m_Parent = this.ParentForm;
            if (m_Parent!=null)
            {
                m_Parent.ResizeBegin += new EventHandler(ParentForm_ResizeBegin);
                m_Parent.ResizeEnd += new EventHandler(ParentForm_ResizeEnd);
                m_Parent.FormClosed += new FormClosedEventHandler(ParentForm_Closed);
            }
        }

        private void MapControl_Load(object sender, EventArgs e)
        {
            /*
            SpatialController.Current.Register(this);

            InitializeBufferedGraphics();

            // Control doesn't expose the start/end of a resize, but the parent form does.
            // ...hmmm, the parent form isn't always known at this stage. We'll go through
            // the motions here, but we may not pick it up till some later call (such as
            // the one in MapControl_Resize).

            // ...may want to try this in a MapControl_Shown event handler
            SetParentResizeHandlers();
            
            // Ensure we have the focus (so that key presses will be recognized)
            this.Focus();
             */
        }

        private void mapPanel_Paint(object sender, PaintEventArgs e)
        {
            if (!this.DesignMode)
                PaintDisplay(e.Graphics);
        }

        /// <summary>
        /// Attempts to immediately paint the content of the display buffer. The caller
        /// is responsible for handling potential access conflicts (for example, see
        /// the <c>DrawQuery</c> class).
        /// </summary>
        public void PaintNow()
        {
            using (Graphics g = mapPanel.CreateGraphics())
            {
                PaintDisplay(g);
                //MessageBox.Show("PaintNow");
            }
        }

        /// <summary>
        /// Restores the display buffer so that it contains the stuff it had upon
        /// completion of the last draw (i.e. the last "hard" draw from the model).
        /// This removes any transient graphics that may have been drawn afterwards
        /// (such as highlighting). The map display is not actually repainted, because
        /// you may wish to draw further transient items before revealing things.
        /// When you have prepared the display buffer, make a call to <c>PaintNow</c>
        /// to reveal it.
        /// </summary>
        public void RestoreLastDraw()
        {
            m_SavedDisplay.Render(m_Display.Graphics);
            /*
            using (Graphics g = mapPanel.CreateGraphics())
            {
                m_SavedDisplay.Render(g);
            }

            CopyMapPanelToBuffer(m_Display);
             */
        }

        void PaintDisplay(Graphics g)
        {
            //SpatialController.Current.OnPaint(this);
            m_Display.Render(g);
            //MessageBox.Show("display painted");
        }

        private void MapControl_Resize(object sender, EventArgs e)
        {
            //MessageBox.Show("MapControl_Resize");

            // Ignore if the parent form isn't yet known (not sure, but I seemed to have a
            // situation where this Resize event handler was getting called before the Load
            // handler).
            SetParentResizeHandlers();
            if (m_Parent==null)
                return;

            // Just return if the parent form is being resized by dragging a corner (we'll catch
            // that sort of size change in ParentForm_ResizeEnd)
            if (m_IsResizing)
                return;

            // Handle maximize/restore. In this case, ParentForm_ResizeEnd never gets called.
            ResizeBufferedGraphics();
        }

        private void ParentForm_ResizeBegin(object sender, EventArgs e)
        {
            m_IsResizing = true;
        }

        private void ParentForm_ResizeEnd(object sender, EventArgs e)
        {
            m_IsResizing = false;
            ResizeBufferedGraphics();
        }

        private void ParentForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        #endregion

        private bool IsInitialized
        {
            get { return (!this.MapModel.IsEmpty && m_MapPanelExtent!=null); }
        }

        private void mapPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                EscapeCurrentTool();
        }

        void mapPanel_KeyDown(object sender, KeyEventArgs e)
        {
            SpatialController.Current.KeyDown(this, e);
        }

        private void mapPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.IsInitialized)
            {
                Position p = DisplayToGround(e.Location);

                if (m_Tool==null)
                    SpatialController.Current.MouseDown(this, p, e.Button);
                else
                    m_Tool.MouseDown(p, e.Button);
            }
        }

        public void OnSelectionChanging(ISpatialSelection oldSelection, ISpatialSelection newSelection)
        {
            using (Graphics g = mapPanel.CreateGraphics())
            {
                if (newSelection==null)
                {
                    m_SavedDisplay.Render(g);
                    //CopyMapPanelToBuffer(m_Display); // is this needed?
                }
                else
                {
                    if (oldSelection!=null)
                    {
                        m_SavedDisplay.Render(g);
                        m_SavedDisplay.Render(m_Display.Graphics);
                        //CopyMapPanelToBuffer(m_Display);
                    }

                    // Highlight the new selection
                    IDrawStyle style = SpatialController.Current.HighlightStyle;
                    newSelection.Render(this, style);
                    //foreach(ISpatialObject item in newSelection.Items)
                    //    item.Render(this, style);

                    m_Display.Render(g);
                }
            }
        }

        private void mapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsInitialized)
            {
                // Ensure the map panel has the focus so that any mouse wheel
                // messages will be picked up... no, if you do that, overlapping
                // command dialogs don't retain the focus they need.
                //mapPanel.Focus();

                Position p = DisplayToGround(e.Location);

                if (m_Tool==null)
                    SpatialController.Current.MouseMove(this, p, e.Button);
                else
                    m_Tool.MouseMove(p, e.Button);
            }
        }

        private void mapPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_Tool!=null && this.IsInitialized)
            {
                Position p = DisplayToGround(e.Location);
                m_Tool.MouseUp(p, e.Button);
            }
        }

        void mapPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.IsInitialized)
            {
                // If we don't have a display tool, just zoom in or out. Otherwise
                // pass it down to the tool (only the MagnifyTool currently does anything
                // about it).

                if (m_Tool==null)
                {
                    double currentScale = this.MapScale;
                    double newScale = (e.Delta < 0 ? 0.95*currentScale : 1.05*currentScale);
                    IPosition mousePosition = DisplayToGround(e.Location);
                    IPosition c = Center;
                    double dx = (mousePosition.X - c.X) / currentScale;
                    double dy = (mousePosition.Y - c.Y) / currentScale;
                    double newCenterX = mousePosition.X - (dx * newScale);
                    double newCenterY = mousePosition.Y - (dy * newScale);
                    c = new Position(newCenterX, newCenterY);
                    SetCenterAndScale(c, newScale, false);
                }
                else
                {
                    m_Tool.MouseWheel(e.Delta, Control.ModifierKeys);
                }
            }
        }

        #region Implement ISpatialDisplay

        public double MapScale
        {
            get { return m_MapScale; }
            set { SetCenterAndScale(Center, value, true); }
        }

        // Not browsable, since the setter otherwise leads to periodic error messages in design mode
        // ...but still seem to get occasional error message
        [Browsable(false)]
        public IPosition Center
        {
            get { return (Extent==null ? null : Extent.Center); }
            set
            {
                if (!DesignMode)
                    SetCenterAndScale(value, MapScale, true);
            }
        }

        public IWindow Extent
        {
            get { return m_MapPanelExtent; }
        }

        public IWindow MaxExtent
        {
            get { return m_OverviewExtent; }
        }

        public Graphics Graphics
        {
            get { return m_Display.Graphics; }
            /*
            get
            {
                if (SpatialController.Current.ActiveEdit==null)
                    return m_Display.Graphics;
                else
                    return mapPanel.CreateGraphics();
            }
             */
        }

        private void GroundToDisplay(double x, double y, ref PointF p)
        {
            p.X = EastingToDisplay(x);
            p.Y = NorthingToDisplay(y);
        }

        #endregion

        public float EastingToDisplay(double x)
        {
            return (float)((x - m_MapPanelExtent.Min.X) * m_GroundToDisplay);
        }

        public float NorthingToDisplay(double y)
        {
            return (float)((m_MapPanelExtent.Max.Y - y) * m_GroundToDisplay);
        }

        /// <summary>
        /// Converts a length on the ground into display units.
        /// </summary>
        /// <param name="groundLength">The ground dimension, in meters.</param>
        /// <returns>The corresponding distance in display units.</returns>
        public float LengthToDisplay(double groundLength)
        {
            return (float)(groundLength * m_GroundToDisplay);
        }
        /// <summary>
        /// Converts a pixel count into a ground dimension
        /// </summary>
        /// <param name="displayLength">The display dimension, in pixels.</param>
        /// <returns>The corresponding distance on the map projection, in meters</returns>
        public double DisplayToLength(float displayLength)
        {
            return (double)displayLength / m_GroundToDisplay;
        }

        private Position DisplayToGround(System.Drawing.Point p)
        {
            double x = (((double)p.X) / m_GroundToDisplay) + m_MapPanelExtent.Min.X;
            double y = m_MapPanelExtent.Max.Y - (((double)p.Y) / m_GroundToDisplay);
            return new Position(x, y);
        }

        public void DrawOverview()
        {
            OnOverview();
        }

        /// <summary>
        /// Handles request to draw an overview. 
        /// </summary>
        /// <returns>True (always)</returns>
        bool OnOverview()
        {
            if (SetOverviewExtent())
                SetNewWindow(m_OverviewExtent, true);
            else
                Draw(false);

            return true;
        }

        /// <summary>
        /// Checks whether the map overview action is enabled
        /// </summary>
        /// <returns>True if the control extent is defined, and it's not the same
        /// as the overview extent</returns>
        bool IsOverviewEnabled()
        {
            if (m_MapPanelExtent==null || m_OverviewExtent==null)
                return false;

            return !m_MapPanelExtent.Equals(m_OverviewExtent);
        }

        /// <summary>
        /// Handles request to zoom in. The current draw window 
        ///	will be reduced by 20% all the way round.
        /// </summary>
        /// <returns>True (always)</returns>
        bool ZoomIn()
        {
            return ZoomIn(0.2);
        }

        internal bool ZoomIn(double factor)
        {
            Window extent = new Window(m_MapPanelExtent);
            extent.Expand(-factor);
            SetNewWindow(extent, true);
            return true;
        }

        /// <summary>
        /// Handles request to zoom out. The current draw window
        ///	will be enlarged by 20% all the way round.
        /// </summary>
        /// <returns>True (always)</returns>
        bool ZoomOut()
        {
            return ZoomOut(0.2);
        }

        internal bool ZoomOut(double factor)
        {
            // Expand the current draw window by 20% all the way round
            Window extent = new Window(m_MapPanelExtent);
            extent.Expand(factor);

            // Never allow expansion beyond the overview scale (restrict
            // to the area of common overlap)
            extent = new Window(extent, m_OverviewExtent);
            SetNewWindow(extent, true);
            return true;
        }

        bool ZoomRectangle()
        {
            //	If we are currently auto-highlighting, temporarily disable
            //	for the duration of the zoom, and ensure that any currently
            //	highlighted features are drawn normally.
	        //if ( m_AutoHighlight>0 ) m_AutoHighlight = -m_AutoHighlight;
	        //m_Sel.RemoveSel();


            m_Tool = new ZoomRectangleTool(this);
            return m_Tool.Start();
        }

        internal bool DrawScale()
        {
            ScaleForm dial = new ScaleForm(this.MapScale);
            if (dial.ShowDialog() == DialogResult.OK)
                SetCenterAndScale(m_MapPanelExtent.Center, dial.MapScale, true);

            dial.Dispose();
            return true;
        }

        bool NewCenter()
        {
            m_Tool = new NewCenterTool(this);
            return m_Tool.Start();
        }

        bool Magnify()
        {
            mapPanel.Focus();
            m_Tool = new MagnifyTool(this);
            return m_Tool.Start();
        }

        /// <summary>
        /// Redraws at a new centre point
        /// </summary>
        /// <param name="p">The position for the new centre</param>
        public void SetCenter(IPosition p)
        {
            SetCenterAndScale(p.X, p.Y, this.MapScale, true);
        }

        internal bool MoveMap(double gdx, double gdy)
        {
            // dx>0 => shifts left, dy>0 => shifts up
            int dx = -(int)(gdx * m_GroundToDisplay);
            int dy =  (int)(gdy * m_GroundToDisplay);
            if (dx==0 && dy==0)
                return false;

            m_MapPanelExtent.Shift(-gdx, -gdy);

            Point upperLeftDst = new Point(0, 0);
            Point upperLeftSrc = new Point(dx, dy);

            Graphics g = m_Display.Graphics;
            g.CopyFromScreen(mapPanel.PointToScreen(upperLeftSrc), upperLeftDst, MapSize);

            // File the space that's been revealed (since it will probably be filled with
            // junk). We'll draw into it only when the user lifts the left mouse button.

            Rectangle horzRect, vertRect;

            if (dx>0)
                vertRect = new Rectangle(mapPanel.Width-dx, 0, mapPanel.Width, mapPanel.Height); // right side
            else
                vertRect = new Rectangle(0, 0, -dx, mapPanel.Height); // left side

            if (dy>0)
                horzRect = new Rectangle(0, mapPanel.Height-dy, mapPanel.Width, mapPanel.Height); // bottom
            else
                horzRect = new Rectangle(0, 0, mapPanel.Width, -dy); // top

            Brush b = new SolidBrush(mapPanel.BackColor);
            g.FillRectangle(b, horzRect);
            g.FillRectangle(b, vertRect);

            // If you go via Refresh, there tends to be flicker, since the screen
            // gets erased. So render right now.
            //Refresh();
            //m_Display.Render();
            return true;
        }

        bool Pan()
        {
            m_Tool = new PanTool(this);
            return m_Tool.Start();
        }

        /// <summary>
        /// Refreshes the map display from the model.
        /// </summary>
        /// <returns>True (always)</returns>
        bool MapRefresh()
        {
            Redraw();
            return true;
        }

        bool Previous()
        {
            if (m_CurrentExtentIndex>0)
            {
                m_CurrentExtentIndex--;
                DrawExtent();
            }
            return true;
        }

        bool IsPreviousEnabled()
        {
            return (m_CurrentExtentIndex>0);
        }

        bool Next()
        {
            m_CurrentExtentIndex++;
            DrawExtent();
            return true;
        }

        bool IsNextEnabled()
        {
            return ((m_CurrentExtentIndex+1) < m_Extents.Count);
        }

        public bool IsEnabled(DisplayToolId id)
        {
            //if (this.MapModel.IsEmpty)
            if (!this.IsInitialized)
                return false;

            switch (id)
            {
                case DisplayToolId.Overview:
                    return IsOverviewEnabled();

                case DisplayToolId.ZoomIn:
                case DisplayToolId.ZoomOut:
                case DisplayToolId.ZoomRectangle:
                case DisplayToolId.DrawScale:
                case DisplayToolId.Magnify:
                case DisplayToolId.NewCentre:
                case DisplayToolId.Pan:
                case DisplayToolId.MapRefresh:
                    return true;

                case DisplayToolId.Previous:
                    return IsPreviousEnabled();

                case DisplayToolId.Next:
                    return IsNextEnabled();
            }

            return false;
        }

        void EscapeCurrentTool()
        {
            if (m_Tool!=null)
            {
                m_Tool.Escape();
                m_Tool = null;
            }
        }

        public bool Do(DisplayToolId id)
        {
            EscapeCurrentTool();

            switch (id)
            {
                case DisplayToolId.Overview:
                    return OnOverview();

                case DisplayToolId.ZoomIn:
                    return ZoomIn();

                case DisplayToolId.ZoomOut:
                    return ZoomOut();

                case DisplayToolId.ZoomRectangle:
                    return ZoomRectangle();

                case DisplayToolId.DrawScale:
                    return DrawScale();

                case DisplayToolId.NewCentre:
                    return NewCenter();

                case DisplayToolId.Pan:
                    return Pan();

                case DisplayToolId.MapRefresh:
                    return MapRefresh();

                case DisplayToolId.Previous:
                    return Previous();

                case DisplayToolId.Next:
                    return Next();

                case DisplayToolId.Magnify:
                    return Magnify();
            }

            return false;
        }

        public void Escape(ISpatialDisplayTool tool)
        {
            if (Object.ReferenceEquals(tool, m_Tool))
            {
                this.Cursor = Cursors.Default;
                m_Tool = null;
            }
        }

        public void SetCursor(Cursor cursor)
        {
            // Ensure we have focus, since we may need to recognize a key
            // stroke (the ESC key) to subsequently cancel the current display tool.
            this.Focus();

            this.Cursor = cursor;
        }

        public void Finish(ISpatialDisplayTool tool)
        {
            Debug.Assert(Object.ReferenceEquals(tool, m_Tool));
            m_Tool = null;
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Appends a new draw extent
        /// </summary>
        void AddExtent()
        {
            // If we currently have 32 extents, drop the head.
            if (m_Extents.Count==32)
                m_Extents.RemoveAt(0);

            // Remember a new extent, add make it the current one
            DrawInfo info = new DrawInfo(m_MapPanelExtent, m_MapScale);
            m_Extents.Add(info);
            m_CurrentExtentIndex = m_Extents.Count-1;

            // Notify the controller (may want to remember the extent elsewhere)
            SpatialController.Current.OnSetExtent(this);            
        }

        /// <summary>
        /// Redraws the current draw extent.
        /// </summary>
        void DrawExtent()
        {
            if(m_CurrentExtentIndex<0 || m_CurrentExtentIndex>=m_Extents.Count)
                return;

            DrawInfo info = m_Extents[m_CurrentExtentIndex];
            SetCenterAndScale(info.CenterX, info.CenterY, info.MapScale, false);
        }

        /// <summary>
        /// Defines draw extent based on a new scale and center.
        /// </summary>
        /// <param name="center">The new center.</param>
        /// <param name="scale">The new draw scale (as a denominator).</param>
        /// 
        internal void SetCenterAndScale(IPosition center, double scale, bool addToHistory)
        {
            if (center!=null)
                SetCenterAndScale(center.X, center.Y, scale, addToHistory);
        }

        void SetCenterAndScale(double xc, double yc, double scale, bool addToHistory)
        {
            // Get the extent of the client area, in meters.
            double width, height;
            GetMapPanelScreenExtent(out width, out height);

            // Figure out the ground dimension based on the supplied scale.
            double dx = width * scale;
            double dy = height * scale;

            // Define a window based on the supplied centre.
            Window win = new Window(xc-dx*0.5, yc-dy*0.5, xc+dx*0.5, yc+dy*0.5);

            // Use that to setup for the draw.
            SetNewWindow(win, addToHistory);
        }

        void ResetContent()
        {
            KillExtents();
        }

        /// <summary>
        /// Gets rid of all draw extents that may have been stored. 
        /// </summary>
        void KillExtents()
        {
            m_Extents = new List<DrawInfo>();
            m_CurrentExtentIndex = -1;
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            MapScroll(e, (double)(e.OldValue-e.NewValue), 0.0);
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            MapScroll(e, 0.0, (double)(e.NewValue-e.OldValue));
        }

        void MapScroll(ScrollEventArgs e, double dx, double dy)
        {
            if (e.Type==ScrollEventType.EndScroll)
                Draw(false);
            else
            {
                MoveMap(dx, dy);
                PaintNow();
            }
        }

        public ISpatialDisplay MapDisplay { get { return this; } }

        public Rectangle DrawReversibleFrame(IWindow x)
        {
            int topLeftX = (int)EastingToDisplay(x.Min.X);
            int topLeftY = (int)NorthingToDisplay(x.Max.Y);
            int width = (int)LengthToDisplay(x.Width);
            int height = (int)LengthToDisplay(x.Height);
            Rectangle rect = new Rectangle(topLeftX, topLeftY, width, height);
            Rectangle screenRect = mapPanel.RectangleToScreen(rect);
            DrawReversibleFrame(screenRect);
            return screenRect;
        }

        public void DrawReversibleFrame(Rectangle rect)
        {
            ControlPaint.DrawReversibleFrame(rect, mapPanel.BackColor, FrameStyle.Dashed);
        }

        /// <summary>
        /// Handles loss of focus by ensuring that any active tool has been cancelled.
        /// The user might otherwise get stuck in something like the pan tool, since the
        /// ESC key only works if the control has focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapControl_Leave(object sender, EventArgs e)
        {
            EscapeCurrentTool();
        }

        public void ShowContextMenu(IPosition p, ContextMenuStrip menu)
        {
            if (menu!=null)
            {
                int x = (int)EastingToDisplay(p.X);
                int y = (int)NorthingToDisplay(p.Y);
                Point display = new Point(x, y);
                Point screen = mapPanel.PointToScreen(display);

                // Remember the displayed menu (in case something needs to explicitly hide the menu)
                this.ContextMenuStrip = menu;

                menu.Show(screen.X, screen.Y);
            }
        }

        private void MapControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                //if (this.Text=="MagnifyForm")
                //    MessageBox.Show("registering map control");

                SpatialController.Current.Register(this);
                InitializeBufferedGraphics();
                SetParentResizeHandlers();

                // Ensure we have the focus (so that key presses will be recognized)
                this.Focus();
            }
        }

        protected void SetMapBackground(Color col)
        {
            mapPanel.BackColor = col;
        }

        public Control MapPanel
        {
            get { return mapPanel; }
        }
    }
}
