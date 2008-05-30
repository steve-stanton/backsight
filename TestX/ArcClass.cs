using System;
using System.Xml;
using Backsight;

namespace TestX
{
    class MyArcClass : MyAbClass
    {
        public int Center;
        public int Radius;

        internal MyArcClass()
        {
        }

        public MyArcClass(XmlContentReader reader)
        {
            ReadContent(reader);
        }

        public override string ToString()
        {
            return String.Format("AbValue={0} Center={1} Radius={2}", AbValue, Center, Radius);
        }

        public override void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);
            writer.WriteInt("Center", Center);
            writer.WriteInt("Radius", Radius);
        }

        public override void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);
            Center = reader.ReadInt("Center");
            Radius = reader.ReadInt("Radius");
        }
    }
}
