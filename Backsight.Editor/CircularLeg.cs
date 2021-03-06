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
using System.Diagnostics;
using Backsight.Editor.Observations;
using Backsight.Editor.Operations;
using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeCircularLeg" />
    /// <summary>
    /// A circular leg in a connection path.
    /// </summary>
    class CircularLeg : Leg
    {
        #region Class data

        /// <summary>
        /// Observations defining the shape of the circular leg.
        /// </summary>
        readonly CircularLegMetrics m_Metrics;

        /// <summary>
        /// The circle that this leg sits on. The radius defined as part of this instance is
        /// the adjusted radius. This may well be slightly different from the observed radius that is
        /// defined as part of the leg metrics.
        /// </summary>
        Circle m_Circle;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>CircularLeg</c> with no spans.
        /// </summary>
        /// <param name="radius">The radius of the circular leg.</param>
        /// <param name="clockwise">True if the curve is clockwise.</param>
        /// <param name="span">The number of spans on the curve.</param>
        internal CircularLeg(Distance radius, bool clockwise, int nspan)
            : base(nspan)
        {
            m_Metrics = new CircularLegMetrics(radius, clockwise);

            // The circle for this leg won't be known till we create a span.
            m_Circle = null;
        }

        #endregion

        /// <summary>
        /// The circle that this leg sits on.
        /// </summary>
        internal override Circle Circle
        {
            get { return m_Circle; }
        }

        /// <summary>
        /// The total length of this leg, in meters on the ground.
        /// </summary>
        internal override ILength Length
        {
            get
            {
                // If we have a cul-de-sac, we can determine the length using
                // just the central angle & the radius. Otherwise ask the base
                // class to return the total observed length.
                if (m_Metrics.IsCulDeSac)
                {
                    double radius = this.RadiusInMeters;
                    return new Length((MathConstants.PIMUL2 - m_Metrics.CentralAngle) * radius);
                }
                else
                    return new Length(base.PrimaryFace.GetTotal());
            }
        }

        /// <summary>
        /// Given the position of the start of this leg, along with an initial bearing,
        /// project the end of the leg, along with an exit bearing.
        /// </summary>
        /// <param name="pos">The position for the start of the leg. Updated to be the position
        /// for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated for
        /// this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances (default=1.0).</param>
        internal override void Project(ref IPosition pos, ref double bearing, double sfac)
        {
            // Get circle info
            IPosition center, ec;
            double ebearing, bear2bc;
            GetPositions(pos, bearing, sfac, out center, out bear2bc, out ec, out ebearing);

            // Stick results into return variables
            pos = ec;
            bearing = ebearing;
        }

        /// <summary>
        /// Observations defining the shape of the circular leg.
        /// </summary>
        internal CircularLegMetrics Metrics
        {
            get { return m_Metrics; }
        }

        /// <summary>
        /// Given the position of the start of this leg, along with an initial bearing, get
        /// other positions (and bearings) relating to the circle.
        /// </summary>
        /// <param name="bc">The position for the BC.</param>
        /// <param name="sbearing">The position for the BC.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        /// <param name="center">Position of the circle centre.</param>
        /// <param name="bear2bc">Bearing from the centre to the BC.</param>
        /// <param name="ec">Position of the EC.</param>
        /// <param name="ebearing">Exit bearing.</param>
        internal void GetPositions(IPosition bc, double sbearing, double sfac,
                            out IPosition center, out double bear2bc, out IPosition ec, out double ebearing)
        {
            // Have we got a cul-de-sac?
            bool cul = m_Metrics.IsCulDeSac;

            // Remember reverse bearing if we have a cul-de-sac.
            double revbearing = sbearing + Math.PI;

            //	Initialize current bearing.
            double bearing = sbearing;

            // Counter-clockwise?
            bool ccw = (m_Metrics.IsClockwise == false);

            // Get radius in meters on the ground (and scale it).
            double radius = RadiusInMeters * sfac;

            // Get to the center (should really get to the P.I., but not sure
            // where that is when the curve is > half a circle -- need to
            // check some book).

            if (cul)
            {
                double halfAngle = m_Metrics.CentralAngle * 0.5;

                if (ccw)
                    bearing -= halfAngle;
                else
                    bearing += halfAngle;
            }
            else
            {
                double entryAngle = m_Metrics.EntryAngle;

                if (ccw)
                    bearing -= (Math.PI - entryAngle);
                else
                    bearing += (Math.PI - entryAngle);
            }

            double dE = radius * Math.Sin(bearing);
            double dN = radius * Math.Cos(bearing);

            double x = bc.X + dE;
            double y = bc.Y + dN;
            center = new Position(x, y);

            // Return the bearing from the center to the BC (the reverse
            // of the bearing we just used).
            bear2bc = bearing + Math.PI;

            // Now go out to the EC. For regular curves, figure out the
            // central angle by comparing the observed length of the curve
            // to the total circumference.
            if (cul)
            {
                double ca = m_Metrics.CentralAngle;

                if (ccw)
                    bearing -= (Math.PI - ca);
                else
                    bearing += (Math.PI - ca);
            }
            else
            {
                double length = PrimaryFace.GetTotal() * sfac;
                double circumf = radius * MathConstants.PIMUL2;
                double ca = MathConstants.PIMUL2 * (length/circumf);

                if (ccw)
                    bearing += (Math.PI-ca);
                else
                    bearing -= (Math.PI-ca);
            }

            // Define the position of the EC.
            x += (radius * Math.Sin(bearing));
            y += (radius * Math.Cos(bearing));
            ec = new Position(x, y);

            // Define the exit bearing. For cul-de-sacs, the exit bearing
            // is the reverse of the original bearing (lines are parallel).

            if (cul)
                ebearing = revbearing;
            else
            {
                // If we have a second angle, use that
                double angle = m_Metrics.ExitAngle;

                if (ccw)
                    ebearing = bearing - (Math.PI-angle);
                else
                    ebearing = bearing + (Math.PI-angle);
            }
        }

        /// <summary>
        /// Calculates the exit bearing for this leg.
        /// </summary>
        /// <param name="bc">The position for the start of the leg.
        /// <param name="bcBearing">The bearing on entry into the leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        /// <returns>The bearing at the end of this leg (in radians).</returns>
        internal double GetExitBearing(IPosition bc, double bcBearing, double sfac)
        {
            IPosition center;
            IPosition ec;
            double bearingToBC;
            double ecBearing;
            GetPositions(bc, bcBearing, sfac, out center, out bearingToBC, out ec, out ecBearing);

            return ecBearing;
        }

        /// <summary>
        /// Obtains the geometry for spans along this leg.
        /// </summary>
        /// <param name="bc">The position for the start of the leg.
        /// <param name="bcBearing">The bearing on entry into the leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        /// <param name="spans">Information for the spans coinciding with this leg.</param>
        /// <returns>The sections along this leg</returns>
        internal override ILineGeometry[] GetSpanSections(IPosition bc, double bcBearing, double sfac, SpanInfo[] spans)
        {
            // Can't do anything if the leg radius isn't defined
            if (m_Metrics.ObservedRadius == null)
                throw new InvalidOperationException("Cannot create sections for circular leg with undefined radius");

            var result = new ILineGeometry[spans.Length];

            // Use supplied stuff to derive info on the center and EC.
            IPosition center;
            IPosition ec;
            double bearingToBC;
            double ecBearing;
            GetPositions(bc, bcBearing, sfac, out center, out bearingToBC, out ec, out ecBearing);

            // Define the underlying circle
            ICircleGeometry circle = new CircleGeometry(PointGeometry.Create(center), BasicGeom.Distance(center, bc));

            // Handle case where the leg is a cul-de-sac with no observed spans
            if (spans.Length == 1 && spans[0].ObservedDistance == null)
            {
                result[0] = new CircularArcGeometry(circle, bc, ec, m_Metrics.IsClockwise);
                return result;
            }

            /// Initialize scaling factor for distances in cul-de-sacs (ratio of the length calculated from
            /// the CA & Radius, versus the observed distances that were actually specified). For curves that
            /// are not cul-de-sacs, this will be 1.0
            double culFactor = 1.0;
            if (m_Metrics.IsCulDeSac)
            {
                double obsv = PrimaryFace.GetTotal();
                if (obsv > MathConstants.TINY)
                    culFactor = Length.Meters / obsv;
            }

            IPosition sPos = bc;
            IPosition ePos = null;
            bool isClockwise = m_Metrics.IsClockwise;
            double radius = RadiusInMeters;
            double edist = 0.0;

            for (int i = 0; i < result.Length; i++, sPos = ePos)
            {
                // Add on the unscaled distance
                edist += spans[i].ObservedDistance.Meters;

                // Get the angle subtended at the center of the circle. We use
                // unscaled values here, since the scale factors would cancel out.
                // However, we DO apply any cul-de-sac scaling factor, to account
                // for the fact that the distance may be inconsistent with the
                // curve length derived from the CA and radius. For example, it
                // is possible that the calculated curve length=200, although the
                // total of the observed spans is somehow only 100. In that case,
                // if the supplied distance is 50, we actually want to use a
                // value of 50 * (200/100) = 100.

                double angle = (edist * culFactor) / radius;

                // Get the bearing of the point with respect to the center of the circle.

                double bearing;

                if (isClockwise)
                    bearing = bearingToBC + angle;
                else
                    bearing = bearingToBC - angle;

                // Calculate the position using the scaled radius.
                ePos = Geom.Polar(center, bearing, radius * sfac);

                result[i] = new CircularArcGeometry(circle, sPos, ePos, isClockwise);
            }

            return result;
        }

        /// <summary>
        /// Obtains the geometry for spans along an alternate face attached to this leg.
        /// </summary>
        /// <param name="legStart">The position for the start of the leg.
        /// <param name="legEnd">The position for the end of the leg.</param>
        /// <param name="spans">Information for the spans coinciding with this leg.</param>
        /// <returns>The sections along this leg</returns>
        internal override ILineGeometry[] GetSpanSections(IPosition legStart, IPosition legEnd, SpanInfo[] spans)
        {
            var result = new ILineGeometry[spans.Length];

            Debug.Assert(AlternateFace != null);

            // Define the arc that corresponds to the complete leg (the circle should have been
            // already defined when we processed the primary face_.
            Debug.Assert(Circle != null);
            var arc = new CircularArcGeometry(Circle, legStart, legEnd, m_Metrics.IsClockwise);

            // Handle case where the leg is a cul-de-sac with no observed spans on the alternate face
            if (spans.Length == 1 && spans[0].ObservedDistance == null)
            {
                result[0] = arc;
                return result;
            }

            // Get the required arc length (in meters on the ground)
            double len = arc.Length.Meters;

            // Get the observed arc length (in meters on the ground)
            double obs = AlternateFace.GetTotal();

            // Get the adjustment factor for stretching-compressing the observed distances.
            double factor = len / obs;

            // Define start of first arc.
            IPosition sPos = legStart;
            IPosition ePos = null;

            // Haven't got anywhere yet.
            double totobs = 0.0;

            // Figure out the location of each span
            for (int i = 0; i < result.Length; i++, sPos = ePos)
            {
                if (i == result.Length - 1)
                {
                    ePos = legEnd;
                }
                else
                {
                    // Add on the unscaled distance
                    totobs += spans[i].ObservedDistance.Meters;

                    // Scale to the required length for the overall leg
                    double elen = totobs * factor;

                    // Define the end position.
                    arc.GetPosition(new Length(elen), out ePos);
                }

                result[i] = new CircularArcGeometry(Circle, sPos, ePos, m_Metrics.IsClockwise);
            }

            return result;
        }

        /// <summary>
        /// Gets the position of a point on the circular leg.
        /// </summary>
        /// <param name="dist">The (unscaled) distance to the desired point.</param>
        /// <returns>The position.</returns>
        IPosition GetPoint(double dist, IPosition center, double radius, double culFactor, double bearingToBC, double sfac)
        {
            // Get the angle subtended at the center of the circle. We use
            // unscaled values here, since the scale factors would cancel out.
            // However, we DO apply any cul-de-sac scaling factor, to account
            // for the fact that the distance may be inconsistent with the
            // curve length derived from the CA and radius. For example, it
            // is possible that the calculated curve length=200, although the
            // total of the observed spans is somehow only 100. In that case,
            // if the supplied distance is 50, we actually want to use a
            // value of 50 * (200/100) = 100.

            double angle = (dist * culFactor) / radius;

            // Get the bearing of the point with respect to the center of the circle.

            double bearing;

            if (m_Metrics.IsClockwise)
                bearing = bearingToBC + angle;
            else
                bearing = bearingToBC - angle;

            // Calculate the position using the scaled radius.
            return Geom.Polar(center, bearing, radius * sfac);
        }

        /// <summary>
        /// Creates a line feature that corresponds to one of the spans on this leg.
        /// Before calling this override, the circle object associated with this leg must
        /// be defined, via a call to <see cref="CreateCircle"/>.
        /// </summary>
        /// <param name="ff">The factory for creating new spatial features</param>
        /// <param name="itemName">The name for the item involved</param>
        /// <param name="from">The point at the start of the line (not null).</param>
        /// <param name="to">The point at the end of the line (not null).</param>
        /// <returns>The created line (never null)</returns>
        /// <exception cref="InvalidOperationException">If the underlying circle for this leg has not
        /// been created via a prior call to <see cref="CreateCircle"/>.</exception>
        internal override LineFeature CreateLine(FeatureFactory ff, string itemName, PointFeature from, PointFeature to)
        {
            if (m_Circle == null)
                throw new InvalidOperationException("Circle for circular arc has not been defined");

            ArcFeature result = ff.CreateArcFeature(itemName, from, to);

            // We have to create a geometry object at this stage, so that the circle can be
            // cross-referenced to created arcs. However, it's not fully defined because the
            // circle radius will likely be zero at this stage.
            result.Geometry = new ArcGeometry(m_Circle, from, to, m_Metrics.IsClockwise);

            return result;
        }

        /// <summary>
        /// Creates any circle that's required for arcs that sit on this leg. This method must
        /// be called before making any calls to <see cref="CreateLine"/>.
        /// </summary>
        /// <param name="ff">The factory for creating new spatial features</param>
        /// <param name="itemName">The name for the item that represents the point at the center of the circle</param>
        /// <returns>The created circle (with an undefined radius)</returns>
        internal Circle CreateCircle(FeatureFactory ff, string itemName)
        {
            // Create a center point, and cross-reference to a new circle (with undefined radius)
            PointFeature center = ff.CreatePointFeature(itemName);
            m_Circle = new Circle(center, 0.0);
            m_Circle.AddReferences();
            return m_Circle;
        }

        /// <summary>
        /// Rollforward this leg.
        /// </summary>
        /// <param name="insert">The point of the end of any new insert that
        /// immediately precedes this leg. This will be updated if this leg also
        /// ends with a new insert (if not, it will be returned as a null value).</param>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg.
        /// Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        /// <returns></returns>
        internal override bool Rollforward(ref PointFeature insert, PathOperation op,
            ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new NotImplementedException();
            /*
            // SS:20080314 - This looks like Save...

            // Create an undefined circular span
            CircularSpan span = new CircularSpan(this, terminal, bearing, sfac);

            // Create list for holding any newly created points
            List<PointFeature> createdPoints = new List<PointFeature>();

            // If this leg already has an associated circle, move it. Otherwise
            // add a circle to the map that corresponds to this leg.
            if (m_Circle == null)
                m_Circle = AddCircle(op, createdPoints, span);
            else
            {
                // Get the centre point associated with the current op. If there
                // is one (i.e. it's not a point that existed before the op), just
                // move it. Otherwise add a new circle (along with a new centre
                // point).

                // Inactive centre points are ok (if you don't search for
                // them, a new circle will be added).

                // 19-OCT-99: During rollforward, the op returned by SaveOp is
                // the op where rollforward started (not necessarily the op
                // that this leg belongs to). This probably needs to be changed
                // for other reasons, but for now, use the op that was supplied
                // (it was not previously supplied). If you don't do this, the
                // GetpCentre call will not find the centre point, even if it
                // was created by this leg, so it would always go to add a new
                // circle.

                //const CeOperation* const pop = CeMap::GetpMap()->SaveOp();
                //CePoint* pCentre = m_pCircle->GetpCentre(pop,FALSE);

                PointFeature center = m_Circle.CenterPoint;

                if (Object.ReferenceEquals(center.Creator, op))
                {
                    // Get the span to modify the radius of the circle.
                    SetCircle(span, m_Circle);

                    // Move the center point.
                    center.MovePoint(uc, span.Center);
                }
                else
                {
                    // The existing center point makes reference to the
                    // circle, so clean that up.
                    center.CutReference(m_Circle);

                    // 19-OCT-99: The AddCircle call just returns
                    // the circle that this leg already knows about,
                    // so clear it first.
                    m_Circle = null;

                    // Add a new circle.
                    m_Circle = AddCircle(op, createdPoints, span);
                }
            }

            // Create (or update) features for each span. Note that for
            // cul-de-sacs, there may be no observed spans.
            int nspan = Math.Max(1, this.Count);

            for (int i = 0; i < nspan; i++)
            {
                SaveSpan(ref insert, op, createdPoints, span, i, uc);
            }

            // Update BC info to refer to the EC.
            terminal = span.EC;
            bearing = span.ExitBearing;
            return true;
             */
        }

/*
//	@mfunc	Draw any angles for this leg.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the BC (may
//			be undefined).
//	@parm	The position of the start of the leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of the leg.
//	@parm	The view to draw in.
//	@parm	Display context for the draw.
//	@parm	The ground window for the draw (0 for no check).
//	@parm	The point the angle is directed to (0 if unknown).
//
//////////////////////////////////////////////////////////////////////

void CeCircularLeg::DrawAngles ( const CePoint* const pFrom
							   , const CeOperation& op
							   , const FLOAT8 sfac
							   , const CeVertex& bs
							   , const CeVertex& from
							   , FLOAT8& bearing
							   , CeVertex& to
							   , CeView* view
							   , CDC* pDC
							   , const CeWindow* const pWin ) const {

//	Get circle info
	CeVertex centre;
	CeVertex ec;
	FLOAT8 ebearing;
	FLOAT8 bear2bc;
	GetPositions ( from, bearing, sfac,
				   centre, bear2bc, ec, ebearing );

//	Stick results into return variables
	to = ec;
	bearing = ebearing;

//	Return if the window of the circle does not overlap
//	the display window.

	if ( pWin ) {
		const FLOAT8 xc = centre.GetEasting();
		const FLOAT8 yc = centre.GetNorthing();
		const FLOAT8 radius = this->GetRadius() * sfac;
		const CeVertex sw(xc-radius,yc-radius);
		const CeVertex ne(xc+radius,yc+radius);
		const CeWindow win(sw,ne);
		if ( !pWin->IsOverlap(win) ) return;
	}

//	Draw dotted lines to the centre of the circle.
	view->DrawDotted(from,centre,COL_BLACK);
	view->DrawDotted(centre,ec,COL_BLACK);

//	If we have a cul-de-sac, stick the central angle along
//	the bisector.

	if ( this->IsCuldesac() ) {

		// Return if a from point has been specified, but
		// it's not the centre point.
		if ( pFrom && GetpCentrePoint(op)!=pFrom ) return;

		view->DrawAngle(pDC,from,centre,ec,m_Angle1,FALSE);
	}
	else {

		// Return if a from point has been specified, but
		// it doesn't match either the BC or EC.
		LOGICAL drawBC = TRUE;
		LOGICAL drawEC = TRUE;
		FLOAT8 angle = 0.0;

		if ( pFrom ) {
			drawBC = (GetpStartPoint(op)==pFrom);
			drawEC = (GetpEndPoint(op)==pFrom);
		}

		// Draw the entry angle if it's not a right angle.
		if ( drawBC ) {
			angle = fabs(m_Angle1);
			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 )
				view->DrawAngle(pDC,bs,from,centre,angle,FALSE);
		}

		// Draw the exit angle too. We need to project to some
		// point that's an arbitrary distance along the exit
		// bearing.

		if ( drawEC ) {
			if ( m_Flag & FLG_TWOANGLES ) angle = fabs(m_Angle2);

			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 ) {
				CeVertex next(ec,ebearing,100.0);
				view->DrawAngle(pDC,centre,ec,next,angle,FALSE);
			}
		}
	}

} // end of DrawAngles

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw any angles for this leg.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the BC (may
//			be undefined).
//	@parm	The position of the start of the leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of the leg.
//	@parm	The thing we're drawing to.
//
//////////////////////////////////////////////////////////////////////

#include "CeDC.h"

void CeCircularLeg::DrawAngles ( const CePoint* const pFrom
							   , const CeOperation& op
							   , const FLOAT8 sfac
							   , const CeVertex& bs
							   , const CeVertex& from
							   , FLOAT8& bearing
							   , CeVertex& to
							   , CeDC& gdc ) const {

//	Get circle info
	CeVertex centre;
	CeVertex ec;
	FLOAT8 ebearing;
	FLOAT8 bear2bc;
	GetPositions ( from, bearing, sfac,
				   centre, bear2bc, ec, ebearing );

//	Stick results into return variables
	to = ec;
	bearing = ebearing;

//	Return if the window of the circle does not overlap
//	the display window.

	const CeWindow& drawin = gdc.GetWindow();

	if ( drawin.IsDefined() ) {
		const FLOAT8 xc = centre.GetEasting();
		const FLOAT8 yc = centre.GetNorthing();
		const FLOAT8 radius = this->GetRadius() * sfac;
		const CeVertex sw(xc-radius,yc-radius);
		const CeVertex ne(xc+radius,yc+radius);
		const CeWindow win(sw,ne);
		if ( !drawin.IsOverlap(win) ) return;
	}

//	Draw dotted lines to the centre of the circle.
	const CeStyle* const pStyle =
		GetpDraw()->GetBlackDottedLineStyle();
	gdc.SetLineStyle(*pStyle);
	gdc.Draw(from,centre);
	gdc.Draw(centre,ec);

//	If we have a cul-de-sac, stick the central angle along
//	the bisector.

	if ( this->IsCuldesac() ) {

		// Return if a from point has been specified, but
		// it's not the centre point.
		if ( pFrom && GetpCentrePoint(op)!=pFrom ) return;

		gdc.DrawAngle(from,centre,ec,m_Angle1,FALSE);
	}
	else {

		// Return if a from point has been specified, but
		// it doesn't match either the BC or EC.
		LOGICAL drawBC = TRUE;
		LOGICAL drawEC = TRUE;
		FLOAT8 angle = 0.0;

		if ( pFrom ) {
			drawBC = (GetpStartPoint(op)==pFrom);
			drawEC = (GetpEndPoint(op)==pFrom);
		}

		// Draw the entry angle if it's not a right angle.
		if ( drawBC ) {
			angle = fabs(m_Angle1);
			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 )
				gdc.DrawAngle(bs,from,centre,angle,FALSE);
		}

		// Draw the exit angle too. We need to project to some
		// point that's an arbitrary distance along the exit
		// bearing.

		if ( drawEC ) {
			if ( m_Flag & FLG_TWOANGLES ) angle = fabs(m_Angle2);

			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 ) {
				CeVertex next(ec,ebearing,100.0);
				gdc.DrawAngle(centre,ec,next,angle,FALSE);
			}
		}
	}

} // end of DrawAngles
        */

        /// <summary>
        /// The observed radius, in meters (0 if the radius is undefined).
        /// </summary>
        internal double RadiusInMeters
        {
            get
            {
                Distance d = m_Metrics.ObservedRadius;
                return (d == null ? 0.0 : d.Meters);
            }
        }

        /// <summary>
        /// Returns the position at the center of the circle that this leg lies on.
        /// </summary>
        internal override IPosition Center
        {
            get
            {
                if (m_Circle != null)
                    return m_Circle.Center;
                else
                    return null;
            }
        }

        /*
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Create transient CeMiscText objects for any observed
//			angles that are part of this leg.
//
//			The caller is responsible for deleting the text
//			objects once done with them.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the BC (may
//			be undefined).
//	@parm	The position of the start of the leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of the leg.
//	@parm	List of pointers to created text objects (appended to).
//	@parm	Should lines be produced too?
//
//	@rdesc	TRUE if the specified from point was encountered (this
//			does not necessarily mean that any text needed to be
//			generated for it).
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeCircularLeg::CreateAngleText ( const CePoint* const pFrom
									   , const CeOperation& op
									   , const FLOAT8 sfac
									   , const CeVertex& bs
									   , const CeVertex& from
									   , FLOAT8& bearing
									   , CeVertex& to
									   , CPtrList& text
									   , const LOGICAL wantLinesToo ) const {

	// Get circle info
	CeVertex centre;
	CeVertex ec;
	FLOAT8 ebearing;
	FLOAT8 bear2bc;
	GetPositions ( from, bearing, sfac,
				   centre, bear2bc, ec, ebearing );

	// Stick results into return variables
	to = ec;
	bearing = ebearing;

	// If we have a cul-de-sac, stick the central angle along
	// the bisector.

	if ( this->IsCuldesac() ) {

		// Return if a from point has been specified, but
		// it's not the centre point.
		if ( pFrom && GetpCentrePoint(op)!=pFrom ) return FALSE;

		// Generate the text.
		MakeText(from,centre,ec,m_Angle1,text);

		// If a from-point was specified, and we found it,
		// indicate that it has now been handled.
		return (pFrom!=0);
	}
	else {

		// Return if a from point has been specified, but
		// it doesn't match either the BC or EC.
		LOGICAL drawBC = TRUE;
		LOGICAL drawEC = TRUE;
		FLOAT8 angle = 0.0;

		if ( pFrom ) {
			drawBC = (GetpStartPoint(op)==pFrom);
			drawEC = (GetpEndPoint(op)==pFrom);
		}

		// Draw the entry angle if it's not a right angle.
		if ( drawBC ) {
			angle = fabs(m_Angle1);
			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 )
					MakeText(bs,from,centre,m_Angle1,text);
		}

		// Draw the exit angle too. We need to project to some
		// point that's an arbitrary distance along the exit
		// bearing.

		if ( drawEC ) {
			if ( m_Flag & FLG_TWOANGLES ) angle = fabs(m_Angle2);

			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 ) {
				CeVertex next(ec,ebearing,100.0);
				MakeText(centre,ec,next,m_Angle2,text);
			}
		}

		// If a from-point was specified, and we found it,
		// indicate that it has now been handled.
		return (pFrom && (drawBC || drawEC));
	}

} // end of CreateAngleText
        */

        /// <summary>
        /// Generates a string that represents the definition of this leg
        /// </summary>
        /// <param name="defaultEntryUnit">The distance units that should be treated as the default.
        /// Formatted distances that were specified using these units will not contain the units
        /// abbreviation</param>
        /// <returns>A formatted representation of this leg</returns>
        /*
        internal override string GetDataString(DistanceUnit defaultEntryUnit)
        {
            StringBuilder sb = new StringBuilder();

            // Initial angle
            sb.Append("(");
            sb.Append(RadianValue.AsShortString(m_Angle1));

            // If it's a cul-de-sac, just append the "CA" characters.
            // Otherwise we could have an exit angle as well.

            if (IsCulDeSac)
                sb.Append("ca ");
            else
            {
                if (IsTwoAngles)
                {
                    sb.Append(" ");
                    sb.Append(RadianValue.AsShortString(m_Angle2));
                }

                sb.Append(" ");
            }

            // The observed radius.
            sb.Append(m_Radius.Format());

            // Is it counter-clockwise?
            if (!IsClockwise)
                sb.Append(" cc");

            // Append any observed distances if there are any.
            if (Count > 0)
            {
                sb.Append("/");
                AddToString(sb, defaultEntryUnit);
            }

            sb.Append(")");

            return sb.ToString();
        }
        */

        /*
        /// <summary>
        /// Records the circle for this leg. This also ensures that the circle has the
        /// correct radius. However, it does NOT alter the circle's center position, since
        /// that is dependent on higher level stuff (see <see cref="CircularLeg.Save"/>).
        /// </summary>
        /// <param name="circle">The circle for this leg (may be null).</param>
        internal void SetCircle(CircularSpan span, Circle circle)
        {
            // Remember the circle.
            m_Circle = circle;

            // Ensure the radius is correct.
            if (m_Circle!=null)
                m_Circle.Radius = span.ScaledRadius;
        }
        */

        /// <summary>
        /// Loads a list of the features that were created by this leg.
        /// </summary>
        /// <param name="op">The operation that this leg relates to.</param>
        /// <param name="flist">The list to store the results. This list will be
        /// appended to, so you may want to clear the list prior to call.</param>
        /// <remarks>The <see cref="CircularLeg"/> provides an override that
        /// is responsible for appending the center point.</remarks>
        internal override void GetFeatures(Operation op, List<Feature> flist)
        {
            if (m_Circle != null)
                flist.Add(m_Circle.CenterPoint);

            base.GetFeatures(op, flist);
        }
    }
}
