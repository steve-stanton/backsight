using System;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Data.SqlTypes;
using System.Xml.Schema;
using Backsight;

namespace TestX
{
    public abstract class Base : IXmlContent
    {
        /*
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
        */

        internal string ToXml()
        {
            XmlContentWriter.TargetNamespace = "TestSpace";
            return XmlContentWriter.GetXml("Test", this);
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

        static internal Base FromXml(XmlReader xr)
        {
            XmlContentReader xcr = new XmlContentReader(xr);
            return (Base)xcr.ReadContent();
        }

        abstract internal void ReadXml(XmlReader reader);

        abstract public void WriteContent(XmlContentWriter writer);
        abstract public void ReadContent(XmlContentReader reader);
    }
}
