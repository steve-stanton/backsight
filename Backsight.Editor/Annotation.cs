// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
    /// Annotation (that appears alongside a line).
    /// </summary>
    class Annotation
    {
        #region Data

        /// <summary>
        /// The annotation text.
        /// </summary>
        readonly string m_Text;

        /// <summary>
        /// The position for the text (center-baseline aligned).
        /// </summary>
        readonly IPosition m_Position;

        /// <summary>
        /// The height of the text (in meters on the ground).
        /// </summary>
        readonly double m_Height;

        /// <summary>
        /// The rotation (in radians clockwise from horizontal).
        /// </summary>
        readonly double m_Rotation;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation"/> class.
        /// </summary>
        /// <param name="text">The annotation text.</param>
        /// <param name="position">The position for the text (center-baseline aligned).</param>
        /// <param name="height">The height of the text (in meters on the ground).</param>
        /// <param name="rotation">The rotation (in radians clockwise from horizontal).</param>
        internal Annotation(string text, IPosition position, double height, double rotation)
        {
            m_Text = text;
            m_Position = position;
            m_Height = height;
            m_Rotation = rotation;
        }

        #endregion

        /// <summary>
        /// The annotation text.
        /// </summary>
        internal string Text
        {
            get { return m_Text; }
        }

        /// <summary>
        /// The position for the text (center-baseline aligned).
        /// </summary>
        internal IPosition Position
        {
            get { return m_Position; }
        }

        /// <summary>
        /// The height of the text (in meters on the ground).
        /// </summary>
        internal double Height
        {
            get { return m_Height; }
        }

        /// <summary>
        /// The rotation (in radians clockwise from horizontal).
        /// </summary>
        internal double Rotation
        {
            get { return m_Rotation; }
        }
    }
}
