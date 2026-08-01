using Backsight.Model;

namespace Backsight.Map.Editor.Mapping;

class Line : FeatureBase
{
    internal LineFeature Feature { get; }
    
    internal Line(LineFeature line) : base(line)
    {
        Feature = line;
    }
}