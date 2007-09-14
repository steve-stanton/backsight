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

namespace Backsight
{
	/// <written by="Steve Stanton" on="13-OCT-2006" />
    /// <summary>
    /// A mutable ground extent
    /// </summary>
    public interface IEditWindow : IWindow
    {
        void SetEmpty();
        void Union(IWindow other);
        void Union(double x, double y);
        void Union(IPosition p);
        void Expand(double factor);
        void Expand(ILength margin);
    }
}
