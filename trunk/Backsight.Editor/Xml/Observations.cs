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
}
