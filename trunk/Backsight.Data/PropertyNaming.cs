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

namespace Backsight.Data
{
    /// <written by="Steve Stanton" on="22-FEB-2008" />
    /// <summary>
    /// The names of standard Backsight properties (stored in the database <c>Property</c> table).
    /// </summary>
    public static class PropertyNaming
    {
        /// <summary>
        /// The name of the property that identifies the default style file
        /// </summary>
        /// <remarks>This was historically accessed through the environment variable called CED_STYLE_FILE</remarks>
        public static string StyleFile
        {
            get { return "StyleFile"; }
        }

        /// <summary>
        /// The name of the property that identifies the entity translation file
        /// </summary>
        /// <remarks>This was historically accessed through the environment variable called CED_ENTITY_FILE</remarks>
        public static string EntityFile
        {
            get { return "EntityFile"; }
        }

        /// <summary>
        /// The names of properties that should be defined in the database
        /// </summary>
        public static string[] MandatoryProperties
        {
            get { return new string[] { StyleFile, EntityFile }; }
        }
    }
}
