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

using Backsight;
using Backsight.Forms;
using Backsight.Geometry;


namespace CadastralViewer.Xml
{
    public partial class Line : ISpatialObject
    {
        #region Class data

        /// <summary>
        /// The point at the start of the line
        /// </summary>
        IPoint m_From;

        /// <summary>
        /// The point at the end of the line
        /// </summary>
        IPoint m_To;

        /// <summary>
        /// The point at the center of a circular arc (null if the line
        /// is not a circular arc).
        /// </summary>
        IPoint m_Center;

        /// <summary>
        /// The parcel that this line is part of.
        /// </summary>
        Parcel m_Parcel;

        #endregion

        /// <summary>
        /// The point at the start of the line
        /// </summary>
        internal IPoint From
        {
            get { return m_From; }
            set { m_From = value; }
        }

        /// <summary>
        /// The point at the end of the line
        /// </summary>
        internal IPoint To
        {
            get { return m_To; }
            set { m_To = value; }
        }

        internal IPoint Center
        {
            get { return m_Center; }
            set { m_Center = value; }
        }

        internal Parcel Parcel
        {
            get { return m_Parcel; }

            set
            {
                // A line should be related to exactly one parcel
                if (m_Parcel != null)
                    throw new ApplicationException("Attempt to define line's parcel more than once");

                m_Parcel = value;
            }
        }


        #region ISpatialObject Members

        public SpatialType SpatialType
        {
            get { return SpatialType.Line; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            if (this.category == CadastralLineCategory.Radial)
                style = new DottedStyle(style.LineColor);

            if (m_Center == null)
                style.Render(display, this.PositionArray);
            else
            {
                // radius less than zero may represent a counter-clockwise direction
                bool isClockwise = (this.radius > 0.0);

                // Define a circular arc that is assumed to run clockwise.
                ICircleGeometry circle = new CircleGeometry(m_Center.Geometry, Math.Abs(this.radius));
                ICircularArcGeometry arc = new CircularArcGeometry(circle, m_From.Geometry, m_To.Geometry, isClockwise);

                // Assume clockwise, see what it looks like
                style.Render(display, arc);
            }

                /*
            else
            {
                if (!this.arcLengthSpecified)
                    throw new ApplicationException("Cannot determine arc direction");

                // Define a circular arc that is assumed to run clockwise.
                CircleGeometry circle = new CircleGeometry(m_Center.Geometry, this.radius);
                CircularArcGeometry arc = new CircularArcGeometry(circle, m_From.Geometry, m_To.Geometry, true);

                // Assume clockwise, see what it looks like
                new DrawStyle(Color.Red).Render(display, arc);

                //double arcLength = arc.Length.Meters;
                //double othLength = circle.Length.Meters;

                //// Get the arc length in meters (TODO: need to access file header to determine how to convert lengths)
                //if (Math.Abs(othLength - this.arcLength) < Math.Abs(arcLength - this.arcLength))
                //    arc.IsClockwise = false;
            }
                 */
        }

        /// <summary>
        /// The spatial extent of this object.
        /// </summary>
        /// <value></value>
        public IWindow Extent
        {
            get { return new Window(this.PositionArray); }
        }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>
        /// The shortest distance between the specified position and this object
        /// </returns>
        public ILength Distance(IPosition point)
        {
            double dsq = BasicGeom.MinDistanceSquared(this.PositionArray, point);
            return new Length(Math.Sqrt(dsq));
        }

        #endregion

        /// <summary>
        /// Expresses the geometry of this line as an array of positions.
        /// </summary>
        IPosition[] PositionArray
        {
            get { return new IPosition[] { m_From.Geometry, m_To.Geometry }; }
        }
    }
}
