using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TestX
{
    [XmlType(TypeName = "First", Namespace = "TestSpace")]
    [XmlRoot(Namespace="TestSpace")]
    public class First : Base
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public string Name;

        internal override string TestType
        {
            get { return "The First Type"; }
        }

        internal override string ToXml()
        {
            return String.Format(
            "<?xml version=\"1.0\"?> <First xmlns=\"TestSpace\" Id=\"{0}\" Name=\"{1}\"/>",
            Id, Name);
        }
    }
}
