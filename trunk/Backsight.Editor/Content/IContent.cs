using System;
using System.Xml;

namespace Backsight.Editor.Content
{
    public interface IContent
    {
        void WriteContent(XmlWriter w, string name);
        void WriteAttributes(XmlWriter w);
        void WriteChildElements(XmlWriter w);
    }
}
