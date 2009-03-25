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

using Backsight.Editor.Operations;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Base class for any sort of serialized editing operation.
    /// </summary>
    /// <remarks>The remainder of this class is auto-generated, and may be found
    /// in the <c>Edits.cs</c> file.</remarks>
    abstract public partial class OperationType
    {
        public OperationType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationType"/> class.
        /// </summary>
        /// <param name="op">The editing operation that needs to be serialized.</param>
        /// <remarks>Should really be protected (since this is an abstract base class), but
        /// that leads to compiler error because this class is public, while <c>Operation</c>
        /// is internal.</remarks>
        internal OperationType(Operation op)
        {
            this.Id = op.DataId;
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        abstract internal Operation LoadOperation(Session s);
    }
}
