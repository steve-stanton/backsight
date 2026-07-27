using Backsight.Model.Operations;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="26-MAR-2008"/>
/// <summary>
/// Information about a connection path. This acts as a helper for the <see cref="PathForm"/> dialog.
/// It's sort of a half-way between the fairly unstructured world of the dialog class, and the
/// regimented world of the operation class.
/// </summary>
class PathInfo
{
    /// <summary>
    /// The point where the path starts.
    /// </summary>
    readonly PointFeature m_From;

    /// <summary>
    /// The point where the path ends.
    /// </summary>
    readonly PointFeature m_To;

    /// <summary>
    /// The legs that make up the path
    /// </summary>
    readonly Leg[] m_Legs;

    /// <summary>
    /// Has the <see cref="Adjust"/> method been successfully called. If true, the values
    /// for <see cref="m_Rotation"/> and <see cref="m_ScaleFactor"/> are meaningful.
    /// </summary>
    bool m_IsAdjusted;

    /// <summary>
    /// Rotation for path (in radians)
    /// </summary>
    double m_Rotation;

    /// <summary>
    /// Scaling to apply to path distances
    /// </summary>
    double m_ScaleFactor;

    /// <summary>
    /// The precision denominator (0 for a perfect match).
    /// </summary>
    double m_Precision;

    /// <summary>
    /// Creates a new <c>PathInfo</c> object
    /// </summary>
    /// <param name="from">The point where the path starts.</param>
    /// <param name="to">The point where the path ends.</param>
    internal PathInfo(PointFeature from, PointFeature to, Leg[] legs)
    {
        m_From = from;
        m_To = to;
        m_Legs = legs;

        m_IsAdjusted = false;
        m_Rotation = 0.0;
        m_ScaleFactor = 0.0;
    }

    /// <summary>
    /// Create a new <c>PathInfo</c> object that corresponds to a previously
    /// saved connection path. For consistency with the other constructor, this
    /// does not attempt to adjust the path (the Rotation and ScaleFactory properties
    /// will retain zero values unless a call is made to Adjust).
    /// </summary>
    /// <param name="pop">The saved connection path</param>
    internal PathInfo(PathOperation pop)
    {
        m_From = pop.StartPoint;
        m_To = pop.EndPoint;
        m_Legs = pop.GetLegs();

        m_IsAdjusted = false;
        m_Rotation = 0.0;
        m_ScaleFactor = 0.0;
        m_Precision = 0.0;
    }

    /// <summary>
    /// The point where the path starts.
    /// </summary>
    internal PointFeature FromPoint => m_From;

    /// <summary>
    /// The point where the path ends.
    /// </summary>
    internal PointFeature ToPoint => m_To;

    /// <summary>
    /// Ensures the Adjust method has been called.
    /// </summary>
    void EnsureAdjusted()
    {
        if (!m_IsAdjusted)
            Adjust(out _, out _, out _, out _, out _, out _);
    }

    /// <summary>
    /// Adjusts the path (Helmert adjustment).
    /// </summary>
    /// <param name="dN">Misclosure in northing.</param>
    /// <param name="dE">Misclosure in easting.</param>
    /// <param name="precision">Precision denominator (zero if no adjustment needed).</param>
    /// <param name="length">Total observed length.</param>
    /// <param name="rotation">The clockwise rotation to apply (in radians).</param>
    /// <param name="sfac">The scaling factor to apply.</param>
    void Adjust(out double dN, out double dE, out double precision, out double length,
        out double rotation, out double sfac)
    {
        dN = dE = precision = length = rotation = 0.0;
        sfac = 1.0;

        // Initialize position to the start of the path, corresponding to the initial
        // un-adjusted end point.
        IPosition gotend = new Position(m_From);

        // Initial bearing is due north.
        double bearing = 0.0;

        // Go through each leg, updating the end position, and getting
        // the total path length.
        foreach (Leg leg in m_Legs)
        {
            length += leg.Length.Meters;
            leg.Project(ref gotend, ref bearing, sfac);
        }

        // Get the bearing and distance of the end point we ended up with.
        double gotbear = BasicGeom.BearingInRadians(m_From, gotend);
        double gotdist = BasicGeom.Distance(m_From, gotend);

        // Get the bearing and distance we want.
        double wantbear = BasicGeom.BearingInRadians(m_From, m_To);
        double wantdist = BasicGeom.Distance(m_From, m_To);

        // Figure out the rotation.
        rotation = wantbear-gotbear;

        // Rotate the end point we got.
        gotend = BasicGeom.Rotate(m_From, gotend, new RadianValue(rotation));

        // Calculate the line scale factor.
        double linefac = m_From.MapModel.SpatialSystem.GetLineScaleFactor(m_From, gotend);

        // Figure out where the rotated end point ends up when we apply the line scale factor.
        gotend = BasicGeom.Polar(m_From, wantbear, gotdist*linefac);

        // What misclosure do we have?
        dN = gotend.Y - m_To.Y;
        dE = gotend.X - m_To.X;
        double delta = Math.Sqrt(dN*dN + dE*dE);

        // What's the precision denominator (use a value of 0 to denote an exact match).
        if (delta > MathConstants.TINY)
            precision = wantdist/delta;
        else
            precision = 0.0;

        // Figure out the scale factor for the adjustment (use a value of 0 if the start and end
        // points are coincident). The distances here have NOT been adjusted for the line scale factor.
        if (gotdist > MathConstants.TINY)
            sfac = wantdist/gotdist;
        else
            sfac = 0.0;

        // Remember the rotation and scaling factor
        m_IsAdjusted = true;
        m_Rotation = rotation;
        m_ScaleFactor = sfac;
        m_Precision = precision;
    }

    /// <summary>
    /// Rotation for path (in radians)
    /// </summary>
    internal double RotationInRadians
    {
        get
        {
            EnsureAdjusted();
            return m_Rotation;
        }
    }

    /// <summary>
    /// Scaling to apply to path distances
    /// </summary>
    internal double ScaleFactor
    {
        get
        {
            EnsureAdjusted();
            return m_ScaleFactor;
        }
    }

    /// <summary>
    /// The precision denominator (0 for a perfect match).
    /// </summary>
    internal double Precision
    {
        get
        {
            EnsureAdjusted();
            return m_Precision;
        }
    }

    /// <summary>
    /// Obtains line sections for a specific face in this path.
    /// </summary>
    /// <param name="face">The face of interest</param>
    /// <returns>The corresponding sections</returns>
    internal ILineGeometry[] GetSections(LegFace face)
    {
        EnsureAdjusted();

        // Initialize position to the start of the path.
        IPosition p = new Position(m_From);

        // Initial bearing is whatever the rotation is.
        double bearing = m_Rotation;

        // Get the position at the start of the required leg.
        foreach (Leg leg in m_Legs)
        {
            if (leg == face.Leg)
                break;

            leg.Project(ref p, ref bearing, m_ScaleFactor);
        }

        // We've now got the position at the start of the required leg, and the bearing of the previous leg.
        // If the leg we actually want if a straight leg (or an extra leg layered on a straight), add on any
        // initial angle.
        if (face.Leg is StraightLeg sLeg)
            bearing = sLeg.AddStartAngle(bearing);

        return face.Leg.GetSpanSections(p, bearing, m_ScaleFactor, face.Spans);
    }
}