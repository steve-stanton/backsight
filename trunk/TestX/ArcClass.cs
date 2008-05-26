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

        internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("xsi", "type", null, "ced:MyArcClass");
            writer.WriteAttributeString("Center", Center.ToString());
            writer.WriteAttributeString("Radius", Radius.ToString());
            base.WriteXml(writer);
        }
    }
}
