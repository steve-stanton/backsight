using Backsight.Database;

namespace Backsight.Environment.Editor;

internal static class IEnvironmentRepositoryEx
{
    extension (IEnvironmentRepository repo)
    {
        /// <summary>
        /// Returns the names of all user-defined tables (excluding Backsight system tables,
        /// and database catalogs)
        /// </summary>
        /// <returns>The user-defined tables in the database</returns>
        internal string[] GetUserTables()
        {
            var systemTables = new HashSet<string>()
            {
                "ColumnDomains",
                "DomainTables",
                "EntityTypeSchemas",
                "EntityTypes",
                "Domains",
                "Fonts",
                "IdGroups",
                "Layers",
                "Properties",
                "Schemas",
                "SchemaTemplates",
                "SysId",
                "Templates",
                "Themes",
                "Zones"
            };
        
            return EnvironmentRepository.Current
                .QueryTableNames()
                .Except(systemTables)
                .OrderBy(x => x)
                .ToArray();
        }
    }
}