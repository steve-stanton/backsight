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

        /// <summary>
        /// Converts XML data into the input for an editing operation.
        /// </summary>
        /// <param name="loader">Deserialization helper</param>
        /// <returns>The input for the editing operation</returns>
        /// <remarks>This should ultimately be declared as an abstract method</remarks>
        internal virtual OperationInput GetInput(ILoader loader)
        {
            throw new NotImplementedException();
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
            this.Point = new FeatureData(op.NewPoint);
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new AttachPointOperation(s, this);
        }

        /// <summary>
        /// Converts XML data into the input for an editing operation.
        /// </summary>
        /// <param name="loader">Deserialization helper</param>
        /// <returns>The input for the editing operation</returns>
        /// <remarks>This should ultimately be declared as an abstract method</remarks>
        internal override OperationInput GetInput(ILoader loader)
        {
            LineFeature line = loader.Find<LineFeature>(this.Line);
            return new AttachPointInput(line, this.PositionRatio);
        }
    }

    public partial class DeletionData
    {
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
            return new DeletionOperation(s, this);
        }
    }

    public partial class GetControlData
    {
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
            return new GetControlOperation(s, this);
        }
    }

    public partial class FeatureTableData
    {
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
            return new IntersectDirectionAndDistanceOperation(s, this);
        }
    }

    public partial class IntersectDirectionAndLineData
    {
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
            return new IntersectDirectionAndLineOperation(s, this);
        }
    }

    public partial class IntersectTwoDirectionsData
    {
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
            return new IntersectTwoDirectionsOperation(s, this);
        }
    }

    public partial class IntersectTwoDistancesData
    {
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
            return new IntersectTwoDistancesOperation(s, this);
        }
    }

    public partial class IntersectTwoLinesData
    {
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
            return new IntersectTwoLinesOperation(s, this);
        }
    }

    public partial class LineExtensionData
    {
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
            return new LineExtensionOperation(s, this);
        }
    }

    public partial class LineSubdivisionData
    {
        internal LineSubdivisionData(LineSubdivisionOperation op)
            : base(op)
        {
            this.Line = op.Parent.DataId;
            this.EntryString = op.GetEntryString();
            MeasuredLineFeature[] sections = op.Sections;

            this.Result = new FeatureTableData(op);
            /*
            this.Result.Lines = new LineArray();
            this.Result.Lines.DefaultEntity = sections[0].Line.EntityType.Id;
            this.Result.Lines.Line = new FeatureData[sections.Length];

            this.Result.Points = new PointArray();
            this.Result.Points.DefaultEntity = sections[0].Line.EndPoint.EntityType.Id;
            this.Result.Points.Point = new FeatureData[sections.Length - 1];

            for (int i = 0; i < sections.Length; i++)
            {
                MeasuredLineFeature mf = sections[i];
                this.Result.Lines.Line[i].Id = mf.Line.DataId;

                if (i < (sections.Length - 1))
                {
                    FeatureData fd = this.Result.Points.Point[i];
                    PointFeature p = mf.Line.EndPoint;
                    fd.Id = p.DataId;

                    if (p.Id != null)
                        fd.Key = p.Id.RawId;
                }
            }
             */
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new LineSubdivisionOperation(s, this);
        }
    }

    public partial class MovePolygonPositionData
    {
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
            return new MovePolygonPositionOperation(s, this);
        }
    }

    public partial class MoveTextData
    {
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
            return new MoveTextOperation(s, this);
        }
    }

    public partial class NewArcData
    {
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
            return new NewArcOperation(s, this);
        }
    }

    public partial class NewCircleData
    {
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
            return new NewCircleOperation(s, this);
        }
    }

    public partial class NewKeyTextData
    {
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
            return new NewKeyTextOperation(s, this);
        }
    }

    public partial class NewMiscTextData
    {
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
            return new NewMiscTextOperation(s, this);
        }
    }

    public partial class NewPointData
    {
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
            return new NewPointOperation(s, this);
        }
    }

    public partial class NewRowTextData
    {
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
            return new NewRowTextOperation(s, this);
        }
    }

    public partial class NewSegmentData
    {
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
            return new NewSegmentOperation(s, this);
        }
    }

    public partial class ParallelLineData
    {
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
            return new ParallelLineOperation(s, this);
        }
    }

    public partial class PathData
    {
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
            throw new NotImplementedException("PathData.LoadOperation");
            //return new PathOperation(s, this);
        }

        /// <summary>
        /// Converts XML data into the input for an editing operation.
        /// </summary>
        /// <param name="loader">Deserialization helper</param>
        /// <returns>The input for the editing operation</returns>
        /// <remarks>This should ultimately be declared as an abstract method</remarks>
        internal override OperationInput GetInput(ILoader loader)
        {
            PointFeature from = loader.Find<PointFeature>(this.From);
            PointFeature to = loader.Find<PointFeature>(this.To);
            return new PathInput(from, to, this.EntryString);
        }
    }

    public partial class PolygonSubdivisionData
    {
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
            return new PolygonSubdivisionOperation(s, this);
        }
    }

    public partial class PropertyChangeData
    {
        internal PropertyChangeData(PropertyChangeOperation op)
            : base(op)
        {
            this.Item = op.Item;
            this.Value = op.NewValue;
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new PropertyChangeOperation(s, this);
        }
    }

    public partial class RadialData
    {
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
            return new RadialOperation(s, this);
        }
    }

    public partial class SetTopologyData
    {
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
            return new SetTopologyOperation(s, this);
        }
    }

    public partial class SimpleLineSubdivisionData
    {
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
            return new SimpleLineSubdivisionOperation(s, this);
        }
    }

    public partial class TextRotationData
    {
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
            return new TextRotationOperation(s, this);
        }
    }

    public partial class TrimLineData
    {
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
            return new TrimLineOperation(s, this);
        }
    }

    public partial class UpdateData
    {
        /// <summary>
        /// Loads this update into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new UpdateOperation(s, this);
        }
    }
}
