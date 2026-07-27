namespace Backsight.Model;

/// <summary>
/// Information about an ID allocation that has been made to a user for a specific editing project.
/// </summary>
class IdAllocation : Change
{
    /// <summary>
    /// The unique ID of the ID group associated with this allocation
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// The lowest value in the allocation (this is the primary key)
    /// </summary>
    public int LowestId { get; set; }

    /// <summary>
    /// The highest value in the allocation
    /// </summary>
    public int HighestId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdAllocation"/> class.
    /// </summary>
    /// <param name="editSequence">An editing sequence number for this allocation.</param>
    internal IdAllocation(uint editSequence)
        : base(editSequence)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdAllocation"/> class.
    /// </summary>
    /// <param name="ed">The mechanism for reading back content.</param>
    internal IdAllocation(EditDeserializer ed)
        : base(ed)
    {
        this.GroupId = ed.ReadInt32(DataField.GroupId);
        this.LowestId = ed.ReadInt32(DataField.LowestId);
        this.HighestId = ed.ReadInt32(DataField.HighestId);
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="es">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer es)
    {
        base.WriteData(es);

        es.WriteInt32(DataField.GroupId, this.GroupId);
        es.WriteInt32(DataField.LowestId, this.LowestId);
        es.WriteInt32(DataField.HighestId, this.HighestId);
    }

    /// <summary>
    /// The number of IDs in this allocation
    /// </summary>
    internal int Size => HighestId - LowestId + 1;
}