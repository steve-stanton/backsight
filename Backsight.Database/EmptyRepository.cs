using Backsight.Environment;

namespace Backsight.Database;

public class EmptyRepository : IEnvironmentRepository
{
    public string Name => "";
    public IEnumerable<string> QueryTableNames()
    {
        return Enumerable.Empty<string>();
    }

    public IEnumerable<ColumnInfo> QueryTableColumns(string tableName)
    {
        return Enumerable.Empty<ColumnInfo>();
    }

    public IEnumerable<IDomainTable> DomainTables => Enumerable.Empty<IDomainTable>();
    public IEnumerable<IEntity> EntityTypes => Enumerable.Empty<IEntity>();
    public IEnumerable<IFont> Fonts => Enumerable.Empty<IFont>();
    public IEnumerable<IIdGroup> IdGroups => Enumerable.Empty<IIdGroup>();
    public IEnumerable<ILayer> Layers => Enumerable.Empty<ILayer>();
    public IEnumerable<IProperty> Properties => Enumerable.Empty<IProperty>();
    public IEnumerable<ITable> Tables => Enumerable.Empty<ITable>();
    public IEnumerable<ITemplate> Templates => Enumerable.Empty<ITemplate>();
    public IEnumerable<ITheme> Themes => Enumerable.Empty<ITheme>();

    public IEnumerable<ITable> FindAssociatedTables(IEntity entity) => Enumerable.Empty<ITable>();
    public void SaveAssociatedTables(IEntity entity, IEnumerable<ITable> tables)
    {
        throw new NotImplementedException();
    }

    public void SaveAssociatedEntities(IIdGroup group, IEnumerable<IEntity> entities)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IColumnDomain> FindColumnDomains(ITable table) => Enumerable.Empty<IColumnDomain>();
    public IColumnDomain CreateColumnDomain(ITable parentTable, string columnName, IDomainTable domainTable)
    {
        throw new NotImplementedException();
    }

    public void SaveColumnDomain(IColumnDomain columnDomain)
    {
        throw new NotImplementedException();
    }

    public void DeleteColumnDomain(IColumnDomain columnDomain)
    {
        throw new NotImplementedException();
    }

    public AttributeRecord CreateNewRecord(ITable table)
    {
        throw new NotImplementedException();
    }

    public object? InsertRecord(AttributeRecord record)
    {
        throw new NotImplementedException();
    }

    public void UpdateRecord(AttributeRecord record)
    {
        throw new NotImplementedException();
    }

    public AttributeRecord[] FindAttributeRecords(ITable table, IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public IIdGroup UpdateIdGroup(IIdGroup group, int maxUsedId)
    {
        throw new NotImplementedException();
    }

    public T? Find<T>(int id) where T : class, IEnvironmentItem
    {
        throw new NotImplementedException();
    }

    public T FindRequired<T>(int id) where T : class, IEnvironmentItem
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> FindMany<T>(Predicate<T> predicate) where T : class, IEnvironmentItem
    {
        throw new NotImplementedException();
    }

    public string? FindPropertyByName(string propertyName)
    {
        return null;
    }

    public TSetter GetSetter<TItem, TSetter>(TItem item) where TItem : class, IEnvironmentItem where TSetter : class, ISetter
    {
        throw new NotImplementedException();
    }

    public void SaveChanges<TItem, TSetter>(TItem item, TSetter setter) where TItem : class, IEnvironmentItem where TSetter : class, ISetter
    {
        throw new NotImplementedException();
    }

    public IEditColumnDomain CreateColumnDomain()
    {
        throw new NotImplementedException();
    }

    public IEditDomainTable CreateDomainTable()
    {
        throw new NotImplementedException();
    }

    public IEntity CreateEntity()
    {
        throw new NotImplementedException();
    }

    public IFont CreateFont()
    {
        throw new NotImplementedException();
    }

    public IEditIdGroup CreateIdGroup()
    {
        throw new NotImplementedException();
    }

    public IEditProperty CreateProperty()
    {
        throw new NotImplementedException();
    }

    public IEditTable CreateTableAssociation()
    {
        throw new NotImplementedException();
    }

    public IEditTemplate CreateTemplate()
    {
        throw new NotImplementedException();
    }

    public IEditTheme CreateTheme()
    {
        throw new NotImplementedException();
    }

    public IEditLayer CreateLayer()
    {
        throw new NotImplementedException();
    }
    
    public TItem CreateNewItem<TItem>() where TItem : class, IEnvironmentItem
    {
        throw new NotImplementedException();
    }
}