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
            if (m_Command!=null)
                m_Command.LButtonDown(p);
            else
                base.MouseDown(sender, p, b);
        }

        public override void MouseMove(ISpatialDisplay sender, IPosition p, MouseButtons b)
        {
            /*
            using (StreamWriter sw = File.AppendText(@"C:\Temp\Debug.txt"))
            {
                object t = Cursor.Current.Tag;
                sw.WriteLine(String.Format("{0}: cursor tag={1}", DateTime.Now, (t==null ? "none" : t.ToString())));
            }
             */

            // The main window of the cadastral editor provides the option to
            // display the current position of the mouse
            m_Main.MouseMove(sender, p, b);

            // Auto-highlight option
            if (m_IsAutoSelect>0)
                Select(sender, p, SpatialType.All);

            if (m_Command!=null)
                m_Command.MouseMove(p);
        }

        /// <summary>
        /// Handles delete key by removing any selected features (so long as a command is not
        /// currently running).
        /// </summary>
        /// <param name="sender">The display where the key event originated</param>
        /// <param name="k">Information about the event</param>
        public override void KeyDown(ISpatialDisplay sender, KeyEventArgs k)
        {
            if (k.KeyCode == Keys.Delete && !IsCommandRunning && Selection.Count>0)
                StartCommand(new DeletionUI(null)); // and finishes!

            if (k.KeyCode == Keys.Escape && m_Command!=null && m_Command.ActiveDisplay==sender)
                m_Command.Escape();
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
            ContextMenuStrip menu = m_Main.CreateContextMenu(this.Selection);
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
            get { return InitializeDrawStyle(base.HighlightStyle); }
        }

        private IDrawStyle InitializeDrawStyle(IDrawStyle style)
        {
            CadastralMapModel cmm = this.CadastralMapModel;
            style.PointHeight = cmm.PointHeight;
            return style;
        }

        public override void Select(ISpatialDisplay display, IPosition p, SpatialType spatialType)
        {
            CadastralMapModel cmm = this.CadastralMapModel;
            ISpatialSelection currentSel = this.Selection;
            ISpatialObject oldItem = currentSel.Item;
            ISpatialObject newItem;

            // Try to find a point feature if points are drawn.
            if ((spatialType & SpatialType.Point)!=0 && display.MapScale <= cmm.ShowPointScale)
            {
                ILength size = new Length(cmm.PointHeight.Meters * 0.5);
                newItem = cmm.QueryClosest(p, size, SpatialType.Point);
                if (newItem!=null)
                {
                    this.Selection = new Selection(newItem, p);
                    return;
                }
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
                {
                    this.Selection = new Selection(newItem, p);
                    return;
                }
            }

            // Try for a text string if text is drawn.
            // The old software handles text by checking that the point is inside
            // the outline, not sure whether the new index provides acceptable alternative.
            if ((spatialType & SpatialType.Text)!=0 && display.MapScale <= cmm.ShowLabelScale)
            {
                newItem = cmm.QueryClosest(p, tol, SpatialType.Text);
                if (newItem!=null)
                {
                    this.Selection = new Selection(newItem, p);
                    return;
                }
            }

            // Try for a polygon. Don't bother if there's a dialog up,
            // since selecting a polygon is distracting at that stage
            // (really, this applies to things like intersect commands).
            // There MIGHT be cases at some later date where we really
            // do want to select pols.

            if ((spatialType & SpatialType.Polygon)!=0 && !IsCommandRunning)
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
                {
                    this.Selection = new Selection(pol, p);
                    return;
                }
            }

            // Allow it if we're doing updates.
            this.Selection = new SpatialSelection();
            /*
	if ( m_pCommand &&
		 m_pCommand->GetCommandId()!=ID_FEATURE_UPDATE ) return 0;

	const CePolygon* pSelPol = this->SelectPol(point);
	return (CeObject*)pSelPol;

             */
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
                this.Selection = new Selection(p, p);
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
            this.Selection = new SpatialSelection();
        }

        public void Select(Feature f)
        {
            this.Selection = new SpatialSelection(f);
        }

        public void SetSelection(ISpatialSelection ss)
        {
            Selection = ss;
        }

        public override ISpatialSelection Selection
        {
            get { return base.Selection; }

            protected set
            {
                Debug.Assert(value!=null);
                base.Selection = value;
                ISpatialObject item = value.Item;
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
            }
        }

        /// <summary>
        /// Are points drawn in the display. If an editing command is currently running,
        /// the display in question is the command display. Otherwise it's the currently
        /// active display.
        /// </summary>
        bool ArePointsDrawn
        {
            get
            {
                ISpatialDisplay display = (m_Command==null ? ActiveDisplay : m_Command.ActiveDisplay);
                if (display==null)
                    return false;

                double displayScale = display.MapScale;
                return (displayScale < CadastralMapModel.ShowPointScale);
            }
        }

        /// <summary>
        /// Is a single spatial object of a specific type currently selected?
        /// </summary>
        /// <param name="t">The type of interest</param>
        /// <returns>True if one object is currently selected, and it has the spatial type of interest</returns>
        internal bool IsItemSelected(SpatialType t)
        {
            ISpatialObject so = Selection.Item;
            return (so!=null && so.SpatialType==t);
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

            if (m_Inverse!=null)
            {
                m_Inverse.Draw();
                repaint = true;
            }

            if (m_Check!=null)
            {
                m_Check.Render(ActiveDisplay, DrawStyle);
                repaint = true;
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
                ActiveDisplay.PaintNow();
        }
    }
}
