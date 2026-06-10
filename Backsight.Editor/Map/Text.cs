namespace Backsight.Editor.Map;

class Text : FeatureBase
{
    internal TextFeature TextFeature { get; }
    
    internal Text(TextFeature text) : base(text)
    {
        TextFeature = text;
    }
}