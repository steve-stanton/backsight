using Backsight.Environment;

namespace Backsight.Database;

public interface IEnvironmentRepository
{
    string Name { get; }
    
    // IEntity[] EntityTypes(SpatialType)
    // IEntity[] EntityTypes(SpatialType, ILayer)
    // ok (as enumerable) ITheme[] Themes
    // ok (as enumerable) ILayer[] Layers
    // ITable[] Tables
    // IIdGroup[] IdGroups
    // ok (as enumerable) IEntity[] EntityTypes
    // ITable[] Schemas(SpatialType, ILayer)
    // IEntity FindBlankEntity
    // IProperty FindPropertyByName(string propertyName)
    // IEntity FindEntityById(int entityId)
    // ok ILayer FindLayerById(int layerId)
    // ok ITemplate FindTemplateById(int templateId)
    // ok IFont FindFontById(int fontId)

    IEnumerable<IEntity> EntityTypes { get; }
    IEnumerable<IFont> Fonts { get; }
    IEnumerable<IIdGroup> IdGroups { get; }
    IEnumerable<ILayer> Layers { get; }
    //IEnumerable<IProperty> Properties { get; }
    IEnumerable<ITable> Tables { get; }
    IEnumerable<ITemplate> Templates { get; }
    IEnumerable<ITheme> Themes { get; }

    /// <summary>
    /// Finds the user data tables that normally hold attributes associated with a specific entity type. 
    /// </summary>
    /// <param name="entity">The entity type.</param>
    /// <returns>Details for the attribute tables normally associated with the entity type.</returns>
    IEnumerable<ITable> FindAssociatedTables(IEntity entity);

    /// <summary>
    /// Finds the columns in an attribute table that are associated with domain values.
    /// </summary>
    /// <param name="table">The attribute table associated with an entity.</param>
    /// <returns>Details for any columns in the table that have an associated <see cref="IDomainTable"/>.</returns>
    IEnumerable<IColumnDomain> FindColumnDomains(ITable table);
    
    T? Find<T>(int id) where T : class, IEnvironmentItem;
    T FindRequired<T>(int id) where T : class, IEnvironmentItem;
    IEnumerable<T> FindMany<T>(Predicate<T> predicate) where T : class, IEnvironmentItem;
}

// string ITheme.Name
// ILayer[] ITheme.Layers
// ITheme ILayer.Theme
// string ILayer.Name
// string IEntity.Name
// IIdGroup IEntity.IdGroup
