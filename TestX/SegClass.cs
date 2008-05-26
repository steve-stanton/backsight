using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TestX
{
    //[XmlRoot(Namespace = "TestSpace")]
    public class MySegClass : MyAbClass
    {
        //[XmlAttribute]
        public int Start;
        //[XmlAttribute]
        public int End;

        internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Start", Start.ToString());
            writer.WriteAttributeString("End", End.ToString());
            base.WriteXml(writer);
        }

        internal virtual void ReadXml(XmlReader reader)
        {
            Start = Int32.Parse(reader.GetAttribute("Start"));
            End = Int32.Parse(reader.GetAttribute("End"));
            base.ReadXml(reader);
        }
    }
}
