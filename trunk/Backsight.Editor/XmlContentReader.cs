/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <summary>
    /// Reads back XML data that was previously created using the
    /// <see cref="XmlContentWriter"/> class.
    /// </summary>
    public class XmlContentReader
    {
        #region Class data

        /// <summary>
        /// The model that's being loaded.
        /// </summary>
        readonly CadastralMapModel m_Model;

        /// <summary>
        /// The object that actually does the reading.
        /// </summary>
        XmlReader m_Reader;

        /// <summary>
        /// Cross-reference of type names to the corresponding constructor.
        /// </summary>
        readonly Dictionary<string, ConstructorInfo> m_Types;

        /// <summary>
        /// Spatial features that have been loaded.
        /// </summary>
        readonly Dictionary<InternalIdValue, Feature> m_Features;

        /// <summary>
        /// The elements that are currently being read
        /// </summary>
        readonly Stack<IXmlContent> m_Elements;

        /// <summary>
        /// Index of feature IDs that are based on the Backsight numbering strategy (as opposed
        /// to user-perceived IDs that originate from some foreign source). The key
        /// is the raw ID, the value the created ID object.
        /// </summary>
        readonly Dictionary<uint, NativeId> m_NativeIds;

        /// <summary>
        /// Index of all foreign IDs (typically IDs that get defined via import from some
        /// alien data source).
        /// </summary>
        readonly Dictionary<string, ForeignId> m_ForeignIds;

        /// <summary>
        /// Information about the features created by the editing operation that
        /// is currently being deserialized from the database. The key is the
        /// sequence number of the created feature, the value holds the assigned
        /// entity type (and perhaps the assigned ID too).
        /// </summary>
        /// <remarks>This is a bit of a hack, introduced in an attempt to overcome
        /// problems with the deserialization of connection paths.</remarks>
        //Dictionary<uint, FeatureData> m_FeatureInfo;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>XmlContentReader</c> that wraps the supplied
        /// reading tool.
        /// </summary>
        /// <param name="model">The model to load</param>
        /// <param name="numItem">The estimated number of items that will be
        /// loaded (used to initialize an index of the loaded data)</param>
        internal XmlContentReader(CadastralMapModel model, uint numItem)
        {
            m_Model = model;
            m_Reader = null;
            m_Types = new Dictionary<string, ConstructorInfo>();
            m_Features = new Dictionary<InternalIdValue, Feature>((int)numItem);
            m_Elements = new Stack<IXmlContent>(10);
            m_NativeIds = new Dictionary<uint, NativeId>(1000);
            m_ForeignIds = new Dictionary<string, ForeignId>(1000);
        }

        #endregion

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

        bool ReadToElement(string name)
        {
            return m_Reader.ReadToFollowing(name);
            /*
            while (m_Reader.NodeType != XmlNodeType.Element && m_Reader.Name != name)
            {
                if (!m_Reader.Read())
                    return false;
            }

            m_Reader.MoveToElement();
            return true;
             */
        }

        public T ReadElement<T>(string name) where T : IXmlContent
        {
            // Ensure we're at an element
            if (!ReadToElement(name))
                return default(T);

            // For some reason, I'd read an ArcFeature with an embedded ArcGeometry, but the
            // xsi:type attribute continues to say it's ArcFeature. So try to nudge it ahead.
            //if (!m_Reader.MoveToFirstAttribute())
            //    return default(T);

            /*
            // After doing the above, we won't be at an element! Whoever wrote this XML
            // reading code sure knew how to make it opaque!
            if (!ReadToElement(name))
                return default(T);

            m_Reader.MoveToFirstAttribute();
            */

            // If there's no type declaration, assume we're dealing with a null object
            string typeName = m_Reader["xsi:type"];
            if (String.IsNullOrEmpty(typeName))
                return default(T);
                //throw new Exception("Content does not contain xsi:type attribute");

            // If we haven't previously encountered the type, look up a
            // default constructor
            if (!m_Types.ContainsKey(typeName))
            {
                Type t = FindType(typeName);
                if (t == null)
                    throw new Exception("Cannot create object with type: " + typeName);

                // Confirm that the type implements IXmlContent
                if (t.FindInterfaces(Module.FilterTypeName, "IXmlContent").Length == 0)
                    throw new Exception("IXmlContent not implemented by type: " + t.FullName);

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
            try
            {
                m_Elements.Push(result);
                result.ReadAttributes(this);
                result.ReadChildElements(this);
            }

            finally
            {
                m_Elements.Pop();
            }

            // If we've just read a spatial feature, remember it
            AddFeature(result as Feature);

            // Read the next node (if any), in case this element has an EndElement node. If it
            // doesn't we should be all ready to start reading the next element.
            //m_Reader.Read();

            return result;
        }

        /// <summary>
        /// Records a new feature loaded by this reader. This indexes the
        /// feature by it's internal ID.
        /// </summary>
        /// <param name="f">The feature that has been loaded (if null,
        /// nothing gets done)</param>
        internal void AddFeature(Feature f)
        {
            if (f != null)
            {
                try { m_Features.Add(f.InternalId, f); }
                catch { throw new Exception("Failed to index feature " + f.DataId); }
            }
        }

        public T[] ReadArray<T>(string arrayName, string itemName) where T : class, IXmlContent
        {
            // Ensure we're at an element
            if (!ReadToElement(arrayName))
                throw new Exception("Array element not found");

            // Pick up array size
            uint capacity = ReadUnsignedInt("Length");
            T[] result = new T[(int)capacity];

            for (int i=0; i<capacity; i++)
            {
                T item = ReadElement<T>(itemName);
                Debug.Assert(item!=null);
                result[i] = item;
            }

            return result;
        }

        string[] ReadStringArray(string arrayName, string itemName)
        {
            // Ensure we're at an element
            if (!ReadToElement(arrayName))
                throw new Exception("Array element not found");

            // Pick up array size
            uint capacity = ReadUnsignedInt("Length");
            string[] result = new string[(int)capacity];

            // Move to the first item (not sure why this is necessary, but otherwise
            // I get an error with 1-element arrays)
            m_Reader.Read();

            for (int i=0; i<capacity; i++)
                result[i] = m_Reader.ReadElementString(itemName);

            return result;
        }

        internal T[] ReadFeatureReferenceArray<T>(string arrayName, string itemName) where T : Feature
        {
            string[] ids = ReadStringArray(arrayName, itemName);
            T[] result = new T[ids.Length];

            for (int i=0; i<ids.Length; i++)
                result[i] = (T)ReadFeatureById(ids[i]);

            return result;
        }

        /// <summary>
        /// Obtains the spatial feature associated with a reference that was
        /// originally output using <see cref="XmlContentWriter.WriteFeatureReference"/>
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>The corresponding feature</returns>
        internal T ReadFeatureByReference<T>(string name) where T : Feature
        {
            // Get the internal ID of the feature
            string sid = m_Reader[name];
            if (sid==null)
                return null;

            return (T)ReadFeatureById(sid);
        }

        /// <summary>
        /// Obtains a previously loaded feature with a specific internal ID
        /// </summary>
        /// <param name="sid">A string representing the internal ID of the feature to get</param>
        /// <returns>The corresponding feature</returns>
        internal T GetFeatureByReference<T>(string sid) where T : Feature
        {
            return (T)ReadFeatureById(sid);
        }

        /// <summary>
        /// Obtains the spatial feature associated with the supplied internal ID.
        /// </summary>
        /// <param name="sid">The internal ID of the feature</param>
        /// <returns>The corresponding feature</returns>
        Feature ReadFeatureById(string sid)
        {
            InternalIdValue iid = new InternalIdValue(sid);

            // The corresponding feature SHOULD have been encountered already
            Feature result;
            if (m_Features.TryGetValue(iid, out result))
                return result;

            throw new Exception("Cannot find: " + iid);
        }

        /// <summary>
        /// Loads the content of an editing operation. Prior to call, the current editing session
        /// must be defined using the <see cref="Session.CurrentSession"/> property.
        /// </summary>
        /// <param name="data">The data that describes the edit</param>
        /// <returns>The created editing object</returns>
        internal Operation ReadEdit(XmlReader data)
        {
            // Experiment...

            try
            {
                // The first child is the "Data" element. Within that is the "Edit" element.
                XmlDocument doc = new XmlDocument();
                doc.Load(data);
                Debug.Assert(doc.ChildNodes.Count==1);
                XmlNode top = doc.ChildNodes[0];
                Debug.Assert(doc.Name == "Data");
                Debug.Assert(top.ChildNodes.Count==1);
                XmlNode ed = top.ChildNodes[0];                
                Debug.Assert(ed.Name == "Edit");
                XmlAttribute a = ed.Attributes["xsi:type"];
                string edType = a.Value;
                return null;
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            try
            {
                m_Model.IsLoading = true;
                m_Reader = data;
                Debug.Assert(m_Elements.Count==0);
                Operation result = ReadElement<Operation>("Edit");
                result.AddReferences();

                // Add created features to the model
                //Feature[] feats = result.Features;
                //m_Model.AddToIndex(feats);

                return result;
            }

            finally
            {
                m_Reader = null;
                m_Model.IsLoading = false;
            }
        }

        /// <summary>
        /// Checks whether an attribute with the specified name is part of
        /// the current element.
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>True if the attribute is defined</returns>
        internal bool HasAttribute(string name)
        {
            return (m_Reader[name] != null);
        }

        /// <summary>
        /// Scans back through parent nodes to attempt to locate a node with a specific
        /// type. For example, if you're processing a node for <c>LineGeometry</c>, you should
        /// be able to find the enclosing <c>LineFeature</c> instance using this technique.
        /// </summary>
        /// <typeparam name="T">The type of element to look for</typeparam>
        /// <returns>The first ancestor node with the specified type (null if not found)</returns>
        internal T FindParent<T>() where T : IXmlContent
        {
            // This works back from the last element that was pushed onto the stack
            foreach (IXmlContent e in m_Elements)
            {
                if (e is T)
                    return (T)e;
            }

            return default(T);
        }

        /// <summary>
        /// Obtains a native ID for a feature
        /// </summary>
        /// <param name="group">The ID group the ID is part of</param>
        /// <param name="rawId">The raw ID to look for</param>
        /// <returns>The ID assigned to the feature</returns>
        internal NativeId FindNativeId(IdGroup group, uint rawId)
        {
            NativeId result;

            if (!m_NativeIds.TryGetValue(rawId, out result))
            {
                result = new NativeId(group, rawId);
                m_NativeIds.Add(rawId, result);
            }

            return result;
        }

        /// <summary>
        /// Obtains a foreign ID for a feature
        /// </summary>
        /// <param name="name">The name of the attribute holding the foreign ID string</param>
        /// <param name="f">The feature that will receive the ID</param>
        /// <returns>The ID assigned to the feature</returns>
        internal ForeignId FindForeignId(string key)
        {
            ForeignId result;

            if (!m_ForeignIds.TryGetValue(key, out result))
            {
                result = new ForeignId(key);
                m_ForeignIds.Add(key, result);
            }

            return result;
        }

        /// <summary>
        /// Reads back a point feature (without any geometry). A subsequent call
        /// to <see cref="Operation.CalculateGeometry"/> needs to be made to
        /// define the geometry for the point.
        /// </summary>
        /// <param name="elementName">The name of the element containing the fields
        /// describing the point</param>
        /// <returns>The created point (without any geometry)</returns>
        internal PointFeature ReadPoint(string elementName)
        {
            // Pick up the information for the point
            FeatureData fd = ReadElement<FeatureData>(elementName);
            if (fd == null)
                return null;
            else
                return CreateCalculatedPoint(fd, null);
        }

        /// <summary>
        /// Creates a calculated point feature
        /// </summary>
        /// <param name="fd">Information to assign to the created point</param>
        /// <param name="p">The calculated position of the point (null if it will be calculated later)</param>
        /// <returns>The created point</returns>
        internal PointFeature CreateCalculatedPoint(FeatureData fd, IPosition p)
        {
            PointGeometry pg = (p==null ? null : PointGeometry.Create(p));
            PointFeature result = new PointFeature(pg, fd.EntityType, FindParent<Operation>());
            InitializeFeature(result, fd);
            return result;
        }

        /// <summary>
        /// Creates a calculated line feature (with the geometry for a simple line segment)
        /// </summary>
        /// <param name="fd">Information to assign to the created line</param>
        /// <param name="from">The point feature at the start of the line</param>
        /// <param name="to">The point feature at the end of the line</param>
        /// <returns>The created line</returns>
        internal LineFeature CreateCalculatedLine(FeatureData fd, PointFeature from, PointFeature to)
        {
            // The LineFeature constructor will also modify the end points by
            // referencing them to the created line.
            LineFeature result = new LineFeature(fd.EntityType, FindParent<Operation>(), from, to);
            InitializeFeature(result, fd);
            return result;
        }

        /// <summary>
        /// Creates a calculated line feature (with the geometry for a circular arc)
        /// </summary>
        /// <param name="fd">Information to assign to the created line</param>
        /// <param name="from">The point feature at the start of the arc</param>
        /// <param name="to">The point feature at the end of the arc</param>
        /// <param name="circle">The circle on which the arc lies. This will be modified to refer
        /// to the newly created arc.</param>
        /// <param name="clockwise">True if the arc is clockwise.</param>
        /// <returns>The created arc</returns>
        internal ArcFeature CreateCalculatedArc(FeatureData fd, PointFeature from, PointFeature to, Circle circle, bool clockwise)
        {
            // The LineFeature constructor will also modify the end points by
            // referencing them to the created line.
            ArcFeature result = new ArcFeature(fd.EntityType, FindParent<Operation>(), circle, from, to, clockwise);
            InitializeFeature(result, fd);
            return result;
        }

        /// <summary>
        /// Performs initialization upon creation of a calculated point or line.
        /// </summary>
        /// <param name="f">The feature that has just been created</param>
        /// <param name="fd">Information to assign to the created point</param>
        internal void InitializeFeature(Feature f, FeatureData fd)
        {
            // Ensure the created feature has the expected session item number
            f.CreatorSequence = fd.CreationSequence;

            // If the feature has a user-perceived ID, ensure the ID points to the
            // feature and vice versa
            FeatureId fid = fd.Id;
            if (fid!=null)
                fid.Add(f);

            // Remember the new feature as part of this reader, since subsequent
            // edits may make reference to it
            AddFeature(f);
        }

        /// <summary>
        /// Reads back a calculated line feature. This is suitable only for simple line
        /// segments and circular arcs (not multi-segments).
        /// </summary>
        /// <param name="elementName">The name of the element containing the fields
        /// desribing the line (and its terminal points)</param>
        /// <param name="from">The calculated position of the start of the line</param>
        /// <param name="to">The calculated position of the end of the line</param>
        /// <param name="isArc">Is the line a circular arc?</param>
        /// <returns>The created line</returns>
        /*
        internal LineFeature ReadCalculatedLine(string elementName, IPosition from, IPosition to, bool isArc)
        {
            // Pick up the information for the point
            LineData lineData = ReadElement<LineData>(elementName);
            if (lineData == null)
                return null;

            LineFeature result = null;
            return result;
        }
         */

        /// <summary>
        /// Reads back an attribute representing an angle that
        /// has been formatted using <see cref="XmlContentWriter.WriteAngle"/>
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <returns>The angle in radians (0.0 if the attribute isn't there)</returns>
        public double ReadAngle(string name)
        {
            string s = m_Reader[name];
            if (s==null)
                return 0.0;

            double result;
            if (RadianValue.TryParse(s, out result))
                return result;

            throw new Exception("Cannot parse angle: "+s);
        }

        /// <summary>
        /// The model that's being loaded.
        /// </summary>
        internal CadastralMapModel Model
        {
            get { return m_Model; }
        }

        /// <summary>
        /// Saves (or clears) information about the features created by the edit
        /// that is currently being deserialized
        /// </summary>
        /// <param name="info">Information about the features created by
        /// the edit (specify null to clear)</param>
        //internal void SaveFeatureData(FeatureData[] info)
        //{
        //    if (info == null)
        //        m_FeatureInfo = null;
        //    else
        //    {
        //        m_FeatureInfo = new Dictionary<uint, FeatureData>(info.Length);
        //        foreach (FeatureData fd in info)
        //            m_FeatureInfo.Add(fd.CreationSequence, fd);
        //    }
        //}

        /// <summary>
        /// Attempts to obtain information about a specific feature created by
        /// the edit that is being deserialized.
        /// </summary>
        /// <param name="creationSequence">The creation sequence number of the feature</param>
        /// <returns>The corresponding information (null if the current edit does
        /// not involve a special feature information cache, or the information cannot
        /// be found)</returns>
        //internal FeatureData GetFeatureData(uint creationSequence)
        //{
        //    if (m_FeatureInfo==null)
        //        return null;

        //    FeatureData result;
        //    if (m_FeatureInfo.TryGetValue(creationSequence, out result))
        //        return result;
        //    else
        //        return null;
        //}

        /// <summary>
        /// Obtains the feature IDs that have been encountered during loading.
        /// </summary>
        /// <returns>The native and foreign IDs that have been
        /// loaded (may be an empty array)</returns>
        internal FeatureId[] GetFeatureIds()
        {
            int numNative = m_NativeIds.Count;
            int numForeign = m_ForeignIds.Count;
            List<FeatureId> result = new List<FeatureId>(numNative + numForeign);

            foreach (NativeId nid in m_NativeIds.Values)
                result.Add(nid);

            foreach (ForeignId fid in m_ForeignIds.Values)
                result.Add(fid);

            return result.ToArray();
        }
    }
}
