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
using System.IO;
using System.Windows.Forms;

using Backsight.Editor.UI;
using Backsight.Environment;
using Backsight.Forms;
using Backsight.Editor.FileStore;
using Backsight.Data;


namespace Backsight.Editor.Forms
{
    public partial class MainForm : Form, IControlContainer
    {
        #region Class data

        private readonly UserActionList m_Actions;
        private readonly EditingController m_Controller;

        /// <summary>
        /// Most-recently used file list
        /// </summary>
        readonly MruStripMenuInline m_MruMenu;

        /// <summary>
        /// The name of the project file used to launch the application (null if user didn't
        /// double-click on a cedx file)
        /// </summary>
        readonly string m_InitialProjectFile;

        #endregion

        public MainForm(string[] args)
        {
            InitializeComponent();

            // If user double-clicked on a file, it should appear as an argument. In that
            // case, remember it as the initial job file (it gets dealt with by MainForm_Shown)
            if (args != null && args.Length > 0)
                m_InitialProjectFile = args[0];
            else
                m_InitialProjectFile = null;

            // Define the controller for the application
            m_Controller = new EditingController(this);

            string regkey = @"Software\Backsight\Editor\MRU";
            m_MruMenu = new MruStripMenuInline(mnuFile, mnuFileRecent, new MruStripMenu.ClickedHandler(OnMruFile), regkey, 10);

            // Hide property panel unless it's requested
            vSplitContainer.Panel2Collapsed = true;

            // Hide command panel unless a command is running
            hSplitContainer.Panel2Collapsed = true;

            // Can't seem to specify shortcut keys involving the "Enter" key at design time
            // ...leads to error starting up (something about invalid enum values)
            //mnuEditRepeat.ShortcutKeys = Keys.Enter;
            //mnuEditRecall.ShortcutKeys = Keys.Alt | Keys.Enter;

            // All user interface actions should be handled via the UserActionList technique.
            // Those that deal with map navigation get routed to the map control, the rest
            // will get routed to methods in this class.

            m_Actions = new UserActionList();
        }

        void ShowLoadProgress(string msg)
        {
            infoLabel.Text = msg;
            statusStrip.Refresh();
            SplashScreen.SetStatus(msg);
        }

        internal void OpenJob(string jobName)
        {
            if (String.IsNullOrEmpty(jobName))
                return;

            IJobContainer jc = new JobCollectionFolder();
            ProjectFile job = jc.OpenJob(jobName);
            if (job == null)
                return;

            ForwardingTraceListener trace = new ForwardingTraceListener(ShowLoadProgress);
            Stopwatch sw = Stopwatch.StartNew();
            ProjectSettings ps = job.Settings;

            try
            {
                string increment = ps.SplashIncrement;
                string percents = ps.SplashPercents;

                // Don't show splash screen if it's a brand new file
                //if (percents.Length > 0)
                SplashScreen.ShowSplashScreen(increment, percents);

                Trace.Listeners.Add(trace);
                Trace.Write("Loading " + jobName);

                // Display the map name in the dialog title (nice to see what's loading
                // rather than the default "Map Title" text)
                this.Text = jobName;
                if (!m_Controller.OpenJob(job))
                    throw new ApplicationException("Cannot access job");
            }

            catch (Exception ex)
            {
                SplashScreen.CloseForm();
                MessageBox.Show(ex.Message);
                Trace.Write(ex.StackTrace);

                // Don't save any changes to the job
                job = null;
            }

            finally
            {
                sw.Stop();
                Trace.Listeners.Remove(trace);
                SplashScreen ss = SplashScreen.SplashForm;

                // If the splash screen has been displayed for less than two seconds, WAIT (block
                // this thread!)
                int waitTime = (int)(2000 - sw.ElapsedMilliseconds);
                if (ss != null && waitTime > 0)
                    System.Threading.Thread.Sleep(waitTime);

                ShowLoadProgress(String.Format("Load time: {0:0.00} seconds", sw.ElapsedMilliseconds / 1000.0));
                SplashScreen.CloseForm();

                if (ps != null && ss != null)
                {
                    // Save the splash settings now. This is perhaps a little premature, since
                    // the original splash screen implementation waited until the screen had
                    // completely faded away. The drawback with that is that the job file would
                    // then be rewritten on another thread, which could trample on things that
                    // are happening here.
                    ps.SplashIncrement = ss.GetIncrement();
                    ps.SplashPercents = ss.GetPercents();
                    job.Save();
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            OpenJob(m_InitialProjectFile);

            // If a model hasn't been obtained, ask
            if (m_Controller.CadastralMapModel==null)
            {
                StartupForm dial = new StartupForm(this);
                dial.ShowDialog();
                dial.Dispose();
            }

            // If we still don't have a model, close
            if (m_Controller.CadastralMapModel==null)
            {
                Close();
                return;
            }

            // If the user double-clicked on a file, it may not be in the MRU list, so add it now.
            string jobName = m_Controller.CadastralMapModel.Name;
            m_MruMenu.AddString(jobName);

            // Don't define the model until the screen gets shown for the first time. Otherwise
            // the map control may end up saving an incorrect screen image.
            try
            {
                // Initialize any entity translations and styles
                new EntityUtil().Open();

                InitializeActions();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void InitializeActions()
        {
            // File menu...
            AddAction(new ToolStripItem[] { mnuFileNew, toolFileNew }, IsFileNewEnabled, FileNew);
            AddAction(new ToolStripItem[] { mnuFileOpen, toolFileOpen }, IsFileOpenEnabled, FileOpen);
            AddAction(new ToolStripItem[] { mnuFileSave, toolFileSave }, IsFileSaveEnabled, FileSave);
            m_MruMenu.LoadFromRegistry();
            AddAction(mnuFileShowChanges, IsFileShowChangesEnabled, FileShowChanges);
            AddAction(mnuFileStatistics, IsFileStatisticsEnabled, FileStatistics);
            AddAction(mnuFileCoordinateSystem, IsFileCoordinateSystemEnabled, FileCoordinateSystem);
            AddAction(mnuFileCheck, IsFileCheckEnabled, FileCheck);
            AddAction(mnuFilePrintWindowRotated, IsFilePrintWindowRotatedEnabled, FilePrintWindowRotated);
            AddAction(mnuFilePrintWindow, IsFilePrintWindowEnabled, FilePrintWindow);
            AddAction(new ToolStripItem[] { mnuFilePrint, toolFilePrint }, IsFilePrintEnabled, FilePrint);
            AddAction(mnuFileExit, IsFileExitEnabled, FileExit);

            // Edit menu...
            AddEdit(
                EditingActionId.Deletion,
                new ToolStripItem[] { mnuEditDelete, ctxPointDelete, ctxLineDelete, ctxTextDelete, ctxMultiDelete, toolEditDelete },
                IsEditDeleteEnabled,
                EditDelete);
            AddAction(new ToolStripItem[] { mnuEditUndo, toolEditUndo }, IsEditUndoEnabled, EditUndo);
            AddAction(mnuEditRepeat, IsEditRepeatEnabled, EditRepeat);
            AddAction(mnuEditRecall, IsEditRecallEnabled, EditRecall);
            AddAction(mnuEditOperationHistory, IsEditOperationHistoryEnabled, EditOperationHistory);
            AddAction(mnuEditIdAllocations, IsEditIdAllocationsEnabled, EditIdAllocations);
            AddAction(mnuEditAutoNumber, IsEditAutoNumberEnabled, EditAutoNumber);
            AddAction(mnuEditPreferences, IsEditPreferencesEnabled, EditPreferences);
            AddAction(mnuEditAutoHighlight, IsEditAutoHighlightEnabled, EditAutoHighlight);

            // View menu...
            AddAction(new ToolStripItem[] { mnuViewOverview
                                          , ctxViewOverview
                                          , toolViewOverview}, DisplayToolId.Overview);
            AddAction(new ToolStripItem[] { mnuViewZoomIn
                                          , ctxViewZoomIn
                                          , toolViewZoomIn}, DisplayToolId.ZoomIn);
            AddAction(new ToolStripItem[] { mnuViewZoomOut
                                          , ctxViewZoomOut
                                          , toolViewZoomOut }, DisplayToolId.ZoomOut);
            AddAction(new ToolStripItem[] { mnuViewZoomRectangle
                                          , ctxViewZoomRectangle
                                          , toolViewZoomRectangle }, DisplayToolId.ZoomRectangle);
            AddAction(new ToolStripItem[] { mnuViewDrawScale
                                          , ctxViewDrawScale
                                          , toolViewDrawScale }, DisplayToolId.DrawScale);
            AddAction(new ToolStripItem[] { mnuViewMagnify
                                          , ctxViewMagnify }, DisplayToolId.Magnify);
            AddAction(new ToolStripItem[] { mnuViewNewCentre
                                          , ctxViewNewCenter
                                          , toolViewNewCenter }, DisplayToolId.NewCentre);
            AddAction(new ToolStripItem[] { mnuViewPan
                                          , ctxViewPan
                                          , toolViewPan }, DisplayToolId.Pan);
            AddAction(new ToolStripItem[] { mnuViewRefresh
                                          , ctxViewMapRefresh }, DisplayToolId.MapRefresh);
            AddAction(new ToolStripItem[] { mnuViewPrevious
                                          , ctxViewPrevious }, DisplayToolId.Previous);
            AddAction(new ToolStripItem[] { mnuViewNext
                                          , ctxViewNext }, DisplayToolId.Next);

            AddAction(mnuViewExtraThemes, IsViewExtraThemesEnabled, ViewExtraThemes);
            AddAction(mnuViewAttributeStructure, IsViewAttributeStructureEnabled, ViewAttributeStructure);
            AddAction(mnuViewPosition, IsViewPositionEnabled, ViewPosition);
            AddAction(mnuViewPropertiesWindow, IsViewPropertiesWindowEnabled, ViewPropertiesWindow);
            AddAction(mnuViewToolbar, IsViewToolbarEnabled, ViewToolbar);
            AddAction(mnuViewStatusBar, IsViewStatusBarEnabled, ViewStatusBar);

            // View-Toolbar pull right items
            AddAction(mnuViewFileToolbar, null, ViewFileToolbar);
            AddAction(mnuViewEditToolbar, null, ViewEditToolbar);
            AddAction(mnuViewViewToolbar, null, ViewViewToolbar);
            AddAction(mnuViewPointToolbar, null, ViewPointToolbar);

            // Data menu...
            AddEdit(
                EditingActionId.GetControl,
                new ToolStripItem[] { mnuDataGetControl },
                IsDataGetControlEnabled,
                DataGetControl);
            AddAction(mnuDataImportMap, IsDataImportMapEnabled, DataImportMap);
            AddAction(mnuDataImportAttributes, IsDataImportAttributesEnabled, DataImportAttributes);
            AddAction(mnuDataImportBackground, IsDataImportBackgroundEnabled, DataImportBackground);
            AddAction(mnuDataBackgroundDisplay, IsDataBackgroundDisplayEnabled, DataBackgroundDisplay);
            AddAction(mnuDataAssociateMap, IsDataAssociateMapEnabled, DataAssociateMap);
            AddAction(mnuDataExportToAutoCad, IsDataExportToAutoCadEnabled, DataExportToAutoCad);

            // Point menu...
            AddEdit(
                EditingActionId.NewPoint,
                new ToolStripItem[] { mnuPointNew, ctxPointNew },
                IsPointNewEnabled,
                PointNew);
            AddEdit(
                EditingActionId.AttachPoint,
                new ToolStripItem[] { mnuPointAddOnLine, ctxPointAddOnLine },
                IsPointAddOnLineEnabled,
                PointAddOnLine);
            AddEdit(
                EditingActionId.Path,
                new ToolStripItem[] { mnuPointConnectionPath, toolPointConnectionPath },
                IsPointConnectionPathEnabled,
                PointConnectionPath);
            AddEdit(
                EditingActionId.Radial,
                new ToolStripItem[] { mnuPointSideshot, ctxPointSideshot, toolPointSideshot },
                IsPointSideshotEnabled,
                PointSideshot);
            AddAction(new ToolStripItem[] { mnuPointUpdate
                                          , ctxPointUpdate }, IsPointUpdateEnabled, PointUpdate);
            AddAction(mnuPointBulkUpdate, IsPointBulkUpdateEnabled, PointBulkUpdate);
            AddAction(mnuPointDefaultEntity, IsPointDefaultEntityEnabled, PointDefaultEntity);
            AddAction(new ToolStripItem[] { mnuPointInverseCalculator
                                          , ctxPointInverseCalculator
                                          , toolPointInverseCalculator }, IsPointInverseCalculatorEnabled, PointInverseCalculator);
            AddAction(ctxPointProperties, null, ShowProperties);
            AddAction(toolPointEnlarge, IsPointEnlargeEnabled, PointEnlarge);
            AddAction(toolPointReduce, IsPointReduceEnabled, PointReduce);

            // Line menu...
            AddEdit(
                EditingActionId.NewLine,
                new ToolStripItem[] { mnuLineAddStraightLine, ctxLineAddStraightLine, ctxPointAddStraightLine, toolLineAddStraightLine },
                IsLineAddStraightLineEnabled,
                LineAddStraightLine);
            AddEdit(
                EditingActionId.NewCircularArc,
                new ToolStripItem[] { mnuLineAddCircularArc, ctxLineAddCircularArc, ctxPointAddCircularArc, toolLineAddCircularArc },
                IsLineAddCircularArcEnabled,
                LineAddCircularArc);
            AddEdit(
                EditingActionId.NewCircle,
                new ToolStripItem[] { mnuLineAddCircleConstructionLine },
                IsLineAddCircleConstructionLineEnabled,
                LineAddCircleConstructionLine);
            AddEdit(
                EditingActionId.LineExtend,
                new ToolStripItem[] { mnuLineExtend, ctxLineExtend, toolLineExtend },
                IsLineExtendEnabled,
                LineExtend);
            AddEdit(
                EditingActionId.LineSubdivision,
                new ToolStripItem[] { mnuLineSubdivideLine, ctxLineSubdivide, toolLineSubdivideLine },
                IsLineSubdivideLineEnabled,
                LineSubdivideLine);
            AddEdit(
                EditingActionId.SimpleLineSubdivision,
                new ToolStripItem[] { mnuLineSubdivideLineOneDistance, ctxLineSubdivideOneDistance },
                IsLineSubdivideLineOneDistanceEnabled,
                LineSubdivideLineOneDistance);
            AddEdit(
                EditingActionId.Parallel,
                new ToolStripItem[] { mnuLineParallel, ctxLineParallel },
                IsLineParallelEnabled,
                LineParallel);
            AddAction(new ToolStripItem[] { mnuLineUpdate
                                          , ctxLineUpdate }, IsLineUpdateEnabled, LineUpdate);
            AddEdit(
                EditingActionId.SetTopology,
                new ToolStripItem[] { mnuLinePolygonBoundary, ctxLinePolygonBoundary },
                IsLinePolygonBoundaryEnabled,
                LinePolygonBoundary);
            AddEdit(
                EditingActionId.Trim,
                new ToolStripItem[] { mnuLineTrimDangles, ctxLineTrimDangle, ctxMultiTrim },
                IsLineTrimDanglesEnabled,
                LineTrimDangles);
            AddAction(mnuLineDefaultEntity, IsLineDefaultEntityEnabled, LineDefaultEntity);
            AddEdit(
                EditingActionId.PolygonSubdivision,
                new ToolStripItem[] { mnuLineSubdividePolygon, ctxLineSubdividePolygon, toolLineSubdividePolygon },
                IsLineSubdividePolygonEnabled,
                LineSubdividePolygon);
            AddAction(ctxLineProperties, null, ShowProperties);

            // Text menu...
            AddEdit(
                EditingActionId.NewText,
                new ToolStripItem[] { mnuTextAddMiscellaneousText, ctxTextAddMiscellaneousText },
                IsTextAddMiscellaneousTextEnabled,
                TextAddMiscellaneousText);
            AddEdit(
                EditingActionId.NewText,
                new ToolStripItem[] { mnuTextAddPolygonLabels, ctxTextAddPolygonLabels, toolTextAddPolygonLabels },
                IsTextAddPolygonLabelsEnabled,
                TextAddPolygonLabels);
            AddEdit(
                EditingActionId.MoveLabel,
                new ToolStripItem[] { mnuTextMove, ctxTextMove },
                IsTextMoveEnabled,
                TextMove);
            AddEdit(
                EditingActionId.MovePolygonPosition,
                new ToolStripItem[] { mnuTextMovePolygonPosition, ctxTextMovePolygonPosition },
                IsTextMovePolygonPositionEnabled,
                TextMovePolygonPosition);
            AddEdit(
                EditingActionId.SetLabelRotation,
                new ToolStripItem[] { mnuTextDefaultRotationAngle, toolTextDefaultRotationAngle },
                IsTextDefaultRotationAngleEnabled,
                TextDefaultRotationAngle);
            AddAction(ctxTextProperties, null, ShowProperties);

            // Intersect menu...
            AddEdit(
                EditingActionId.DirIntersect,
                new ToolStripItem[] { mnuIntersectTwoDirections },
                IsIntersectTwoDirectionsEnabled,
                IntersectTwoDirections);
            AddEdit(
                EditingActionId.DistIntersect,
                new ToolStripItem[] { mnuIntersectTwoDistances },
                IsIntersectTwoDistancesEnabled,
                IntersectTwoDistances);
            AddEdit(
                EditingActionId.LineIntersect,
                new ToolStripItem[] { mnuIntersectTwoLines },
                IsIntersectTwoLinesEnabled,
                IntersectTwoLines);
            AddEdit(
                EditingActionId.DirDistIntersect,
                new ToolStripItem[] { mnuIntersectDirectionAndDistance },
                IsIntersectDirectionAndDistanceEnabled,
                IntersectDirectionAndDistance);
            AddEdit(
                EditingActionId.DirLineIntersect,
                new ToolStripItem[] { mnuIntersectDirectionAndLine },
                IsIntersectDirectionAndLineEnabled,
                IntersectDirectionAndLine);

            // Help menu...
            AddAction(mnuHelpTopics, IsHelpTopicsEnabled, HelpTopics);
            AddAction(mnuHelpAbout, IsHelpAboutEnabled, HelpAbout);

            Application.Idle += OnIdle;
        }

        void AddEdit( EditingActionId id
                    , ToolStripItem[] items
                    , UserAction.IsActionEnabled isActionEnabled
                    , UserAction.DoAction doAction)
        {
            m_Actions.Add(new EditingAction(id, items, isActionEnabled, doAction));
        }

        void AddAction(ToolStripItem[] items, DisplayToolId commandId)
        {
            m_Actions.Add(new MapControlAction(mapControl, commandId, items));
        }

        void AddAction(ToolStripItem item
                      , UserAction.IsActionEnabled isActionEnabled
                      , UserAction.DoAction doAction)
        {
            ToolStripItem[] items = new ToolStripItem[] { item };
            AddAction(items, isActionEnabled, doAction);
        }

        void AddAction(ToolStripItem[] items
                      , UserAction.IsActionEnabled isActionEnabled
                      , UserAction.DoAction doAction)
        {
            m_Actions.Add(new UserAction(items, isActionEnabled, doAction));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CadastralMapModel mapModel = CadastralMapModel.Current;
            if (mapModel != null)
            {
                AddRecentJob(mapModel.Name);
                EditingController.Current.CheckSave();
            }

            m_Controller.Close();
            Application.Idle -= OnIdle;
        }

        private void OnIdle(object sender, EventArgs e)
        {
            m_Actions.Update();
            double mapScale = mapControl.MapScale;
            mapScaleLabel.Text = (Double.IsNaN(mapScale) ? "Scale undefined" : String.Format("1:{0:0}", mapScale));

            CadastralMapModel map = CadastralMapModel.Current;

            if (m_Controller==null) // this isn't supposed to happen
            {                
                activeLayerStatusLabel.Text = "No active layer";
                unitsStatusLabel.Text = String.Empty;
                pointEntityStatusLabel.Text = String.Empty;
                lineEntityStatusLabel.Text = String.Empty;
                this.Text = "(Undefined map model)";
            }
            else
            {
                ILayer layer = m_Controller.ActiveLayer;
                activeLayerStatusLabel.Text = (layer==null ? "No active layer" : layer.Name);
                unitsStatusLabel.Text = m_Controller.EntryUnit.ToString();
                IEntity ent = map.DefaultPointType;
                pointEntityStatusLabel.Text = (ent==null ? "No default" : ent.Name);
                ent = map.DefaultLineType;
                lineEntityStatusLabel.Text = (ent==null ? "No default" : ent.Name);
                string name = m_Controller.Project.Name;
                this.Text = (String.IsNullOrEmpty(name) ? "(Untitled map)" : name);
            }

            mnuEditAutoHighlight.Checked = m_Controller.AutoSelect;
            mnuEditAutoNumber.Checked = m_Controller.IsAutoNumber;

            mnuViewPosition.Checked = positionLabel.Visible;
            mnuViewPropertiesWindow.Checked = !vSplitContainer.Panel2Collapsed;
            mnuViewStatusBar.Checked = statusStrip.Visible;
            mnuViewToolbar.Checked = toolStripContainer.TopToolStripPanel.Visible;
            mnuViewFileToolbar.Checked = viewToolStrip.Visible;
            mnuViewEditToolbar.Checked = editToolStrip.Visible;
            mnuViewViewToolbar.Checked = viewToolStrip.Visible;
            mnuViewPointToolbar.Checked = pointToolStrip.Visible;

            m_Controller.OnIdle();
        }

        // Called by EditingController
        new internal void MouseMove(ISpatialDisplay sender, IPosition p, System.Windows.Forms.MouseButtons b)
        {
            if (positionLabel.Visible)
                positionLabel.Text = String.Format("{0:0.000}N, {1:0.000}E", p.Y, p.X);
        }

        internal ContextMenuStrip CreateContextMenu(ISpatialSelection s)
        {
            if (s==null)
                throw new ArgumentNullException();
            /*
void CeView::OnRButtonUp(UINT nFlags, CPoint point) 
{
	// Ensure any limit selection has been included.
	m_Sel.UseLimit();

	// If we were panning, cancel it now (cursor gets screwed up
	// if you don't).
	if ( m_DoPan ) OnViewPan();

	// If we have an active command, and that command can process
	// the right click, just leave it at that (the command thinks
	// it's actually OnRButtonDown, because I originally thought
	// this function was reacting the the initial event!).

	// Commands always expect logical coordinates.

	if ( m_pCommand ) {
		CClientDC dc(this);
		OnPrepareDC(&dc);
		CPoint lpt(point);
		dc.DPtoLP(&lpt);
		if ( m_pCommand->RButtonDown(lpt) ) return;
	}

//	Create appropriate cursor menu
	CMenu menu;
	menu.LoadMenu(IDR_CURSOR_MENU);
	ClientToScreen(&point);

	if ( m_Op == ID_LINE_NEW ) {

		menu.GetSubMenu(1)->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON,
							point.x,point.y,this);
		return;
	}
	else if ( m_Op == ID_LINE_CURVE ) {

		if ( m_IsShortArc )
			menu.CheckMenuRadioItem(ID_SHORT_ARC,ID_LONG_ARC,
										ID_SHORT_ARC, MF_BYCOMMAND);
		else
			menu.CheckMenuRadioItem(ID_SHORT_ARC,ID_LONG_ARC,
										ID_LONG_ARC, MF_BYCOMMAND);

		menu.GetSubMenu(2)->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON,
							point.x,point.y,this);

		return;
	}
             */

            // Handle single-item selections...

            if (s.Item!=null)
            {
                SpatialType t = s.Item.SpatialType;

                if (t == SpatialType.Point)
                    return pointContextMenu;

                if (t == SpatialType.Line)
                {
                    LineFeature line = (s.Item as LineFeature);
                    if (line == null && s.Item is DividerObject)
                        line = (s.Item as DividerObject).Divider.Line;

                    if (line != null)
                        ctxLinePolygonBoundary.Checked = line.HasTopology;

                    return lineContextMenu;
                }

                if (t == SpatialType.Text)
                    return textContextMenu;
            }

            if (s.Count>1)
                return multiSelectContextMenu;

            // Show the default menu, enabling the "Subdivide Polygon" item if a single polygon
            // is currently selected
            ctxLineSubdividePolygon.Enabled = (s.Item!=null && s.Item.SpatialType==SpatialType.Polygon);
            return noSelectionContextMenu;
        }

        private void ShowProperties(IUserAction action)
        {
            if (vSplitContainer.Panel2Collapsed)
                vSplitContainer.Panel2Collapsed = false;

            ISpatialObject so = m_Controller.SpatialSelection.Item;
            propertyDisplay.SetSelectedObject(so);
        }

        #region File menu

        private bool IsFileNewEnabled()
        {
            return true;
        }

        private void FileNew(IUserAction action)
        {
            using (NewJobForm dial = new NewJobForm())
            {
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    ProjectFile job = dial.NewJob;
                    if (m_Controller.OpenJob(job))
                        AddRecentJob(job.Name);
                }
            }
        }

        private bool IsFileOpenEnabled()
        {
            return true;
        }

        private void FileOpen(IUserAction action)
        {
            OpenFile();
        }

        internal bool OpenFile()
        {
            using (GetJobForm dial = new GetJobForm())
            {
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    if (m_Controller.OpenJob(dial.Job))
                    {
                        AddRecentJob(dial.Job.Name);
                        return true;
                    }
                }
            }

            return false;
        }

        void AddRecentJob(string jobName)
        {
            m_MruMenu.AddString(jobName);
            m_MruMenu.SaveToRegistry();
        }

        private bool IsFileSaveEnabled()
        {
            return !m_Controller.IsSaved;
        }

        private void FileSave(IUserAction action)
        {
            ISession s = CadastralMapModel.Current.WorkingSession;
            Debug.Assert(s != null);
            s.SaveChanges();
        }

        void OnMruFile(int number, string filename)
        {
            try
            {
                ProjectFile jf = new ProjectFile(filename);
                if (!m_Controller.OpenJob(jf))
                    throw new ApplicationException();

                m_MruMenu.SetFirstFile(number);
            }

            catch
            {
                string msg = String.Format("Cannot access '{0}'", filename);
                MessageBox.Show(msg);
                m_MruMenu.RemoveFile(number);

                try
                {
                    if (m_Controller.OpenJob(null))
                        m_MruMenu.AddString(m_Controller.Project.Name);
                }

                catch (Exception ex2)
                {
                    // For now, you just get one extra shot at it
                    MessageBox.Show(ex2.Message);
                    Close();
                }
            }

            m_MruMenu.SaveToRegistry();
        }

        private bool IsFileShowChangesEnabled()
        {
            return false;
        }

        private void FileShowChanges(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsFileStatisticsEnabled()
        {
            return false;
        }

        private void FileStatistics(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        /// <summary>
        /// Is the File - Coordinate System command enabled?
        /// </summary>
        /// <returns>True (always)</returns>
        private bool IsFileCoordinateSystemEnabled()
        {
            return true;
        }

        /// <summary>
        /// Displays the dialog the lets the user review the coordinate system of the
        /// current map model.
        /// </summary>
        /// <param name="action">The action that initiated this call</param>
        private void FileCoordinateSystem(IUserAction action)
        {
            using (CoordSystemForm dial = new CoordSystemForm())
            {
                dial.ShowDialog();
            }
        }

        private bool IsFileCheckEnabled()
        {
            return (HasMap && !m_Controller.IsChecking);
        }

        private void FileCheck(IUserAction action)
        {
            EditingController.Current.StartCheck();
        }

        private bool IsFilePrintWindowRotatedEnabled()
        {
            return false;
        }

        private void FilePrintWindowRotated(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsFilePrintWindowEnabled()
        {
            return false;
        }

        private void FilePrintWindow(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsFilePrintEnabled()
        {
            return false;
        }

        private void FilePrint(IUserAction action)
        {
            MessageBox.Show(action.Title);
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

        #region Edit menu

        private bool IsEditDeleteEnabled()
        {
            return (m_Controller.SpatialSelection.Count>0);
        }

        private void EditDelete(IUserAction action)
        {
            DeleteSelection(action);
        }

        void DeleteSelection(IUserAction action)
        {
            CommandUI cmd = new DeletionUI(action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsEditUndoEnabled()
        {
            return !m_Controller.IsCommandRunning;
        }

        private void EditUndo(IUserAction action)
        {
            try
            {
                m_Controller.CheckUndo();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            CommandUI cmd = new UndoUI(action);
            m_Controller.StartCommand(cmd);
        }

        /// <summary>
        /// Handles the KeyDown event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks>The form only reacts to <c>KeyDown</c> events if the <see cref="Form.KeyPreview"/> property
        /// is set to true.</remarks>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // If there is no command running, and the user has hit the Enter (or Return) key,
            // they are indicating that they want to run either the Repeat or the Recall command.
            // The reason for testing this here is because attempting to set the ShortcutKey to
            // Keys.Enter (as I originally did in the constructor) leads to a runtime error that
            // talks about a bad enum value.

            if (e.KeyValue == (int)Keys.Enter)
            {
                if (e.Alt && IsEditRecallEnabled())
                {
                    e.Handled = true;
                    EditRecall(null);
                }
                else if (IsEditRepeatEnabled())
                {
                    e.Handled = true;
                    EditRepeat(null);
                }
            }
        }

        /// <summary>
        /// Checks whether the Edit - Repeat command is enabled or not.
        /// </summary>
        /// <returns>True if the current editing session contains at least one edit, an
        /// edit is not currently running, and a file check is not in progress</returns>
        private bool IsEditRepeatEnabled()
        {
            return (m_Controller.IsCommandRunning==false &&
                    m_Controller.IsChecking==false &&
                    !IsEmptySession);
        }

        /// <summary>
        /// Is the current working session empty (either undefined, or defined but with no edits)?
        /// </summary>
        bool IsEmptySession
        {
            get
            {
                ISession s = CadastralMapModel.Current.WorkingSession;
                return (s==null ? true : s.LastOperation==null);
            }
        }

        /// <summary>
        /// Runs the Edit - Repeat command
        /// </summary>
        /// <param name="action">The action that initiated this edit (null if initiated via a keystroke)</param>
        private void EditRepeat(IUserAction action)
        {
            // Ignore if a command or check is currently active.
            if (!IsEditRepeatEnabled())
                return;

            // Get the last operation.
            Operation op = CadastralMapModel.Current.WorkingSession.LastOperation;
            if (op==null)
                return;

            // Locate the action that initiated the last edit
            IUserAction lastAction = Array.Find<IUserAction>(m_Actions.Actions, delegate (IUserAction a)
            {
                if (a is EditingAction)
                    return ((a as EditingAction).EditId == op.EditId);
                else
                    return false;
            });

            if (lastAction == null)
            {
                MessageBox.Show("Cannot determine action that initiated "+op.Name);
                return;
            }

            // debug...
            //string msg = String.Format("Repeat '{0}' command?", op.Name);
            //if (MessageBox.Show(msg, "Repeat command", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            //    return;

            (lastAction as EditingAction).Do(this, null);
        }

        /// <summary>
        /// Checks whether the Edit - Recall command is enabled or not.
        /// </summary>
        /// <returns>True if the current editing session contains at least one edit, and an
        /// edit is not currently running.</returns>
        private bool IsEditRecallEnabled()
        {
            return (m_Controller.IsCommandRunning==false && !IsEmptySession);
        }

        /// <summary>
        /// Runs the Edit - Recall command
        /// </summary>
        /// <param name="action">The action that initiated this edit (null if initiated via a keystroke)</param>
        private void EditRecall(IUserAction action)
        {
            // Ignore if an edit is currently active.
            if (!IsEditRecallEnabled())
            {
                if (IsEmptySession)
                    MessageBox.Show("There is nothing to recall");
                else
                    MessageBox.Show("An editing command is already running");

                return;
            }

            // Get the user to select an edit from the current session (restricted
            // to those edits that implement IRecallable)
            ISession s = CadastralMapModel.Current.WorkingSession;
            Operation op = null;

            using (PickEditForm dial = new PickEditForm(s))
            {
                if (dial.ShowDialog() == DialogResult.OK)
                    op = dial.SelectedEdit;
            }

            if (op == null)
                return;

            // Locate the action that normally performs the selected edit
            EditingAction recallAction = (EditingAction)Array.Find<IUserAction>(m_Actions.Actions, delegate(IUserAction a)
            {
                if (a is EditingAction)
                    return ((a as EditingAction).EditId == op.EditId);
                else
                    return false;
            });

            if (recallAction == null)
            {
                MessageBox.Show("Cannot determine action that initiated " + op.Name);
                return;
            }

            // Disable auto-highlight.
            m_Controller.AutoSelect = false;

            // Wrap the action alongside information about the edit the user wants to recall
            RecalledEditingAction recall = new RecalledEditingAction(recallAction, op);
            recall.Do(this, null);
        }

        private bool IsEditOperationHistoryEnabled()
        {
            return !m_Controller.IsCommandRunning;
        }

        private void EditOperationHistory(IUserAction action)
        {
            // Turn off any highlighted features (if a feature is undone
            // by rollback, that's bad).
            m_Controller.ClearSelection();

            // Display the info (modal)
            HistoryForm dial = new HistoryForm();
            dial.ShowDialog();
            dial.Dispose();
        }

        private bool IsEditIdAllocationsEnabled()
        {
            CadastralMapModel mapModel = CadastralMapModel.Current;
            return (mapModel != null && mapModel.IdManager != null);
        }

        private void EditIdAllocations(IUserAction action)
        {
            IdAllocationForm dial = new IdAllocationForm();
            dial.ShowDialog();
            dial.Dispose();
        }

        private bool IsEditAutoNumberEnabled()
        {
            return this.HasMap;
        }

        private void EditAutoNumber(IUserAction action)
        {
            if (m_Controller.IsAutoNumber == false && ConnectionFactory.CanConnect() == false)
                MessageBox.Show("Cannot switch auto-number on because a database connection is not available.");
            else
                m_Controller.ToggleAutoNumber();
        }

        private bool IsEditPreferencesEnabled()
        {
            return this.HasMap;
        }

        private void EditPreferences(IUserAction action)
        {
            PreferencesForm dial = new PreferencesForm();
            bool ok = (dial.ShowDialog() == DialogResult.OK);
            dial.Dispose();
            if (ok)
                m_Controller.RefreshAllDisplays();
        }

        private bool IsEditAutoHighlightEnabled()
        {
            return !m_Controller.MapModel.IsEmpty;
        }

        private void EditAutoHighlight(IUserAction action)
        {
            //vSplitContainer.Panel2Collapsed = !vSplitContainer.Panel2Collapsed;
            m_Controller.AutoSelect = !m_Controller.AutoSelect;
        }

        #endregion

        #region View menu

        // Most of the View menu is handled by the map display control

        private bool IsViewExtraThemesEnabled()
        {
            return false;
        }

        private void ViewExtraThemes(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsViewPositionEnabled()
        {
            return !m_Controller.MapModel.IsEmpty;
        }

        private void ViewPosition(IUserAction action)
        {
            positionLabel.Visible = !positionLabel.Visible;
        }

        private bool IsViewPropertiesWindowEnabled()
        {
            return HasMap;
        }

        private void ViewPropertiesWindow(IUserAction action)
        {
            vSplitContainer.Panel2Collapsed = !vSplitContainer.Panel2Collapsed;
        }

        internal void ClosePropertiesWindow()
        {
            vSplitContainer.Panel2Collapsed = true;
        }

        private void propertyDisplay_ControlClosed()
        {
            // Hide the property display
            ClosePropertiesWindow();
        }

        private bool IsViewAttributeStructureEnabled()
        {
            return true;
        }

        private void ViewAttributeStructure(IUserAction action)
        {
            EnvironmentStructureForm dial = new EnvironmentStructureForm();
            dial.ShowDialog();
            dial.Dispose();
        }

        private bool IsViewToolbarEnabled()
        {
            return true;
        }

        private void ViewToolbar(IUserAction action)
        {
            // Toggle visibility of the entire top toolstrip
            toolStripContainer.TopToolStripPanel.Visible = !toolStripContainer.TopToolStripPanel.Visible;
        }

        private void ViewFileToolbar(IUserAction action)
        {
            fileToolStrip.Visible = !fileToolStrip.Visible;
        }

        private void ViewEditToolbar(IUserAction action)
        {
            editToolStrip.Visible = !editToolStrip.Visible;
        }

        private void ViewViewToolbar(IUserAction action)
        {
            viewToolStrip.Visible = !viewToolStrip.Visible;
        }

        private void ViewPointToolbar(IUserAction action)
        {
            pointToolStrip.Visible = !pointToolStrip.Visible;
        }

        private bool IsViewStatusBarEnabled()
        {
            return true;
        }

        private void ViewStatusBar(IUserAction action)
        {
            statusStrip.Visible = !statusStrip.Visible;
        }
        
        #endregion

        #region Data menu

        /// <summary>
        /// Checks whether the Data - GetControl command is enabled or not.
        /// </summary>
        /// <returns>True if a map is available, and no other editing command is running</returns>
        private bool IsDataGetControlEnabled()
        {
            return (HasMap && !m_Controller.IsCommandRunning);
        }

        /// <summary>
        /// Handles the Data - GetControl edit.
        /// </summary>
        /// <param name="action">The user action that initiated this method</param>
        private void DataGetControl(IUserAction action)
        {
            CommandUI cmd = new GetControlUI(action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsDataImportMapEnabled()
        {
            return (CadastralMapModel.Current.WorkingSession != null);
        }

        private void DataImportMap(IUserAction action)
        {
            NtxImportForm dial = new NtxImportForm();
            dial.ShowDialog();
            dial.Dispose();
        }

        private bool IsDataImportAttributesEnabled()
        {
            return false;
        }

        private void DataImportAttributes(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsDataImportBackgroundEnabled()
        {
            return false;
        }

        private void DataImportBackground(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsDataBackgroundDisplayEnabled()
        {
            return false;
        }

        private void DataBackgroundDisplay(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsDataAssociateMapEnabled()
        {
            return false;
        }

        private void DataAssociateMap(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsDataExportToAutoCadEnabled()
        {
            return true;
        }

        private void DataExportToAutoCad(IUserAction action)
        {
            DxfWriter writer = null;

            using (AutoCadExportForm dial = new AutoCadExportForm())
            {
                if (dial.ShowDialog(this) == DialogResult.OK)
                {
                    writer = dial.Writer;
                }
            }

            if (writer != null)
            {
                try
                {
                    writer.WriteFile();
                    MessageBox.Show("Done");
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message);
                }
            }
        }

        #endregion

        #region Point menu

        private bool IsPointNewEnabled()
        {
            return !m_Controller.IsCommandRunning;
        }

        private void PointNew(IUserAction action)
        {
            CommandUI cmd = new NewPointUI(this, action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsPointAddOnLineEnabled()
        {
            return !m_Controller.IsCommandRunning;
        }

        private void PointAddOnLine(IUserAction action)
        {
            CommandUI cmd = new AttachPointUI(action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsPointConnectionPathEnabled()
        {
            return (!m_Controller.IsCommandRunning && ArePointsDrawn);
        }

        private void PointConnectionPath(IUserAction action)
        {
            CommandUI cmd = new PathUI(this, action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsPointSideshotEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Point) && !m_Controller.IsCommandRunning);
        }

        private void PointSideshot(IUserAction action)
        {
            try
            {
                IControlContainer cc = CreateContainer(action);
                CommandUI cmd = new RadialUI(cc, action);
                m_Controller.StartCommand(cmd);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        IControlContainer CreateContainer(IUserAction action)
        {
            // The action title may contain a mnemonic
            string title = action.Title.Replace("&", String.Empty);

            return new ContainerForm(title);
        }

        /// <summary>
        /// The point feature that is currently selected (null if a point is not
        /// selected, or the current selection contains more than one feature).
        /// </summary>
        PointFeature SelectedPoint
        {
            get
            {
                ISpatialObject so = m_Controller.SpatialSelection.Item;
                return (so as PointFeature);
            }
        }

        /// <summary>
        /// The line feature that is currently selected (null if a point is not
        /// selected, or the current selection contains more than one feature).
        /// </summary>
        LineFeature SelectedLine
        {
            get
            {
                ISpatialObject so = m_Controller.SpatialSelection.Item;
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
        TextFeature SelectedText
        {
            get
            {
                ISpatialObject so = m_Controller.SpatialSelection.Item;
                return (so as TextFeature);
            }
        }

        /// <summary>
        /// Displays a control in the container.
        /// </summary>
        /// <param name="c">The control to display</param>
        public void Display(Control c)
        {
            hSplitContainer.Panel2Collapsed = false;
            int tabControlHeight = c.Height + tabControl.ItemSize.Height;
            //int mapHeight = (hSplitContainer.Height - c.Height);
            int mapHeight = (hSplitContainer.Height - tabControlHeight);

            // You'll get an exception if you try to set the SplitterPanel height directly.
            if (mapHeight>10)
                hSplitContainer.SplitterDistance = mapHeight;
            //c.Parent = hSplitContainer.Panel2;
            c.Parent = tabControl.TabPages[0];
            c.Parent.Text = "Testing";
        }

        /// <summary>
        /// Hides all controls in the container.
        /// </summary>
        public void Clear()
        {
            hSplitContainer.Panel2.Controls.Clear();
            hSplitContainer.Panel2Collapsed = true;
        }

        private bool IsPointUpdateEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Point) && !m_Controller.IsCommandRunning);
        }

        private void PointUpdate(IUserAction action)
        {
            PointFeature selPoint = this.SelectedPoint;
            if (selPoint == null)
            {
                MessageBox.Show("You need to select a specific point first.");
                return;
            }

            m_Controller.RunUpdate(action, selPoint);
        }

        private bool IsPointBulkUpdateEnabled()
        {
            return false;
        }

        private void PointBulkUpdate(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsPointDefaultEntityEnabled()
        {
            return HasActiveLayer;
        }

        private void PointDefaultEntity(IUserAction action)
        {
            SetDefaultEntity(SpatialType.Point);
        }

        /// <summary>
        /// Determines whether the inverse calculator can be used
        /// </summary>
        /// <returns>True if points are drawn, and an inverse calculator is not already
        /// in use. Note that an inverse calculator may be used even if an editing
        /// command is in progress.</returns>
        private bool IsPointInverseCalculatorEnabled()
        {
            return (!m_Controller.IsInverseCalculatorRunning && ArePointsDrawn);
        }

        private void PointInverseCalculator(IUserAction action)
        {
            // Display the choices (modal). Then display the selected dialog modeless.
            InverseChoiceForm dial = new InverseChoiceForm();
            if (dial.ShowDialog() == DialogResult.OK)
                m_Controller.StartInverseCalculator(dial.SelectedForm);

            dial.Dispose();
        }

        private bool ArePointsDrawn
        {
            get
            {
                return m_Controller.ArePointsDrawn;
                //ISpatialDisplay display = m_Controller.ActiveDisplay;
                //return (display!=null && display.MapScale <= m_Controller.JobInfo.ShowPointScale);
            }
        }

        private bool IsPointEnlargeEnabled()
        {
            return true;
        }

        private void PointEnlarge(IUserAction action)
        {
            CadastralMapModel cmm = m_Controller.CadastralMapModel;
            ProjectSettings ps = m_Controller.Project.Settings;

            // If points are not currently drawn, set the scale threshold
            // to match the current scale, and set the size to be 1mm at scale.
            if (!this.ArePointsDrawn)
            {
                ISpatialDisplay display = m_Controller.ActiveDisplay;
                Debug.Assert(display!=null);
                double scale = display.MapScale;
                ps.ShowPointScale = (scale + 1.0);
                double height = 0.001 * scale;
                m_Controller.Project.Settings.PointHeight = Math.Max(0.01, height);
            }
            else
            {
                // Increase by a meter on the ground.
                double oldHeight = m_Controller.Project.Settings.PointHeight;
                ps.PointHeight = oldHeight + 1.0;
            }

            // Redraw (no need for erase).
            m_Controller.RefreshAllDisplays();
        }

        private bool IsPointReduceEnabled()
        {
            return this.ArePointsDrawn;
        }

        private void PointReduce(IUserAction action)
        {
            CadastralMapModel cmm = m_Controller.CadastralMapModel;
            ProjectSettings ps = m_Controller.Project.Settings;
            ISpatialDisplay display = m_Controller.ActiveDisplay;
            if (display==null)
                return;

            // Reduce the current size of points by a metre. 
            double height = ps.PointHeight;
            if ((height-1.0) < 1.0)
                height -= 0.2;
            else
                height -= 1.0;

            // What's the new size at the current draw scale? If it ends
            // up below 0.1mm at the current draw scale, turn them off.
            double size = height / display.MapScale;
            if (size < 0.0001)
                ps.ShowPointScale = 0.01; // not sure why 0.01 rather than 0.0
            else
                ps.PointHeight = Math.Max(0.01, height);

            // Force redraw (with erase).
            m_Controller.RefreshAllDisplays();
        }

        #endregion

        #region Line menu

        private bool IsLineAddStraightLineEnabled()
        {
            return (HasMap && !m_Controller.IsCommandRunning);
        }

        private void LineAddStraightLine(IUserAction action)
        {
            CommandUI cmd = new NewLineUI(this, action, SelectedPoint);
            m_Controller.StartCommand(cmd);
        }

        private bool IsLineAddCircularArcEnabled()
        {
            return (HasMap && !m_Controller.IsCommandRunning);
        }

        private void LineAddCircularArc(IUserAction action)
        {
            CommandUI cmd = new NewCircularArcUI(this, action, SelectedPoint);
            m_Controller.StartCommand(cmd);
        }

        private bool IsLineAddCircleConstructionLineEnabled()
        {
            return (HasMap && !m_Controller.IsCommandRunning);
        }

        private void LineAddCircleConstructionLine(IUserAction action)
        {
            CommandUI cmd = new NewCircleUI(this, action);
            m_Controller.StartCommand(cmd);
        }

        /// <summary>
        /// Checks whether the Line - Extend command is enabled or not.
        /// A specific line has to be selected, and there can be no other command currently running.
        /// </summary>
        /// <returns></returns>
        private bool IsLineExtendEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Line) && !m_Controller.IsCommandRunning);
        }

        private void LineExtend(IUserAction action)
        {
            try
            {
                IControlContainer cc = CreateContainer(action);
                CommandUI cmd = new LineExtensionUI(cc, action);
                m_Controller.StartCommand(cmd);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Checks whether the Line - Subdivide command is enabled or not.
        /// </summary>
        /// <returns>True if a specific line has to be selected, and there is no other command
        /// currently running.</returns>
        private bool IsLineSubdivideLineEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Line) && !m_Controller.IsCommandRunning);
        }

        private void LineSubdivideLine(IUserAction action)
        {
            try
            {
                IControlContainer cc = CreateContainer(action);
                CommandUI cmd = new LineSubdivisionUI(cc, action);
                m_Controller.StartCommand(cmd);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Checks whether the Line - Subdivide (One Distance) command is enabled or not.
        /// </summary>
        /// <returns>True if a specific line is selected, and no other command is currently running.</returns>
        private bool IsLineSubdivideLineOneDistanceEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Line) && !m_Controller.IsCommandRunning);
        }

        private void LineSubdivideLineOneDistance(IUserAction action)
        {
            try
            {
                IControlContainer cc = CreateContainer(action);
                CommandUI cmd = new SimpleLineSubdivisionUI(cc, action);
                m_Controller.StartCommand(cmd);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsLineParallelEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Line) && !m_Controller.IsCommandRunning);
        }

        private void LineParallel(IUserAction action)
        {
            try
            {
                IControlContainer cc = CreateContainer(action);
                CommandUI cmd = new ParallelLineUI(cc, action);
                m_Controller.StartCommand(cmd);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsLineUpdateEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Line) && !m_Controller.IsCommandRunning);
        }

        private void LineUpdate(IUserAction action)
        {
            LineFeature selLine = this.SelectedLine;
            if (selLine == null)
            {
                MessageBox.Show("You need to select a specific line first.");
                return;
            }

            m_Controller.RunUpdate(action, selLine);
        }

        private bool IsLinePolygonBoundaryEnabled()
        {
            LineFeature line = SelectedLine;
            mnuLinePolygonBoundary.Checked = (line!=null && line.IsTopological);
            return (line!=null && !m_Controller.IsCommandRunning);
        }

        private void LinePolygonBoundary(IUserAction action)
        {
            LineFeature line = SelectedLine;
            if (line==null)
            {
                MessageBox.Show("You must select a specific line first.");
                return;
            }

            CommandUI cmd = new SetTopologyUI(action, line);
            m_Controller.StartCommand(cmd);
        }

        /// <summary>
        /// Checks whether the Line - Trim Dangles command is enabled or not.
        /// </summary>
        /// <returns>True if the current selection contains at least one line, and no other command is
        /// currently running.</returns>
        /// <remarks>
        /// The UI will be enabled even if a selected line is non-topological. If you don't, and
        /// you've picked a non-topological line, and you trim via the right click button, nothing
        /// appears to happen. By letting it through, the user will at least be told why the line
        /// can't be trimmed.
        /// </remarks>
        private bool IsLineTrimDanglesEnabled()
        {
            return (m_Controller.IsLineSelected() && !m_Controller.IsCommandRunning);
        }

        private void LineTrimDangles(IUserAction action)
        {
            CommandUI cmd = new TrimLineUI(action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsLineDefaultEntityEnabled()
        {
            return HasActiveLayer;
        }

        private void LineDefaultEntity(IUserAction action)
        {
            SetDefaultEntity(SpatialType.Line);
        }

        private bool IsLineSubdividePolygonEnabled()
        {
            return !m_Controller.IsCommandRunning;
        }

        private void LineSubdividePolygon(IUserAction action)
        {
            CommandUI cmd = new PolygonSubdivisionUI(action);
            m_Controller.StartCommand(cmd);
        }

        #endregion

        #region Text menu

        /// <summary>
        /// Checks whether the Text - Add Miscellaneous Text command is currently enabled.
        /// </summary>
        /// <returns>True if an editing command is not running</returns>
        private bool IsTextAddMiscellaneousTextEnabled()
        {
            return !m_Controller.IsCommandRunning;
        }

        /// <summary>
        /// Starts the Text - Add Miscellaneous Text command. This involves an initial
        /// check to confirm that text is currently drawn (if not, the user will be told
        /// and the command will not be started).
        /// </summary>
        /// <param name="action">The action that initiated this call</param>
        private void TextAddMiscellaneousText(IUserAction action)
        {
            // Confirm that text is currently displayed
            if (!IsTextDrawn)
            {
                MessageBox.Show("Text is not currently displayed. Use Edit-Preferences to change the scale at which text will be drawn");
                return;
            }

            CommandUI cmd = new NewTextUI(this, action);
            m_Controller.StartCommand(cmd);
        }

        /// <summary>
        /// Are text labels currently drawn to the active display?
        /// </summary>
        bool IsTextDrawn
        {
            get
            {
                return m_Controller.AreLabelsDrawn;
                //ISpatialDisplay display = m_Controller.ActiveDisplay;
                //return (display!=null && display.MapScale <= m_Controller.JobInfo.Settings.ShowLabelScale);
            }
        }

        private bool IsTextAddPolygonLabelsEnabled()
        {
            return !m_Controller.IsCommandRunning;
        }

        private void TextAddPolygonLabels(IUserAction action)
        {
            // Confirm that text is currently displayed
            if (!IsTextDrawn)
            {
                MessageBox.Show("Text is not currently displayed. Use Edit-Preferences to change the scale at which text will be drawn");
                return;
            }

            CommandUI cmd = new NewLabelUI(this, action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsTextMoveEnabled()
        {
            return (SelectedText!=null && !m_Controller.IsCommandRunning);
        }

        private void TextMove(IUserAction action)
        {
            // Confirm that text is currently displayed
            if (!IsTextDrawn)
            {
                MessageBox.Show("Text is not currently displayed. Use Edit-Preferences to change the scale at which text will be drawn");
                return;
            }

            TextFeature text = SelectedText;
            if (text == null)
            {
                MessageBox.Show("You must first select some text.");
                return;
            }

            CommandUI cmd = new MoveTextUI(this, action, text);
            m_Controller.StartCommand(cmd);
        }

        private bool IsTextMovePolygonPositionEnabled()
        {
            TextFeature t = SelectedText;
            return (t!=null && t.IsTopological && !m_Controller.IsCommandRunning);
        }

        private void TextMovePolygonPosition(IUserAction action)
        {
            // Confirm that text is currently displayed
            if (!IsTextDrawn)
            {
                MessageBox.Show("Text is not currently displayed. Use Edit-Preferences to change the scale at which text will be drawn");
                return;
            }

            TextFeature text = SelectedText;
            if (text == null)
            {
                MessageBox.Show("You must first select some text.");
                return;
            }

            if (!text.IsTopological)
            {
                MessageBox.Show("Selected text is not marked as a polygon label");
                return;
            }

            CommandUI cmd = new MovePolygonPositionUI(this, action, text);
            m_Controller.StartCommand(cmd);
        }

        private bool IsTextDefaultRotationAngleEnabled()
        {
            return !m_Controller.IsCommandRunning;
        }

        private void TextDefaultRotationAngle(IUserAction action)
        {
            CommandUI cmd = new TextRotationUI(this, action);
            m_Controller.StartCommand(cmd);
        }

        #endregion

        #region Intersect menu

        bool IsIntersectEnabled()
        {
            return (HasMap && !m_Controller.IsCommandRunning);
        }

        void StartIntersectUI(IUserAction action)
        {
            CommandUI cmd = new IntersectUI(action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsIntersectTwoDirectionsEnabled()
        {
            return IsIntersectEnabled();
        }

        private void IntersectTwoDirections(IUserAction action)
        {
            StartIntersectUI(action);
        }

        private bool IsIntersectTwoDistancesEnabled()
        {
            return IsIntersectEnabled();
        }

        private void IntersectTwoDistances(IUserAction action)
        {
            StartIntersectUI(action);
        }

        private bool IsIntersectTwoLinesEnabled()
        {
            return IsIntersectEnabled();
        }

        private void IntersectTwoLines(IUserAction action)
        {
            StartIntersectUI(action);
        }

        private bool IsIntersectDirectionAndDistanceEnabled()
        {
            return IsIntersectEnabled();
        }

        private void IntersectDirectionAndDistance(IUserAction action)
        {
            StartIntersectUI(action);
        }

        private bool IsIntersectDirectionAndLineEnabled()
        {
            return IsIntersectEnabled();
        }

        private void IntersectDirectionAndLine(IUserAction action)
        {
            StartIntersectUI(action);
        }

        #endregion

        #region Help menu

        private bool IsHelpTopicsEnabled()
        {
            return false;
        }

        private void HelpTopics(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsHelpAboutEnabled()
        {
            return true; // for test stubs
        }

        private void HelpAbout(IUserAction action)
        {
            /*
            Stopwatch sw = Stopwatch.StartNew();
            string wkt = EditingController.Current.GetCoordinateSystemText();
            sw.Stop();
            MessageBox.Show(wkt);
            MessageBox.Show("That took " + sw.ElapsedMilliseconds / 1000.0);
             */

            /*
            string cfile = GlobalUserSetting.Read("ControlFile");
            MessageBox.Show(cfile);
            ControlPoint control;
            int nfound = 0;
            double maxShift = 0.0;

            using (StreamReader sr = File.OpenText(cfile))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    if (ControlPoint.TryParse(str, out control))
                    {
                        nfound++;
                        IPosition b = new Position(control.X + 100.0, control.Y);
                        double sfac1 = cmm.CoordinateSystem.GetLineScaleFactor(control, b);
                        double sfac2 = cs.GetLineScaleFactor(control, b);
                        double diff = 100.0 * (sfac1 - sfac2);
                        maxShift = Math.Max(maxShift, Math.Abs(diff));
                        String msg = String.Format("SF1={0}\t SF2={1}\t diff={2:0.000000}", sfac1, sfac2, diff);
                    }
                }
            }

            MessageBox.Show(String.Format("maxShift={0:0.000000}", maxShift));
            return;
            */

            /*
            // Experiment with recursive query

            using (Backsight.Data.IConnection ic = Backsight.Data.ConnectionFactory.Create())
            {
                SqlConnection c = ic.Value;
 
                string sql = "create table TestChain (EditId int not null, PreviousEditId int not null)";
                SqlCommand cmd = new SqlCommand(sql, c);
                cmd.ExecuteNonQuery();


                SqlCommand del = new SqlCommand("DELETE FROM TestChain", c);
                del.ExecuteNonQuery();

                SqlCommand ins = new SqlCommand("insert into TestChain (EditId, PreviousEditId) values (@EditId, @PreviousEditId)", c);
                SqlParameter p0 = new SqlParameter("@EditId", SqlDbType.Int);
                SqlParameter p1 = new SqlParameter("@PreviousEditId", SqlDbType.Int);
                p0.Value = 0;
                p1.Value = 0;
                ins.Parameters.Add(p0);
                ins.Parameters.Add(p1);
                ins.ExecuteNonQuery();

                for (int i=1; i<50000; i++)
                {
                    p0.Value = i;
                    p1.Value = i-1;
                    ins.ExecuteNonQuery();
                }

                MessageBox.Show("inserted 50000 rows");
            }
             */

            /*
             * 
             * You'll need PK on EditId as well
             * 
WITH result(EditId, PreviousEditId) AS
(
  SELECT EditId, PreviousEditId
  FROM TestChain
  WHERE (EditId = 49999)

  UNION ALL

  SELECT t.EditId, t.PreviousEditId
  FROM TestChain AS t, result AS r WHERE t.EditId = r.PreviousEditId
  and r.EditId != r.PreviousEditId
)
SELECT EditId FROM Result
OPTION (MAXRECURSION 0)
             */
        }

        #endregion

        bool HasActiveLayer
        {
            get { return (ActiveLayer != null); }
        }

        ILayer ActiveLayer
        {
            get { return m_Controller.ActiveLayer;; }
        }

        void SetDefaultEntity(SpatialType t)
        {
            ILayer layer = ActiveLayer;
            if (layer==null)
                return;

            IEntity e = CadastralMapModel.Current.GetDefaultEntity(t);
            int entId = (e == null ? 0 : e.Id);
            GetEntityForm dial = new GetEntityForm(layer, t, entId);
            if (dial.ShowDialog() == DialogResult.OK)
            {
                e = dial.SelectedEntity;
                CadastralMapModel.Current.SetDefaultEntity(t, e);
            }
            dial.Dispose();
        }

        bool HasMap
        {
            get { return (m_Controller.MapModel is CadastralMapModel); }
        }

        /// <summary>
        /// Displays properties of currently selected spatial object (so long as
        /// the properties grid is shown).
        /// </summary>
        /// <param name="o"></param>
        internal void SetSelection(ISpatialObject o)
        {
            /*
            // TEST...
            Polygon p = (o as Polygon);
            if (p != null)
            {
                IPosition[] edge = p.GetOutline(new Length(0.001));
                double ga1 = CadastralMapModel.Current.SpatialSystem.GetGroundArea(edge);
                double ga2 = CadastralMapModel.Current.CS.GetGroundArea(edge);
                MessageBox.Show(String.Format("ground area: {0:0.000000} vs {1:0.000000} - diff = {2:0.000000}",
                            ga1, ga2, ga1 - ga2));
            }
            */
            if (vSplitContainer.Panel2Collapsed)
                return;

            propertyDisplay.SetSelectedObject(o);
        }

        private void mnuPointDefaultEntity_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Invokes the dialog for updating the attributes of a polygon
        /// </summary>
        /// <param name="p">The polygon of interest (not null)</param>
        internal void UpdatePolygon(Polygon p)
        {
            // Locate the attributes for the first label associated with RowText
            TextFeature[] labels = p.Labels;
            Row r = null;
            foreach (TextFeature tf in labels)
            {
                RowTextGeometry tg = (tf.TextGeometry as RowTextGeometry);
                if (tg != null)
                {
                    r = tg.Row;
                    break;
                }
            }

            // If we didn't find any RowText, arbitrarily select the first row
            if (r == null)
            {
                foreach (TextFeature tf in labels)
                {
                    FeatureId fid = tf.FeatureId;
                    if (fid!=null && fid.RowCount>0)
                    {
                        r = fid.Rows[0];
                        break;
                    }
                }
            }

            if (r == null)
                MessageBox.Show("No attributes for selected polygon.");
            else
                AttributeData.Update(r);
        }
    }
}
