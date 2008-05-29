using System;
using System.Xml;
using Backsight;

namespace TestX
{
    class MySegClass : MyAbClass
    {
        public int Start;
        public int End;

        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            Start = Int32.Parse(reader["Start"]);
            End = Int32.Parse(reader["End"]);
        }

        internal override void WriteContent(XmlWriter writer)
        {
            writer.WriteAttributeString("Start", Start.ToString());
            writer.WriteAttributeString("End", End.ToString());
        }

        public virtual void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);
            writer.WriteInt("Start", Start);
            writer.WriteInt("End", End);
        }
    }
}
