using System.Diagnostics;
using Backsight.Geometry;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="19-SEP-1997" />
/// <summary>
/// The definition of a circle
/// </summary>
/// <seealso cref="Backsight.Geometry.CircleGeometry"/>
class Circle : IMapObject, ICircleGeometry, IFeatureDependent
{
    /// <summary>
    /// The radius of the circle, in meters
    /// </summary>
    private double m_Radius;

    /// <summary>
    /// The center of the circle. This may be quite remote from the main body of the map.
    /// </summary>
    private PointFeature m_Center;

    /// <summary>
    /// The arcs that coincide with the perimeter of this circle.
    /// </summary>
    private readonly List<ArcFeature> m_Arcs;

    /// <summary>
    /// Has this circle been spatially indexed?
    /// </summary>
    private bool IsIndexed { get; set; }

    /// <summary>
    /// Creates a new <c>Circle</c> with the specified center and radius.
    /// </summary>
    /// <param name="center">The point at the center of the circle.</param>
    /// <param name="radius">The radius of the circle, in meters</param>
    internal Circle(PointFeature center, double radius)
    {
        m_Center = center;
        m_Radius = radius;
        m_Arcs = new List<ArcFeature>();
        IsIndexed = false;
    }

    /// <summary>
    /// Value denoting the spatial object type.
    /// </summary>
    public SpatialType SpatialType => SpatialType.Line;

    /// <summary>
    /// The spatial extent of this object.
    /// </summary>
    public IWindow Extent => CircleGeometry.GetExtent(this);

    /// <summary>
    /// Calculates the distance from the perimeter of this circle to the specified position.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public ILength Distance(IPosition point)
    {
        return CircleGeometry.Distance(this, point);
    }

    /// <summary>
    /// Performs any processing that needs to be done just before the position of
    /// a referenced feature is changed.
    /// </summary>
    /// <param name="f">The feature that is about to be moved  - something that
    /// the <c>IFeatureDependent</c> is dependent on (not null).</param>
    /// <param name="ctx">The context in which the move is being made (not null).</param>
    public void OnFeatureMoving(Feature f, UpdateEditingContext ctx)
    {
        if (this.IsIndexed)
        {
            EditingIndex index = f.MapModel.EditingIndex;
            index.RemoveCircle(this);
            this.IsIndexed = false;
        }
    }

    /// <summary>
    /// Records this circle as part of the spatial index in the current map model.
    /// </summary>
    internal void AddToIndex()
    {
        if (!this.IsIndexed)
        {
            EditingIndex index = m_Center.MapModel.EditingIndex;
            index.AddCircle(this);
            this.IsIndexed = true;
        }
    }

    /// <summary>
    /// Adds references to the features that this dependent is dependent on.
    /// </summary>
    internal void AddReferences()
    {
        m_Center.AddReference(this);
    }

    /// <summary>
    /// Obtains the features that are referenced by this operation (including features
    /// that are indirectly referenced by observation classes).
    /// </summary>
    /// <returns>The referenced features (never null, but may be an empty array).</returns>
    public Feature[] GetRequiredFeatures()
    {
        return [m_Center];
    }

    /// <summary>
    /// The position of the center of the circle.
    /// </summary>
    public IPointGeometry Center => m_Center;

    /// <summary>
    /// The radius of the circle, in meters
    /// </summary>
    public double Radius
    {
        get => m_Radius;
        internal set => m_Radius = value;
    }

    /// <summary>
    /// Associates an arc with this circle.
    /// </summary>
    /// <param name="arc">The arc that coincides with the perimeter of this circle (must
    /// already be cross-referenced to this circle)</param>
    /// <exception cref="ArgumentException">If the specified arc does not already
    /// refer to this circle.</exception>
    internal void AddArc(ArcFeature arc)
    {
        if (arc.Circle != this)
            throw new ArgumentException();

        if (!m_Arcs.Contains(arc))
            m_Arcs.Add(arc);
    }

    /// <summary>
    /// The arcs attached to this circle.
    /// </summary>
    internal ArcFeature[] Arcs => m_Arcs.ToArray();

    /// <summary>
    /// Removes an arc from this circle. This might be called if the operation that
    /// created the arc is getting rolled back. Another possible scenario is where
    /// the arc is being moved to coincide with a different circle.
    /// </summary>
    /// <param name="arc">The arc that no longer references this circle.</param>
    /// <returns>True if the specified arc was removed. False if it wasn't referenced.</returns>
    internal bool RemoveArc(ArcFeature arc)
    {
        return m_Arcs.Remove(arc);
    }

    /// <summary>
    /// Returns a point feature that sits at the center of this circle
    /// </summary>
    /// <param name="op">The operation that must be the creator of the centre
    /// point. Specify null (the default) if the creator doesn't matter.</param>
    /// <param name="onlyActive">True (the default) if the point has to be active.
    /// Specify false if inactive points are ok too.</param>
    /// <returns>The centre point (null if no such point).</returns>
    internal PointFeature GetCenter(Operation op, bool onlyActive)
    {
        if (m_Center==null)
            return null;

        if (op==null)
            return m_Center;

        return (m_Center.Creator==op ? m_Center : null);
        /*
         * TODO?
         *
        if (op==null)
            return m_Center.GetPoint(null, null, onlyActive);
        else
            return m_Center.GetPoint(op, onlyActive);
         */
    }

    /// <summary>
    /// The point at the center of this circle.
    /// </summary>
    internal PointFeature CenterPoint => m_Center;

    /// <summary>
    /// The operation that created this circle is the operation that created
    /// the first arc associated with the circle.
    /// </summary>
    public Operation Creator =>
        // Perhaps this should scan the list, since the first element may not
        // necessarily be the earliest edit (alternatively, modify AddArc to
        // ensure edit order is maintained).
        m_Arcs.Count==0 ? null : m_Arcs[0].Creator;

    /// <summary>
    /// Returns the area between a specific quadrant of a circle, and the Y-axis.
    /// By convention, the area of the north-west and south-west quadrants are
    /// returned as negative values, while the other two are positive (assuming
    /// that the circle is to the right of the Y-axis). This same convention is
    /// followed by <c>QuadVertex::GetCurveArea</c>.
    /// </summary>
    /// <param name="quadrant">The desired quadrant</param>
    /// <returns>The area (in square meters on the (projected) ground)</returns>
    internal double GetQuadrantArea(Quadrant quadrant)
    {
        if (quadrant==Quadrant.NE || quadrant==Quadrant.SE)
            return (m_Radius * (m_Center.X + m_Radius * MathConstants.PIDIV4));
        else
            return -(m_Radius * (m_Center.X - m_Radius * MathConstants.PIDIV4));
    }

    /// <summary>
    /// Gets the most easterly position for this circle.
    /// </summary>
    /// <returns>The most easterly position</returns>
    internal IPosition GetEastPoint()
    {
        return new Position(m_Center.X + m_Radius, m_Center.Y);
    }

    /// <summary>
    /// Inserts this circle into the supplied index.
    /// </summary>
    /// <param name="index">The spatial index to add to (should be an instance of
    /// <see cref="EditingIndex"/>)</param>
    internal void AddToIndex(IEditMapIndex index)
    {
        EditingIndex cx = (index as EditingIndex);
        Debug.Assert(cx!=null);
        cx.AddCircle(this);
    }

    /// <summary>
    /// Obtains a list of circles that exist in two lists
    /// </summary>
    /// <param name="a">The first list</param>
    /// <param name="b">The second list</param>
    /// <returns>The circles that exist in both lists (the test is based simply
    /// on reference equality)</returns>
    internal static List<Circle> GetCommonCircles(List<Circle> a, List<Circle> b)
    {
        var result = new List<Circle>();

        foreach (Circle c in a)
        {
            if (b.Contains(c))
                result.Add(c);
        }

        return result;
    }

    /// <summary>
    /// Checks whether this circle is referenced to arcs that terminaye at
    /// a specific point. This excludes arcs that correspond to the whole circle.
    /// </summary>
    /// <param name="p">The point to look for</param>
    /// <returns>True if an incident arc was found.</returns>
    internal bool HasArcsAt(PointFeature p)
    {
        // A location to check has to be specified!
        if (p==null)
            return false;

        // Loop through each arc (including inactive ones).
        foreach (ArcFeature a in m_Arcs)
        {
            // Skip if the arc represents the whole circle.
            if (a.Geometry.IsCircle)
                continue;

            // Check whether either end of the arc coincides with
            // the check location.
            if (p.IsCoincident(a.StartPoint) || p.IsCoincident(a.EndPoint))
                return true;
        }

        return false;
    }

    /// <summary>
    /// The first arc associated with this circle (null if no arcs are currently
    /// associated with this circle).
    /// </summary>
    internal ArcFeature FirstArc => (m_Arcs.Count > 0 ? m_Arcs[0] : null);
}