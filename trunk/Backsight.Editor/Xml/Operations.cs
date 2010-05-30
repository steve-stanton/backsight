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
using Backsight.Editor.Operations;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (op.Previous != null)
                this.PreviousId = op.Previous.DataId;
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
            this.Point = new FeatureData(op.NewPoint);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            AttachPointOperation op = new AttachPointOperation(s, sequence);
            op.Line = s.MapModel.Find<LineFeature>(this.Line);
            op.PositionRatio = this.PositionRatio;
            op.NewPoint = new PointFeature(op, this.Point);
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
                this.Point[i] = (PointData)features[i].GetSerializableFeature();
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
                PointFeature pf = p.LoadPoint(op);
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

    public partial class FeatureTableData
    {
        public FeatureTableData()
        {
        }

        internal FeatureTableData(Operation op)
        {
            Feature[] feats = op.Features;

            List<PointFeature> points = GetFeaturesByType<PointFeature>(feats);

            if (points != null)
            {
                int defaultEntityId = points[0].EntityType.Id;

                this.Points = new PointArray();
                this.Points.DefaultEntity = defaultEntityId;
                this.Points.Point = new FeatureData[points.Count];

                for (int i=0; i<points.Count; i++)
                    this.Points.Point[i] = new FeatureData(points[i], defaultEntityId);
            }

            List<LineFeature> lines = GetFeaturesByType<LineFeature>(feats);

            if (lines != null)
            {
                int defaultEntityId = lines[0].EntityType.Id;

                this.Lines = new LineArray();
                this.Lines.DefaultEntity = defaultEntityId;
                this.Lines.Line = new FeatureData[lines.Count];

                for (int i = 0; i < lines.Count; i++)
                    this.Lines.Line[i] = new FeatureData(lines[i], defaultEntityId);
            }
        }

        List<T> GetFeaturesByType<T>(Feature[] features) where T : Feature
        {
            List<T> result = null;

            foreach (Feature f in features)
            {
                if (f is T)
                {
                    if (result == null)
                        result = new List<T>();

                    result.Add((T)f);
                }
            }

            return result;
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
                this.Feature[i] = features[i].GetSerializableFeature();
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
            this.To = new FeatureData(op.IntersectionPoint);

            if (op.CreatedDirectionLine != null)
                this.DirLine = new FeatureData(op.CreatedDirectionLine);

            if (op.CreatedDistanceLine != null)
                this.DistLine = new FeatureData(op.CreatedDistanceLine);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            IntersectDirectionAndDistanceOperation op = new IntersectDirectionAndDistanceOperation(s, sequence);

            Direction dir = (Direction)this.Direction.LoadObservation(op);
            Observation dist = this.Distance.LoadObservation(op);
            PointFeature from = s.MapModel.Find<PointFeature>(this.From);
            op.SetInput(dir, dist, from, this.Default);

            op.IntersectionPoint = new PointFeature(op, this.To);

            if (this.DirLine == null)
                op.CreatedDirectionLine = null;
            else
                op.CreatedDirectionLine = new LineFeature(op, dir.From, op.IntersectionPoint, this.DirLine);

            if (this.DistLine != null)
                op.CreatedDistanceLine = new LineFeature(op, op.DistanceFromPoint, op.IntersectionPoint, this.DistLine);

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
            this.To = new FeatureData(op.IntersectionPoint);

            if (op.CreatedDirectionLine != null)
                this.DirLine = new FeatureData(op.CreatedDirectionLine);

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
            uint sequence = GetEditSequence(s);
            IntersectDirectionAndLineOperation op = new IntersectDirectionAndLineOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            Direction dir = (Direction)this.Direction.LoadObservation(op);
            LineFeature line = mapModel.Find<LineFeature>(this.Line);
            PointFeature closeTo = mapModel.Find<PointFeature>(this.CloseTo);
            op.SetInput(dir, line, closeTo);

            op.IntersectionPoint = new PointFeature(op, this.To);

            if (this.DirLine == null)
                op.CreatedDirectionLine = null;
            else
                op.CreatedDirectionLine = new LineFeature(op, dir.From, op.IntersectionPoint, this.DirLine);

            LineFeature lineA, lineB;
            op.IsSplit = op.MakeSections(line, this.SplitBefore, op.IntersectionPoint, this.SplitAfter,
                                            out lineA, out lineB);
            op.LineBeforeSplit = lineA;
            op.LineAfterSplit = lineB;

            op.EnsureFeaturesAreIndexed();

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
            this.To = new FeatureData(op.IntersectionPoint);

            if (op.CreatedLine1 != null)
                this.Line1 = new FeatureData(op.CreatedLine1);

            if (op.CreatedLine2 != null)
                this.Line2 = new FeatureData(op.CreatedLine2);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            IntersectTwoDirectionsOperation op = new IntersectTwoDirectionsOperation(s, sequence);

            Direction dir1 = (Direction)this.Direction1.LoadObservation(op);
            Direction dir2 = (Direction)this.Direction2.LoadObservation(op);
            op.SetInput(dir1, dir2);

            op.IntersectionPoint = new PointFeature(op, this.To);

            if (this.Line1 == null)
                op.CreatedLine1 = null;
            else
                op.CreatedLine1 = new LineFeature(op, dir1.From, op.IntersectionPoint, this.Line1);

            if (this.Line2 == null)
                op.CreatedLine2 = null;
            else
                op.CreatedLine2 = new LineFeature(op, dir2.From, op.IntersectionPoint, this.Line2);

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
            this.To = new FeatureData(op.IntersectionPoint);
            this.Default = op.IsDefault;

            if (op.CreatedLine1 != null)
                this.Line1 = new FeatureData(op.CreatedLine1);

            if (op.CreatedLine2 != null)
                this.Line2 = new FeatureData(op.CreatedLine2);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            IntersectTwoDistancesOperation op = new IntersectTwoDistancesOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            Observation dist1 = this.Distance1.LoadObservation(op);
            PointFeature from1 = mapModel.Find<PointFeature>(this.From1);
            Observation dist2 = this.Distance2.LoadObservation(op);
            PointFeature from2 = mapModel.Find<PointFeature>(this.From2);
            op.SetInput(dist1, from1, dist2, from2, this.Default);

            op.IntersectionPoint = new PointFeature(op, this.To);

            if (this.Line1 == null)
                op.CreatedLine1 = null;
            else
                op.CreatedLine1 = new LineFeature(op, from1, op.IntersectionPoint, this.Line1);

            if (this.Line2 == null)
                op.CreatedLine2 = null;
            else
                op.CreatedLine2 = new LineFeature(op, from2, op.IntersectionPoint, this.Line2);

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
            this.To = new FeatureData(op.IntersectionPoint);

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
            IntersectTwoLinesOperation op = new IntersectTwoLinesOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            LineFeature line1 = mapModel.Find<LineFeature>(this.Line1);
            LineFeature line2 = mapModel.Find<LineFeature>(this.Line2);
            PointFeature closeTo = mapModel.Find<PointFeature>(this.CloseTo);
            op.SetInput(line1, line2, closeTo);

            op.IntersectionPoint = new PointFeature(op, this.To);

            LineFeature lineA, lineB;
            op.IsSplit1 = op.MakeSections(line1, this.SplitBefore1, op.IntersectionPoint, this.SplitAfter1, out lineA, out lineB);
            op.Line1BeforeSplit = lineA;
            op.Line1AfterSplit = lineB;

            op.IsSplit2 = op.MakeSections(line2, this.SplitBefore2, op.IntersectionPoint, this.SplitAfter2, out lineA, out lineB);
            op.Line2BeforeSplit = lineA;
            op.Line2AfterSplit = lineB;

            op.EnsureFeaturesAreIndexed();

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
            this.NewPoint = new FeatureData(op.NewPoint);

            if (op.NewLine != null)
                this.NewLine = new FeatureData(op.NewLine);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            LineExtensionOperation op = new LineExtensionOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            LineFeature extendLine = mapModel.Find<LineFeature>(this.Line);
            Distance length = (Distance)this.Distance.LoadObservation(op);
            op.SetInput(extendLine, this.ExtendFromEnd, length);

            op.NewPoint = new PointFeature(op, this.NewPoint);

            if (this.NewLine == null)
                op.NewLine = null;
            else
            {
                PointFeature p = (this.ExtendFromEnd ? extendLine.EndPoint : extendLine.StartPoint);

                if (extendLine is ArcFeature)
                {
                    ArcFeature arc = (extendLine as ArcFeature);
                    bool isClockwise = arc.IsClockwise;
                    if (!this.ExtendFromEnd)
                        isClockwise = !isClockwise;

                    op.NewLine = new ArcFeature(op, arc.Circle, p, op.NewPoint, isClockwise, this.NewLine);
                }
                else
                    op.NewLine = new LineFeature(op, p, op.NewPoint, this.NewLine);
            }

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
            this.DefaultEntryUnit = (int)EditingController.Current.EntryUnit.UnitType;
            this.EntryFromEnd = op.EntryFromEnd;
            this.Result = new FeatureTableData(op);
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
            DistanceUnit defaultEntryUnit = EditingController.Current.GetUnits(unitType);

            uint sequence = GetEditSequence(s);
            LineSubdivisionOperation op = new LineSubdivisionOperation(line, this.EntryString,
                defaultEntryUnit, this.EntryFromEnd, s, sequence);

            Distance[] dists = LineSubdivisionOperation.GetDistances(this.EntryString,
                                    defaultEntryUnit, this.EntryFromEnd);

            FeatureData[] lines = this.Result.Lines.Line;
            FeatureData[] points = this.Result.Points.Point;

            Debug.Assert(dists.Length == lines.Length);
            Debug.Assert(dists.Length == 1 + points.Length);

            MeasuredLineFeature[] sections = new MeasuredLineFeature[dists.Length];
            PointFeature start = line.StartPoint;
            PointFeature end;

            // Define sections without any underlying geometry
            for (int i = 0; i < dists.Length; i++)
            {
                if (i == (dists.Length - 1))
                    end = line.EndPoint;
                else
                    end = new PointFeature(op, points[i]);

                // Get the internal ID to assign to the line
                uint sessionId, lineSequence;
                InternalIdValue.Parse(lines[i].Id, out sessionId, out lineSequence);
                SectionGeometry sectionGeom = new SectionGeometry(line, start, end);
                LineFeature sectionFeature = line.MakeSubSection(sectionGeom, op);
                sectionFeature.CreatorSequence = lineSequence;
                MeasuredLineFeature mf = new MeasuredLineFeature(sectionFeature, dists[i]);
                sections[i] = mf;

                start = end;
            }

            op.Sections = sections;
            op.EnsureFeaturesAreIndexed();
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
            uint sequence = GetEditSequence(s);
            MovePolygonPositionOperation op = new MovePolygonPositionOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            op.Label = s.MapModel.Find<TextFeature>(this.Label);
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
            MoveTextOperation op = new MoveTextOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            op.MovedText = mapModel.Find<TextFeature>(this.Text);
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
            op.SetNewLine(new ArcFeature(op, this.Line));
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
            NewCircleOperation op = new NewCircleOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            op.Center = mapModel.Find<PointFeature>(this.Center);
            op.Radius = this.Radius.LoadObservation(op);

            // In order to create the construction line, we need to have the Circle object,
            // but to be able to find the circle, the radius has to be known... and if the
            // radius is specified via an offset point, the point probably has no defined
            // position at this stage.

            // ...so for now, just work with a circle that has no radius. This may get changed
            // when RunEdit is ultimately called.

            Circle c = new Circle(op.Center, 0.0);

            // If the closing point does not already exist, create one at some unspecified position
            PointFeature p = mapModel.Find<PointFeature>(this.ClosingPoint);
            if (p == null)
            {
                FeatureData ft = new FeatureData();
                ft.Id = this.ClosingPoint;
                p = new PointFeature(op, ft);
            }

            // Form the construction line (this will also cross-reference the circle to
            // the new arc)
            FeatureData at = new FeatureData();
            at.Id = this.Arc;
            ArcFeature arc = new ArcFeature(op, c, p, p, true, at);

            op.SetNewLine(arc);

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
            this.Text = new KeyTextData(op.Text);
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
            op.SetText(new TextFeature(op, this.Text));
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
            this.Text = new MiscTextData(op.Text);
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
            op.SetText(new TextFeature(op, this.Text));
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
            op.SetNewPoint((PointFeature)this.Point.LoadFeature(op));
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
            this.Text = new RowTextData(op.Text);
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
            op.SetText(new TextFeature(op, this.Text));
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
            this.Line = new SegmentData(op.Line);
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
            op.SetNewLine((LineFeature)this.Line.LoadFeature(op));
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
            this.From = new FeatureData(parLine.StartPoint, (parLine.StartPoint.Creator == op));
            this.To = new FeatureData(parLine.EndPoint, (parLine.EndPoint.Creator == op));
            this.NewLine = parLine.GetSerializableLine();
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            ParallelLineOperation op = new ParallelLineOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            LineFeature refLine = mapModel.Find<LineFeature>(this.RefLine);
            Observation offset = this.Offset.LoadObservation(op);
            LineFeature term1 = (this.Term1==null ? null : mapModel.Find<LineFeature>(this.Term1));
            LineFeature term2 = (this.Term2==null ? null : mapModel.Find<LineFeature>(this.Term2));
            op.SetInput(refLine, offset, term1, term2, this.ReverseArc);

            // Ensure the line end points have been created

            PointFeature from = mapModel.Find<PointFeature>(this.From.Id);
            if (from == null)
                from = new PointFeature(op, this.From);

            PointFeature to = mapModel.Find<PointFeature>(this.To.Id);
            if (to == null)
                to = new PointFeature(op, this.To);

            op.ParallelLine = (LineFeature)this.NewLine.LoadFeature(op);

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
            this.Result = new FeatureTableData(op);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            throw new NotImplementedException("PathData.LoadOperation");
            //return new PathOperation(s, this);
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

            LineFeature[] newLines = op.NewLines;
            SegmentData[] data = new SegmentData[newLines.Length];
            for (int i = 0; i < newLines.Length; i++)
            {
                data[i] = (SegmentData)newLines[i].GetSerializableLine();
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

            // Pick up any label to deactivate (this won't actually happen until
            // CalculateGeometry is called)

            CadastralMapModel mapModel = s.MapModel;
            if (this.DeactivatedLabel != null)
                op.DeactivatedLabel = mapModel.Find<TextFeature>(this.DeactivatedLabel);

            // Pick up the line segments that were created

            LineFeature[] newLines = new LineFeature[this.Line.Length];

            for (int i = 0; i < this.Line.Length; i++)
                newLines[i] = new LineFeature(op, this.Line[i]);

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
            this.To = new FeatureData(op.Point);

            if (op.Line != null)
                this.Line = new FeatureData(op.Line);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            RadialOperation op = new RadialOperation(s, sequence);

            Direction dir = (Direction)this.Direction.LoadObservation(op);
            Observation length = this.Length.LoadObservation(op);
            op.SetInput(dir, length);

            op.Point = new PointFeature(op, this.To);

            if (this.Line != null)
                op.Line = new LineFeature(op, dir.From, op.Point, this.Line);

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
            CadastralMapModel mapModel = s.MapModel;
            LineFeature line = mapModel.Find<LineFeature>(this.Line);
            uint sequence = GetEditSequence(s);
            return new SetTopologyOperation(s, line, sequence);
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
            this.NewPoint = new FeatureData(op.NewPoint);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            SimpleLineSubdivisionOperation op = new SimpleLineSubdivisionOperation(s, sequence);

            CadastralMapModel mapModel = s.MapModel;
            LineFeature line = mapModel.Find<LineFeature>(this.Line);
            Distance distance = (Distance)this.Distance.LoadObservation(op);
            op.SetInput(line, distance);

            op.NewPoint = new PointFeature(op, this.NewPoint);

            // Create the sections

            uint sessionId, lineSequence;

            op.NewLine1 = op.MakeSection(line.StartPoint, op.NewPoint);
            InternalIdValue.Parse(this.NewLine1, out sessionId, out lineSequence);
            op.NewLine1.CreatorSequence = lineSequence;
            mapModel.AddFeature(op.NewLine1);

            op.NewLine2 = op.MakeSection(op.NewPoint, line.EndPoint);
            InternalIdValue.Parse(this.NewLine2, out sessionId, out lineSequence);
            op.NewLine2.CreatorSequence = lineSequence;
            mapModel.AddFeature(op.NewLine2);

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
            return op;
        }
    }

    public partial class UpdateData
    {
        public UpdateData()
        {
        }

        /// <summary>
        /// Loads this update into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            uint sequence = GetEditSequence(s);
            return new UpdateOperation(s, this);
        }
    }
}
