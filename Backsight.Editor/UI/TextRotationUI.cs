// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Windows.Forms;

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Editor.Operations;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="13-APR-2008"/>
    /// <summary>
    /// User interface for defining the default text rotation angle (by specifying
    /// two positions)
    /// </summary>
    class TextRotationUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The first position specified by the user
        /// </summary>
        IPosition m_Point1;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>TextRotationUI</c>
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        internal TextRotationUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            m_Point1 = null;
        }

        #endregion

        internal override bool Run()
        {
            SetCommandCursor();
            return true;
        }

        internal override void SetCommandCursor()
        {
            if (m_Point1==null)
                ActiveDisplay.MapPanel.Cursor = EditorResources.Point1Cursor;
            else
                ActiveDisplay.MapPanel.Cursor = EditorResources.Point2Cursor;
        }

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="p">The position where the click occurred</param>
        /// <returns>True if the command handled the mouse down. False if it did nothing.</returns>
        internal override bool LButtonDown(IPosition p)
        {
            if (m_Point1 == null)
            {
                // Save the specified position and switch to the point#2 cursor
                m_Point1 = p;
                SetCommandCursor();
            }
            else
            {
                // Second point received, so execute the edit...
                SetLabelRotation(m_Point1, p);
            }

            return true;
        }

        /// <summary>
        /// Executes the operation to set the rotation angle of labels in
        /// the map, given the two orientation points.
        /// </summary>
        /// <param name="point1">The first position</param>
        /// <param name="point2">The second position</param>
        void SetLabelRotation(IPosition point1, IPosition point2)
        {
            TextRotationOperation op = null;

            try
            {
                op = new TextRotationOperation(Session.WorkingSession);
                op.Execute(point1, point2);
                FinishCommand();
            }

            catch (Exception ex)
            {
                Session.WorkingSession.Remove(op);
                MessageBox.Show(ex.Message);
                AbortCommand();
            }
        }

        /// <summary>
        /// Reacts to a situation where the user presses the ESC key, by aborting this command.
        /// </summary>
        internal override void Escape()
        {
            AbortCommand();
        }
    }
}
