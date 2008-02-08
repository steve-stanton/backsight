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
using System.Drawing;
using System.Windows.Forms;

namespace Backsight
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>
    /// Controls spatially related data in an application that is based on the
    /// model-view-controller architecture.
    /// </summary>
    public interface ISpatialController
    {
        /// <summary>
        /// The current map model.
        /// </summary>
        ISpatialModel MapModel { get; }

        /// <summary>
        /// The current selection (some subset of the stuff in the model). Never null,
        /// though it may be empty.
        /// </summary>
        ISpatialSelection SpatialSelection { get; }

        /// <summary>
        /// Registers a map display with this controller.
        /// </summary>
        /// <param name="display">The display this controller needs to manage</param>
        void Register(ISpatialDisplay display);

        /// <summary>
        /// Un-registers a map display with this controller.
        /// </summary>
        /// <param name="display">The display this controller no longer needs to manage</param>
        void Unregister(ISpatialDisplay display);

        /// <summary>
        /// The currently "active" map display (null if no displays are registered with this controller).
        /// </summary>
        ISpatialDisplay ActiveDisplay { get; }

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="sender">The display where the mouse event originated</param>
        /// <param name="p">The position where the mouse click occurred</param>
        /// <param name="b">The specific mouse button that was pressed</param>
        void MouseDown(ISpatialDisplay sender, IPosition p, MouseButtons b);

        /// <summary>
        /// Handles a mouse up event
        /// </summary>
        /// <param name="sender">The display where the mouse event originated</param>
        /// <param name="p">The position where the mouse up occurred</param>
        /// <param name="b">The specific mouse button that was pressed</param>
        void MouseUp(ISpatialDisplay sender, IPosition p, MouseButtons b);

        /// <summary>
        /// Handles a mouse move event
        /// </summary>
        /// <param name="sender">The display where the mouse event originated</param>
        /// <param name="p">The position the mouse has moved to</param>
        /// <param name="b">The specific mouse button that was pressed</param>
        void MouseMove(ISpatialDisplay sender, IPosition p, MouseButtons b);

        /// <summary>
        /// Handles a key down event
        /// </summary>
        /// <param name="sender">The display where the key event originated</param>
        /// <param name="k">Information about the event</param>
        void KeyDown(ISpatialDisplay sender, KeyEventArgs k);

        /// <summary>
        /// Returns the default drawing style
        /// </summary>
        /// <returns></returns>
        IDrawStyle DrawStyle { get; }

        /// <summary>
        /// Returns the drawing style used for highlighting selected features.
        /// </summary>
        IDrawStyle HighlightStyle { get; }

        /// <summary>
        /// Perform any processing whenever a display has changed the drawn extent
        /// of a map.
        /// </summary>
        /// <param name="sender">The display that has changed</param>
        void OnSetExtent(ISpatialDisplay sender);

        /// <summary>
        /// Perform any processing whenever a display has been painted (give
        /// the controller the opportunity to draw on top of the regular display).
        /// </summary>
        /// <param name="sender">The display that was just painted</param>
        //void OnPaint(ISpatialDisplay sender);
    }
}
