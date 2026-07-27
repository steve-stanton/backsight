namespace Backsight.Model.Index.Rectangle;

/// <written by="Steve Stanton" on="15-DEC-2006" />
/// <summary>
/// An entry in the spatial index, consisting of a reference to a spatial
/// object, as well as a reference to an object that represents its extent.
/// This is considered useful because it may be relatively expensive to
/// calculate the spatial object's extent (given that this will likely
/// be done very repetitively while working with the spatial index).
/// </summary>
class Item
{
    /// <summary>
    /// The spatial object of interest
    /// </summary>
    private readonly IMapObject m_Object;

    /// <summary>
    /// The extent of the spatial object (expressed in a form that
    /// is easily accessible to the spatial index)
    /// </summary>
    private readonly Extent m_Window;

    /// <summary>
    /// Creates a new <c>Item</c> for a spatial object. After
    /// creating an instance, the expectation is that you will add
    /// the item into a spatial index.
    /// </summary>
    /// <param name="o">The spatial object that you intend to add
    /// to a spatial index</param>
    internal Item(IMapObject o)
    {
        m_Object = o;
        m_Window = new Extent(o.Extent);
    }

    /// <summary>
    /// The extent of the spatial object, expressed in a form that
    /// can be easily worked with by the spatial index. For performance
    /// reasons, use this property instead of a call to <c>Item.Extent</c>.
    /// </summary>
    internal Extent Window => m_Window;

    /// <summary>
    /// The spatial object associated with this item. Note that if you need
    /// to obtain the spatial extent of the object, it is preferable to
    /// use the <c>Window</c> property (since a call to <c>Item.Extent</c>
    /// is not necessarily cheap).
    /// </summary>
    internal IMapObject MapObject => m_Object;
}