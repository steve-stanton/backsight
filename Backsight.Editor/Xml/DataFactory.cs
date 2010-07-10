// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Factory for generating XML
    /// </summary>
    class DataFactory
    {
        #region Static

        /// <summary>
        /// The singleton instance of the data factory.
        /// </summary>
        static DataFactory s_Instance = null;

        /// <summary>
        /// The singleton instance of the data factory (created on first access).
        /// </summary>
        internal static DataFactory Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new DataFactory();

                return s_Instance;
            }
        }

        /// <summary>
        /// Generates an index of types in the assembly containing this class.
        /// </summary>
        /// <returns>The types in this assembly, indexed by short name. Excludes
        /// abstract classes and interfaces.</returns>
        /// <exception cref="InvalidOperationException">If more than one type has
        /// the same short name.</exception>
        static Dictionary<string, Type> GetTypeIndex()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Type[] types = a.GetTypes();

            Dictionary<string, Type> result = new Dictionary<string, Type>(types.Length);
            foreach (Type t in types)
            {
                if (t.Name.StartsWith("<") == false && t.IsAbstract == false && t.IsInterface == false)
                {
                    if (result.ContainsKey(t.Name))
                        throw new InvalidOperationException("Duplicate class name: " + t.Name);

                    result.Add(t.Name, t);
                }
            }

            return result;
        }

        #endregion

        #region Class data

        /// <summary>
        /// Index of the classes in this assembly, keyed by short name. Excludes
        /// abstract classes, interfaces, as well as miscellaneous mystery classes
        /// that seem to be produced by NET (with type names that start with the
        /// "&lt;" character).
        /// </summary>
        readonly Dictionary<string, Type> m_TypeIndex;

        /// <summary>
        /// Index that maps source classes to the corresponding constructor
        /// in the xml data class.
        /// </summary>
        readonly Dictionary<Type, ConstructorInfo> m_DataMapping;

        #endregion

        #region Constructors

        internal DataFactory()
        {
            m_TypeIndex = GetTypeIndex();
            m_DataMapping = new Dictionary<Type, ConstructorInfo>();

            // Observations
            AddMapping("AngleDirection", "AngleData");
            AddMapping("BearingDirection", "BearingData");
            AddMapping("DeflectionDirection", "DeflectionData");
            AddMapping("Distance", "DistanceData");
            AddMapping("OffsetDistance", "OffsetDistanceData");
            AddMapping("OffsetPoint", "OffsetPointData");
            AddMapping("ParallelDirection", "ParallelData");

            // Features
            AddMapping("ArcFeature", "ArcData");
            AddMapping("KeyTextFeature", "KeyTextData");
            AddMapping("SegmentLineFeature", "SegmentData");
            AddMapping("MiscTextFeature", "MiscTextData");
            AddMapping("MultiSegmentLineFeature", "MultiSegmentData");
            AddMapping("PointFeature", "PointData");
            AddMapping("RowTextFeature", "RowTextData");
            AddMapping("SectionLineFeature", "SectionData");
            //AddMapping("FeatureStub", "FeatureStubData");
        }

        #endregion

        void AddMapping(string fromTypeName, string toTypeName)
        {
            Type fromType, toType;

            if (!m_TypeIndex.TryGetValue(fromTypeName, out fromType))
                throw new ArgumentException();

            if (!m_TypeIndex.TryGetValue(toTypeName, out toType))
                throw new ArgumentException();

            ConstructorInfo ci = toType.GetConstructor( BindingFlags.Public |
                                                        BindingFlags.NonPublic |
                                                        BindingFlags.Instance |
                                                        BindingFlags.DeclaredOnly, null,
                                                        new Type[] { fromType }, null);
            if (ci == null)
            {
                string msg = String.Format("Cannot find constructor {0}({1})",
                                                toTypeName, fromTypeName);
                throw new NotImplementedException(msg);
            }

            m_DataMapping.Add(fromType, ci);
        }

        /// <summary>
        /// Represents an editing operation in XML (suitable for inserting
        /// into the database)
        /// </summary>
        /// <returns>The XML for this edit</returns>
        internal string ToXml<T>(T op) where T : Operation
        {
            return ToXml(op, false);
        }

        /// <summary>
        /// Represents an editing operation in XML, with optional indentation of elements.
        /// </summary>
        /// <param name="indent">Should the XML be indented or not?</param>
        /// <returns>The XML for this edit</returns>
        internal string ToXml<T>(T op, bool indent) where T : Operation
        {
            string typeName = op.GetType().Name;
            const string TAIL = "Operation";
            if (!typeName.EndsWith(TAIL))
                throw new NotSupportedException();

            typeName = "Backsight.Editor.Xml." + typeName.Substring(0, typeName.Length - TAIL.Length) + "Data";

            Assembly a = Assembly.GetExecutingAssembly();
            Type t = a.GetType(typeName, true);

            // Get the constructor of the form *Data(*Operation)
            ConstructorInfo ci = t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null,
                new Type[] { op.GetType() }, null);

            if (ci == null)
                throw new NotImplementedException();

            object o = ci.Invoke(new object[] { op });
            OperationData sed = (o as OperationData);
            if (sed == null)
                throw new InvalidOperationException("Data class does not extend OperationData");

            StringBuilder sb = new StringBuilder(1000);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = indent;

            using (XmlWriter writer = XmlWriter.Create(sb, xws))
            {
                // Wrap the serializable edit in an EditData object (means we always know what to
                // cast the result to upon deserialization)

                EditData e = new EditData();
                e.Operation = new OperationData[] { sed };
                XmlSerializer xs = new XmlSerializer(typeof(EditData));
                xs.Serialize(writer, e);
            }

            return sb.ToString();
        }

        internal T ToData<T>(Observation o) where T : ObservationData
        {
            return (T)CreateData(o);
            //ConstructorInfo ci;
            //if (!m_DataMapping.TryGetValue(o.GetType(), out ci))
            //    throw new NotImplementedException();

            //return (T)ci.Invoke(new object[] { o });
        }

        internal T ToData<T>(Feature f) where T : FeatureData
        {
            return (T)CreateData(f);
        }

        object CreateData(object o)
        {
            ConstructorInfo ci;
            if (!m_DataMapping.TryGetValue(o.GetType(), out ci))
                throw new NotImplementedException();

            return ci.Invoke(new object[] { o });
        }

        /// <summary>
        /// Loads the content of an editing operation. Prior to call, the current editing session
        /// must be defined using the <see cref="Session.CurrentSession"/> property.
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <param name="data">The data that describes the edit</param>
        /// <returns>The created editing object</returns>
        /// <remarks>Edits are converted into xml using the <see cref="Operation.ToXml"/> method.</remarks>
        internal Operation ToOperation(Session s, XmlReader data)
        {
            XmlSerializer xs = new XmlSerializer(typeof(EditData));
            EditData et = (EditData)xs.Deserialize(data);
            Debug.Assert(et.Operation.Length == 1);
            OperationData ot = et.Operation[0];
            Operation result = ot.LoadOperation(s);

            // Note that calculated geometry is NOT defined at this stage. That happens
            // when the model is asked to index the data.

            // Associate referenced features with the edit
            result.AddReferences();

            return result;
        }
    }
}
