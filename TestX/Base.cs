using System;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Data.SqlTypes;

namespace TestX
{
    [XmlIncludeAttribute(typeof(First))]
    [XmlIncludeAttribute(typeof(Second))]
    [XmlIncludeAttribute(typeof(Third))]
    [XmlType(TypeName="Base", Namespace="TestSpace")]
    public abstract class Base
    {
        abstract internal string TestType { get; }

        internal static Dictionary<RuntimeTypeHandle, XmlSerializer> GetSerializers()
        {
            Type[] types = GetDerivedTypes();
            XmlSerializer[] xsa = XmlSerializer.FromTypes(types);

            Dictionary<RuntimeTypeHandle, XmlSerializer> result = new Dictionary<RuntimeTypeHandle, XmlSerializer>(types.Length);
            for (int i=0; i<types.Length; i++)
                result.Add(types[i].TypeHandle, xsa[i]);

            return result;
        }

        /// <summary>
        /// Obtains the class types derived from this class (in the current assembly).
        /// This gives all types in the class hierarchy, not just the immediate
        /// sub-classes. Ignores abstract classes.
        /// </summary>
        /// <returns></returns>
        internal static Type[] GetDerivedTypes()
        {
            return GetDerivedTypes<Base>();
        }

        static Type[] GetDerivedTypes<T>()
        {
            List<Type> result = new List<Type>();

            Type baseType = typeof(T);
            foreach (Type t in baseType.Assembly.GetTypes())
            {
                if (t!=null && t.IsSubclassOf(baseType) && !t.IsAbstract)
                    result.Add(t);
            }

            return result.ToArray();
        }

        internal static Type[] GetIncludedTypes()
        {
            List<Type> result = new List<Type>();

            Type t = typeof(Base);
            object[] atts = t.GetCustomAttributes(false);
            foreach (object o in atts)
            {
                if (o is XmlIncludeAttribute)
                {
                    XmlIncludeAttribute xia = (XmlIncludeAttribute)o;
                    result.Add(xia.Type);
                }
            }

            return result.ToArray();
        }

        internal static void CheckIncludedTypes()
        {
            Type[] derivedTypes = GetDerivedTypes();
            Type[] includedTypes = GetIncludedTypes();

            foreach (Type dt in derivedTypes)
            {
                if (Array.IndexOf<Type>(includedTypes, dt)<0)
                    throw new Exception(String.Format("Class '{0}' is not defined in the Base class", dt.Name));
            }
        }

        internal string ToXml()
        {
            StringBuilder sb = new StringBuilder(1000);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(sb, xws);
            writer.WriteProcessingInstruction("xml", "version=\"1.0\"");
            writer.WriteString(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");

            // The top-level element MUST have a name that matches the class name (this acts
            // as the handle for deserializing stuff in FromXml)
            string localName = GetType().Name;

            // The initial element must be namespace qualified (thereafter, it doesn't matter)
            writer.WriteStartElement(localName, "TestSpace");

            // Call abstract method to write out the content
            WriteXml(writer);

            writer.WriteEndElement();
            writer.Close();
            return sb.ToString();
        }

        abstract internal void WriteXml(XmlWriter writer);

        /// <summary>
        /// Writes an XML element with the specified name. This method is called by implementations
        /// of <see cref="WriteXml"/> to write out embedded objects. It does not qualify the element
        /// with a namespace (so it should not be used for writing out top-level elements).
        /// </summary>
        /// <param name="writer">The writing tool</param>
        /// <param name="localName">The name for the element</param>
        internal void WriteElement(XmlWriter writer, string localName)
        {
            writer.WriteStartElement(localName);
            WriteXml(writer);
            writer.WriteEndElement();
        }

        static internal Base FromXml(SqlXml sx, XmlSerializer xs)
        {
            using (XmlReader xr = sx.CreateReader())
            {
                xr.Read();

                /*
                Type t = xr.ValueType; // it's initially String, after xr.Read it's Object (which isn't
                                        // good enough for feeding into the XmlSerializer cstr)
                */

                // Note that the name passed to GetType isn't assembly qualified, so it will only look
                // in the calling assembly and mscorlib.dll (see
                // http://blogs.msdn.com/suzcook/archive/2003/05/30/using-type-gettype-typename.aspx)

                // The name of the initial element must match the name of the class that was
                // originally written (see ToXml). The type for nested elements should be obtained
                // by the deserializer using information from the xml schema.

                string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                string typeName = String.Format("{0}.{1}", assemblyName, xr.Name);
                Type t = Type.GetType(typeName);
                if (t==null)
                    Console.WriteLine("didn't get type for "+typeName);

                //XmlSerializer xs = GetSerializer(t);
                return (Base)xs.Deserialize(xr);
            }
        }
    }
}
