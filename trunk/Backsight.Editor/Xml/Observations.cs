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
            : base(loader)
        {
            //return new AngleDirection(loader, this);
            double observation;
            if (!RadianValue.TryParse(this.Value, out observation))
                throw new Exception("AngleData - Cannot parse 'Value' attribute");

            PointFeature backsight = loader.Find<PointFeature>(this.Backsight);
            PointFeature from = loader.Find<PointFeature>(this.From);

            //return 
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
            return new BearingDirection(loader, this);
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
            return new DeflectionDirection(loader, this);
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

        /// <summary>
        /// Deserializes an observation
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <returns>The observation obtained from this data</returns>
        internal override Observation LoadObservation(ILoader loader)
            : base(loader)
        {
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
            DistanceUnit enteredUnit = EditingController.Current.GetUnits(dut);
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
            return new ParallelDirection(loader, this);
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
            return new OffsetPoint(loader, this);
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
            return new OffsetDistance(loader, this);
        }
    }
}
