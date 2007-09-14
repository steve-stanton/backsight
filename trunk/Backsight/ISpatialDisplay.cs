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
    /// <summary>
    /// Something that draws maps
    /// </summary>
    public interface ISpatialDisplay
    {
        /// <summary>
        /// The current ground extent of the map display
        /// </summary>
        IWindow Extent { get; }

        /// <summary>
        /// The biggest extent for the map display
        /// </summary>
        IWindow MaxExtent { get; }

        /// <summary>
        /// The current draw scale, as a scale denominator (i.e. 100 for a map at
        /// a scale of 1:100)
        /// </summary>
        double MapScale { get; }

        /// <summary>
        /// The surface on which to draw
        /// </summary>
        Graphics Graphics { get; }

        /// <summary>
        /// Converts an easting into display units
        /// </summary>
        /// <param name="x">The easting to convert</param>
        /// <returns>The corresponding position on the display</returns>
        float EastingToDisplay(double x);

        /// <summary>
        /// Converts a northing into display units
        /// </summary>
        /// <param name="y">The northing to convert</param>
        /// <returns>The corresponding position on the display</returns>
        float NorthingToDisplay(double y);

        /// <summary>
        /// Converts a length on the ground into display units.
        /// </summary>
        /// <param name="groundLength">The ground dimension, in meters.</param>
        /// <returns>The corresponding distance in display units.</returns>
        float LengthToDisplay(double groundLength);

        /// <summary>
        /// Handles model replacement (e.g. when user opens a different map). The
        /// newly opened map should be obtained through <c>ISpatialController.MapModel</c>
        /// </summary>
        /// <param name="initialDrawExtent">The extent that should be initially drawn (null for
        /// an overview of the data in the map model)</param>
        void ReplaceMapModel(IWindow initialDrawExtent);

        /// <summary>
        /// Handles a change to the current selection
        /// </summary>
        /// <param name="oldSelection">The current selection (may be null)</param>
        /// <param name="newSelection">The new selection (may be null)</param>
        void OnSelectionChanging(ISpatialSelection oldSelection, ISpatialSelection newSelection);

        /// <summary>
        /// Force the display to reveal anything accumulated in its draw buffer. Meant
        /// to provide user feedback during protracted draws.
        /// </summary>
        void PaintNow();

        /// <summary>
        /// Restores the display buffer so that it contains the stuff it had upon
        /// completion of the last draw (i.e. the last "hard" draw from the model).
        /// This removes any transient graphics that may have been drawn afterwards
        /// (such as highlighting). The map display is not actually repainted, because
        /// you may wish to draw further transient items before revealing things.
        /// When you have prepared the display buffer, make a call to <c>PaintNow</c>
        /// to reveal it.
        /// </summary>
        void RestoreLastDraw();

        /// <summary>
        /// Redraws the display from the model.
        /// </summary>
        void Redraw();

        /// <summary>
        /// Draws an overview of the entire model.
        /// </summary>
        void DrawOverview();

        /// <summary>
        /// Draws the specified spatial extent.
        /// </summary>
        /// <param name="win">The window to draw</param>
        void DrawWindow(IWindow win);

        /// <summary>
        /// The position at the center of the display (when set, the display will be redrawn).
        /// </summary>
        IPosition Center { get; set; }

        /// <summary>
        /// Displays a context menu
        /// </summary>
        /// <param name="p">The position where the menu should appear</param>
        /// <param name="menu">The menu to display</param>
        void ShowContextMenu(IPosition p, ContextMenuStrip menu);

        /// <summary>
        /// The panel holding the display. You might need this to do low-level things
        /// like working out screen positions.
        /// </summary>
        Control MapPanel { get; }
    }
}
