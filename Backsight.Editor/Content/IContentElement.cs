using System;

namespace Backsight.Editor.Content
{
    /// <summary>
    /// Something that will be serialized as an XML element
    /// </summary>
    public interface IContentElement : IContent
    {
        void WriteAttributes(ContentWriter writer);
        void WriteChildElements(ContentWriter writer);

        void ReadAttributes(ContentReader reader);
        void ReadChildElements(ContentReader reader);
    }
}
