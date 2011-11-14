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
        /// Definition of the original face for this subdivision.
        /// </summary>
        readonly LineSubdivisionFace m_PrimaryFace;

        /// <summary>
        /// A secondary face that was included via an update to the original edit.
        /// </summary>
        LineSubdivisionFace m_AlternateFace;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionOperation"/> class.
        /// </summary>
        /// <param name="s">The session the operation should be referred to (the session itself
        /// is not modified until the editing operation is saved to the database).</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="line">The line that is being subdivided.</param>
        /// <param name="entryString">The data entry string that defines the subdivision sections.</param>
        /// <param name="defaultEntryUnit">The default distance units to use when decoding
        /// the data entry string.</param>
        /// <param name="isEntryFromEnd">Are the distances observed from the end of the line?</param>
        internal LineSubdivisionOperation(Session session, uint sequence, LineFeature line,
                                            string entryString, DistanceUnit defaultEntryUnit, bool isEntryFromEnd)
            : base(session, sequence)
        {
            m_Line = line;
            m_PrimaryFace = new LineSubdivisionFace(entryString, defaultEntryUnit, isEntryFromEnd);
            m_AlternateFace = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal LineSubdivisionOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            FeatureStub[] sections;
            ReadData(editDeserializer, out m_Line, out m_PrimaryFace, out sections);

            DeserializationFactory result = new DeserializationFactory(this, sections);
            ProcessFeatures(result);
        }

        #endregion

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
            get { return "Line subdivision"; }
        }

        /// <summary>
        /// Execute line subdivision.
        /// </summary>
        internal void Execute()
        {
            FeatureFactory ff = new FeatureFactory(this);

            // There's no need to actually define anything in the factory as far as points are
            // concerned - the ProcessFeatures method will end up using the default entity type
            // for points, and assign new feature IDs if necessary.

            // Same deal for lines. In that caee, ProcessFeatures creates sections that are
            // like the subdivided line.

            base.Execute(ff);
        }

        /// <summary>
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            // Create the sections along the primary face (the alternate face is
            // handled via an editing update).
            m_PrimaryFace.CreateSections(m_Line, ff);

            // Retire the original line
            ff.Deactivate(m_Line);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            m_PrimaryFace.CalculateGeometry(m_Line, ctx);
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The corresponding distance (null if not found)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            if (line==null)
                return null;

            Distance d = null;

            if (m_PrimaryFace != null)
                d = m_PrimaryFace.GetDistance(line);

            if (d == null && m_AlternateFace != null)
                d = m_AlternateFace.GetDistance(line);

            return d;
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>();

                if (m_PrimaryFace != null)
                    result.AddRange(m_PrimaryFace.GetNewFeatures(m_Line));

                if (m_AlternateFace != null)
                    result.AddRange(m_AlternateFace.GetNewFeatures(m_Line));

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
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            m_Line.CutOp(this);

            // Go through each section we created, marking each one as
            // deleted. Also mark the point features at the start of each
            // section, so long as it was created by this operation (should
            // do nothing for the 1st section).

            // Only the primary face is relevant, since the alternate face
            // is created via an editing update.

            Feature[] fa = m_PrimaryFace.GetNewFeatures(m_Line);
            foreach (Feature f in fa)
                f.Undo();

            // Restore the original line
            m_Line.Restore();
            return true;
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The subdivided line (if any section corresponds to the supplied line).</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            if (m_PrimaryFace != null && m_PrimaryFace.HasSection(line))
                return m_Line;

            if (m_AlternateFace != null && m_AlternateFace.HasSection(line))
                return m_Line;

            return null;
        }

        /// <summary>
        /// Definition of the original face for this subdivision.
        /// </summary>
        internal LineSubdivisionFace PrimaryFace
        {
            get { return m_PrimaryFace; }
        }

        /// <summary>
        /// A secondary face that was included via an update to the original edit.
        /// </summary>
        internal LineSubdivisionFace AlternateFace
        {
            get { return m_AlternateFace; }
            set { m_AlternateFace = value; }
        }

        /// <summary>
        /// Exchanges update items that were previously generated via
        /// a call to <see cref="LineSubdivisionUpdateForm.GetUpdateItems"/>.
        /// </summary>
        /// <param name="data">The update data to apply to the edit (modified to
        /// hold the values that were previously defined for the edit)</param>
        public override void ExchangeData(UpdateItemCollection data)
        {
            foreach (UpdateItem item in data.ToArray())
            {
                // Items that start with "A" relate to the flip status of the annotation
                // (see LineSubdivisionUpdateForm.GetUpdateItems)
                string dataId = item.Name;
                bool isAnnoChange = dataId.StartsWith("A");
                if (isAnnoChange)
                    dataId = dataId.Substring(1);

                MeasuredLineFeature mf = FindObservedLine(dataId);

                if (isAnnoChange)
                    mf.Line.IsLineAnnotationFlipped = !mf.Line.IsLineAnnotationFlipped;
                else
                    mf.ObservedLength = data.ExchangeObservation<Distance>(this, mf.Line.DataId, mf.ObservedLength);
            }
        }

        /// <summary>
        /// Attempts to locate the line section with a specific ID.
        /// </summary>
        /// <param name="dataId">The ID to look for</param>
        /// <returns>The observation for the corresponding section (null if not found)</returns>
        MeasuredLineFeature FindObservedLine(string dataId)
        {
            MeasuredLineFeature result = null;

            if (m_PrimaryFace != null)
                result = m_PrimaryFace.FindObservedLine(dataId);

            if (result == null && m_AlternateFace != null)
                result = m_AlternateFace.FindObservedLine(dataId);

            return result;
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            // The alternate face is currently added only via update edits
            editSerializer.WriteFeatureRef<LineFeature>("Line", m_Line);
            editSerializer.WritePersistent<LineSubdivisionFace>("PrimaryFace", m_PrimaryFace);
            editSerializer.WriteFeatureStubArray("Result", this.Features);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="line">The line that was subdivided.</param>
        /// <param name="primaryFace">Definition of the original face for this subdivision.</param>
        /// <param name="result">Information about the created line sections.</param>
        static void ReadData(EditDeserializer editDeserializer, out LineFeature line, out LineSubdivisionFace primaryFace,
                                out FeatureStub[] result)
        {
            line = editDeserializer.ReadFeatureRef<LineFeature>("Line");
            primaryFace = editDeserializer.ReadPersistent<LineSubdivisionFace>("PrimaryFace");
            result = editDeserializer.ReadFeatureStubArray("Result");
        }
    }
}
