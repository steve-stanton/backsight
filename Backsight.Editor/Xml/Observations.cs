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
using Backsight.Editor.Observations;
using Backsight.Editor;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Base class for any sort of serialized <see cref="Observation"/>
    /// </summary>
    /// <remarks>The remainder of this class is auto-generated, and may be found
    /// in the <c>Edits.cs</c> file.</remarks>
    public partial class ObservationData
    {
        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal virtual Observation LoadObservation(ILoader loader)
        {
            throw new NotImplementedException("LoadObservation not implemented by: " + GetType().Name);
        }
    }

    /// <summary>
    /// A serialized <see cref="AngleDirection"/>
    /// </summary>
    public partial class AngleData
    {
        public AngleData()
        {
        }

        internal AngleData(AngleDirection d)
            : base(d)
        {
            this.Backsight = d.Backsight.DataId;
            this.From = d.From.DataId;
            this.Value = RadianValue.AsShortString(d.ObservationInRadians);
        }

        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal override Observation LoadObservation(ILoader loader)
        {
            AngleDirection result = new AngleDirection();
            LoadAngleDirection(loader, result);
            return result;
        }

        internal void LoadAngleDirection(ILoader loader, AngleDirection d)
        {
            base.LoadDirection(loader, d);

            double observation;
            if (!RadianValue.TryParse(this.Value, out observation))
                throw new Exception("AngleData - Cannot parse 'Value' attribute");

            d.Backsight = loader.Find<PointFeature>(this.Backsight);
            d.From = loader.Find<PointFeature>(this.From);
            d.SetObservationInRadians(observation);
        }
    }

    /// <summary>
    /// A serialized <see cref="BearingDirection"/>
    /// </summary>
    public partial class BearingData
    {
        public BearingData()
        {
        }

        internal BearingData(BearingDirection d)
            : base(d)
        {
            this.From = d.From.DataId;
            this.Value = RadianValue.AsShortString(d.ObservationInRadians);
        }

        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal override Observation LoadObservation(ILoader loader)
        {
            BearingDirection result = new BearingDirection();
            LoadBearingDirection(loader, result);
            return result;
        }

        void LoadBearingDirection(ILoader loader, BearingDirection d)
        {
            base.LoadDirection(loader, d);

            double observation;
            if (!RadianValue.TryParse(this.Value, out observation))
                throw new Exception("AngleData - Cannot parse 'Value' attribute");

            d.From = loader.Find<PointFeature>(this.From);
            d.SetObservationInRadians(observation);
        }
    }

    /// <summary>
    /// A serialized <see cref="DeflectionDirection"/>
    /// </summary>
    public partial class DeflectionData
    {
        public DeflectionData()
        {
        }

        internal DeflectionData(DeflectionDirection d)
            : base(d)
        {
            // nothing to do
        }

        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal override Observation LoadObservation(ILoader loader)
        {
            DeflectionDirection result = new DeflectionDirection();
            LoadAngleDirection(loader, result);
            return result;
        }
    }

    public partial class DirectionData
    {
        public DirectionData()
        {
        }

        // should be protected
        internal DirectionData(Direction d)
        {
            Offset o = d.Offset;
            if (o != null)
                this.Offset = DataFactory.Instance.ToData<OffsetData>(o);
        }

        internal void LoadDirection(ILoader loader, Direction d)
        {
            if (this.Offset != null)
                d.Offset = (Offset)this.Offset.LoadObservation(loader);
        }
    }

    /// <summary>
    /// A serialized <see cref="Distance"/>
    /// </summary>
    public partial class DistanceData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceData"/> class.
        /// </summary>
        /// <param name="d">The object to serialize</param>
        internal DistanceData(Distance d)
            : base()
        {
            this.Value = d.ObservedValue;
            this.Unit = (int)d.EntryUnit.UnitType;
            this.Fixed = d.IsFixed;
        }

        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal override Observation LoadObservation(ILoader loader)
        {
            DistanceUnitType dut = (DistanceUnitType)this.Unit;
            DistanceUnit enteredUnit = EditingController.GetUnits(dut);
            return new Distance(this.Value, enteredUnit, this.Fixed);
        }
    }

    /// <summary>
    /// A serialized <see cref="ParallelDirection"/>
    /// </summary>
    public partial class ParallelData
    {
        public ParallelData()
        {
        }

        internal ParallelData(ParallelDirection d)
            : base(d)
        {
            this.From = d.From.DataId;
            this.Start = d.Start.DataId;
            this.End = d.End.DataId;
        }

        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal override Observation LoadObservation(ILoader loader)
        {
            ParallelDirection result = new ParallelDirection();
            LoadParallelDirection(loader, result);
            return result;
        }

        void LoadParallelDirection(ILoader loader, ParallelDirection d)
        {
            base.LoadDirection(loader, d);

            d.From = loader.Find<PointFeature>(this.From);
            d.Start = loader.Find<PointFeature>(this.Start);
            d.End = loader.Find<PointFeature>(this.End);
        }
    }

    /// <summary>
    /// A serialized <see cref="OffsetPoint"/>
    /// </summary>
    public partial class OffsetPointData
    {
        public OffsetPointData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetPointData"/> class.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        internal OffsetPointData(OffsetPoint o)
            : base()
        {
            this.Point = o.Point.DataId;
        }

        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal override Observation LoadObservation(ILoader loader)
        {
            OffsetPoint result = new OffsetPoint();
            result.Point = loader.Find<PointFeature>(this.Point);
            return result;
        }
    }

    /// <summary>
    /// A serialized <see cref="OffsetDistance"/>
    /// </summary>
    public partial class OffsetDistanceData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetDistanceData"/> class.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        internal OffsetDistanceData(OffsetDistance o)
            : base()
        {
            this.Distance = new DistanceData(o.Offset);
            this.Left = !o.IsRight;
        }

        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal override Observation LoadObservation(ILoader loader)
        {
            OffsetDistance result = new OffsetDistance();
            result.Offset = (Distance)this.Distance.LoadObservation(loader);

            if (this.Left)
                result.SetLeft();
            else
                result.SetRight();

            return result;
        }
    }
}
