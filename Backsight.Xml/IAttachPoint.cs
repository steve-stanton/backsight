/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;

namespace Backsight.Xml
{
    /// <summary>
    /// The information needed to create an instance of <see cref="AttachPointData"/>.
    /// </summary>
    public interface IAttachPoint
    {
        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        Guid Line { get; }

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        uint PositionRatio { get; }

        /// <summary>
        /// The point that was created (without any geometry)
        /// </summary>
        FeatureData Point { get; }
    }
}
