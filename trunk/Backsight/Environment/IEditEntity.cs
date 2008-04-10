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
    /// A mutable version of <c>IEntity</c>
    /// </summary>
    public interface IEditEntity : IEntity, IEditControl
    {
        new string Name { get; set; }
        new bool IsPointValid { get; set; }
        new bool IsLineValid { get; set; }
        new bool IsPolygonValid { get; set; }
        new bool IsPolygonBoundaryValid { get; set; }
        new bool IsTextValid { get; set; }
        new IIdGroup IdGroup { get; set; }
        new ILayer Layer { get; set; }
        new IFont Font { get; set; }
    }
}
