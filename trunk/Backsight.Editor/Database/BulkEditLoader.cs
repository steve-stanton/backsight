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
using System.Collections.Generic;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Bulk loading of data stored in the <c>Edits</c> table. This will be used whenever
    /// the Cadastral Editor starts up.
    /// </summary>
    class BulkEditLoader
    {
        #region Class data
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>BulkEditLoader</c>. This creates a (de)serializer for all concrete
        /// classes derived from the <see cref="Operation"/> class.
        /// </summary>
        internal BulkEditLoader()
        {
        }

        #endregion

    }
}