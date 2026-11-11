using System.Diagnostics;

namespace Backsight.Model.Operations;

/// <summary>
/// Base class for operations that subdivide a line at an instance of <see cref="ITerminal"/>.
/// </summary>
public abstract class LineSubdivisionAtTerminal : Operation
{
    /// <summary>
    /// The line the point sits on. This line gets de-activated.
    /// </summary>
    private readonly LineFeature m_Line;

    /// <summary>
    /// The line created prior to the terminal.
    /// </summary>
    private LineFeature? m_NewLine1;
    
    /// <summary>
    /// The line created after the terminal.
    /// </summary>
    private LineFeature? m_NewLine2;

    protected LineSubdivisionAtTerminal(LineFeature line)
        : base(line.MapStore)
    {
        m_Line = line;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSubdivisionAtTerminal"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    protected LineSubdivisionAtTerminal(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        m_Line = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Line);
        
        // The line sections cannot be created here because we need to know both end points,
        // and one of those won't be known until the constructor of the derived class is called.
        // It can then call the MakeSections method below.
    }

    /// <summary>
    /// Deserializes the line section features.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    /// <param name="splitPoint">The point at which the line is split.</param>
    protected void MakeSections(EditDeserializer editDeserializer, PointFeature splitPoint)
    {
        var dataId1 = editDeserializer.ReadInternalId(DataField.NewLine1);
        var section1 = new SectionGeometry(m_Line, m_Line.StartPoint, splitPoint);
        m_NewLine1 = m_Line.MakeSubSection(this, dataId1, section1);

        var dataId2 = editDeserializer.ReadInternalId(DataField.NewLine2);
        var section2 = new SectionGeometry(m_Line, splitPoint, m_Line.EndPoint);
        m_NewLine2 = m_Line.MakeSubSection(this, dataId2, section2);

        m_Line.IsInactive = true;
    }
    
    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer editSerializer)
    {
        Debug.Assert(m_NewLine1 is not null);
        Debug.Assert(m_NewLine2 is not null);
        Debug.Assert(SplitPoint is not null);
        
        base.WriteData(editSerializer);
        editSerializer.WriteFeatureRef(DataField.Line, m_Line);
        
        if (SplitPoint.Creator == this)
            editSerializer.WritePersistent(DataField.NewPoint, new FeatureStub(SplitPoint));
        else
            editSerializer.WriteFeatureRef(DataField.Point, SplitPoint!);

        editSerializer.WriteInternalId(DataField.NewLine1, m_NewLine1.InternalId);
        editSerializer.WriteInternalId(DataField.NewLine2, m_NewLine2.InternalId);
    }

    /// <summary>
    /// The subdivided line.
    /// </summary>
    protected LineFeature BaseLine => m_Line;
    
    /// <summary>
    /// The line section that precedes the split point.
    /// </summary>
    public LineFeature? Section1 => m_NewLine1;

    /// <summary>
    /// The line section that follows the split point.
    /// </summary>
    public LineFeature? Section2 => m_NewLine2;
    
    /// <summary>
    /// The point at which the line is subdivided.
    /// </summary>
    protected abstract PointFeature? SplitPoint { get; }

    /// <inheritdoc/>
    internal override LineFeature? GetPredecessor(LineFeature line)
    {
        if (ReferenceEquals(line, m_NewLine1) || ReferenceEquals(line, m_NewLine2))
            return m_Line;
        else
            return null;
    }

    /// <inheritdoc/>
    public override Feature[] GetRequiredFeatures()
    {
        return [.. ReferencedFeatures()];
    }

    /// <inheritdoc/>
    internal override Feature[] Features => [.. CreatedFeatures()];

    /// <inheritdoc/>
    internal override void Undo()
    {
        base.OnRollback();

        // Mark created features for undo
        foreach (var feature in CreatedFeatures())
            Rollback(feature);

        // Remove the references to this edit
        foreach (var feature in ReferencedFeatures())
            feature.CutOp(this);
        
        // Restore the original line
        m_Line.Restore();
    }

    /// <summary>
    /// The existing features referenced by this edit.
    /// </summary>
    protected virtual IEnumerable<Feature> ReferencedFeatures()
    {
        yield return m_Line;
    }

    /// <summary>
    /// The features created by this edit.
    /// </summary>
    protected virtual IEnumerable<Feature> CreatedFeatures()
    {
        if (m_NewLine1 is not null)
            yield return m_NewLine1;
        
        if (m_NewLine2 is not null)
            yield return m_NewLine2;
    }

    /// <summary>
    /// Executes this operation.
    /// </summary>
    public void Execute()
    {
        var ff = new FeatureFactory(this);
        Execute(ff);
    }

    /// <inheritdoc/>
    public override void Execute(FeatureFactory ff)
    {
        // See FeatureFactory.MakeSections - the only thing that really matters is the
        // session sequence number that will get picked up by the FeatureStub constructor.
        ff.AddFeatureDescription(DataField.NewLine1, new FeatureStub(this, m_Line.EntityType, null));
        ff.AddFeatureDescription(DataField.NewLine2, new FeatureStub(this, m_Line.EntityType, null));

        base.Execute(ff);
    }

    /// <inheritdoc/>
    internal override void ProcessFeatures(FeatureFactory ff)
    {
        Debug.Assert(SplitPoint is not null);
        
        // Create the line sections and deactivate the original line
        ff.MakeSections(m_Line, DataField.NewLine1, SplitPoint, DataField.NewLine2, out m_NewLine1, out m_NewLine2);
    }
}