using System;
using Backsight.Model;

namespace Backsight.Map.Editor.ViewModels;

/// <written by="Steve Stanton" on="22-NOV-2007" />
/// <summary>
/// Wrapper on an instance of <see cref="IDivider"/> that implements <see cref="IMapObject"/>.
/// This makes it possible to include dividers in things like a <see cref="Selection"/>.
/// </summary>
internal class DividerObject : IMapObject
{
    /// <summary>
    /// The divider that this instance wraps.
    /// </summary>
    readonly IDivider m_Divider;

    /// <summary>
    /// The geometry for the divider. While this is also available via <c>m_Divider</c>, this can
    /// hide the fact that getting the geometry may involve quite a number of steps (e.g. getting
    /// the geometry for a divider that is a section on a multi-segment). Since the intention is
    /// that the <c>DividerObject</c> class will only be used for special handling of dividers,
    /// it seems reasonable to cache the geometry here.
    /// </summary>
    readonly LineGeometry m_Geom;

    /// <summary>
    /// Creates a new <c>DividerObject</c> that wraps the supplied divider.
    /// </summary>
    /// <param name="d">The divider to wrap</param>
    internal DividerObject(IDivider d)
    {
        m_Divider = d ?? throw new ArgumentNullException();
        m_Geom = m_Divider.LineGeometry;
    }

    /// <inheritdoc cref="IMapObject.SpatialType"/>
    public SpatialType SpatialType => SpatialType.Line;

    /*
    /// <summary>
    /// Draws this object on the specified display
    /// </summary>
    /// <param name="display">The display to draw to</param>
    /// <param name="style">The drawing style</param>
    public void Draw(IMapDisplay mapDisplay)
    {
        m_Geom.Draw(mapDisplay);
    }
*/
    /// <inheritdoc cref="IMapObject.Extent"/>
    public IWindow Extent => m_Geom.Extent;

    /// <inheritdoc cref="IMapObject.Distance"/>
    public ILength Distance(IPosition point)
    {
        return m_Geom.Distance(point);
    }

    /// <summary>
    /// The geometry for this divider.
    /// </summary>
    internal LineGeometry Geometry => m_Geom;
    
    /// <summary>
    /// The divider that this instance wraps.
    /// </summary>
    internal IDivider Divider => m_Divider;

    /*
    /// <summary>
    /// The derived entity type associated with this divider (in normal
    /// situations, this corresponds to the entity type of the associated
    /// line feature);
    /// </summary>
    public string EntityTypeName
    {
        get
        {
            return EntityUtil.GetDerivedType(m_Divider,
                EditingController.Current.ActiveLayer);
        }
    }
    */
}