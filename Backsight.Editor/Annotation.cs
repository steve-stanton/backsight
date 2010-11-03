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
using System.Drawing;

namespace Backsight.Editor
{
    /// <summary>
    /// Annotation (that appears alongside a line).
    /// </summary>
    class Annotation : IString
    {
        #region Static

        /// <summary>
        /// The text alignment for all annotations
        /// </summary>
        static StringFormat s_AnnotationFormat;

        static Annotation()
        {
            s_AnnotationFormat = new StringFormat();
            s_AnnotationFormat.Alignment = StringAlignment.Center;
            s_AnnotationFormat.LineAlignment = StringAlignment.Far; // bottom            
        }

        #endregion

        #region Class data

        /// <summary>
        /// The annotation text.
        /// </summary>
        readonly string m_Text;

        /// <summary>
        /// The position for the text (center-baseline aligned).
        /// </summary>
        readonly IPointGeometry m_Position;

        /// <summary>
        /// The height of the text (in meters on the ground).
        /// </summary>
        readonly double m_Height;

        /// <summary>
        /// The rotation (clockwise from horizontal).
        /// </summary>
        readonly IAngle m_Rotation;

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
            m_Position = PointGeometry.Create(position);
            m_Height = height;
            m_Rotation = new RadianValue(rotation);
        }

        #endregion

        /// <summary>
        /// The annotation text.
        /// </summary>
        public string Text
        {
            get { return m_Text; }
        }

        /// <summary>
        /// The position for the text (center-baseline aligned).
        /// </summary>
        public IPointGeometry Position
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
        /// The rotation (clockwise from horizontal).
        /// </summary>
        public IAngle Rotation
        {
            get { return m_Rotation; }
        }

        /// <summary>
        /// A closed outline that surrounds the string (could be null if the implementing
        /// class doesn't care).
        /// </summary>
        /// <value>Null (always)</value>
        public IPosition[] Outline
        {
            get { return null; }
        }

        /// <summary>
        /// Creates the font used to present the string.
        /// </summary>
        /// <param name="display">The display on which the string will be displayed</param>
        /// <returns>
        /// The corresponding font (may be null if the font is too small to be drawn)
        /// </returns>
        public Font CreateFont(ISpatialDisplay display)
        {
            float heightInPixels = display.LengthToDisplay(m_Height);
            return new Font("Arial", heightInPixels, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Any special layout information for the string (used for specifying special
        /// text alignment options).
        /// </summary>
        public StringFormat Format
        {
            get { return s_AnnotationFormat; }
        }
    }
}
