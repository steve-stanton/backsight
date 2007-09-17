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

using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="13-JUN-2007" />
    /// <summary>
    /// Exception indicating that an editing operation failed to rollforward. This type
    /// of exception should be raised only by implementations of the <c>Operation.Rollforward</c>
    /// method. A <c>RollforwardException</c> is not meant to indicate an application error;
    /// it refers to a problem that the user could rectify by altering the observations
    /// supplied as part of a map update.
    /// </summary>
    class RollforwardException : Exception
    {
        #region Class data

        /// <summary>
        /// The operation that failed to rollforward
        /// </summary>
        readonly Operation m_Problem;

        #endregion

        #region Constructors

        internal RollforwardException(Operation op, string msg)
            : base(msg)
        {
            m_Problem = op;
        }

        #endregion

        /// <summary>
        /// The operation that failed to rollforward
        /// </summary>
        internal Operation Problem
        {
            get { return m_Problem; }
        }
    }
}
