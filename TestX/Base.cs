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

        static internal Base FromXml(XmlReader xr)
        {
            XmlContentReader xcr = new XmlContentReader(xr);
            return (Base)xcr.ReadContent();
        }

        abstract public void WriteContent(XmlContentWriter writer);
        abstract public void ReadContent(XmlContentReader reader);
    }
}
