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

        public override string ToString()
        {
            return String.Format("Id={0}", Id);
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id.ToString());
            writer.WriteAttributeString("Name", Name.ToString());
        }
    }
}
