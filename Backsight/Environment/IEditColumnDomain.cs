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

namespace Backsight.Environment
{
    /// <written by="Steve Stanton" on="17-FEB-2009" />
    /// <summary>
    /// A mutable version of <see cref="IColumnDomain"/>
    /// </summary>
    public interface IEditColumnDomain : IColumnDomain, IEditControl
    {
        /// <summary>
        /// The name of the database column that the domain is associated with
        /// </summary>
        new string ColumnName { get; set; }

        /// <summary>
        /// The associated domain
        /// </summary>
        new IDomainTable Domain { get; set; }
    }
}
