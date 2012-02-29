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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Backsight.Editor.Forms;
using Backsight.Editor.Observations;
using Backsight.Editor.Operations;
using Backsight.Forms;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="30-NOV-1999" was="CuiUpdate"/>
    /// <summary>
    /// User interface for updating editing operations
    /// </summary>
    class UpdateUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The update that is currently being executed.
        /// </summary>
        CommandUI m_Cmd;
        
        /// <summary>
        /// Is <see cref="m_Cmd"/> currently being finished?
        /// </summary>
        bool m_IsFinishing;

        /// <summary>
        /// The feature currently selected for update.
        /// </summary>
        Feature m_Update;

        /// <summary>
        /// The last update that was created on a call to <see cref="AddUpdate"/>.
        /// </summary>
        UpdateOperation m_LastUpdate;

        /// <summary>
        /// Edits that are dependent on the edit that is currently being revised
        /// </summary>
        Operation[] m_DepOps;

        /// <summary>
        /// The operation that rollforward has had problems re-executing.
        /// </summary>
        Operation m_Problem;

        /// <summary>
        /// The editing context at the moment that a rollforward problem was detected. This context
        /// contains information about positional changes that have already been made to support the
        /// last update. When the user has attempted corrective action, these changes must be undone
        /// before the update is applied again.
        /// </summary>
        UpdateEditingContext m_ProblemContext;

        /// <summary>
        /// Info about the current feature that's selected for update.
        /// </summary>
        UpdateForm m_Info;

        /// <summary>
        /// The ID of the last edit prior to start of updates (or the ID of the session
        /// itself if there were no prior edits in the current working session).
        /// </summary>
        uint m_PreUpdateId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUI"/> class.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        public UpdateUI(IUserAction action)
            : base(action)
        {
            m_Update = null;
            m_Info = null;
            m_Cmd = null;
            m_IsFinishing = false;
            m_DepOps = null;
            m_Problem = null;

            Session s = CadastralMapModel.Current.WorkingSession;
            m_PreUpdateId = (s.LastOperation == null ? s.Id : s.LastOperation.EditSequence);
        }

        #endregion

        public override void Dispose()
        {
            // If a command is running(!), abort it now.
            if (m_Cmd != null)
            {
                m_Cmd.DialAbort(null);
                m_Cmd = null;
            }

            // Get rid of the dialog that shows info about the current update feature.
            if (m_Info != null)
            {
                m_Info.Dispose();
                m_Info = null;
            }

            base.Dispose();
        }

        /// <summary>
        /// Updates a specific feature.
        /// </summary>
        /// <param name="update">The feature selected for update.</param>
        /// <returns>True if feature accepted for update. False if an update is already in progress.</returns>
        internal bool Run(Feature update)
        {
            // Return if we're currently updating something, or we've
            // hit a problem during rollforward.
            if (m_Cmd != null || m_Problem != null)
                return false;

            // If we prevously had something selected for update,
            // undo any drawing that we did for it.
            ErasePainting();

            // Remember the specified feature.
            m_Update = update;

            // If the info dialog has not already been displayed, display it now.
            if (m_Info == null)
            {
		        m_Info = new UpdateForm(this);
                m_Info.Show();
            }

            // Get the info window to display stuff about the selected feature.
            m_Info.Display(m_Update);

            // Leave keyboard focus with the info dialog.
            m_Info.Focus();

            // Draw stuff.
            Draw();
            ActiveDisplay.PaintNow();
            return true;
        }

        /// <summary>
        /// Displays info about the edits that are dependent on the feature
        /// currently selected for update.
        /// </summary>
        internal void Dependencies()
        {
            // Get the operation that created the update feature.
            Operation pop = GetOp();
            if (pop == null)
                return;

            // Get the edits that depend on features created by the update op
            Operation[] deps = pop.MapModel.Touch(pop);

            // Draw the features that were created by the dependent edits
            ISpatialDisplay display = ActiveDisplay;
            IDrawStyle style = Controller.Style(Color.Magenta);
            style.IsFixed = true;
            foreach (Operation d in deps)
                d.Render(display, style, true);

            // List the dependent operations.
            using (ListOperationsForm dial = new ListOperationsForm(deps))
            {
                dial.ShowDialog();
            }

            // Redraw the map the normal way (with the current update op in magenta)
            ErasePainting();
        }

        /// <summary>
        /// Returns the first predecessor (if any) for a specific feature.
        /// </summary>
        /// <param name="feat"></param>
        /// <returns>The predecessor (must be a line). Null if none.</returns>
        internal static LineFeature GetPredecessor(Feature feat)
        {
            LineFeature line = (feat as LineFeature);
            if (line == null)
                return null;
            else
                return line.GetPredecessor();
        }

        /// <summary>
        /// Displays info about any previous incarnations of the feature
        /// currently selected for update.
        /// </summary>
        internal void Predecessors()
        {
            // Return if there is no predecessor.
            LineFeature prevLine = GetPredecessor(m_Update);
            if (prevLine == null)
                return;

            // Get a list of all the predecessor lines
            List<LineFeature> prev = new List<LineFeature>();

            while (prevLine != null)
            {
                prev.Add(prevLine);
                prevLine = prevLine.GetPredecessor();
            }

            // Display list of the edits
            using (PickPredecessorForm dial = new PickPredecessorForm(prev.ToArray(), true))
            {
                if (dial.ShowDialog() == DialogResult.OK)
                    Run(dial.SelectedLine);
            }
        }

        /// <summary>
        /// Cancels all updates.
        /// </summary>
        internal void Cancel()
        {
            // No can do if an update is currently in progress!
            if (m_Cmd != null)
            {
                string msg = "You are in the middle of making an update." + System.Environment.NewLine;
                msg += "You must cancel that first.";
                MessageBox.Show(msg);
                return;
            }

            // If we've currently got a problem, treat the cancellation
            // request as a request to undo just the last update.
            if (m_Problem != null)
            {
                Undo();
                m_Problem = null;
                if (m_Update != null)
                    Run(m_Update);

                m_Info.OnFinishUpdate(null);
                ErasePainting();
            }
            else
            {
                // If we set any undo points, undo them all.
                UndoAll();

                FinishCommand();
            }
        }

        /// <summary>
        /// Shuts down the update interface, accepting any updates that have been completed.
        /// </summary>
        internal void AcceptAllUpdates()
        {
            if (m_Cmd != null)
            {
                string msg = "You are in the middle of making an update." + System.Environment.NewLine;
                msg += "You must finish that first.";
                MessageBox.Show(msg);
                return;
            }

            if (m_Problem != null)
            {
                MessageBox.Show("An unresolved problem needs to be rectified before you can accept changes.");
                return;
            }

            FinishCommand();
        }

        /// <summary>
        /// Invokes the update dialog for the selected feature.
        /// </summary>
        internal void StartUpdate()
        {
            // Nothing to do if update feature is (somehow) undefined.
            if (m_Update == null)
                return;

            // Return if already updating something.
            if (m_Cmd != null)
                return;

            // Get the operation that created the feature and re-run that operation.
            Operation pop = GetOp();
            Debug.Assert(pop != null);

            // Get a list of the dependent operations.
            m_DepOps = pop.MapModel.Touch(pop);

            // Erase anything we've drawn (the command should be able
            // to draw stuff in it's own way, and that might conflict
            // with what this object does).
            ErasePainting();

            // Run the update command. If it's left running, declare
            // a save point (so long as we're not responding to a problem
            // that has arisen during rollforward).
            if (RunUpdate())
            {
                if (m_Problem == null)
                    SetUndoMarker();
            }
        }

        /// <summary>
        /// Do any drawing that is specific to the current update.
        /// </summary>
        void Draw()
        {
            // Get the operation associated with the update feature (or
            // the current problem op)
            Operation op = GetOp();
            if (op==null)
                return;

            // Draw the features that were created by the operation that created the feature
            // selected for update.
            ISpatialDisplay display = ActiveDisplay;
            IDrawStyle style = Controller.Style(Color.Magenta);
            style.IsFixed = true;
            op.Render(display, style, true);

            // If the update feature is a line, ensure that it is highlighted the normal
            // way (so that the direction of the line is apparent).
            if (m_Update is LineFeature)
                m_Update.Render(display, Controller.HighlightStyle);
        }

        /// <summary>
        /// Runs the update for the current update feature.
        /// </summary>
        /// <returns>True if an update command dialog has been started.</returns>
        bool RunUpdate()
        {
            // Get the operation that created the feature selected for update.
            Operation pop = GetOp();
            if (pop == null)
                return false;

            // There shouldn't already be a command running.
            if (m_Cmd != null)
            {
                MessageBox.Show("UpdateUI.RunUpdate - Update already running?");
                return false;
            }

            // The IControlContainer is a bit of a dodo.
            IControlContainer cc = new ContainerForm("Update");

            switch (pop.EditId)
            {
                case EditingActionId.LineExtend:
                {
                    m_Cmd = new LineExtensionUI(cc, pop.EditId, this);
                    break;
                }

                case EditingActionId.LineSubdivision:
                {
                    m_Cmd = new LineSubdivisionUI(cc, pop.EditId, this);
                    break;
                }

                // SS20101011 - In the past, you were allowed to change the lines that were intersected
                // as part of Direction-Line and Line-Line intersects. This would not be a problem if
                // the lines were left un-split. However, if you split the lines, it is possible that
                // subsequent edits would refer to the resultant sections. So if you later refer to a
                // different line, those edits would become invalid. That's why the ability to change
                // Line-Line intersects has been removed. The Direction-Line option is still valid,
                // because you are allowed to change the direction.

                // In the future, it would be better to modify the Direction-Line and Line-Line edits
                // to prohibit splits. That would probably be better handled by a new edit that would
                // let users split a line at an intersection (although I haven't thought that through -
                // it's possible that such an edit would also be subject to similar issues).

                case EditingActionId.DirIntersect:
                case EditingActionId.DirDistIntersect:
                case EditingActionId.DirLineIntersect:
                case EditingActionId.DistIntersect:
                //case EditingActionId.LineIntersect:
                {
                    m_Cmd = new IntersectUI(pop.EditId, this);
                    break;
                }

                case EditingActionId.NewPoint:
                case EditingActionId.GetControl:
                {
                    m_Cmd = new NewPointUI(cc, pop.EditId, this);
                    break;

                }

                case EditingActionId.NewCircle:
                {
                    m_Cmd = new NewCircleUI(cc, pop.EditId, this);
                    break;
                }

                case EditingActionId.Path:
                {
                    m_Cmd = new PathUI(cc, pop.EditId, this);
                    break;
                }

                case EditingActionId.Parallel:
                {
                    m_Cmd = new ParallelLineUI(cc, pop.EditId, this);
                    break;
                }

                case EditingActionId.Radial:
                {
                    m_Cmd = new RadialUI(cc, pop.EditId, this);
                    break;
                }

                case EditingActionId.SimpleLineSubdivision:
                {
                    m_Cmd = new SimpleLineSubdivisionUI(cc, pop.EditId, this);
                    break;
                }
            }

            if (m_Cmd != null)
            {
                m_Cmd.Run();
                return true;
            }

            MessageBox.Show("You cannot update the selected feature this way.");
            return false;
        }

        /// <summary>
        /// Returns the operation that created the feature that is currently selected for update.
        /// </summary>
        /// <returns>The operation (could conceivably be null).</returns>
        internal Operation GetOp()
        {
            if (m_Problem != null)
                return m_Problem;

            if (m_Update == null)
                return null;

            return (Operation)m_Update.Creator;
        }

        /// <summary>
        /// Handles completion of an editing command.
        /// </summary>
        /// <param name="cmd">The command that is finishing.</param>
        /// <returns>True if the command is one that was created by this update object.</returns>
        internal bool FinishCommand(CommandUI cmd)
        {
	        // Return if we somehow don't have an operation (this refers to the original editing operation)
	        Operation pop = GetOp();
	        if (pop==null)
                return false;

            // Grab the description of the changes
            UpdateOperation rev = m_LastUpdate;
            if (rev == null)
                return false;

        	// Delete the command.
	        if (!DeleteCommand(cmd))
                return false;

	        // Was the command run to fix a problem?
	        bool wasProblem = (m_Problem!=null);
	        m_Problem = null;

	        // If so, re-display the info for the originally selected op.
	        if ( wasProblem && m_Update!=null )
                Run(m_Update);

	        // Propagate the change (breaking if an operation can no
	        // longer be calculated, which assigns m_Problem)
	        //Rollforward(pop);
            ApplyUpdate(rev);

	        // Ensure info window is shown
	        m_Info.OnFinishUpdate(m_Problem);

	        // Force a redraw.
            this.Controller.RefreshAllDisplays();
	        return true;
        }

        void ApplyUpdate(UpdateOperation uop)
        {
            UpdateEditingContext uec = new UpdateEditingContext(uop);

            try
            {
                // Remember the original values (we'll need to restore them before serializing)
                UpdateItemCollection originalItems = new UpdateItemCollection(uop.Changes);

                // Apply changes, then rework the map model to account for the update
                uop.ApplyChanges();
                uec.Recalculate();

                // Update topology and save the update
                uop.MapModel.CleanEdit();

                // Temporarily restore the original change items so that we serialize
                // the correct data (as things stand, the modified values have already
                // been applied to the edit)
                UpdateItemCollection revisedItems = uop.Changes;
                uop.Changes = originalItems;
                uop.Session.SaveOperation(uop);
                uop.Changes = revisedItems;
            }

            catch (RollforwardException rex)
            {
                // Remember the problem edit, as well as the update context (since we may need to
                // move stuff back to their original positions).
                m_Problem = rex.Problem;
                m_ProblemContext = uec;

                /*
                // The spatial index may be missing stuff, so rework it entirely!
                CadastralMapModel model = uop.MapModel;
                Operation[] allEdits = model.GetAllEdits();
                uop.MapModel.CreateIndex(allEdits);
                */

                // Update topology and cleanup any junk
                //model.CleanEdit();

                // Re-center on the problem edit if it's off-screen
                ISpatialDisplay display = base.ActiveDisplay;
                IPosition newCenter = m_Problem.GetRecenter(display.Extent);
                if (newCenter != null)
                    display.Center = newCenter;

                MessageBox.Show("Cannot re-work geometry due to edit " + m_Problem.EditSequence, "Problem",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
                uec.RevertChanges();
                uop.ApplyChanges();
            }
        }

        /// <summary>
        /// Propogates changes, starting at the specified operation. If any problem arises,
        /// the draw window will be recentred on the problem area.
        /// </summary>
        /// <param name="op">The operation to start with</param>
        /// <returns>Any operation that caused a problem during rollfoward (this is also
        /// assigned to <c>m_Problem</c>)</returns>
        Operation Rollforward(Operation op)
        {
            throw new NotImplementedException("UpdateUI.Rollforward");
            /*
	        // Propagate the change (breaking if an operation can no longer be calculated).
            CadastralMapModel map = CadastralMapModel.Current;
	        m_Problem = map.Rollforward(op);

	        // Cleanup the map (if there's a problem, don't fix up the
	        // topology, since things may be screwed up royally).
	        if (m_Problem!=null)
		        map.MaintainTopology = false;
	        else
		        map.FinishRollforward();

	        // Clean up the map.
	        map.CleanEdit();

	        // If we switched off topology, switch it on again now just
	        // so we don't forget later. Also ensure that the problem
	        // is on screen.
	        if (m_Problem!=null)
            {
		        map.MaintainTopology = true;

		        // Get the view's current draw window
                ISpatialDisplay display = EditingController.Current.ActiveDisplay;
                IWindow drawin = display.Extent;

		        // If the problem isn't on screen, re-centre at the current scale
                IPosition center;
		        if (m_Problem.GetReCentre(drawin,center))
		        {
                    double scale = display.MapScale;
                    display.SetDraw(center,scale);
		        }
	        }

	        return m_Problem;
             */
        }

        /// <summary>
        /// Handles cancellation of an editing command.
        /// </summary>
        /// <param name="cmd">The command that is being cancelled.</param>
        /// <returns>True if the command is one that was created by this update object.</returns>
        internal bool AbortCommand(CommandUI cmd)
        {
	        // Return if the command wasn't started by this update.
	        if (!Object.ReferenceEquals(cmd, m_Cmd))
                return false;

	        // Delete the command.
            m_Cmd.Dispose();
            m_Cmd = null;

	        // Revert to the save point that was set when we started the command.

	        // SS 27-MAR-2003 -- In the case of a command that was
	        // invoked to fix a problem, DON'T undo. That has to
	        // wait until the user clicks the Undo button that's
	        // part of the m_Info dialog.

	        if (m_Problem==null)
        		Undo();

	        // Paint in our own way.
	        Draw();

	        // Ensure info window is shown
	        m_Info.OnAbortUpdate();
	        return true;
        }

        /// <summary>
        /// Runs this instance (implements the abstract method declared by the base class, but
        /// does nothing).
        /// </summary>
        /// <returns>False (always). To start an update, use <see cref="Run(Feature)"/></returns>
        internal override bool Run()
        {
            return false;
        }

        /// <summary>
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <see cref="Paint"/> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return true; }
        }

        /// <summary>
        /// Does any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn. Not used.</param>
        internal override void Paint(PointFeature point)
        {
            if (m_Cmd == null)
                Draw();
            else
                m_Cmd.Paint(point);
        }

        /// <summary>
        /// Handles mouse-move.
        /// </summary>
        /// <param name="p">The new position of the mouse</param>
        internal override void MouseMove(IPosition p)
        {
            if (m_Cmd != null)
                m_Cmd.MouseMove(p);
        }

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="p">The position where the click occurred</param>
        /// <returns>True if the command handled the mouse down. False if it did nothing.</returns>
        internal override bool LButtonDown(IPosition p)
        {
            if (m_Cmd == null)
                return false;
            else
                return m_Cmd.LButtonDown(p);
        }

        /// <summary>
        /// Handles left-up mouse click.
        /// </summary>
        /// <param name="p">The position where the left-up occurred.</param>
        internal override void LButtonUp(IPosition p)
        {
            if (m_Cmd != null)
                m_Cmd.LButtonUp(p);
        }

        /// <summary>
        /// Handles double-click.
        /// </summary>
        /// <param name="p">The position where the double-click occurred.</param>
        internal override void LButtonDblClick(IPosition p)
        {
            if (m_Cmd != null)
                m_Cmd.LButtonDblClick(p);
        }

        /// <summary>
        /// Creates any applicable context menu
        /// </summary>
        /// <returns>The context menu (null if the command does not utilize a context menu).</returns>
        internal override ContextMenuStrip CreateContextMenu()
        {
            if (m_Cmd == null)
                return null;
            else
                return m_Cmd.CreateContextMenu();
        }

        /// <summary>
        /// Reacts to selection of the Cancel button in the dialog. This will forward to
        /// the current update command if one is running (otherwise it does nothing).
        /// </summary>
        /// <param name="wnd">The currently active control (not used)</param>
        internal override void DialAbort(Control wnd)
        {
            if (m_Cmd != null)
            {
                m_Cmd.DialAbort(wnd);
                m_Cmd = null;
            }
        }

        /// <summary>
        /// Reacts to selection of the OK button in the dialog. This will forward to
        /// the current update command if one is running (otherwise it does nothing).
        /// </summary>
        /// <param name="wnd">The dialog window</param>
        /// <returns>True if an update command was in progress and it finished ok.</returns>
        internal override bool DialFinish(Control wnd)
        {
            if (m_Cmd == null)
                return false;
            else
            {
                try
                {
                    m_IsFinishing = true; // see comment in OnSelectPoint
                    return m_Cmd.DialFinish(wnd);
                }

                finally
                {
                    m_IsFinishing = false;
                }
            }
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            // In some situations (e.g. Direction/Distance intersection), the
            // act of finishing a command may mean that the editing controller
            // will be asked to select something (e.g. the intersection point).
            // In that case, we don't want to do anything when the select point
            // message comes through.

            if (m_IsFinishing)
                return;

            if (m_Cmd != null)
            {
                // Can't pick something created by a dependent op.
                if (IsDependent(point))
                    DependencyMessage();
                else
                    m_Cmd.OnSelectPoint(point);
            }
            else if (point != null)
                Run(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            if (m_Cmd != null)
            {
                // Can't pick something created by a dependent op.
                if (IsDependent(line))
                    DependencyMessage();
                else
                    m_Cmd.OnSelectLine(line);
            }
            else if (line != null)
                Run(line);
        }

        /// <summary>
        /// Checks if a feature that the user is trying to select is dependent on the
        /// edit that is currently being updated.
        /// </summary>
        /// <param name="feat">The feature the user is trying to use.</param>
        /// <returns>True if it's a dependent feature.</returns>
        internal bool IsDependent(Feature feat)
        {
	        // No problem if nothing specified!
	        if (feat==null)
                return false;

	        // The dependencies have to be defined!
	        if (m_DepOps==null)
                return false;

	        // No dependency if the feature has a primitive that's a
	        // multi-segment (since the position of multi-segments is
	        // expected to be immutable). This is perhaps a bit of a
	        // hack that is meant to get around problems that may
	        // arise when dealing with a complex series of intersections
	        // with river lines. If this solution proves to be unacceptable,
	        // the place to introduce an alternative is in the
	        // CuiIntersect::OnUpdateSelect method (which is currently
	        // only a placeholder).
            /*
	            const CePrimitive* const pPrim = pFeat->GetPrimitive();
	            if ( pPrim->GetType() == PTY_MULTI ) return FALSE;
            */

	        // Get the operation that created the feature.
	        Operation op = feat.Creator;
            return (Array.IndexOf(m_DepOps, op) >= 0);
        }

        /// <summary>
        /// Displays message that indicates an attempt to make use of a dependent feature.
        /// </summary>
        internal void DependencyMessage()
        {
            string msg = String.Format("{0}{1}{2}", 
                            "You are trying to use a feature with a position", System.Environment.NewLine,
		                    "that is dependent on the edit you are changing.");
            MessageBox.Show(msg, "Dependent feature", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Ensures the command cursor (if any) is shown.
        /// </summary>
        internal override void SetCommandCursor()
        {
            if (m_Cmd != null)
                m_Cmd.SetCommandCursor();
        }

        /// <summary>
        /// The object currently selected for update
        /// </summary>
        /// <returns></returns>
        internal Feature GetUpdate()
        {
            return m_Update;
        }

        /// <summary>
        /// Accepts a new offset
        /// </summary>
        /// <param name="offset"></param>
        internal override void SetOffset(Offset offset)
        {
            if (m_Cmd != null)
                m_Cmd.SetOffset(offset);
        }

        /// <summary>
        /// Deletes any command dialog previously created to make an update.
        /// </summary>
        /// <param name="cmd">The command to get rid of</param>
        /// <returns>True if the supplied command matches the one previously created
        /// by this update object (in that case, m_Cmd gets disposed and nulled out).
        /// False if the supplied command is anything else.</returns>
        bool DeleteCommand(CommandUI cmd)
        {
	        // Return if the command wasn't started by this update.
	        if (!Object.ReferenceEquals(cmd, m_Cmd))
                return false;

	        // Get rid of the command.
            if (m_Cmd!=null)
            {
                m_Cmd.Dispose();
                m_Cmd = null;
            }

	        return true;
        }

        /// <summary>
        /// Rolls back the last update.
        /// </summary>
        void Undo()
        {
            if (CanUndo())
            {
                CadastralMapModel.Current.UndoLastEdit();

                if (m_Info != null)
                    m_Info.SetUpdateCount(GetUpdateCount());
            }
        }

        /// <summary>
        /// Obtains the number of update operations that have been performed since the user started
        /// performing updates.
        /// </summary>
        /// <returns>The number of update operations that have been added to the working
        /// session since the update dialog was presented.</returns>
        uint GetUpdateCount()
        {
            Session s = CadastralMapModel.Current.WorkingSession;
            Operation[] edits = s.Edits;
            uint result = 0;

            foreach (Operation op in edits)
            {
                if (op.EditSequence > m_PreUpdateId && op is UpdateOperation)
                    result++;
            }

            return result;
        }

        /// <summary>
        /// Have any edits been performed since the user started performing updates?
        /// </summary>
        /// <returns></returns>
        bool CanUndo()
        {
            Session s = CadastralMapModel.Current.WorkingSession;
            Operation op = s.LastOperation;
            if (op == null)
                return false;
            else
                return (op.EditSequence > m_PreUpdateId);
        }

        /// <summary>
        /// Rolls back all undo points created by this command, and updates dialog to reflect this.
        /// </summary>
        void UndoAll()
        {
            while (CanUndo())
            {
                CadastralMapModel.Current.UndoLastEdit();
            }

            if (m_Info != null)
                m_Info.SetUpdateCount(GetUpdateCount());
        }

        /// <summary>
        /// Sets an undo marker and and updates dialog to reflect this.
        /// </summary>
        void SetUndoMarker()
        {
            //throw new NotImplementedException("UpdateUI.SetUndoMarker");
            //m_Context.SetUndoMarker();

            //if (m_Info != null)
            //    m_Info.SetUpdateCount(m_Context.NumUndoMarkers);
        }

        internal CommandUI ActiveCommand
        {
            get { return m_Cmd; }
        }

        /// <summary>
        /// Remembers details for an updated edit.
        /// </summary>
        /// <param name="revisedEdit">The edit that is being revised</param>
        /// <param name="changes">The changes to apply</param>
        /// <returns>True if an update was recorded, false if the supplied change collection is empty (in that
        /// case, the user receives a warning message).</returns>
        /// <remarks>This will be called when the user has finished making changes to an old
        /// edit. The call comes from the UI for the revised edit, which goes on to call
        /// CommandUI.FinishCommand, which routes back to UpdateUI.FinishCommand when an
        /// update UI is in progress.</remarks>
        internal bool AddUpdate(Operation revisedEdit, UpdateItemCollection changes)
        {
            if (changes.Count == 0)
            {
                MessageBox.Show("You do not appear to have made any changes.");
                return false;
            }

            m_LastUpdate = new UpdateOperation(revisedEdit, changes);

            //if (m_Info != null)
            //    m_Info.SetUpdateCount(GetUpdateCount());

            return true;
        }
    }
}
