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

namespace Backsight.Editor;

/// <written by="Steve Stanton" on="13-NOV-2007" />
/// <summary>
/// A spatial selection
/// </summary>
class Selection : ISpatialSelection
{
    #region Class data

    /// <summary>
    /// The currently selected spatial objects (never null)
    /// </summary>
    private readonly List<ISpatialObject> m_Items;

    /// <summary>
    /// The topological section that coincides with this selection. This will be
    /// defined only if the selection refers to a single topological line that has
    /// been divided into a series of sections.
    /// </summary>
    private readonly DividerObject? m_Section;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new <c>Selection</c> that contains a single item (or nothing).
    /// </summary>
    /// <param name="so">The object to remember as part of this selection (if null, it
    /// will not be added to the selection)</param>
    /// <param name="searchPosition">A position associated with the selection (null
    /// if a specific position isn't relevant). This is used to determine whether a
    /// topological section is relevant when a line is selected.</param>
    public Selection(ISpatialObject so, IPosition? searchPosition = null)
    {
        m_Items = new List<ISpatialObject>(1);
        if (so!=null)
            m_Items.Add(so);

        // If we're dealing with a single line that's been topologically sectioned,
        // determine which divider we're closest to.

        m_Section = null;

        if (searchPosition is not null)
        {
            LineFeature line = (so as LineFeature);
            if (line?.Topology is SectionTopologyList sections)
            {
                //SectionTopologyList sections = (line.Topology as SectionTopologyList);
                IDivider d = sections.FindClosestSection(searchPosition);
                if (d is not null)
                    m_Section = new DividerObject(d);
            }
        }
    }

    /// <summary>
    /// Creates a new <c>Selection</c> that refers to nothing.
    /// </summary>
    internal Selection()
    {
        m_Section = null;
        m_Items = new List<ISpatialObject>();
    }

    /// <summary>
    /// Creates a new <c>Selection</c> that consists of the items in the supplied list.
    /// </summary>
    /// <param name="items">The items defining the content of the new selection</param>
    internal Selection(IEnumerable<ISpatialObject> items)
    {
        m_Section = null;
        m_Items = new List<ISpatialObject>(items);
    }

    #endregion

    /// <summary>
    /// Removes a spatial object from this selection.
    /// </summary>
    /// <param name="so">The object to remove from this selection</param>
    /// <returns>True if object removed. False if the object isn't part of this selection.</returns>
    internal bool Remove(ISpatialObject so)
    {
        return m_Items.Remove(so);
    }

    /// <summary>
    /// The one and only item in this selection (null if the selection is empty, or
    /// it contains more than one item).
    /// </summary>
    public ISpatialObject? SingleOrDefault
    {
        get
        {
            if (m_Section is not null)
                return m_Section;

            return (m_Items.Count==1 ? m_Items[0] : null);
        }
    }

    /// <summary>
    /// The number of items in the selection
    /// </summary>
    public int Count
    {
        get { return (m_Items.Count); }
    }

    /// <summary>
    /// The items in the selection
    /// </summary>
    public IEnumerable<ISpatialObject> Items
    {
        get { return m_Items; }
    }

    /// <summary>
    /// Draws the items in this selection.
    /// </summary>
    /// <param name="display">The display to draw to</param>
    public void Draw(IMapDisplay display)
    {
        foreach (ISpatialObject item in m_Items)
            display.Draw(item);
    }

    /// <summary>
    /// Checks whether this selection refers to the same spatial objects as
    /// another selection, and has the same reference position.
    /// </summary>
    /// <param name="that">The selection to compare with</param>
    /// <returns>True if the two selections refer to the same spatial objects (not
    /// necessarily in the same order)</returns>
    internal bool IsEqual(Selection that)
    {
        // The same spatial objects have to be involved
        if (!SameItems(this, that))
            return false;

        // If both selections refer to the same divider (or null), they're the same
        IDivider? d1 = this.m_Section?.Divider;
        IDivider? d2 = that.m_Section?.Divider;
        return ReferenceEquals(d1, d2);
    }

    /// <summary>
    /// Checks whether two selections refer to the same objects
    /// </summary>
    /// <param name="a">The 1st selection</param>
    /// <param name="b">The 2nd selection</param>
    /// <returns>True if both selections are not null, contain the same number
    /// of elements, and refer to the same spatial objects (the same instances)</returns>
    private static bool SameItems(ISpatialSelection? a, ISpatialSelection? b)
    {
        if (a is null || b is null)
            return false;

        if (Object.ReferenceEquals(a, b))
            return true;

        if (a.Count != b.Count)
            return false;
        
        foreach (ISpatialObject sob in b.Items)
        {
            bool found = false;

            foreach (ISpatialObject soa in a.Items)
            {
                if (Object.ReferenceEquals(soa, sob))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;
        }

        return true;
    }

    internal DividerObject? Divider => m_Section;

    /// <summary>
    /// Adds a spatial object to this selection, given that it is not already
    /// part of the selection.
    /// </summary>
    /// <param name="so">The object to remember as part of this selection (not null)</param>
    /// <exception cref="ArgumentNullException">If the specified object is null</exception>
    internal void Add(ISpatialObject so)
    {
        if (so==null)
            throw new ArgumentNullException();

        if (!m_Items.Contains(so))
            m_Items.Add(so);
    }

    /// <summary>
    /// Adds a collection of spatial objects to this selection (checking to confirm
    /// that they're not already part of this selection)
    /// </summary>
    /// <param name="items">The items to add</param>
    internal void AddRange(IEnumerable<ISpatialObject> items)
    {
        foreach (ISpatialObject so in items)
            Add(so);
    }
}