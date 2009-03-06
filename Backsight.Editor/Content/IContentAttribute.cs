using System;

namespace Backsight.Editor.Content
{
    /// <summary>
    /// Something that will be serialized as an XML attribute
    /// </summary>
    public interface IContentAttribute : IContent
    {
        string AttributeString { get; }
    }
}
