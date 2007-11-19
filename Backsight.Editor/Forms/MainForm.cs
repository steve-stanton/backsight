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
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

using Backsight.Editor.Operations;
using Backsight.Forms;
using Backsight.Editor.Properties;
using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    public partial class MainForm : Form, IControlContainer
    {
        #region Class data

        private readonly UserActionList m_Actions;
        private readonly CadastralEditController m_Controller;

        /// <summary>
        /// File check command (null if not active).
        /// </summary>
        /// <remarks>Will probably be moved to CadastralEditController when it's implemented for real.
        /// </remarks>
        FileCheckUI m_Check;

        #endregion

        public MainForm()
        {
            // Define the controller for the application
            m_Controller = new CadastralEditController(this);

            InitializeComponent();

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

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string cs = GlobalUserSetting.LastConnection;
            if (String.IsNullOrEmpty(cs))
            {
                MessageBox.Show("Cannot obtain connection to environment database.");
                Close();
            }

            // Don't define the model until the screen gets shown for the first time. Otherwise
            // the map control may end up saving an incorrect screen image.
            m_Controller.OnStartup(cs);

            InitializeActions();
        }

        private void InitializeActions()
        {
            // File menu...
            AddAction(new ToolStripItem[] { mnuFileNew, toolFileNew }, IsFileNewEnabled, FileNew);
            AddAction(new ToolStripItem[] { mnuFileOpen, toolFileOpen }, IsFileOpenEnabled, FileOpen);
            AddAction(new ToolStripItem[] { mnuFileSave, toolFileSave }, IsFileSaveEnabled, FileSave);
            AddAction(new ToolStripItem[] { mnuFileSaveAs }, IsFileSaveAsEnabled, FileSaveAs);
            AddAction(mnuFileShowChanges, IsFileShowChangesEnabled, FileShowChanges);
            AddAction(mnuFileStatistics, IsFileStatisticsEnabled, FileStatistics);
            AddAction(mnuFileCoordinateSystem, IsFileCoordinateSystemEnabled, FileCoordinateSystem);
            AddAction(mnuFileCheck, IsFileCheckEnabled, FileCheck);
            AddAction(mnuFileUpdateSchema, IsFileUpdateSchemaEnabled, FileUpdateSchema);
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
            AddAction(mnuEditSetActiveTheme, IsEditSetEditLayerEnabled, EditSetEditLayer);
            AddAction(mnuEditIdAllocations, IsEditIdAllocationsEnabled, EditIdAllocations);
            AddAction(mnuEditAutoNumber, IsEditAutoNumberEnabled, EditAutoNumber);
            AddAction(mnuEditPreferences, IsEditPreferencesEnabled, EditPreferences);
            AddAction(mnuEditAutoHighlight, IsEditAutoHighlightEnabled, EditAutoHighlight);
            AddAction(mnuEditBackupLimit, IsEditBackupLimitEnabled, EditBackupLimit);

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
            AddAction(new ToolStripItem[] { mnuPointConnectionPath, toolPointConnectionPath }, IsPointConnectionPathEnabled, PointConnectionPath);
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
            AddAction(new ToolStripItem[] { mnuLineAddCircularArc
                                          , ctxLineAddCircularArc
                                          , ctxPointAddCircularArc
                                          , toolLineAddCircularArc }, IsLineAddCircularArcEnabled, LineAddCircularArc);
            AddAction(mnuLineAddCircleConstructionLine, IsLineAddCircleConstructionLineEnabled, LineAddCircleConstructionLine);
            AddEdit(
                EditingActionId.LineExtend,
                new ToolStripItem[] { mnuLineExtend, ctxLineExtend, toolLineExtend },
                IsLineExtendEnabled,
                LineExtend);
            AddAction(new ToolStripItem[] { mnuLineSubdivideLine
                                          , ctxLineSubdivide
                                          , toolLineSubdivideLine }, IsLineSubdivideLineEnabled, LineSubdivideLine);
            AddEdit(
                EditingActionId.PointOnLine,
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
            AddAction(new ToolStripItem[] { mnuLineTrimDangles
                                          , ctxLineTrimDangle
                                          , ctxMultiTrim }, IsLineTrimDanglesEnabled, LineTrimDangles);
            AddAction(mnuLineDefaultEntity, IsLineDefaultEntityEnabled, LineDefaultEntity);
            AddEdit(
                EditingActionId.PolygonSubdivision,
                new ToolStripItem[] { mnuLineSubdividePolygon, ctxLineSubdividePolygon, toolLineSubdividePolygon },
                IsLineSubdividePolygonEnabled,
                LineSubdividePolygon);
            AddAction(ctxLineProperties, null, ShowProperties);

            // Text menu...
            AddAction(new ToolStripItem[] { mnuTextAddMiscellaneousText
                                          , ctxTextAddMiscellaneousText }, IsTextAddMiscellaneousTextEnabled, TextAddMiscellaneousText);
            AddAction(new ToolStripItem[] { mnuTextAddPolygonLabels
                                          , ctxTextAddPolygonLabels
                                          , toolTextAddPolygonLabels }, IsTextAddPolygonLabelsEnabled, TextAddPolygonLabels);
            AddAction(new ToolStripItem[] { mnuTextMove
                                          , ctxTextMove }, IsTextMoveEnabled, TextMove);
            AddAction(new ToolStripItem[] { mnuTextDefaultRotationAngle
                                          , toolTextDefaultRotationAngle }, IsTextDefaultRotationAngleEnabled, TextDefaultRotationAngle);
            AddAction(ctxTextProperties, null, ShowProperties);

            // Polygon menu (only part of context menu)...
            AddAction(ctxPolygonDisplayAttributes, IsPolygonDisplayAttributesEnabled, PolygonDisplayAttributes);
            AddAction(ctxPolygonEditAttributes, IsPolygonEditAttributesEnabled, PolygonEditAttributes);
            AddAction(ctxPolygonProperties, null, ShowProperties);

            // Intersect menu...
            AddAction(mnuIntersectTwoDirections, IsIntersectTwoDirectionsEnabled, IntersectTwoDirections);
            AddAction(mnuIntersectTwoDistances, IsIntersectTwoDistancesEnabled, IntersectTwoDistances);
            AddAction(mnuIntersectTwoLines, IsIntersectTwoLinesEnabled, IntersectTwoLines);
            AddAction(mnuIntersectDirectionAndDistance, IsIntersectDirectionAndDistanceEnabled, IntersectDirectionAndDistance);
            AddAction(mnuIntersectDirectionAndLine, IsIntersectDirectionAndLineEnabled, IntersectDirectionAndLine);

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
            CadastralMapModel map = CadastralMapModel.Current;
            if (map!=null && String.IsNullOrEmpty(map.Name) && !map.IsEmpty)
            {
                if (MessageBox.Show("Do you want to save the current map?", "Map not saved", MessageBoxButtons.YesNo)
                    == DialogResult.Yes)
                {
                    string name = AskForFileName();
                    if (!String.IsNullOrEmpty(name))
                        map.Name = name;
                    else
                        m_Controller.DiscardModel();
                }
                else
                    m_Controller.DiscardModel();
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
            if (map==null) // this isn't supposed to happen
            {                
                activeLayerStatusLabel.Text = "No active layer";
                unitsStatusLabel.Text = String.Empty;
                pointEntityStatusLabel.Text = String.Empty;
                lineEntityStatusLabel.Text = String.Empty;
                this.Text = "(Undefined map model)";
            }
            else
            {
                ILayer layer = map.ActiveLayer;
                activeLayerStatusLabel.Text = (layer==null ? "No active layer" : layer.Name);
                unitsStatusLabel.Text = map.EntryUnit.ToString();
                IEntity ent = map.DefaultPointType;
                pointEntityStatusLabel.Text = (ent==null ? "No default" : ent.Name);
                ent = map.DefaultLineType;
                lineEntityStatusLabel.Text = (ent==null ? "No default" : ent.Name);
                string name = map.Name;
                this.Text = (String.IsNullOrEmpty(name) ? "(Untitled map)" : name);
            }

            mnuEditAutoHighlight.Checked = m_Controller.AutoSelect;

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

        // Called by CadastralEditController
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
                    if (line != null)
                        ctxLinePolygonBoundary.Checked = line.HasTopology;

                    return lineContextMenu;
                }

                if (t == SpatialType.Text)
                    return textContextMenu;
            }

            if (s.Count>1)
                return multiSelectContextMenu;

            // Show the default menu, enabling the "Polygon" item if a single polygon
            // is currently selected
            ctxPolygon.Enabled = (s.Item!=null && s.Item.SpatialType==SpatialType.Polygon);
            return noSelectionContextMenu;
        }

        private void ShowProperties(IUserAction action)
        {
            if (vSplitContainer.Panel2Collapsed)
                vSplitContainer.Panel2Collapsed = false;

            ISpatialObject so = m_Controller.Selection.Item;
            propertyGrid.SelectedObject = so;
        }

        #region File menu

        private bool IsFileNewEnabled()
        {
            return true;
        }

        private void FileNew(IUserAction action)
        {
            m_Controller.Create();
        }

        private bool IsFileOpenEnabled()
        {
            return true;
        }

        private void FileOpen(IUserAction action)
        {
            OpenFileDialog dial = new OpenFileDialog();
            dial.Filter = "Backsight files (*.bs)|*.bs|All files (*)|*";

            if (dial.ShowDialog() == DialogResult.OK)
                m_Controller.Open(dial.FileName);
        }

        private bool IsFileSaveEnabled()
        {
            CadastralMapModel mm = CadastralMapModel.Current;
            return (mm!=null && !String.IsNullOrEmpty(mm.Name));
        }

        private void FileSave(IUserAction action)
        {
            CadastralMapModel.Current.Write();
        }

        private bool IsFileSaveAsEnabled()
        {
            return true;
        }

        private void FileSaveAs(IUserAction action)
        {
            string name = AskForFileName();
            if (!String.IsNullOrEmpty(name))
                CadastralMapModel.Current.Write(name);
        }

        internal string AskForFileName()
        {
            SaveFileDialog dial = new SaveFileDialog();
            dial.DefaultExt = "bs";
            if (dial.ShowDialog() == DialogResult.OK)
                return dial.FileName;

            return String.Empty;
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

        private bool IsFileCoordinateSystemEnabled()
        {
            return true;
        }

        private void FileCoordinateSystem(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsFileCheckEnabled()
        {
            return (HasMap && !m_Controller.IsChecking);
        }

        private void FileCheck(IUserAction action)
        {
            CadastralEditController.Current.StartCheck();
        }

        private bool IsFileUpdateSchemaEnabled()
        {
            return false;
        }

        private void FileUpdateSchema(IUserAction action)
        {
            MessageBox.Show(action.Title);
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
            return (m_Controller.Selection.Count>0);
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
            CadastralMapModel map = CadastralMapModel.Current;

            // If a check is running, confirm that we can really rollback (you can only undo
            // edits that you made since the check was started)
            if (m_Check!=null)
            {
                uint lastop = map.LastOpSequence;
                if (!m_Check.CanRollback(lastop))
                {
                    MessageBox.Show("Cannot undo prior to beginning of File-Check");
                    return;
                }
            }

            CommandUI cmd = new UndoUI(action);
            m_Controller.StartCommand(cmd);
        }

        private bool IsEditRepeatEnabled()
        {
            return false;
        }

        private void EditRepeat(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsEditRecallEnabled()
        {
            return false;
        }

        private void EditRecall(IUserAction action)
        {
            MessageBox.Show(action.Title);
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

        private bool IsEditSetEditLayerEnabled()
        {
            //	You can select the editing layer if the map contains at least
            //	two defined layers and a file check is not underway.

            if (m_Controller.IsChecking)
                return false;

            ILayer[] layers = EnvironmentContainer.Current.Layers;
            return layers.Length>1;
        }

        private void EditSetEditLayer(IUserAction action)
        {
            GetLayerForm dial = new GetLayerForm(m_Controller.ActiveLayer);
            if (dial.ShowDialog() == DialogResult.OK)
                m_Controller.ActiveLayer = dial.SelectedLayer;
        }

        private bool IsEditIdAllocationsEnabled()
        {
            return this.HasMap;
        }

        private void EditIdAllocations(IUserAction action)
        {
            IdAllocationForm dial = new IdAllocationForm();
            dial.ShowDialog();
            dial.Dispose();
        }

        private bool IsEditAutoNumberEnabled()
        {
            return false;
        }

        private void EditAutoNumber(IUserAction action)
        {
            MessageBox.Show(action.Title);
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

        private bool IsEditBackupLimitEnabled()
        {
            return false;
        }

        private void EditBackupLimit(IUserAction action)
        {
            MessageBox.Show(action.Title);
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

        private bool IsViewAttributeStructureEnabled()
        {
            return false;
        }

        private void ViewAttributeStructure(IUserAction action)
        {
            MessageBox.Show(action.Title);
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

        private bool IsDataGetControlEnabled()
        {
            return false;
        }

        private void DataGetControl(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsDataImportMapEnabled()
        {
            return (Session.CurrentSession!=null);
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
            return false;
        }

        private void DataExportToAutoCad(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }
        #endregion

        #region Point menu

        private bool IsPointNewEnabled()
        {
            return true;
        }

        private void PointNew(IUserAction action)
        {
            CommandUI cmd = new NewPointUI(this, action);
            m_Controller.StartCommand(cmd);

            /*
            NewPointForm dial = new NewPointForm();
            dial.ShowDialog();
            dial.Dispose();
             */
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
            return false;
        }

        private void PointConnectionPath(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsPointSideshotEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Point) && !m_Controller.IsCommandRunning);
        }

        private void PointSideshot(IUserAction action)
        {
            PointFeature selPoint = this.SelectedPoint;
            if (selPoint==null)
            {
                MessageBox.Show("You must initially select the point the sideshot radiates from.");
                return;
            }

            IControlContainer cc = CreateContainer(action);
            CommandUI cmd = new RadialUI(cc, action, selPoint);
            m_Controller.StartCommand(cmd);
        }

        IControlContainer CreateContainer(IUserAction action)
        {
            //return this;
            return new ContainerForm(action);
        }

        PointFeature SelectedPoint
        {
            get
            {
                ISpatialObject so = m_Controller.Selection.Item;
                return (so as PointFeature);
            }
        }

        LineFeature SelectedLine
        {
            get
            {
                ISpatialObject so = m_Controller.Selection.Item;
                return (so as LineFeature);
            }
        }

        TextFeature SelectedText
        {
            get
            {
                ISpatialObject so = m_Controller.Selection.Item;
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
            return false;
        }

        private void PointUpdate(IUserAction action)
        {
            MessageBox.Show(action.Title);
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
                CadastralMapModel cmm = m_Controller.CadastralMapModel;
                ISpatialDisplay display = m_Controller.ActiveDisplay;
                return (display!=null && display.MapScale <= cmm.ShowPointScale);
            }
        }

        private bool IsPointEnlargeEnabled()
        {
            return true;
        }

        private void PointEnlarge(IUserAction action)
        {
            CadastralMapModel cmm = m_Controller.CadastralMapModel;

            // If points are not currently drawn, set the scale threshold
            // to match the current scale, and set the size to be 1mm at scale.
            if (!this.ArePointsDrawn)
            {
                ISpatialDisplay display = m_Controller.ActiveDisplay;
                Debug.Assert(display!=null);
                double scale = display.MapScale;
                cmm.ShowPointScale = (scale + 1.0);
                double height = 0.001 * scale;
                cmm.PointHeight = new Length(Math.Max(0.01, height));
            }
            else
            {
                // Increase by a metre on the ground.
                double oldHeight = cmm.PointHeight.Meters;
                cmm.PointHeight = new Length(oldHeight + 1.0);
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
            ISpatialDisplay display = m_Controller.ActiveDisplay;
            if (display==null)
                return;

            // Reduce the current size of points by a metre. 
            double height = cmm.PointHeight.Meters;
            if ((height-1.0) < 1.0)
                height -= 0.2;
            else
                height -= 1.0;

            // What's the new size at the current draw scale? If it ends
            // up below 0.1mm at the current draw scale, turn them off.
            double size = height / display.MapScale;
            if (size < 0.0001)
                cmm.ShowPointScale = 0.01; // not sure why 0.01 rather than 0.0
            else
                cmm.PointHeight = new Length(Math.Max(0.01, height));

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
            return false;
        }

        private void LineAddCircularArc(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsLineAddCircleConstructionLineEnabled()
        {
            return false;
        }

        private void LineAddCircleConstructionLine(IUserAction action)
        {
            MessageBox.Show(action.Title);
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
            LineFeature selLine = this.SelectedLine;
            if (selLine==null)
            {
                MessageBox.Show("You must initially select the line you want to extend.");
                return;
            }

            IControlContainer cc = CreateContainer(action);
            CommandUI cmd = new LineExtensionUI(cc, action, selLine);
            m_Controller.StartCommand(cmd);
        }

        /// <summary>
        /// Checks whether the Line - Subdivide command is enabled or not.
        /// A specific line has to be selected, and there can be no other command currently running.
        /// </summary>
        /// <returns></returns>
        private bool IsLineSubdivideLineEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Line) && !m_Controller.IsCommandRunning);
        }

        private void LineSubdivideLine(IUserAction action)
        {
            LineFeature selLine = this.SelectedLine;
            if (selLine==null)
            {
                MessageBox.Show("You must initially select the line you want to subdivide.");
                return;
            }

            IControlContainer cc = CreateContainer(action);
            CommandUI cmd = new LineSubdivisionUI(cc, action, selLine);
            m_Controller.StartCommand(cmd);
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
            LineFeature selLine = this.SelectedLine;
            if (selLine==null)
            {
                MessageBox.Show("You must initially select the line you want to subdivide.");
                return;
            }

            IControlContainer cc = CreateContainer(action);
            CommandUI cmd = new PointOnLineUI(cc, action, selLine);
            m_Controller.StartCommand(cmd);
        }

        private bool IsLineParallelEnabled()
        {
            return (m_Controller.IsItemSelected(SpatialType.Line) && !m_Controller.IsCommandRunning);
        }

        private void LineParallel(IUserAction action)
        {
            LineFeature selLine = this.SelectedLine;
            if (selLine==null)
            {
                MessageBox.Show("You must initially select the reference line for the parallel.");
                return;
            }

            IControlContainer cc = CreateContainer(action);
            CommandUI cmd = new ParallelUI(cc, action, selLine);
            m_Controller.StartCommand(cmd);
        }

        private bool IsLineUpdateEnabled()
        {
            return false;
        }

        private void LineUpdate(IUserAction action)
        {
            MessageBox.Show(action.Title);
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

        private bool IsLineTrimDanglesEnabled()
        {
            return false;
        }

        private void LineTrimDangles(IUserAction action)
        {
            MessageBox.Show(action.Title);
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

        private bool IsTextAddMiscellaneousTextEnabled()
        {
            return false;
        }

        private void TextAddMiscellaneousText(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsTextAddPolygonLabelsEnabled()
        {
            return false;
        }

        private void TextAddPolygonLabels(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsTextMoveEnabled()
        {
            return false;
        }

        private void TextMove(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsTextDefaultRotationAngleEnabled()
        {
            return false;
        }

        private void TextDefaultRotationAngle(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }
        #endregion

        #region Polygon menu

        private bool IsPolygonDisplayAttributesEnabled()
        {
            return false;
        }

        private void PolygonDisplayAttributes(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsPolygonEditAttributesEnabled()
        {
            return false;
        }

        private void PolygonEditAttributes(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        #endregion

        #region Intersect menu

        private bool IsIntersectTwoDirectionsEnabled()
        {
            return false;
        }

        private void IntersectTwoDirections(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsIntersectTwoDistancesEnabled()
        {
            return false;
        }

        private void IntersectTwoDistances(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsIntersectTwoLinesEnabled()
        {
            return false;
        }

        private void IntersectTwoLines(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsIntersectDirectionAndDistanceEnabled()
        {
            return false;
        }

        private void IntersectDirectionAndDistance(IUserAction action)
        {
            MessageBox.Show(action.Title);
        }

        private bool IsIntersectDirectionAndLineEnabled()
        {
            return false;
        }

        private void IntersectDirectionAndLine(IUserAction action)
        {
            MessageBox.Show(action.Title);
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
            //MessageBox.Show(action.Title);
            CadastralIndex index = (CadastralIndex)CadastralMapModel.Current.Index;
            uint nx = index.GetIntersectCount();
            MessageBox.Show("nx=" + nx);
            //TestForm test = new TestForm();
            //test.ShowDialog();
            //test.Dispose();
        }

        #endregion

        bool HasActiveLayer
        {
            get { return (ActiveLayer!=null); }
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

            GetEntityForm dial = new GetEntityForm(layer, t);
            if (dial.ShowDialog() == DialogResult.OK)
            {
                IEntity e = dial.SelectedEntity;
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
            if (vSplitContainer.Panel2Collapsed)
                return;

            try { propertyGrid.SelectedObject = o; }
            catch { propertyGrid.SelectedObject = null; }
            propertyGrid.Refresh();
        }

        private void mnuPointDefaultEntity_Click(object sender, EventArgs e)
        {
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
        }

        private void mnuHelpTopics_Click(object sender, EventArgs e)
        {
        }
    }
}
