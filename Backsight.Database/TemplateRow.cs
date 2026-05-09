using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation of a template for a text feature that
/// refers to content found is an associated attribute table.
/// </summary>
[Map("Templates")]
internal partial class TemplateRow : Row, ITemplate
{
    [Primary] public int TemplateId { get; set; }
    public string Name { get; set; } = "";
    public string TemplateFormat { get; set; } = "";
    public int SchemaId { get; set; }
}

// Additional properties to satisfy the readonly interface.
internal partial class TemplateRow
{
    public int Id => TemplateId;
    public ITable Schema => Repository.FindRequired<ITable>(SchemaId);
    public bool IsNew => false; // is this really needed?
    public string Format => TemplateFormat;
}