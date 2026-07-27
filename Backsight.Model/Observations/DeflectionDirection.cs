namespace Backsight.Model.Observations;

/// <written by="Steve Stanton" on="09-JUN-1999" />
/// <summary>A deflection angle.</summary>
class DeflectionDirection : AngleDirection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeflectionDirection"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal DeflectionDirection(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="backsight">The backsight point.</param>
    /// <param name="occupied">The occupied station.</param>
    /// <param name="observation">The angle to an observed point, measured with respect
    /// to the projection of an orientation line defined by the backsight and the occupied
    /// station. Positive values indicate a clockwise rotation & negated values for
    /// counter-clockwise.
    /// </param>
    internal DeflectionDirection(PointFeature backsight, PointFeature occupied, IAngle observation)
        : base(backsight, occupied, observation)
    {
    }

    /// <summary>
    /// The angle as a bearing
    /// </summary>
    internal override IAngle Bearing
    {
        get
        {
            // Get the bearing to the backsight
            double bb = BasicGeom.BearingInRadians(this.Backsight, this.From);

            // Add on the observed angle, and restrict to [0,2*PI]
            double a = bb + this.ObservationInRadians;
            return new RadianValue(Direction.Normalize(a));
        }
    }
}