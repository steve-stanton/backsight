using Backsight.Environment;

namespace Backsight.Database;

public class EmptyRepository : IEnvironmentRepository
{
    public string Name => "";
    public IEnumerable<IEntity> EntityTypes => Enumerable.Empty<IEntity>();
    public IEnumerable<IFont> Fonts => Enumerable.Empty<IFont>();
    public IEnumerable<IIdGroup> IdGroups => Enumerable.Empty<IIdGroup>();
    public IEnumerable<ILayer> Layers => Enumerable.Empty<ILayer>();
    public IEnumerable<ITable> Tables => Enumerable.Empty<ITable>();
    public IEnumerable<ITemplate> Templates => Enumerable.Empty<ITemplate>();
    public IEnumerable<ITheme> Themes => Enumerable.Empty<ITheme>();

    public IEnumerable<ITable> FindAssociatedTables(IEntity entity) => Enumerable.Empty<ITable>();
    public IEnumerable<IColumnDomain> FindColumnDomains(ITable table) => Enumerable.Empty<IColumnDomain>();

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
}