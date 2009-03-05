using System;

namespace Backsight.Editor.Content
{
    public struct GroupKey : ISpatialKey
    {
        int GroupId;
        uint Value;

        public GroupKey(int groupId, uint value)
        {
            GroupId = groupId;
            Value = value;
        }

        #region IContent Members

        public void WriteContent(System.Xml.XmlWriter writer, string name)
        {
            writer.WriteStartElement(name, "Backsight");
            writer.WriteAttributeString("xsi", "type", null, GetType().Name);
            WriteAttributes(writer);
            WriteChildElements(writer);
            writer.WriteEndElement();
        }

        public void WriteAttributes(System.Xml.XmlWriter w)
        {
            w.WriteAttributeString("Group", GroupId.ToString());
            w.WriteAttributeString("Value", Value.ToString());
        }

        public void WriteChildElements(System.Xml.XmlWriter w)
        {
        }

        #endregion
    }
}
