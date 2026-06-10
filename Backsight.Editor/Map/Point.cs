namespace Backsight.Editor.Map;

class Point : FeatureBase
{
    internal PointFeature PointFeature { get; }
    
    internal Point(PointFeature point) : base(point)
    {
        PointFeature = point;
    }
}