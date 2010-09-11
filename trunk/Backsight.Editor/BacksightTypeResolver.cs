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
using System.Web.Script.Serialization;
using System.Collections.Generic;

using Backsight.Editor.Xml;


namespace Backsight.Editor
{
    /// <summary>
    /// Custom type resolver for working with objects that are serialized
    /// as JSON-style strings. Only suitable for classes that are part of
    /// the <c>Backsight.Editor</c> project.
    /// </summary>
    class BacksightTypeResolver : JavaScriptTypeResolver
    {
        #region Class data

        /// <summary>
        /// Index of the classes in this assembly, keyed by short name. Excludes
        /// abstract classes, interfaces, as well as miscellaneous mystery classes
        /// that seem to be produced by NET (with type names that start with the
        /// "&lt;" character).
        /// </summary>
        readonly Dictionary<string, Type> m_TypeIndex;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BacksightTypeResolver"/> class.
        /// </summary>
        internal BacksightTypeResolver()
        {
            m_TypeIndex = DataFactory.GetTypeIndex();
        }

        #endregion

        /// <summary>
        /// The <see cref="T:System.Type"/> object that is associated with the specified type name.
        /// </summary>
        /// <param name="id">The name of the managed type.</param>
        /// <returns>
        /// The <see cref="T:System.Type"/> object that is associated with the specified type name.
        /// </returns>
        public override Type ResolveType(string id)
        {
            return m_TypeIndex[id];
        }

        /// <summary>
        /// The type name for the specified <see cref="T:System.Type"/> object.
        /// </summary>
        /// <param name="type">The managed type to be resolved.</param>
        /// <returns>The name of the specified managed type.</returns>
        public override string ResolveTypeId(Type type)
        {
            return type.Name;
        }
    }
}
