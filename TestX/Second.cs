using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TestX
{
    [XmlType("Second")]
    [XmlRoot(Namespace="TestSpace")]
    public class Second : Base
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public string Name;

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
    }
}
