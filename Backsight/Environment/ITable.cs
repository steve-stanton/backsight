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

namespace Backsight.Environment
{
    /// <written by="Steve Stanton" on="14-APR-2008" />
    /// <summary>
    /// A database table that has been associated with the Backsight environment
    /// </summary>
    public interface ITable : IEnvironmentItem
    {
        /// <summary>
        /// The name of the database table (possibly decorated with schema name)
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// The name of the column that corresponds to the user-perceived ID
        /// associated with spatial features (this may or may not be the primary
        /// key of the table).
        /// </summary>
        string IdColumnName { get; }

        /// <summary>
        /// Any text formatting templates associated with the table (may be an empty array)
        /// </summary>
        ITemplate[] Templates { get; }

        /// <summary>
        /// Any domains associated with columns in the table
        /// </summary>
        IColumnDomain[] ColumnDomains { get; }
    }
}
