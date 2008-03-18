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
using System.Drawing;

using Backsight.Forms;
using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="28-OCT-1999" was="CuiPath"/>
    /// <summary>
    /// User interface for doing a connection path.
    /// </summary>
    class PathUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// Dialog for getting the from point.
        /// </summary>
        GetPointForm m_DialFrom;

        /// <summary>
        /// Dialog for getting the to point.
        /// </summary>
        GetPointForm m_DialTo;

        /// <summary>
        /// The actual connection path.
        /// </summary>
//        PathForm m_DialPath;

        /// <summary>
        /// The update dialog.
        /// </summary>
//        UpdatePathForm m_DialUp;

        /// <summary>
        /// The from point.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The to point.
        /// </summary>
        PointFeature m_To;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a new connection path.
        /// </summary>
        /// <param name="cc">The container for any dialog controls</param>
        /// <param name="action">The action that initiated this command</param>
        internal PathUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            Zero();
        }

        /// <summary>
        /// Constructor for command recall.
        /// </summary>
        /// <param name="cc">The container for any dialog controls</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="op">The operation that's being recalled.</param>
        internal PathUI(IControlContainer cc, IUserAction action, Operation op)
            : base(cc, action, null, op)
        {
            Zero();
        }

        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="cc">The container for any dialog controls</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="updcmd">The update command.</param>
        internal PathUI(IControlContainer cc, IUserAction action, UpdateUI updcmd)
            : base(cc, action, updcmd)
        {
            Zero();
        }

        #endregion

        /// <summary>
        /// Ensures all class data has initial values (for use by constructors)
        /// </summary>
        void Zero()
        {
            m_DialFrom = null;
            m_DialTo = null;
            //m_DialPath = null;
            //m_DialUp = null;
            m_From = null;
            m_To = null;
        }

        public override void Dispose()
        {
            base.Dispose(); // removes any controls from container

            // Ensure any sub-dialogs have been destroyed.
            KillDialogs();
        }

        internal override bool Run()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /*
        //	@mfunc	Start the user interface (if any) for this command.
        //
        //	@rdesc	TRUE if command started ok.
        //			
        //////////////////////////////////////////////////////////////////////

        LOGICAL CuiPath::Run ( void ) {

            if ( !StartUpdate() ) StartFrom();

            return TRUE;

        } // end of Run

        //////////////////////////////////////////////////////////////////////
        //
        //	@mfunc	Do any command-specific drawing.
        //
        //	@parm	The specific point (if any) that the parent window has
        //			drawn. Not used.
        //
        //////////////////////////////////////////////////////////////////////

        void CuiPath::Paint ( const CePoint* const pPoint ) {

            if ( m_pDialFrom ) m_pDialFrom->Paint();
            if ( m_pDialTo   ) m_pDialTo->Paint();
            if ( m_pDialPath ) m_pDialPath->OnDraw(pPoint);
            if ( m_pDialUp	 ) m_pDialUp->Paint();

        } // end of Paint

        //////////////////////////////////////////////////////////////////////
        //
        //	@mfunc	React to the selection of a point feature.
        //
        //	@parm	The point (if any) that has been selected.
        //
        //////////////////////////////////////////////////////////////////////

        void CuiPath::OnSelectPoint ( const CePoint* const pPoint ) {

            if ( m_pDialFrom )
                m_pDialFrom->OnSelectPoint((CePoint*)pPoint);
            else if ( m_pDialTo   )
                m_pDialTo->OnSelectPoint((CePoint*)pPoint);
            else if ( m_pDialPath )
                m_pDialPath->OnSelectPoint((CePoint*)pPoint);

        } // end of OnSelectPoint

        //////////////////////////////////////////////////////////////////////
        //
        //	@mfunc	React to selection of the Cancel button in the dialog.
        //
        //	@parm	The dialog window.
        //
        //////////////////////////////////////////////////////////////////////

        void CuiPath::DialAbort ( CWnd* pWnd ) {

            // Destroy any sub-dialogs we have going.
            KillDialogs();

            // Get the base class to finish off.
            AbortCommand();

        } // end of DialAbort
        */

        /// <summary>
        /// Gets rid of any active sub-dialog(s).
        /// </summary>
        void KillDialogs()
        {
            if (m_DialFrom!=null)
            {
                m_DialFrom.Dispose();
                m_DialFrom = null;
            }

            if (m_DialTo!=null)
            {
                m_DialTo.Dispose();
                m_DialTo = null;
            }

            //if (m_DialPath!=null)
            //{
            //    m_DialPath.Dispose();
            //    m_DialPath = null;
            //}

            //if (m_DialUp!=null)
            //{
            //    m_DialUp.Dispose();
            //    m_DialUp = null;
            //}
        }
        /*
        //////////////////////////////////////////////////////////////////////
        //
        //	@mfunc	React to selection of the OK button in the dialog.
        //
        //	@parm	The dialog window.
        //
        //////////////////////////////////////////////////////////////////////

        LOGICAL CuiPath::DialFinish ( CWnd* pWnd ) {

            // Get rid of sub-dialog(s).
            KillDialogs();

            // Get the base class to finish off.
            return FinishCommand();

        } // end of DialFinish
        */

        /// <summary>
        /// Handles stuff when user clicks on the "Back" button on one of the point sub-dialogs.
        /// </summary>
        internal void OnPointBack()
        {
            if (m_DialTo!=null)
            {
                StopTo();
                StartFrom();
            }
        }

        /// <summary>
        /// Handles stuff when user clicks on the "Next" button on one of the point sub-dialogs.
        /// </summary>
        internal void OnPointNext()
        {
            if (m_DialFrom!=null)
            {
                StopFrom();
                StartTo();
            }
            else if (m_DialTo!=null)
            {
                StopTo();
                StartPath();
            }
        }

        /// <summary>
        /// Handles stuff when user clicks on the "Cancel" button on one of the point sub-dialogs.
        /// </summary>
        internal void OnPointCancel()
        {
            // Destroy the whole shooting match.
            DialAbort(null);
        }

        /// <summary>
        /// Start update dialog (if applicable).
        /// </summary>
        /// <returns>True if update dialog started.</returns>
        bool StartUpdate()
        {
            // Return if we're not doing an update.
            UpdateUI up = this.Update;
            if (up==null)
                return false;

            throw new NotImplementedException("PathUI.StartUpdate");

            /*
            m_DialUp = new UpdatePathForm(up);
            m_DialUp.Show();
            return true;
             */
        }

        /// <summary>
        /// Starts the from-point sub-dialog.
        /// </summary>
        void StartFrom()
        {
            m_DialFrom = new GetPointForm(this, "Starting Point", Color.DarkBlue, false);
            m_DialFrom.Show();
            m_DialFrom.OnSelectPoint(m_From, false);
        }

        /// <summary>
        /// Stops the from-point sub-dialog.
        /// </summary>
        void StopFrom()
        {
            m_From = m_DialFrom.Point;
            m_DialFrom = null;

            if (m_From!=null)
                m_From.Draw(ActiveDisplay, Color.DarkBlue);
        }

        /// <summary>
        /// Starts the to-point sub-dialog.
        /// </summary>
        void StartTo()
        {
            m_DialTo = new GetPointForm(this, "Finishing Point", Color.LightBlue, true);
            m_DialTo.Show();
            m_DialTo.OnSelectPoint(m_To, false);
        }

        /// <summary>
        /// Stops the to-point sub-dialog.
        /// </summary>
        void StopTo()
        {
            m_To = m_DialTo.Point;
            m_DialTo = null;

            if (m_To!=null)
                m_To.Draw(ActiveDisplay, Color.LightBlue);
        }

        void StartPath()
        {
            throw new NotImplementedException("PathUI.StartPath");
        }
        /*
        //	@mfunc	Start the main connection path sub-dialog.

        void CuiPath::StartPath ( void ) {

            assert(m_pDialFrom==0);
            assert(m_pDialTo==0);

            if ( m_pFrom==0 || m_pTo==0 ) {
                ShowMessage("Terminal points are unavailable.");
                return;
            }

            m_pDialPath = new CdPath(*this,*m_pFrom,*m_pTo);
            m_pDialPath->Create(CdPath::IDD);

        } // end of StartPath
         */
    }
}
