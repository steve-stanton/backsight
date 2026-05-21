using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

[Map("Properties")]
public record PropertyItem(string Name, string Value, string Description = "") : IProperty;