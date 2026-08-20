using Backsight.Environment;
using Backsight.Model.Observations;

namespace Backsight.Model;

class ArcFeature : LineFeature
{
    /// <summary>
    /// Creates a new <c>ArcFeature</c>
    /// </summary>
    /// <param name="creator">The operation that created the feature (not null)</param>
    /// <param name="id">The internal ID of this feature within the project that created it.</param>
    /// <param name="e">The entity type for the feature.</param>
    /// <param name="c">The circle the arc coincides with</param>
    /// <param name="bc">The point at the start of the arc</param>
    /// <param name="ec">The point at the end of the arc</param>
    /// <param name="isClockwise">True if the arc is directed clockwise from start to end</param>
    internal ArcFeature(Operation creator, InternalIdValue id, IEntity e, Circle c, PointFeature bc, PointFeature ec, bool isClockwise)
        : base(creator, id, e, bc, ec, new ArcGeometry(c, bc, ec, isClockwise))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArcFeature"/> class, and records it
    /// as part of the map model.
    /// </summary>
    /// <param name="f">Basic information about the feature (not null).</param>
    /// <param name="g">The geometry for the line (could be null, although this is only really
    /// expected during deserialization)</param>
    /// <exception cref="ArgumentNullException">If <paramref name="f"/> is null.</exception>
    internal ArcFeature(IFeature f, PointFeature bc, PointFeature ec, ArcGeometry g, bool isTopological)
        : base(f, bc, ec, g, isTopological)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArcFeature"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal ArcFeature(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
    }

    /// <summary>
    /// Modifies any referenced features by cross-referencing them to the line
    /// that contains this geometry.
    /// </summary>
    /// <param name="container">The line that refers to this geometry.</param>
    internal override void AddReferences()
    {
        // The circle may not be known at this stage (this method is called
        // by the LineFeature constructor, and the geometry may be undefined
        // at that stage -- come to think of it, I believe more recent logic
        // means that the geometry will NEVER be known when a LineFeature is
        // created. The circle->arc cross reference needs to be made when the
        // arc geometry is defined.

        Circle c = this.Circle;
        if (c!=null)
            c.AddArc(this);

        base.AddReferences();
    }

    /// <summary>
    /// Obtains the features that are referenced by this operation (including features
    /// that are indirectly referenced by observation classes).
    /// </summary>
    /// <returns>The referenced features (never null, but may be an empty array).</returns>
    public override Feature[] GetRequiredFeatures()
    {
        List<Feature> result = new List<Feature>(base.GetRequiredFeatures());

        Circle c = this.Circle;
        if (c!=null)
            result.AddRange(c.GetRequiredFeatures()); // the center point

        return result.ToArray();
    }

    /// <summary>
    /// The geometry for this feature (just casts the <c>LineGeometry</c> property).
    /// </summary>
    internal ArcGeometry Geometry
    {
        get => (ArcGeometry)LineGeometry;

        set
        {
            if (value == null)
                throw new ArgumentNullException();

            base.LineGeometry = value;

            // Ensure the circle is associated with this arc
            this.Circle.AddArc(this);
        }
    }

    /// <summary>
    /// The circle the arc falls on.
    /// </summary>
    internal override Circle? Circle => Geometry?.Circle as Circle;

    /// <summary>
    /// Is the geometry for this arc directed clockwise from BC to EC?
    /// </summary>
    internal bool IsClockwise => Geometry.IsClockwise;

    /// <summary>
    /// Gets the exact positions for the BC or EC. The "exact" positions are obtained
    /// by projecting the stored BC/EC to a position that is exactly consistent with the
    /// definition of the underlying circle. Typically, the shifts will be less than 1
    /// micron on the ground.
    /// </summary>
    /// <param name="pos">The position to project (either the BC or EC)</param>
    /// <returns>The position on the circle</returns>
    IPosition GetCirclePosition(IPosition pos)
    {
        // Get the deltas of the position with respect to the centre of the circle.
        ICircleGeometry c = Circle;
        double cx = c.Center.X;
        double cy = c.Center.Y;
        double dx = pos.X - cx;
        double dy = pos.Y - cy;
        double dist = Math.Sqrt(dx*dx + dy*dy);

        // Return the centre if the position is coincident with the centre.
        if (dist < MathConstants.TINY)
            return c.Center;

        // Get the factor for projecting the position.
        double factor = c.Radius/dist;

        // Figure out the position on the circle.
        double x = cx + dx*factor;
        double y = cy + dy*factor;
        return new Position(x,y);
    }

    /// <summary>
    /// Moves this arc by changing the circle on which it lies. Note that this
    /// does NOT move the locations that represent the BC and the EC; you must
    /// make separate calls to <c>PointFeature.Move</c> in order to do that.
    /// 
    /// This is called during rollforward processing (due to some changes, a circle
    /// that was formerly re-used may end up being no good, so a different circle
    /// needs to be referenced).
    /// </summary>
    /// <param name="newCircle">The new circle for the arc.</param>
    /// <param name="isClockwise">True if the arc is supposed to go clockwise.</param>
    internal void Move(Circle newCircle, bool isClockwise)
    {
        Circle? oldCircle = this.Circle;

        if (!ReferenceEquals(oldCircle, newCircle))
        {
            // Cut the reference from the arc's current circle.
            oldCircle?.RemoveArc(this);

            // Add reference to the new circle (and vice versa).
            ChangeGeometry(new ArcGeometry(newCircle, StartPoint, EndPoint, isClockwise));
            newCircle.AddArc(this);
        }
        else
            Geometry.IsClockwise = isClockwise;
    }

    /// <summary>
    /// Attempts to locate the circular arc (if any) that this line is based on.
    /// </summary>
    /// <returns><c>this</c> (always).</returns>
    internal override ArcFeature? GetArcBase()
    {
        return this;
    }

    /// <summary>
    /// Performs any processing that needs to be done just before the position of
    /// a referenced feature is changed.
    /// </summary>
    /// <param name="f">The feature that is about to be moved  - something that
    /// the <c>IFeatureDependent</c> is dependent on (not null).</param>
    /// <param name="ctx">The context in which the move is being made (not null).</param>
    public override void OnFeatureMoving(Feature f, UpdateEditingContext ctx)
    {
        // Remove the circle from the spatial index
        this.Circle?.OnFeatureMoving(f, ctx);

        base.OnFeatureMoving(f, ctx);
    }

    /// <summary>
    /// Calculates the start and end positions of an extension to this circular arc.
    /// </summary>
    /// <param name="isFromEnd">True if extending from the end of the line.</param>
    /// <param name="dist">The length of the extension.</param>
    /// <param name="start">The position of the start of the extension.</param>
    /// <param name="end">The position of the end of the extension.</param>
    /// <param name="center">The center of the circle on which the arc lies.</param>
    /// <param name="iscw">Is the circular arc directed clockwise?</param>
    /// <returns>True if position have been worked out. False if there is insufficient data,
    /// or the extension is not on a circular arc, or the length is more than the circumference
    /// of the circle (in those cases, the start and end positions come back as nulls)</returns>
    internal bool CalculateExtension(
        bool isFromEnd,
        Distance dist,
        out IPosition? start,
        out IPosition? end,
        out IPosition? center,
        out bool iscw)
    {
        start = end = null;
        center = null;
        iscw = true;

        // The length must be defined.
        if (!dist.IsDefined)
            return false;

        center = Circle.Center;
        double radius = Circle.Radius;
        iscw = IsClockwise;

        // Get the length of the arc extension, in meters on the ground.
        double arclen = dist.Meters;

        // If the arc length exceeds the length of the circumference,
        // the end point can't be calculated.
        double circumf = MathConstants.PIMUL2 * radius;
        if (arclen > circumf)
            return false;

        // If we're extending from the start of the arc, the curve direction has to be reversed too.
        if (!isFromEnd)
            iscw = !iscw;

        // Get the point we're extending from.
        start = (isFromEnd ? EndPoint : StartPoint);

        // Get the point we're extending to ...

        // Get the bearing from the center of the circle to the start of the arc.
        Turn turn = new Turn(center, start);
        double sbearing = turn.BearingInRadians;

        // Get the sector angle (in radians).
        double sector = arclen / radius;

        double ebearing = sbearing;
        if (iscw)
            ebearing += sector;
        else
            ebearing -= sector;

        end = BasicGeom.Polar(center, ebearing, radius);

        // Re-calculate the arc length on the mapping plane,
        arclen = dist.GetPlanarMetric(start, end, SpatialSystem);

        // And adjust the end position accordingly.
        sector = arclen / radius;

        if (iscw)
            ebearing = sbearing + sector;
        else
            ebearing = sbearing - sector;

        end = BasicGeom.Polar(center, ebearing, radius);
        return true;
    }
}