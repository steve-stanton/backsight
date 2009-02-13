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
    /// <written by="Steve Stanton" on="13-FEB-2009" />
    /// <summary>
    /// A domain where the values are recorded in some database table.
    /// </summary>
    /// <remarks>In the long run, it is possible that there will be different types
    /// of domains. For example, the old CEdit implementation permitted domains that
    /// were defined as a numeric range.</remarks>
    public interface IDomainTable : IEnvironmentItem
    {
        /// <summary>
        /// The name of the database table that holds the domain values (may be
        /// prepended with schema name).
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// The name of the database column that holds the lookup value for the
        /// domain. This will most likely be the primary key of the domain table.
        /// The default name used by Backsight is "ShortValue".
        /// </summary>
        string LookupColumnName { get; }

        /// <summary>
        /// The name of the database column (if any) that holds an expanded version
        /// of the lookup value. A blank value indicates that there is no expanded value. 
        /// The default name used by Backsight is "LongValue".
        /// </summary>
        string ValueColumnName { get; }
    }
}
