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

namespace Backsight.Environment
{
    /// <written by="Steve Stanton" on="29-APR-2008"/>
    /// <summary>
    /// A mutable version of <c>INamespace</c>
    /// </summary>
    public interface IEditNamespace : INamespace, IEditControl
    {
        /// <summary>
        /// The name for the namespace (e.g. "Vancouver")
        /// </summary>
        new string Name { get; set; }

        /// <summary>
        /// The namespace (if any) that encloses this one. Null if this namespace is
        /// at the root of a tree.
        /// </summary>
        new INamespace Parent { get; set; }
    }
}
