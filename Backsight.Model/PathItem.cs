using Backsight.Model.Observations;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="21-JAN-1998" was="CePathItem" />
/// <summary>
/// An item in a path description. This is a helper class used by
/// the <see cref="PathForm"/> dialog.
/// </summary>
class PathItem
{
    /// <summary>
    /// Obtains the highest leg number in the supplied item array.
    /// </summary>
    /// <param name="items">Array of path items.</param>
    /// <returns>The maximum value for the leg number in the supplied items.</returns>
    internal static int GetMaxLegNumber(PathItem[] items)
    {
        // Each path item contains a leg number, arranged sequentially.
        int nleg=0;

        foreach (PathItem item in items)
            nleg = Math.Max(nleg, item.LegNumber);

        return nleg;
    }

    /// <summary>
    /// The type of item
    /// </summary>
    PathItemType m_Item;

    /// <summary>
    /// Associated value (if any). The meaning of the value
    /// depends on the type of item.
    /// </summary>
    double m_Value;

    /// <summary>
    /// The type of distance unit in effect for the item (defined
    /// for all items, even if the value is undefined).
    /// </summary>
    DistanceUnit m_Unit;

    /// <summary>
    /// Leg sequence number (defined values start at 1). Circular
    /// legs have a negated leg number.
    /// </summary>
    int m_Leg;

    /// <summary>
    /// Default constructor creates a null item.
    /// </summary>
    internal PathItem()
    {
        m_Item = PathItemType.Null;
        m_Unit = null;
        m_Value = 0.0;
        m_Leg = 0;
    }

    /// <summary>
    /// Creates a new <c>PathItem</c>
    /// </summary>
    /// <param name="itemType">The type of item involved</param>
    /// <param name="unit">The distance unit (was default=null)</param>
    /// <param name="value">The item value (was default=0.0)</param>
    internal PathItem(PathItemType itemType, DistanceUnit unit, double value)
    {
        m_Item = itemType;
        m_Unit = unit;
        m_Value = value;
        m_Leg = 0;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copy">The item to copy from</param>
    internal PathItem(PathItem copy)
    {
        m_Item = copy.m_Item;
        m_Unit = copy.m_Unit;
        m_Value = copy.m_Value;
        m_Leg = copy.m_Leg;
    }

    internal int LegNumber 
    {
        get => Math.Abs(m_Leg);
        set => m_Leg = value;
    }

    internal PathItemType ItemType
    {
        get => m_Item;
        set => m_Item = value;
    }

    internal double Value
    {
        get => m_Value;
        set => m_Value = value;
    }

    internal bool IsDistance =>
        m_Unit is not null &&
        m_Item is PathItemType.Distance or PathItemType.Radius;

    /// <summary>
    /// Defines a distance object based on this path item.
    /// </summary>
    /// <returns>The distance corresponding to this item (null if it's not
    /// a distance item)</returns>
    internal Distance GetDistance()
    {
        if (IsDistance)
            return new Distance(m_Value, m_Unit);
        else
            return null;
    }

    /// <summary>
    /// The type of distance unit in effect for the item (should be defined
    /// for all items, even if the value is undefined).
    /// </summary>
    internal DistanceUnit Units => m_Unit;
}