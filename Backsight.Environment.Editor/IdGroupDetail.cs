namespace Backsight.Environment.Editor;

/// <summary>
/// Current details for an ID group, as used by the <see cref="IdGroupForm"/>.
/// </summary>
internal record IdGroupDetail(
    string Name,
    int LowestId,
    int HighestId,
    bool HasCheckDigit,
    int PacketSize,
    string KeyFormat)
{
    internal IdGroupDetail(IIdGroup g)
        : this(g.Name, g.LowestId, g.HighestId, g.HasCheckDigit, g.PacketSize, g.KeyFormat) { }
}