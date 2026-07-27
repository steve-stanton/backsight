using System.Diagnostics;

namespace Backsight.Model.Operations;

class NewKeyTextOperation : NewTextOperation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewKeyTextOperation"/> class.
    /// </summary>
    /// <param name="store">The map store this operation is part of.</param>
    internal NewKeyTextOperation(IMapStore store)
        : base(store)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewKeyTextOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal NewKeyTextOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        // Nothing to do
    }

    /// <summary>
    /// Executes the new label operation.
    /// </summary>
    /// <param name="vtx">The position of the new label.</param>
    /// <param name="polygonId">The ID and entity type to assign to the new label.</param>
    /// <param name="pol">The polygon that the label falls inside. It should not already refer to a label.</param>
    /// <param name="height">The height of the text, in meters on the ground.</param>
    /// <param name="width">The width of the text, in meters on the ground.</param>
    /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
    internal void Execute(IPosition vtx, IdHandle polygonId, Polygon pol,
        double height, double width, double rotation)
    {
        // Add the label.
        CadastralMapModel map = MapModel;
        TextFeature text = map.AddKeyLabel(this, polygonId, vtx, height, width, rotation);
        SetText(text);

        // Associate the polygon with the label, and vice versa.
        text.SetTopology(true);
        pol.ClaimLabel(text);

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
        Debug.Assert(base.Text.TextGeometry is KeyTextGeometry);
    }
}