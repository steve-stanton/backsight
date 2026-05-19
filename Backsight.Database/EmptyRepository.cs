using Backsight.Environment;

namespace Backsight.Database;

public class EmptyRepository : IEnvironmentRepository
{
    public string Name => "";
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
    public IEnumerable<IColumnDomain> FindColumnDomains(ITable table) => Enumerable.Empty<IColumnDomain>();
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

    public IEditColumnDomain CreateColumnDomain()
    {
        throw new NotImplementedException();
    }

    public IEditDomainTable CreateDomainTable()
    {
        throw new NotImplementedException();
    }

    public IEditEntity CreateEntity()
    {
        throw new NotImplementedException();
    }

    public IEditFont CreateFont()
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
}