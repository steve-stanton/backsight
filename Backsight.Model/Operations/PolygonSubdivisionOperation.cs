using Backsight.Environment;

namespace Backsight.Model.Operations;

/// <written by="Steve Stanton" on="27-FEB-1998" was="CeAreaSubdivision" />
/// <summary>
/// Subdivision of a polygon.
/// </summary>
class PolygonSubdivisionOperation : Operation
{
    /// <summary>
    /// Any polygon label that was de-activated as a result of the subdivision.
    /// </summary>
    TextFeature? m_Label;

    /// <summary>
    /// The lines that were created (all simple line segments).
    /// </summary>
    LineFeature[]? m_Lines;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonSubdivisionOperation"/> class.
    /// </summary>
    /// <param name="store">The map store this operation is part of.</param>
    internal PolygonSubdivisionOperation(IMapStore store)
        : base(store)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonSubdivisionOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal PolygonSubdivisionOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        if (editDeserializer.IsNextField(DataField.DeactivatedLabel))
        {
            m_Label = editDeserializer.ReadFeatureRef<TextFeature>(DataField.DeactivatedLabel);
            m_Label.IsInactive = true; // later ?
        }

        m_Lines = editDeserializer.ReadPersistentArray<LineFeature>(DataField.Lines);
    }

    /// <summary>
    /// A user-perceived title for this operation.
    /// </summary>
    public override string Name => "Polygon subdivision";

    /// <summary>
    /// The features that were created by this operation.
    /// </summary>
    internal override Feature[] Features => m_Lines;

    /// <summary>
    /// The lines that were created.
    /// </summary>
    internal LineFeature[] NewLines => m_Lines;

    internal override EditingActionId EditId => EditingActionId.PolygonSubdivision;

    /// <summary>
    /// Rollback this operation (occurs when a user undoes the last edit).
    /// </summary>
    internal override void Undo()
    {
        base.OnRollback();

        // Mark each created line for undo
        foreach (LineFeature line in m_Lines)
            Rollback(line);

        // If the polygon originally had a label, restore it.
        if (m_Label!=null)
            m_Label.Restore();
    }

    /// <summary>
    /// Executes this operation.
    /// </summary>
    /// <param name="sub">The polygon subdivision information.</param>
    internal void Execute(PolygonSub sub)
    {
        int numLine = sub.NumLink;
        if (numLine==0)
            throw new Exception("PolygonSubdivisionOperation.Execute - Nothing to add");

        // If the polygon contains just one label, de-activate it. This covers a "well-behaved" situation,
        // where the label inside the polygon is likely to be redundant after the subdivision (it also
        // conforms to logic used in the past). In a situation where the polygon contains multiple labels,
        // it's less clear whether the labels become redundant or not, so we keep them all.
        Polygon pol = sub.Polygon;
        if (pol.LabelCount == 1)
        {
            m_Label = pol.Label;
            if (m_Label!=null)
                m_Label.IsInactive = true;
        }

        // Mark the polygon for deletion
        pol.IsDeleted = true;

        // Get the default entity type for lines.
        IEntity ent = Session.MapStore.Settings.DefaultLineType;

        // Allocate array to point to the lines we will be creating.
        m_Lines = new LineFeature[numLine];

        // Add lines for each link
        PointFeature start, end;
        for (int i=0; sub.GetLink(i, out start, out end); i++)
        {
            m_Lines[i] = MapModel.AddLine(start, end, ent, this);
        }

        // Peform standard completion steps
        Complete();
    }

    /// <summary>
    /// Attempts to locate a superseded (inactive) line that was the parent of
    /// a specific line.
    /// </summary>
    /// <param name="line">The line of interest</param>
    /// <returns>Null (always), since this edit doesn't supersede any lines.</returns>
    internal override LineFeature GetPredecessor(LineFeature line)
    {
        return null;
    }

    /// <summary>
    /// Any polygon label that was de-activated as a result of the subdivision.
    /// </summary>
    internal TextFeature DeactivatedLabel
    {
        get => m_Label;
        set => m_Label = value;
    }

    /// <summary>
    /// Gets the features that are referenced by this operation (including features
    /// that are indirectly referenced by observation classes).
    /// </summary>
    /// <returns>
    /// The referenced features (never null, but may be an empty array).
    /// </returns>
    public override Feature[] GetRequiredFeatures()
    {
        List<Feature> result = new List<Feature>(m_Lines.Length * 2);

        foreach (LineFeature line in m_Lines)
        {
            result.Add(line.StartPoint);
            result.Add(line.EndPoint);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer editSerializer)
    {
        base.WriteData(editSerializer);

        if (m_Label != null)
            editSerializer.WriteFeatureRef<TextFeature>(DataField.DeactivatedLabel, m_Label);

        editSerializer.WritePersistentArray<LineFeature>(DataField.Lines, m_Lines);
    }
}