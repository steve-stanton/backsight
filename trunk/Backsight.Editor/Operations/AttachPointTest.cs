using System;
//using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Backsight.Editor.Operations
{
    [XmlRoot]
    [XmlSchemaProvider("GetXmlSchema")]
    public class AttachPointTest
    {
        [XmlAttribute]
        public uint EditSequence;

        [XmlAttribute]
        public uint PositionRatio;

        public static XmlSchema GetXmlSchema()
        {
            return null;
        }
    }
}
