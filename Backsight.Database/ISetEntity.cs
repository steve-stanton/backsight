using Backsight.Environment;

namespace Backsight.Database;

public interface ISetEntity : ISetter
{
    string Name { set; }
    bool IsPointValid { set; }
    bool IsLineValid { set; }
    bool IsLineAutoTrimmed { set; }
    bool IsPolygonValid { set; }
    bool IsPolygonBoundaryValid { set; }
    bool IsTextValid { set; }
    IIdGroup? IdGroup { set; }
    ILayer? Layer { set; }
    IFont? Font { set; }
}