using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Backsight;
using Backsight.Editor;

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

        public override void WriteContent(XmlContentWriter writer)
        {
            writer.WriteInt("Id", Id);
            writer.WriteString("Name", Name);
        }

        public override void ReadContent(XmlContentReader reader)
        {
            Id = reader.ReadInt("Id");
            Name = reader.ReadString("Name");
        }
    }
}
