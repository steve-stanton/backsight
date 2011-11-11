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

        /// <summary>
        /// Records information for a line split
        /// </summary>
        /// <param name="dff">The factory used to generate new features</param>
        /// <param name="parentLine">The line that may be getting split</param>
        /// <param name="itemName">The name of the item that should be attached to the line split into</param>
        /// <param name="dataId">The internal ID of the resultant section (null if there is no split)</param>
        /// <returns>True if a line split was recorded, false if the <paramref name="dataId"/> is null.</returns>
        internal bool AddLineSplit(DeserializationFactory dff, LineFeature parentLine, string itemName, string dataId)
        {
            if (dataId == null)
                return false;

            uint sessionId, ss;
            InternalIdValue.Parse(dataId, out sessionId, out ss);
            dff.AddFeatureDescription(itemName, new FeatureStub(dff.Creator, ss, parentLine.EntityType, null));
            return true;
        }
    }

    /*
    public partial class NewSegmentData
    {
        public NewSegmentData()
        {
        }

        internal NewSegmentData(NewSegmentOperation op)
            : base(op)
        {
            this.Line = new SegmentData((SegmentLineFeature)op.Line);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            NewSegmentOperation op = new NewSegmentOperation(s, sequence);
            SegmentLineFeature f = this.Line.CreateSegmentLineFeature(op);
            op.SetNewLine(f);
            return op;
        }
    }
    */

    public partial class ParallelLineData
    {
        //public ParallelLineData()
        //{
        //}

        internal ParallelLineData(ParallelLineOperation op)
            : base(op)
        {
            this.RefLine = op.ReferenceLine.DataId;

            if (op.Terminal1 != null)
                this.Term1 = op.Terminal1.DataId;

            if (op.Terminal2 != null)
                this.Term2 = op.Terminal2.DataId;

            if (op.IsArcReversed)
                this.ReverseArc = true;

            this.Offset = DataFactory.Instance.ToData<ObservationData>(op.Offset);
            LineFeature parLine = op.ParallelLine;
            this.From = new FeatureStubData(parLine.StartPoint, (parLine.StartPoint.Creator == op));
            this.To = new FeatureStubData(parLine.EndPoint, (parLine.EndPoint.Creator == op));
            this.NewLine = new FeatureStubData(parLine);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            ILoader loader = s.MapModel;
            LineFeature refLine = loader.Find<LineFeature>(this.RefLine);
            Observation offset = this.Offset.LoadObservation(loader);
            LineFeature term1 = (this.Term1==null ? null : loader.Find<LineFeature>(this.Term1));
            LineFeature term2 = (this.Term2==null ? null : loader.Find<LineFeature>(this.Term2));
            ParallelLineOperation op = new ParallelLineOperation(s, sequence, refLine, offset, term1, term2, this.ReverseArc);

            DeserializationFactory dff = new DeserializationFactory(op);

            // Define terminal points only if they need to be created
            PointFeature p = op.OffsetPoint;
            if (p==null || p.DataId!=this.From.Id)
                dff.AddFeatureStub("From", this.From);

            if (p==null || p.DataId!=this.To.Id)
                dff.AddFeatureStub("To", this.To);

            dff.AddFeatureStub("NewLine", this.NewLine);

            op.ProcessFeatures(dff);
            return op;
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

    public partial class PolygonSubdivisionData
    {
        public PolygonSubdivisionData()
        {
        }

        internal PolygonSubdivisionData(PolygonSubdivisionOperation op)
            : base(op)
        {
            if (op.DeactivatedLabel != null)
                this.DeactivatedLabel = op.DeactivatedLabel.DataId;

            SegmentLineFeature[] newLines = op.NewLines;
            SegmentData[] data = new SegmentData[newLines.Length];
            for (int i = 0; i < newLines.Length; i++)
            {
                data[i] = new SegmentData(newLines[i]);
            }

            this.Line = data;
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            PolygonSubdivisionOperation op = new PolygonSubdivisionOperation(s, sequence);

            // Pick up any label that needs to be deactivated
            if (this.DeactivatedLabel != null)
            {
                TextFeature label = s.MapModel.Find<TextFeature>(this.DeactivatedLabel);
                label.IsInactive = true;
                op.DeactivatedLabel = label;
            }

            // Create the line segments
            SegmentLineFeature[] newLines = new SegmentLineFeature[this.Line.Length];

            for (int i = 0; i < this.Line.Length; i++)
                newLines[i] = this.Line[i].CreateSegmentLineFeature(op);

            op.NewLines = newLines;
            return op;
        }
    }

    public partial class RadialData
    {
        public RadialData()
        {
        }

        internal RadialData(RadialOperation op)
            : base(op)
        {
            this.Direction = DataFactory.Instance.ToData<DirectionData>(op.Direction);
            this.Length = DataFactory.Instance.ToData<ObservationData>(op.Length);
            this.To = new FeatureStubData(op.Point);

            if (op.Line != null)
                this.Line = new FeatureStubData(op.Line);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            ILoader loader = s.MapModel;
            uint sequence = GetEditSequence(s);
            Direction dir = (Direction)this.Direction.LoadObservation(loader);
            Observation length = this.Length.LoadObservation(loader);
            RadialOperation op = new RadialOperation(s, sequence, dir, length);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("To", this.To);
            dff.AddFeatureStub("Line", this.Line);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class SetTopologyData
    {
        public SetTopologyData()
        {
        }

        internal SetTopologyData(SetTopologyOperation op)
            : base(op)
        {
            this.Line = op.Line.DataId;
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            LineFeature line = s.MapModel.Find<LineFeature>(this.Line);
            SetTopologyOperation op = new SetTopologyOperation(s, sequence, line);
            op.ProcessFeatures(new DeserializationFactory(op));
            return op;
        }
    }

    public partial class SimpleLineSubdivisionData
    {
        public SimpleLineSubdivisionData()
        {
        }

        internal SimpleLineSubdivisionData(SimpleLineSubdivisionOperation op)
            : base(op)
        {
            this.Line = op.Line.DataId;
            this.NewLine1 = op.NewLine1.DataId;
            this.NewLine2 = op.NewLine2.DataId;
            this.Distance = DataFactory.Instance.ToData<DistanceData>(op.Distance);
            this.NewPoint = new FeatureStubData(op.NewPoint);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            CadastralMapModel mapModel = s.MapModel;
            LineFeature line = mapModel.Find<LineFeature>(this.Line);
            Distance distance = (Distance)this.Distance.LoadObservation(mapModel);
            SimpleLineSubdivisionOperation op = new SimpleLineSubdivisionOperation(s, sequence, line, distance);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("NewPoint", this.NewPoint);
            AddLineSplit(dff, line, "NewLine1", this.NewLine1);
            AddLineSplit(dff, line, "NewLine2", this.NewLine2);
            op.ProcessFeatures(dff);

            return op;
        }
    }

    public partial class TextRotationData
    {
        public TextRotationData()
        {
        }

        internal TextRotationData(TextRotationOperation op)
            : base(op)
        {
            this.Value = RadianValue.AsString(op.RotationInRadians);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            TextRotationOperation op = new TextRotationOperation(s, sequence);

            double rotation;
            if (!RadianValue.TryParse(this.Value, out rotation))
                throw new ArgumentException("Cannot parse angle: " + this.Value);
            op.RotationInRadians = rotation;

            return op;
        }
    }

    public partial class TrimLineData
    {
        public TrimLineData()
        {
        }

        internal TrimLineData(TrimLineOperation op)
            : base(op)
        {
            LineFeature[] trimmedLines = op.TrimmedLines;
            if (trimmedLines == null)
                this.Line = new string[0];
            else
            {
                this.Line = new string[trimmedLines.Length];
                for (int i = 0; i < trimmedLines.Length; i++)
                    this.Line[i] = trimmedLines[i].DataId;
            }

            PointFeature[] trimPoints = op.TrimPoints;
            if (trimPoints == null)
                this.Point = new string[0];
            else
            {
                this.Point = new string[trimPoints.Length];
                for (int i = 0; i < trimPoints.Length; i++)
                    this.Point[i] = trimPoints[i].DataId;
            }
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            TrimLineOperation op = new TrimLineOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;

            string[] lineIds = this.Line;
            LineFeature[] lines = new LineFeature[lineIds.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = mapModel.Find<LineFeature>(lineIds[i]);
                Debug.Assert(lines[i] != null);
            }

            string[] pointIds = this.Point;
            PointFeature[] points = new PointFeature[pointIds.Length];
            for (int i=0; i< points.Length; i++)
            {
                points[i] = mapModel.Find<PointFeature>(pointIds[i]);
                Debug.Assert(points[i] != null);
            }

            op.TrimmedLines = lines;
            op.TrimPoints = points;
            op.ProcessFeatures(null);
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
