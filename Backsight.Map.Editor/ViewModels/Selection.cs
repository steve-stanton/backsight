using System.Collections.Generic;
using Backsight.Model;

namespace Backsight.Map.Editor.ViewModels;

/// <written by="Steve Stanton" on="13-NOV-2007" />
/// <summary>
/// A spatial selection
/// </summary>
internal class Selection : IMapSelection
{
    /// <summary>
    /// The currently selected spatial objects (never null)
    /// </summary>
    private readonly List<IMapObject> m_Items;

    /// <summary>
    /// The topological section that coincides with this selection. This will be
    /// defined only if the selection refers to a single topological line that has
    /// been divided into a series of sections.
    /// </summary>
    private readonly DividerObject? m_Section;

    /// <summary>
    /// Creates a new <c>Selection</c> that contains a single item (or nothing).
    /// </summary>
    /// <param name="o">The object to remember as part of this selection.</param>
    /// <param name="searchPosition">A position associated with the selection (null
    /// if a specific position isn't relevant). This is used to determine whether a
    /// topological section is relevant when a line is selected.</param>
    internal Selection(IMapObject o, IPosition? searchPosition = null)
    {
        m_Items = new List<IMapObject>(1);
        m_Items.Add(o);

        // If we're dealing with a single line that's been topologically sectioned,
        // determine which divider we're closest to.

        m_Section = null;

        if (searchPosition is not null && o is LineFeature line)
        {
            if (line.Topology is SectionTopologyList sections)
            {
                IDivider? d = sections.FindClosestSection(searchPosition);
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
        m_Items = new List<IMapObject>();
    }

    /// <summary>
    /// Creates a new <c>Selection</c> that consists of the items in the supplied list.
    /// </summary>
    /// <param name="items">The items defining the content of the new selection</param>
    internal Selection(IEnumerable<IMapObject> items)
    {
        m_Section = null;
        m_Items = new List<IMapObject>(items);
    }

    /// <summary>
    /// Removes a spatial object from this selection.
    /// </summary>
    /// <param name="o">The object to remove from this selection</param>
    /// <returns>True if object removed. False if the object isn't part of this selection.</returns>
    internal bool Remove(IMapObject o)
    {
        return m_Items.Remove(o);
    }

    /// <summary>
    /// The one and only item in this selection (null if the selection is empty, or
    /// it contains more than one item).
    /// </summary>
    internal IMapObject? SingleOrDefault
    {
        get
        {
            if (m_Section is not null)
                return m_Section;

            return m_Items.Count == 1 ? m_Items[0] : null;
        }
    }

    /// <summary>
    /// The number of items in the selection
    /// </summary>
    internal int Count => m_Items.Count;

    /// <summary>
    /// The items in the selection
    /// </summary>
    internal IEnumerable<IMapObject> Items => m_Items;

    /// <inheritingdoc cref="IMapSelection.Items" />
    IReadOnlyList<IMapObject> IMapSelection.Items => m_Items;

    /// <inheritingdoc cref="IMapSelection.LineSection" />
    LineGeometry? IMapSelection.LineSection => m_Section?.Geometry;
    
    /*
    /// <summary>
    /// Draws the items in this selection.
    /// </summary>
    /// <param name="display">The display to draw to</param>
    public void Draw(IMapDisplay display)
    {
        foreach (ISpatialObject item in m_Items)
            display.Draw(item);
    }
*/
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
    private static bool SameItems(Selection? a, Selection? b)
    {
        if (a is null || b is null)
            return false;

        if (ReferenceEquals(a, b))
            return true;

        if (a.Count != b.Count)
            return false;
        
        foreach (var ob in b.Items)
        {
            bool found = false;

            foreach (var oa in a.Items)
            {
                if (ReferenceEquals(oa, ob))
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
    /// <param name="o">The object to remember as part of this selection.</param>
    internal void Add(IMapObject o)
    {
        if (!m_Items.Contains(o))
            m_Items.Add(o);
    }

    /// <summary>
    /// Adds a collection of spatial objects to this selection (checking to confirm
    /// that they're not already part of this selection)
    /// </summary>
    /// <param name="items">The items to add</param>
    internal void AddRange(IEnumerable<IMapObject> items)
    {
        foreach (var item in items)
            Add(item);
    }
}