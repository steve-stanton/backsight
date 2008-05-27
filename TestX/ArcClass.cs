using System;
using System.Xml;

namespace TestX
{
    class MyArcClass : MyAbClass
    {
        public int Center;
        public int Radius;

        public override string ToString()
        {
            return String.Format("AbValue={0} Center={1} Radius={2}", AbValue, Center, Radius);
        }

        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            Center = Int32.Parse(reader["Center"]);
            Radius = Int32.Parse(reader["Radius"]);
        }

        internal override void WriteContent(XmlWriter writer)
        {
            writer.WriteAttributeString("Center", Center.ToString());
            writer.WriteAttributeString("Radius", Radius.ToString());
        }
    }
}
