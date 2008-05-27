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
using System.Reflection;

namespace Backsight
{
    /// <summary>
    /// Base class for XML persistence classes.
    /// </summary>
    abstract public class XmlBase
    {
        #region Class data

        /// <summary>
        /// Cross-reference of type names to the corresponding constructor.
        /// </summary>
        readonly Dictionary<string, ConstructorInfo> m_Types;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>XmlBase</c>
        /// </summary>
        protected XmlBase()
        {
            m_Types = new Dictionary<string, ConstructorInfo>();
        }

        #endregion
    }
}
