using System.Diagnostics;
using Backsight.Model.Operations;
using Backsight.Geometry;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="21-JAN-1998" was="CeStraightLeg" />
/// <summary>
/// A straight leg in a connection path.
/// </summary>
class StraightLeg : Leg
{
    /// <summary>
    /// Angle at the start of the leg (signed). 
    /// </summary>
    double m_StartAngle;

    /// <summary>
    /// Is the angle at the start of this a deflection?
    /// </summary>
    bool m_IsDeflection;

    /// <summary>
    /// Creates a new <c>StraightLeg</c>
    /// </summary>
    /// <param name="nspan">The number of spans for the leg.</param>
    internal StraightLeg(int nspan)
        : base(nspan)
    {
        m_StartAngle = 0.0;
        m_IsDeflection = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StraightLeg"/> class that corresponds to
    /// the end of a face on another leg (for use when breaking a leg).
    /// </summary>
    /// <param name="face">The face on the other leg</param>
    /// <param name="startIndex">The array index of the first span that should be copied.</param>
    StraightLeg(LegFace face, int startIndex)
        : base(face, startIndex)
    {
        // Stick in a (clockwise) angle of 180 degrees.
        m_StartAngle = Math.PI;
        m_IsDeflection = false;
    }

    /// <summary>
    /// Angle at the start of the leg (signed). 
    /// </summary>
    internal double StartAngle
    {
        get => m_StartAngle;
        set => m_StartAngle = value;
    }

    internal override Circle Circle => null;

    internal override IPosition Center => null;

    /// <summary>
    /// The total observed length of this leg
    /// </summary>
    internal override ILength Length => new Length(PrimaryFace.GetTotal());

    /// <summary>
    /// Given the position of the start of this leg, along with an initial bearing,
    /// project the end of the leg, along with an exit bearing.
    /// </summary>
    /// <param name="pos">The position at the start of the leg.</param>
    /// <param name="bearing">The initial bearing (e.g. if the previous leg was also
    /// a straight leg from A to B, the bearing is from A through B).</param>
    /// <param name="sfac">Scaling factor to apply. Default=1.0</param>
    internal override void Project(ref IPosition pos, ref double bearing, double sfac)
    {
        // Add on any initial angle
        bearing = AddStartAngle(bearing);

        // Get the total length of the leg.
        double length = PrimaryFace.GetTotal() * sfac;

        // Figure out shifts.
        double dE = length * Math.Sin(bearing);
        double dN = length * Math.Cos(bearing);

        // Define the end position.
        pos = new Position(pos.X + dE, pos.Y + dN);
    }

    /// <summary>
    /// Obtains the geometry for spans along this leg.
    /// </summary>
    /// <param name="pos">The position for the start of the leg.
    /// <param name="bearing">The bearing of the leg.</param>
    /// <param name="sfac">Scale factor to apply to distances.</param>
    /// <param name="spans">Information for the spans coinciding with this leg.</param>
    /// <returns>The sections along this leg</returns>
    internal override ILineGeometry[] GetSpanSections(IPosition pos, double bearing, double sfac, SpanInfo[] spans)
    {
        var result = new ILineGeometry[spans.Length];

        // A leg with just one span, but no observed distance is due to the fact that the Leg constructor
        // that accepts a span count will always produce an array with at least one span (this covers cul-de-sacs
        // defined only with a central angle). May be better to handle it there.
        if (spans.Length == 1 && spans[0].ObservedDistance == null)
        {
            result[0] = new LineSegmentGeometry(pos, pos);
            return result;
        }

        double sinBearing = Math.Sin(bearing);
        double cosBearing = Math.Cos(bearing);

        IPosition sPos = pos;
        IPosition ePos = null;

        double edist = 0.0;

        for (int i = 0; i < result.Length; i++, sPos=ePos)
        {
            edist += (spans[i].ObservedDistance.Meters * sfac);
            ePos = new Position(pos.X + (edist * sinBearing), pos.Y + (edist * cosBearing));
            result[i] = new LineSegmentGeometry(sPos, ePos);
        }

        return result;
    }

    /// <summary>
    /// Obtains the geometry for spans along an alternate face attached to this leg.
    /// </summary>
    /// <param name="start">The position for the start of the leg.
    /// <param name="end">The position for the end of the leg.</param>
    /// <param name="spans">Information for the spans coinciding with this leg.</param>
    /// <returns>The sections along this leg</returns>
    internal override ILineGeometry[] GetSpanSections(IPosition start, IPosition end, SpanInfo[] spans)
    {
        Debug.Assert(AlternateFace != null);

        // Get the desired length (in meters on the ground)
        double len = Geom.Distance(start, end);

        // Get the observed length (in meters on the ground)
        double obs = AlternateFace.GetTotal();

        // Get the adjustment factor for stretching-compressing the observed distances.
        double factor = len / obs;

        // Get the bearing of the line.
        double bearing = Geom.BearingInRadians(start, end);

        return GetSpanSections(start, bearing, factor, spans);
    }

    /// <summary>
    /// Creates a line feature that corresponds to one of the spans on this leg.
    /// </summary>
    /// <param name="ff">The factory for creating new spatial features</param>
    /// <param name="itemName">The name for the item involved</param>
    /// <param name="from">The point at the start of the line (not null).</param>
    /// <param name="to">The point at the end of the line (not null).</param>
    /// <returns>The created line (never null)</returns>
    internal override LineFeature CreateLine(FeatureFactory ff, string itemName, PointFeature from, PointFeature to)
    {
        return ff.CreateSegmentLineFeature(itemName, from, to);
    }

    /// <summary>
    /// Adds on any angle at the start of this leg.
    /// </summary>
    /// <param name="bearing">The bearing at the end of the preceding leg.</param>
    /// <returns>The bearing of this leg (in radians)</returns>
    internal double AddStartAngle(double bearing)
    {
        if (Math.Abs(m_StartAngle) < MathConstants.TINY)
            return bearing;

        if (m_IsDeflection)
            return bearing + m_StartAngle;
        else
            return bearing + m_StartAngle - Math.PI;
    }

    /// <summary>
    /// Rollforward this leg.
    /// </summary>
    /// <param name="insert">The point of the end of any new insert that
    /// immediately precedes this leg. This will be updated if this leg also
    /// ends with a new insert (if not, it will be returned as a null value).</param>
    /// <param name="op">The connection path that this leg belongs to.</param>
    /// <param name="terminal">The position for the start of the leg. Updated to be
    /// the position for the end of the leg.</param>
    /// <param name="bearing">The bearing at the end of the previous leg.
    /// Updated for this leg.</param>
    /// <param name="sfac">Scale factor to apply to distances.</param>
    /// <returns></returns>
    internal override bool Rollforward(ref PointFeature insert, PathOperation op,
        ref IPosition terminal, ref double bearing, double sfac)
    {
        throw new NotImplementedException();
        /*
        // Add on any initial angle (it may be a deflection).
        if (Math.Abs(m_StartAngle) > MathConstants.TINY)
        {
            if (m_IsDeflection)
                bearing += m_StartAngle;
            else
                bearing += (m_StartAngle-Math.PI);
        }

        // Create a straight span
        StraightSpan span = new StraightSpan(this, terminal, bearing, sfac);

        // The very end of a connection path should never be moved.
        PointFeature veryEnd = op.EndPoint;

        // Create list for holding any newly created points
        List<PointFeature> createdPoints = new List<PointFeature>();

        int nspan = this.Count;
        for (int i=0; i<nspan; i++)
        {
            // Get info for the current span (this defines the
            // adjusted start and end positions, among other things).
            span.Get(i);

            // If we've got a newly inserted span
            if (IsNewSpan(i))
            {
                bool isLast = (i==(nspan-1) && op.IsLastLeg(this));
                LineFeature newLine = SaveInsert(span, i, op, isLast);
                AddNewSpan(i, newLine);
                insert = newLine.EndPoint;
            }
            else
            {
                // See if the span previously had a saved feature.
                Feature old = GetFeature(i);
                if (old!=null)
                    SaveSpan(span, op, createdPoints, insert, old, veryEnd, uc);
                else
                {
                    Feature feat = SaveSpan(span, op, createdPoints, insert, null, veryEnd, uc);
                    SetFeature(i, feat);
                }

                // That wasn't an insert.
                insert = null;
            }
        }

        // Return the end position of the last span.
        terminal = span.End;
        return true;
         */
    }

    /// <summary>
    /// Records a deflection angle at the start of this leg. There must be a preceding
    /// leg for this to make any sense.
    /// </summary>
    /// <param name="value">The deflection, in radians. Negated values go
    /// counter-clockwise.</param>
    internal void SetDeflection(double value)
    {
        // Record the deflection angle at the start of this leg.
        m_StartAngle = value;

        // Remember that it's a deflection (as opposed to a regular angle).
        m_IsDeflection = true;
        //base.SetDeflection(true);
    }

    /// <summary>
    /// Breaks this leg into two legs. The break must leave at least
    /// one distance in each of the resultant legs.
    /// </summary>
    /// <param name="index">The index of the span that should be at the
    /// start of the extra leg.</param>
    /// <returns>The extra leg (at the end of the original leg).</returns>
    internal StraightLeg Break(int index)
    {
        if (this.AlternateFace != null)
            throw new InvalidOperationException("Cannot break a staggered leg");

        // Can't break right at the start or end.
        int nTotal = PrimaryFace.Count;
        if (index <= 0 || index >= nTotal)
            return null;

        // Create a new straight leg with the right number of spans.
        StraightLeg newLeg = new StraightLeg(PrimaryFace, index);

        // Retain the spans prior to that
        PrimaryFace.TruncateLeg(index);

        return newLeg;
    }

    /// <summary>
    /// Is the angle at the start of this a deflection?
    /// </summary>
    internal bool IsDeflection => m_IsDeflection;
}