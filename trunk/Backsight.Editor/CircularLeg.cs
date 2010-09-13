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
using System.Diagnostics;
using System.Collections.Generic;

using Backsight.Editor.Operations;
using Backsight.Editor.Observations;

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
        /// Observed radius. There will be a slight difference betweeen this value and the
        /// adjusted radius available via <see cref="m_Circle"/>
        /// </summary>
        Distance m_Radius;

        /// <summary>
        /// The circle that this leg sits on. The radius defined as part of this instance is
        /// the adjusted radius (see also <seealso cref="m_Radius"/>)
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Flag bits
        /// </summary>
        CircularLegFlag m_Flag;

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
        public override Circle Circle
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
        public override ILength Length
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
        public override void Project(ref IPosition pos, ref double bearing, double sfac)
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
        public void GetPositions(IPosition bc, double sbearing, double sfac,
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
        /// <param name="display">The display to draw to</param>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated
        /// for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        public override void Render(ISpatialDisplay display, ref IPosition pos, ref double bearing, double sfac)
        {
            //	Create an undefined circular span
            CircularSpan span = new CircularSpan(this, pos, bearing, sfac);

            // Draw each visible span in turn. Note that for cul-de-sacs, there may be
            // no observed spans.

            int nspan = this.Count;

            if (nspan==0)
            {
                span.Get(0);
                span.Render(display);
            }
            else
            {
                for (int i = 0; i < nspan; i++)
                {
                    span.Get(i);
                    span.Render(display);
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
            PathOperation op = (PathOperation)ff.Creator;

            // Create an undefined circular span
            CircularSpan span = new CircularSpan(this, terminal, bearing, sfac);

            // If this leg already has an associated circle, move it. Otherwise
            // add a circle to the map that corresponds to this leg.
            if (m_Circle == null)
                m_Circle = AddCircle(op, createdPoints, span);
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
                    SetCircle(span, m_Circle);

                    // Define the position of the center point.
                    center.SetPointGeometry(PointGeometry.Create(span.Center));
                }
                else
                {
                    // The existing center point makes reference to the
                    // circle, so clean that up.
                    center.CutReference(m_Circle);

                    // 19-OCT-99: The AddCircle call just returns
                    // the circle that this leg already knows about, so
                    // clear it first.
                    m_Circle = null;

                    // Add a new circle.
                    m_Circle = AddCircle(op, createdPoints, span);
                }
            }

            // Create (or update) features for each span. Note that for
            // cul-de-sacs, there may be no observed spans.
            int nspan = Math.Max(1, this.Count);
            PointFeature noInsert = null;

            for (int i = 0; i < nspan; i++)
            {
                Feature feat = SaveSpan(ref noInsert, op, createdPoints, span, i, null);
            }

            // Update BC info to refer to the EC.
            terminal = span.EC;
            bearing = span.ExitBearing;
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
            result.Geometry = new ArcGeometry(m_Circle, from, to, IsClockwise);

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
        /// Defines the geometry for this leg.
        /// </summary>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        internal override void CreateGeometry(ref IPosition terminal, ref double bearing, double sfac)
        {
            // Create an undefined circular span
            CircularSpan span = new CircularSpan(this, terminal, bearing, sfac);

            // The circle should have been created already, but with an undefined radius
            Debug.Assert(m_Circle != null);
            m_Circle.Radius = span.ScaledRadius;
            m_Circle.CenterPoint.SetPointGeometry(PointGeometry.Create(span.Center));

            // Create geometry for each span. Note that for cul-de-sacs, there may be
            // no observed spans.
            int nspan = Math.Max(1, this.Count);

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
                    endPoint.SetPointGeometry(PointGeometry.Create(span.End));
            }

            // Update BC info to refer to the EC.
            terminal = span.EC;
            bearing = span.ExitBearing;
        }

        /// <summary>
        /// Adds a circle to the map, suitable for this leg. Called by <see cref="CircularLeg.Save"/>.
        /// </summary>
        /// <param name="creator">The edit operation containing the leg</param>
        /// <param name="createdPoints">Newly created point features</param>
        /// <param name="span">The span that's being saved</param>
        /// <returns>The circle for the span (may be a circle that previously existed)</returns>
        Circle AddCircle(PathOperation creator, List<PointFeature> createdPoints, CircularSpan span)
        {
            // If a circle was previously created, just return that.
            if (m_Circle!=null)
                return m_Circle;

            // Add the circle (checks if it's already there).
            // This will cross-reference the center point to the circle.
            PointFeature centerPoint = creator.EnsurePointExists(span.Center, createdPoints);
            m_Circle = creator.MapModel.AddCircle(centerPoint, span.ScaledRadius);
            return m_Circle;
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
        }

        /// <summary>
        /// Saves a specific span of this leg.
        /// </summary>
        /// <param name="insert">The point of the end of any new insert that
        /// immediately precedes this span. This will be updated if this span is
        /// also a new insert (if not, it will be returned as a null value).</param>
        /// <param name="op">The connection path that this leg is part of.</param>
        /// <param name="createdPoints">Newly created point features</param>
        /// <param name="span">The span for the leg.</param>
        /// <param name="index">The index of the span to save.</param>
        /// <param name="uc">The context in which editing revisions are being made (defined only
        /// when performing rollforward). Used to hold a record of any positional changes.</param>
        /// <returns>The feature (if any) that represents the span. If the span has a line,
        /// this will be a <see cref="LineFeature"/>. If the span has no line, it may be
        /// a <see cref="PointFeature"/> at the END of the span. A null is also valid,
        /// meaning that there is no line & no terminal point.</returns>
        Feature SaveSpan(ref PointFeature insert, PathOperation op, List<PointFeature> createdPoints,
                            CircularSpan span, int index, UpdateContext uc)
        {
            // The very end of a connection path should never be moved.
            PointFeature veryEnd = op.EndPoint;

            if (IsNewSpan(index))
            {
                // Is this the very last span in the connection path?
                int nspan = Math.Max(1, this.Count);
                bool isLast = (index==(nspan-1) && op.IsLastLeg(this));

                // Save the insert.
                LineFeature newLine = SaveInsert(span, index, op, isLast);

                // Record the new line as part of this leg
                AddNewSpan(index, newLine);

                // Remember the last insert position.
                insert = newLine.EndPoint;

                return newLine;
            }
            else
            {
                // Get the span to save
                span.Get(index);

                // See if the span previously had a saved feature.
                Feature old = GetFeature(index);

                // Save the span.
                Feature feat = SaveSpan(insert, op, createdPoints, span, old, veryEnd, uc);

                // If the saved span is different from what we had before,
                // tell the base class about it.
                if (!Object.ReferenceEquals(feat, old))
                    SetFeature(index, feat);

                // That wasn't an insert.
                insert = null;

                return feat;
            }
        }

        /// <summary>
        /// Saves a newly inserted span.
        /// </summary>
        /// <param name="span">The new span</param>
        /// <param name="index">The index of the new span.</param>
        /// <param name="creator">The operation that the new span should be referred to.</param>
        /// <param name="isLast">Is the new span going to be the very last span in the last
        /// leg of a connection path?</param>
        /// <returns>The line that was created.</returns>
        LineFeature SaveInsert(CircularSpan span, int index, PathOperation creator, bool isLast)
        {
            // SS:20080314 - Most of what follows is identical to the corresponding method
            // in StraightSpan. The only difference is right at the end, where the line
            // gets created...

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

            // Add a line (this will cross-reference the points and the circle to the new line).
            span.HasLine = true;
            return map.AddCircularArc(m_Circle, pS, pE, IsClockwise, map.DefaultLineType, creator);
        }

        /// <summary>
        /// Saves a span in the map.
        /// </summary>
        /// <param name="insert">Reference to a new point that was inserted just before
        /// the span. Defined only during rollforward.</param>
        /// <param name="op">The editing operation this leg is part of</param>
        /// <param name="createdPoints">Newly created point features</param>
        /// <param name="span">The span for the leg.</param>
        /// <param name="old">Pointer to the feature that was previously associated with
        /// the span. This will be not null when the span is being saved as part of
        /// rollforward processing (in that case, <paramref name="uc"/> must also be supplied).</param>
        /// <param name="veryEnd">The location at the very end of the connection path
        /// that this leg is part of.</param>
        /// <param name="uc">The context in which editing revisions are being made (null if
        /// not making revisions). Must be specified if <paramref name="old"/> is not null.</param>
        /// <returns>The feature (if any) that represents the span. If the span has a line,
        /// this will be a <see cref="LineFeature"/>. If the span has no line, it may be
        /// a <see cref="PointFeature"/> at the END of the span. A null is also valid,
        /// meaning that there is no line & no terminal point.</returns>
        Feature SaveSpan(PointFeature insert, PathOperation op, List<PointFeature> createdPoints,
                            CircularSpan span, Feature old, PointFeature veryEnd, UpdateContext uc)
        {
            // The circle on which the span is based should already be defined
            // (see the call that CircularLeg.Save makes to AddCircle).
            if (m_Circle==null)
                throw new Exception("CircularLeg.SaveSpan -- Circle has not been defined");

            // Get map info.
            CadastralMapModel map = op.MapModel;

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
                        throw new Exception("CircularSpan.Save - Mismatched line");

                    if (insert!=null)
                    {
                        throw new NotImplementedException("CircularLeg.SaveSpan - insert");

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
                            throw new NotImplementedException("CircularSpan.Save");
                            //pArc->Move(sloc, eloc, m_pCircle, m_IsClockwise);
                            //line.StartPoint.Move(sloc, m_Circle, m_IsClockwise);
                            //line.EndPoint.Move(eloc, m_Circle, m_IsClockwise);
                        }
                    }
                }
                else if (span.HasEndPoint) // Feature should be a point
                {
                    PointFeature point = (old as PointFeature);
                    if (point==null)
                        throw new Exception("CircularSpan.Save - Mismatched point");

                    if (!point.IsCoincident(veryEnd))
                        point.MovePoint(uc, eloc);
                }

                feat = old; // SS:20080308 - Not sure if this is correct (it's not in the comparable block of StraightSpan.cs)
            }
            else
            {
                // If we have an end point, add it. If it creates something
                // new, assign an ID to it.
                if (span.HasEndPoint)
                    feat = op.EnsurePointExists(eloc, createdPoints);

                // Add a line if we have one.
                if (span.HasLine)
                {
                    Debug.Assert(span.HasEndPoint);
                    PointFeature ps = op.EnsurePointExists(sloc, createdPoints);
                    PointFeature pe = (PointFeature)feat;
                    feat = map.AddCircularArc(m_Circle, ps, pe, IsClockwise, map.DefaultLineType, op);
                }
            }

            return feat;
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
        internal double CentralAngle
        {
            get { return m_Angle1; }
        }

        /// <summary>
        /// The entry angle for this leg (assuming it isn't a cul-de-sac -
        /// see the <see cref="IsCulDeSac"/> property)
        /// </summary>
        internal double EntryAngle
        {
            get { return m_Angle1; }
        }

        /// <summary>
        /// The exit angle for this leg (assuming it isn't a cul-de-sac -
        /// see the <see cref="IsCulDeSac"/> property)
        /// </summary>
        internal double ExitAngle
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
        public bool IsClockwise
        {
            get { return (m_Flag & CircularLegFlag.CounterClockwise) == 0; }
            set { SetFlag(CircularLegFlag.CounterClockwise, !value); }
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
        public double Radius
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
        public bool IsCulDeSac
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
            return face.UpdateCurves(uc, insert, op, spos, epos, m_Circle, IsClockwise);
        }

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
