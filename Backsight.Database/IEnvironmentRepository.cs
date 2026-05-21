using Backsight.Environment;

namespace Backsight.Database;

public interface IEnvironmentRepository
{
    string Name { get; }

    IEnumerable<string> QueryTableNames();
    
    /// <summary>
    /// Gets details for the columns in a table. 
    /// </summary>
    /// <param name="tableName">The name of the database table.</param>
    /// <returns>Metadata for each column in the specified table.</returns>
    IEnumerable<ColumnInfo> QueryTableColumns(string tableName);
    
    IEnumerable<IDomainTable> DomainTables { get; }
    IEnumerable<IEntity> EntityTypes { get; }
    IEnumerable<IFont> Fonts { get; }
    IEnumerable<IIdGroup> IdGroups { get; }
    IEnumerable<ILayer> Layers { get; }
    IEnumerable<IProperty> Properties { get; }
    IEnumerable<ITable> Tables { get; }
    IEnumerable<ITemplate> Templates { get; }
    IEnumerable<ITheme> Themes { get; }

    /// <summary>
    /// Finds the user data tables that normally hold attributes associated with a specific entity type. 
    /// </summary>
    /// <param name="entity">The entity type.</param>
    /// <returns>Details for the attribute tables normally associated with the entity type.</returns>
    IEnumerable<ITable> FindAssociatedTables(IEntity entity);
    
    void SaveAssociatedTables(IEntity entity, IEnumerable<ITable> tables);
    
    /// <summary>
    /// Saves any changes to the entity types associated with an ID group. 
    /// </summary>
    /// <param name="group">The ID group.</param>
    /// <param name="entities">The entity types to be associated with the group.</param>
    void SaveAssociatedEntities(IIdGroup group, IEnumerable<IEntity> entities);
    
    /// <summary>
    /// Finds the columns in an attribute table that are associated with domain values.
    /// </summary>
    /// <param name="table">The attribute table associated with an entity.</param>
    /// <returns>Details for any columns in the table that have an associated <see cref="IDomainTable"/>.</returns>
    IEnumerable<IColumnDomain> FindColumnDomains(ITable table);

    /// <summary>
    /// Creates a new association between a column in an attribute table, and a table that
    /// holds the domain values for that column (but does not save it to the database). 
    /// </summary>
    /// <param name="parentTable">The attribute table.</param>
    /// <param name="columnName">The name of a column within the attribute table.</param>
    /// <param name="domainTable">The table that holds the values permitted in that column..</param>
    /// <returns>The new association.</returns>
    IColumnDomain CreateColumnDomain(ITable parentTable, string columnName, IDomainTable domainTable);
    
    /// <summary>
    /// Saves a new item created by <see cref="CreateColumnDomain"/>.
    /// </summary>
    /// <param name="columnDomain">The item to be saved.</param>
    void SaveColumnDomain(IColumnDomain columnDomain);
    
    /// <summary>
    /// Removes the association between a column in an attribute table, and a domain table. 
    /// </summary>
    /// <param name="columnDomain">The association to be removed.</param>
    void DeleteColumnDomain(IColumnDomain columnDomain);
    
    /// <summary>
    /// Creates a new runtime-shaped record for the specified attribute table.
    /// </summary>
    /// <param name="table">The attribute table that the record will eventually be inserted into.</param>
    /// <returns>A record for holding the content of the new row.</returns>
    AttributeRecord CreateNewRecord(ITable table);
    
    /// <summary>
    /// Inserts a runtime-shaped record into the specified attribute table.
    /// </summary>
    /// <param name="record">The attribute record to insert.</param>
    /// <returns>The value returned by the insert operation.</returns>
    object? InsertRecord(AttributeRecord record);
    
    /// <summary>
    /// Updates a runtime-shaped record in the associated attribute table.
    /// </summary>
    /// <param name="record">The attribute record containing updated values.</param>
    void UpdateRecord(AttributeRecord record);

    /// <summary>
    /// Retrieves attribute records from the specified table that match the given key.
    /// </summary>
    /// <param name="table">Reference to the table containing the attribute records.</param>
    /// <param name="key">The user-perceived key used to identify the attribute records.</param>
    /// <returns>An array of attribute records that match the specified key.</returns>
    /// <remarks>
    /// The supplied key corresponds to the column referred to by <see cref="ITable.IdColumnName"/>.
    /// This is the user-perceived key, which is not necessarily the primary key of
    /// the table. That is why more than one record may be returned.
    /// </remarks>
    AttributeRecord[] FindAttributeRecordsByKey(ITable table, string key)
        => FindAttributeRecords(table, [key]);
    
    /// <summary>
    /// Retrieves attribute records from the specified table that have a matching key.
    /// </summary>
    /// <param name="table">Reference to the table containing the attribute records.</param>
    /// <param name="keys">The user-perceived keys used to identify the attribute records.</param>
    /// <returns>An array of attribute records that match the specified keys.</returns>
    /// <remarks>
    /// The supplied keys will be matched with the column referred to by <see cref="ITable.IdColumnName"/>.
    /// This is the user-perceived key, which is not necessarily the primary key of
    /// the table. That is why more than one record may be returned.
    /// </remarks>
    AttributeRecord[] FindAttributeRecords(ITable table, IEnumerable<string> keys);

    /// <summary>
    /// Updates an ID group by setting the value for the last allocated ID.
    /// </summary>
    /// <param name="group">The group to be updated.</param>
    /// <param name="maxUsedId">The last ID allocated for the group.</param>
    /// <returns>The updated group.</returns>
    IIdGroup UpdateIdGroup(IIdGroup group, int maxUsedId);
    
    T? Find<T>(int id) where T : class, IEnvironmentItem;
    T FindRequired<T>(int id) where T : class, IEnvironmentItem;
    IEnumerable<T> FindMany<T>(Predicate<T> predicate) where T : class, IEnvironmentItem;
    string? FindPropertyByName(string propertyName);

    TSetter GetSetter<TItem, TSetter>(TItem item)
        where TItem : class, IEnvironmentItem
        where TSetter : class, ISetter;

    void SaveChanges<TItem, TSetter>(TItem item, TSetter setter)
        where TItem : class, IEnvironmentItem
        where TSetter : class, ISetter;
    
    TItem CreateNewItem<TItem>() where TItem : class, IEnvironmentItem;
}
