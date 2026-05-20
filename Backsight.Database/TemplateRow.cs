using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation of a template for a text feature that
/// refers to content found is an associated attribute table.
/// </summary>
[Map("Templates")]
internal partial class TemplateRow
{
    [Primary] public int TemplateId { get; set; }
    public string Name { get; set; } = "";
    public string TemplateFormat { get; set; } = "";
    public int SchemaId { get; set; }
}

// Additional properties to satisfy interfaces.
internal partial class TemplateRow : Row, ITemplate, ISetTemplate
{
    public override string ToString() => Name;
    public int Id
    {
        get => TemplateId;
        set => TemplateId = value;
    }

    public ITable Schema
    {
        get => Repository.FindRequired<ITable>(SchemaId);
        set => SchemaId = value.Id;
    }

    public string Format
    {
        get => TemplateFormat;
        set => TemplateFormat = value;
    }
}