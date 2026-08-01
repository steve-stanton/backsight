namespace Backsight.Model;

/// <written by="Steve Stanton" on="03-AUG-2007" />
/// <summary>
/// Base class for any sort of line geometry.
/// </summary>
public abstract class LineGeometry : ILineGeometry, IIntersectable, IPersistent
{
    /// <summary>
    /// The start of the connection.
    /// </summary>
    ITerminal m_Start;

    /// <summary>
    /// The end of the connection.
    /// </summary>
    ITerminal m_End;

    /// <summary>
    /// Creates a new <c>LineGeometry</c> using the supplied terminals.
    /// </summary>
    /// <param name="start">The start of the line.</param>
    /// <param name="end">The end of the line.</param>
    protected LineGeometry(ITerminal start, ITerminal end)
    {
        if (start==null || end==null)
            throw new ArgumentNullException("Null terminal for line geometry");

        m_Start = start;
        m_End = end;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGeometry"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    protected LineGeometry(EditDeserializer editDeserializer)
    {
        // When deserializing line geometry (in the context of line features), passing
        // the terminal down via the EditDeserializer is a bit too contrived. Instead,
        // we'll set the terminals back in LineFeature.ReadData.

        m_Start = m_End = null;
    }

    public IPointGeometry Start => m_Start;

    public IPointGeometry End => m_End;

    internal ITerminal StartTerminal
    {
        get => m_Start;
        set => m_Start = value;
    }

    internal ITerminal EndTerminal
    {
        get => m_End;
        set => m_End = value;
    }

    public abstract ILength Length { get; }
    public abstract IWindow Extent { get; }
    public abstract ILength Distance(IPosition point);

    internal abstract uint Intersect(IntersectionResult results);
    internal abstract uint IntersectSegment(IntersectionResult results, ILineSegmentGeometry seg);
    internal abstract uint IntersectMultiSegment(IntersectionResult results, IMultiSegmentGeometry line);
    internal abstract uint IntersectArc(IntersectionResult results, ICircularArcGeometry arc);
    internal abstract uint IntersectCircle(IntersectionResult results, ICircleGeometry circle);

    /// <summary>
    /// Gets the position that is a specific distance from the start of this line.
    /// </summary>
    /// <param name="dist">The distance from the start of the line.</param>
    /// <param name="result">The position found</param>
    /// <returns>True if the distance is somewhere ON the line. False if the distance
    /// was less than zero, or more than the line length (in that case, the position
    /// found corresponds to the corresponding terminal point).</returns>
    internal abstract bool GetPosition(ILength dist, out IPosition pos);

    /// <summary>
    /// Calculates the distance from the start of this line to a specific position (on the map projection)
    /// </summary>
    /// <param name="asFarAs">Position on the line that you want the length to. Specify
    /// null for the length of the whole line.</param>
    /// <returns>The length. Less than zero if a position was specified and it is
    /// not on the line.</returns>
    internal abstract ILength GetLength(IPosition? asFarAs);

    /// <summary>
    /// Gets the orientation point for a line. This is utilized to form
    /// network topology at the ends of a topological line.
    /// </summary>
    /// <param name="fromStart">True if the orientation from the start of the line is
    /// required. False to get the end orientation.</param>
    /// <param name="crvDist">Orientation distance for circular arcs (irrelevant if
    /// the line isn't a circular arc). Default=0.0</param>
    /// <returns>The orientation point.</returns>
    internal abstract IPosition GetOrient(bool fromStart, double crvDist);

    /// <summary>
    /// The geometry that acts as the base for this one.
    /// </summary>
    internal abstract UnsectionedLineGeometry SectionBase { get; }

    /// <summary>
    /// Gets geometric info for this geometry. For use during the formation
    /// of <c>Polygon</c> objects.
    /// </summary>
    /// <param name="window">The window of the geometry</param>
    /// <param name="area">The area (in square meters) between the geometry and the Y-axis.</param>
    /// <param name="length">The length of the geometry (in meters on the (projected) ground).</param>
    internal abstract void GetGeometry(out IWindow win, out double area, out double length);

    /// <summary>
    /// Gets the most easterly position for this line. If more than one position has the
    /// same easting, one of them will be picked arbitrarily.
    /// </summary>
    /// <returns>The most easterly position</returns>
    internal abstract IPosition GetEastPoint();

    /// <summary>
    /// Determines which side of a line a horizontal line segment lies on.
    /// Used in point in polygon.
    /// </summary>
    /// <param name="hr">The horizontal line segment</param>
    /// <returns>Code indicating the position of the horizontal segment with respect to this line.
    /// Side.Left if the horizontal segment is to the left of this line; Side.Right if to the
    /// right of this line; Side.Unknown if the side cannot be determined (this line is
    /// horizontal).
    /// </returns>
    internal abstract Side GetSide(HorizontalRay hr);

    /// <summary>
    /// Cuts back a horizontal line segment to the closest intersection with this line.
    /// Used in point in polygon.
    /// </summary>
    /// <param name="s">Start of horizontal segment.</param>
    /// <param name="e">End of segment (will be modified if segment intersects this line)</param>
    /// <param name="status">Return code indicating whether an error has arisen (returned
    /// as 0 if no error).</param>
    /// <returns>True if the horizontal line was cut back.</returns>
    internal abstract bool GetCloser(IPointGeometry s, ref PointGeometry e, out uint status);

    /// <summary>
    /// Gets the point on this line that is closest to a specified position.
    /// </summary>
    /// <param name="p">The position to search from.</param>
    /// <param name="tol">Maximum distance from line to the search position</param>
    /// <returns>The closest position (null if the line is further away than the specified
    /// max distance)</returns>
    internal abstract IPosition GetClosest(IPointGeometry p, ILength tol);

    /// <summary>
    /// Loads a list of positions with data for this line.
    /// </summary>
    /// <param name="positions">The list to append to</param>
    /// <param name="reverse">Should the data be appended in reverse order?</param>
    /// <param name="wantFirst">Should the first position be appended? (last if <paramref name="reverse"/> is true)</param>
    /// <param name="arcTol">Tolerance for approximating circular arcs (used only if the
    /// geometry is an instance of <see cref="ArcGeometry"/>)</param>
    internal abstract void AppendPositions(List<IPosition> positions, bool reverse, bool wantFirst, ILength arcTol);
    
    /// <summary>
    /// Implements <see cref="IIntersectable"/> by returning <c>this</c> as the
    /// line geometry that's involved.
    /// </summary>
    LineGeometry IIntersectable.LineGeometry => this;

    /// <summary>
    /// Assigns sort values to the supplied intersections (each sort value
    /// indicates the distance from the start of this line).
    /// </summary>
    /// <param name="data">The intersection data to update</param>
    internal abstract void SetSortValues(List<IntersectionData> data);

    /// <summary>
    /// Calculates an angle that is parallel to this line (suitable for adding text)
    /// </summary>
    /// <param name="p">A significant point on the line. In the case of lines
    /// that are multi-segments, the individual line segment that contains this
    /// position should be used to obtain the angle.</param>
    /// <returns>The rotation (in radians, clockwise from horizontal)</returns>
    internal abstract double GetRotation(IPointGeometry p);

    /*
     * For use with display of line annotations
     * 
    /// <summary>
    /// Gets the distance string to annotate a line with.
    /// </summary>
    /// <param name="len">The adjusted length (in meters on the ground).</param>
    /// <param name="dist">The observed length (if any).</param>
    /// <param name="drawObserved">Draw the observed distance?</param>
    /// <returns>The distance string (null if the distance is supposed to be the
    /// observed distance, but there is no observed distance.</returns>
    protected string? GetDistance(double len, Distance? dist, bool drawObserved)
    {
        // Return if we are drawing the observed distance, and we don't have one.
        if (drawObserved && dist is null)
            return null;

        // Get the current display units.
        EditingController ec = EditingController.Current;
        DistanceUnit dunit = ec.DisplayUnit;
        string distr = String.Empty;

        // If we are drawing the observed distance
        if (drawObserved)
        {
            // Display the units only if the distance does not
            // correspond to the current data entry units.

            if (dunit.UnitType == DistanceUnitType.AsEntered)
                dunit = ec.EntryUnit;

            if (!dist.EntryUnit.Equals(dunit))
                distr = dist.Format(true); // with units abbreviation
            else
                distr = dist.Format(false); // no units abbreviation
        }
        else
        {
            // Drawing adjusted distance.

            // If the current display units are "as entered"

            if (dunit.UnitType == DistanceUnitType.AsEntered)
            {
                // What's the current data entry unit?
                DistanceUnit eunit = ec.EntryUnit;

                // Display the units only if the distance does not
                // correspond to the current data entry units.
                if (dist != null)
                {
                    DistanceUnit entryUnit = dist.EntryUnit;
                    if (entryUnit != eunit)
                        distr = entryUnit.Format(len, true); // with abbrev
                    else
                        distr = entryUnit.Format(len, false); // no abbrev
                }
                else
                {
                    // No observed length, so format the actual length using
                    // the current data entry units (no abbreviation).
                    distr = eunit.Format(len, false);
                }
            }
            else
            {
                // Displaying in a specific display unit. Format the
                // result without any units abbreviation.
                distr = dunit.Format(len, false);
            }
        }

        // Never show distances with a leading negative sign.
        if (distr.StartsWith("-"))
            distr = distr.Substring(1);

        return distr;
    }
*/
    
    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public abstract void WriteData(EditSerializer editSerializer);
}