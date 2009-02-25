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
    /// Some sort of content that cannot be directly stored in the database. Instead,
    /// an alternative content class must be used.
    /// </summary>
    public interface IXmlAlternateContent : IXmlContent
    {
        /// <summary>
        /// Obtains an instance of the content object that can be persisted in the
        /// database. On deserialization, the alternate will usually need to be
        /// converted into an instance of the original class (the specifics will
        /// vary from one class to the next).
        /// </summary>
        /// <returns>The content to save to the database</returns>
        IXmlContent GetAlternate();
    }
}
