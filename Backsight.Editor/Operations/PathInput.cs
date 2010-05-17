// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// Input data for <see cref="PathOperation"/>
    /// </summary>
    class PathInput : OperationInput
    {
        #region Class data

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        internal readonly PointFeature From;

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        internal readonly PointFeature To;

        /// <summary>
        /// The data entry string that defines the connection path.
        /// </summary>
        internal readonly string EntryString;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathInput"/> class
        /// </summary>
        /// <param name="from">The point where the path starts.</param>
        /// <param name="to">The point where the path ends.</param>
        /// <param name="entryString">The data entry string that defines the connection path.</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        internal PathInput(PointFeature from, PointFeature to, string entryString)
        {
            if (from == null || to == null || entryString == null)
                throw new ArgumentNullException();

            From = from;
            To = to;
            EntryString = entryString;
        }

        #endregion
    }
}
