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
using System.Diagnostics;

using Backsight.Environment;
using Backsight.Forms;

namespace Backsight.Editor.Observations
{
	/// <written by="Steve Stanton" on="06-NOV-1997" />
    /// <summary>
    /// Some sort of direction observation.
    /// </summary>
    abstract class Direction : Observation
    {
        #region Class data

        /// <summary>
        /// Offset for the direction (if any)
        /// </summary>
        Offset m_Offset;

        #endregion

        #region Constructors

        internal Direction()
        {
            m_Offset = null;
        }

        #endregion

        /// <summary>
        /// The observed direction, in radians
        /// </summary>
        abstract internal double ObservationInRadians { get; }

        /// <summary>
        /// The observed direction, as a bearing.
        /// </summary>
        abstract internal IAngle Bearing { get; }

        /// <summary>
        /// The point the direction was measured from (also see the <c>StartPosition</c> property).
        /// </summary>
        abstract internal PointFeature From { get; set; }

        /*
//	Draw the direction angle.
	virtual void DrawAngle ( const CePoint* const pFrom
						   , CeView* view
						   , CDC* pDC
						   , const CeWindow* const pWin=0
						   , const CePoint* const pTo=0 ) const = 0;

	virtual void DrawAngle ( const CePoint* const pFrom
						   , CeDC& gdc
						   , const CePoint* const pTo=0 ) const = 0;
         */

        /// <summary>
        /// Creates the text that represents an observed angle.
        /// </summary>
        /*
        abstract void CreateAngleText
	virtual void CreateAngleText ( CPtrList& text
								 , const LOGICAL wantLinesToo
								 , const CePoint* const pFrom
								 , const CePoint* const pTo ) const = 0;
         */

        internal Offset Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        /// <summary>
        /// This direction's offset (if any), in meters on the ground. Negated offsets
        /// mean that the offset is to the left of the direction.
        /// </summary>
        double MetricOffset
        {
            get { return (m_Offset==null ? 0.0 : m_Offset.GetMetric(this)); }
        }


        /// <summary>
        /// Relational equality test. When comparing any offsets, it's the actual ground
        /// dimension of the offset we're concerned with -- NOT the address of the offset,
        /// or the way it was specified (i.e. an offset can be the same, even if one is
        ///	an OffsetDistance, and the other is an OffsetPoint).
        ///	
        /// The result of the comparison is from the Direction class & down. Directions will
        /// compare the same, even if the info in parent classes is different.
        /// </summary>
        /// <param name="other">The direction to compare with</param>
        /// <returns></returns>
        internal bool IsEquivalent(Direction other)
        {
            // Check the simple fields.
            if (this.GetType() != other.GetType())
                return false;

            // If one direction has an offset & the other doesn't, the directions are different.

	        if ( (m_Offset!=null && other.m_Offset==null) ||
		         (m_Offset==null && other.m_Offset!=null) )
                return false;

            //	If BOTH offsets are defined, see if they are equivalent.
	        if (m_Offset!=null && other.m_Offset!=null )
            {
                // It is assumed that the point from which the direction
                // was measured has already been checked by the operator==
                // function in derived sub-classes.

                // Get the (signed) offsets, in meters on the ground.
                double offThis = m_Offset.GetMetric(this);
		        double offThat = other.m_Offset.GetMetric(other);

                // The sign matters.
		        if ((offThis<0.0 && offThat>=0.0) || (offThis>=0.0 && offThat<0.0))
                    return false;

                // Same sign, so if the offsets are the same, they should subtract to zero.
		        if (Math.Abs(offThis-offThat) > Double.Epsilon)
                    return false;
	        }

            // Return the result of comparing the two derived classes.
            // ...not sure what's going on here... (shouldn't this just call an abstract method? -- maybe
            // needs to be a templated method, since both types need to be the same.

            if (this is AngleDirection || this is DeflectionDirection)
            {
                AngleDirection rThis = (AngleDirection)this;
                AngleDirection rThat = (AngleDirection)other;
                return rThis.Equals(rThat);
            }
            else if (this is BearingDirection)
            {
                BearingDirection rThis = (BearingDirection)this;
                BearingDirection rThat = (BearingDirection)other;
                return rThis.Equals(rThat);
            }
            else if (this is ParallelDirection)
            {
                ParallelDirection rThis = (ParallelDirection)this;
                ParallelDirection rThat = (ParallelDirection)other;
                return rThis.Equals(rThat);
            }

            return false;
        }

        /// <summary>
        /// The position at the start of this direction (which may be offset with
        /// respect to the position obtained via the <c>From</c> property).
        /// </summary>
        internal IPosition StartPosition
        {
            get
            {
                // If there is no offset, the start position corresponds to the point
                // that the direction starts from.
                if (m_Offset==null)
                    return this.From;

                // If the offset is an offset point, that's the position we want
                if (m_Offset is OffsetPoint)
                    return (m_Offset as OffsetPoint).Point;

                // This leaves us with an offset distance, so get it. The value is signed (left<0 right>0).
                double offset = m_Offset.GetMetric(this);

                // Get the bearing of the direction & add 90 degrees clockwise rotation).
                double bearing = this.Bearing.Radians + Constants.PIDIV2;

                // Get the origin of the direction (it SHOULD be defined).
                PointFeature from = this.From;
                Debug.Assert(from!=null);

                // Figure out the approximate perpendicular point (treating the
                // offset as a distance on the mapping plane, even though
                // it's actually a distance on the ground).
                IPosition approx = Geom.Polar(from, bearing, offset);

                // Calculate the line scale factor between the from-point and the approximate position.
                ISpatialSystem sys = CadastralMapModel.Current.SpatialSystem;
                double sfac = sys.GetLineScaleFactor(from, approx);

                // Figure out the exact offset position.
                return Geom.Polar(from, bearing, offset*sfac);
            }
        }

        /// <summary>
        /// Intersects this direction with another direction.
        /// </summary>
        /// <param name="other">The other direction.</param>
        /// <returns>The calculated intersection (null if it doesn't exist)</returns>
        internal IPosition Intersect(Direction other)
        {
            // Get the starting positions for the two directions (may be offset).
            IPosition from1 = this.StartPosition;
            IPosition from2 = other.StartPosition;

            // Get the two directions in the form of a bearing.
            double b1 = this.Bearing.Radians;
            double b2 = other.Bearing.Radians;

            // The equation of each line is given in parametric form as:
            //
            //		x = xo + f * r
            //		y = yo + g * r
            //
            // where	xo,yo is the from point
            // and		f = sin(bearing)
            // and		g = cos(bearing)
            // and		r is the position ratio

	        double cosb1 = Math.Cos(b1);
	        double sinb1 = Math.Sin(b1);
	        double cosb2 = Math.Cos(b2);
	        double sinb2 = Math.Sin(b2);

	        double f1g2 = sinb1 * cosb2;
	        double f2g1 = sinb2 * cosb1;
	        double det = f2g1 - f1g2;

            // Check whether lines are parallel.
	        if (Math.Abs(det) < Double.Epsilon)
                return null;

            // Work out the intersection.

            double yo = from1.Y;
	        double xo = from1.X;
	        double dy = from2.Y - yo;
	        double dx = from2.X - xo;
	        double prat = (sinb2*dy - cosb2*dx)/det;
	        return new Position((xo+sinb1*prat), (yo+cosb1*prat));
        }

        /// <summary>
        /// Intersects this direction with a circle. This may lead to 0, 1, or 2 intersections.
        /// If 2 intersections are found, the first intersection is defined as the one that is
        /// closer to the origin of the direction object.
        /// </summary>
        /// <param name="circle">The circle to intersect with.</param>
        /// <param name="x1">The 1st intersection (if any). Returned as null if no intersection.</param>
        /// <param name="x2">The 2nd intersection (if any). Returned as null if no intersection.</param>
        /// <returns>The number of intersections found.</returns>
        internal uint Intersect(ICircleGeometry circle, out IPosition x1, out IPosition x2)
        {
            // Initialize both intersections.
            x1 = x2 = null;

            // Get the origin of the direction & its bearing.
            IPosition start = this.StartPosition;
            double bearing = this.Bearing.Radians;

            // Get the centre of the circle & its radius.
            IPosition center = circle.Center;
            double radius = circle.Radius;

            // The equation of each line is given in parametric form as:
            //
            //		x = xo + f * r
            //		y = yo + g * r
            //
            // where	xo,yo is the from point
            // and		f = sin(bearing)
            // and		g = cos(bearing)
            // and		r is the position ratio

	        double g = Math.Cos(bearing);
	        double f = Math.Sin(bearing);
	        double fsq = f*f;
	        double gsq = g*g;
	        double fgsq = fsq + gsq;

	        if (fgsq<Double.Epsilon) // Should be impossible
                return 0; 

            double startx = start.X;
	        double starty = start.Y;
	        double dx = center.X - startx;
	        double dy = center.Y - starty;
	        double fygx = f*dy - g*dx;
	        double root = radius*radius*fgsq - fygx*fygx;

            // Check for no intersection.
	        if (root < -Constants.TINY )
                return 0;

            // Check for tangential intersection (but don't count a
            // tangent if it precedes the start of the direction line).

            double fxgy = f*dx + g*dy;
	        double prat;

            if (root < Constants.TINY)
            {
		        prat = fxgy/fgsq;
		        if (prat<0.0)
                    return 0;
		        x1 = new Position(startx + f*prat, starty + g*prat);
		        return 1;
	        }

            // We have two intersections, but one or both of them could
            // precede the start of the direction line.

	        root = Math.Sqrt(root);
	        double fginv = 1.0/fgsq;
	        double prat1 = (fxgy - root)*fginv;
	        double prat2 = (fxgy + root)*fginv;

            // No intersections if both are prior to start of direction.
	        if (prat1<0.0 && prat2<0.0)
                return 0;

            // If both intersections are valid ...
	        prat = Math.Min(prat1,prat2);
	        if (prat>=0.0)
            {
		        x1 = new Position(startx + f*prat, starty + g*prat);
		        prat = Math.Max(prat1,prat2);
		        x2 = new Position(startx + f*prat, starty + g*prat);
		        return 2;
	        }

            // So only one is valid ...
	        prat = Math.Max(prat1,prat2);
	        x1 = new Position(startx + f*prat, starty + g*prat);
	        return 1;
        }

        /// <summary>
        /// Performs actions when the operation that uses this observation is marked
        /// for deletion as part of its rollback function. This cuts any reference from any
        /// previously existing feature that was cross-referenced to the operation (see
        /// calls made to AddOp).
        /// </summary>
        /// <param name="op">The operation that makes use of this observation.</param>
        internal override void OnRollback(Operation op)
        {
            Debug.Assert(op!=null);

            if (m_Offset!=null)
                m_Offset.OnRollback(op);
        }

        /// <summary>
        /// Cuts references to an operation that are made by the features this
        /// direction refers to.
        /// </summary>
        /// <param name="op">The operation that should no longer be referred to.</param>
        abstract internal void CutReferences(Operation op);

        /// <summary>
        /// Cut references to an operation that are made by the features the direction refers to.
        /// </summary>
        /// <param name="op">The operation that should no longer be referred to.</param>
        internal void CutRef(Operation op)
        {
            CutReferences(op);

            // Any offset may conceivably refer to a feature.
            if (m_Offset!=null)
                m_Offset.CutRef(op);
        }

        /// <summary>
        /// Intersects this direction with a specific entity type. 
        /// </summary>
        /// <param name="ent">The entity type to look for.</param>
        /// <param name="maxdist">Observation defining the maximum distance between the
        /// direction's from-point & the intersection. This can either be a <c>Distance</c>
        /// or an <c>OffsetPoint</c> object.</param>
        /// <param name="xsect">The position of the intersection (if any). Null if no intersection found.</param>
        /// <returns>True if an intersection was found.</returns>
        bool Intersect(IEntity ent, Observation maxdist, out IPosition xsect)
        {
            // Initialize the intersection.
            xsect = null;

            //	Get the max distance. It has to be defined to be SOMETHING.
            double dist = maxdist.GetDistance(this.From).Meters;
	        if (Math.Abs(dist) < Constants.TINY)
                return false;

            //	Define the position of the direction line.
            IPosition from = this.StartPosition;
            IPosition to = Geom.Polar(from, this.Bearing.Radians, dist);

            throw new NotImplementedException("Direction.Intersect");

            /*
//	Construct a corresponding line segment.
	CeLocation start(vfrom);
	CeLocation end(vto);
	CeSegment seg(start,end);

//	Intersect the segment with the map (all features on
//	the currently active theme).
	CeLayerList curlayer;
	CeXObject xseg(seg,&curlayer);

//	Get the first intersection (if any) with the specified
//	entity type.

	UINT4 nprim = xseg.GetCount();	// How many primitives did we intersect?
	FLOAT8 bestdsq = 10e+38;		// Closest distance (squared) so far.
	FLOAT8 tdistsq;					// Test distance (squared).
	LOGICAL gotone=FALSE;

	for ( UINT4 i=0; i<nprim; i++ ) {

//		Get the next thing we intersected.
		const CeXResult& xres = xseg[i];
		const CeLine* const pLine =
			dynamic_cast<const CeLine* const>(xres.GetpObject());

//		Skip if it wasn't a line.
		if ( !pLine ) continue;

//		Find the attached arc that has the current theme. Has
//		to be topological(?)
		const CeArc* const pArc = pLine->GetpArc(curlayer);

//		Skip if it doesn't have the desired entity type.
		if ( pArc && pArc->GetpEntity()!=&ent ) continue;

//		Determine the intersection on the primitive that is
//		closest to the start of the direction (ignoring any
//		intersections that are right at the start of the
//		direction line).

		gotone = TRUE;
		UINT4 nx = xres.GetCount();
		CeVertex vtest;

		for ( UINT4 j=0; j<nx; j++ ) {

			vtest = xres.GetX1(j);
			tdistsq = vfrom.DistanceSquared(vtest);
			if ( tdistsq<bestdsq && tdistsq>TINY ) {
				xsect = vtest;
				bestdsq = tdistsq;
			}

//			Check any grazing intersection too.
			if ( xres.IsGrazing() ) {
				vtest = xres.GetX2(j);
				tdistsq = vfrom.DistanceSquared(vtest);
				if ( tdistsq<bestdsq && tdistsq>TINY ) {
					xsect = vtest;
					bestdsq = tdistsq;
				}
			}

		} // next intersection

	} // next intersected primitive

	return gotone;

} // end of Intersect
            */
        }

        /// <summary>
        /// Intersects this direction with a line.
        /// </summary>
        /// <param name="line">The line to intersect with.</param>
        /// <param name="closeTo">The point that the intersection should be closest to.
        /// Specify null if you don't care. In that case, if there are multiple intersections,
        /// you get the intersection that is closest to one of 3 points: the start of the
        /// direction line, the start of the line, or the end of the line.</param>
        /// <param name="xsect">The position of the intersection (if any). Null if not found.</param>
        /// <param name="closest">The default point that is closest to the intersection. Null if
        /// intersection wasn't found.</param>
        /// <returns>True if intersection was found.</returns>
        internal bool Intersect( LineFeature line
                               , PointFeature closeTo
                               , out IPosition xsect
                               , out PointFeature closest )
        {
        	// Initialize results
            xsect = null;
            closest = null;

            // Define the length of the direction line as the length
	        // of a diagonal that crosses the map's extent.
            IWindow mapWin = SpatialController.Current.MapModel.Extent;
            Debug.Assert(mapWin!=null);

            // If the window is currently undefined (e.g. during deserialization),
            // just use a really big distance.
            // TODO: This is a hack, but hopefully it may be shortlived, because
            // new logic is in the works for handling updates.
            double dist = (mapWin.IsEmpty ? 100000.0 : Geom.Distance(mapWin.Min, mapWin.Max));

        	// Define the position of the direction line. DON'T use the from-
	        // point, because there may be an offset to the direction.
            IPosition fromPos = this.StartPosition;
            IPosition toPos = Geom.Polar(fromPos, this.Bearing.Radians, dist);

            // Construct a corresponding line segment.
            ITerminal start = new FloatingTerminal(fromPos);
            ITerminal end = new FloatingTerminal(toPos);
            SegmentGeometry seg = new SegmentGeometry(start, end);

            // Intersect the line segment with the other one.
            IntersectionResult xres = new IntersectionResult(line);
            uint nx = seg.Intersect(xres);
            if (nx==0)
                return false;

        	// Determine which terminal point is the best. Start with the
            // ends of the intersected line.
	        double mindsq = Double.MaxValue;

            if (xres.GetCloserPoint(line.StartPoint, ref mindsq, ref xsect))
                closest = line.StartPoint;

            if (xres.GetCloserPoint(line.EndPoint, ref mindsq, ref xsect))
                closest = line.EndPoint;

            // Check whether the direction from-point is any closer (the position may be
            // different from the start of the direction line, because the direction may
            // have an offset).

            if (xres.GetCloserPoint(this.From, ref mindsq, ref xsect))
                closest = this.From;

	        // If a close-to point has been specified, that overrides
	        // everything else (however, doing the above has the desired
	        // effect of defining the best of the default points). In
	        // this case, we allow an intersection that coincides with
	        // the line being intersected.

        	if (closeTo!=null)
                xres.GetClosest(closeTo, out xsect, 0.0);

            return (xsect!=null);
        }

        /// <summary>
        /// Checks whether this observation makes reference to a specific feature.
        /// </summary>
        /// <param name="feature">The feature to check for.</param>
        /// <returns>True if this direction refers to the feature (via the direction's offset)</returns>
        internal override bool HasReference(Feature feature)
        {
            return (m_Offset==null ? false : m_Offset.HasReference(feature));
        }

        /// <summary>
        /// Normalizes some sort of direction so that it is in the range [0, 2*PI]
        /// </summary>
        /// <param name="a">The value to normalize</param>
        /// <returns>The value in the range [0, 2*PI]</returns>
        internal static double Normalize(double a)
        {
            double result;
            for (result=a; result<0.0; result += Constants.PIMUL2) { }
            for (; result>=Constants.PIMUL2; result -= Constants.PIMUL2) { }
            return result;
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        internal override Feature[] GetReferences()
        {
            if (m_Offset == null)
                return new Feature[0];
            else
                return m_Offset.GetReferences();
        }

        /// <summary>
        /// Renders this direction as a dotted magenta line.
        /// </summary>
        /// <param name="display"></param>
        internal void Render(ISpatialDisplay display)
        {
            // Figure out where the direction line is
            IPosition from = this.StartPosition;
            double len = GetMaxDiagonal(display);
            IPosition to = Geom.Polar(from, this.Bearing.Radians, len);

            new DottedStyle().Render(display, new IPosition[] {from, to});
        }

        /// <summary>
        /// The diagonal length of a line that spans the display when it is
        /// drawn at the overview scale.
        /// </summary>
        /// <param name="display"></param>
        /// <returns></returns>
        double GetMaxDiagonal(ISpatialDisplay display)
        {
            IWindow x = display.MaxExtent;
            return Geom.Distance(x.Min, x.Max);
        }
    }
}
