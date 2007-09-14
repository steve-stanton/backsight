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

namespace Ntx
{
    /// <summary>
    /// An NTX point symbol (code 8).
    /// </summary>
    public class Symbol : Feature
    {
        #region Class data

        /// <summary>
        /// Location of symbol (may be null)
        /// </summary>
        Position m_Position;

        /// <summary>
        /// Is symbol an explicit node?
        /// </summary>
        bool m_IsNode;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Symbol</c> with an undefined (null) position,
        /// flagged as a plain symbol (not an explicit node).
        /// </summary>
        internal Symbol()
        {
            m_Position = null;
            m_IsNode = false;
        }

        #endregion

        /// <summary>
        /// The position of this symbol (not necessarily the visual center
        /// of the symbol). May be null.
        /// </summary>
        public Position Position
        {
            get { return m_Position; }
            internal set { m_Position = value; }
        }

        /// <summary>
        /// Does this symbol act as an explicit topological node?
        /// </summary>
        internal bool IsNode
        {
            get { return m_IsNode; }
            set { m_IsNode = value; }
        }

    }
}
