using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TestX
{
    //[XmlType("Third")]
    [XmlRoot(Namespace = "TestSpace")]
    public class Third : Second
    {
    }
}
