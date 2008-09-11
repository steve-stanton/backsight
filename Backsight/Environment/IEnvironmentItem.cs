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

namespace Backsight.Environment
{
	/// <written by="Steve Stanton" on="08-MAR-2007" />
    /// <summary>
    /// An item of information that relates to the Backsight operating environment.
    /// </summary>
    public interface IEnvironmentItem : IExpandablePropertyItem
    {
        /// <summary>
        /// The item ID. No two items in an environment should have the same ID (with the
        /// exception of Id==0, which is intended to refer to various types of "empty" item).
        /// </summary>
        [Browsable(false)]
        int Id { get; }
    }
}
