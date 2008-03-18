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
using System.Text;

using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeCircularLeg" />
    /// <summary>
    /// A circular leg in a connection path.
    /// </summary>
    [Serializable]
    class CircularLeg : Leg
    {
        #region Class data

        /// <summary>
        /// First angle. Either at the BC, or a central angle. In radians. It's
        /// a central angle if the <see cref="IsCulDeSac"/> property is true.
        /// </summary>
        double m_Angle1;

        /// <summary>
        /// The angle at the EC (in radians). This will only be defined if the FLG_TWOANGLES
        /// flag bit is set (if not set, this value will be 0.0).
        /// </summary>
        double m_Angle2;

        /// <summary>
        /// Observed radius.
        /// </summary>
        Distance m_Radius;

        /// <summary>
        /// The circle that this leg sits on.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Flag bits
        /// </summary>
        CircularLegFlag m_Flag;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        CircularLeg()
            : base(0)
        {
            m_Angle1 = 0.0;
            m_Angle2 = 0.0;
            m_Radius = null;
            m_Circle = null;
            m_Flag = 0;
        }

        /// <summary>
        /// Creates a new <c>CircularLeg</c> with no spans.
        /// </summary>
        /// <param name="radius">The radius of the circular leg.</param>
        /// <param name="clockwise">True if the curve is clockwise.</param>
        /// <param name="span">The number of spans on the curve.</param>
        internal CircularLeg(Distance radius, bool clockwise, int nspan)
            : base(nspan)
        {
            // Angles were not specified.
            m_Angle1 = 0.0;
            m_Angle2 = 0.0;

            // Start out with undefined flag.
            m_Flag = 0;

            // Remember the radius.
            m_Radius = radius;

            // The circle for this leg won't be known till we create a span.
            m_Circle = null;

            // Remember if its NOT a clockwise curve.
            if (!clockwise)
                m_Flag |= CircularLegFlag.CounterClockwise;
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
        /// Observed radius.
        /// </summary>
        internal Distance ObservedRadius
        {
            get { return m_Radius; }
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
                if (IsCulDeSac)
                {
                    double radius = m_Radius.Meters;
                    return new Length((MathConstants.PIMUL2 - m_Angle1) * radius);
                }
                else
                    return new Length(base.GetTotal());
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
            bool cul = IsCulDeSac;

            // Remember reverse bearing if we have a cul-de-sac.
            double revbearing = sbearing + Math.PI;

            //	Initialize current bearing.
            double bearing = sbearing;

            // Counter-clockwise?
            bool ccw = (m_Flag & CircularLegFlag.CounterClockwise)!=0;

            // Get radius in meters on the ground (and scale it).
            double radius = m_Radius.Meters * sfac;

            // Get to the center (should really get to the P.I., but not sure
            // where that is when the curve is > half a circle -- need to
            // check some book).

            if (cul)
            {
                if (ccw)
                    bearing -= (m_Angle1*0.5);
                else
                    bearing += (m_Angle1*0.5);
            }
            else
            {
                if (ccw)
                    bearing -= (Math.PI-m_Angle1);
                else
                    bearing += (Math.PI-m_Angle1);
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
                if (ccw)
                    bearing -= (Math.PI-m_Angle1);
                else
                    bearing += (Math.PI-m_Angle1);
            }
            else
            {
                double length = GetTotal() * sfac;
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
                double angle;
                if (IsTwoAngles)
                    angle = m_Angle2;
                else
                    angle = m_Angle1;

                if (ccw)
                    ebearing = bearing - (Math.PI-angle);
                else
                    ebearing = bearing + (Math.PI-angle);
            }
        }

        /// <summary>
        /// Draws this leg
        /// </summary>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated
        /// for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        internal override void Draw(ref IPosition pos, ref double bearing, double sfac)
        {
            //	Create an undefined circular span
            CircularSpan span = new CircularSpan(this, pos, bearing, sfac);

            // Draw each visible span in turn. Note that for cul-de-sacs, there may be
            // no observed spans.

            int nspan = this.Count;

            if (nspan==0)
            {
                span.Get(0);
                span.Draw();
            }
            else
            {
                for (int i = 0; i < nspan; i++)
                {
                    span.Get(i);
                    span.Draw();
                }
            }

            // Update BC info to refer to the EC.
            pos = span.EC;
            bearing = span.ExitBearing;
        }

        /// <summary>
        /// Draws a previously saved leg.
        /// </summary>
        /// <param name="preview">True if the path should be drawn in preview
        /// mode (i.e. in the normal construction colour, with miss-connects
        /// shown as dotted lines).</param>
        internal override void Draw(bool preview)
        {
            // Identical to StraightLeg.Draw...

            EditingController ec = EditingController.Current;
            ISpatialDisplay display = ec.ActiveDisplay;
            IDrawStyle style = ec.DrawStyle;

            int nfeat = this.Count;

            for (int i = 0; i < nfeat; i++)
            {
                Feature feat = GetFeature(i);
                if (feat != null)
                {
                    if (preview)
                        feat.Draw(display, Color.Magenta);
                    else
                        feat.Render(display, style);
                }
            }
        }

        /// <summary>
        /// Saves features for this leg.
        /// </summary>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg.
        /// Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        internal override void Save(PathOperation op, ref IPosition terminal, ref double bearing, double sfac)
        {
            // Create an undefined circular span
            CircularSpan span = new CircularSpan(this, terminal, bearing, sfac);

            // If this leg already has an associated circle, move it. Otherwise
            // add a circle to the map that corresponds to this leg.
            if (m_Circle == null)
                m_Circle = span.AddCircle(op);
            else
            {
                // Get the center point associated with the current op. If there
                // is one (i.e. it's not a point that existed before the op), just
                // move it. Otherwise add a new circle (along with a new center
                // point).

                // Inactive center points are ok (if you don't search for
                // them, a new circle will be added).

                PointFeature center = m_Circle.CenterPoint;

                if (Object.ReferenceEquals(center.Creator, op))
                {
                    // Get the span to modify the radius of the circle.
                    span.SetCircle(m_Circle);

                    // Move the center point.
                    center.Move(span.Center);
                }
                else
                {
                    // The existing center point makes reference to the
                    // circle, so clean that up.
                    center.CutReference(m_Circle);

                    // 19-OCT-99: The span.AddCircle call just returns
                    // the circle that this leg already knows about! (the
                    // span picked it up via it's constructor). So add the
                    // new circle explicitly here & let the span know about
                    // it.
                    span.SetCircle(null);

                    // Add a new circle.
                    m_Circle = span.AddCircle(op);
                }
            }

            // Create (or update) features for each span. Note that for
            // cul-de-sacs, there may be no observed spans.
            int nspan = Math.Max(1, this.Count);
            PointFeature noInsert = null;

            for (int i = 0; i < nspan; i++)
                SaveSpan(ref noInsert, op, span, i);

            // Update BC info to refer to the EC.
            terminal = span.EC;
            bearing = span.ExitBearing;
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
            // SS:20080314 - This looks like Save...

            // Create an undefined circular span
            CircularSpan span = new CircularSpan(this, terminal, bearing, sfac);

            // If this leg already has an associated circle, move it. Otherwise
            // add a circle to the map that corresponds to this leg.
            if (m_Circle == null)
                m_Circle = span.AddCircle(op);
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
                    span.SetCircle(m_Circle);

                    // Move the center point.
                    center.Move(span.Center);
                }
                else
                {
                    // The existing center point makes reference to the
                    // circle, so clean that up.
                    center.CutReference(m_Circle);

                    // 19-OCT-99: The span.AddCircle call just returns
                    // the circle that this leg already knows about! (the
                    // span picked it up via it's constructor). So add the
                    // new circle explicitly here & let the span know about
                    // it.
                    span.SetCircle(null);

                    // Add a new circle.
                    m_Circle = span.AddCircle(op);
                }
            }

            // Create (or update) features for each span. Note that for
            // cul-de-sacs, there may be no observed spans.
            int nspan = Math.Max(1, this.Count);

            for (int i = 0; i < nspan; i++)
                SaveSpan(ref insert, op, span, i);

            // Update BC info to refer to the EC.
            terminal = span.EC;
            bearing = span.ExitBearing;
            return true;
        }

        /// <summary>
        /// Saves a specific span of this leg.
        /// </summary>
        /// <param name="insert">The point of the end of any new insert that
        /// immediately precedes this span. This will be updated if this span is
        /// also a new insert (if not, it will be returned as a null value).</param>
        /// <param name="op">The connection path that this span is part of.</param>
        /// <param name="span">The span for the leg.</param>
        /// <param name="index">The index of the span to save.</param>
        void SaveSpan(ref PointFeature insert, PathOperation op, CircularSpan span, int index)
        {
            // The very end of a connection path should never be moved.
            PointFeature veryEnd = op.EndPoint;

            if (IsNewSpan(index))
            {
                // Is this the very last span in the connection path?
                int nspan = Math.Max(1, this.Count);
                bool isLast = (index==(nspan-1) && op.IsLastLeg(this));

                // Save the insert.
                LineFeature newLine = span.SaveInsert(index, op, isLast);

                // Record the new line as part of this leg
                AddNewSpan(index, newLine);

                // Remember the last insert position.
                insert = newLine.EndPoint;
            }
            else
            {
                // Get the span to save
                span.Get(index);

                // See if the span previously had a saved feature.
                Feature old = GetFeature(index);

                // Save the span.
                Feature feat = span.Save(op, insert, old, veryEnd);

                // If the saved span is different from what we had before,
                // tell the base class about it.
                if (!Object.ReferenceEquals(feat, old))
                    SetFeature(index, feat);

                // That wasn't an insert.
                insert = null;
            }
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
        /// The central angle for this leg (assuming the
        /// <see cref="IsCulDeSac"/> property is true)
        /// </summary>
        double CentralAngle
        {
            get { return m_Angle1; }
        }

        /// <summary>
        /// The entry angle for this leg (assuming it isn't a cul-de-sac -
        /// see the <see cref="IsCulDeSac"/> property)
        /// </summary>
        double EntryAngle
        {
            get { return m_Angle1; }
        }

        /// <summary>
        /// The exit angle for this leg (assuming it isn't a cul-de-sac -
        /// see the <see cref="IsCulDeSac"/> property)
        /// </summary>
        double ExitAngle
        {
            get
            {
                if (IsTwoAngles)
                    return m_Angle2;
                else
                    return m_Angle1;
            }
        }

        /// <summary>
        /// Defines this leg using the info in another leg. This does NOT
        /// touch the base class in any way.
        /// </summary>
        /// <param name="master"></param>
        void Define(CircularLeg master)
        {
            m_Angle1 = master.m_Angle1;
            m_Angle2 = master.m_Angle2;
            m_Radius = master.m_Radius;
            m_Flag = master.m_Flag;
            m_Circle = master.m_Circle;
        }

        /// <summary>
        /// Is this leg has been defined?. This just confirms that the
        /// radius is defined.
        /// </summary>
        bool IsDefined
        {
            get { return (m_Radius!=null && m_Radius.IsDefined); }
        }

        /// <summary>
        /// Is the leg directed clockwise?
        /// </summary>
        internal bool IsClockwise
        {
            get { return (m_Flag & CircularLegFlag.CounterClockwise) == 0; }
            set
            {
                // SS:20080309 - Don't know why the following was done...

                // Assume clockwise by clearing the flag bit.
                SetFlag(CircularLegFlag.CounterClockwise, false);

                // Set bit if NOT clockwise.
                if (value == false)
                    SetFlag(CircularLegFlag.CounterClockwise, true);
            }
        }

        /// <summary>
        /// Records the radius of this leg.
        /// </summary>
        /// <param name="radius">The radius to assign.</param>
        void SetRadius(Distance radius)
        {
            m_Radius = radius;
        }

        /// <summary>
        /// The observed radius, in meters
        /// </summary>
        internal double Radius
        {
            get { return (m_Radius == null ? 0.0 : m_Radius.Meters); }
        }

        /// <summary>
        /// Sets the entry (BC) angle. Note that when setting both the entry
        /// and exit angles, this function should be called BEFORE a call to
        /// <see cref="SetExitAngle"/>. This function should NOT be called
        /// for cul-de-sacs (use <see cref="SetCentralAngle"/> instead).
        /// </summary>
        /// <param name="bangle">The angle to assign, in radians.</param>
        internal void SetEntryAngle(double bangle)
        {
            // Store the specified angle.
            m_Angle1 = bangle;

            // This leg is not a cul-de-sac.
            IsCulDeSac = false;
        }

        /// <summary>
        /// Sets the exit (BC) angle. Note that when setting both the entry and
        /// exit angles, this function should be called AFTER a call to
        /// <see cref="SetEntryAngle"/>. This function should NOT be called
        /// for cul-de-sacs (use <see cref="SetCentralAngle"/> instead).
        /// </summary>
        /// <param name="eangle">The angle to assign, in radians.</param>
        internal void SetExitAngle(double eangle)
        {
            // If the angle is the same as the entry angle, store an
            // undefined exit angle, and set the flag bit to indicate
            // that only the entry angle is valid.

            if (Math.Abs(m_Angle1 - eangle) < MathConstants.TINY)
            {
                m_Angle2 = 0.0;
                IsTwoAngles = false;
            }
            else
            {
                m_Angle2 = eangle;
                IsTwoAngles = true;
            }

            // This leg is not a cul-de-sac.
            IsCulDeSac = false;
        }

        /// <summary>
        /// Sets the central angle for this leg. The leg will be flagged
        /// as being a cul-de-sac. 
        /// </summary>
        /// <param name="cangle">The central angle, in radians.</param>
        internal void SetCentralAngle(double cangle)
        {
            // Store the central angle.
            m_Angle1 = cangle;

            // The other angle is unused.
            m_Angle2 = 0.0;
            IsTwoAngles = false;

            // This leg is a cul-de-sac
            IsCulDeSac = true;
        }

        /// <summary>
        /// Is the leg flagged as a cul-de-sac?
        /// </summary>
        internal bool IsCulDeSac
        {
            get { return (m_Flag & CircularLegFlag.CulDeSac) != 0; }
            private set { SetFlag(CircularLegFlag.CulDeSac, value); }
        }

        /// <summary>
        /// Does this leg have two angles?
        /// </summary>
        bool IsTwoAngles
        {
            get { return (m_Flag & CircularLegFlag.TwoAngles) != 0; }
            set { SetFlag(CircularLegFlag.TwoAngles, value); }
        }

        /// <summary>
        /// Sets flag bit(s)
        /// </summary>
        /// <param name="flag">The flag bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        void SetFlag(CircularLegFlag flag, bool setting)
        {
            if (setting)
                m_Flag |= flag;
            else
                m_Flag &= (~flag);
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
        /// Defines a string with the observations that make up this leg.
        /// </summary>
        internal override string DataString
        {
            get
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
                    AddToString(sb);
                }

                sb.Append(")");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Saves features for a second face that is based on this leg.
        /// </summary>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="face">The extra face to create features for.</param>
        /// <returns>True if created ok.</returns>
        internal override bool SaveFace(PathOperation op, ExtraLeg face)
        {
            // Get the terminal positions for this leg.
            IPosition spos, epos;
            if (!op.GetLegEnds(this, out spos, out epos))
                return false;

            return face.MakeCurves(op, spos, epos, m_Circle, IsClockwise);
        }

        /// <summary>
        /// Rollforward the second face of this leg.
        /// </summary>
        /// <param name="insert">The point of the end of any new insert that immediately precedes
        /// this leg. This will be updated if this leg also ends with a new insert (if not, it
        /// will be returned as a null value).</param>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="face">The second face.</param>
        /// <param name="spos">The new position for the start of this leg.</param>
        /// <param name="epos">The new position for the end of this leg.</param>
        /// <returns>True if rolled forward ok.</returns>
        /// <remarks>
        /// The start and end positions passed in should correspond to where THIS leg currently ends.
        /// They are passed in because this leg may contain miss-connects (and maybe even missing
        /// end points). So it would be tricky trying trying to work it out now.
        /// </remarks>
        internal override bool RollforwardFace(ref IPointGeometry insert, PathOperation op, ExtraLeg face, IPosition spos, IPosition epos)
        {
            // Get the extra face to do it.
            return face.UpdateCurves(insert, op, spos, epos, m_Circle, IsClockwise);
        }
    }
}
