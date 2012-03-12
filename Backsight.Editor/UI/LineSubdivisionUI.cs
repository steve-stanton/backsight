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
using System.Windows.Forms;

using Backsight.Forms;
using Backsight.Editor.Forms;
using Backsight.Editor.Operations;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="10-DEC-1999" was="CuiArcSubdivision" />
    /// <summary>
    /// User interface for subdividing a line (using multiple distances).
    /// </summary>
    class LineSubdivisionUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The line that's being subdivided.
        /// </summary>
        LineFeature m_Parent;

        /// <summary>
        /// The dialog for the command.
        /// </summary>
        LineSubdivisionControl m_Dialog;

        /// <summary>
        /// The dialog when doing an update.
        /// </summary>
        LineSubdivisionUpdateForm m_UpDial;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a new line subdivision.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <exception cref="InvalidOperationException">If a specific line is not currently selected</exception>
        internal LineSubdivisionUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            LineFeature selLine = EditingController.SelectedLine;
            if (selLine == null)
                throw new InvalidOperationException("You must initially select the line you want to subdivide.");

            m_Parent = selLine;
        }

        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="editId">The ID of the edit this command deals with.</param>
        /// <param name="updcmd">The update command.</param>
        internal LineSubdivisionUI(IControlContainer cc, EditingActionId editId, UpdateUI updcmd)
            : base(cc, editId, updcmd)
        {
            // The dialog will be created by Run()
            m_Dialog = null;
            m_UpDial = null;

            // The line being subdivided is known via the update.
            m_Parent = null;
        }
    
        #endregion

        /// <summary>
        /// Starts the user interface for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
	        // Don't run more than once.
	        if (m_Dialog!=null || m_UpDial!=null)
                throw new Exception("LineSubdivisionUI.Run - Command is already running.");

	        // Are we doing an update?
        	UpdateUI pup = this.Update;

        	// Create modeless dialog.
            if (pup != null)
            {
                m_UpDial = new LineSubdivisionUpdateForm(pup);
                m_UpDial.Show();
	        }
	        else
            {
                Debug.Assert(m_Parent!=null);
		        m_Dialog = new LineSubdivisionControl(this, m_Parent, this.Recall);
                this.Container.Display(m_Dialog);
            }

        	return true;
        }

        /// <summary>
        /// Does any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn. Not used.</param>
        internal override void Paint(PointFeature point)
        {
            Draw();
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
        /// Draws the current state of the edit
        /// </summary>
        internal void Draw()
        {
            if (m_Dialog != null)
                m_Dialog.Draw();
            else if (m_UpDial != null)
                m_UpDial.Draw();
        }

        internal override void DialAbort(Control wnd)
        {
            KillDialogs();
            AbortCommand();
        }

        /// <summary>
        /// Destroys any dialogs that are currently displayed.
        /// </summary>
        void KillDialogs()
        {
            this.Container.Clear();

            if (m_Dialog!=null)
            {
                m_Dialog.Dispose();
                m_Dialog = null;
            }

            if (m_UpDial != null)
            {
                m_UpDial.Close();
                m_UpDial.Dispose();
                m_UpDial = null;
            }
        }

        internal override bool DialFinish(Control wnd)
        {
            if (m_Dialog==null && m_UpDial==null)
                throw new Exception("LineSubdivisionUI.DialFinish - No dialog!");

            UpdateUI up = this.Update;
            bool doAbort = false;

            if (up!=null)
            {
                // Get the original operation.
                LineSubdivisionOperation pop = (up.GetOp() as LineSubdivisionOperation);
                if (pop == null)
                {
                    MessageBox.Show("LineSubdivisionUI.DialFinish - Unexpected edit type.");
                    return false;
                }

                // Ensure we've got the edit for the primary face
                if (!pop.IsPrimaryFace)
                {
                    pop = pop.OtherSide;
                    Debug.Assert(pop != null);
                    Debug.Assert(pop.IsPrimaryFace);
                }

                // Package up any changes to the primary face
                UpdateItemCollection changes = pop.GetUpdateItems(m_UpDial.WorkingFace1);
                if (changes.Count > 0)
                    up.AddUpdate(pop, changes);

                // If a second face has just been defined, append a new edit.
                LineSubdivisionFace face2 = m_UpDial.WorkingFace2;
                if (face2 != null)
                {
                    if (pop.OtherSide == null)
                        AddOtherSide(pop, face2);
                    else
                    {
                        changes = pop.OtherSide.GetUpdateItems(face2);
                        if (changes.Count > 0)
                            up.AddUpdate(pop.OtherSide, changes);
                    }
                }

                // If all we've done is add a new face, UpdateUI.AddUpdate wouldn't have been
                // called, which will later lead to an exception from UpdateUI.FinishCommand (via
                // the FinishCommand done below). So remember to abort in that case.
                doAbort = (up.HasRevisions == false);
            }
            else
            {
                // Ensure that the parent line has been de-selected by the view.
                Controller.ClearSelection();

                // Get the dialog to save.
                m_Dialog.Save();
            }

            // Destroy the dialog(s).
            KillDialogs();

        	// Get the base class to finish up.
            if (doAbort)
                return AbortCommand();
            else
	            return FinishCommand();
        }

        /// <summary>
        /// Saves a new edit that defines an alternate face for a previously subdivided line.
        /// </summary>
        /// <param name="firstSide">The edit that initially subdivided the line.</param>
        /// <param name="altFace">The definition of the alternate face</param>
        void AddOtherSide(LineSubdivisionOperation firstSide, LineSubdivisionFace altFace)
        {
            try
            {
                LineSubdivisionOperation op = new LineSubdivisionOperation(firstSide.Parent, altFace.ObservedLengths, altFace.IsEntryFromEnd);
                op.OtherSide = firstSide;
                op.Execute();
                firstSide.OtherSide = op;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
        }
    }
}
