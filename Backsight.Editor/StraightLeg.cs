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
using System.Drawing;

using Backsight.Editor.Operations;
using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeStraightLeg" />
    /// <summary>
    /// A straight leg in a connection path.
    /// </summary>
    class StraightLeg : Leg
    {
        #region Class data

        /// <summary>
        /// Angle at the start of the leg (signed). 
        /// </summary>
        double m_StartAngle;

        /// <summary>
        /// Is the angle at the start of this a deflection?
        /// </summary>
        bool m_IsDeflection;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>StraightLeg</c>
        /// </summary>
        /// <param name="nspan">The number of spans for the leg.</param>
        internal StraightLeg(int nspan)
            : base(nspan)
        {
            m_StartAngle = 0.0;
            m_IsDeflection = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StraightLeg"/> class that corresponds to
        /// the end of a face on another leg (for use when breaking a leg).
        /// </summary>
        /// <param name="face">The face on the other leg</param>
        /// <param name="startIndex">The array index of the first span that should be copied.</param>
        StraightLeg(LegFace face, int startIndex)
            : base(face, startIndex)
        {
            // Stick in a (clockwise) angle of 180 degrees.
            m_StartAngle = Math.PI;
            m_IsDeflection = false;
        }

        #endregion

        /// <summary>
        /// Angle at the start of the leg (signed). 
        /// </summary>
        internal double StartAngle
        {
            get { return m_StartAngle; }
            set { m_StartAngle = value; }
        }

        internal override Circle Circle
        {
            get { return null; }
        }

        internal override IPosition Center
        {
            get { return null; }
        }

        /// <summary>
        /// The total observed length of this leg
        /// </summary>
        internal override ILength Length
        {
            get { return new Length(PrimaryFace.GetTotal()); }
        }

        /// <summary>
        /// Given the position of the start of this leg, along with an initial bearing,
        /// project the end of the leg, along with an exit bearing.
        /// </summary>
        /// <param name="pos">The position at the start of the leg.</param>
        /// <param name="bearing">The initial bearing (e.g. if the previous leg was also
        /// a straight leg from A to B, the bearing is from A through B).</param>
        /// <param name="sfac">Scaling factor to apply. Default=1.0</param>
        internal override void Project(ref IPosition pos, ref double bearing, double sfac)
        {
            // Add on any initial angle
            bearing = AddStartAngle(bearing);

            // Get the total length of the leg.
            double length = PrimaryFace.GetTotal() * sfac;

            // Figure out shifts.
            double dE = length * Math.Sin(bearing);
            double dN = length * Math.Cos(bearing);

            // Define the end position.
            pos = new Position(pos.X + dE, pos.Y + dN);
        }

        /// <summary>
        /// Obtains the geometry for spans along this leg.
        /// </summary>
        /// <param name="pos">The position for the start of the leg.
        /// <param name="bearing">The bearing of the leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        /// <param name="spans">Information for the spans coinciding with this leg.</param>
        /// <returns>The sections along this leg</returns>
        internal override ILineGeometry[] GetSpanSections(IPosition pos, double bearing, double sfac, SpanInfo[] spans)
        {
            var result = new ILineGeometry[spans.Length];

            // A leg with just one span, but no observed distance is due to the fact that the Leg constructor
            // that accepts a span count will always produce an array with at least one span (this covers cul-de-sacs
            // defined only with a central angle). May be better to handle it there.
            if (spans.Length == 1 && spans[0].ObservedDistance == null)
            {
                result[0] = new LineSegmentGeometry(pos, pos);
                return result;
            }

            double sinBearing = Math.Sin(bearing);
            double cosBearing = Math.Cos(bearing);

            IPosition sPos = pos;
            IPosition ePos = null;

            double edist = 0.0;

            for (int i = 0; i < result.Length; i++, sPos=ePos)
            {
                edist += (spans[i].ObservedDistance.Meters * sfac);
                ePos = new Position(pos.X + (edist * sinBearing), pos.Y + (edist * cosBearing));
                result[i] = new LineSegmentGeometry(sPos, ePos);
            }

            return result;
        }

        /*
        /// <summary>
        /// Draws a previously saved leg.
        /// </summary>
        /// <param name="preview">True if the path should be drawn in preview
        /// mode (i.e. in the normal construction colour, with miss-connects
        /// shown as dotted lines).</param>
        internal override void Draw(bool preview)
        {
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
        */

        //internal override void Save(FeatureFactory ff, List<PointFeature> createdPoints,
        //                            ref IPosition terminal, ref double bearing, double sfac)
        //{
        //    // Add on any initial angle (it may be a deflection).
        //    if (Math.Abs(m_StartAngle) > MathConstants.TINY)
        //    {
        //        if (m_IsDeflection)
        //            bearing += m_StartAngle;
        //        else
        //            bearing += (m_StartAngle-Math.PI);
        //    }

        //    // Create a straight span
        //    StraightSpan span = new StraightSpan(this, terminal, bearing, sfac);

        //    int nspan = this.Count;
        //    for (int i = 0; i < nspan; i++)
        //    {
        //        // Get info for the current span (this defines the
        //        // adjusted start and end positions, among other things).
        //        span.Get(i);

        //        // Save the span
        //        Feature feat = SaveSpan(span, ff, createdPoints, null, null, null, null);
        //        SetFeature(i, feat);
        //    }

        //    // Return the end position of the last span.
        //    terminal = span.End;
        //}

        /// <summary>
        /// Creates a line feature that corresponds to one of the spans on this leg.
        /// </summary>
        /// <param name="ff">The factory for creating new spatial features</param>
        /// <param name="itemName">The name for the item involved</param>
        /// <param name="from">The point at the start of the line (not null).</param>
        /// <param name="to">The point at the end of the line (not null).</param>
        /// <returns>The created line (never null)</returns>
        internal override LineFeature CreateLine(FeatureFactory ff, string itemName, PointFeature from, PointFeature to)
        {
            return ff.CreateSegmentLineFeature(itemName, from, to);
        }

        /// <summary>
        /// Adds on any angle at the start of this leg.
        /// </summary>
        /// <param name="bearing">The bearing at the end of the preceding leg.</param>
        /// <returns>The bearing of this leg (in radians)</returns>
        internal double AddStartAngle(double bearing)
        {
            if (Math.Abs(m_StartAngle) < MathConstants.TINY)
                return bearing;

            if (m_IsDeflection)
                return bearing + m_StartAngle;
            else
                return bearing + m_StartAngle - Math.PI;
        }

        /*
        /// <summary>
        /// Defines the geometry for this leg.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated</param>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        internal override void CreateGeometry(EditingContext ctx, ref IPosition terminal, ref double bearing, double sfac)
        {
            // Add on any initial angle (it may be a deflection).
            bearing = AddStartAngle(bearing);

            // Create a straight span
            StraightSpan span = new StraightSpan(terminal, bearing, sfac);

            int nspan = this.Count;
            for (int i = 0; i < nspan; i++)
            {
                // Get info for the current span (this defines the
                // adjusted start and end positions, among other things).
                span.Get(this, i);

                // Create the geometry for the point at the end of the span
                SpanInfo data = GetSpanData(i);
                Feature feat = data.CreatedFeature;
                PointFeature endPoint = null;

                if (feat is PointFeature)
                    endPoint = (PointFeature)feat;
                else if (feat is LineFeature)
                    endPoint = (feat as LineFeature).EndPoint;

                if (endPoint != null && endPoint.PointGeometry == null)
                    endPoint.ApplyPointGeometry(ctx, PointGeometry.Create(span.End));
            }

            // Return the end position of the last span.
            terminal = span.End;
        }
        */

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
            // Add on any initial angle (it may be a deflection).
            if (Math.Abs(m_StartAngle) > MathConstants.TINY)
            {
                if (m_IsDeflection)
                    bearing += m_StartAngle;
                else
                    bearing += (m_StartAngle-Math.PI);
            }

            // Create a straight span
            StraightSpan span = new StraightSpan(this, terminal, bearing, sfac);

            // The very end of a connection path should never be moved.
            PointFeature veryEnd = op.EndPoint;

            // Create list for holding any newly created points
            List<PointFeature> createdPoints = new List<PointFeature>();

            int nspan = this.Count;
            for (int i=0; i<nspan; i++)
            {
                // Get info for the current span (this defines the
                // adjusted start and end positions, among other things).
                span.Get(i);

                // If we've got a newly inserted span
                if (IsNewSpan(i))
                {
                    bool isLast = (i==(nspan-1) && op.IsLastLeg(this));
                    LineFeature newLine = SaveInsert(span, i, op, isLast);
                    AddNewSpan(i, newLine);
                    insert = newLine.EndPoint;
                }
                else
                {
                    // See if the span previously had a saved feature.
                    Feature old = GetFeature(i);
                    if (old!=null)
                        SaveSpan(span, op, createdPoints, insert, old, veryEnd, uc);
                    else
                    {
                        Feature feat = SaveSpan(span, op, createdPoints, insert, null, veryEnd, uc);
                        SetFeature(i, feat);
                    }

                    // That wasn't an insert.
                    insert = null;
                }
            }

            // Return the end position of the last span.
            terminal = span.End;
            return true;
             */
        }

        /*
        PointFeature EnsurePointExists(FeatureFactory ff, IPointGeometry pg, List<PointFeature> createdPoints, PointFeature veryEnd)
        {
            // If the position coincides with the very last point in a connection path,
            // just return that point.
            if (pg.IsCoincident(veryEnd))
                return veryEnd;

            // If the position coincides with any of the points that have already been
            // created,... what to do? If the point is re-used, but the end points of
            // the connection path are later moved, intermediate points may no longer
            // coincide. However, if we add a point always, any topology may well get
            // screwed up (there will be confusion about which point to reference the
            // incident lines). In aiming for a solution, bear in mind that the position
            // of the points may well be unknown - with the exception of the very last
            // point in a connection path, a new feature WILL be required for all intermediate
            // points. The method that ultimately calculates the geometry needs to relate
            // coincident points to a single underlying geometry. In a situation where
            // things subsequently move, the shared geometry can be changed without altering
            // the spatial features involved.

            //ff.CreatePointFeature();
            return null;
        }
        */

        /*
        /// <summary>
        /// Saves a newly inserted span.
        /// </summary>
        /// <param name="index">The index of the new span.</param>
        /// <param name="creator">The operation that the new span should be referred to.</param>
        /// <param name="isLast">Is the new span going to be the very last span in the last
        /// leg of a connection path?</param>
        /// <returns>The line that was created.</returns>
        LineFeature SaveInsert(StraightSpan span, int index, PathOperation creator, bool isLast)
        {
            // Get the end positions for the new span.
            span.Get(this, index);

            // Make sure the start and end points have been rounded to
            // the internal resolution.
            IPointGeometry sloc = PointGeometry.Create(span.Start);
            IPointGeometry eloc = PointGeometry.Create(span.End);

            // Get the location at the start of the span (in most cases,
            // it should be there already -- the only exception is a
            // case where the point was omitted).
            CadastralMapModel map = CadastralMapModel.Current;
            PointFeature pS = map.EnsurePointExists(sloc, creator);

            // If the insert is going to be the very last span in the
            // enclosing connection path, just pick up the terminal
            // location of the path.
            PointFeature pE = null;

            if (isLast)
            {
                // Pick up the end of the path.
                pE = creator.EndPoint;

                // And ensure there has been no roundoff in the end position.
                eloc = pE;
            }
            else
            {
                // Add a point at the end of the span. Do NOT attempt to re-use any existing
                // point that happens to fall there. If you did, we could be re-using a location
                // that comes later in the connection path (i.e. it may later be moved again!).
                pE = map.AddPoint(eloc, map.DefaultPointType, creator);
                span.HasEndPoint = true;

                // Assign the next available ID to the point
                pE.SetNextId();
            }

            // Add a line.
            span.HasLine = true;
            return map.AddLine(pS, pE, map.DefaultLineType, creator);
        }
        */

        /*
//	@mfunc	Draw any angles for this leg.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the start of
//			the leg (may be undefined).
//	@parm	The position of the start of this leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of this leg.
//	@parm	The view to draw in.
//	@parm	Display context for the draw.
//	@parm	The ground window for the draw (0 for no check).
//
//////////////////////////////////////////////////////////////////////

void CeStraightLeg::DrawAngles ( const CePoint* const pFrom
							   , const CeOperation& op
							   , const FLOAT8 sfac
							   , const CeVertex& bs
							   , const CeVertex& from
							   , FLOAT8& bearing
							   , CeVertex& to
							   , CeView* view
							   , CDC* pDC
							   , const CeWindow* const pWin ) const {

//	Project to the end of the leg.
	to = from;
	this->Project(to,bearing,sfac);

	// If a from point has been specified, and this leg does
	// not start at it, just return.
	if ( pFrom && GetpStartPoint(op)!=pFrom ) return;

//	Return if this leg does not have an initial angle.
	const FLOAT8 angle = fabs(m_StartAngle);
	if ( angle<TINY ) return;

//	Return if the angle is real-close to a right angle
	if ( fabs(angle-PIDIV2  )<0.000001 ||
		 fabs(angle-PIMUL1P5)<0.000001 ) return;

//	Return if the from-point is not visible.
	if ( pWin && !pWin->IsOverlap(from) ) return;

	// Turn it over to the view, remembering that the angle may
	// in fact be a deflection.
	view->DrawAngle(pDC,bs,from,to,angle,IsDeflection());

} // end of DrawAngles

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw any angles for this leg.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the start of
//			the leg (may be undefined).
//	@parm	The position of the start of this leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of this leg.
//	@parm	The thing we're drawing to.

void CeStraightLeg::DrawAngles ( const CePoint* const pFrom
							   , const CeOperation& op
							   , const FLOAT8 sfac
							   , const CeVertex& bs
							   , const CeVertex& from
							   , FLOAT8& bearing
							   , CeVertex& to
							   , CeDC& gdc ) const {

//	Project to the end of the leg.
	to = from;
	this->Project(to,bearing,sfac);

	// If a from point has been specified, and this leg does
	// not start at it, just return.
	if ( pFrom && GetpStartPoint(op)!=pFrom ) return;

//	Return if this leg does not have an initial angle.
	const FLOAT8 angle = fabs(m_StartAngle);
	if ( angle<TINY ) return;

//	Return if the angle is real-close to a right angle
	if ( fabs(angle-PIDIV2  )<0.000001 ||
		 fabs(angle-PIMUL1P5)<0.000001 ) return;

	// Turn it over to the view, remembering that the angle may
	// in fact be a deflection.
	gdc.DrawAngle(bs,from,to,angle,IsDeflection());

} // end of DrawAngles
*/

        /// <summary>
        /// Records a deflection angle at the start of this leg. There must be a preceding
        /// leg for this to make any sense.
        /// </summary>
        /// <param name="value">The deflection, in radians. Negated values go
        /// counter-clockwise.</param>
        internal void SetDeflection(double value)
        {
            // Record the deflection angle at the start of this leg.
            m_StartAngle = value;

            // Remember that it's a deflection (as opposed to a regular angle).
            m_IsDeflection = true;
            //base.SetDeflection(true);
        }
        /*
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

LOGICAL CeStraightLeg::CreateAngleText ( const CePoint* const pFrom
									   , const CeOperation& op
									   , const FLOAT8 sfac
									   , const CeVertex& bs
									   , const CeVertex& from
									   , FLOAT8& bearing
									   , CeVertex& to
									   , CPtrList& text
									   , const LOGICAL wantLinesToo ) const {

	// Project to the end of the leg.
	to = from;
	this->Project(to,bearing,sfac);

	// If a from point has been specified, and this leg does
	// not start at it, just return.
	if ( pFrom && GetpStartPoint(op)!=pFrom ) return FALSE;

	// Return if this leg does not have an initial angle.
	const FLOAT8 angle = fabs(m_StartAngle);
	if ( angle<TINY ) return TRUE;

	// Return if the angle is real-close to a right angle
	if ( fabs(angle-PIDIV2  )<0.000001 ||
		 fabs(angle-PIMUL1P5)<0.000001 ) return TRUE;

	// Create the text.
	MakeText(bs,from,to,m_StartAngle,text);

	return TRUE;

} // end of CreateAngleText
*/

        /// <summary>
        /// Breaks this leg into two legs. The break must leave at least
        /// one distance in each of the resultant legs.
        /// </summary>
        /// <param name="index">The index of the span that should be at the
        /// start of the extra leg.</param>
        /// <returns>The extra leg (at the end of the original leg).</returns>
        internal StraightLeg Break(int index)
        {
            if (this.AlternateFace != null)
                throw new InvalidOperationException("Cannot break a staggered leg");

            // Can't break right at the start or end.
            int nTotal = PrimaryFace.Count;
            if (index <= 0 || index >= nTotal)
                return null;

            // Create a new straight leg with the right number of spans.
            StraightLeg newLeg = new StraightLeg(PrimaryFace, index);

            // Retain the spans prior to that
            PrimaryFace.TruncateLeg(index);

            return newLeg;
        }

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
            StringBuilder sb = new StringBuilder(100);

            // The initial angle.
            if (Math.Abs(m_StartAngle) > Double.Epsilon)
            {
                sb.Append(RadianValue.AsShortString(m_StartAngle));
                sb.Append(" ");
            }

            // The observed lengths.
            AddToString(sb, defaultEntryUnit);

            return sb.ToString();
        }
        */

        /// <summary>
        /// Is the angle at the start of this a deflection?
        /// </summary>
        internal bool IsDeflection
        {
            get { return m_IsDeflection; }
        }
    }
}
