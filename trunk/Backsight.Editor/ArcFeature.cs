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
using System.Diagnostics;
using System.Xml;

using Backsight.Environment;
using Backsight.Geometry;
using Backsight.Forms;

namespace Backsight.Editor
{
    class ArcFeature : LineFeature
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ArcFeature</c>
        /// </summary>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="c">The circle the arc coincides with</param>
        /// <param name="bc">The point at the start of the arc</param>
        /// <param name="ec">The point at the end of the arc</param>
        /// <param name="isClockwise">True if the arc is directed clockwise from start to end</param>
        internal ArcFeature(IEntity e, Operation creator, Circle c, PointFeature bc, PointFeature ec, bool isClockwise)
            : base(e, creator, c, bc, ec, isClockwise)
        {
        }

        #endregion

        /// <summary>
        /// Modifies any referenced features by cross-referencing them to the line
        /// that contains this geometry.
        /// </summary>
        /// <param name="container">The line that refers to this geometry.</param>
        public override void AddReferences()
        {
            Circle.AddArc(this);
            base.AddReferences();
        }

        /// <summary>
        /// The geometry for this feature (just casts the <c>LineGeometry</c> property).
        /// </summary>
        internal ArcGeometry Geometry
        {
            get { return (ArcGeometry)LineGeometry; }
        }

        /// <summary>
        /// The circle the arc falls on.
        /// </summary>
        internal override Circle Circle
        {
            get { return (Circle)Geometry.Circle; }
        }

        /// <summary>
        /// Is the geometry for this arc directed clockwise from BC to EC?
        /// </summary>
        internal bool IsClockwise
        {
            get { return Geometry.IsClockwise; }
        }

        /// <summary>
        /// Gets the exact positions for the BC or EC. The "exact" positions are obtained
        /// by projecting the stored BC/EC to a position that is exactly consistent with the
        /// definition of the underlying circle. Typically, the shifts will be less than 1
        /// micron on the ground.
        /// </summary>
        /// <param name="pos">The position to project (either the BC or EC)</param>
        /// <returns>The position on the circle</returns>
        IPosition GetCirclePosition(IPosition pos)
        {
	        // Get the deltas of the position with respect to the centre of the circle.
            ICircleGeometry c = Circle;
            double cx = c.Center.X;
            double cy = c.Center.Y;
            double dx = pos.X - cx;
            double dy = pos.Y - cy;
	        double dist = Math.Sqrt(dx*dx + dy*dy);

	        // Return the centre if the position is coincident with the centre.
	        if (dist < Constants.TINY)
                return c.Center;

            // Get the factor for projecting the position.
	        double factor = c.Radius.Meters/dist;

	        // Figure out the position on the circle.
            double x = cx + dx*factor;
            double y = cy + dy*factor;
            return new Position(x,y);
        }

        /// <summary>
        /// Moves this arc by changing the circle on which it lies. Note that this
        /// does NOT move the locations that represent the BC and the EC; you must
        /// make separate calls to <c>PointFeature.Move</c> in order to do that.
        /// 
        /// This is called during rollforward processing (due to some changes, a circle
        /// that was formerly re-used may end up being no good, so a different circle
        /// needs to be referenced).
        /// </summary>
        /// <param name="newCircle">The new circle for the arc.</param>
        /// <param name="isClockwise">True if the arc is supposed to go clockwise.</param>
        internal void Move(Circle newCircle, bool isClockwise)
        {
            Circle oldCircle = this.Circle;

            if (!Object.ReferenceEquals(oldCircle, newCircle))
            {
                // Cut the reference from the arc's current circle.
                if (oldCircle!=null)
                    oldCircle.RemoveArc(this);

                // Add reference to the new circle (and vice versa).
                ChangeGeometry(new ArcGeometry(newCircle, StartPoint, EndPoint, isClockwise));
                newCircle.AddArc(this);
            }
            else
                Geometry.IsClockwise = isClockwise;
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // If we're dealing with a construction line, it's drawn dotted
            // regardless of the supplied style.
            if (Geometry.IsCircle && Creator.EditId == EditingActionId.NewCircle)
                CircleGeometry.Render(Geometry.Circle, display, new DottedStyle(style.LineColor));
            else
                base.Render(display, style);
        }
    }
}
