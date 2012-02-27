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
    /// <written by="Steve Stanton" on="22-DEC-1997" was="CeDeletion" />
    /// <summary>
    /// Operation to delete features. When a feature gets deleted, it doesn't disappear,
    /// and it doesn't get garbage collected. It just gets marked as deleted, and will
    /// be retained as part of the map history.
    /// </summary>
    class DeletionOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The features that were deleted.
        /// </summary>
        List<Feature> m_Deletions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionOperation"/> class
        /// that refers to nothing.
        /// </summary>
        internal DeletionOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal DeletionOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            ReadData(editDeserializer, out m_Deletions);

            // Deactivate features (means they will never make it into the spatial index, and
            // any lines will be invisible as far as intersection tests are concerned).
            DeserializationFactory dff = new DeserializationFactory(this);
            ProcessFeatures(dff);
        }

        #endregion

        /// <summary>
        /// The user-perceived title for this operation is "Deletion"
        /// </summary>
        public override string Name
        {
            get { return "Deletion"; }
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        /// <returns>An empty array</returns>
        internal override Feature[] Features
        {
            get { return new Feature[0]; }
        }

        /// <summary>
        /// The deleted features
        /// </summary>
        internal Feature[] Deletions
        {
            get
            {
                if (m_Deletions==null)
                    return new Feature[0];

                List<Feature> result = new List<Feature>(m_Deletions.Count);
                result.AddRange(m_Deletions);
                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.Deletion; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
        {
            base.OnRollback();

            // Restore everything that was deleted.
            foreach(Feature f in m_Deletions)
                f.Restore();
        }

        /// <summary>
        /// Adds an additional feature to the things to be deleted.
        /// </summary>
        /// <param name="f">The feature to add to the deletions list</param>
        internal void AddDeletion(Feature f)
        {
            if (m_Deletions==null)
                m_Deletions = new List<Feature>(1);

            m_Deletions.Add(f);
        }

        /// <summary>
        /// Executes this operation. Before calling this function, you must make at
        /// least one call to <see cref="AddDeletion"/>.
        /// </summary>
        internal void Execute()
        {
            // Confirm that at least one call to AddDeletion has been made.
            if (m_Deletions==null)
                throw new Exception("Deletion.Execute - Nothing to delete.");

            // Stick the features that were explicitly noted into the complete list
            List<Feature> all = new List<Feature>(m_Deletions);

            // Loop through the features, checking for point features that
            // have attached lines.
            foreach (Feature f in m_Deletions)
            {
                if (f is PointFeature && f.HasDependents)
                {
                    // Go through incident lines, checking to see it they're in the
                    // deletions list. If not, remember them in the extras list.
                    PointFeature p = (f as PointFeature);
                    foreach (IFeatureDependent fd in p.Dependents)
                    {
                        if (fd is LineFeature)
                        {
                            LineFeature line = (fd as LineFeature);
                            
                            // Ignore lines that pass THROUGH the point (we only want to remove
                            // lines that terminate at the point)
                            if (line.StartPoint == f || line.EndPoint == f)
                            {
                                if (!line.IsUndoing && !all.Contains(line))
                                    all.Add(line);
                            }
                        }
                    }
                }
            }

            // Refresh the list if we added in anything extra
            if (all.Count > m_Deletions.Count)
                m_Deletions = all;

            FeatureFactory ff = new FeatureFactory(this);
            base.Execute(ff);

            //// Mark the features as deleted
            //foreach (Feature f in m_Deletions)
            //    f.IsInactive = true;

            //Complete();
        }

        /// <summary>
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            foreach (Feature f in m_Deletions)
                ff.Deactivate(f);
        }

        /// <summary>
        /// The number of features deleted by this edit.
        /// </summary>
        /// <remarks>This property is used by <see cref="SessionForm"/> to show
        /// the number of feature involved in each edit.</remarks>
        public override uint FeatureCount
        {
            get { return (uint)m_Deletions.Count; }
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>Null (always), since this edit doesn't supersede any lines.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>
        /// The referenced features (never null, but may be an empty array).
        /// </returns>
        public override Feature[] GetRequiredFeatures()
        {
            return m_Deletions.ToArray();
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// <para/>
        /// This override does nothing. A deletions operates directly on the referenced features
        /// (sets a special flag bit).
        /// </summary>
        public override void AddReferences()
        {
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);
            editSerializer.WriteFeatureRefArray<Feature>(DataField.Delete, m_Deletions.ToArray());
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="line">The line the point should appear on </param>
        /// <param name="positionRatio">The position ratio of the attached point.</param>
        /// <param name="point">The point that was created.</param>
        static void ReadData(EditDeserializer editDeserializer, out List<Feature> deletions)
        {
            Feature[] dels = editDeserializer.ReadFeatureRefArray<Feature>(DataField.Delete);
            deletions = new List<Feature>(dels);
        }
    }
}
