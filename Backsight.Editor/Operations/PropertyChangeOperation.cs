// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

using Backsight.Editor.Xml;
using Backsight.Editor.Observations;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="06-APR-2009" />
    /// <summary>
    /// An edit that changes some sort of global editing property (e.g. change default
    /// entity type for lines).
    /// </summary>
    /// <remarks>This is experimental (and not currently used)</remarks>
    class PropertyChangeOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The property this edit relates to
        /// </summary>
        readonly PropertyItemType m_Item;

        /// <summary>
        /// The new value of the property
        /// </summary>
        string m_NewValue;

        /// <summary>
        /// The original value of the property
        /// </summary>
        string m_OldValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="propertyItem">The property this edit relates to</param>
        /// <param name="oldPropertyValue">The original value for the property</param>
        /// <param name="newPropertyValue">The new value for the property</param>
        internal PropertyChangeOperation(Session s, PropertyItemType propertyItem, string oldPropertyValue, string newPropertyValue)
            : base(s)
        {
            m_Item = propertyItem;
            m_OldValue = oldPropertyValue;
            m_NewValue = newPropertyValue;
        }

        /// <summary>
        /// Constructor for use during deserialization. The features created by this edit
        /// are defined without any geometry. A subsequent call to <see cref="CalculateGeometry"/>
        /// is needed to define the geometry.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal PropertyChangeOperation(Session s, PropertyChangeType t)
            : base(s, t)
        {
            m_Item = t.Item;
            m_OldValue = null;
            m_NewValue = t.Value;
        }

        #endregion

        /// <summary>
        /// Performs the data processing associated with this edit.
        /// </summary>
        internal override void RunEdit()
        {
            CadastralMapModel mapModel = this.MapModel;

            switch (m_Item)
            {
                case PropertyItemType.DefaultPointType:
                {
                    m_OldValue = mapModel.DefaultPointType.Id.ToString();
                    mapModel.SetDefaultEntity(SpatialType.Point, Int32.Parse(m_NewValue));
                    break;
                }

                case PropertyItemType.DefaultLineType:
                {
                    m_OldValue = mapModel.DefaultLineType.Id.ToString();
                    mapModel.SetDefaultEntity(SpatialType.Line, Int32.Parse(m_NewValue));
                    break;
                }

                case PropertyItemType.DefaultTextType:
                {
                    m_OldValue = mapModel.DefaultTextType.Id.ToString();
                    mapModel.SetDefaultEntity(SpatialType.Text, Int32.Parse(m_NewValue));
                    break;
                }

                case PropertyItemType.DefaultPolygonType:
                {
                    m_OldValue = mapModel.DefaultPolygonType.Id.ToString();
                    mapModel.SetDefaultEntity(SpatialType.Polygon, Int32.Parse(m_NewValue));
                    break;
                }

                default:
                {
                    throw new NotSupportedException("Unexpected property item: "+m_Item);
                }
            }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return String.Format("Change property ({0})", m_Item); }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>Null (always)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation (always an empty array)
        /// </summary>
        internal override Feature[] Features
        {
            get { return new Feature[0]; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.PropertyChange; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            m_NewValue = m_OldValue;
            RunEdit();

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// <para/>
        /// This is called by the <see cref="Complete"/> method, to ensure
        /// that the referenced features are cross-referenced to the editing operations
        /// that depend on them.
        /// </summary>
        public override void AddReferences()
        {
            // Nothing to do
        }

        /// <summary>
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// </summary>
        /// <returns>The serializable version of this edit</returns>
        internal override OperationType GetSerializableEdit()
        {
            PropertyChangeType t = new PropertyChangeType();
            base.SetSerializableEdit(t);

            t.Item = m_Item;
            t.Value = m_NewValue;

            return t;
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>Null (always)</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }
    }
}
