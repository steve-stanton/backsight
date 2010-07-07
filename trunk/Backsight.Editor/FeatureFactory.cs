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
        /// The features created by this factory.
        /// </summary>
        readonly List<Feature> m_Features;

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
            m_Features = new List<Feature>();
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
        /// Creates a new instance of <see cref="DirectPointFeature"/>, with the currently
        /// active entity type (and a user-perceived ID if it applies), and adds to the model.
        /// </summary>
        /// <returns>The new feature</returns>
        internal virtual DirectPointFeature CreateDirectPointFeature(string itemName)
        {
            uint ss = Session.ReserveNextItem();
            DirectPointFeature result = new DirectPointFeature(m_Operation, ss, PointType, null);
            result.SetNextId();
            m_Features.Add(result);
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
            m_Features.Add(result);
            return result;
        }

        /// <summary>
        /// The features created by this factory (never null, but may be an empty array).
        /// </summary>
        internal Feature[] CreatedFeatures
        {
            get { return m_Features.ToArray(); }
        }
    }
}
