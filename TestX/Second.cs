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

        internal override string TestType
        {
            get { return "The Second Type"; }
        }

        internal override string ToXml()
        {
            return String.Format(
            "<?xml version=\"1.0\"?> <Second xmlns=\"TestSpace\" Id=\"{0}\" Name=\"{1}\"/>",
            Id, Name);
        }
    }
}
