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

using Backsight.Forms;
using Backsight.Editor.Forms;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="13-DEC-1999" />
    /// <was>CuiNewPoint</was>
    /// <summary>
    /// User interface for defining a new point (also used for updating control points).
    /// </summary>
    class NewPointUI : CommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The dialog for the command.
        /// </summary>
        private NewPointForm m_Dialog;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        internal NewPointUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            m_Dialog = null;
        }

        /// <summary>
        /// Constructor for command recall.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="op">The operation that's being recalled.</param>
        internal NewPointUI(IControlContainer cc, IUserAction action, Operation op)
            : base(cc, action, null, op)
        {
            m_Dialog = null;
        }

        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="updcmd">The update command.</param>
        internal NewPointUI(IControlContainer cc, IUserAction action, UpdateUI updcmd)
            : base(cc, action, updcmd)
        {
            m_Dialog = null;
        }

        #endregion

        public override void Dispose()
        {
            if (m_Dialog!=null)
            {
                m_Dialog.Dispose();
                m_Dialog = null;
            }
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
	        // Don't run more than once.
	        if (m_Dialog!=null)
                throw new InvalidOperationException("NewPointUI.Run - Command is already running.");

        	UpdateUI pup = this.Update;

	        // Create the appropriate sort of dialog.
	        switch (this.EditId)
            {
                case EditingActionId.NewPoint:
                {
        		    if (pup!=null)
			            m_Dialog = new NewPointForm(pup, "Update Point", null);
		            else
			            m_Dialog = new NewPointForm(this, "Define New Point", this.Recall);

		            break;
                }

                case EditingActionId.GetControl:
                {
		            if (pup!=null)
                    {
			            m_Dialog = new NewPointForm(pup, "Update Control Point", null);
			            break;
		            }

                    // Drop through to default if not making an update.
                    goto default;
                }

            	default:
                {
                        throw new Exception("NewPointUI.Run - Unexpected command id.");
                }
            }

        	m_Dialog.Show();
	        return true;
        }

        /// <summary>
        /// Do any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn.</param>
        internal override void Paint(PointFeature point)
        {
            // do nothing
        }

        /// <summary>
        /// Handles mouse-move.
        /// </summary>
        /// <param name="p">The new position of the mouse</param>
        internal override void MouseMove(IPosition p)
        {
            // do nothing
        }

        /// <summary>
        /// Handles left mouse click.
        /// </summary>
        /// <param name="p">The position where the left-click occurred.</param>
        internal override void LButtonDown(IPosition p)
        {
            // do nothing
        }

        /// <summary>
        /// Handles left-up mouse click.
        /// </summary>
        /// <param name="p">The position where the left-up occurred.</param>
        internal override void LButtonUp(IPosition p)
        {
            // do nothing
        }

        /// <summary>
        /// Handles double-click.
        /// </summary>
        /// <param name="p">The position where the double-click occurred.</param>
        internal override void LButtonDblClick(IPosition p)
        {
            // do nothing
        }

        /// <summary>
        /// Handles right mouse click.
        /// </summary>
        /// <param name="p">The position where the right-click occurred.</param>
        /// <returns>True if handled.</returns>
        internal override bool RButtonDown(IPosition p)
        {
            return false;
        }

        /// <summary>
        /// Reacts to selection of the Cancel button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window.</param>
        internal override void DialAbort(Control wnd)
        {
	        // Get the base class to finish up.
	        AbortCommand();
        }

        /// <summary>
        /// Reacts to selection of the OK button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window.</param>
        /// <returns></returns>
        internal override bool DialFinish(Control wnd)
        {
	        if (m_Dialog==null)
                throw new Exception("NewPointUI.DialFinish -- No dialog!");

            // Hide the dialog so it doesn't become part of any saved display
            m_Dialog.Visible = false;

	        // If we are doing an update, the original position will
	        // be updated by the base class.

	        if (this.Update==null)
            {
		        // Save the new point.
                PointFeature p = m_Dialog.Save();
		        if (p==null)
                    return false;

                // Ensure the point is on screen, and select it.
                Controller.EnsureVisible(p, true);
	        }

	        // Get the base class to finish up.
	        return FinishCommand();
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Receives a sub-command. These actually get sent down via the controller class.
        /// </summary>
        /// <param name="id">The ID of the sub-command.</param>
        /// <returns>True if sub-command was dispatched.</returns>
        internal override bool Dispatch(int id)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// The position that has been specified for the point (null if no position available)
        /// </summary>
        internal IPosition Position
        {
            get
            {
                if (m_Dialog!=null)
                    return m_Dialog.Position;
                else
                    return null;
        	}
        }

        internal override void SetOffset(Offset offset)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
