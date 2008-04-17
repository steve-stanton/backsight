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

namespace Backsight.Environment
{
	/// <written by="Steve Stanton" on="12-MAR-2007" />
    /// <summary>
    /// The real-world meaning of some sort of spatial feature (e.g. road, river).
    /// Frequently referred to as an "entity type". Each feature must be associated with
    /// an entity type, indicating what the feature is.
    /// <para>
    /// This is essentially an attribute, so by rights it should really be part of some
    /// attribute structure that is related to each feature. It's noted as part of the
    /// feature only for convenience (it allows you to quickly say what sort of thing
    /// you're dealing with, without having to be overly concerned with the details).
    /// </para>
    /// </summary>
    public interface IEntity : IEnvironmentItem
    {
        /// <summary>
        /// The user-perceived name for the entity type - what it signifies in the
        /// real world (e.g. "Road", "River")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Can this entity type be associated with a point feature?
        /// </summary>
        bool IsPointValid { get; }

        /// <summary>
        /// Can this entity type be associated with a line feature?
        /// </summary>
        bool IsLineValid { get; }

        /// <summary>
        /// Can this entity type be associated with a polygon label?
        /// </summary>
        bool IsPolygonValid { get; }

        bool IsPolygonBoundaryValid { get; }

        /// <summary>
        /// Can this entity type be associated with miscellaneous text?
        /// Note that while a polygon label is represented by an item of text, it is
        /// <b>not</b> regarded as "miscellaneous".
        /// </summary>
        bool IsTextValid { get; }

        /// <summary>
        /// Checks whether this entity type can be associated with the supplied spatial data type.
        /// This should end up calling an implementation of an <c>Is*Valid</c> property.
        /// </summary>
        /// <param name="t">The type of data to check (could conceivably be a combination
        /// of types)</param>
        /// <returns>True if this entity type can be associated with the spatial data type</returns>
        bool IsValid(SpatialType t);

        /// <summary>
        /// The table(s) that are usually associated with this entity type.
        /// </summary>
        ITable[] DefaultTables { get; }

        IIdGroup IdGroup { get; }
        ILayer Layer { get; }
        IFont Font { get; }
    }
}
