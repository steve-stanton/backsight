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

using Backsight.Environment;
using Backsight.Geometry;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="" />
    /// <summary>
    /// A text object is some sort of string that appears on a map. This is the base class for
    //	MiscText, KeyText, RowText, and FeatureText.
    /// </summary>
    abstract public class TextGeometry : IString, IXmlContent
    {
        #region Class data

        /// <summary>
        /// The text style (defines the type-face and the height of the text). The enclosing
        /// CadastralMapModel object maintains a list of all the fonts that have been used
        /// by text that appears within the map. If null, the system's default font will be used.
        /// </summary>
        private IFont m_Font;

        /// <summary>
        /// The total width of the text, in meters on the ground.
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
        private PointGeometry m_Position;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>TextGeometry</c>
        /// </summary>
        /// <param name="pos">Position of the text's reference point (always the top left corner of the string).</param>
        /// <param name="font">The text style (defines the type-face and the height of the text).</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The total width of the text, in meters on the ground.</param>
        /// <param name="rotation">Clockwise rotation from horizontal</param>
        protected TextGeometry(PointGeometry pos, IFont font, double height, double width, float rotation)
        {
            m_Font = font;
            m_Position = pos;
            m_Height = (float)height;
            m_Width = (float)width;
            m_Rotation = new RadianValue((double)rotation);
        }

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        protected TextGeometry()
        {
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
            internal set { m_Position = PointGeometry.Create(value); }
        }

        /// <summary>
        /// The spacing between each character in this text object (in meters on the ground)
        /// </summary>
        /*
        internal float Spacing
        {
            get { return m_Width; }
            set { m_Width = value; }            
        }
        */

        /// <summary>
        /// Clockwise rotation from horizontal
        /// </summary>
        public IAngle Rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
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
        internal IFont Font
        {
            get { return m_Font; }
            set { m_Font = value; }
        }

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
                double ght = (double)m_Height;
                double gwd = (double)m_Width;

                /*
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
                */
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
            double ght = (double)m_Height;
            float dht = display.LengthToDisplay(ght);
            int ht = (int)dht;
            if (ht<=0)
                return null;

            return CreateFont(ht, 0.0);
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

            IFont fontInfo = (m_Font==null ? FontInfo.Default : m_Font);
            return new Font(fontInfo.TypeFace, (float)heightInPixels, fontInfo.Modifiers, GraphicsUnit.Pixel);

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

        /// <summary>
        /// Gets the size of the supplied text (when drawn horizontally)
        /// </summary>
        /// <param name="text">The text to obtain size for</param>
        /// <param name="ent">The entity type for the text</param>
        /// <returns>The size of the text when rendered on screen (in pixels)</returns>
        //internal static Size GetDisplaySize(string text, IEntity ent)
        //{
        //    IFont fontInfo = (ent==null ? null : ent.Font);
        //    if (fontInfo==null)
        //        fontInfo = FontInfo.Default;
        //    Font font = new Font(fontInfo.TypeFace, fontInfo.PointSize, fontInfo.Modifiers);
        //    Size proposedSize = new Size(int.MaxValue, int.MaxValue);
        //    return TextRenderer.MeasureText(text, font, proposedSize,
        //        (TextFormatFlags.NoPadding | TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix));
        //}

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public virtual void WriteContent(XmlContentWriter writer)
        {
            writer.WriteLong("X", m_Position.Easting.Microns);
            writer.WriteLong("Y", m_Position.Northing.Microns);
            writer.WriteInt("FontId", (m_Font==null ? 0 : m_Font.Id));
            writer.WriteString("Height", String.Format("{0:0.00}", m_Height));
            writer.WriteString("Width", String.Format("{0:0.00}", m_Width));

            // TODO: May want to cover indirect rotations
            writer.WriteString("Rotation", RadianValue.AsString(m_Rotation.Radians));
        }

        public virtual void ReadContent(XmlContentReader reader)
        {
            m_Position = new PointGeometry();
            m_Position.ReadContent(reader);
            m_Font = EnvironmentContainer.FindFontById(reader.ReadInt("FontId"));
            m_Height = Single.Parse(reader.ReadString("Height"));
            m_Width = Single.Parse(reader.ReadString("Width"));

            double rot;
            if (RadianValue.TryParse(reader.ReadString("Rotation"), out rot))
                m_Rotation = new RadianValue(rot);
            else
                throw new Exception("Cannot parse text rotation");
        }

        public void WriteContent(ContentWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region IXmlContent Members


        public void ReadContent(ContentReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
