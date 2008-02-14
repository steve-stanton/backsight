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
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

using Backsight.Editor.Operations;
using Backsight.Editor.Forms;
using Backsight.Editor.Properties;
using Backsight.Forms;
using Backsight.Environment;
using Backsight.SqlServer;
using Backsight.Geometry;

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

        #endregion

        #region Class data

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
        /// Are ID numbers selected automatically?
        /// </summary>
        private bool m_IsAutoNumber;

        /// <summary>
        /// User interface for the command that is currently active (if any).
        /// </summary>
        private CommandUI m_Command;

        /// <summary>
        /// The edit the is currently being saved.
        /// </summary>
        private Operation m_CurrentEdit;

        /// <summary>
        /// The object responsible for saving & backing up any data file
        /// </summary>
        private readonly AutoSaver m_AutoSaver;

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

        #endregion

        #region Constructors

        internal EditingController(MainForm main)
            : base()
        {
            if (main==null)
                throw new ArgumentNullException();

            m_Main = main;
            m_IsAutoSelect = 0;
            m_CurrentEdit = null;
            m_AutoSaver = new AutoSaver(this);
            m_Inverse = null;
            m_Check = null;
            m_Sel = null;
            m_HasSelectionChanged = false;
        }

        #endregion

        internal void DiscardModel()
        {
            CadastralMapModel cmm = this.CadastralMapModel;
            if (cmm!=null)
            {
                cmm.Discard();
                this.MapModel = CadastralMapModel.Create();
            }
        }

        public override void Close()
        {
            // Shut down any inverse calculator
            if (m_Inverse!=null)
            {
                m_Inverse.Dispose();
                m_Inverse = null;
            }

            m_AutoSaver.OnClose();
            CadastralMapModel cmm = this.CadastralMapModel;
            if (cmm!=null)
                cmm.Close();

            base.Close();
        }

        public CadastralMapModel CadastralMapModel
        {
            get { return (this.MapModel as CadastralMapModel); }
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
            Selection sel = this.Selection;
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

        internal void Create()
        {
            try
            {
                Close();
                CadastralMapModel cmm = CadastralMapModel.Create();
                this.MapModel = cmm;

                // Ensure an editing layer is defined
                SetActiveLayer();

                // Pick up any default entity types for points, lines, text, polygons
                ILayer layer = ActiveLayer;
                cmm.SetDefaultEntity(SpatialType.Point, layer.DefaultPointType);
                cmm.SetDefaultEntity(SpatialType.Line, layer.DefaultLineType);
                cmm.SetDefaultEntity(SpatialType.Polygon, layer.DefaultPolygonType);
                cmm.SetDefaultEntity(SpatialType.Text, layer.DefaultTextType);

                // Add initial session
                cmm.AddSession();

                InitializeIdManager();
                m_AutoSaver.OnNew();
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Attempts to open a map.
        /// </summary>
        /// <param name="connectionString">The file name of the map to open</param>
        /// <returns>True if map opened ok. False if map couldn't be opened (in that case,
        /// a brand new map gets created)</returns>
        public bool Open(string connectionString)
        {
            try
            {
                Close();
                CadastralMapModel cmm = CadastralMapModel.Open(connectionString);
                cmm.AddSession();
                SetMapModel(cmm, cmm.DrawExtent);
                InitializeIdManager();
                m_AutoSaver.OnOpen();
                return true;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("Creating new map");
                Create();
            }

            return false;
        }

        void InitializeIdManager()
        {
            IdManager idMan = IdManager.Current;
            if (idMan==null)
            {
                idMan = new IdManager();
                Debug.Assert(IdManager.Current!=null);
            }

            idMan.MapOpen(CadastralMapModel);
        }

        /// <summary>
        /// Performs an auto-save. This function should be called ONLY by the
        /// <c>AutoSaver</c> class. This may be called before the user has specified
        /// the name to give to a new map.
        /// </summary>
        internal void AutoSave()
        {
            CadastralMapModel map = this.CadastralMapModel;
            if (map==null)
                return;

            // Update the timestamp for the current editing session.
            map.UpdateSession();
            map.Write();
        }

        /// <summary>
        /// Performs initialization on application startup
        /// </summary>
        /// <param name="cs">Connection string for the database holding the Backsight operating environment
        /// (not null or blank)</param>
        internal void OnStartup(string cs)
        {
            // Load info about the operating environment...
            if (String.IsNullOrEmpty(cs))
                throw new ArgumentException("Environment connection string not defined");

            IEnvironmentContainer ec = new EnvironmentDatabase(cs);
            EnvironmentContainer.Current = ec;

            // Initialize layer ranges
            LayerRange.Initialize(ec);

            // Open the last map (if any)...
            string fileName = Settings.Default.LastMap;

            if (!String.IsNullOrEmpty(fileName) && File.Exists(fileName))
                Open(fileName);
            else
                Create();

            // Ensure an editing layer is defined
            SetActiveLayer();
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
            CadastralMapModel cmm = this.CadastralMapModel;
            style.PointHeight = cmm.PointHeight;
            return style;
        }

        private ISpatialObject SelectObject(ISpatialDisplay display, IPosition p, SpatialType spatialType)
        {
            CadastralMapModel cmm = this.CadastralMapModel;
            ISpatialSelection currentSel = this.SpatialSelection;
            ISpatialObject oldItem = currentSel.Item;
            ISpatialObject newItem;

            // Try to find a point feature if points are drawn.
            if ((spatialType & SpatialType.Point)!=0 && display.MapScale <= cmm.ShowPointScale)
            {
                ILength size = new Length(cmm.PointHeight.Meters * 0.5);
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
            if ((spatialType & SpatialType.Text)!=0 && display.MapScale <= cmm.ShowLabelScale)
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

        internal Operation CurrentEdit
        {
            get { return m_CurrentEdit; }
            set { m_CurrentEdit = value; }
        }

        internal ILayer ActiveLayer
        {
            get { return CadastralMapModel.ActiveLayer; }
            set { CadastralMapModel.ActiveLayer = value; }
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

            m_AutoSaver.StartEdit(cmd);
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

            m_AutoSaver.AbortEdit(m_Command);

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

            // Save the map model
            m_AutoSaver.FinishEdit(m_Command);

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

        internal bool IsAutoNumber
        {
            get { return m_IsAutoNumber; }
            set { m_IsAutoNumber = value; }
        }

        /// <summary>
        /// Ensures the map model has a defined value for the active editing layer
        /// </summary>
        /// <returns>True if the map model has an active layer. False if it
        /// hasn't been defined (you shouldn't be able to do any editing until
        /// it gets defined)</returns>
        bool SetActiveLayer()
        {
            ILayer layer = CadastralMapModel.ActiveLayer;
            if (layer!=null)
                return true;

            int layerId = Settings.Default.DefaultLayerId;
            if (layerId != 0)
                layer = EnvironmentContainer.FindLayerById(layerId);

            if (layer==null)
            {
                GetLayerForm dial = new GetLayerForm();
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    layer = dial.SelectedLayer;
                    Settings.Default.DefaultLayerId = layer.Id;
                    Settings.Default.Save();
                }
                dial.Dispose();
            }

            if (layer==null)
                return false;

            LayerFacade oldLayer = (LayerFacade)CadastralMapModel.ActiveLayer;
            CadastralMapModel.ActiveLayer = layer;

            try
            {
                // Un-highlight stuff. Otherwise it will still be showing after the change of layer,
                // even if it's supposed to be invisible.
                RemoveSelection();
                /*
                SetActiveLayer op = new SetActiveLayer(this.MapModel,
                                                        (LayerFacade)oldLayer,
                                                        (LayerFacade)CadastralMapModel.ActiveLayer);
                StartEdit(op);
                m_CurrentEdit =  op;
                */
            }

            finally
            {
                //FinishEdit();
                m_CurrentEdit = null;
            }
            /*

			StartEdit((INT4)&dial);

			// Create an operation and execute it.
			CeSetTheme* pop = new ( os_database::of(pMap)
								  , os_ts<CeSetTheme>::get() ) CeSetTheme();

			// Tell the map an operation is starting.
			pMap->SaveOp(pop);

			// Execute the operation.
			LOGICAL ok = pop->Execute(*pTheme);

			// Tell the map the operation has finished.
			pMap->SaveOp(pop,ok);

			// On success, ensure the list of any extra draw
			// themes gets cleared. If the change actually failed,
			// get rid of the operation.
			if ( ok )
				m_ExtraThemes.RemoveAll();
			else
				delete pop;

			FinishEdit((INT4)&dial);

			// Force a redraw (with erase).
			InvalidateRect(0);
             */
            
            return true;
        }

        void RemoveSelection()
        {
            // see CeSelection
            //m_Sel.RemoveSel();
        }

        /*
        void SetChanged()
        {
            m_AutoSaver.SetChanged();
        }
        */

        void OnStartUpdate()
	    {
            m_AutoSaver.OnStartUpdate();
        }

        void OnFinishUpdate()
	    {
            m_AutoSaver.OnFinishUpdate();
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

            if (drawExtent.IsEmpty)
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
            SetSelection(new SpatialSelection(f));
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
            }

            m_HasSelectionChanged = true;
            return true;
        }

        /// <summary>
        /// Are points drawn in the display. If an editing command is currently running,
        /// the display in question is the command display. Otherwise it's the currently
        /// active display.
        /// </summary>
        bool ArePointsDrawn
        {
            get { return IsVisible(CadastralMapModel.ShowPointScale); }
        }

        /// <summary>
        /// Are labels drawn in the display. If an editing command is currently running,
        /// the display in question is the command display. Otherwise it's the currently
        /// active display.
        /// </summary>
        bool AreLabelsDrawn
        {
            get { return IsVisible(CadastralMapModel.ShowLabelScale); }
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
        /// of a map. This saves the extent as part of the map model.
        /// </summary>
        /// <param name="sender">The display that has changed</param>
        public override void OnSetExtent(ISpatialDisplay sender)
        {
            CadastralMapModel cmm = this.CadastralMapModel;
            cmm.DrawExtent = sender.Extent;
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
    }
}
