using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace TestX
{
    //[XmlRoot(Namespace = "TestSpace")]
    public abstract class MyAbClass
    {
        //[XmlAttribute]
        public int AbValue;

        internal virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("AbValue", AbValue.ToString());
        }

        internal virtual void WriteElement(XmlWriter writer, string localName)
        {        
            writer.WriteStartElement(localName);
            WriteXml(writer);
            writer.WriteEndElement();
        }

        static internal MyAbClass FromXml(XmlReader reader)
        {
            string type = reader["xsi:type"];
            //Console.WriteLine("Type=" + type);
            string[] names = type.Split(':');
            Debug.Assert(names.Length == 2);
            Debug.Assert(names[0] == "ced");
            string className = names[1];

            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            string typeName = String.Format("{0}.{1}", assemblyName, className);
            Type t = Type.GetType(typeName);
            if (t == null)
                Console.WriteLine("didn't get type for " + typeName);

            ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
            MyAbClass result = (MyAbClass)ci.Invoke(null);
            result.ReadXml(reader);
            return result;
        }

        // Derived classes are expected to override
        internal virtual void ReadXml(XmlReader reader)
        {
            AbValue = Int32.Parse(reader["AbValue"]);
        }
    }
}
