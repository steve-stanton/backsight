using System;
using System.Xml.Serialization;

namespace TestX
{
    [XmlIncludeAttribute(typeof(Second))]
    [XmlIncludeAttribute(typeof(First))]
    [XmlType(TypeName = "Base", Namespace = "TestSpace")]
    public abstract class Base
    {
        abstract internal string TestType { get; }
        abstract internal string ToXml();
    }
}
