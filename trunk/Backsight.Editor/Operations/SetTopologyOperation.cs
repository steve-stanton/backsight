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

using Backsight.Editor.Observations;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="30-MAR-1999" was="CeSetTopology" />
    /// <summary>
    /// Edit that toggles the topological status of a line.
    /// </summary>
    class SetTopologyOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The line altered by the edit.
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// True if the line should be marked as topological.
        /// </summary>
        /// <remarks>During normal editing work, this should just be the reverse of the initial
        /// topological status. The value is noted explicitly mainly to cover confusion that
        /// could arise when importing old data from CEdit.
        /// </remarks>
        readonly bool m_Topological;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTopologyOperation"/> class.
        /// </summary>
        /// <param name="line">The line that needs to be changed.</param>
        internal SetTopologyOperation(LineFeature line)
            : base()
        {
            if (line == null)
                throw new ArgumentNullException();

            m_Line = line;
            m_Topological = !line.IsTopological;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTopologyOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal SetTopologyOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            m_Line = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Line);
            m_Topological = editDeserializer.ReadBool(DataField.Topological);
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Polygon boundary status"; }
        }

        /// <summary>
        /// The features created by this editing operation (an empty array).
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
            get { return EditingActionId.SetTopology; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
        {
            base.OnRollback();

            if (m_Line.IsTopological != m_Topological)
                m_Line.SwitchTopology(false);
        }

        /// <summary>
        /// Executes the edit.
        /// </summary>
        internal void Execute()
        {
            base.Execute(new FeatureFactory(this));
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            if (m_Line.IsTopological != m_Topological)
            {
                bool isLoading = (ctx is LoadingContext);

                // Do nothing if the line is already inactive! During normal editing work, this should
                // never be the case. However, during project loading, the line may have been marked as
                // inactive when a DeletionOperation was deserialized (I believe the only good reason
                // for doing it at that stage is so that any subsequent processing can be skipped).
                // Since CalculateGeometry gets called for all edits after ALL edits have been deserialized,
                // a SetTopologyOperation that came prior to the deletion would end up adding a topological
                // construct (something that should never apply to inactive features).

                // The later call to DeletionOperation.CalculateGeometry does nothing (since DeletionOperation
                // doesn't actually implement it), so we end up with topology for an inactive line. We could
                // implement it (making it remove any topology during loading), but doing it here seems equally ok.

                if (!m_Line.IsInactive)
                    m_Line.SwitchTopology(isLoading);
            }
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
        /// The line altered by the edit.
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
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
            return new Feature[] { m_Line };
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);
            editSerializer.WriteFeatureRef<LineFeature>(DataField.Line, m_Line);
            editSerializer.WriteBool(DataField.Topological, m_Topological);
        }
    }
}
