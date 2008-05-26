using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace TestX
{
    //[XmlType(TypeName = "First", Namespace = "TestSpace")]
    [XmlType("First")]
    [XmlRoot(Namespace="TestSpace")]
    public class First : Base
    {
        //[XmlAttribute]
        public int Id;

        //[XmlAttribute]
        public string Name;

        public MyAbClass More;

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
    }
}
