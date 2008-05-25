using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace TestX
{
    //[XmlType(TypeName = "First", Namespace = "TestSpace")]
    [XmlType("First")]
    [XmlRoot(Namespace="TestSpace")]
    public class First : Base
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public string Name;

        public MyAbClass More;

        internal override string TestType
        {
            get { return "The First Type"; }
        }

        public override string ToString()
        {
            if (More==null)
                return String.Format("ID={1}, TestType='{0}' with no more", TestType, Id);
            else
                return String.Format("ID={2}, TestType='{0}' with more... {1}", TestType, More.ToString(), Id);
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id.ToString());
            writer.WriteAttributeString("Name", Name.ToString());

            if (More!=null)
                More.WriteElement(writer, "More");
            else
                writer.WriteElementString("More", String.Empty);
        }
    }
}
