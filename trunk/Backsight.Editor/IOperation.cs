// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <summary>
    /// An editing operation that can be serialized to the database as an XML fragment.
    /// You can safely cast an instance of this interface to an instance of the <see cref="Operation"/>
    /// class. The only reason for the interface is that it is utilized by xsd-generated classes that
    /// are created as public, but the <c>Operation</c> class is not public.
    /// </summary>
    public interface IOperation
    {
    }
}
