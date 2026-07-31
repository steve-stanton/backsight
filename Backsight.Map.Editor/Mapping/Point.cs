using Backsight.Model;

namespace Backsight.Map.Editor.Mapping;

class Point : FeatureBase
{
    internal IMapPoint PointFeature { get; }
    
    internal Point(IMapPoint point) : base(point)
    {
        PointFeature = point;
    }
}