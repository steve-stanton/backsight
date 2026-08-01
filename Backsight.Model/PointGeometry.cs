namespace Backsight.Model;

/// <written by="Steve Stanton" on="18-OCT-2006" />
/// <summary>
/// Implementation of point geometry that consists of a pair of 64-bit integers,
/// where position is expressed to the nearest micron on the ground.
/// </summary>
/// <remarks>
/// Integer (long) values are used largely for historical reasons, since various items
/// of software are coded to accommodate the consequences of roundoff.
/// </remarks>
public class PointGeometry : IPointGeometry
{
    /// <summary>
    /// The easting of the point.
    /// </summary>
    private ILength m_X;

    /// <summary>
    /// The northing of the point.
    /// </summary>
    private ILength m_Y;

    /// <summary>
    /// Creates a new <c>PointGeometry</c> from the supplied position (or casts
    /// the supplied position if it's already an instance of <c>PointGeometry</c>).
    /// </summary>
    /// <param name="p">The position the geometry should correspond to</param>
    /// <returns>A newly created <c>PointGeometry</c> instance, or the supplied
    /// position if it's already an instance of <c>PointGeometry</c></returns>
    public static PointGeometry Create(IPosition p)
    {
        if (p is PointGeometry pg)
            return pg;
        else
            return new PointGeometry(p);
    }

    /// <summary>
    /// Creates a new <c>PointGeometry</c> at the specified position
    /// (rounded to the nearest micron on the ground).
    /// </summary>
    /// <param name="position">The position the geometry should correspond to</param>
    public PointGeometry(IPosition position)
        : this(position.X, position.Y)
    {
    }

    /// <summary>
    /// Creates a new <c>PointGeometry</c> at the specified position.
    /// (rounded to the nearest micron on the ground).
    /// </summary>
    /// <param name="x">The easting of the point, in meters on the ground.</param>
    /// <param name="y">The northing of the point, in meters on the ground.</param>
    public PointGeometry(double x, double y)
    {
        m_X = new MicronValue(x);
        m_Y = new MicronValue(y);
    }

    /// <summary>
    /// Creates a new <c>PointGeometry</c> at the specified position.
    /// </summary>
    /// <param name="xInMicrons">The easting of the point, in microns on the ground.</param>
    /// <param name="yInMicrons">The northing of the point, in microns on the ground.</param>
    public PointGeometry(long xInMicrons, long yInMicrons)
    {
        m_X = new MicronValue(xInMicrons);
        m_Y = new MicronValue(yInMicrons);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public PointGeometry(PointGeometry copy)
        : this(copy.Easting.Microns, copy.Northing.Microns)
    {
    }

    public override string ToString()
    {
        return $"{Y:0.0000}N {X:0.0000}E";
    }

    public double X => m_X.Meters;

    public double Y => m_Y.Meters;

    public bool IsAt(IPosition p, double tol)
    {
        return Position.IsCoincident(this, p, tol);
    }

    public ILength Distance(IPosition point)
    {
        return new Length(BasicGeom.Distance(this, point));
    }

    public IWindow Extent => new Window(this);

    /// <summary>
    /// Is this point at the same position as another point.
    /// </summary>
    /// <param name="p">The point to compare with</param>
    /// <returns>True if the positions are identical (to the nearest micron)</returns>
    public bool IsCoincident(IPointGeometry that)
    {
        return (this.Easting.Microns==that.Easting.Microns &&
                this.Northing.Microns==that.Northing.Microns);
    }

    /// <summary>
    /// Checks if a position is coincident with a line segment
    /// </summary>
    /// <param name="p">The position to test</param>
    /// <param name="start">The start of the segment.</param>
    /// <param name="end">The end of the segment.</param>
    /// <param name="tolsq">The tolerance (squared) to use. Default is XYTOLSQ.</param>
    /// <returns>True if the test position lies somewhere along the segment.</returns>
    public static bool IsCoincidentWith(IPointGeometry p, IPointGeometry start, IPointGeometry end, double tolsq)
    {
        // Check whether there is exact coincidence at either end.
        if (p.IsCoincident(start) || p.IsCoincident(end))
            return true;

        // Get the distance squared of a perpendicular dropped from
        // the test position to the segment (or the closest end if the
        // perpendicular does not fall ON the segment).
        return (BasicGeom.DistanceSquared(p.X, p.Y, start.X, start.Y, end.X, end.Y) < tolsq);
    }

    public ILength Easting => m_X;

    public ILength Northing => m_Y;
}