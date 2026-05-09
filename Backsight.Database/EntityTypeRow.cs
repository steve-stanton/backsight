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
internal partial class EntityTypeRow : Row, IEntity
{
    public int Id => EntityId;
    public bool IsPointValid => IsPoint == YES;
    public bool IsLineValid => IsLine == YES;
    public bool IsLineAutoTrimmed => IsLineTrimmed == YES;
    public bool IsPolygonValid => IsPolygon == YES;
    public bool IsPolygonBoundaryValid => IsLineTopological == YES;
    public bool IsTextValid => IsText == YES && IsPolygon == NO;

    /// <summary>
    /// Checks whether this entity type can be associated with the supplied spatial data type.
    /// </summary>
    /// <param name="t">The type of data to check (could conceivably be a combination of types).</param>
    /// <returns>True if this entity type can be associated with the spatial data type.</returns>
    public bool IsValid(SpatialType t)
    {
        return ((t & SpatialType.Point) != 0 && IsPointValid) ||
               ((t & SpatialType.Line) != 0 && IsLineValid) ||
               ((t & SpatialType.Text) != 0 && IsTextValid) ||
               ((t & SpatialType.Polygon) != 0 && IsPolygonValid);
    }

    public ITable[] DefaultTables => Repository.FindAssociatedTables(this).ToArray();
    public IIdGroup IdGroup => Repository.FindRequired<IIdGroup>(GroupId);
    public ILayer? Layer => Repository.Find<ILayer>(LayerId);
    public IFont Font => Repository.FindRequired<IFont>(FontId);
}
