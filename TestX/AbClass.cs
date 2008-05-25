using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace TestX
{
    [XmlRoot(Namespace = "TestSpace")]
    public abstract class MyAbClass
    {
        [XmlAttribute]
        public int AbValue;

        abstract internal void WriteXml(XmlWriter writer);

        internal virtual void WriteElement(XmlWriter writer, string localName)
        {        
            writer.WriteStartElement(localName);
            WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}
