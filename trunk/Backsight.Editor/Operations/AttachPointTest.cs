using System;
//using System.Xml;
using System.Xml.Serialization;

namespace Backsight.Editor.Operations
{
    [XmlRoot]
    //[XmlSchemaProvider(GetXmlSchema)]
    public class AttachPointTest
    {
        [XmlAttribute]
        public uint EditSequence;

        [XmlAttribute]
        public uint PositionRatio;
    }
}
