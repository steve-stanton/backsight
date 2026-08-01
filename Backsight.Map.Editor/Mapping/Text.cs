using Backsight.Model;

namespace Backsight.Map.Editor.Mapping;

class Text : FeatureBase
{
    internal TextFeature Feature { get; }
    
    internal Text(TextFeature text) : base(text)
    {
        Feature = text;
    }
}