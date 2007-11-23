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
using System.Text;

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
        static internal CheckType GetOption(char c)
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

        /// <summary>
        /// Gets a string that contains code letters for all defined checks. This
        /// is used when storing check options as part of the system registry.
        /// </summary>
        /// <returns>A concatenation of all defined code letters (each letter
        /// can be decoded using a call to <see cref="GetOption"/>)</returns>
        static internal string GetAllCheckLetters()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CheckTypeSmallLine);
            sb.Append(CheckTypeDangle);
            sb.Append(CheckTypeOverlap);
            sb.Append(CheckTypeFloating);
            sb.Append(CheckTypeBridge);
            sb.Append(CheckTypeSmallPolygon);
            sb.Append(CheckTypeNotEnclosed);
            sb.Append(CheckTypeNoLabel);
            sb.Append(CheckTypeNoPolygonForLabel);
            sb.Append(CheckTypeNoAttributes);
            sb.Append(CheckTypeMultiLabel);

            return sb.ToString();
        }

        /// <summary>
        /// Converts the supplied checks into a string of code letters
        /// </summary>
        /// <param name="checks">To check flags to convert</param>
        /// <returns>The equivalent code letters</returns>
        static internal string GetCheckLetters(CheckType checks)
        {
            StringBuilder sb = new StringBuilder();

            if ((checks & CheckType.SmallLine) != 0)
                sb.Append(CheckTypeSmallLine);

            if ((checks & CheckType.Dangle) != 0)
                sb.Append(CheckTypeDangle);

            if ((checks & CheckType.Overlap) != 0)
                sb.Append(CheckTypeOverlap);

            if ((checks & CheckType.Floating) != 0)
                sb.Append(CheckTypeFloating);

            if ((checks & CheckType.Bridge) != 0)
                sb.Append(CheckTypeBridge);

            if ((checks & CheckType.SmallPolygon) != 0)
                sb.Append(CheckTypeSmallPolygon);

            if ((checks & CheckType.NotEnclosed) != 0)
                sb.Append(CheckTypeNotEnclosed);

            if ((checks & CheckType.NoLabel) != 0)
                sb.Append(CheckTypeNoLabel);

            if ((checks & CheckType.NoPolygonForLabel) != 0)
                sb.Append(CheckTypeNoPolygonForLabel);

            if ((checks & CheckType.NoAttributes) != 0)
                sb.Append(CheckTypeNoAttributes);

            if ((checks & CheckType.MultiLabel) != 0)
                sb.Append(CheckTypeMultiLabel);

            return sb.ToString();
        }

        #endregion

        #region Class data

        /// <summary>
        /// What sort of problem(s) do we have?
        /// </summary>
        CheckType m_Types;

        /// <summary>
        /// Position of the reference point that was last defined via a call to <see cref="Render"/>,
        /// for subsequent use by <see cref="PaintOut"/>. For example, it's the position of the
        /// top-left corner of text at the time it was drawn. It gets stored here to cover the
        /// fact that the feature involved may get moved while the check dialog is still active
        /// (in a situation like that we need the original position to paint out check markers
        /// properly).
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

        /// <summary>
        /// Repaint icon(s) representing this check result.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        abstract internal void Render(ISpatialDisplay display, IDrawStyle style);

        /// <summary>
        /// Draws the checked item in a way that highlights it during a check review.
        /// This implementation does nothing. The <see cref="RingCheck"/> class
        /// overrides.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal virtual void HighlightCheckedItem(ISpatialDisplay display)
        {
        }

        /// <summary>
        /// Paints out those results that no longer apply.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        /// <param name="newTypes">The new results</param>
        abstract internal void PaintOut(ISpatialDisplay display, IDrawStyle style, CheckType newTypes);

        /// <summary>
        /// Determines whether a specific check needs to be painted out.
        /// </summary>
        /// <param name="flag">Flag bit indicating the check of interest</param>
        /// <param name="oldTypes">Original check flags</param>
        /// <param name="newTypes">Check flags after some sort of edit</param>
        /// <returns>True if the <paramref name="flag"/> was set in <paramref name="oldTypes"/>, and is now
        /// clear in <paramref name="newTypes"/></returns>
        protected bool IsPaintOut(CheckType flag, CheckType oldTypes, CheckType newTypes)
        {
            if ((oldTypes & flag)==0)
                return false;

            return ((newTypes & flag)==0);
        }

        /// <summary>
        /// Rechecks this result.
        /// </summary>
        /// <returns>The result(s) that still apply.</returns>
        abstract internal CheckType ReCheck();

        /// <summary>
        /// A textual explanation about this check result.
        /// </summary>
        abstract internal string Explanation { get; }

        /// <summary>
        /// Returns a display position for this check. This is the position to
        /// use for auto-centering the draw. It is not necessarily the same as
        /// the position of the problem icon.
        /// </summary>
        abstract internal IPosition Position { get; }

        /// <summary>
        /// Selects the object that this check relates to.
        /// </summary>
        abstract internal void Select();

        /// <summary>
        /// The size of icons used during check reviews, in meters on the ground.
        /// </summary>
        /// <param name="display">The display where the icons will be drawn</param>
        /// <returns>The size (height and width) of icons used as check markers</returns>
        protected double IconSize(ISpatialDisplay display)
        {
            return display.DisplayToLength(32.0F);
        }
    }
}
