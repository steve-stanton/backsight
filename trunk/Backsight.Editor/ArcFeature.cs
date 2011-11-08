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
using System.Collections.Generic;

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
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="sessionSequence">The 1-based creation sequence of this feature within the
        /// session that created it.</param>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="c">The circle the arc coincides with</param>
        /// <param name="bc">The point at the start of the arc</param>
        /// <param name="ec">The point at the end of the arc</param>
        /// <param name="isClockwise">True if the arc is directed clockwise from start to end</param>
        internal ArcFeature(Operation creator, uint sessionSequence, IEntity e, Circle c, PointFeature bc, PointFeature ec, bool isClockwise)
            : base(creator, sessionSequence, e, bc, ec, new ArcGeometry(c, bc, ec, isClockwise))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArcFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="g">The geometry for the line (could be null, although this is only really
        /// expected during deserialization)</param>
        /// <exception cref="ArgumentNullException">If <paramref name="f"/> is null.</exception>
        internal ArcFeature(IFeature f, PointFeature bc, PointFeature ec, ArcGeometry g, bool isTopological)
            : base(f, bc, ec, g, isTopological)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArcFeature"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal ArcFeature(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
        }

        #endregion

        /// <summary>
        /// Modifies any referenced features by cross-referencing them to the line
        /// that contains this geometry.
        /// </summary>
        /// <param name="container">The line that refers to this geometry.</param>
        internal override void AddReferences()
        {
            // The circle may not be known at this stage (this method is called
            // by the LineFeature constructor, and the geometry may be undefined
            // at that stage -- come to think of it, I believe more recent logic
            // means that the geometry will NEVER be known when a LineFeature is
            // created. The circle->arc cross reference needs to be made when the
            // arc geometry is defined.

            Circle c = this.Circle;
            if (c!=null)
                c.AddArc(this);

            base.AddReferences();
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>(base.GetRequiredFeatures());

            Circle c = this.Circle;
            if (c!=null)
                result.AddRange(c.GetRequiredFeatures()); // the center point

            return result.ToArray();
        }

        /// <summary>
        /// The geometry for this feature (just casts the <c>LineGeometry</c> property).
        /// </summary>
        internal ArcGeometry Geometry
        {
            get { return (ArcGeometry)LineGeometry; }

            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                base.LineGeometry = value;

                // Ensure the circle is associated with this arc
                this.Circle.AddArc(this);
            }
        }

        /// <summary>
        /// The circle the arc falls on.
        /// </summary>
        internal override Circle Circle
        {
            get
            {
                ArcGeometry ag = Geometry;
                return (ag==null ? null : (Circle)ag.Circle);
            }
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
	        double factor = c.Radius/dist;

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

        /// <summary>
        /// Attempts to locate the circular arc (if any) that this line is based on.
        /// </summary>
        /// <returns><c>this</c> (always).</returns>
        internal override ArcFeature GetArcBase()
        {
            return this;
        }
    }
}
