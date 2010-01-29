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

namespace Backsight.Environment
{
	/// <written by="Steve Stanton" on="08-MAR-2007" />
    /// <summary>
    /// Something that holds information relating to the Backsight operating environment.
    /// </summary>
    public interface IEnvironmentContainer
    {
        /// <summary>
        /// Loads this container with environment-related data.
        /// </summary>
        void Read();

        /// <summary>
        /// Saves the content of this container.
        /// </summary>
        void Write();

        /// <summary>
        /// Reserves the next available ID
        /// </summary>
        /// <returns>The reserved ID</returns>
        int ReserveId();

        /// <summary>
        /// Attempts to release an ID that was previously obtained using <c>ReserveId</c>.
        /// Calling this method is not really necessary if you don't mind gaps in the
        /// sequence of used IDs.
        /// </summary>
        /// <param name="id">The ID to release</param>
        /// <returns>True if the ID was released. False if it doesn't match the ID
        /// returned by the last system-wide call to <c>ReserveId</c></returns>
        bool ReleaseId(int id);

        /// <summary>
        /// Factory for creating new items in this container.
        /// </summary>
        IEnvironmentFactory Factory { get; }

        /// <summary>
        /// Has the content of this container been modified since it was loaded?
        /// </summary>
        bool IsModified { get; }

        /// <summary>
        /// Is this container empty?
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// A name for this container (blank if the container has never been saved)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// All defined domain tables
        /// </summary>
        IDomainTable[] DomainTables { get; }

        /// <summary>
        /// All defined entity types
        /// </summary>
        IEntity[] EntityTypes { get; }

        /// <summary>
        /// All defined font descriptions
        /// </summary>
        IFont[] Fonts { get; }

        /// <summary>
        /// All defined ID groups
        /// </summary>
        IIdGroup[] IdGroups { get; }

        /// <summary>
        /// All defined map layers
        /// </summary>
        ILayer[] Layers { get; }

        /// <summary>
        /// All defined map themes
        /// </summary>
        ITheme[] Themes { get; }

        /// <summary>
        /// All defined Backsight properties
        /// </summary>
        IProperty[] Properties { get; }

        /// <summary>
        /// All database tables associated with the Backsight environment
        /// </summary>
        ITable[] Tables { get; }

        /// <summary>
        /// All defined text formatting templates
        /// </summary>
        ITemplate[] Templates { get; }

        /// <summary>
        /// All defined spatial zones
        /// </summary>
        IZone[] Zones { get; }
    }
}
