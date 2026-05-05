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

using System.Timers;
using Timer = System.Timers.Timer;

namespace Backsight.Editor;

/// <written by="Steve Stanton" on="15-DEC-2006" />
/// <summary>
/// Draws all features within a window. To provide feedback during longer draws,
/// the map display will be painted every 0.5 seconds.
/// </summary>
class DrawQuery
{
    /// <summary>
    /// The display to draw to
    /// </summary>
    readonly IMapDisplay m_MapDisplay;

    /// <summary>
    /// Does the display need to be painted?
    /// </summary>
    bool m_DoPaint;

    /// <summary>
    /// Creates a new <c>DrawQuery</c> that refers to the specified feature type(s),
    /// drawing the results of the spatial query to the specified display.
    /// </summary>
    /// <param name="index">The index to query</param>
    /// <param name="mapDisplay">The display to draw to</param>
    /// <param name="types">The type(s) of spatial feature to draw</param>
    public DrawQuery(ISpatialIndex index, IMapDisplay mapDisplay, SpatialType types)
    {
        m_MapDisplay = mapDisplay;
        m_DoPaint = false;
        Timer t = new Timer(500);
        t.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        t.Start();
        index.QueryWindow(m_MapDisplay.Extent, types, OnQueryHit);
        t.Close();
        mapDisplay.PaintNow();
    }

    /// <summary>
    /// Timer event handler that gets called every 0.5 seconds during the
    /// draw. This records that the map display should be painted when
    /// <see cref="OnQueryHit"/> is next called.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        m_DoPaint = true;
    }

    /// <summary>
    /// Delegate that's called whenever the index finds an object with an extent that
    /// overlaps the query window.
    /// </summary>
    /// <param name="item">The item to process</param>
    /// <returns>True (always), meaning the query should continue.</returns>
    bool OnQueryHit(ISpatialObject item)
    {
        if (m_DoPaint)
        {
            m_DoPaint = false;
            m_MapDisplay.PaintNow();
        }

        m_MapDisplay.Draw(item);
        return true;
    }
}