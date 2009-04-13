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

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// An editing action that is being recalled using the Cadastral Editor's
    /// <c>Edit - Recall</c> command
    /// </summary>
    class RecalledEditingAction : EditingAction
    {
        #region Class data

        /// <summary>
        /// The edit that is currently being recalled (if any)
        /// </summary>
        readonly Operation m_Recall;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecalledEditingAction"/> class.
        /// </summary>
        /// <param name="action">The editing action</param>
        /// <param name="recall">The editing operation that is being recalled</param>
        internal RecalledEditingAction(EditingAction action, Operation recall)
            : base(action)
        {
            m_Recall = op;
        }

        #endregion

    }
}
