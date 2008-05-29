using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using Backsight;

namespace TestX
{
    //[XmlType(TypeName = "First", Namespace = "TestSpace")]
    //[XmlType("First")]
    //[XmlRoot(Namespace="TestSpace")]
    class First : Base
    {
        //[XmlAttribute]
        public int Id;

        //[XmlAttribute]
        public string Name;

        public MyAbClass More;

        internal First()
        {
        }

        public First(XmlContentReader reader)
        {
            ReadContent(reader);
        }

        public override string ToString()
        {
            if (More==null)
                return String.Format("Type={0}, ID={1}, Name={2}, with no more", GetType().Name, Id, Name);
            else
                return String.Format("Type={0}. ID={1}, Name={2}, with more... {3}", GetType().Name, Id, Name, More.ToString());
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            Console.WriteLine("writing " + Id);
            writer.WriteAttributeString("Id", Id.ToString());
            writer.WriteAttributeString("Name", Name.ToString());

            if (More!=null)
                More.WriteElement(writer, "More");
        }

        internal override void ReadXml(XmlReader reader)
        {
            // The attributes don't need to be "read"
            Id = Int32.Parse(reader["Id"]);
            Name = reader["Name"];

            reader.Read();
            if (reader.IsStartElement("More"))
                More = MyAbClass.FromXml(reader);
        }

        public override void WriteContent(Backsight.XmlContentWriter writer)
        {
            writer.WriteInt("Id", Id);
            writer.WriteString("Name", Name);
            writer.WriteElement("More", More);
        }

        public override void ReadContent(XmlContentReader reader)
        {
            Id = reader.ReadInt("Id");
            Name = reader.ReadString("Name");
            More = (MyAbClass)reader.ReadElement("More");
        }
    }
}
