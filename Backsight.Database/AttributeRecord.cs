using Backsight.Environment;

namespace Backsight.Database;

/// <summary>
/// The content of a row in a miscellaneous attribute table.
/// </summary>
/// <param name="Table">The definition for the attribute table.</param>
/// <param name="Columns">Metadata for the columns in the table.</param>
/// <param name="Content">The content of the database row (indexed by column name).</param>
public record AttributeRecord(ITable Table, ColumnInfo[] Columns, Dictionary<string, object?> Content)
{
    public string Id
    {
        get => Content.GetValueOrDefault(Table.IdColumnName)?.ToString() ?? String.Empty;
        set => Content[Table.IdColumnName] = value;
    }
    
    /// <summary>
    /// Assigns the content of another record to this one.
    /// </summary>
    /// <param name="from">The record to copy from (referring to the same attribute
    /// table as this record).</param>
    /// <exception cref="ArgumentException">Supplied record refers to a different table.</exception>
    public void Assign(AttributeRecord from)
    {
        if (Table.Id != from.Table.Id)
            throw new ArgumentException("Cannot assign records from different tables");

        foreach (var (key, value) in from.Content)
            Content[key] = value;
    }

    /// <summary>
    /// Assigns default values for all non-nullable columns.
    /// </summary>
    /// <remarks>
    /// Numeric fields are assigned a value of 0 (even though this could conceivably break check
    /// constraints). Fields associated with a domain table are assigned the first lookup value
    /// (whatever that is). Any other [var]char field that is not nullable will be assigned a
    /// blank value.
    /// </remarks>
    public void AssignDefaultValues()
    {
        IColumnDomain[] cds = Table.ColumnDomains;
        
        foreach (var c in Columns.Where(x => !x.Nullable))
        {
            Type t = c.DataType;
            
            if (t == typeof(long) || t == typeof(int) || t == typeof(short) || t == typeof(byte))
            {
                Content[c.Name] = 0;
            }
            else if (t == typeof(double) || t == typeof(float))
            {
                Content[c.Name] = 0.0;
            }
            else if (t == typeof(string))
            {
                var cd = cds.FirstOrDefault(x =>
                    String.Compare(x.ColumnName, c.Name, StringComparison.OrdinalIgnoreCase) == 0);
                
                if (cd is not null)
                {
                    string[] vals = cd.Domain.GetLookupValues();
                    if (vals.Length > 0)
                        Content[c.Name] = vals[0];
                }
                else
                {
                    // Default to a blank string is the field isn't nullable (the column could be
                    // the ID field, it needs to get set later)
                    Content[c.Name] = String.Empty;
                }
            }
        }
    }
}
