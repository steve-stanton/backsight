using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TestX
{
    //[XmlRoot(Namespace = "TestSpace")]
    public class MyArcClass : MyAbClass
    {
        //[XmlAttribute]
        public int Center;
        //[XmlAttribute]
        public int Radius;

        public override string ToString()
        {
            return String.Format("AbValue={0} Center={1} Radius={2}", AbValue, Center, Radius);
        }

        internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("xsi", "type", null, "ced:MyArcClass");
            writer.WriteAttributeString("Center", Center.ToString());
            writer.WriteAttributeString("Radius", Radius.ToString());
            base.WriteXml(writer);
        }

        internal override void ReadXml(XmlReader reader)
        {
            Console.WriteLine("reading an ArcClass");
            Center = Int32.Parse(reader.GetAttribute("Center"));
            Radius = Int32.Parse(reader.GetAttribute("Radius"));
            base.ReadXml(reader);
        }
    }
}
