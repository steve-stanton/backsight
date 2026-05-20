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
    public string Name { get; set; } = "";
    public int LowestId { get; set; }
    public int HighestId { get; set; }
    public int PacketSize { get; set; }
    public string CheckDigit { get; set; } = NO;
    public string KeyFormat { get; set; } = "{0}";
    public int MaxUsedId { get; set;  }
}

// Additional properties to satisfy the readonly interface.
internal partial class IdGroupRow : Row, IIdGroup, ISetIdGroup
{
    public override string ToString() => Name;
    public int Id
    {
        get => GroupId;
        set => GroupId = value;
    }
    
    public bool HasCheckDigit
    {
        get => CheckDigit == YES;
        set => CheckDigit = AsString(value);
    }

    public IEntity[] EntityTypes => Repository
        .FindMany<IEntity>(x => x.IdGroup.Id == GroupId)
        .ToArray();
}