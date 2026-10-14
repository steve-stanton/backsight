using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation of an entity type.
/// </summary>
[Map("EntityTypes")]
internal partial class EntityTypeRow
{
    [Primary] public int EntityId { get; set; }
    public string Name { get; set; } = "";
    public string IsPoint { get; set; } = NO;
    public string IsLine { get; set; } = NO;
    public string IsLineTopological { get; set; } = NO;
    public string IsPolygon { get; set; } = NO;
    public string IsText { get; set; } = NO;
    public int FontId { get; set; }
    public int LayerId { get; set; }
    public int GroupId { get; set; }
    public string IsLineTrimmed { get; set; } = NO;
}

// Additional properties to satisfy the readonly interface.
internal partial class EntityTypeRow : Row, IEntity, ISetEntity
{
    public override string ToString() => Name;
    public int Id
    {
        get => EntityId;
        set => EntityId = value;
    }

    public bool IsPointValid
    {
        get => IsPoint == YES;
        set => IsPoint = AsString(value);
    }

    public bool IsLineValid
    {
        get => IsLine == YES;
        set => IsLine = AsString(value);
    }

    public bool IsLineAutoTrimmed
    {
        get => IsLineTrimmed == YES;
        set => IsLineTrimmed = AsString(value);
    }

    public bool IsPolygonValid
    {
        get => IsPolygon == YES;
        set => IsPolygon = AsString(value);
    }

    public bool IsPolygonBoundaryValid
    {
        get => IsLineTopological == YES;
        set => IsLineTopological = AsString(value);
    }

    public bool IsTextValid
    {
        get => IsText == YES && IsPolygon == NO;
        set => IsText = AsString(value);
    }

    public ILayer? Layer
    {
        get => Repository.Find<ILayer>(LayerId);
        set => LayerId = value?.Id ?? 0;
    }

    public IIdGroup? IdGroup
    {
        get => Repository.Find<IIdGroup>(GroupId);
        set => GroupId = value?.Id ?? 0;
    }
    
    public IFont? Font
    {
        get => Repository.Find<IFont>(FontId);
        set => FontId = value?.Id ?? 0;
    }

    public ITable[] DefaultTables => Repository.FindAssociatedTables(this).ToArray();
}
