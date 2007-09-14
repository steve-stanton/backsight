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
        string Name { get; }
        bool IsPointValid { get; }
        bool IsLineValid { get; }
        bool IsPolygonValid { get; }
        bool IsPolygonBoundaryValid { get; }
        bool IsTextValid { get; }
        bool IsValid(SpatialType t);
        IIdGroup IdGroup { get; }
        ILayer Layer { get; }
    }
}
