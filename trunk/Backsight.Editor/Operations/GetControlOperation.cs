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

using Backsight.Environment;
using Backsight.Editor.Observations;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="22-SEP-1998" was="CeGetControl" />
    /// <summary>
    /// Import control points.
    /// </summary>
    class GetControlOperation : Operation, IRevisable
    {
        #region Class data

        /// <summary>
        /// The point features that were added.
        /// </summary>
        readonly List<PointFeature> m_Features;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetControlOperation"/> class
        /// that refers to nothing.
        /// </summary>
        /// <param name="s">The session the operation should be referred to (the session itself
        /// is not modified until the editing operation is saved to the database).</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal GetControlOperation(Session s, uint sequence)
            : base(s, sequence)
        {
            m_Features = new List<PointFeature>();
        }

        #endregion

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The superseded line that the line of interest was derived from. Null if
        /// this edit did not create the line of interest.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            // This edit doesn't supersede anything
            return null;
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            return false;
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Import control"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { return m_Features.ToArray(); }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.GetControl; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Mark for deletion each created point
            foreach (PointFeature p in m_Features)
                Rollback(p);

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            throw new NotImplementedException();
            /*
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do, since the import data comes in with
            // absolute positions ...

            // Rollforward the base class.
            return base.OnRollforward();
             */
        }

        /// <summary>
        /// Executes this operation. 
        /// </summary>
        /// <param name="cps">The points to save</param>
        /// <param name="ent">The entity type to assign to control points</param>
        internal void Execute(ControlPoint[] cps, IEntity ent)
        {
            CadastralMapModel mapModel = CadastralMapModel.Current;

            foreach (ControlPoint cp in cps)
            {
                // Add a new point to the map & define it's ID
                PointFeature p = mapModel.AddPoint(cp, ent, this);
                IdHandle idh = new IdHandle(p);
                string keystr = cp.ControlId.ToString();
                idh.CreateForeignId(keystr);

                m_Features.Add(p);
            }

            Complete();
        }

        /// <summary>
        /// The number of point features that were created.
        /// </summary>
        internal int Count
        {
            get { return m_Features.Count; }
        }

        /// <summary>
        /// Records an additional control point as part of this edit.
        /// </summary>
        /// <param name="p">The point created by this edit.</param>
        /// <remarks>Used during deserialization from the database</remarks>
        internal void AddControlPoint(PointFeature p)
        {
            m_Features.Add(p);
        }
    }
}
