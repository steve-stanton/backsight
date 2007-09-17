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
using System.Drawing;
using System.Windows.Forms;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="" />
    /// <summary>
    /// A text object is some sort of string that appears on a map. This is the base class for
    //	MiscText, KeyText, RowText, and FeatureText.
    /// </summary>
    [Serializable]
    abstract public class TextGeometry : IString
    {
        #region Class data

        /// <summary>
        /// The text style (defines the type-face and the height of the text). The enclosing
        /// CadastralMapModel object maintains a list of all the fonts that have been used
        /// by text that appears within the map.
        /// </summary>
        [NonSerialized] // for now
        //private ISimpleFont m_Font;

        /// <summary>
        /// The average width of characters in the text, in meters on the ground (this
        /// will be zero if there's only one character in the text -- should perhaps
        /// change this so that it's worked out once and for all).
        /// </summary>
        private float m_Width;
        
        /// <summary>
        /// The height of the text, in meters on the ground.
        /// </summary>
        private float m_Height;

        /// <summary>
        /// Clockwise rotation from horizontal
        /// </summary>
        private IAngle m_Rotation;

        /// <summary>
        /// Position of the text's reference point (always the top left corner of the string).
        /// </summary>
        private IPointGeometry m_Position;

        #endregion

        #region Constructors

        protected TextGeometry(IPointGeometry pos, float height, float spacing, float rotation)
        {
            m_Position = pos;
            m_Height = height;
            m_Width = spacing;
            m_Rotation = new RadianValue((double)rotation);
        }

        #endregion

        /// <summary>
        /// The text string represented by this geometry
        /// </summary>
        abstract public string Text { get; }

        /// <summary>
        /// Position of the text's reference point (always the top left corner of the string).
        /// </summary>
        public IPointGeometry Position
        {
            get { return m_Position; }
            internal set { m_Position = value; }
        }

        /// <summary>
        /// The spacing between each character in this text object (in meters on the ground)
        /// </summary>
        internal float Spacing
        {
            get { return m_Width; }
            set { m_Width = value; }            
        }

        /// <summary>
        /// Clockwise rotation from horizontal
        /// </summary>
        public IAngle Rotation
        {
            get { return m_Rotation; }
        }

        /// <summary>
        /// The height of the text, in meters on the ground. Corresponds to text
        /// em-size (the sum of the font ascent + descent).
        /// </summary>
        internal float Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        /// <summary>
        /// The text style
        /// </summary>
        /*
        internal ISimpleFont Font
        {

        }
        */

        public ILength Distance(IPosition point)
        {
            // If the test position falls anywhere inside the outline of the text,
            // treat it as a distance of zero.
            IPosition[] outline = this.Outline;
            if (Geom.IsOverlap(outline, point))
                return Length.Zero;

            double dsq = Geom.MinDistanceSquared(outline, point);
            return new Length(Math.Sqrt(dsq));
        }

        public IWindow Extent
        {
            get
            {
                Window result = new Window();
                IPosition[] outline = this.Outline;
                foreach (IPosition p in outline)
                {
                    result.Union(p);
                }
                return result;
            }
        }

        public IPosition[] Outline
        {
            get
            {
                // The following may not hack it... (the original implementation was rather more involved)
                string text = this.Text;
                double ght = (double)this.Height;
                double gwd;

                if (m_Width > Constants.TINY)
                {
                    gwd = (double)text.Length * (double)m_Width;
                }
                else
                {
                    // Use a font with an arbitrary height of 100 pixels to derive the
                    // width of the text on the ground

                    Font f = this.CreateFont(100, 0.0);
                    Size proposedSize = new Size(int.MaxValue, int.MaxValue);

                    // The TextFormatFlags don't seem to do anything...
                    Size size = TextRenderer.MeasureText(text, f, proposedSize,
                        (TextFormatFlags.NoPadding | TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix));

                    gwd = ght * ((double)size.Width / (double)size.Height);
                }

                double topToBottomBearing = this.VBearing + Constants.PI;

                IPosition[] result = new IPosition[5];
                result[0] = m_Position;
                result[1] = Geom.Polar(m_Position, this.HBearing, gwd);
                result[2] = Geom.Polar(result[1], topToBottomBearing, ght);
                result[3] = Geom.Polar(m_Position, topToBottomBearing, ght);
                result[4] = result[0];

                return result;
            }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, this);
            //style.Render(display, this.Outline); // debug
        }

        public Font CreateFont(ISpatialDisplay display)
        {
            double ght = (double)this.Height;
            float dht = display.LengthToDisplay(ght);
            int ht = (int)dht;
            if (ht<=0)
                return null;

            Font f = this.CreateFont(ht, 0.0);
            return f;
        }

        /// <summary>
        /// Defines some text that was created via an implementation of the <c>MakeText</c> function.
        /// </summary>
        /// <param name="newtext">The text to define.</param>
        /*
        protected void DefineText(TextGeometry newtext)
        {
	        // There's no need to touch the base class, since the
	        // default constructor defines everything we need.

            // Copy info for the text.
	        newtext.m_Font = m_Font;
	        newtext.m_Width = m_Width;
	        newtext.m_Height = m_Height;
	        newtext.m_Rotation = m_Rotation;

            // Only copy over the position if it has not been defined already.
            if (newtext.Position==null)
                newtext.Position = m_Position;
        }
         */

        /// <summary>
        /// Creates a font that has the characteristics of this text.
        /// </summary>
        /// <param name="heightInPixels">The height of the text, in pixels</param>
        /// <param name="extraRotation">Any additional rotation (clockwise, in radians). A
        /// non-zero value is used when doing rotated plots.</param>
        /// <returns>The matching font</returns>
        Font CreateFont(int heightInPixels, double extraRotation)
        {
            // Convert the rotation (if any) into units of 0.1 degrees. MFC
            // reckons angles anti-clockwise.
            //int rotation = -(int)((m_Rotation + extraRotation) * Constants.RADTODEG * 10.0);

            Font f = new Font(FontFamily.GenericSansSerif, heightInPixels, GraphicsUnit.Pixel);
            return f;
            /*
            string familyName = (m_Font==null ? "Arial" : m_Font.Name);
            Font f = new Font(familyName, heightInPixels, GraphicsUnit.Pixel);

            if (m_Font!=null)
            {
                string faceName = m_Font.Name;
                f.Bold = m_Font.IsBold;
                f.Italic = m_Font.IsItalic;
                f.Underline = m_Font.IsUnderline;
            }
            */

            /*
            // Create font for the text, and select into the device context.
            LOGFONT lf;

            lf.lfHeight = height;
            lf.lfWidth = 0;
            lf.lfEscapement = rotation;
            lf.lfOrientation = lf.lfEscapement;
            lf.lfWeight = weight;
            lf.lfStrikeOut = 0;
            lf.lfCharSet = ANSI_CHARSET;
            lf.lfOutPrecision = OUT_TT_PRECIS;
            lf.lfClipPrecision = CLIP_DEFAULT_PRECIS;
            lf.lfQuality = PROOF_QUALITY;
            lf.lfPitchAndFamily = DEFAULT_PITCH;

            // If a typeface isn't known, use Arial. If you don't do this,
            // a system font would be used. However, the system font might
            // be different on different machines, meaning that any metrics
            // we've already calculated could be incorrect. Consequently,
            // things like the covering rectangle (used to populate the spatial
            // index) would be suspect.

            if (pFaceName)
                strcpy(lf.lfFaceName, pFaceName);
            else
                strcpy(lf.lfFaceName, "Arial");

            font.CreateFontIndirect(&lf);
             */
        }

        /// <summary>
        /// The vertical bearing of the text string (i.e. the bearing from the
        /// bottom of the characters to the top).
        /// </summary>
        double VBearing
        {
            get { return m_Rotation.Radians; }
        }

        /// <summary>
        /// The horizontal bearing of the text string (i.e. the bearing from
        /// the first to last char).
        /// </summary>
        double HBearing
        {
            get
            {
                double v = m_Rotation.Radians;

                if (v < Constants.TINY)
                    return Constants.PIDIV2;
                else if (v > Constants.PIMUL1P5)
                    return v - Constants.PIMUL1P5;
                else
                    return v + Constants.PIDIV2;
            }
        }
    }
}
