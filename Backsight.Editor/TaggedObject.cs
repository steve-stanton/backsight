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

namespace Backsight.Editor
{
    /// <summary>
    /// Some sort of object that is tagged with something
    /// </summary>
    /// <typeparam name="S">The type of the object</typeparam>
    /// <typeparam name="T">The type for the tag</typeparam>
    class TaggedObject<S,T>
    {
        #region Class data

        readonly S m_Thing;
        readonly T m_Tag;

        #endregion

        #region Constructors

        internal TaggedObject(S thing, T tag)
        {
            m_Thing = thing;
            m_Tag = tag;
        }

        #endregion

        internal T Tag
        {
            get { return m_Tag; }
        }

        internal S Thing
        {
            get { return m_Thing; }
        }
    }
}
