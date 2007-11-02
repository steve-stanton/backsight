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
using System.Diagnostics;
using System.Windows.Forms;

using Backsight.Forms;
using Backsight.Editor.Forms;

namespace Backsight.Editor
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
        object m_UpDial; //CdUpdateSub

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a new line subdivision.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="parent">The line to subdivide (not null).</param>
        internal LineSubdivisionUI(IControlContainer cc, IUserAction action, LineFeature parent)
            : base(cc, action)
        {
            m_Parent = parent;
        }

        /// <summary>
        /// Constructor for command recall.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="parent">The line to subdivide (not null).</param>
        /// <param name="op">The operation that's being recalled.</param>
        internal LineSubdivisionUI(IControlContainer cc, IUserAction action, LineFeature parent, Operation op)
            : base(cc, action, null, op)
        {
            // The dialog will be created by Run()
            m_Dialog = null;
            m_UpDial = null;

            // Remember the arc that's being subdivided.
	        m_Parent = parent;
        }

        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="updcmd">The update command.</param>
        internal LineSubdivisionUI(IControlContainer cc, IUserAction action, UpdateUI updcmd)
            : base(cc, action, updcmd)
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
            /*
	        if (pup!=null)
            {
		        m_pUpDial = new CdUpdateSub(*pup);
		        m_pUpDial->Create(CdUpdateSub::IDD);
	        }
	        else
            {
             */
                Debug.Assert(m_Parent!=null);
		        m_Dialog = new LineSubdivisionControl(this, m_Parent, this.Recall);
                this.Container.Display(m_Dialog);
                //}

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
            if (m_Dialog!=null)
                m_Dialog.Draw();
            /*
            if (m_pDialog)
                m_pDialog->Paint();
            else if (m_pUpDial)
                m_pUpDial->Paint();
             */
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
        }

        internal override bool DialFinish(Control wnd)
        {
            if (m_Dialog==null && m_UpDial==null)
                throw new Exception("LineSubdivisionUI.DialFinish - No dialog!");

            // If we are doing an update, alter the original operation.
            //ISpatialDisplay view = ActiveDisplay;
            UpdateUI up = this.Update;

            if (up!=null)
            {
                throw new NotImplementedException("LineSubdivisionUI.DialFinish");
                /*
		                // Get the original operation.
		                CeArcSubdivision* pop = dynamic_cast<CeArcSubdivision*>(pup->GetOp());
		                if ( !pop ) {
			                ShowMessage("CuiArcSubdivision::DialFinish\nUnexpected edit type.");
			                return FALSE;
		                }

		                // Make the update.
                        // pop->Correct();
                 */
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
	        return FinishCommand();
        }
    }
}
