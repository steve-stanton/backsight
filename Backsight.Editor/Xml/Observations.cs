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
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal virtual Observation LoadObservation(Operation op)
        {
            throw new NotImplementedException("LoadObservation not implemented by: " + GetType().Name);
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
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal override Observation LoadObservation(Operation op)
        {
            return new Distance(op, this);
        }
    }

    /// <summary>
    /// A serialized <see cref="AngleDirection"/>
    /// </summary>
    public partial class AngleData
    {
        /// <summary>
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal override Observation LoadObservation(Operation op)
        {
            return new AngleDirection(op, this);
        }
    }

    /// <summary>
    /// A serialized <see cref="DeflectionDirection"/>
    /// </summary>
    public partial class DeflectionData
    {
        /// <summary>
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal override Observation LoadObservation(Operation op)
        {
            return new DeflectionDirection(op, this);
        }
    }

    /// <summary>
    /// A serialized <see cref="BearingDirection"/>
    /// </summary>
    public partial class BearingData
    {
        /// <summary>
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal override Observation LoadObservation(Operation op)
        {
            return new BearingDirection(op, this);
        }
    }

    /// <summary>
    /// A serialized <see cref="ParallelDirection"/>
    /// </summary>
    public partial class ParallelData
    {
        /// <summary>
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal override Observation LoadObservation(Operation op)
        {
            return new ParallelDirection(op, this);
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
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal override Observation LoadObservation(Operation op)
        {
            return new OffsetPoint(op, this);
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
            Distance = new DistanceData(o.Offset);
            Left = !o.IsRight;
        }

        /// <summary>
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal override Observation LoadObservation(Operation op)
        {
            return new OffsetDistance(op, this);
        }
    }
}
