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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="26-MAR-1998" />
    /// <summary>
    /// An association between a string representing the name of something, and an
    ///	associated object. This is used during data imports to relate cadastral editor
    ///	objects to their corresponding external names (e.g. feature codes are associated
    ///	with <c>IEntity</c> objects).
    /// </summary>
    class Translation<T>
    {
        #region Class data

        /// <summary>
        /// The input string, as it was specifed.
        /// </summary>
        readonly string m_Input;

        /// <summary>
        /// External name for the object (possibly containing wildcards).
        /// </summary>
        readonly Wildcard m_Name;

        /// <summary>
        /// The associated object (may be null).
        /// </summary>
        readonly T m_Object;
        
        /// <summary>
        /// Number associated with the translation (0 means it was user-specified, while
        /// numbers greater than 0 refer to the number of translation files that were used).
        /// </summary>
        int m_FileNum;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new user-specified <c>Translation</c> (where the <c>FileNum</c>
        /// property is zero).
        /// </summary>
        /// <param name="input">External name for the object</param>
        /// <param name="output">The associated object</param>
        internal Translation(string input, T output)
            :this(input, output, 0)
        {
        }

        /// <summary>
        /// Creates a new <c>Translation</c>
        /// </summary>
        /// <param name="input">External name for the object</param>
        /// <param name="output">The associated object (may be null)</param>
        /// <param name="fileNum">
        /// Number associated with the translation (0 means it was user-specified, while
        /// numbers greater than 0 refer to the number of translation files that were used).
        /// </param>
        internal Translation(string input, T output, int fileNum)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentNullException();

            m_Input = input;
            m_Name = new Wildcard(input);
            m_Object = output;
            m_FileNum = fileNum;
        }

        #endregion

        /// <summary>
        /// Number associated with the translation (0 means it was user-specified, while
        /// numbers greater than 0 refer to the number of translation files that were used).
        /// </summary>
        internal int FileNum
        {
            get { return m_FileNum; }
            set { m_FileNum = value; }
        }

        /// <summary>
        /// The input for the translation (the name of the thing that needs to be translated).
        /// May contain wildcards.
        /// </summary>
        internal string Input
        {
            get { return m_Input; }

        }
        /// <summary>
        /// The result of the translation (may be null).
        /// </summary>
        internal T Output
        {
            get { return m_Object; }
        }

        /// <summary>
        /// Checks if a string representing the name of some external thing is
        /// a match with this object.
        /// </summary>
        /// <param name="str">The external name.</param>
        /// <returns>True if the external name matches. In that case, you can
        /// use the <c>Output</c> property to retrieve the associated
        /// cadastral editor object.</returns>
        internal bool IsMatch(string str)
        {
            return m_Name.IsMatch(str);
        }
    }
}
