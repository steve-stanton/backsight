using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// The association of a database column with a corresponding domain table that holds allowable values.
/// </summary>
[Map("ColumnDomains")]
internal partial class ColumnDomainRow
{
    public int TableId { get; set; }
    public string ColumnName { get; set; } = "";
    public int DomainId { get; set; }
}

// Additional properties to satisfy the readonly interface.
/// <remarks>
/// Unlike most other tables, this class does not extend from <see cref="Row"/> because the <c>ColumnDomains</c>
/// table has a composite primary key.
/// </remarks>
internal partial class ColumnDomainRow : IColumnDomain
{
    internal IEnvironmentRepository Repository { get; set; }

    public ITable ParentTable => Repository.FindRequired<ITable>(TableId);
    public IDomainTable Domain => Repository.FindRequired<IDomainTable>(DomainId);
}