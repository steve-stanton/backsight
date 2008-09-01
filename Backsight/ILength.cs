// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.ComponentModel;

namespace Backsight
{
	/// <written by="Steve Stanton" on="28-NOV-2006" />
    /// <summary>
    /// The length of (could be position of) something on the ground. This
    /// could either be a real ground length, of a length on the mapping projection.
    /// </summary>
    [TypeConverter(typeof(StringConverter))]
    public interface ILength
    {
        /// <summary>
        /// The length in meters.
        /// </summary>
        double Meters { get; }

        /// <summary>
        /// The length in microns.
        /// </summary>
        /// <remarks>
        /// You can obviously obtain the same result by multiplying the <c>Meters</c>
        /// property by a million. This property is provided because Backsight tends to
        /// store data in microns.</remarks>
        long Microns { get; }
    }
}
