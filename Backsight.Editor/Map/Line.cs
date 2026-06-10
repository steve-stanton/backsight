namespace Backsight.Editor.Map;

class Line : FeatureBase
{
    internal LineFeature LineFeature { get; }
    
    internal Line(LineFeature line) : base(line)
    {
        LineFeature = line;
    }
}