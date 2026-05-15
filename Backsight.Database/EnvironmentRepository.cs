using Backsight.Environment;
using Microsoft.Data.Sqlite;
using RepoDb;

namespace Backsight.Database;

public class EnvironmentRepository : DbRepository<SqliteConnection>, IEnvironmentRepository
{
    /// <summary>
    /// The one (and only) container for environment-related information.
    /// </summary>
    private static IEnvironmentRepository _repository = new EmptyRepository();
    public static IEnvironmentRepository Current => _repository;

    private readonly Dictionary<int, IDomainTable> _domainTableIndex = new();
    private readonly Dictionary<int, IEntity> _entityIndex = new();
    private readonly Dictionary<int, IFont> _fontIndex = new();
    private readonly Dictionary<int, IIdGroup> _idGroupIndex = new();
    private readonly Dictionary<int, ILayer> _layerIndex = new();
    private readonly Dictionary<int, ITable> _schemaIndex = new();
    private readonly Dictionary<int, ITemplate> _templateIndex = new();
    private readonly Dictionary<int, ITheme> _themeIndex = new();

    /// <summary>
    /// The association between columns in attribute tables with their corresponding domains.
    /// </summary>
    private readonly List<ColumnDomainRow> _columnDomains = new();
    
    /// <summary>
    /// The association between entity types and corresponding attribute data tables
    /// (indexed by the ID of the entity type).
    /// </summary>
    private readonly Dictionary<int, List<ITable>> _entitySchemasIndex = new();

    /// <summary>
    /// Miscellaneous properties of the environment.
    /// </summary>
    private readonly Dictionary<string, string> _properties = new();

    public EnvironmentRepository(string connectionString) :
        base(connectionString,
            commandTimeout: null,
            cache: null, // use MemoryCache -- consider using a do-nothing cache
            cacheItemExpiration: Int32.MaxValue,
            trace: new ConsoleTrace())
    {
        _repository = this;
    }

    public void Load()
    {
        LoadIndex<IDomainTable, DomainTableRow>();
        LoadIndex<IEntity, EntityTypeRow>();
        LoadIndex<IFont, FontRow>();
        LoadIndex<IIdGroup, IdGroupRow>();
        LoadIndex<ILayer, LayerRow>();
        LoadIndex<ITable, SchemaRow> ();
        LoadIndex<ITemplate, TemplateRow>();
        LoadIndex<ITheme, ThemeRow>();

        // Load the association between entity types and attribute data tables
        foreach (var row in LoadEntityTypeSchemas())
            _entitySchemasIndex.Add(row.Key, row.Value);
        
        // Load all domain tables
        foreach (var d in _domainTableIndex.Values.Cast<DomainTableRow>())
            d.LoadContent(this);
        
        // Load the association between columns in attribute tables with their corresponding domains
        foreach (var cd in QueryAll<ColumnDomainRow>())
        {
            cd.Repository = this;
            _columnDomains.Add(cd);
        }
        
        // Load miscellaneous properties
        foreach (var row in ExecuteQuery($"SELECT Name, Value FROM Properties"))
            _properties.Add(row.Name.ToString(), row.Value.ToString());
    }

    private void LoadIndex<TItem, TRow>()
        where TItem : class, IEnvironmentItem
        where TRow : Row, TItem
    {
        var index = GetIndex<TItem>(typeof(TItem).Name);

        foreach (var row in QueryAll<TRow>())
        {
            row.Repository = this;
            index.Add(row.Id, row);
        }
    }

    private Dictionary<int, T> GetIndex<T>(string typeName) where T : class
    {
        object? result = typeName switch
        {
            nameof(IDomainTable) => _domainTableIndex,
            nameof(IEntity) => _entityIndex,
            nameof(IFont) => _fontIndex,
            nameof(IIdGroup) => _idGroupIndex,
            nameof(ILayer) => _layerIndex,
            nameof(ITable) => _schemaIndex,
            nameof(ITemplate) => _templateIndex,
            nameof(ITheme) => _themeIndex,
            _ => null
        };

        var typedResult = result as Dictionary<int, T>;
        if (typedResult is null)
            throw new NotImplementedException();

        return typedResult;
    }
    
    public string Name
    {
        get
        {
            var csb = new SqliteConnectionStringBuilder(ConnectionString);
            return Path.GetFileNameWithoutExtension(csb.DataSource);
        }
    }

    public IEnumerable<IDomainTable> DomainTables => _domainTableIndex.Values;
    public IEnumerable<IEntity> EntityTypes => _entityIndex.Values;
    public IEnumerable<IFont> Fonts => _fontIndex.Values;
    public IEnumerable<IIdGroup> IdGroups => _idGroupIndex.Values;
    public IEnumerable<ILayer> Layers => _layerIndex.Values;
    public IEnumerable<ITable> Tables => _schemaIndex.Values;
    public IEnumerable<ITemplate> Templates => _templateIndex.Values;
    public IEnumerable<ITheme> Themes => _themeIndex.Values;

    public IEnumerable<ITable> FindAssociatedTables(IEntity entity)
    {
        if (_entitySchemasIndex.TryGetValue(entity.Id, out List<ITable>? result))
            return result;
        
        return Enumerable.Empty<ITable>();
    }

    public IEnumerable<IColumnDomain> FindColumnDomains(ITable table)
    {
        return _columnDomains.Where(x => x.TableId == table.Id);
    }

    private Dictionary<int,List<ITable>> LoadEntityTypeSchemas()
    {
        var result = new Dictionary<int, List<ITable>>();
        
        // Doing a raw select because RepoDb doesn't appear to support composite primary keys 
        foreach (var row in ExecuteQuery($"SELECT EntityId, SchemaId FROM EntityTypeSchemas"))
        {
            var entityId = (int)row.EntityId;
            var schemaId = (int)row.SchemaId;

            var schema = _schemaIndex[schemaId];

            if (!result.TryGetValue(entityId, out List<ITable>? schemas))
            {
                schemas = new List<ITable>();
                result.Add(entityId, schemas);
            }

            schemas.Add(schema);
        }
        
        return result;
    }

    public T? Find<T>(int id) where T : class, IEnvironmentItem
    {
        var lookup = GetIndex<T>(typeof(T).Name);
        return lookup.GetValueOrDefault(id);
    }

    public T FindRequired<T>(int id) where T : class, IEnvironmentItem
    {
        var result = Find<T>(id);
        if (result is null)
            throw new KeyNotFoundException($"No {typeof(T).Name} found with id {id}.");

        return result;
    }

    public IEnumerable<T> FindMany<T>(Predicate<T> predicate) where T : class, IEnvironmentItem
    {
        var lookup = GetIndex<T>(typeof(T).Name);
        return lookup.Values.Where(x => predicate(x));
    }

    public string? FindPropertyByName(string propertyName)
    {
        return _properties.GetValueOrDefault(propertyName);
    }
    
    /// <summary>
    /// Locates an entity type based on it's unique ID.
    /// </summary>
    /// <param name="id">The ID of the required entity type.</param>
    /// <returns>The corresponding entity type (null if not found)</returns>
    public static IEntity FindEntityById(int id)
    {
        return _repository.FindRequired<IEntity>(id);
    }

    /// <summary>
    /// The entity types that relate to the specified spatial type, regardless of the
    /// mapping layer they may be restricted to.
    /// </summary>
    /// <param name="t">The type(s) of interest</param>
    /// <returns>The entity types associated with the specified spatial type.</returns>
    public static IEnumerable<IEntity> FindEntityTypes(SpatialType t)
    {
        return _repository.EntityTypes.Where(x => x.IsValid(t));
    }

    /// <summary>
    /// The entity types that relate to the specified spatial type and mapping layer.
    /// </summary>
    /// <param name="t">The type(s) of interest</param>
    /// <param name="layer">The layer of interest (null for all layers)</param>
    /// <returns>The entity types associated with the specified spatial type and layer
    /// (i.e. entities that either refer explicitly to the layer, or which can be
    /// used on all layers).
    /// </returns>
    public static IEnumerable<IEntity> FindEntityTypes(SpatialType t, ILayer? layer)
    {
        if (layer is null || layer.Id == 0)
            return _repository.EntityTypes.Where(x => x.IsValid(t));
        
        // The entity type should always be associated with a layer, but it may be the
        // default layer (with an ID of 0).
        return _repository.EntityTypes.Where(x =>
            x.IsValid(t) && (x.Layer.Id == layer.Id || x.Layer.Id == 0));
    }

    /// <summary>
    /// Locates font information based on it's unique ID.
    /// </summary>
    /// <param name="id">The ID of the required font.</param>
    /// <returns>The corresponding font information (null if not found).</returns>
    public static IFont? FindFontById(int id)
    {
        return _repository.Find<IFont>(id);
    }

    /// <summary>
    /// Locates a text template based on it's unique ID.
    /// </summary>
    /// <param name="id">The ID of the required template.</param>
    /// <returns>The corresponding template.</returns>
    public static ITemplate FindTemplateById(int id)
    {
        return _repository.FindRequired<ITemplate>(id);
    }

    /// <summary>
    /// Locates a map layer based on it's unique ID.
    /// </summary>
    /// <param name="id">The ID of the required layer.</param>
    /// <returns>The corresponding layer (null if not found).</returns>
    public static ILayer FindLayerById(int id)
    {
        return _repository.FindRequired<ILayer>(id);
    }
    
    /// <inheritdoc cref="IEnvironmentRepository.CreateNewRecord"/>
    public AttributeRecord CreateNewRecord(ITable table)
    {
        Dictionary<string, object?> content = new();
        var columns = QueryTableColumns(table.TableName).ToArray();

        foreach (var column in columns)
        {
            if (!column.IsAutoGeneratedPrimaryKey)
                content.Add(column.Name, null);
        }

        return new AttributeRecord(table, columns, content);
    }

    /// <inheritdoc cref="IEnvironmentRepository.InsertRecord"/>
    public object? InsertRecord(AttributeRecord record)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();

            return connection.Insert(
                tableName: record.Table.TableName,
                entity: record.Content);
        }
    }

    /// <inheritdoc cref="IEnvironmentRepository.UpdateRecord"/>
    public void UpdateRecord(AttributeRecord record)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();

            connection.Update(
                tableName: record.Table.TableName,
                entity: record.Content);
        }
    }

    private IEnumerable<ColumnInfo> QueryTableColumns(string tableName)
    {
        string quotedTableName = QuoteSqliteIdentifier(tableName);

        foreach (var row in ExecuteQuery($"PRAGMA table_info({quotedTableName})"))
        {
            string name = row.name.ToString();
            string type = row.type?.ToString().ToUpper() ?? "TEXT";
            bool nullable = Convert.ToInt32(row.notnull) == 0;
            bool isPrimaryKey = Convert.ToInt32(row.pk) > 0;

            yield return new ColumnInfo(name, GetColumnType(type), nullable, isPrimaryKey);
        }
    }

    private static Type GetColumnType(string type)
    {
        return type switch
        {
            "INTEGER" => typeof(int),
            "REAL" => typeof(double),
            "TEXT" => typeof(string),
            _ => throw new NotImplementedException(type)
        };
    }

    private static string QuoteSqliteIdentifier(string identifier)
    {
        if (String.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier is not defined.", nameof(identifier));

        return "\"" + identifier.Replace("\"", "\"\"") + "\"";
    }

    /// <inheritdoc cref="IEnvironmentRepository.FindAttributeRecords"/>
    public AttributeRecord[] FindAttributeRecords(ITable table, IEnumerable<string> keys)
    {
        // The following does a query with an IN clause, but might want to consider using an in-memory temp table
        // PRAGMA temp_store = MEMORY;
        // CREATE TEMP TABLE temp_ids (FeatureId TEXT NOT NULL PRIMARY KEY)
        
        string[] distinctKeys = keys
            .Where(x => !String.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToArray();

        if (distinctKeys.Length == 0)
            return Array.Empty<AttributeRecord>();

        string quotedTableName = QuoteSqliteIdentifier(table.TableName);
        string quotedIdColumnName = QuoteSqliteIdentifier(table.IdColumnName);
        ColumnInfo[] columns = QueryTableColumns(table.TableName).ToArray();

        List<AttributeRecord> result = new();

        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();

            foreach (string[] keyBatch in distinctKeys.Chunk(100))
            {
                Dictionary<string, object?> parameters = new();
                List<string> parameterNames = new(keyBatch.Length);

                for (int i = 0; i < keyBatch.Length; i++)
                {
                    string parameterName = $"K{i}";
                    parameters.Add(parameterName, keyBatch[i]);
                    parameterNames.Add("@" + parameterName);
                }

                string sql = $"""
                              SELECT *
                              FROM {quotedTableName}
                              WHERE {quotedIdColumnName} IN ({String.Join(", ", parameterNames)})
                              """;

                foreach (var row in connection.ExecuteQuery(sql, parameters))
                {
                    Dictionary<string, object?> content = new();

                    foreach (var value in (IDictionary<string, object?>)row)
                        content[value.Key] = value.Value;

                    result.Add(new AttributeRecord(table, columns, content));
                }
            }
        }

        return result.ToArray();
    }

    /// <inheritdoc cref="IEnvironmentRepository.UpdateIdGroup"/>
    public IIdGroup UpdateIdGroup(IIdGroup group, int maxUsedId)
    {
        var row = group as IdGroupRow;
        if (row is null)
            throw new ArgumentException("Unexpected group type.");
            
        row.MaxUsedId = maxUsedId;
        int rowCount = Update(row);
        
        if (rowCount != 1)
            throw new InvalidOperationException("Unexpected row count for update: " + rowCount);

        return row;
    }
}