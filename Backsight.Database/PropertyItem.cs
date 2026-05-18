using Backsight.Environment;

namespace Backsight.Database;

public record PropertyItem(string Name, string Value) : IProperty;