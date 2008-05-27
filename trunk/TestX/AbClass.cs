using System;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace TestX
{
    abstract class MyAbClass
    {
        public int AbValue;

        // Writes this object with the a name that matches the class name
        internal void WriteElement(XmlWriter writer)
        {
            WriteElement(writer, GetType().Name);
        }

        // Writes this object with the specified name
        internal void WriteElement(XmlWriter writer, string localName)
        {        
            writer.WriteStartElement(localName);
            writer.WriteAttributeString("xsi", "type", null, "ced:"+GetType().Name);
            writer.WriteAttributeString("AbValue", AbValue.ToString());
            WriteContent(writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the content of this class. This is called by <see cref="WriteElement"/>
        /// after the class type (xsi:type) has been written, and after any attributes
        /// and elements that are part of the base class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        internal abstract void WriteContent(XmlWriter writer);

        /// <summary>
        /// Creates an object that was previously written using <see cref="WriteElement"/>.
        /// </summary>
        /// <param name="reader">The reading tool, positioned at the start of the element</param>
        /// <returns></returns>
        //static internal MyAbClass ReadElement(XmlReader reader)
        //{
        //}

        // Derived classes are expected to override and call this at the start
        internal virtual void ReadXml(XmlReader reader)
        {
            AbValue = Int32.Parse(reader["AbValue"]);
        }

        static internal MyAbClass FromXml(XmlReader reader)
        {
            string type = reader["xsi:type"];
            string[] names = type.Split(':');
            Debug.Assert(names.Length == 2);
            Debug.Assert(names[0] == "ced");
            string className = names[1];

            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            string typeName = String.Format("{0}.{1}", assemblyName, className);
            Type t = Type.GetType(typeName);
            if (t == null)
                throw new Exception(String.Format("Couldn't create object for "+type));

            ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
            MyAbClass result = (MyAbClass)ci.Invoke(null);
            result.ReadXml(reader);
            return result;
        }
    }
}
