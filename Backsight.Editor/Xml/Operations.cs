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
using System.Collections.Generic;
using System.Diagnostics;
using System.Yaml.Serialization;
using System.Yaml;

using Backsight.Editor.Operations;
using Backsight.Editor.Observations;

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
        abstract internal Operation LoadOperation(Session s);

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

    public partial class AttachPointData
    {
        public AttachPointData()
        {
        }

        internal AttachPointData(AttachPointOperation op)
            : base(op)
        {
            this.Line = op.Line.DataId;
            this.PositionRatio = op.PositionRatio;
            this.Point = new FeatureStubData(op.NewPoint);
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
            AttachPointOperation op = new AttachPointOperation(s, sequence, line, this.PositionRatio);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("Point", this.Point);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class DeletionData
    {
        public DeletionData()
        {
        }

        internal DeletionData(DeletionOperation op)
            : base(op)
        {
            Feature[] dels = op.Deletions;
            this.Delete = new string[dels.Length];
            for (int i = 0; i < dels.Length; i++)
                this.Delete[i] = dels[i].DataId;
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            DeletionOperation op = new DeletionOperation(s, sequence);
            CadastralMapModel mapModel = s.MapModel;

            foreach (string id in this.Delete)
            {
                Feature f = mapModel.Find<Feature>(id);
                Debug.Assert(f != null);
                op.AddDeletion(f);
            }

            // Deactivate features (means they will never make it into the spatial index, and
            // any lines will be invisible as far as intersection tests are concerned).
            DeserializationFactory dff = new DeserializationFactory(op);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class GetControlData
    {
        public GetControlData()
        {
        }

        internal GetControlData(GetControlOperation op)
            : base(op)
        {
            Feature[] features = op.Features;
            this.Point = new PointData[features.Length];
            for (int i = 0; i < this.Point.Length; i++)
                this.Point[i] = DataFactory.Instance.ToData<PointData>(features[i]);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            GetControlOperation op = new GetControlOperation(s, sequence);

            foreach (PointData p in this.Point)
            {
                PointFeature pf = p.CreatePointFeature(op);
                op.AddControlPoint(pf);
            }

            return op;
        }
    }

    public partial class EditData
    {
        public EditData()
        {
        }
    }

    public partial class ImportData
    {
        public ImportData()
        {
        }

        internal ImportData(ImportOperation op)
            : base(op)
        {
            Feature[] features = op.Features;
            this.Feature = new FeatureData[features.Length];
            for (int i = 0; i < features.Length; i++)
                this.Feature[i] = DataFactory.Instance.ToData<FeatureData>(features[i]);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            ImportOperation op = new ImportOperation(s, sequence);

            Feature[] data = new Feature[this.Feature.Length];
            for (int i=0; i<data.Length; i++)
            {
                FeatureData f = this.Feature[i];
                data[i] = f.LoadFeature(op);
            }

            op.SetFeatures(data);
            return op;
        }
    }

    public partial class IntersectDirectionAndDistanceData
    {
        //public IntersectDirectionAndDistanceData()
        //{
        //}

        internal IntersectDirectionAndDistanceData(IntersectDirectionAndDistanceOperation op)
            : base(op)
        {
            this.From = op.DistanceFromPoint.DataId;
            this.Default = op.IsDefault;
            this.Direction = DataFactory.Instance.ToData<DirectionData>(op.Direction);
            this.Distance = DataFactory.Instance.ToData<ObservationData>(op.Distance);
            this.To = new FeatureStubData(op.IntersectionPoint);

            if (op.CreatedDirectionLine != null)
                this.DirLine = new FeatureStubData(op.CreatedDirectionLine);

            if (op.CreatedDistanceLine != null)
                this.DistLine = new FeatureStubData(op.CreatedDistanceLine);
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
            Observation dist = this.Distance.LoadObservation(loader);
            PointFeature from = loader.Find<PointFeature>(this.From);
            IntersectDirectionAndDistanceOperation op = new IntersectDirectionAndDistanceOperation(s, sequence,
                                                                dir, dist, from, this.Default);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("To", this.To);
            dff.AddFeatureStub("DirLine", this.DirLine);
            dff.AddFeatureStub("DistLine", this.DistLine);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class IntersectDirectionAndLineData
    {
        public IntersectDirectionAndLineData()
        {
        }

        internal IntersectDirectionAndLineData(IntersectDirectionAndLineOperation op)
            : base(op)
        {
            this.Line = op.Line.DataId;
            this.CloseTo = op.ClosePoint.DataId;
            this.Direction = DataFactory.Instance.ToData<DirectionData>(op.Direction);
            this.To = new FeatureStubData(op.IntersectionPoint);

            if (op.CreatedDirectionLine != null)
                this.DirLine = new FeatureStubData(op.CreatedDirectionLine);

            if (op.LineBeforeSplit != null)
                this.SplitBefore = op.LineBeforeSplit.DataId;

            if (op.LineAfterSplit != null)
                this.SplitAfter = op.LineAfterSplit.DataId;
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
            LineFeature line = loader.Find<LineFeature>(this.Line);
            bool wantSplit = (this.SplitBefore != null && this.SplitAfter != null);
            PointFeature closeTo = loader.Find<PointFeature>(this.CloseTo);
            IntersectDirectionAndLineOperation op = new IntersectDirectionAndLineOperation(s, sequence,
                                                            dir, line, wantSplit, closeTo);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("To", this.To);
            dff.AddFeatureStub("DirLine", this.DirLine);
            AddLineSplit(dff, line, "SplitBefore", this.SplitBefore);
            AddLineSplit(dff, line, "SplitAfter", this.SplitAfter);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class IntersectTwoDirectionsData
    {
        public IntersectTwoDirectionsData()
        {
        }

        internal IntersectTwoDirectionsData(IntersectTwoDirectionsOperation op)
            : base(op)
        {
            this.Direction1 = DataFactory.Instance.ToData<DirectionData>(op.Direction1);
            this.Direction2 = DataFactory.Instance.ToData<DirectionData>(op.Direction2);
            this.To = new FeatureStubData(op.IntersectionPoint);

            if (op.CreatedLine1 != null)
                this.Line1 = new FeatureStubData(op.CreatedLine1);

            if (op.CreatedLine2 != null)
                this.Line2 = new FeatureStubData(op.CreatedLine2);
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
            Direction dir1 = (Direction)this.Direction1.LoadObservation(loader);
            Direction dir2 = (Direction)this.Direction2.LoadObservation(loader);
            IntersectTwoDirectionsOperation op = new IntersectTwoDirectionsOperation(s, sequence, dir1, dir2);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("To", this.To);
            dff.AddFeatureStub("Line1", this.Line1);
            dff.AddFeatureStub("Line2", this.Line2);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class IntersectTwoDistancesData
    {
        //public IntersectTwoDistancesData()
        //{
        //}

        internal IntersectTwoDistancesData(IntersectTwoDistancesOperation op)
            : base(op)
        {
            this.From1 = op.Distance1FromPoint.DataId;
            this.Distance1 = DataFactory.Instance.ToData<ObservationData>(op.Distance1);
            this.From2 = op.Distance2FromPoint.DataId;
            this.Distance2 = DataFactory.Instance.ToData<ObservationData>(op.Distance2);
            this.To = new FeatureStubData(op.IntersectionPoint);
            this.Default = op.IsDefault;

            if (op.CreatedLine1 != null)
                this.Line1 = new FeatureStubData(op.CreatedLine1);

            if (op.CreatedLine2 != null)
                this.Line2 = new FeatureStubData(op.CreatedLine2);
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
            Observation dist1 = this.Distance1.LoadObservation(loader);
            PointFeature from1 = loader.Find<PointFeature>(this.From1);
            Observation dist2 = this.Distance2.LoadObservation(loader);
            PointFeature from2 = loader.Find<PointFeature>(this.From2);
            IntersectTwoDistancesOperation op = new IntersectTwoDistancesOperation(s, sequence, dist1, from1,
                                                        dist2, from2, this.Default);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("To", this.To);
            dff.AddFeatureStub("Line1", this.Line1);
            dff.AddFeatureStub("Line2", this.Line2);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class IntersectTwoLinesData
    {
        public IntersectTwoLinesData()
        {
        }

        internal IntersectTwoLinesData(IntersectTwoLinesOperation op)
            : base(op)
        {
            this.Line1 = op.Line1.DataId;
            this.Line2 = op.Line2.DataId;
            this.CloseTo = op.ClosePoint.DataId;
            this.To = new FeatureStubData(op.IntersectionPoint);

            if (op.Line1BeforeSplit != null)
                this.SplitBefore1 = op.Line1BeforeSplit.DataId;

            if (op.Line1AfterSplit != null)
                this.SplitAfter1 = op.Line1AfterSplit.DataId;

            if (op.Line2BeforeSplit != null)
                this.SplitBefore2 = op.Line2BeforeSplit.DataId;

            if (op.Line2AfterSplit != null)
                this.SplitAfter2 = op.Line2AfterSplit.DataId;
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
            LineFeature line1 = mapModel.Find<LineFeature>(this.Line1);
            bool wantSplit1 = (this.SplitBefore1 != null && this.SplitAfter1 != null);
            LineFeature line2 = mapModel.Find<LineFeature>(this.Line2);
            bool wantSplit2 = (this.SplitBefore2 != null && this.SplitAfter2 != null);
            PointFeature closeTo = mapModel.Find<PointFeature>(this.CloseTo);
            IntersectTwoLinesOperation op = new IntersectTwoLinesOperation(s, sequence, line1, wantSplit1,
                                                                            line2, wantSplit2, closeTo);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("To", this.To);
            AddLineSplit(dff, line1, "SplitBefore1", this.SplitBefore1);
            AddLineSplit(dff, line1, "SplitAfter1", this.SplitAfter1);
            AddLineSplit(dff, line2, "SplitBefore2", this.SplitBefore2);
            AddLineSplit(dff, line2, "SplitAfter2", this.SplitAfter2);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class LineExtensionData
    {
        //public LineExtensionData()
        //{
        //}

        internal LineExtensionData(LineExtensionOperation op)
            : base(op)
        {
            this.Line = op.ExtendedLine.DataId;
            this.ExtendFromEnd = op.IsExtendFromEnd;
            this.Distance = new DistanceData(op.Length);
            this.NewPoint = new FeatureStubData(op.NewPoint);

            if (op.NewLine != null)
                this.NewLine = new FeatureStubData(op.NewLine);
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
            LineFeature extendLine = loader.Find<LineFeature>(this.Line);
            Distance length =(Distance)this.Distance.LoadObservation(loader);
            LineExtensionOperation op = new LineExtensionOperation(s, sequence, extendLine,
                                                                    this.ExtendFromEnd, length);

            DeserializationFactory dff = new DeserializationFactory(op);
            dff.AddFeatureStub("NewPoint", this.NewPoint);
            dff.AddFeatureStub("NewLine", this.NewLine);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class LineSubdivisionData
    {
        public LineSubdivisionData()
        {
        }

        internal LineSubdivisionData(LineSubdivisionOperation op)
            : base(op)
        {
            this.Line = op.Parent.DataId;
            this.EntryString = op.EntryString;
            this.DefaultEntryUnit = (int)op.EntryUnit.UnitType;
            this.EntryFromEnd = op.EntryFromEnd;
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
            LineFeature line = mapModel.Find<LineFeature>(this.Line);
            if (line == null)
                throw new Exception("Cannot find line " + this.Line);

            DistanceUnitType unitType = (DistanceUnitType)this.DefaultEntryUnit;
            DistanceUnit defaultEntryUnit = EditingController.GetUnits(unitType);

            uint sequence = GetEditSequence(s);
            LineSubdivisionOperation op = new LineSubdivisionOperation(s, sequence,
                line, this.EntryString, defaultEntryUnit, this.EntryFromEnd);

            DeserializationFactory dff = this.Result.CreateFactory(op);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class MovePolygonPositionData
    {
        public MovePolygonPositionData()
        {
        }

        internal MovePolygonPositionData(MovePolygonPositionOperation op)
            : base(op)
        {
            PointGeometry oldPosition = op.OldPosition;
            PointGeometry newPosition = op.NewPosition;

            this.Label = op.Label.DataId;
            this.NewX = newPosition.Easting.Microns;
            this.NewY = newPosition.Northing.Microns;

            if (oldPosition != null)
            {
                this.OldX = oldPosition.Easting.Microns;
                this.OldY = oldPosition.Northing.Microns;

                this.OldXSpecified = this.OldYSpecified = true;
            }
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            TextFeature label = s.MapModel.Find<TextFeature>(this.Label);
            uint sequence = GetEditSequence(s);
            MovePolygonPositionOperation op = new MovePolygonPositionOperation(s, sequence, label);

            op.NewPosition = new PointGeometry(this.NewX, this.NewY);

            if (this.OldXSpecified && this.OldYSpecified)
                op.OldPosition = new PointGeometry(this.OldX, this.OldY);
            else
                op.OldPosition = null;

            return op;
        }
    }

    public partial class MoveTextData
    {
        public MoveTextData()
        {
        }

        internal MoveTextData(MoveTextOperation op)
            : base(op)
        {
            this.Text = op.MovedText.DataId;
            this.OldX = op.OldPosition.Easting.Microns;
            this.OldY = op.OldPosition.Northing.Microns;
            this.NewX = op.NewPosition.Easting.Microns;
            this.NewY = op.NewPosition.Northing.Microns;

            if (op.OldPolPosition != null)
            {
                this.OldPolygonX = op.OldPolPosition.Easting.Microns;
                this.OldPolygonY = op.OldPolPosition.Northing.Microns;

                this.OldPolygonXSpecified = this.OldPolygonYSpecified = true;
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
            TextFeature text = s.MapModel.Find<TextFeature>(this.Text);
            MoveTextOperation op = new MoveTextOperation(s, sequence, text);

            op.OldPosition = new PointGeometry(this.OldX, this.OldY);
            op.NewPosition = new PointGeometry(this.NewX, this.NewY);

            if (this.OldPolygonXSpecified && this.OldPolygonYSpecified)
                op.OldPolPosition = new PointGeometry(this.OldPolygonX, this.OldPolygonY);
            else
                op.OldPolPosition = null;

            return op;
        }
    }

    public partial class NewArcData
    {
        public NewArcData()
        {
        }

        internal NewArcData(NewArcOperation op)
            : base(op)
        {
            ArcFeature arc = (op.Line as ArcFeature);
            if (arc == null)
                throw new InvalidOperationException();

            this.Line = new ArcData(arc);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            NewArcOperation op = new NewArcOperation(s, sequence);
            ArcFeature arc = this.Line.CreateArcFeature(op);
            op.SetNewLine(arc);
            return op;
        }
    }

    public partial class NewCircleData
    {
        public NewCircleData()
        {
        }

        internal NewCircleData(NewCircleOperation op)
            : base(op)
        {
            this.Radius = DataFactory.Instance.ToData<ObservationData>(op.Radius);
            this.Center = op.Center.DataId;
            this.ClosingPoint = op.Line.StartPoint.DataId;
            this.Arc = op.Line.DataId;
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
            PointFeature center = loader.Find<PointFeature>(this.Center);
            Observation radius = this.Radius.LoadObservation(loader);
            NewCircleOperation op = new NewCircleOperation(s, sequence, center, radius);

            DeserializationFactory dff = new DeserializationFactory(op);

            // Remember closing point if it was created by the op.
            InternalIdValue cpid = new InternalIdValue(this.ClosingPoint);
            if (cpid.SessionId == s.Id && cpid.ItemSequence > sequence)
            {
                FeatureStubData cp = new FeatureStubData();
                cp.Id = this.ClosingPoint;
                dff.AddFeatureStub("ClosingPoint", cp);
            }

            FeatureStubData arc = new FeatureStubData();
            arc.Id = this.Arc;
            dff.AddFeatureStub("Arc", arc);
            op.ProcessFeatures(dff);
            return op;
        }
    }

    public partial class NewKeyTextData
    {
        public NewKeyTextData()
        {
        }

        internal NewKeyTextData(NewKeyTextOperation op)
            : base(op)
        {
            this.Text = new KeyTextData((KeyTextFeature)op.Text);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            NewKeyTextOperation op = new NewKeyTextOperation(s, sequence);
            KeyTextFeature f = this.Text.CreateKeyTextFeature(op);
            op.SetText(f);
            return op;
        }
    }

    public partial class NewMiscTextData
    {
        public NewMiscTextData()
        {
        }

        internal NewMiscTextData(NewMiscTextOperation op)
            : base(op)
        {
            this.Text = new MiscTextData((MiscTextFeature)op.Text);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            NewMiscTextOperation op = new NewMiscTextOperation(s, sequence);
            MiscTextFeature f = this.Text.CreateMiscTextFeature(op);
            op.SetText(f);
            return op;
        }
    }

    public partial class NewPointData
    {
        public NewPointData()
        {
        }

        internal NewPointData(NewPointOperation op)
            : base(op)
        {
            this.Point = new PointData(op.Point);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            NewPointOperation op = new NewPointOperation(s, sequence);
            PointFeature f = this.Point.CreatePointFeature(op);
            op.SetNewPoint(f);
            return op;
        }
    }

    public partial class NewRowTextData
    {
        public NewRowTextData()
        {
        }

        internal NewRowTextData(NewRowTextOperation op)
            : base(op)
        {
            this.Text = new RowTextData((RowTextFeature)op.Text);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            NewRowTextOperation op = new NewRowTextOperation(s, sequence);
            RowTextFeature f = this.Text.CreateRowTextFeature(op);
            op.SetText(f);
            return op;
        }
    }

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
            /*
            this.Name = item.Name;

            // If non-observation items are necessary, it may be better to work with
            // sub-classes (leave UpdateItem as an abstract base class).

            object iv = item.Value;
            Observation o = (iv as Observation);
            if (o != null)
                this.Value = DataFactory.Instance.ObservationToString(o);
            //else if (iv is Feature)
            //    this.Value = (iv as Feature).DataId;
            else
                throw new NotImplementedException("Cannot serialize update item: " + item.Name);
             */
        }

        internal UpdateItem LoadValue(ILoader loader)
        {
            /*
            object value = DataFactory.Instance.StringToObservation(this.Value);
            return new UpdateItem(this.Name, value);
             */
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

            // The root node always identifies an array of UpdateItem
            //YamlConfig yc = new YamlConfig();
            //yc.OmitTagForRootNode = true;
            //this.Changes = new YamlSerializer(yc).Serialize(dataItems);
            this.Changes = new YamlSerializer().Serialize(dataItems);
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
        }
    }
}