using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation of a table that holds domain values (the associated table(s) are identified
/// via <see cref="ColumnDomainRow"/>).
/// </summary>
[Map("DomainTables")]
internal partial class DomainTableRow
{
    [Primary] public int DomainId { get; set; }
    public string TableName { get; set; } = "";
}

// Additional properties to satisfy the readonly interface.
internal partial class DomainTableRow : Row, IDomainTable
{
    public override string ToString() => TableName;
    
    /// <summary>
    /// The content of the domain table. The key is the lookup value, the
    /// value is the expanded value.
    /// </summary>
    private readonly Dictionary<string, string> _content = new();

    internal void LoadContent(EnvironmentRepository repository)
    {
        foreach (var row in repository.ExecuteQuery($"SELECT ShortValue, LongValue FROM {TableName}"))
            _content.Add(row.ShortValue.ToString(), row.LongValue.ToString());
    }
    
    public int Id => DomainId;
    public string Lookup(string shortValue)
    {
        // The connection string is not currently used (in the past, it was possible to hold
        // user data in a database apart from the Backsight environment data, but the current
        // implementation aims to simplify things).
        
        return _content.GetValueOrDefault(shortValue, "");
    }

    public string[] GetLookupValues()
    {
        return _content.Keys.ToArray();
    }
}