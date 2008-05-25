using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TestX
{
    [XmlRoot(Namespace = "TestSpace")]
    public class MyArcClass : MyAbClass
    {
        [XmlAttribute]
        public int Center;
        [XmlAttribute]
        public int Radius;
        /*
        internal override void WriteElement(XmlWriter writer, string localName)
        {
            writer.WriteStartElement(
            WriteXml(writer);
            writer.WriteEndElement();
        }
        */
        internal override void WriteXml(XmlWriter writer)
        {
            //writer.WriteString(" xsi:type=\"ArcClass\" ");
            //writer.WriteAttributeString("xsi", "type", null, "Backsight.ArcClass");
            //writer.WriteAttributeString("xsi", "type", "Backsight", "ArcClass");
            //writer.WriteAttributeString("xsi", "type=\"ArcClass\"");
            //writer.WriteRaw(" xsi:type=\"ArcClass\" ");
            writer.WriteAttributeString("Center", Center.ToString());
            writer.WriteAttributeString("Radius", Radius.ToString());
        }
    }
}
