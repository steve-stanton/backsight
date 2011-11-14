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
using System.Diagnostics;

using Backsight.Editor.Observations;
using Backsight.Editor.Operations;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Base class for any sort of serialized editing operation.
    /// </summary>
    /// <remarks>The remainder of this class is auto-generated, and may be found
    /// in the <c>Edits.cs</c> file.</remarks>
    abstract public partial class OperationData
    {
        public OperationData()
        {
        }

        internal OperationData(Operation op)
        {
            this.Id = op.DataId;
        }

        /// <summary>
        /// Obtains the session sequence number associated with this edit.
        /// </summary>
        /// <param name="s">The session that this edit is supposedly part of (used for internal
        /// consistency check)</param>
        /// <returns>The sequence number of this edit.</returns>
        internal uint GetEditSequence(Session s)
        {
            uint sessionId, sequence;
            InternalIdValue.Parse(this.Id, out sessionId, out sequence);
            Debug.Assert(s.Id == sessionId);
            return sequence;
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        //abstract internal Operation LoadOperation(Session s);
        internal virtual Operation LoadOperation(Session s)
        {
            throw new ApplicationException();
        }
    }

    public partial class PathData
    {
        public PathData()
        {
        }

        internal PathData(PathOperation op)
            : base(op)
        {
            this.From = op.StartPoint.DataId;
            this.To = op.EndPoint.DataId;
            this.EntryString = op.EntryString;
            this.DefaultEntryUnit = (int)op.EntryUnit.UnitType;
            this.Result = new FactoryData(op);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            CadastralMapModel mapModel = s.MapModel;
            PointFeature from = mapModel.Find<PointFeature>(this.From);
            PointFeature to = mapModel.Find<PointFeature>(this.To);
            DistanceUnitType unitType = (DistanceUnitType)this.DefaultEntryUnit;
            DistanceUnit defaultEntryUnit = EditingController.GetUnits(unitType);

            uint sequence = GetEditSequence(s);
            PathOperation op = new PathOperation(s, sequence, from, to, this.EntryString, defaultEntryUnit);

            DeserializationFactory dff = this.Result.CreateFactory(op);
            op.ProcessFeatures(dff);

            // Create the legs
            /*
            LegData[] legs = t.Leg;
            m_Legs = new List<Leg>(legs.Length);
            PointFeature startPoint = m_From;
            IEntity lineType = EnvironmentContainer.FindEntityById(t.LineType);

            for (int i = 0; i < legs.Length; i++)
            {
                Leg leg = t.Leg[i].LoadLeg(this);
                m_Legs.Add(leg);

                // Create features for each span (without any geometry)
                startPoint = leg.CreateSpans(this, t.Leg[i].Span, startPoint, lineType);
            }
            */
            return op;
        }
    }

    public partial class UpdateItemData
    {
        public UpdateItemData()
        {
        }

        internal UpdateItemData(UpdateItem item)
        {
            throw new NotImplementedException("UpdateItemData");
        }

        internal UpdateItem LoadValue(ILoader loader)
        {
            throw new NotImplementedException("UpdateItemData.LoadValue");
        }
    }

    public partial class UpdateData
    {
        public UpdateData()
        {
        }

        internal UpdateData(UpdateOperation op)
            : base(op)
        {
            this.RevisedEdit = op.RevisedEdit.DataId;

            // Re-express update items using *Data objects
            UpdateItem[] items = op.Changes.ToArray();
            UpdateItem[] dataItems = new UpdateItem[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                object o = items[i].Value;

                if (o is Feature)
                    o = (o as Feature).DataId;
                else if (o is Observation)
                    o = DataFactory.Instance.ToData<ObservationData>((Observation)o);

                dataItems[i] = new UpdateItem(items[i].Name, o);
            }

            throw new NotImplementedException();

            // The root node always identifies an array of UpdateItem
            //this.Changes = new YamlSerializer().Serialize(dataItems);
        }

        /// <summary>
        /// Loads this update into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            CadastralMapModel mapModel = s.MapModel;
            uint sequence = GetEditSequence(s);
            Operation rev = mapModel.FindOperation(this.RevisedEdit);

            throw new NotImplementedException();
            /*
            YamlSerializer ys = new YamlSerializer();
            object[] oa = ys.Deserialize(this.Changes);
            Debug.Assert(oa.Length == 1);
            UpdateItem[] dataItems = (UpdateItem[])oa[0];
            UpdateItemCollection uc = new UpdateItemCollection();

            foreach (UpdateItem item in dataItems)
            {
                object o = item.Value;

                if (o is ObservationData)
                    item.Value = (o as ObservationData).LoadObservation(mapModel);
                else
                {
                    // If it's a string, it could be the internal ID for a feature
                    if (o is string)
                    {
                        try
                        {
                            InternalIdValue id = new InternalIdValue(o.ToString());
                            Feature f = mapModel.Find<Feature>(id);
                            item.Value = f;
                        }

                        catch {}
                    }
                }

                uc.Add(item);
            }

            return new UpdateOperation(s, sequence, rev, uc);
             */
        }
    }
}
