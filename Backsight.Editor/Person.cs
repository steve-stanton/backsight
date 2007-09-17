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

namespace Backsight.Editor
{
    /// <summary>
    ///  A specific user of the Backsight system
    /// </summary>
    [Serializable]
    class Person
    {
        #region Statics

        /// <summary>
        /// Attempts to locate the current user in a list of people
        /// </summary>
        /// <param name="people">The list to check</param>
        /// <returns>The person matching the current user (null if not found)</returns>
        public static Person FindCurrentUser(List<Person> people)
        {
            return FindCurrentUser(Person.CurrentUserName, people);
        }

        internal static Person FindCurrentUser(string name, List<Person> people)
        {
            return people.Find(delegate(Person p) { return String.Compare(p.Name, name, true)==0; });
        }

        internal static string CurrentUserName
        {
            get { return System.Environment.UserName; }
        }

        #endregion

        #region Class data

        private string m_Name;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a person corresponding to the user who is currently logged on
        /// </summary>
        internal Person()
        {
            m_Name = Person.CurrentUserName;
        }

        internal Person(string name)
        {
            m_Name = name;
        }

        #endregion

        public override string ToString()
        {
            return m_Name;
        }

        internal string Name
        {
            get { return m_Name; }
        }
    }
}
