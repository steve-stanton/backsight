using System;
using System.Xml;

namespace Backsight.Editor.Content
{
    public struct FeatureInfo : IContent
    {
        public string Id;
        public ISpatialKey Key;
        public int Type;

        #region IContent Members

        public void WriteContent(XmlWriter writer, string name)
        {
            //writer.WriteStartElement(name, "Backsight");
            writer.WriteAttributeString("xsi", "type", null, GetType().Name);
            WriteAttributes(writer);
            WriteChildElements(writer);
            writer.WriteEndElement();
        }

        public void WriteAttributes(System.Xml.XmlWriter w)
        {
            w.WriteAttributeString("Id", Id);
            w.WriteAttributeString("Type", Type.ToString());
        }

        public void WriteChildElements(System.Xml.XmlWriter w)
        {
            Key.WriteContent(w, "Key");
        }

        #endregion
    }
}
