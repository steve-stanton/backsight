using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Backsight.Environment;

namespace Backsight.Editor
{
    class EditDeserializer
    {
        #region Static

        /// <summary>
        /// Loads constructor information for persistent classes.
        /// </summary>
        /// <returns>The constructors for each persistent class in this assembly, keyed by the
        /// short type name.</returns>
        static Dictionary<string, ConstructorInfo> LoadConstructors()
        {
            Dictionary<string, ConstructorInfo> result = new Dictionary<string, ConstructorInfo>();
            string iName = typeof(IPersistent).Name; // do it this way in case I rename the interface
            Assembly a = Assembly.GetExecutingAssembly();
            Type[] types = a.GetTypes();

            foreach (Type t in types)
            {
                if (t.Name.StartsWith("<") == false && t.IsAbstract == false && t.IsInterface == false && t.GetInterface(iName) != null)
                {
                    ConstructorInfo ci = t.GetConstructor(BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.Instance |
                                                          BindingFlags.DeclaredOnly, null,
                                                          new Type[] { typeof(EditDeserializer) }, null);

                    if (ci == null)
                        throw new ApplicationException("Class " + t.Name + " implements IPersistent but does not provide deserialization constructor");

                    result.Add(t.Name, ci);
                }
            }

            return result;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The mechanism for loading persistent data (not null).
        /// </summary>
        IEditReader m_Reader;

        /// <summary>
        /// Index of the constructors that accept an instance of <see cref="DeserializationFactory"/> (and which
        /// belong to classes that implement <see cref="IPersistent"/>), keyed by the short type name.
        /// This is restricted to the current assembly, and excludes abstract classes, as well as miscellaneous
        /// mystery classes that seem to be produced by NET (with type names that start with the
        /// "&lt;" character).
        /// </summary>
        readonly Dictionary<string, ConstructorInfo> m_Constructors;

        #endregion

        #region Constructors

        internal EditDeserializer()
        {
            m_Constructors = LoadConstructors();
            if (m_Constructors.Count == 0)
                throw new ApplicationException("Cannot find any deserialization constructors");
        }

        #endregion

        internal IEditReader Reader
        {
            get { return m_Reader; }
            set { m_Reader = value; }
        }

        /// <summary>
        /// Reads the content of an object that implements <see cref="IPersistent"/>.
        /// </summary>
        /// <typeparam name="T">The type of object expected by the caller.</typeparam>
        /// <param name="name">A name tag associated with the object</param>
        /// <returns>
        /// The object that was read (may actually have a type that is derived
        /// from the supplied type).
        /// </returns>
        /// <remarks>
        /// In addition to implementing <see cref="IPersistent"/>, the Backsight implementation
        /// assumes that the created type will also provide a constructor that accepts an
        /// instance of the <see cref="IEditReader"/>.
        /// </remarks>
        /// <exception cref="ApplicationException">If the type name read from storage does
        /// not correspond to a suitable type within this assembly.</exception>
        internal T ReadObject<T>(string name) where T : IPersistent
        {
            string typeName = m_Reader.ReadString(name);

            // A string of "null" is used to denote a null
            if (typeName == "null")
                return default(T);

            // Getting back an *actual* null means there was nothing after the name tag, meaning
            // that the type known to the caller is what we want to create.
            if (typeName == null)
                typeName = typeof(T).Name;

            ConstructorInfo ci;
            if (!m_Constructors.TryGetValue(typeName, out ci))
                throw new ApplicationException("Cannot locate constructor for type: " + typeName);


            m_Reader.ReadBeginObject(); // Read opening bracket

            try
            {
                return (T)ci.Invoke(new object[] { this });
            }

            finally
            {
                m_Reader.ReadEndObject(); // Read the closing bracket
            }
        }

        /// <summary>
        /// Reads an entity type for a spatial feature.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The entity type that was read.</returns>
        internal IEntity ReadEntity(string name)
        {
            int id = m_Reader.ReadInt32(name);
            return EnvironmentContainer.FindEntityById(id);
        }

        /// <summary>
        /// Reads a value in radians.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The radian value that was read.</returns>
        internal RadianValue ReadRadians(string name)
        {
            string s = m_Reader.ReadString(name);
            double d = RadianValue.Parse(s);
            return new RadianValue(d);
        }

        /// <summary>
        /// Reads a reference to a spatial feature, using that reference to obtain the
        /// corresponding feature.
        /// </summary>
        /// <typeparam name="T">The type of spatial feature expected by the caller</typeparam>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>
        /// The feature that was read (null if not found, or the reference was undefined). May
        /// actually have a type that is derived from the supplied type.
        /// </returns>
        /// <remarks>This does not create a brand new feature. Rather, it uses a reference
        /// to try to obtain a feature that should have already been created.
        /// </remarks>
        internal T ReadFeature<T>(string name) where T : Feature
        {
            string dataId = m_Reader.ReadString(name);
            if (dataId == null)
                return default(T);

            throw new NotImplementedException();
        }
    }
}
