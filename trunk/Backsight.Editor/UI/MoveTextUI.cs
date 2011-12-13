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
using System.Drawing;

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Editor.Operations;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="16-JAN-2009"/>
    /// <summary>
    /// User interface for moving an item of text.
    /// </summary>
    class MoveTextUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The text that's being moved (not null)
        /// </summary>
        readonly TextFeature m_Text;

        /// <summary>
        /// The last position for the text
        /// </summary>
        PointGeometry m_LastPosition;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>MoveTextUI</c>
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="text">The text to move (not null)</param>
        internal MoveTextUI(IControlContainer cc, IUserAction action, TextFeature text)
            : base(cc, action)
        {
            if (text == null)
                throw new ArgumentNullException();

            m_Text = text;
        }

        #endregion

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns>True (always)</returns>
        internal override bool Run()
        {
            // Ensure the text is un-selected
            Controller.ClearSelection();

            // Switch to a null cursor (we'll be dragging the text).
            Cursor.Current = null;

            return true;
        }

        /// <summary>
        /// Handles mouse-move.
        /// </summary>
        /// <param name="p">The new position of the mouse</param>
        internal override void MouseMove(IPosition p)
        {
            // Erase the text at its previous position, and draw it at the new position.
            Controller.ActiveDisplay.RestoreLastDraw();

            // Remember the mouse position. We'll draw at this new position when
            // the Paint method gets called
            m_LastPosition = PointGeometry.Create(p);
        }

        /// <summary>
        /// Do any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn.
        /// Not used.</param>
        internal override void Paint(PointFeature point)
        {
            if (m_LastPosition != null)
            {
                // Draw gray text in the original position
                ISpatialDisplay display = ActiveDisplay;
                m_Text.Draw(display, Color.Gray);

                // Draw the text at the last mouse position
                IPointGeometry p = m_Text.Position;
                try
                {
                    m_Text.TextGeometry.Position = m_LastPosition;
                    m_Text.Draw(display, Color.Red);
                }

                finally
                {
                    m_Text.TextGeometry.Position = p;
                }
            }
        }

        /// <summary>
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <c>Paint</c> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return true; }
        }

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="p">The position where the click occurred</param>
        /// <returns>True, indicating that the text was moved.</returns>
        internal override bool LButtonDown(IPosition p)
        {
            MoveTextOperation op = null;

            try
            {
                op = new MoveTextOperation(m_Text);
                op.Execute(PointGeometry.Create(p));
                FinishCommand();
                return true;
            }

            catch (Exception ex)
            {
                //Session.CurrentSession.Remove(op);
                MessageBox.Show(ex.StackTrace, ex.Message);
            }

            return false;
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
