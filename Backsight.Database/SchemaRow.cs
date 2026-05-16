using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation of a "schema" (a database table associated with map features).
/// </summary>
[Map("Schemas")]
internal partial class SchemaRow
{
    [Primary] public int SchemaId { get; set; }
    public string TableName { get; set; } = "";
    public string IdColumnName { get; set; } = "";
}

// Additional properties to satisfy the readonly interface.
internal partial class SchemaRow : Row, ITable
{
    public override string ToString() => TableName;
    
    public int Id => SchemaId;
    public ITemplate[] Templates => Repository
        .FindMany<ITemplate>(x => x.Id == SchemaId)
        .ToArray();

    public IColumnDomain[] ColumnDomains => Repository
        .FindColumnDomains(this)
        .ToArray();
}