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
using System.Drawing;

namespace Backsight.Environment
{
    /// <written by="Steve Stanton" on="09-APR-2008"/>
    /// <summary>
    /// A mutable version of <c>IFont</c>
    /// </summary>
    public interface IEditFont : IFont, IEditControl
    {
        /// <summary>
        /// The name of the font family (e.g. "Arial"). A null name is valid,
        /// and means that any system-defined font should be used.
        /// </summary>
        new string TypeFace { get; set; }

        /// <summary>
        /// The point-size of the font
        /// </summary>
        new float PointSize { get; set; }

        /// <summary>
        /// Flag bits defining font modifiers.
        /// </summary>
        new FontStyle Modifiers { get; set; }
    }
}
