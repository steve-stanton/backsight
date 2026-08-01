namespace Backsight.Model;

/// <written by="Steve Stanton" on="21-AUG-2007" />
/// <summary>
/// A series of connected dividers, forming the edge of a polygon.
/// </summary>
public abstract class Ring : RingMetrics, IMapObject
{
    /// <summary>
    /// Creates a new <c>Ring</c> that's either a <see cref="Polygon"/> or an <see cref="Island"/>.
    /// </summary>
    /// <param name="mapModel">The model that the ring will be added to.</param>
    /// <param name="edge">The faces that define the perimeter of the ring</param>
    internal static Ring Create(CadastralMapModel mapModel, List<Face> edge)
    {
        RingMetrics rm = new RingMetrics(edge);

        if (rm.SignedArea <= 0.0)
            return new Island(mapModel, rm, edge);
        else
            return new Polygon(mapModel, rm, edge);
    }

    /// <summary>
    /// The model that this ring is part of.
    /// </summary>
    readonly CadastralMapModel m_MapModel;
    
    /// <summary>
    /// The dividers defining the edge of the ring.
    /// </summary>
    readonly IDivider[] m_Edge;

    /// <summary>
    /// Flag bits
    /// </summary>
    RingFlag m_Flag;

    /// <summary>
    /// Constructor used by <c>Ring.Create</c>.
    /// </summary>
    /// <param name="mapModel">The model that the ring will be added to.</param>
    /// <param name="metrics">The metrics for this ring.</param>   
    /// <param name="edge">The dividers that define the perimeter of the ring.</param>
    protected Ring(CadastralMapModel mapModel, RingMetrics metrics, List<Face> edge)
        : base(metrics)
    {
        m_MapModel = mapModel ?? throw new ArgumentNullException();
        m_Edge = new IDivider[edge.Count];
        m_Flag = 0;

        for (int i=0; i<m_Edge.Length; i++)
        {
            Face face = edge[i];
            IDivider d = face.Divider;

            // Check for a divider that's already marked as being totally built.
            if (Topology.IsBuilt(d))
                throw new Exception("Polygon - Wrong build status for component divider");

            // Remember the divider and update polygon geometry
            m_Edge[i] = d;

            if (face.IsLeft)
                d.Left = this;
            else
                d.Right = this;
        }
    }
    
    /// <summary>
    /// The model that this ring is part of.
    /// </summary>
    internal CadastralMapModel MapModel => m_MapModel;

    /// <summary>
    /// The area of this ring, excluding any islands. This version is suitable only for
    /// rings that are instances of <c>Island</c> (the <c>Polygon</c> class overrides).
    /// </summary>
    internal virtual double AreaExcludingIslands => Area;

    /// <summary>
    /// The dividers defining the edge of the ring.
    /// </summary>
    internal IDivider[] Edge => m_Edge;

    /// <summary>
    /// Returns the user-perceived polygon associated with this ring. For instances of
    /// <see cref="Polygon"/>, it's just a reference to <c>this</c>. For instances of
    /// <see cref="Island"/>, it's a reference to the polygon that encloses the island.
    /// </summary>
    internal abstract Polygon? RealPolygon { get; }

    /// <summary>
    /// The spatial type is <c>Polygon</c> for both the <see cref="Polygon"/>
    /// and the <see cref="Island"/> classes.
    /// </summary>
    public SpatialType SpatialType => SpatialType.Polygon;

    /// <summary>
    /// The shortest distance from the specified position to the perimeter of this ring.
    /// </summary>
    /// <param name="point">The position of interest</param>
    /// <returns>The shortest distance to a component divider</returns>
    public ILength Distance(IPosition point)
    {
        // I guess the sensible thing to do would be to find the shortest distance
        // to the component dividers. Not sure whether Polygon class would override
        // to also consider island dividers.

        throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>
    /// Is a flag bit set?
    /// </summary>
    /// <param name="flag">The flag(s) to check for (may be a combination of more
    /// than one flag)</param>
    /// <returns>True if any of the supplied flag bits are set</returns>
    protected bool IsFlagSet(RingFlag flag)
    {
        return ((m_Flag & flag)!=0);
    }

    /// <summary>
    /// Sets flag bit(s)
    /// </summary>
    /// <param name="flag">The flag bit(s) to set</param>
    /// <param name="setting">True to set, false to clear</param>
    protected void SetFlag(RingFlag flag, bool setting)
    {
        if (setting)
            m_Flag |= flag;
        else
            m_Flag &= (~flag);
    }

    /// <summary>
    /// Has this ring been spatially indexed?
    /// </summary>
    internal bool IsIndexed
    {
        get => IsFlagSet(RingFlag.Indexed);
        set => SetFlag(RingFlag.Indexed, value);
    }

    /// <summary>
    /// Has this ring been marked for deletion? Marking a ring for deletion
    /// does not change the value of the <see cref="IsIndexed"/> property, although
    /// it is likely that any index entry will be removed shortly thereafter.
    /// </summary>
    internal bool IsDeleted
    {
        get => IsFlagSet(RingFlag.Deleted);
        set => SetFlag(RingFlag.Deleted, value);
    }

    /// <summary>
    /// Checks if this ring encloses a specific position. 
    /// </summary>
    /// <param name="p">The position to check</param>
    /// <returns>True is this ring encloses the position. False if the position
    /// is outside, or ON the edge.</returns>
    /// <remarks>Do not override. Any island polygons are intentionally ignored.</remarks>
    internal bool IsRingEnclosing(IPosition p)
    {
        // Return if the position doesn't fall within the window of this polygon
        if (!this.Extent.IsOverlap(p))
            return false;

        // Set up a search line from the test position to the edge of this
        // polygon's window.
        IPosition east = new Position(this.Extent.Max.X, p.Y);

        // Round off to the nearest micron on the ground
        PointGeometry vs = new PointGeometry(p);
        PointGeometry ve = new PointGeometry(east);

        IDivider closest = null;

        // For each divider we have in the list, locate the closest
        // point of intersection with the search line.
        foreach (IDivider d in m_Edge)
        {
            uint error;
            if (d.LineGeometry.GetCloser(vs, ref ve, out error))
                closest = d;

            // Return if the vertex coincides with the closest point
            // on the divider (point exactly on the edge of this polygon).
            if (error!=0 || vs.IsCoincident(ve))
                return false;
        }

        // We SHOULD have got something.
        if (closest==null)
            return false;

        // The point can't be inside if we're dealing with an island, and the
        // divider we've found is either a bridge, or a dangle that radiates outwards
        // from the island (this assumes that points exactly coincident with the edge
        // can be treated as "outside").
        if (this is Island && closest.Left==closest.Right)
            return false;

        // Which side of the divider does the directed line hit?
        IDivider sideDivider;
        Side side = Topology.GetSide(closest, vs, ve, out sideDivider);

        // Check if we're hitting the divider from the inside. If this
        // ring is an island, things are backwards.
        if (this is Island)
        {
            if (side==Side.Right && sideDivider.Left==this)
                return true;
            if (side==Side.Left && sideDivider.Right==this)
                return true;
        }
        else
        {
            if (side==Side.Left && sideDivider.Left==this)
                return true;
            if (side==Side.Right && sideDivider.Right==this)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Inserts this polygon into the supplied index, so long as it's not already
    /// marked as indexed. Then marks it as indexed.
    /// </summary>
    /// <param name="index">The spatial index to add to</param>
    /// <returns>True if entry added to index. False if this ring is already marked
    /// as indexed.</returns>
    internal bool AddToIndex(EditingIndex index)
    {
        if (IsIndexed)
            return false;

        index.Add(this);
        IsIndexed = true;
        return true;
    }

    /// <summary>
    /// The perimeter of this polygon as a single array of positions.
    /// </summary>
    /// <param name="curvetol">Approximation tolerance for any circular arcs along
    /// the edge of this ring.</param>
    /// <returns></returns>
    internal IPosition[] GetOutline(ILength curvetol)
    {
        return GetOutline(curvetol, this, m_Edge);
    }

    /// <summary>
    /// The perimeter of a polygon as a single array of positions.
    /// </summary>
    /// <param name="curvetol">Approximation tolerance for any circular arcs along
    /// the edge of the ring.</param>
    /// <param name="ring">The ring of interest that the <paramref name="edge"/> contains</param>
    /// <param name="edge">The dividers to consider</param>
    /// <returns></returns>
    internal static IPosition[] GetOutline(ILength curvetol, Ring ring, IDivider[] edge)
    {
        List<IPosition> pts = new List<IPosition>(1000);

        // When doing the 1st arc, we need to utilize the very first position
        bool isFirst = true;

        // Loop through each divider in the ring to create a list of
        // positions defining the polygon...
        foreach (IDivider d in edge)
        {
            // Skip divider lines if they have the same ring on both sides. This is
            // meant to exclude dividers that radiate out from islands. Note that
            // potential bridges towards the interior of polygons are expected to
            // be weeded out prior to call.
            if (d.Left==ring && d.Right==ring)
                continue;

            // See which way the positions should be arranged.
            bool reverse = (d.Right==ring ? false : true);

            // Get the geometric primitive for the divider
            LineGeometry line = d.LineGeometry;
            line.AppendPositions(pts, reverse, isFirst, curvetol);
            isFirst = false;
        }

        return pts.ToArray();
    }

    /// <summary>
    /// Ensures this ring is clean after some sort of edit. If this ring has been marked
    /// for deletion, references from component dividers will be nulled out.
    /// <para/>
    /// Any override should first do it's stuff, then call this implementation.
    /// </summary>
    internal virtual void Clean()
    {
        // Return if this ring hasn't been marked for deletion
        if (!IsDeleted)
            return;

        // Ensure component dividers no longer refer to this ring.
        foreach (IDivider d in m_Edge)
        {
            if (d.Left == this)
                d.Left = null;

            if (d.Right == this)
                d.Right = null;
        }
    }

    /// <summary>
    /// Gets the most easterly point for this ring. If more than one point has the
    /// same easting, one of them will be picked arbitrarily.
    /// </summary>
    /// <returns>The east point of the area.</returns>
    internal IPosition GetEastPoint()
    {
        IPosition mosteast = null;

        foreach (IDivider d in m_Edge)
        {
            IPosition east = d.LineGeometry.GetEastPoint();
            if (mosteast==null || east.X > mosteast.X)
                mosteast = east;
        }

        return mosteast;
    }

    /// <summary>
    /// Calculates the sum of the dividers that make up the edge of this ring.
    /// </summary>
    /// <returns></returns>
    internal ILength GetEdgeLength()
    {
        double total = 0.0;

        foreach (IDivider d in m_Edge)
            total += d.LineGeometry.Length.Meters;

        return new Length(total);
    }
}