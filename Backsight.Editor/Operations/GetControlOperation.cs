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
        internal GetControlOperation()
            : base()
        {
            m_Features = new List<PointFeature>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetControlOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal GetControlOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            PointFeature[] points = editDeserializer.ReadPersistentArray<PointFeature>(DataField.Points);
            m_Features = new List<PointFeature>(points);
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
        internal override void Undo()
        {
            base.OnRollback();

            // Mark for deletion each created point
            foreach (PointFeature p in m_Features)
                Rollback(p);
        }

        /// <summary>
        /// Executes this operation. 
        /// </summary>
        /// <param name="cps">The points to save (none of these should correspond to previously created
        /// features in the map model, otherwise an exception will be raised).</param>
        /// <param name="ent">The entity type to assign to control points</param>
        internal void Execute(ControlPoint[] cps, IEntity ent)
        {
            CadastralMapModel mapModel = CadastralMapModel.Current;

            foreach (ControlPoint cp in cps)
            {
                // Add a new point to the map & define it's ID
                PointFeature p = mapModel.AddPoint(cp, ent, this);

                // Create the new ID (and point the ID and feature to each other).
                string keystr = cp.ControlId.ToString();
                ForeignId fid = new ForeignId(keystr);
                fid.Add(p);

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
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>
        /// The referenced features (never null, but may be an empty array).
        /// </returns>
        public override Feature[] GetRequiredFeatures()
        {
            return new Feature[0];
        }

        /// <summary>
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/>).
        /// </summary>
        /// <param name="p">The control point that was modified</param>
        /// <param name="newPosition">The revised position</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItemCollection GetUpdateItems(PointFeature p, Position newPosition)
        {
            UpdateItemCollection result = new UpdateItemCollection();

            // Unconditionally add an item that identifies the feature involved. This
            // is kind of klunky, covering the fact that this dialog for updating control
            // only deals with one point at a time.
            result.AddFeature<PointFeature>(DataField.UpdatedPoint, p);

            result.AddItem<double>(DataField.X, p.PointGeometry.Easting.Meters, newPosition.X);
            result.AddItem<double>(DataField.Y, p.PointGeometry.Northing.Meters, newPosition.Y);
            return result;
        }

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            data.WriteFeature<PointFeature>(editSerializer, DataField.UpdatedPoint);
            data.WriteItem<double>(editSerializer, DataField.X);
            data.WriteItem<double>(editSerializer, DataField.Y);
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.ReadFeature<PointFeature>(editDeserializer, DataField.UpdatedPoint);
            result.ReadItem<double>(editDeserializer, DataField.X);
            result.ReadItem<double>(editDeserializer, DataField.Y);
            return result;
        }

        /// <summary>
        /// Exchanges any previously generated update items (this is currently done
        /// by <see cref="NewPointForm.GetUpdateItems"/>).
        /// </summary>
        /// <param name="data">The update data to apply to the edit (modified to
        /// hold the values that were previously defined for the edit)</param>
        public override void ExchangeData(UpdateItemCollection data)
        {
            // Only exchange if the model is being loaded - see comments in NewPointOperation.ExchangeData
            if (MapModel.WorkingSession == null)
                ApplyUpdateItems(null, data);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            // We only have to do stuff when processing an update (see comments in ExchangeData).

            if (ctx is UpdateEditingContext)
            {
                UpdateEditingContext uec = (ctx as UpdateEditingContext);
                ApplyUpdateItems(ctx, uec.UpdateSource.GetChanges(this));
            }
        }

        /// <summary>
        /// Applies changes to this editing operation.
        /// </summary>
        /// <param name="ctx">The editing context (null if the model is being deserialized)</param>
        /// <param name="data">The changes to apply</param>
        void ApplyUpdateItems(EditingContext ctx, UpdateItemCollection data)
        {
            // Locate the specific point that was modified
            //string id = data.GetValue<string>(DataField.Id);
            //InternalIdValue iid = new InternalIdValue(id);
            //PointFeature p = this.MapModel.Find<PointFeature>(iid);
            PointFeature p = data.GetValue<PointFeature>(DataField.UpdatedPoint);
            double x = data.ExchangeValue<double>(DataField.X, p.Easting.Meters);
            double y = data.ExchangeValue<double>(DataField.Y, p.Northing.Meters);
            PointGeometry pg = new PointGeometry(x, y);
            p.ApplyPointGeometry(ctx, pg);
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);
            editSerializer.WritePersistentArray<PointFeature>(DataField.Points, m_Features.ToArray());
        }
    }
}
