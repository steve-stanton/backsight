using Backsight.Model;

namespace Backsight.Map.Editor.Mapping;

class Point : FeatureBase
{
    internal PointFeature Feature { get; }
    
    internal Point(PointFeature point) : base(point)
    {
        Feature = point;
    }
}