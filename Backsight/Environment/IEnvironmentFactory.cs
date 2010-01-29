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

namespace Backsight.Environment
{
	/// <written by="Steve Stanton" on="08-MAR-2007" />
    /// <summary>
    /// A factory for creating new items in a Backsight environment. All <c>Create</c>
    /// methods are expected to assign a unique ID to new items, but with default
    /// values for everything else. On creation, items do not get added to any
    /// container (to avoid constraint violations, there's a good chance the
    /// content of the row will need to be defined first).
    /// </summary>
    public interface IEnvironmentFactory
    {
        /// <summary>
        /// Creates an association between a database column and a domain
        /// </summary>
        /// <returns>The newly created column-domain association</returns>
        IEditColumnDomain CreateColumnDomain();

        /// <summary>
        /// Creates the definition for a new domain table
        /// </summary>
        /// <returns>The newly created domain table</returns>
        IEditDomainTable CreateDomainTable();

        /// <summary>
        /// Creates a new entity type
        /// </summary>
        /// <returns>The newly created entity type</returns>
        IEditEntity CreateEntity();

        /// <summary>
        /// Creates a new font definition
        /// </summary>
        /// <returns>The newly created font definition</returns>
        IEditFont CreateFont();

        /// <summary>
        /// Creates a new ID group
        /// </summary>
        /// <returns>The newly created ID group</returns>
        IEditIdGroup CreateIdGroup();

        /// <summary>
        /// Creates a miscellaneous property
        /// </summary>
        /// <returns>The newly created property</returns>
        IEditProperty CreateProperty();

        /// <summary>
        /// Creates a new association with a database table
        /// </summary>
        /// <returns>The newly created table association</returns>
        IEditTable CreateTableAssociation();

        /// <summary>
        /// Creates a new text formatting template
        /// </summary>
        /// <returns>The newly created text template</returns>
        IEditTemplate CreateTemplate();

        /// <summary>
        /// Creates a new theme
        /// </summary>
        /// <returns>The newly created theme</returns>
        IEditTheme CreateTheme();

        /// <summary>
        /// Creates a new map layer
        /// </summary>
        /// <returns>The newly created map layer</returns>
        IEditLayer CreateLayer();

        /// <summary>
        /// Creates a new zone
        /// </summary>
        /// <returns>The newly created zone</returns>
        IEditZone CreateZone();
    }
}
