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
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

using Backsight.Editor.Operations;
using Backsight.Forms;
using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    class UpdateUI : SimpleCommandUI
    {
        /// <summary>
        /// The update that is currently being executed.
        /// </summary>
        CommandUI m_Cmd;

        /// <summary>
        /// The feature currently selected for update.
        /// </summary>
        Feature m_Update;

        /// <summary>
        /// The number of undo markers that have been set.
        /// </summary>
        uint m_NumUndo;

        /// <summary>
        /// Dependent operations.
        /// </summary>
        List<Operation> m_DepOps;

        /// <summary>
        /// The operation that rollforward has had problems re-executing.
        /// </summary>
        Operation m_Problem;

        /// <summary>
        /// Info about the current feature that's selected for update.
        /// </summary>
        UpdateForm m_Info;
        /*
	CeView&			m_View;		// The view that's running the show. ... to use the controller instead
         */

        public UpdateUI(IControlContainer cc, IUserAction cmdId) : base(cc, cmdId)
        {
//            base.Update = this;
        }

        /// <summary>
        /// Accepts a new offset
        /// </summary>
        /// <param name="offset"></param>
        internal override void  SetOffset(Offset offset)
        {
            if (m_Cmd!=null)
                m_Cmd.SetOffset(offset);
        }

        /// <summary>
        /// Handles completion of an editing command.
        /// </summary>
        /// <param name="cmd">The command that is finishing.</param>
        /// <returns>True if the command is one that was created by this update object.</returns>
        internal bool FinishCommand(CommandUI cmd)
        {
	        // Return if we somehow don't have an operation.
	        Operation pop = GetOp();
	        if (pop==null)
                return false;

	        // Were we just updating the absolute position of a point?
	        // If so, grab the new position before destroying the
	        // command (we need to do this because CeNewPoint::Rollforward
	        // cannot calculate a new absolute position).

            NewPointUI newPoint = (cmd as NewPointUI);
            PointFeature point = null;
            IPosition newpos = null;
	        if (newPoint!=null)
            {
                point = m_Update as PointFeature;
                Debug.Assert(point!=null);
                newpos = newPoint.Position;
	        }

        	// Delete the command.
	        if (!DeleteCommand(cmd))
                return false;

	        // Was the command run to fix a problem?
	        bool wasProblem = (m_Problem!=null);
	        m_Problem = null;

	        // If so, re-display the info for the originally selected op.
	        if ( wasProblem && m_Update!=null )
                Run(m_Update);

	        // If we're not responding to a rollforward problem, tell
	        // the map that rollforward is about to begin.
            CadastralMapModel map = CadastralMapModel.Current;
	        if (!wasProblem)
                map.StartRollforward();

	        // If a new absolute position has been defined for a point,
	        // just move it. Otherwise mark the modified operation as
	        // changed, so that rollforward will re-calculate stuff.

        	if (newpos!=null)
		        point.Move(newpos);
	        else
		        pop.OnMove(null);

	        // Propagate the change (breaking if an operation can no
	        // longer be calculated, which assigns m_Problem)
	        Rollforward(pop);

	        // Ensure info window is shown
	        m_Info.OnFinishUpdate(m_Problem);

	        // Force a redraw.
            this.Controller.RefreshAllDisplays();
	        return true;
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
	        return m_DepOps.Contains(op);
        }

        /// <summary>
        /// Displays message that indicates an attempt to make use of a dependent feature.
        /// </summary>
        internal void DependencyMessage()
        {
            string msg = String.Format("{0}{1}{2}", 
                            "You are trying to use a feature with a position", System.Environment.NewLine,
		                    "that is dependent on the edit you are changing.");
            MessageBox.Show(msg);
        }

        /// <summary>
        /// Returns the operation that created the feature that is currently selected for update.
        /// </summary>
        /// <returns>The operation (could conceivably be null).</returns>
        internal Operation GetOp()
        {
	        if (m_Problem!=null)
                return m_Problem;

	        if (m_Update==null)
                return null;

	        return (Operation)m_Update.Creator;
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

        void Draw()
        {
            Draw(true);
        }

        /// <summary>
        /// Do any drawing that is specific to the current update.
        /// </summary>
        /// <param name="isDraw">True to draw. False to redraw normal.</param>
        void Draw(bool isDraw)
        {
            /*
	        // If we've got an update command running, let it do it's
	        // own drawing (ignore the un-draw option).
	        if (m_Cmd!=null)
            {
		        m_Cmd.Paint();
		        return;
	        }

	        // Get the operation associated with the update feature (or
	        // the current problem op)
	        IOperation pop = GetOp();
	        if (pop==null)
                return;

	        // If not actually updating as yet, draw the features that
	        // were created by the operation that created the feature
	        // selected for update.

        	if (isDraw)
            {
		        // Get the operation to draw everything.
		        pop.Draw(Color.Magenta);

		        // If the update feature is a line, ensure that it
		        // is highlighted the normal way (so that the direction
		        // of the line is apparent).
                if (m_Update is LineFeature)
                    (m_Update as LineFeature).Highlight();
        	}
	        else
            {
		        // Ensure update line is not highlighted.
                if (m_Update is LineFeature)
                    (m_Update as LineFeature).UnHighlight();

		        // Get the operation to erase everything, and then
		        // redraw the stuff that's active.
		        pop.Draw();
        	}
             */
        }

        /// <summary>
        /// Updates a specific feature.
        /// </summary>
        /// <param name="update">The feature selected for update.</param>
        /// <returns>True if feature accepted for update. False if an update is already in progress.</returns>
        bool Run(Feature update)
        {
	        // Return if we're currently updating something, or we've
	        // hit a problem during rollforward.
	        if (m_Cmd!=null || m_Problem!=null)
                return false;

	        // If we prevously had something selected for update,
	        // undo any drawing that we did for it.
	        Erase();

	        // Remember the specified feature.
	        m_Update = update;

	        // If the info dialog has not already been displayed, display it now.
	        if (m_Info==null)
            {
                throw new NotImplementedException("UpdateUI.Run");
                /*
		        m_Info = new UpdateForm(this,m_View);
		        m_Info.Create(IDD_UPDATE,&m_View);
                 */
	        }

	        // Get the info window to display stuff about the
	        // selected feature.
	        m_Info.Display(m_Update);

	        // Leave keyboard focus with the info dialog.
	        m_Info.Focus();

	        // Draw stuff.
	        Draw();
	        return true;
        }

        internal override bool Run()
        {
            throw new Exception("The method or operation is not implemented.");
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
        /// Rolls back to the last undo point, and updates dialog to reflect this.
        /// </summary>
        void Undo()
        {
            throw new NotImplementedException("UpdateUI.Undo");

            /*
	        CadastralMapModel.Current.Undo();
	        if (m_NumUndo>0)
                m_NumUndo--;

	        if (m_Info!=null)
                m_Info.SetUpdateCount(m_NumUndo);
             */
        }

        /// <summary>
        /// Erases any drawing we might have done.
        /// </summary>
        void Erase()
        {
            Draw(false);
        }

        internal CommandUI ActiveCommand
        {
            get { return m_Cmd; }
        }
    }
}
