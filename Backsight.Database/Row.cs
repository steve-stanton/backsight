using System.ComponentModel.DataAnnotations.Schema;

namespace Backsight.Database;

internal abstract class Row
{
    protected const string YES = "y";
    protected const string NO = "n";

    [NotMapped] public IEnvironmentRepository Repository { get; set; } = null!;
}