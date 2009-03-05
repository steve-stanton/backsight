using System;
using System.Xml;

namespace Backsight.Editor.Content
{
    public struct StringKey : ISpatialKey
    {
        public string Value;

        public StringKey(string value)
        {
            Value = value;
        }

        #region IContent Members

        public void WriteContent(XmlWriter writer, string name)
        {
            writer.WriteStartElement(name, "Backsight");
            writer.WriteAttributeString("xsi", "type", null, GetType().Name);
            WriteAttributes(writer);
            WriteChildElements(writer);
            writer.WriteEndElement();
        }

        public void WriteAttributes(System.Xml.XmlWriter w)
        {
            w.WriteAttributeString("Key", Value);
        }

        public void WriteChildElements(System.Xml.XmlWriter w)
        {
        }

        #endregion
    }
}
