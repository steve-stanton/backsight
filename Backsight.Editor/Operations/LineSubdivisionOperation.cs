// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.Diagnostics;

using Backsight.Editor.Observations;
using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="23-OCT-1997" />
    /// <summary>
    /// Operation to subdivide a line.
    /// </summary>
    class LineSubdivisionOperation : Operation, IRecallable, IRevisable
    {
        #region Class data

        /// <summary>
        /// The line that was subdivided.
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// Definition of the line sections for the subdivided line.
        /// </summary>
        readonly LineSubdivisionFace m_Face;

        /// <summary>
        /// Any subdivision for the other side of the line (null if the subdivision applies
        /// to both sides of the line).
        /// </summary>
        internal LineSubdivisionOperation OtherSide { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionOperation"/> class.
        /// </summary>
        /// <param name="line">The line that is being subdivided.</param>
        /// <param name="distances">The lengths for each subdivision section.</param>
        /// <param name="isEntryFromEnd">Are the distances observed from the end of the line?</param>
        internal LineSubdivisionOperation(LineFeature line, Distance[] distances, bool isEntryFromEnd)
            : base()
        {
            m_Line = line;
            m_Face = new LineSubdivisionFace(distances, isEntryFromEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal LineSubdivisionOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            m_Line = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Line);
            m_Face = editDeserializer.ReadPersistent<LineSubdivisionFace>(DataField.Face);
            //FeatureStub[] sections = editDeserializer.ReadFeatureStubArray(DataField.Result);

            if (editDeserializer.IsNextField(DataField.OtherSide))
            {
                InternalIdValue id = editDeserializer.ReadInternalId(DataField.OtherSide);
                OtherSide = (LineSubdivisionOperation)editDeserializer.MapModel.FindOperation(id);
                Debug.Assert(OtherSide != null);
                OtherSide.OtherSide = this;
            }

            Project p = editDeserializer.Project;
            IEntity pointType = editDeserializer.ReadEntity(DataField.PointType);
            FeatureStub[] sections = CreateStubs(p, pointType, m_Line.EntityType);

            DeserializationFactory result = new DeserializationFactory(this, sections);
            ProcessFeatures(result);

            // Apply any IDs
            if (editDeserializer.IsNextField(DataField.Ids))
                editDeserializer.ReadIdMappings(DataField.Ids);
        }

        #endregion

        /// <summary>
        /// Creates stubs for all items that will be created by executing this edit.
        /// </summary>
        /// <param name="p">The project the stubs relate to.</param>
        /// <param name="pointType">The entity type for created points</param>
        /// <param name="lineType">The entity type for created lines</param>
        /// <returns></returns>
        FeatureStub[] CreateStubs(Project p, IEntity pointType, IEntity lineType)
        {
            List<FeatureStub> stubList = new List<FeatureStub>();
            uint sequence = this.InternalId.ItemSequence;

            // Reserve two items for each span (except for the last)
            int nSpan = m_Face.ObservedLengths.Length;
            for (int i=0; i<nSpan; i++)
            {
                if (i < (nSpan-1))
                    stubList.Add(CreateStub(++sequence, pointType));

                stubList.Add(CreateStub(++sequence, lineType));
            }

            // Remember the last internal ID that has been allocated
            p.SetLastItem(sequence);

            return stubList.ToArray();
        }

        /// <summary>
        /// Creates a stub for an item in this path.
        /// </summary>
        /// <param name="itemSequence">The sequence number to assign to the stub</param>
        /// <param name="ent">The entity type for the stub</param>
        /// <returns>The created stub</returns>
        FeatureStub CreateStub(uint itemSequence, IEntity ent)
        {
            InternalIdValue iid = new InternalIdValue(itemSequence);
            return new FeatureStub(this, iid, ent, null);
        }

        /// <summary>
        /// The line that was subdivided.
        /// </summary>
        internal LineFeature Parent
        {
            get { return m_Line; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get
            {
                if (OtherSide == null)
                    return "Line subdivision";

                if (IsPrimaryFace)
                    return "Line subdivision (first face)";

                return "Line subdivision (second face)";
            }
        }

        /// <summary>
        /// Execute line subdivision.
        /// </summary>
        internal void Execute()
        {
            // Create stubs for all items that will be created.
            Project p = EditingController.Current.Project;
            FeatureStub[] stubs = CreateStubs(p, p.DefaultPointType, m_Line.EntityType);

            // And create features, geometry, and IDs
            DeserializationFactory ff = new DeserializationFactory(this, stubs);
            base.Execute(ff);
        }

        /// <summary>
        /// Creates user-perceived IDs for features that should have one.
        /// </summary>
        /// <param name="features">The features to assign IDs to</param>
        /// <remarks>Called via <see cref="Execute"/></remarks>
        internal override void CreateIds(Feature[] features)
        {
            foreach (Feature f in features)
            {
                if (f is PointFeature)
                    f.SetNextId();
            }
        }

        /// <summary>
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            // Create the sections along the face
            m_Face.CreateSections(m_Line, ff, (OtherSide == null));

            // Retire the original line if not already retired (it will already be inactive
            // if we're dealing with an alternate face).
            if (!m_Line.IsInactive)
                ff.Deactivate(m_Line);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            m_Face.CalculateGeometry(m_Line, ctx);
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>();

                if (m_Face != null)
                    result.AddRange(m_Face.GetNewFeatures(m_Line));

                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        /// <value>EditingActionId.LineSubdivision</value>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.LineSubdivision; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            // Assumes the Distance class isn't a base for a derived class
            // that might reference other stuff.

            return new Feature[] { m_Line };
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
        {
            base.OnRollback();

            if (IsPrimaryFace)
                m_Line.CutOp(this);

            // Go through each section we created, marking each one as
            // deleted. Also mark the point features at the start of each
            // section, so long as it was created by this operation (should
            // do nothing for the 1st section).

            Feature[] fa = m_Face.GetNewFeatures(m_Line);
            foreach (Feature f in fa)
                f.Undo();

            // Restore the original line so long as this is the primary face
            if (IsPrimaryFace)
                m_Line.Restore();
        }

        /// <summary>
        /// Is this the initial subdivision for the line?
        /// </summary>
        /// <value>True if there is no alternate face, or there is an alternate face but this
        /// edit came first.</value>
        internal bool IsPrimaryFace
        {
            get
            {
                if (OtherSide == null)
                    return true;
                else
                    return (this.EditSequence < OtherSide.EditSequence);
            }
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The subdivided line (if any section corresponds to the supplied line).</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            if (m_Face != null && m_Face.HasSection(line))
                return m_Line;

            return null;
        }

        /// <summary>
        /// Definition of the line sections for this subdivision.
        /// </summary>
        internal LineSubdivisionFace Face
        {
            get { return m_Face; }
        }

        /// <summary>
        /// Exchanges update items that were previously generated via
        /// a call to <see cref="LineSubdivisionUpdateForm.GetUpdateItems"/>.
        /// </summary>
        /// <param name="data">The update data to apply to the edit (modified to
        /// hold the values that were previously defined for the edit)</param>
        public override void ExchangeData(UpdateItemCollection data)
        {
            UpdateItem face = data.GetUpdateItem(DataField.Face);
            if (face != null)
                m_Face.ExchangeData(face);
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<LineFeature>(DataField.Line, m_Line);
            editSerializer.WritePersistent<LineSubdivisionFace>(DataField.Face, m_Face);

            // When we initially write the primary face, this will always be undefined. Only the
            // alternate face will write out the reference to the other side.
            if (OtherSide != null)
                editSerializer.WriteInternalId(DataField.OtherSide, OtherSide.InternalId);

            editSerializer.WriteEntity(DataField.PointType, EditingController.Current.Project.DefaultPointType);
            editSerializer.WriteIdMappings(DataField.Ids, this.Features);
        }

        /// <summary>
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/> and <see cref="WriteUpdateItems"/>).
        /// </summary>
        /// <param name="face">The revised observed lengths for each subdivision section</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method). Never null, but may be an empty collection
        /// if the supplied face does not involve any changes.</returns>
        internal UpdateItemCollection GetUpdateItems(LineSubdivisionFace face)
        {
            UpdateItemCollection result = new UpdateItemCollection();

            if (!m_Face.HasIdenticalObservedLengths(face))
                result.Add(m_Face.GetUpdateItem(DataField.Face, face.ObservedLengths));

            return result;
        }

        #region IRevisable Members

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            // The logic that follows is based on the update items that get defined by GetUpdateItems

            UpdateItem face = data.GetUpdateItem(DataField.Face);
            if (face != null)
                editSerializer.WritePersistentArray<Distance>(DataField.Face, (Distance[])face.Value);
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();

            if (editDeserializer.IsNextField(DataField.Face))
            {
                Distance[] face = editDeserializer.ReadPersistentArray<Distance>(DataField.Face);
                result.Add(new UpdateItem(DataField.Face, face));
            }

            return result;
        }

        #endregion
    }
}
