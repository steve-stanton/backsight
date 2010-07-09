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
using System.Collections.Generic;

using Backsight.Environment;


namespace Backsight.Editor
{
    /// <summary>
    /// Factory for generating objects that are derived from <see cref="Feature"/>.
    /// </summary>
    class FeatureFactory
    {
        #region Class data

        /// <summary>
        /// The editing operation that needs to create features (not null).
        /// </summary>
        readonly Operation m_Operation;

        /// <summary>
        /// Information about features that will be created, keyed by a name (that
        /// corresponds to the element name when represented in XML).
        /// </summary>
        readonly Dictionary<string, IFeature> m_FeatureInfo;

        /// <summary>
        /// The features created by this factory. This may include features in addition
        /// to those in <see cref="m_FeatureInfo"/>.
        /// </summary>
        readonly List<Feature> m_CreatedFeatures;

        /// <summary>
        /// The entity type for new point features
        /// </summary>
        IEntity m_PointType;

        /// <summary>
        /// The entity type for new line features
        /// </summary>
        IEntity m_LineType;

        /// <summary>
        /// The entity type for new polygon labels
        /// </summary>
        IEntity m_PolygonType;

        /// <summary>
        /// The entity type for new polygon labels
        /// </summary>
        IEntity m_TextType;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureFactory"/> class.
        /// </summary>
        /// <param name="op">The editing operation that needs to create features (not null).</param>
        /// <exception cref="ArgumentNullException">If the supplied editing operation is undefined</exception>
        internal FeatureFactory(Operation op)
        {
            if (op == null)
                throw new ArgumentNullException();

            m_Operation = op;
            m_FeatureInfo = new Dictionary<string, IFeature>();
            m_CreatedFeatures = new List<Feature>();
        }

        #endregion

        /// <summary>
        /// The model containing the edit that this factory is for.
        /// </summary>
        CadastralMapModel MapModel
        {
            get { return m_Operation.MapModel; }
        }

        /// <summary>
        /// The editing operation that needs to create features (not null).
        /// </summary>
        protected Operation Creator
        {
            get { return m_Operation; }
        }

        /// <summary>
        /// The entity type for new point features (if not previously
        /// defined, the default will be obtained from the map model).
        /// </summary>
        internal IEntity PointType
        {
            get
            {
                if (m_PointType == null)
                    m_PointType = MapModel.DefaultPointType;

                return m_PointType;
            }

            set { m_PointType = value; }
        }

        /// <summary>
        /// The entity type for new line features (if not previously
        /// defined, the default will be obtained from the map model).
        /// </summary>
        internal IEntity LineType
        {
            get
            {
                if (m_LineType == null)
                    m_LineType = MapModel.DefaultLineType;

                return m_LineType;
            }

            set { m_LineType = value; }
        }

        /// <summary>
        /// The entity type for new text features (if not previously
        /// defined, the default will be obtained from the map model).
        /// </summary>
        internal IEntity TextType
        {
            get
            {
                if (m_TextType == null)
                    m_TextType = MapModel.DefaultTextType;

                return m_TextType;
            }

            set { m_TextType = value; }
        }

        /// <summary>
        /// The entity type for new polygon label (if not previously
        /// defined, the default will be obtained from the map model).
        /// </summary>
        internal IEntity PolygonType
        {
            get
            {
                if (m_PolygonType == null)
                    m_PolygonType = MapModel.DefaultPolygonType;

                return m_PolygonType;
            }

            set { m_PolygonType = value; }
        }

        /// <summary>
        /// Records information for a feature that needs to be produced by this factory.
        /// </summary>
        /// <param name="itemName">A name associated with the feature (unique to the editing
        /// operation that this factory is for).</param>
        /// <param name="f">Basic information for the feature.</param>
        internal void AddFeatureDescription(string itemName, IFeature f)
        {
            if (f.Creator != m_Operation)
                throw new ArgumentException();

            m_FeatureInfo.Add(itemName, f);
        }

        /// <summary>
        /// Attempts to obtain information for a feature that was previously noted via a
        /// call to <see cref="AddFeatureDescription"/>. This is an indexed lookup.
        /// </summary>
        /// <param name="itemName">The name associated with the feature (unique to the editing
        /// operation that this factory is for).</param>
        /// <returns>The corresponding description (null if not found)</returns>
        protected IFeature FindFeatureDescription(string itemName)
        {
            IFeature result;
            if (m_FeatureInfo.TryGetValue(itemName, out result))
                return result;
            else
                return null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DirectPointFeature"/>, with the currently
        /// active entity type (and a user-perceived ID if it applies), and adds to the model.
        /// </summary>
        /// <returns>The new feature</returns>
        internal virtual DirectPointFeature CreateDirectPointFeature(string itemName)
        {
            DirectPointFeature result = null;
            IFeature f = FindFeatureDescription(itemName);

            if (f == null)
            {
                uint ss = Session.ReserveNextItem();
                result = new DirectPointFeature(m_Operation, ss, PointType, null);
                result.SetNextId();
            }
            else
            {
                result = new DirectPointFeature(f, null);
            }

            m_CreatedFeatures.Add(result);
            return result;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DirectPointFeature"/>, with the currently
        /// active entity type (and a user-perceived ID if it applies), and adds to the model.
        /// </summary>
        /// <returns>The new feature</returns>
        internal virtual DirectPointFeature CreateDirectPointFeature()
        {
            uint ss = Session.ReserveNextItem();
            DirectPointFeature result = new DirectPointFeature(m_Operation, ss, PointType, null);
            result.SetNextId();
            m_CreatedFeatures.Add(result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="SegmentLineFeature"/> using information previously
        /// recorded via a call to <see cref="AddFeatureDescription"/>.
        /// </summary>
        /// <param name="itemName">The name for the item involved</param>
        /// <param name="from">The point at the start of the line (not null).</param>
        /// <param name="to">The point at the end of the line (not null).</param>
        /// <returns>The created feature (null if a feature description was not previously added)</returns>
        internal SegmentLineFeature CreateSegmentLineFeature(string itemName, PointFeature from, PointFeature to)
        {
            IFeature f = FindFeatureDescription(itemName);
            if (f == null)
                return null;

            SegmentLineFeature result = new SegmentLineFeature(f, from, to);
            m_CreatedFeatures.Add(result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="SegmentLineFeature"/> with the default line entity type.
        /// </summary>
        /// <param name="from">The point at the start of the line (not null).</param>
        /// <param name="to">The point at the end of the line (not null).</param>
        /// <returns>The created feature (never null)</returns>
        internal SegmentLineFeature CreateSegmentLineFeature(PointFeature from, PointFeature to)
        {
            uint ss = Session.ReserveNextItem();
            SegmentLineFeature result = new SegmentLineFeature(m_Operation, ss, LineType, from, to);
            m_CreatedFeatures.Add(result);
            return result;
        }

        internal bool MakeSections(LineFeature baseLine, string itemBefore, PointFeature x, string itemAfter,
                                        out SectionLineFeature lineBefore, out SectionLineFeature lineAfter)
        {
            lineBefore = lineAfter = null;

            if (itemBefore == null || itemAfter == null)
                return false;

            // Split the line (the sections should get an undefined creation sequence). Note that
            // you cannot use the SplitLine method at this stage, because that requires defined
            // geometry.

            lineBefore = MakeSection(itemBefore, baseLine, baseLine.StartPoint, x);
            lineAfter = MakeSection(itemAfter, baseLine, x, baseLine.EndPoint);

            DeactivateLine(baseLine);
            return true;
        }

        /// <summary>
        /// Creates a new <see cref="SectionLineFeature"/> using the session sequence number
        /// that was previously recorded via a call to <see cref="AddFeatureDescription"/>.
        /// <para/>
        /// Only the session sequence number will be used when creating the section (any
        /// entity type and feature ID that may have been presented through <see cref="AddFeatureDescription"/>
        /// will be ignored - the values from the parent line will be applied instead).
        /// </summary>
        /// <param name="itemName">The name for the item involved (must refer to information
        /// previously attached via a call to <see cref="AddFeatureDescription"/>)</param>
        /// <param name="baseLine">The line that's being subdivided</param>
        /// <param name="from">The point at the start of the section (not null).</param>
        /// <param name="to">The point at the end of the section (not null).</param>
        /// <returns>The created feature (null if a feature description was not previously added)</returns>
        SectionLineFeature MakeSection(string itemName, LineFeature baseLine, PointFeature from, PointFeature to)
        {
            IFeature f = FindFeatureDescription(itemName);
            if (f == null)
                throw new InvalidOperationException();

            SectionGeometry section = new SectionGeometry(baseLine, from, to);
            SectionLineFeature result = baseLine.MakeSubSection(m_Operation, f.SessionSequence, section);
            m_CreatedFeatures.Add(result);
            return result;
        }

        /// <summary>
        /// Deactivates a line as part of regular editing work.
        /// </summary>
        /// <param name="line">The line that needs to be deactivated</param>
        /// <remarks>
        /// The <see cref=""/> class provides an override (the logic is different when
        /// a line needs to be deactivated during deserialization from the database).
        /// </remarks>
        internal virtual void DeactivateLine(LineFeature line)
        {
            line.IsInactive = true;
        }

        /// <summary>
        /// The features created by this factory (never null, but may be an empty array).
        /// </summary>
        internal Feature[] CreatedFeatures
        {
            get { return m_CreatedFeatures.ToArray(); }
        }
    }
}
