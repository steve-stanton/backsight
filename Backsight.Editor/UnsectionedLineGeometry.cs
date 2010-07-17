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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="02-NOV-2007" />
    /// <summary>
    /// Line geometry that <b>cannot</b> act as the base for <see cref="SectionGeometry"/>
    /// </summary>
    abstract class UnsectionedLineGeometry : LineGeometry
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>UnsectionedLineGeometry</c> using the supplied terminals.
        /// </summary>
        /// <param name="start">The start of the line.</param>
        /// <param name="end">The end of the line.</param>
        protected UnsectionedLineGeometry(ITerminal start, ITerminal end)
            : base(start, end)
        {
        }

        #endregion

        /// <summary>
        /// The geometry that acts as the base for this one is <c>this</c>
        /// </summary>
        internal override UnsectionedLineGeometry SectionBase
        {
            get { return this; }
        }

        /// <summary>
        /// The line geometry that corresponds to a section of a line.
        /// </summary>
        /// <param name="s">The required section</param>
        /// <returns>The corresponding geometry for the section</returns>
        abstract internal UnsectionedLineGeometry Section(ISection s);
    }
}
