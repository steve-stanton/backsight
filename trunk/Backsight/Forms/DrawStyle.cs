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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;

using Backsight.Geometry;

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="02-OCT-2006" />
    /// <summary>Basic methods for drawing map data</summary>
    public class DrawStyle : IDrawStyle
    {
        #region Class data

        private Pen m_Pen;
        private IFill m_Fill;
        private GraphicsPath m_Path;

        /// <summary>
        /// The default height for point features
        /// </summary>
        private ILength m_PointHeight;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>DrawStyle</c> with black pen & black fill
        /// </summary>
        public DrawStyle() : this(Color.Black)
        {
        }

        /// <summary>
        /// Creates a <c>DrawStyle</c> with a specific pen & fill color.
        /// </summary>
        /// <param name="c">The color for the pen & the fill</param>
        public DrawStyle(Color c)
        {
            m_Pen = new Pen(c);
            m_Fill = new Fill(c);
            m_Path = new GraphicsPath();
            m_PointHeight = new Length(1.0);
        }

        #endregion

        public Pen Pen
        {
            get { return m_Pen; }
            set { m_Pen = value; }
        }

        public Brush Brush
        {
            get { return m_Fill.Brush; }
        }

        /// <summary>
        /// The color used to fill things
        /// </summary>
        public Color FillColor
        {
            get { return m_Fill.Color; }
            set { m_Fill.Color = value; }
        }

        /// <summary>
        /// The color used to draw lines
        /// </summary>
        public Color LineColor
        {
            get { return m_Pen.Color; }
            set { m_Pen.Color = value; }
        }

        /// <summary>
        /// Draws a point
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position of the center of the point</param>
        public void Render(ISpatialDisplay display, IPosition position)
        {
            float size = display.LengthToDisplay(m_PointHeight.Meters);
            if (size>1.0F)
            {
                PointF p = CreatePoint(display, position);
                float d = size * 0.5F;
                if (FillColor == Color.Transparent)
                    display.Graphics.DrawRectangle(m_Pen, p.X-d, p.Y-d, size, size);
                else
                    display.Graphics.FillRectangle(Brush, p.X-d, p.Y-d, size, size);
            }
        }

        /// <summary>
        /// Draws a point as a plus sign.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position of the center of the point</param>
        public void RenderPlus(ISpatialDisplay display, IPosition position)
        {
            float size = display.LengthToDisplay(m_PointHeight.Meters);
            if (size>1.0F)
            {
                PointF p = CreatePoint(display, position);
                float d = size * 0.5F;
                display.Graphics.DrawLine(m_Pen, p.X-d, p.Y, p.X+d, p.Y);
                display.Graphics.DrawLine(m_Pen, p.X, p.Y-d, p.X, p.Y+d);
            }
        }

        /// <summary>
        /// Draws a point as a triangle.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position of the center of the point</param>
        public void RenderTriangle(ISpatialDisplay display, IPosition position)
        {
            float size = display.LengthToDisplay(m_PointHeight.Meters);
            if (size>1.0F)
            {
                PointF p = CreatePoint(display, position);
                float d = size * 0.5F;
                PointF[] outline = new PointF[4];
                outline[0] = new PointF(p.X-d, p.Y+d);
                outline[1] = new PointF(p.X+d, p.Y+d);
                outline[2] = new PointF(p.X, p.Y-d);
                outline[3] = outline[0];
                display.Graphics.FillPolygon(m_Fill.Brush, outline);
            }
        }

        /// <summary>
        /// Draws the supplied icon
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position for the center of the icon</param>
        /// <param name="icon">The icon to display</param>
        public void Render(ISpatialDisplay display, IPosition position, Icon icon)
        {
            PointF p = CreatePoint(display, position);
            int x = (int)p.X - icon.Width/2;
            int y = (int)p.Y + icon.Height/2;
            display.Graphics.DrawIcon(icon, x, y);
        }

        PointF[] GetDisplayPoints(ISpatialDisplay display, IPosition[] line)
        {
            PointF[] pts = new PointF[line.Length];

            for (int i=0; i<line.Length; i++)
            {
                IPosition gp = line[i];
                pts[i].X = display.EastingToDisplay(gp.X);
                pts[i].Y = display.NorthingToDisplay(gp.Y);
            }

            return pts;
        }

        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="line">The positions defining the line (expected to be at
        /// least two positions)</param>
        public void Render(ISpatialDisplay display, IPosition[] line)
        {
            if (line.Length==1)
            {
                Render(display, line[0]);
                return;
            }

            try
            {
                PointF[] pts = GetDisplayPoints(display, line);
                m_Path.AddLines(pts);
                display.Graphics.DrawPath(m_Pen, m_Path);
            }

            finally
            {
                m_Path.Reset();
            }


            /*
            try
            {
                PointF[] pts = new PointF[line.Length];
                for (int i=0; i<line.Length; i++)
                {
                    IPosition gp = line[i];
                    pts[i].X = display.EastingToDisplay(gp.X);
                    pts[i].Y = display.NorthingToDisplay(gp.Y);
                }

                m_Path.AddLines(pts);

                try { display.Graphics.DrawPath(m_Pen, m_Path); }
                catch
                {
                    // Attempt to avoid occasional exception by doing it again
                    m_Path = new GraphicsPath();
                    m_Path.AddLines(pts);
                    display.Graphics.DrawPath(m_Pen, m_Path);
                }
            }

            finally
            {
                // This occasionally leads to an InvalidOperationException with the message
                // "Object is currently in use elsewhere" (tends to happen when I'm quickly
                // panning the display). Perhaps the Graphics class ends up calling some
                // flakey unmanaged code. To try to get around this, just create a brand
                // new (empty) path.

                try { m_Path.Reset(); }
                catch { m_Path = new GraphicsPath(); }
            }
             */
        }

        /// <summary>
        /// Draws a circular arc
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="arc">The circular arc</param>
        public void Render(ISpatialDisplay display, IClockwiseCircularArcGeometry arc)
        {
            ICircleGeometry circle = arc.Circle;
            IWindow extent = CircleGeometry.GetExtent(circle);
            float topLeftX = display.EastingToDisplay(extent.Min.X);
            float topLeftY = display.NorthingToDisplay(extent.Max.Y);
            float size = 2.0f * display.LengthToDisplay(circle.Radius.Meters);
            float startAngle = (float)(arc.StartBearingInRadians * MathConstants.RADTODEG - 90.0);
            float sweepAngle = (float)(arc.SweepAngleInRadians * MathConstants.RADTODEG);
            display.Graphics.DrawArc(m_Pen, topLeftX, topLeftY, size, size, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a text string (annotation)
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="text">The item of text</param>
        public void Render(ISpatialDisplay display, IString text)
        {
            // Draw the outline if it's too small
            Font f = text.CreateFont(display);
            IPosition[] outline = text.Outline;

            if (f==null)
            {
                Render(display, outline);
            }
            else
            {
                string s = text.Text;

                // Note that the order you apply the transforms is significant...

                PointF p = CreatePoint(display, outline[0]);
                display.Graphics.TranslateTransform(p.X, p.Y);

                double rotation = text.Rotation.Degrees;
                display.Graphics.RotateTransform((float)rotation);

                Size size = TextRenderer.MeasureText(s, f);
                double groundWidth = BasicGeom.Distance(outline[0], outline[1]);
                float xScale = display.LengthToDisplay(groundWidth) / (float)size.Width;
                float yScale = f.Size / (float)size.Height;
                display.Graphics.ScaleTransform(xScale, yScale);

                // I tried StringFormat.GenericDefault, but that seems to leave too much
                // leading space.
                display.Graphics.DrawString(s, f, Brush, 0, 0, StringFormat.GenericTypographic);
                display.Graphics.ResetTransform();
            }
        }

        PointF CreatePoint(ISpatialDisplay display, IPosition p)
        {
            float x = display.EastingToDisplay(p.X);
            float y = display.NorthingToDisplay(p.Y);
            return new PointF(x, y);
        }

        /// <summary>
        /// The default height for point features (on the ground)
        /// </summary>
        public ILength PointHeight
        {
            get { return m_PointHeight; }
            set { m_PointHeight = value; }
        }

        /// <summary>
        /// Draws a circle
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="center">The position of the center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        public void Render(ISpatialDisplay display, IPosition center, ILength radius)
        {
            float xc = display.EastingToDisplay(center.X);
            float yc = display.NorthingToDisplay(center.Y);
            float r = display.LengthToDisplay(radius.Meters);
            float sz = r+r;
            display.Graphics.DrawEllipse(m_Pen, xc-r, yc-r, sz, sz);
        }

        /// <summary>
        /// Fills a polygon, possibly including holes.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="outlines">The outlines of one or more closed shapes. The first
        /// array corresponds to the outline of the enclosing polygon, while the
        /// remaining arrays correspond to islands.</param>
        public void Render(ISpatialDisplay display, IPosition[][] outlines)
        {
            try
            {
                foreach(IPosition[] a in outlines)
                {
                    PointF[] pts = GetDisplayPoints(display, a);
                    GraphicsPath p = new GraphicsPath();

                    // TODO: I periodically get an ArgumentException with this call, don't see anything
                    // in the online help. So quietly ignore it (I think it's relating to circular arcs,
                    // so it might be an idea to draw something like a straight line).

                    try
                    {
                        p.AddLines(pts);
                        m_Path.AddPath(p, false);
                    }

                    catch { }
                }

                Brush b = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Yellow, Color.Transparent);
                display.Graphics.FillPath(b, m_Path);
            }

            finally
            {
                m_Path.Reset();
            }
        }

        /// <summary>
        /// The fill for closed shapes
        /// </summary>
        public IFill Fill
        {
            get { return m_Fill; }
            set { m_Fill = value; }
        }
    }
}
