// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using Backsight.Editor.Xml;

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
        /// The current node that's being processed
        /// </summary>
        XmlNode m_CurrentNode;

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
        }

        #endregion

        /// <summary>
        /// Reads an attribute that is expected to be an <c>Int32</c> value
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>The attribute value (0 if the attribute is not present)</returns>
        public int ReadInt(string name)
        {
            XmlAttribute a = m_CurrentNode.Attributes[name];
            return (a == null ? 0 : Int32.Parse(a.Value));
        }

        /// <summary>
        /// Reads an attribute that is expected to be a <c>UInt32</c> value
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>The attribute value (0 if the attribute is not present)</returns>
        public uint ReadUnsignedInt(string name)
        {
            XmlAttribute a = m_CurrentNode.Attributes[name];
            return (a == null ? 0 : UInt32.Parse(a.Value));
        }

        /// <summary>
        /// Reads an attribute that is expected to be an <c>Int64</c> value
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>The attribute value (0 if the attribute is not present)</returns>
        public long ReadLong(string name)
        {
            XmlAttribute a = m_CurrentNode.Attributes[name];
            return (a == null ? 0 : Int64.Parse(a.Value));
        }

        /// <summary>
        /// Reads an attribute that is expected to be a <c>String</c> value
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>The attribute value (null if the attribute is not present)</returns>
        public string ReadString(string name)
        {
            XmlAttribute a = m_CurrentNode.Attributes[name];
            return (a == null ? null : a.Value);
        }

        public T ReadElement<T>(string name) where T : Content
        {
            throw new NotSupportedException("XmlContentReader.ReadElement");

            string typeName = ReadString("xsi:type");
            if (String.IsNullOrEmpty(typeName))
                throw new Exception("Content does not contain xsi:type attribute");

            // Get an instance of the type
            //Content result = ContentMapping.GetInstance<T>(typeName);
            Content result = null;

            // Load the instance
            try
            {
                m_Elements.Push(result);
                result.ReadAttributes(this);
                result.ReadChildElements(this);
                result = result.GetContent();
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

            return (T)result;
        }

        /// <summary>
        /// Records a new feature loaded by this reader. This indexes the
        /// feature by it's internal ID.
        /// </summary>
        /// <param name="f">The feature that has been loaded (if null,
        /// nothing gets done)</param>
        void AddFeature(Feature f)
        {
            if (f != null)
            {
                try { m_Features.Add(f.InternalId, f); }
                catch { throw new Exception("Failed to index feature " + f.DataId); }
            }
        }

        public T[] ReadArray<T>(string itemName) where T : Content
        {
            List<T> result = new List<T>();
            XmlNode initialNode = m_CurrentNode;

            try
            {
                foreach (XmlNode child in initialNode.ChildNodes)
                {
                    if (child.Name == itemName)
                    {
                        m_CurrentNode = child;
                        T item = ReadElement<T>(itemName);
                        result.Add(item);
                    }
                }

                return result.ToArray();
            }

            finally
            {
                m_CurrentNode = initialNode;
            }
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
            string sid = ReadString(name);
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
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <param name="data">The data that describes the edit</param>
        /// <returns>The created editing object</returns>
        internal Operation ReadEdit(Session s, XmlReader data)
        {
            try
            {
                m_Model.IsLoading = true;
                m_Reader = data;

                XmlSerializer xs = new XmlSerializer(typeof(EditType));
                EditType et = (EditType)xs.Deserialize(data);
                Debug.Assert(et.Operation.Length==1);
                OperationType ot = et.Operation[0];
                Operation result = ot.LoadOperation(s);

                // Note that calculated geometry is NOT defined at this stage. That happens
                // when the model is asked to index the data.

                // Associate referenced features with the edit
                result.AddReferences();

                return result;
            }

            finally
            {
                m_Reader = null;
                m_Model.IsLoading = false;
            }
        }

        /// <summary>
        /// Scans back through parent nodes to attempt to locate a node with a specific
        /// type. For example, if you're processing a node for <c>LineGeometry</c>, you should
        /// be able to find the enclosing <c>LineFeature</c> instance using this technique.
        /// </summary>
        /// <typeparam name="T">The type of element to look for</typeparam>
        /// <returns>The first ancestor node with the specified type (null if not found)</returns>
        T FindParent<T>() where T : IXmlContent
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
        /// Performs initialization upon creation of a calculated point or line.
        /// </summary>
        /// <param name="f">The feature that has just been created</param>
        /// <param name="fd">Information to assign to the created point</param>
        void InitializeFeature(Feature f, FeatureData fd)
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
        /// The model that's being loaded.
        /// </summary>
        internal CadastralMapModel Model
        {
            get { return m_Model; }
        }

        /// <summary>
        /// The XML type name of the element that is currently being processed
        /// </summary>
        internal string XmlTypeName
        {
            get { return ReadString("xsi:type"); }
        }
    }
}
