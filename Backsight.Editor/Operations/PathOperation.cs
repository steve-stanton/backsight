/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="24-JAN-1998" was="CePath" />
    /// <summary>
    /// A connection path between two points. Like a traverse.
    /// </summary>
    [Serializable]
    class PathOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The legs that make up the path
        /// </summary>
        List<Leg> m_Legs;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PathOperation</c> with everything set to null.
        /// </summary>
        internal PathOperation()
        {
        }

        #endregion

        public override string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override Distance GetDistance(LineFeature line)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override Feature[] Features
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override EditingActionId EditId
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override void AddReferences()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool Undo()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool Rollforward()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
