using Backsight.Environment;

namespace Backsight.Database;

public interface ISetLayer : ISetter
{
    string Name { set; }
    IEntity DefaultPointType { set; }
    IEntity DefaultLineType { set; }
    IEntity DefaultTextType { set; }
    IEntity DefaultPolygonType { set; }
}