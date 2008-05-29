using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Backsight;

namespace TestX
{
    //[XmlType("Second")]
    //[XmlRoot(Namespace="TestSpace")]
    class Second : Base
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public string Name;

        internal Second()
        {
        }

        public Second(XmlContentReader reader)
        {
            ReadContent(reader);
        }

        public override string ToString()
        {
            return String.Format("Type={2}, Id={0}, Name={1}", Id, Name, GetType().Name);
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id.ToString());
            writer.WriteAttributeString("Name", Name.ToString());
        }

        internal override void ReadXml(System.Xml.XmlReader reader)
        {
            Id = Int32.Parse(reader["Id"]);
            Name = reader["Name"];
        }

        public override void WriteContent(Backsight.XmlContentWriter writer)
        {
            writer.WriteInt("Id", Id);
            writer.WriteString("Name", Name);
        }

        public void ReadContent(XmlContentReader reader)
        {
            Id = reader.ReadInt("Id");
            Name = reader.ReadString("Name");
        }
    }
}
