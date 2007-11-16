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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="18-NOV-1999" was="CeCheck" />
    /// <summary>
    /// The result of some sort of check. This is the base class for
    /// <see cref="DividerCheck"/>, <see cref="TextCheck"/>, and <see cref="PolygonCheck"/>
    /// </summary>
    abstract class CheckItem
    {
        #region Constants

        // Code letters that correspond to the flag bits defined in the <c>CheckType</c> enum.
        // These are used when saving options to the registry.

        /// <summary>
        /// Undefined code
        /// </summary>
        const char CheckTypeNull = ' ';

        /// <summary>
        /// Very small line
        /// </summary>
        const char CheckTypeSmallLine = 's';

        /// <summary>
        /// Line is dangling
        /// </summary>
        const char CheckTypeDangle = 'd';

        /// <summary>
        /// Line overlap
        /// </summary>
        const char CheckTypeOverlap = 'o';

        /// <summary>
        /// Line is floating in space (same polygon on both sides, end points un-connected)
        /// </summary>
        const char CheckTypeFloating = 'f';

        /// <summary>
        /// Bridging line (same polygon on both sides)
        /// </summary>
        const char CheckTypeBridge = 'b';

        /// <summary>
        /// Very small polygon
        /// </summary>
        const char CheckTypeSmallPolygon = 't';

        /// <summary>
        /// Island ring is not enclosed by any polygon
        /// </summary>
        const char CheckTypeNotEnclosed = 'e';

        /// <summary>
        /// Polygon has no label
        /// </summary>
        const char CheckTypeNoLabel = 'l';

        /// <summary>
        /// Label has no enclosing polygon
        /// </summary>
		const char CheckTypeNoPolygonForLabel = 'p';

        /// <summary>
        /// No attributes associated with polygon label
        /// </summary>
        const char CheckTypeNoAttributes = 'a';

        /// <summary>
        /// More than one label inside a single polygon
        /// </summary>
        const char CheckTypeMultiLabel = 'm';

        #endregion

        #region Static

        /// <summary>
        /// Returns a bit mask of options corresponding to a string.
        /// </summary>
        /// <param name="str">The string to decode (usually obtained from the registry).</param>
        /// <returns>The checks that correspond to the code letters in the supplied string</returns>
        static internal CheckType GetOptions(string str)
        {
            CheckType mask = 0;

            foreach (char c in str)
                mask |= GetOption(c);

            return mask;
        }

        /// <summary>
        /// Return the bit mask that corresponds to the supplied code letter.
        /// </summary>
        /// <param name="c">The letter to decode</param>
        /// <returns>The corresponding flag bit</returns>
        static CheckType GetOption(char c)
        {
            if (c==CheckTypeSmallLine)
                return CheckType.SmallLine;

            if (c==CheckTypeDangle)
                return CheckType.Dangle;

            if (c==CheckTypeOverlap)
                return CheckType.Overlap;

            if (c==CheckTypeFloating)
                return CheckType.Floating;

            if (c==CheckTypeBridge)
                return CheckType.Bridge;

            if (c==CheckTypeSmallPolygon)
                return CheckType.SmallPolygon;

            if (c==CheckTypeNotEnclosed)
                return CheckType.NotEnclosed;

            if (c==CheckTypeNoLabel)
                return CheckType.NoLabel;

            if (c==CheckTypeNoPolygonForLabel)
                return CheckType.NoPolygonForLabel;

            if (c==CheckTypeNoAttributes)
                return CheckType.NoAttributes;

            if (c==CheckTypeMultiLabel)
                return CheckType.MultiLabel;

            return 0;
        }

        #endregion

        #region Class data

        /// <summary>
        /// What sort of problem(s) do we have?
        /// </summary>
        CheckType m_Types;

        /// <summary>
        /// Position of the reference point that was last defined via a call to <c>Paint</c>
        /// </summary>
        IPosition m_Place;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>CheckItem</c> that refers to the specified check type(s).
        /// </summary>
        /// <param name="types">The type(s) of check this item corresponds to</param>
        internal CheckItem(CheckType types)
        {
            m_Types = types;
            m_Place = null;
        }

        #endregion

        /// <summary>
        /// The type(s) of check this item corresponds to
        /// </summary>
        internal CheckType Types
        {
            get { return m_Types; }
            set { m_Types = value; }
        }

        /// <summary>
        /// Position of a reference point for this check (any sort of significant position).
        /// May be null.
        /// </summary>
        protected IPosition Place
        {
            get { return m_Place; }
            set { m_Place = value; }
        }

        abstract internal void Paint();
        abstract internal void PaintOut(CheckType newTypes);
        abstract internal CheckType ReCheck();
        abstract internal string Explanation { get; }
        abstract internal IPosition Position { get; }
        abstract internal void Select();
    }
}
