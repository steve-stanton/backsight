using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

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

// Additional properties to satisfy the readonly interface.
internal partial class LayerRow : Row, ILayer
{
    public override string ToString() => Name;
    public int Id => LayerId;
    public ITheme Theme => Repository.FindRequired<ITheme>(ThemeId);
    public IEntity DefaultPointType => Repository.FindRequired<IEntity>(DefaultPointId); 
    public IEntity DefaultLineType => Repository.FindRequired<IEntity>(DefaultLineId);
    public IEntity DefaultTextType => Repository.FindRequired<IEntity>(DefaultTextId);
    public IEntity DefaultPolygonType => Repository.FindRequired<IEntity>(DefaultPolygonId);
}