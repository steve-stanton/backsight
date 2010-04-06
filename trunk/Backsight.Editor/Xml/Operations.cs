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
            this.Point = new CalculatedFeatureData(op.NewPoint);
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
            return new ImportOperation(s, this);
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
            this.To = new CalculatedFeatureData(op.IntersectionPoint);

            if (op.CreatedDirectionLine != null)
                this.DirLine = new CalculatedFeatureData(op.CreatedDirectionLine);

            if (op.CreatedDistanceLine != null)
                this.DistLine = new CalculatedFeatureData(op.CreatedDistanceLine);
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
            this.To = new CalculatedFeatureData(op.IntersectionPoint);

            if (op.CreatedDirectionLine != null)
                this.DirLine = new CalculatedFeatureData(op.CreatedDirectionLine);

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
            this.To = new CalculatedFeatureData(op.IntersectionPoint);

            if (op.CreatedLine1 != null)
                this.Line1 = new CalculatedFeatureData(op.CreatedLine1);

            if (op.CreatedLine2 != null)
                this.Line2 = new CalculatedFeatureData(op.CreatedLine2);
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
            this.To = new CalculatedFeatureData(op.IntersectionPoint);
            this.Default = op.IsDefault;

            if (op.CreatedLine1 != null)
                this.Line1 = new CalculatedFeatureData(op.CreatedLine1);

            if (op.CreatedLine2 != null)
                this.Line2 = new CalculatedFeatureData(op.CreatedLine2);
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
            this.To = new CalculatedFeatureData(op.IntersectionPoint);

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
            this.NewPoint = new CalculatedFeatureData(op.NewPoint);

            if (op.NewLine != null)
                this.NewLine = new CalculatedFeatureData(op.NewLine);
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
            MeasuredLineFeature[] sections = op.Sections;
            this.Span = new SpanData[sections.Length];

            for (int i = 0; i < sections.Length; i++)
            {
                MeasuredLineFeature mf = sections[i];
                SpanData st = new SpanData();
                st.Length = new DistanceData(mf.ObservedLength);
                st.LineId = mf.Line.DataId;

                if (i < (sections.Length - 1))
                    st.EndPoint = new CalculatedFeatureData(mf.Line.EndPoint);

                this.Span[i] = st;
            }
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
            return new NewCircleOperation(s, this);
        }
    }

    public partial class NewKeyTextData
    {
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
        //internal NewTextData(NewTextOperation op)
        //    : base(op)
        //{
        //    this.Text = new TextData();
        //}

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
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new ParallelOperation(s, this);
        }
    }

    public partial class PathData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new PathOperation(s, this);
        }
    }

    public partial class PolygonSubdivisionData
    {
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
