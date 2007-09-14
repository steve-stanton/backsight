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

namespace Backsight.Forms
{
    public interface ISpatialDisplayTool
    {
        /// <summary>
        /// Conclude the display tool (close down any dialogs, restore default cursor, throw away
        /// whatever the user was doing).
        /// </summary>
        void Escape();

        /// <summary>
        /// Returns an ID number that uniquely identifies the display tool (such as those
        /// defined in <c>DisplayToolId</c>).
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Starts the display tool
        /// </summary>
        /// <returns>True if successfully started</returns>
        bool Start();

        /// <summary>
        /// Finishes the display tool
        /// </summary>
        /// <returns>True if successfully finished</returns>
        bool Finish();

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="p">The position where the mouse click occurred</param>
        void MouseDown(IPosition p, MouseButtons b);

        /// <summary>
        /// Handles a mouse up event
        /// </summary>
        /// <param name="p">The position where the mouse up occurred</param>
        void MouseUp(IPosition p, MouseButtons b);

        /// <summary>
        /// Handles a mouse move event
        /// </summary>
        /// <param name="p">The position the mouse has moved to</param>
        void MouseMove(IPosition p, MouseButtons b);

        /// <summary>
        /// Handles a mouse wheel event
        /// </summary>
        /// <param name="delta">The amount of wheel movement (as returned by
        /// the <c>MouseEventArgs.Delta</c> property)</param>
        /// <param name="k">Any modifier keys pressed during the wheel (as returned
        /// by <c>Control.ModifierKeys</c>)</param>
        void MouseWheel(int delta, Keys k);
    }
}
