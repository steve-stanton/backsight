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
    /// <written by="Steve Stanton" on="15-APR-2008" />
    /// <summary>
    /// A template describing how to form a text string out of information that appears in
    /// a database table.
    /// </summary>
    public interface ITemplate : IEnvironmentItem
    {
        /// <summary>
        /// The database table the template applies to (the table may be associated with
        /// several templates).
        /// </summary>
        ITable Schema { get; }

        /// <summary>
        /// Is this a new item (not yet saved in the database)?
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// A name for the template
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The format that defines the template
        /// </summary>
        string Format { get; }
    }
}
