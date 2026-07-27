using System.Diagnostics;
using Backsight.Environment;

namespace Backsight.Model.Operations;

class NewMiscTextOperation : NewTextOperation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewMiscTextOperation"/> class.
    /// </summary>
    /// <param name="store">The map store this operation is part of.</param>
    internal NewMiscTextOperation(IMapStore store)
        : base(store)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewMiscTextOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal NewMiscTextOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        // Nothing to do
    }

    /// <summary>
    /// Executes this operation. This version is suitable for adding miscellaneous
    /// non-topological trim.
    /// </summary>
    /// <param name="trim">The text of the label.</param>
    /// <param name="ent">The entity type to assign to the new label.</param>
    /// <param name="position">The reference position for the label.</param>
    /// <param name="ght">The height of the new label, in meters on the ground.</param>
    /// <param name="gwd">The width of the new label, in meters on the ground.</param>
    /// <param name="rot">The clockwise rotation of the text, in radians from the horizontal.</param>
    internal void Execute(string trim, IEntity ent, IPosition position, double ght, double gwd, double rot)
    {
        // Add the label.
        TextFeature text = MapModel.AddMiscText(this, trim, ent, position, ght, gwd, rot);
        SetText(text);

        // The trim is always non-topological.
        text.SetTopology(false);

        Complete();
    }

    /// <summary>
    /// Obtains the features that are referenced by this operation (including features
    /// that are indirectly referenced by observation classes).
    /// </summary>
    /// <returns>
    /// The referenced features (never null, but may be an empty array).
    /// </returns>
    public override Feature[] GetRequiredFeatures()
    {
        return new Feature[0];
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer editSerializer)
    {
        base.WriteData(editSerializer);

        // Nothing to do - the relevant info should have come out via the geometry object attached
        // to the created text feature
        Debug.Assert(base.Text.TextGeometry is MiscTextGeometry);
    }
}