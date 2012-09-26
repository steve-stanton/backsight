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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Backsight.Editor.Forms;
using Backsight.Editor.Properties;
using Backsight.Editor.UI;
using Backsight.Environment;
using Backsight.Forms;
using Backsight.Data;

namespace Backsight.Editor
{
    /// <summary>
    /// The controller for the Cadastral Editor application. The controller is a singleton.
    /// </summary>
    class EditingController : SpatialController
    {
        #region Statics

        /// <summary>
        /// The one and only controller (to be defined as soon as the Cadastral Editor
        /// starts up).
        /// </summary>
        new public static EditingController Current
        {
            get { return (SpatialController.Current as EditingController); }
        }

        static readonly DistanceUnit s_Meters = new DistanceUnit(DistanceUnitType.Meters);
        static readonly DistanceUnit s_Feet = new DistanceUnit(DistanceUnitType.Feet);
        static readonly DistanceUnit s_Chains = new DistanceUnit(DistanceUnitType.Chains);
        static readonly DistanceUnit s_AsEntered = new DistanceUnit(DistanceUnitType.AsEntered);

        #endregion

        #region Class data

        /// <summary>
        /// The current user
        /// </summary>
        readonly User m_User;

        /// <summary>
        /// Information about the current project
        /// </summary>
        Project m_Project;

        /// <summary>
        /// Information about the editing layer
        /// </summary>
        // There's now just one editing ayer per project - this should be obtained probably from Project.LayerId
        ILayer m_ActiveLayer;

        /// <summary>
        /// The main screen (largely comprised of a control for displaying the current
        /// map model).
        /// </summary>
        private readonly MainForm m_Main;

        /// <summary>
        /// Is auto-highlight enabled? (0=false, 1=true, -1=true(but currently disabled)
        /// </summary>
        private int m_IsAutoSelect;

        /// <summary>
        /// User interface for the command that is currently active (if any).
        /// </summary>
        private CommandUI m_Command;

        /// <summary>
        /// Modeless dialog used to perform inverse calculations (null if dialog
        /// is not currently displayed).
        /// </summary>
        InverseForm m_Inverse;

        /// <summary>
        /// Modeless dialog used during file checks (null if a check is not
        /// currently underway).
        /// </summary>
        FileCheckUI m_Check;

        /// <summary>
        /// Helper for performing complex selections. This gets created when the user
        /// moves the mouse while holding down the CTRL key. It gets deleted
        /// when the CTRL key is released (at that time, any selected features will be
        /// merged with the current selection).
        /// </summary>
        SelectionTool m_Sel;

        /// <summary>
        /// Has the selection been changed?
        /// </summary>
        bool m_HasSelectionChanged;

        /// <summary>
        /// The database server (null if a database has not been specified).
        /// </summary>
        IDataServer m_DataServer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditingController"/> class.
        /// </summary>
        /// <param name="main">The main dialog for the Cadastral Editor application</param>
        internal EditingController(MainForm main)
            : base()
        {
            if (main==null)
                throw new ArgumentNullException();

            m_User = new User(System.Environment.UserName);
            m_Project = null;
            m_ActiveLayer = null;
            m_Main = main;
            m_IsAutoSelect = 0;
            m_Inverse = null;
            m_Check = null;
            m_Sel = null;
            m_HasSelectionChanged = false;
            m_DataServer = null;

            // If we know of a database, create the access class
            /*
            string cs = LastDatabase.ConnectionString;
            if (!String.IsNullOrWhiteSpace(cs))
                m_DataServer = new DataServer(cs);
             */
        }

        #endregion

        public override void Close()
        {
            // Shut down any inverse calculator
            if (m_Inverse!=null)
            {
                m_Inverse.Dispose();
                m_Inverse = null;
            }

            // Write out the project settings
            if (m_Project != null)
            {
                new ProjectDatabase().CloseProject(m_Project);
                m_Project = null;
            }
        }

        public CadastralMapModel CadastralMapModel
        {
            get { return (this.MapModel as CadastralMapModel); }
        }

        /// <summary>
        /// Information about the current project
        /// </summary>
        internal Project Project
        {
            get { return m_Project; }
        }

        /// <summary>
        /// Handles a mouse double click event by attempting to select a spatial object
        /// at the supplied position. If the resultant selection refers to just one object,
        /// it will be processed with <see cref="RunUpdate"/>.
        /// </summary>
        /// <param name="sender">The display where the mouse event originated</param>
        /// <param name="p">The position where the mouse click occurred</param>
        public override void MouseDoubleClick(ISpatialDisplay sender, IPosition p)
        {
            // Attempt to select something
            OnSelect(sender, p, false);

            // Update the selected item
            ISpatialSelection ss = SpatialSelection;
            RunUpdate(null, ss.Item);
        }

        /// <summary>
        /// Starts some sort of update. If the selected item is a polygon that has associated
        /// database attributes, a dialog will be displayed to let the user change the attributes.
        /// For any other spatial feature, the dialog for updating an editing operation will be
        /// displayed.
        /// </summary>
        /// <param name="so">The item selected for update</param>
        internal void RunUpdate(IUserAction action, ISpatialObject so)
        {
            if (so==null)
                return;

            // There can't be any command currently running.
            if (m_Command != null)
                return;

            // If we're currently auto-highlighting, get rid of it.
            AutoSelect = false;

            // If a polygon has been selected, invoke the attribute update dialog. Otherwise
            // start an update command.
            if (so.SpatialType == SpatialType.Polygon)
            {
                m_Main.UpdatePolygon((Polygon)so);
            }
            else
            {
                ClearSelection();
                UpdateUI upui = new UpdateUI(action);
                m_Command = upui;

                // We will have a DividerObject if the user selected a line
                // that has been intersected against other lines. In that case,
                // run the update using the underlying line.
                if (so is DividerObject)
                    upui.Run((so as DividerObject).Divider.Line);
                else
                    upui.Run((Feature)so);
            }
        }

        public override void MouseDown(ISpatialDisplay sender, IPosition p, MouseButtons b)
        {
            if (b == MouseButtons.Right)
                ShowContextMenu(sender, p);

            // If there's no command, or it doesn't handle left clicks...
            else if (m_Command == null || !m_Command.LButtonDown(p))
            {
                bool isMultiSelect = (Control.ModifierKeys & Keys.Shift) != 0;

                // If we're currently auto-highlighting, and the user is doing
                // a multi-select, turn off auto-highlight and get rid of the
                // properties window (confusing).

                // TODO: May want to keep the properties window, but disabled. In the
                // past, it was ok to close because the dialog rested on top of the
                // map. Now, closing the property window causes a redraw, which is
                // a bit unexpected in the middle of a multiselect.

                if (isMultiSelect)
                {
                    m_IsAutoSelect = 0;
                    m_Main.ClosePropertiesWindow();
                }

                if (m_Sel==null)
                    OnSelect(sender, p, isMultiSelect);
                else
                    m_Sel.CtrlMouseDown(p);
            }
        }

        /// <summary>
        /// Ensures the area selection tool has been created.
        /// </summary>
        /// <returns>The area selection tool (never null)</returns>
        /// <remarks>
        /// The selection tool should get created when the user moves the mouse while
        /// holding down the CTRL key. However, it's possible the user might do something
        /// like press CTRL and then do a mouse down (without moving the mouse).
        /// </remarks>
        SelectionTool GetAreaSelectionTool()
        {
            // If we're currently auto-highlighting, get rid of it altogether (confuses things).
            if (m_IsAutoSelect != 0)
                AutoSelect = false;

            // Ensure any property window has been removed
            m_Main.ClosePropertiesWindow();

            if (m_Sel==null)
            {
                m_Sel = new SelectionTool(this);
                ActiveDisplay.MapPanel.Cursor = SelectionTool.Cursor;
            }

            return m_Sel;
        }

        /// <summary>
        /// Frees any area selection tool.
        /// </summary>
        void FreeAreaSelectionTool()
        {
            if (m_Sel!=null)
            {
                m_Sel = null;
                ActiveDisplay.MapPanel.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Tries to select something at the specified position
        /// </summary>
        /// <param name="p">The position where a left-click has occurred</param>
        /// <param name="isMultiSelect">True if performing a multi-select (SHIFT key is pressed)</param>
        void OnSelect(ISpatialDisplay display, IPosition p, bool isMultiSelect)
        {
            /*
            // If importing from background, there's no way to select
            // anything from the main map.
            if (m_pGetBack)
            {
                m_pGetBack->OnSelect(point);
                return;
            }
             */

            // Try to select something.
            ISpatialObject thing = SelectObject(display, p, SpatialType.All);

            if (thing!=null)
            {
                // Caution: If we're auto-highlighting, and the thing
                // we've just selected is the thing that's already
                // selected, don't do ANYTHING (not even if the user
                // is apparently doing a multi-select).

                // Note that if the user IS doing a multi-select, any
                // auto-highlighting is supposed to go away automatically
                // (see OnLButtonDown && OnMouseMove).

                if (m_IsAutoSelect==1 && Object.ReferenceEquals(thing, SpatialSelection.Item))
                    return;

                if (isMultiSelect)
                {
                    // Add the thing to the selection (or remove it if
                    // it's currently selected).
                    AddOrRemoveFromSelection(thing);
                }
                else
                {
                    SetSelection(new Selection(thing, p));
                }
            }
            else
            {
                // Ensure the selection has been unhighlited & clear out the selection.
                if (!isMultiSelect)
                    ClearSelection(); // was m_Sel.RemoveSel();
            }

            // If we've now got a simple selection, notify any commands
            // that are running so that their stuff will draw on top
            // of the highlighting.
            //OnSelect();

            // If we are doing an inverse dialog, make sure its point
            // coloring remains regardless of what is currently selected.
            if (m_Inverse!=null)
                m_Inverse.Draw();
        }

        public override void Select(ISpatialDisplay display, IPosition p, SpatialType spatialType)
        {
            ISpatialObject so = SelectObject(display, p, spatialType);
            if (so!=null)
                SetSelection(new Selection(so, p));
            else
                SetSelection(null);
        }

        void AddOrRemoveFromSelection(ISpatialObject so)
        {
            Selection sel = new Selection(this.SpatialSelection.Items);
            if (!sel.Remove(so))
                sel.Add(so);

            SetSelection(sel);
        }

        public override void MouseMove(ISpatialDisplay sender, IPosition p, MouseButtons b)
        {
            if (m_Sel != null) // means the CTRL key is pressed
            {
                m_Sel.CtrlMouseMoveTo(p);
            }
            else
            {
                // The main window of the cadastral editor provides the option to
                // display the current position of the mouse
                m_Main.MouseMove(sender, p, b);

                // Auto-highlight option
                if (m_IsAutoSelect > 0)
                    Select(sender, p, SpatialType.All);

                if (m_Command != null)
                    m_Command.MouseMove(p);
            }
        }

        /// <summary>
        /// Handles delete key by removing any selected features (so long as a command is not
        /// currently running).
        /// </summary>
        /// <param name="sender">The display where the key event originated</param>
        /// <param name="k">Information about the event</param>
        public override void KeyDown(ISpatialDisplay sender, KeyEventArgs k)
        {
            if (k.KeyCode == Keys.Delete && !IsCommandRunning && SpatialSelection.Count>0)
                StartCommand(new DeletionUI(null)); // and finishes!

            if (k.KeyCode == Keys.Escape && m_Command!=null && m_Command.ActiveDisplay==sender)
                m_Command.Escape();

            if (k.KeyCode == Keys.ControlKey && m_Sel == null && !IsCommandRunning)
                GetAreaSelectionTool();
        }

        /// <summary>
        /// Handles a key up event
        /// </summary>
        /// <param name="sender">The display where the key event originated</param>
        /// <param name="k">Information about the event</param>
        public override void KeyUp(ISpatialDisplay sender, KeyEventArgs k)
        {
            // Whereas Control.ModifierKeys sees Keys.Control, the KeyUp event passes Keys.ControlKey
            if (k.KeyCode == Keys.ControlKey && m_Sel!=null)
            {
                // Grab the selected items (if any) and merge any currently selected features.
                Selection s = m_Sel.Selection;
                FreeAreaSelectionTool();
                if (s.Count > 0)
                {
                    s.AddRange(this.SpatialSelection.Items);
                    SetSelection(s);
                }

                // Ensure everything is back to normal
                ActiveDisplay.RestoreLastDraw();

                // Force any prior selection to show
                m_HasSelectionChanged = true;
                //ActiveDisplay.PaintNow();
            }
        }

        /// <summary>
        /// Records the active project.
        /// </summary>
        /// <param name="p">The project the user is working with</param>
        internal void SetProject(Project p)
        {
            m_ActiveLayer = EnvironmentContainer.FindLayerById(p.LayerId);
            if (m_ActiveLayer == null)
                throw new ApplicationException("Cannot locate map layer associated with selected project");

            m_Project = p;
        }

        internal bool AutoSelect
        {
            get { return (m_IsAutoSelect==1); }
            set { m_IsAutoSelect = (value==false ? 0 : 1); }
        }

        public override void ShowContextMenu(ISpatialDisplay where, IPosition p)
        {
            ContextMenuStrip menu = null;
            if (m_Command != null)
                menu = m_Command.CreateContextMenu();

            if (menu==null)
                menu = m_Main.CreateContextMenu(this.SpatialSelection);

            if (menu!=null)
                where.ShowContextMenu(p, menu);
        }

        public override IDrawStyle DrawStyle
        {
            get { return InitializeDrawStyle(base.DrawStyle); }
        }

        internal IDrawStyle Style(Color c)
        {
            IDrawStyle result = InitializeDrawStyle(base.DrawStyle);
            result.LineColor = result.FillColor = c;
            return result;
        }

        internal IDrawStyle Style(HatchStyle hs, Color foreColor, Color backColor)
        {
            DrawStyle result = new DrawStyle();
            result.Fill = new Fill(hs, foreColor, backColor);
            InitializeDrawStyle(result);
            return result;
        }

        public override IDrawStyle HighlightStyle
        {
            get
            {
                HighlightStyle style = (HighlightStyle)base.HighlightStyle;
                style.ShowLineEndPoints = (SelectionCount==1 && m_Sel==null);
                return InitializeDrawStyle(style);
            }
        }

        private IDrawStyle InitializeDrawStyle(IDrawStyle style)
        {
            style.PointHeight = new Length(m_Project.Settings.PointHeight);
            return style;
        }

        private ISpatialObject SelectObject(ISpatialDisplay display, IPosition p, SpatialType spatialType)
        {
            ProjectSettings ps = m_Project.Settings;
            CadastralMapModel cmm = this.CadastralMapModel;
            ISpatialSelection currentSel = this.SpatialSelection;
            ISpatialObject oldItem = currentSel.Item;
            ISpatialObject newItem;

            // Try to find a point feature if points are drawn.
            if ((spatialType & SpatialType.Point) != 0 && display.MapScale <= ps.ShowPointScale)
            {
                ILength size = new Length(ps.PointHeight * 0.5);
                newItem = cmm.QueryClosest(p, size, SpatialType.Point);
                if (newItem!=null)
                    return newItem;
            }

            // If we are adding a line, don't bother trying to select
            // lines or polygons or text.
            /*
            if (m_Op==ID_LINE_NEW || m_Op==ID_LINE_CURVE)
                return 0;
             */

            ILength tol = new Length(0.001 * display.MapScale);

            // Try to find a line, using a tolerance of 1mm at the draw scale.
            if ((spatialType & SpatialType.Line)!=0)
            {
                // If we previously selected something, see if the search point
                // lies within tolerance. If so, just return with what we've already got
                // ...just make the query (the issue here has to do with special highlighting
                // for topological sections -- if you point at another section of a line, the
                // highlighting doesn't move).

                // if (oldItem!=null && oldItem.SpatialType==SpatialType.Line)
                // {
                //     ILength dist = oldItem.Distance(p);
                //     if (dist.Meters < tol.Meters)
                //         return;
                // }

                newItem = cmm.QueryClosest(p, tol, SpatialType.Line);
                if (newItem!=null)
                    return newItem;
            }

            // Try for a text string if text is drawn.
            // The old software handles text by checking that the point is inside
            // the outline, not sure whether the new index provides acceptable alternative.
            if ((spatialType & SpatialType.Text)!=0 && display.MapScale <= ps.ShowLabelScale)
            {
                newItem = cmm.QueryClosest(p, tol, SpatialType.Text);
                if (newItem!=null)
                    return newItem;
            }

            // Just return if a command dialog is up,
            // since selecting a polygon is distracting at that stage
            // (really, this applies to things like intersect commands).
            // There MIGHT be cases at some later date where we really
            // do want to select pols...
            // For updates, allow polygon selection

            if (IsCommandRunning && !(m_Command is UpdateUI))
                return null;

            if ((spatialType & SpatialType.Polygon)!=0)
            {
                // If we currently have a selected polygon, see if we're still inside it.
                /*
                if (oldItem is Polygon)
                {
                    Polygon oldPol = (oldItem is Polygon);
                    if (oldPol.IsEnclosing(p))

                }
                */

                IPointGeometry pg = PointGeometry.Create(p);
                ISpatialIndex index = cmm.Index;
                Polygon pol = new FindPointContainerQuery(index, pg).Result;
                if (pol!=null)
                    return pol;
            }

            return null;
        }

        internal ILayer ActiveLayer
        {
            get { return m_ActiveLayer; }
        }

        internal void StartCommand(CommandUI cmd)
        {
            if (cmd==null)
                throw new ArgumentNullException();

            if (m_Command!=null)
                throw new InvalidOperationException();

            // Disable auto-highlight for the duration of the command
            if (m_IsAutoSelect>0)
                m_IsAutoSelect = -m_IsAutoSelect;

            // Reserve an item number for any editing operation that gets created (it
            // may not get used if the user quits, but it's convenient to do it here)
            //Operation.CurrentEditSequence = Session.ReserveNextItem();

            m_Command = cmd;
            m_Command.Run();
        }

        internal void AbortCommand(CommandUI cmd)
        {
            if (!Object.ReferenceEquals(cmd, m_Command))
                throw new InvalidOperationException();

            // Make sure the normal cursor is on screen.
            SetNormalCursor();

            cmd.ActiveDisplay.RestoreLastDraw();
            RedrawSelection();

            // Re-enable auto-highlighting if it was on before.
            if (m_IsAutoSelect<0)
                m_IsAutoSelect = -m_IsAutoSelect;

            cmd.ActiveDisplay.PaintNow();
            m_Command.Dispose();
            m_Command = null;
        }

        internal void FinishCommand(CommandUI cmd)
        {
            if (!Object.ReferenceEquals(cmd, m_Command))
                throw new InvalidOperationException();

            /*
		    if ( pCmd->GetCommandId() == ID_FILE_PRINT_WINDOW )
		    {
			    CuiGetRectangle* pRect = dynamic_cast<CuiGetRectangle*>(pCmd);
			    CeVertex corners[5];
			    if ( pRect->GetCorners(corners) )
				    m_PrintData.SetPrintCorners(corners);
			    else
			    {
				    m_PrintData.SetPrintCorners(0);
				    m_PrintData.SetRotation(0.0);
			    }
		    }
             */
            SetNormalCursor();

            // Refresh everything from the model. This may seem a bit of an effort, considering
            // that many edits don't do much to the display (some don't do anything). However,
            // it's fast and keeps things clean in more complex cases. Do it before saving the
            // map model, since it gives the impression that things are more responsive than
            // they actually are!
            RefreshAllDisplays();

            // Notify any check dialog (re-check all potential problems).
            // And repaint immediately to avoid flicker (icons wouldn't otherwise be repainted
            // until the idle handler gets called)
            if (m_Check!=null)
            {
                m_Check.OnFinishOp();
                ActiveDisplay.PaintNow();
            }

            // Re-enable auto-highlighting if it was on before.
            if (m_IsAutoSelect<0)
                m_IsAutoSelect = -m_IsAutoSelect;

            /*
            if ( pCmd->GetCommandId() == ID_FEATURE_UPDATE )
                GetDocument()->OnFinishUpdate();
            else
                FinishEdit((INT4)m_pCommand);
            // m_pAutoSaver->FinishEdit(edid);
            */

            m_Command.Dispose();
            m_Command = null;
        }

        /// <summary>
        /// Ensures cursor is the "normal" arrow cursor.
        /// </summary>
        void SetNormalCursor()
        {
            ISpatialDisplay display = ActiveDisplay;
            if (display!=null)
                display.MapPanel.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Should feature IDs be assigned automatically? (false if the user must specify).
        /// </summary>
        internal bool IsAutoNumber
        {
            get { return m_Project.Settings.IsAutoNumber; }
            set { m_Project.Settings.IsAutoNumber = value; }
        }

        /// <summary>
        /// Toggles the setting that says whether user-perceived IDs should be
        /// assigned automatically or not.
        /// </summary>
        internal void ToggleAutoNumber()
        {
            IsAutoNumber = !IsAutoNumber;
        }

        /// <summary>
        /// Ensures a point is visible on the active display (expands the draw window
        /// if necessary), and optionally selects it.
        /// </summary>
        /// <param name="p">The point that needs to be shown.</param>
        /// <param name="select">Specify true if the point should be selected.</param>
        internal void EnsureVisible(PointFeature p, bool select)
        {
            // Ensure the draw window shows the point.
            ISpatialDisplay display = ActiveDisplay;
            IWindow drawExtent = display.Extent;

            if (drawExtent==null || drawExtent.IsEmpty)
                display.DrawOverview();
            else if (!drawExtent.IsOverlap(p))
                display.Center = p;

            // Select the point if requested
            if (select)
                SetSelection(new Selection(p, p));
        }

        /// <summary>
        /// Is a file check underway?
        /// </summary>
        internal bool IsChecking
        {
            get { return (m_Check!=null); }
        }

        /// <summary>
        /// Starts a new file check. Prior to call, you should ensure that a check is not already
        /// running (see the <see cref="IsChecking"/> property).
        /// </summary>
        /// <exception cref="InvalidOperationException">If a file check is already underway</exception>
        /// <returns>True if check started ok. False if the <see cref="FileCheckUI.Run"/> method
        /// returned false.</returns>
        internal bool StartCheck()
        {
            if (m_Check != null)
                throw new InvalidOperationException("File check is already running");

            m_Check = new FileCheckUI();
            if (m_Check.Run())
            {
                TextFeature.AreReferencePointsDrawn = true;
                return true;
            }

            m_Check = null;
            return false;
        }

        /// <summary>
        /// Performs any processing when a file check is being terminated. This nulls out the
        /// reference the controller has to the check driver object, and ensures all map displays
        /// have been refreshed (in case the check has left over any transient draw stuff).
        /// </summary>
        internal void OnFinishCheck()
        {
            m_Check = null;
            TextFeature.AreReferencePointsDrawn = false;
            RefreshAllDisplays();
        }

        internal void ClearSelection()
        {
            SetSelection(null);
        }

        public void Select(Feature f)
        {
            SetSelection(new SpatialSelection((ISpatialObject)f));
        }

        /// <summary>
        /// The currently selected objects, expressed as a <see cref="Selection"/> object.
        /// </summary>
        internal Selection Selection
        {
            get
            {
                ISpatialSelection ss = this.SpatialSelection;
                if (ss is Selection)
                    return (ss as Selection);
                else
                    return new Selection(ss.Items);
            }
        }

        public override bool SetSelection(ISpatialSelection newSel)
        {
            ISpatialSelection ss = (newSel==null ? new Selection() : newSel);
            bool isChanged = base.SetSelection(ss);
            if (!isChanged)
                return false;

            ISpatialObject item = ss.Item;
            if (m_Main!=null)
                m_Main.SetSelection(item);

            // If a single item has been selected
            if (item!=null)
            {
                if (item is DividerObject)
                    item = (item as DividerObject).Divider.Line;

                if (item is PointFeature)
                {
                    if (ArePointsDrawn)
                    {
                        PointFeature selPoint = (item as PointFeature);

                        if (m_Inverse!=null)
                            m_Inverse.OnSelectPoint(selPoint);

                        if (m_Command!=null)
                            m_Command.OnSelectPoint(selPoint);
                    }
                }
                else if (item is LineFeature)
                {
                    if (m_Command!=null)
                    {
                        LineFeature selLine = (item as LineFeature);
                        m_Command.OnSelectLine(selLine);
                    }
                }

                // 20100709 -- Not sure about this. If the user wants to point at the
                // same point twice in succession, the fact that the point is still
                // selected means the 2nd pointing won't get passed down (we tested
                // for a change above).

                if (m_Command != null)
                {
                    // 20101005 -- Allow highlighting of polygons, since their selection
                    // should not interfere with command dialogs (and in things like the
                    // update UI, it can be useful to confirm that topology is ok).

                    if (!(item is Polygon))
                        ClearSelection();
                }
            }

            m_HasSelectionChanged = true;
            return true;
        }

        /// <summary>
        /// Are points drawn in the display. If an editing command is currently running,
        /// the display in question is the command display. Otherwise it's the currently
        /// active display.
        /// </summary>
        internal bool ArePointsDrawn
        {
            get { return IsVisible(m_Project.Settings.ShowPointScale); }
        }

        /// <summary>
        /// Are labels drawn in the display. If an editing command is currently running,
        /// the display in question is the command display. Otherwise it's the currently
        /// active display.
        /// </summary>
        internal bool AreLabelsDrawn
        {
            get { return IsVisible(m_Project.Settings.ShowLabelScale); }
        }

        /// <summary>
        /// Are line annotations drawn in the display. If an editing command is currently running,
        /// the display in question is the command display. Otherwise it's the currently
        /// active display.
        /// </summary>
        internal bool AreLineAnnotationsDrawn
        {
            get
            {
                LineAnnotationStyle annoStyle = this.LineAnnotationStyle;
                if (annoStyle.ShowAdjustedLengths || annoStyle.ShowObservedLengths)
                    return IsVisible(annoStyle.ShowScale);
                else
                    return false;
            }
        }

        /// <summary>
        /// Checks whether something is visible on the active display.
        /// </summary>
        /// <param name="thresholdScale">The threshold scale for some sort of item</param>
        /// <returns>True if the scale of the active display is such that the items would be drawn</returns>
        bool IsVisible(double thresholdScale)
        {
            ISpatialDisplay display = (m_Command==null ? ActiveDisplay : m_Command.ActiveDisplay);
            if (display==null)
                return false;

            double displayScale = display.MapScale;
            return (displayScale < thresholdScale);
        }

        /// <summary>
        /// Determines what type of spatial features are drawn on the active display
        /// </summary>
        internal SpatialType VisibleFeatureTypes
        {
            get
            {
                // Lines are always visible
                SpatialType result = SpatialType.Line;

                if (ArePointsDrawn)
                    result |= SpatialType.Point;

                if (AreLabelsDrawn)
                    result |= SpatialType.Text;

                return result;
            }
        }

        /// <summary>
        /// The number of spatial objects that are currently selected
        /// </summary>
        internal int SelectionCount
        {
            get { return SpatialSelection.Count; }
        }

        /// <summary>
        /// Is a single spatial object of a specific type currently selected?
        /// </summary>
        /// <param name="t">The type of interest</param>
        /// <returns>True if one object is currently selected, and it has the spatial type of interest</returns>
        internal bool IsItemSelected(SpatialType t)
        {
            ISpatialObject so = SpatialSelection.Item;
            return (so!=null && so.SpatialType==t);
        }

        /// <summary>
        /// Does the current selection refer to any line?
        /// </summary>
        /// <returns>True if the selection refers to at least one line</returns>
        internal bool IsLineSelected()
        {
            foreach (ISpatialObject so in SpatialSelection.Items)
            {
                if (so.SpatialType == SpatialType.Line)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Is some sort of editing command currently active?
        /// </summary>
        internal bool IsCommandRunning
        {
            get { return (m_Command!=null); }
        }

        /// <summary>
        /// Is an inverse calculator currently active?
        /// </summary>
        internal bool IsInverseCalculatorRunning
        {
            get { return (m_Inverse!=null); }
        }

        /// <summary>
        /// Starts the specified inverse calculation dialog.
        /// </summary>
        /// <param name="invCalcForm">The dialog to start</param>
        internal void StartInverseCalculator(InverseForm invCalcForm)
        {
            m_Inverse = invCalcForm;
            m_Inverse.FormClosing += new FormClosingEventHandler(InverseFormClosing);
            m_Inverse.Show();
        }

        /// <summary>
        /// FormClosing event handler that's called when inverse calculator dialog
        /// is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InverseFormClosing(object sender, FormClosingEventArgs e)
        {
            m_Inverse = null;

            ISpatialDisplay display = EditingController.Current.ActiveDisplay;
            display.RestoreLastDraw();
            display.PaintNow();
        }

        /// <summary>
        /// Perform any processing whenever a display has changed the drawn extent
        /// of a map. This saves the extent as part of the project settings.
        /// </summary>
        /// <param name="sender">The display that has changed</param>
        public override void OnSetExtent(ISpatialDisplay sender)
        {
            m_Project.Settings.LastDraw = new DrawInfo(sender.Extent, sender.MapScale);
        }

        /// <summary>
        /// Application idle handler gives editing commands the opportunity to re-paint
        /// any command-specific stuff.
        /// </summary>
        internal void OnIdle()
        {
            bool repaint = false;
            ISpatialDisplay display = ActiveDisplay;

            if (m_Inverse!=null)
            {
                m_Inverse.Draw();
                repaint = true;
            }

            if (m_Check!=null)
            {
                m_Check.Render(display, DrawStyle);
                repaint = true;
            }

            if (m_Sel!=null)
            {
                m_Sel.Render(display);
                repaint = true;
            }

            if (m_Sel!=null || m_HasSelectionChanged)
            {
                SpatialSelection.Render(display, HighlightStyle);
                repaint = true;
                m_HasSelectionChanged = false;
            }

            if (m_Command!=null && m_Command.PerformsPainting)
            {
                // Restoring the last draw here can cause flickering of the command-specific
                // stuff, so make it the responsibility of the command to call erase stuff.
                // (the main thing to ensure is that the call to RestoreLastDraw is made
                // only when the user has really made some sort of change).
                // m_Command.ActiveDisplay.RestoreLastDraw();

                m_Command.Paint(null);
                repaint = true;
            }

            if (repaint)
                display.PaintNow();
        }

        /// <summary>
        /// The units that should be used on display of observed lengths.
        /// </summary>
        internal DistanceUnit DisplayUnit
        {
            get
            {
                DistanceUnitType du = m_Project.Settings.DisplayUnitType;
                return GetUnits(du);
            }
        }

        internal DistanceUnit EntryUnit
        {
            get
            {
                DistanceUnitType du = m_Project.Settings.EntryUnitType;
                return GetUnits(du);
            }
        }

        internal static DistanceUnit GetUnits(DistanceUnitType unitType)
        {
            switch (unitType)
            {
                case DistanceUnitType.Meters:
                    return s_Meters;
                case DistanceUnitType.Feet:
                    return s_Feet;
                case DistanceUnitType.Chains:
                    return s_Chains;
                case DistanceUnitType.AsEntered:
                    return s_AsEntered;
            }

            throw new ArgumentException("Unexpected unit type");
        }

        /// <summary>
        /// Converts a string that represents a distance unit abbreviation into one
        /// of the <c>DistanceUnit</c> instances known to the map.
        /// </summary>
        /// <param name="abbr">The abbreviation to look for (not case-sensitive)</param>
        /// <returns>The corresponding unit (null if the unit cannot be determined)</returns>
        internal DistanceUnit GetUnit(string abbrev)
        {
            string a = abbrev.ToUpper().Trim();
            if (a.Length == 0)
                return null;

            if (s_Meters.Abbreviation.ToUpper().StartsWith(a))
                return s_Meters;

            if (s_Feet.Abbreviation.ToUpper().StartsWith(a))
                return s_Feet;

            if (s_Chains.Abbreviation.ToUpper().StartsWith(a))
                return s_Chains;

            return null;
        }

        /// <summary>
        /// Checks whether the current project needs to be saved or not.
        /// If it hasn't been saved, the user will be asked. If the user
        /// decided not to save, any edits performed since the last save
        /// will be discarded. This should be called when the Editor
        /// application is being closed.
        /// </summary>
        internal void CheckSave()
        {
            // Always save project settings
            if (m_Project != null)
                m_Project.SaveSettings();

            if (IsSaved)
                return;

            Session s = CadastralMapModel.Current.WorkingSession;
            Debug.Assert(s != null);

            if (MessageBox.Show("Do you want to save changes?", "Changes not saved", MessageBoxButtons.YesNo)
                == DialogResult.Yes)
                s.SaveChanges();
            else
                s.DiscardChanges();
        }

        /// <summary>
        /// Have all edits been saved?
        /// </summary>
        internal bool IsSaved
        {
            get
            {
                CadastralMapModel model = CadastralMapModel.Current;
                if (model == null)
                    return true;

                Session s = model.WorkingSession;
                if (s == null)
                    return true;

                return s.IsSaved;
            }
        }

        /// <summary>
        /// The current user
        /// </summary>
        internal User User
        {
            get { return m_User; }
        }

        /// <summary>
        /// Checks whether it is OK to undo the last edit in the current
        /// editing session.
        /// </summary>
        /// <exception cref="Exception">If there is any reason why it's inappropriate
        /// to undo at the present time</exception>
        internal void CheckUndo()
        {
            // If a check is running, confirm that we can really rollback (you can only undo
            // edits that you made since the check was started)
            if (m_Check != null)
            {
                CadastralMapModel map = CadastralMapModel.Current;
                uint lastop = map.LastOpSequence;
                if (!m_Check.CanRollback(lastop))
                    throw new Exception("Cannot undo prior to beginning of File-Check");
            }
        }

        /// <summary>
        /// The point feature that is currently selected (null if a point is not
        /// selected, or the current selection contains more than one feature).
        /// </summary>
        internal static PointFeature SelectedPoint
        {
            get
            {
                ISpatialObject so = EditingController.Current.SpatialSelection.Item;
                return (so as PointFeature);
            }
        }

        /// <summary>
        /// The line feature that is currently selected (null if a point is not
        /// selected, or the current selection contains more than one feature).
        /// </summary>
        internal static LineFeature SelectedLine
        {
            get
            {
                ISpatialObject so = EditingController.Current.SpatialSelection.Item;
                if (so is DividerObject)
                    return (so as DividerObject).Divider.Line;
                else
                    return (so as LineFeature);
            }
        }

        /// <summary>
        /// The text feature that is currently selected (null if a point is not
        /// selected, or the current selection contains more than one feature).
        /// </summary>
        internal static TextFeature SelectedText
        {
            get
            {
                ISpatialObject so = EditingController.Current.SpatialSelection.Item;
                return (so as TextFeature);
            }
        }

        /// <summary>
        /// Style information for annotating lines.
        /// </summary>
        internal LineAnnotationStyle LineAnnotationStyle
        {
            get { return Project.Settings.LineAnnotation; }
        }

        /// <summary>
        /// Gets the well known text for the coordinate system associated with the
        /// current map model.
        /// </summary>
        /// <returns>The WKT for the coordinate system</returns>
        /// <remarks>
        /// This aims to avoid the huge delay that arises when CSMap is asked to return
        /// the WKT for the first time (takes about 8 seconds on my machine). Given that
        /// it is highly likely that just one coordinate system will be in regular use,
        /// this method caches the last result as part of the application settings.
        /// <para/>
        /// It may be preferable to save the WKT as part of the cedx file.
        /// </remarks>
        internal string GetCoordinateSystemText()
        {
            ISpatialSystem ss = CadastralMapModel.Current.SpatialSystem;
            if (ss.Name != Settings.Default.LastSystemName)
            {
                string wkt = ss.GetWellKnownText();

                Settings.Default.LastSystemName = ss.Name;
                Settings.Default.LastSystemText = wkt;
                Settings.Default.Save();
            }

            return Settings.Default.LastSystemText;
        }

        /// <summary>
        /// The database server (null if a database has not been specified).
        /// </summary>
        internal IDataServer DataServer
        {
            get { return m_DataServer; }
        }

        /// <summary>
        /// Defines (or clears) a database server.
        /// </summary>
        /// <param name="connectionString">The database connection string (specify null if you want
        /// to clear a previously defined database).</param>
        /// <returns>The database server (null if a null connection string was supplied).</returns>
        internal IDataServer SetDataServer(string connectionString)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
                m_DataServer = null;
            else
                m_DataServer = new DataServer(connectionString);

            return m_DataServer;
        }

        /// <summary>
        /// The main screen (largely comprised of a control for displaying the current map model).
        /// </summary>
        internal Form MainForm
        {
            get { return m_Main; }
        }
    }
}
