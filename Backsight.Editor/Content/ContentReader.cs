using System;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

namespace Backsight.Editor.Content
{
    /// <summary>
    /// Reads back data that was previously created using the
    /// <see cref="ContentWriter"/> class.
    /// </summary>
    public class ContentReader
    {
        #region Class data

        /// <summary>
        /// The object that actually does the reading.
        /// </summary>
        XmlReader m_Reader;

        /// <summary>
        /// Cross-reference of type names to the corresponding constructor.
        /// </summary>
        readonly Dictionary<string, ConstructorInfo> m_Types;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ContentReader</c> that wraps the supplied
        /// reading tool.
        /// </summary>
        /// <param name="numItem">The estimated number of items that will be
        /// loaded (used to initialize an index of the loaded data)</param>
        public ContentReader(uint numItem)
        {
            //m_Model = model;
            m_Reader = null;
            m_Types = new Dictionary<string, ConstructorInfo>();
             /*
            m_Features = new Dictionary<InternalIdValue, Feature>((int)numItem);
            m_Elements = new Stack<IXmlContent>(10);
            m_NativeIds = new Dictionary<uint, NativeId>(1000);
            m_ForeignIds = new Dictionary<string, ForeignId>(1000);
             */
        }

        #endregion

        /// <summary>
        /// Loads the content of an editing operation. Prior to call, the current editing session
        /// must be defined using the <see cref="Session.CurrentSession"/> property.
        /// </summary>
        /// <param name="editSequence">The item sequence for the edit</param>
        /// <param name="data">The data that describes the edit</param>
        /// <returns>The created editing object</returns>
        internal Edit ReadEdit(XmlReader data)
        {
            try
            {
                m_Reader = data;
                Edit result = ReadElement<Edit>("Edit");
                return result;
            }

            finally
            {
                m_Reader = null;
            }
        }

        bool ReadToElement(string name)
        {
            return m_Reader.ReadToFollowing(name);
        }

        /// <summary>
        /// Attempts to locate the <c>Type</c> corresponding to the supplied type name
        /// </summary>
        /// <param name="typeName">The name of the type (unqualified with any assembly stuff)</param>
        /// <returns>The correspond class type (null if not found in application domain)</returns>
        /// <remarks>This stuff is in pretty murky territory for me. I would guess it's possible
        /// that the same type name could appear in more than one assembly, so there could be
        /// some ambiguity.</remarks>
        Type FindType(string typeName)
        {
            // The type is most likely part of the assembly that holds this class.
            // Note: I thought it might be sufficient to just call Type.GetType("Backsight.Editor."+typeName),
            // but that doesn't find Operation classes (it only works if you specify the sub-folder name too).
            Type result = FindType(GetType().Assembly, typeName);
            if (result != null)
                return result;

            // If things get moved about though, it's a bit more complicated...
            Assembly[] aa = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in aa)
            {
                result = FindType(a, typeName);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Searches the supplied assembly for a <c>Type</c> corresponding to the
        /// supplied type name
        /// </summary>
        /// <param name="a">The assembly to search</param>
        /// <param name="typeName">The name of the type (unqualified with any assembly stuff)</param>
        /// <returns>The corresponding type (null if not found)</returns>
        Type FindType(Assembly a, string typeName)
        {
            foreach (Type type in a.GetTypes())
            {
                if (type.Name == typeName)
                    return type;
            }

            return null;
        }

        public T ReadElement<T>(string name) where T : IContentElement
        {
            // Ensure we're at an element
            if (!ReadToElement(name))
                return default(T);

            // If there's no type declaration, assume we're dealing with a null object
            string typeName = m_Reader["xsi:type"];
            if (String.IsNullOrEmpty(typeName))
                return default(T);

            // If we haven't previously encountered the type, look up a default constructor
            if (!m_Types.ContainsKey(typeName))
            {
                Type t = FindType(typeName);
                if (t == null)
                    throw new Exception("Cannot create object with type: " + typeName);

                // Confirm that the type implements IContentElement
                if (t.FindInterfaces(Module.FilterTypeName, "IContentElement").Length == 0)
                    throw new Exception("IContentElement not implemented by type: " + t.FullName);

                // Locate default constructor
                ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                    throw new Exception("Cannot locate default constructor for type: " + t.FullName);

                m_Types.Add(typeName, ci);
            }

            // Create the instance
            ConstructorInfo c = m_Types[typeName];
            T result = (T)c.Invoke(new object[0]);

            // Load the instance
            result.ReadAttributes(this);
            result.ReadChildElements(this);

            return result;
        }

        public int ReadInt(string name)
        {
            string s = m_Reader[name];
            return (s==null ? 0 : Int32.Parse(s));
        }

        public uint ReadUnsignedInt(string name)
        {
            string s = m_Reader[name];
            return (s == null ? 0 : UInt32.Parse(s));
        }

        public long ReadLong(string name)
        {
            string s = m_Reader[name];
            return (s == null ? 0 : Int64.Parse(s));
        }

        /// <summary>
        /// Reads a boolean attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>The value of the attribute (false if the attribute isn't there)</returns>
        public bool ReadBool(string name)
        {
            string s = m_Reader[name];
            return (s == null ? false : Boolean.Parse(s));
        }

        public double ReadDouble(string name)
        {
            string s = m_Reader[name];
            return (s == null ? 0 : Double.Parse(s));
        }

        public string ReadString(string name)
        {
            return m_Reader[name];
        }

        string[] ReadStringArray(string itemName)
        {
            // Ensure we're at an element
            string arrayName = itemName+"Array";
            if (!ReadToElement(arrayName))
                throw new Exception("Array element not found");

            // Pick up array size
            uint length = ReadUnsignedInt("Length");
            string[] result = new string[(int)length];

            // Move to the first item (not sure why this is necessary, but otherwise
            // I get an error with 1-element arrays)
            m_Reader.Read();

            for (int i=0; i<length; i++)
                result[i] = m_Reader.ReadElementString(itemName);

            return result;
        }

        public Id[] ReadIdArray(string itemName)
        {
            string[] items = ReadStringArray(itemName);
            Id[] result = new Id[items.Length];

            for (int i=0; i<result.Length; i++)
                result[i] = new Id(items[i]);

            return result;
        }
    }
}
