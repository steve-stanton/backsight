using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation of an ID group.
/// </summary>
[Map("IdGroups")]
internal partial class IdGroupRow
{
    [Primary] public int GroupId { get; set; }
    public string Name { get; set; }
    public int LowestId { get; set; }
    public int HighestId { get; set; }
    public int PacketSize { get; set; }
    public string CheckDigit { get; set; }
    public string KeyFormat { get; set; }
    public int MaxUsedId { get; set;  }
}

// Additional properties to satisfy the readonly interface.
internal partial class IdGroupRow : Row, IIdGroup
{
    public int Id => GroupId;
    public bool HasCheckDigit => CheckDigit == YES;
    public IEntity[] EntityTypes => Repository
        .FindMany<IEntity>(x => x.Id == GroupId)
        .ToArray();
}