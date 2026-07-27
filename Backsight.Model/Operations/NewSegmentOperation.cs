using System.Diagnostics;

namespace Backsight.Model.Operations;

/// <summary>
/// An edit that creates a line segment connecting a pair of pre-existing points.
/// </summary>
class NewSegmentOperation : NewLineOperation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewSegmentOperation"/> class.
    /// </summary>
    /// <param name="store">The map store this operation is part of.</param>
    internal NewSegmentOperation(IMapStore store)
        : base(store)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewSegmentOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal NewSegmentOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        // Nothing to do
    }

    /// <summary>
    /// Creates a new simple line segment.
    /// </summary>
    /// <param name="start">The point at the start of the new line.</param>
    /// <param name="end">The point at the end of the new line.</param>
    /// <remarks>When you add a new line segment, the two end points will be referenced both to the
    /// new line, and to the editing operation that defined the line. While this is a bit verbose,
    /// it's consistent.</remarks>
    internal void Execute(PointFeature start, PointFeature end)
    {
        // Disallow an attempt to add a null line.
        if (start.Geometry.IsCoincident(end.Geometry))
            throw new Exception("NewLineOperation.Execute - Attempt to add null line.");

        // Add the new line with default entity type.
        var ent = Session.MapStore.Settings.DefaultLineType;
        LineFeature newLine = MapModel.AddLine(start, end, ent, this);
        base.SetNewLine(newLine);

        // Peform standard completion steps
        Complete();
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer editSerializer)
    {
        base.WriteData(editSerializer);

        // Nothing to do - the relevant info should have come out via the geometry object attached
        // to the created line feature
        Debug.Assert(base.Line.LineGeometry is SegmentGeometry);
    }
}