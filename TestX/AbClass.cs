using System;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using Backsight;
using Backsight.Editor;

namespace TestX
{
    abstract class MyAbClass : IXmlContent
    {
        public int AbValue;

        public virtual void WriteContent(XmlContentWriter writer)
        {
            writer.WriteInt("AbValue", AbValue);
        }

        public virtual void ReadContent(XmlContentReader reader)
        {
            AbValue = reader.ReadInt("AbValue");
        }
    }
}
