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
using Backsight.Editor.Xml;

namespace Backsight.Editor.Observations
{
	/// <written by="Steve Stanton" on="13-NOV-1997" />
    /// <summary>
    /// An offset with respect to something else.
    /// </summary>
    abstract class Offset : Observation
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Offset() : base()
        {
        }

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="loader">Helper for load-related tasks</param>
        /// <param name="t">The serialized version of this observation</param>
        protected Offset(ILoader loader, OffsetData t)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="copy">The offset to copy</param>
        protected Offset(Offset copy) : base(copy)
        {
        }

        abstract internal double GetMetric (Direction dir);
        abstract internal PointFeature Point { get; set; }
        abstract internal void CutRef(Operation op);
    }
}
