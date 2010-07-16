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
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using Backsight.Editor.Operations;
using Backsight.Geometry;
using Backsight.Environment;

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

        #endregion

        /// <summary>
        /// Angle at the start of the leg (signed). 
        /// </summary>
        internal double StartAngle
        {
            get { return m_StartAngle; }
            set { m_StartAngle = value; }
        }

        public override Circle Circle
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
        public override ILength Length
        {
            get { return new Length(GetTotal()); }
        }

        /// <summary>
        /// Given the position of the start of this leg, along with an initial bearing,
        /// project the end of the leg, along with an exit bearing.
        /// </summary>
        /// <param name="pos">The position at the start of the leg.</param>
        /// <param name="bearing">The initial bearing (e.g. if the previous leg was also
        /// a straight leg from A to B, the bearing is from A through B).</param>
        /// <param name="sfac">Scaling factor to apply. Default=1.0</param>
        public override void Project(ref IPosition pos, ref double bearing, double sfac)
        {
            // Add on any initial angle (it may be a deflection).
            if (Math.Abs(m_StartAngle) > Double.Epsilon)
            {
                if (m_IsDeflection)
                    bearing += m_StartAngle;
                else
                    bearing += (m_StartAngle - Math.PI);
            }

            // Get the total length of the leg.
            double length = GetTotal() * sfac;

            // Figure out shifts.
            double dE = length * Math.Sin(bearing);
            double dN = length * Math.Cos(bearing);

            // Define the end position.
            pos = new Position(pos.X + dE, pos.Y + dN);
        }

        /// <summary>
        /// Draws this leg
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated
        /// for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        public override void Render(ISpatialDisplay display, ref IPosition pos, ref double bearing, double sfac)
        {
            // Add on any initial angle (it may be a deflection).
            if (Math.Abs(m_StartAngle) > Double.Epsilon)
            {
                if (m_IsDeflection)
                    bearing += m_StartAngle;
                else
                    bearing += (m_StartAngle - Math.PI);
            }

            // Create a straight span
            StraightSpan span = new StraightSpan(this, pos, bearing, sfac);

            // Draw each visible span in turn.
            for (int i = 0; i < this.Count; i++)
            {
                span.Get(i);
                span.Render(display);
            }

            // Return the end position of the last span.
            pos = span.End;
        }

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

        /// <summary>
        /// Saves features for this leg.
        /// </summary>
        /// <param name="ff">The factory for creating new spatial features</param>
        /// <param name="createdPoints">Newly created point features</param>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg.
        /// Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        internal override void Save(FeatureFactory ff, List<PointFeature> createdPoints,
                                    ref IPosition terminal, ref double bearing, double sfac)
        {
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

            int nspan = this.Count;
            for (int i = 0; i < nspan; i++)
            {
                // Get info for the current span (this defines the
                // adjusted start and end positions, among other things).
                span.Get(i);

                // Save the span
                Feature feat = SaveSpan(span, ff, createdPoints, null, null, null, null);
                SetFeature(i, feat);
            }

            // Return the end position of the last span.
            terminal = span.End;
        }

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
        /// Defines the geometry for this leg.
        /// </summary>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        internal override void CreateGeometry(ref IPosition terminal, ref double bearing, double sfac)
        {
            // Much like the Save method...

            // Add on any initial angle (it may be a deflection).
            if (Math.Abs(m_StartAngle) > MathConstants.TINY)
            {
                if (m_IsDeflection)
                    bearing += m_StartAngle;
                else
                    bearing += (m_StartAngle - Math.PI);
            }

            // Create a straight span
            StraightSpan span = new StraightSpan(this, terminal, bearing, sfac);

            int nspan = this.Count;
            for (int i = 0; i < nspan; i++)
            {
                // Get info for the current span (this defines the
                // adjusted start and end positions, among other things).
                span.Get(i);

                // Create the geometry for the point at the end of the span
                SpanInfo data = GetSpanData(i);
                Feature feat = data.CreatedFeature;
                PointFeature endPoint = null;

                if (feat is PointFeature)
                    endPoint = (PointFeature)feat;
                else if (feat is LineFeature)
                    endPoint = (feat as LineFeature).EndPoint;

                if (endPoint != null && endPoint.PointGeometry == null)
                    endPoint.PointGeometry = PointGeometry.Create(span.End);
            }

            // Return the end position of the last span.
            terminal = span.End;
        }

        /// <summary>
        /// Rollforward this leg.
        /// </summary>
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
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
        internal override bool Rollforward(UpdateContext uc, ref PointFeature insert, PathOperation op,
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

        /// <summary>
        /// Saves a span in the map.
        /// </summary>
        /// <param name="span">The span to save</param>
        /// <param name="ff">The factory for creating new spatial features</param>
        /// <param name="createdPoints">Newly created point features</param>
        /// <param name="insert">Reference to a new point that was inserted just before
        /// this span. Defined only during rollforward.</param>
        /// <param name="old">Pointer to the feature that was previously associated with
        /// this span. This will be not null when the span is being saved as part of
        /// rollforward processing (in that case, <paramref name="uc"/> must also be supplied).</param>
        /// <param name="veryEnd">The location at the very end of the connection path
        /// that this span is part of.</param>
        /// <param name="uc">The context in which editing revisions are being made (null if
        /// not making revisions). Must be specified if <paramref name="old"/> is not null.</param>
        /// <returns>The feature (if any) that represents the span. If the span has a line,
        /// this will be a <see cref="LineFeature"/>. If the span has no line, it may be
        /// a <see cref="PointFeature"/> at the END of the span. A null is also valid,
        /// meaning that there is no line & no terminal point.</returns>
        internal Feature SaveSpan(StraightSpan span, FeatureFactory ff,
                                  List<PointFeature> createdPoints, PointFeature insert, Feature old,
                                  PointFeature veryEnd, UpdateContext uc)
        {
            // Get map info.
            CadastralMapModel map = CadastralMapModel.Current;

            // Reference to the created feature (if any).
            Feature feat = null;

            // Make sure the start and end points have been rounded to
            // the internal resolution.
            IPointGeometry sloc = PointGeometry.Create(span.Start);
            IPointGeometry eloc = PointGeometry.Create(span.End);

            // If the span was previously associated with a feature, just
            // move it. If the feature is a line, we want to move the
            // location at the end (except in a case where a new line
            // has just been inserted prior to it, in which case we
            // need to change the start location so that it matches
            // the end of the new guy).

            if (old!=null)
            {
                Debug.Assert(uc!=null);

                if (span.HasLine) // Feature should therefore be a line
                {
                    LineFeature line = (old as LineFeature);
                    if (line==null)
                        throw new Exception("StraightSpan.Save - Mismatched line");

                    if (insert!=null)
                    {
                        throw new NotImplementedException("StraightLeg.SaveSpan - insert");

                        //line.ChangeEnds(insert, line.EndPoint);
                        //if (!line.EndPoint.IsCoincident(veryEnd))
                        //    line.EndPoint.Move(eloc);
                    }
                    else
                    {
                        if (line.EndPoint.IsCoincident(veryEnd))
                            line.StartPoint.MovePoint(uc, sloc);
                        else
                        {
                            line.StartPoint.MovePoint(uc, sloc);
                            line.EndPoint.MovePoint(uc, eloc);
                        }
                    }
                }
                else if (span.HasEndPoint) // Feature should be a point
                {
                    PointFeature point = (old as PointFeature);
                    if (point==null)
                        throw new Exception("StraightSpan.Save - Mismatched point");

                    if (!point.IsCoincident(veryEnd))
                        point.MovePoint(uc, eloc);
                }
            }
            else
            {
                // If we have an end point, add it.
                if (span.HasEndPoint)
                    feat = EnsurePointExists(ff, eloc, createdPoints, veryEnd);

                // Add a line if we have one.
                if (span.HasLine)
                {
                    Debug.Assert(span.HasEndPoint);
                    PointFeature ps = EnsurePointExists(ff, sloc, createdPoints, veryEnd);
                    PointFeature pe = (PointFeature)feat;
                    PathOperation op = (PathOperation)ff.Creator;
                    feat = map.AddLine(ps, pe, map.DefaultLineType, op);
                    //feat = ff.CreateSegmentLineFeature(
                }
            }

            return feat;
        }

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
            span.Get(index);

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
        /// <param name="op">The connection path that contains this leg.</param>
        /// <param name="index">The index of the span that should be at the
        /// start of the extra leg.</param>
        /// <returns>The extra leg (at the end of the original leg).</returns>
        Leg Break(PathOperation op, int index)
        {
            // Can't break right at the start or end.
            int nTotal = this.Count;
            if (index <= 0 || index >= nTotal)
                return null;

            // Create a new straight leg with the right number of spans.
            int nSpan = nTotal - index;
            StraightLeg newLeg = new StraightLeg(nSpan);

            // Tell the operation to insert the new leg.
            if (!op.InsertLeg(this, newLeg))
                return null;

            // Stick in a (clockwise) angle of 180 degrees.
            newLeg.StartAngle = Math.PI;

            // Move observations etc from the end of the original leg.
            MoveEndLeg(index, newLeg);

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

            // Get the extra leg to do the rest.
            return face.MakeSegments(op, spos, epos);
        }

        /// <summary>
        /// Rollforward the second face of this leg.
        /// </summary>
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
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
        internal override bool RollforwardFace(UpdateContext uc, ref IPointGeometry insert, PathOperation op,
                                                ExtraLeg face, IPosition spos, IPosition epos)
        {
            // Get the extra face to do it.
            return face.UpdateSegments(uc, insert, op, spos, epos);
        }

        /// <summary>
        /// Is the angle at the start of this a deflection?
        /// </summary>
        internal bool IsDeflection
        {
            get { return m_IsDeflection; }
        }
    }
}
