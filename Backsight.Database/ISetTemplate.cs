using Backsight.Environment;

namespace Backsight.Database;

public interface ISetTemplate : ISetter
{
    ITable Schema { set; }
    string Name { set; }
    string Format { set; }
}