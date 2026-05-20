using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation for a layer definition.
/// </summary>
[Map("Layers")]
internal partial class LayerRow
{
    [Primary] public int LayerId { get; set; }
    public string Name { get; set; } = "";
    public int ThemeId { get; set; }
    public int ThemeSequence { get; set; }
    public int DefaultPointId { get; set; }
    public int DefaultLineId { get; set; }
    public int DefaultPolygonId { get; set; }
    public int DefaultTextId { get; set; }
}

// Additional properties to satisfy interfaces.
internal partial class LayerRow : Row, ILayer, ISetLayer
{
    public override string ToString() => Name;
    public int Id
    {
        get => LayerId;
        set => LayerId = value;
    }

    public ITheme Theme => Repository.FindRequired<ITheme>(ThemeId);

    public IEntity DefaultPointType
    {
        get => Repository.FindRequired<IEntity>(DefaultPointId);
        set => DefaultPointId = value?.Id ?? 0;
    }

    public IEntity DefaultLineType
    {
        get => Repository.FindRequired<IEntity>(DefaultLineId);
        set => DefaultLineId = value?.Id ?? 0;
    }

    public IEntity DefaultTextType
    {
        get => Repository.FindRequired<IEntity>(DefaultTextId);
        set => DefaultTextId = value?.Id ?? 0;
    }

    public IEntity DefaultPolygonType
    {
        get => Repository.FindRequired<IEntity>(DefaultPolygonId);
        set => DefaultPolygonId = value?.Id ?? 0;
    }
}