using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation of a theme (involving a hierarchy of <see cref="ILayer"/>).
/// </summary>
[Map("Themes")]
internal partial class ThemeRow
{
    [Primary] public int ThemeId { get; set; }
    public string Name { get; set; } = "";
}

// Additional properties to satisfy the readonly interface.
internal partial class ThemeRow : Row, ITheme
{
    public int Id => ThemeId;
    public ILayer[] Layers => Repository
        .FindMany<ILayer>(x => x.Id == ThemeId)
        .OrderBy(x => x.ThemeSequence)
        .ToArray();
}