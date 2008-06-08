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
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using Backsight.Forms;
using Backsight.Editor.Forms;
using Backsight.Editor.Operations;
using Backsight.Geometry;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="11-JUN-1999" />
    /// <summary>
    /// User interface for generating a parallel line.
    /// </summary>
    class ParallelUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The reference line
        /// </summary>
        LineFeature m_Line; // m_pArc

        /// <summary>
        /// The dialog for getting the offset.
        /// </summary>
        ParallelControl m_ParDial;

        // The offset will be ONE of the following ...

        /// <summary>
        /// The distance to parallel.
        /// </summary>
        Distance m_Offset;

        /// <summary>
        /// Offset point the parallel should pass through.
        /// </summary>
        PointFeature m_OffsetPoint;

        /// <summary>
        /// True if arc direction of the parallel is the reverse of the
        /// reference line's direction (does not apply to straight lines).
        /// </summary>
        bool m_IsReversed;

        /// <summary>
        /// The dialog for getting southern terminal.
        /// </summary>
        TerminalControl m_TermDial1;

        /// <summary>
        /// The southern terminal line.
        /// </summary>
        LineFeature m_TermLine1; // m_pTerm1

        /// <summary>
        /// The dialog for getting northern terminal.
        /// </summary>
        TerminalControl m_TermDial2;

        /// <summary>
        /// The northern terminal line.
        /// </summary>
        LineFeature m_TermLine2; // m_pTerm2

        // For painting ...

        /// <summary>
        /// The start of the parallel.
        /// </summary>
        IPosition m_Par1;

        /// <summary>
        /// The end of the parallel.
        /// </summary>
        IPosition m_Par2;

        /// <summary>
        /// The position on 1st terminal.
        /// </summary>
        IPosition m_Term1;

        /// <summary>
        /// The position on 2nd terminal.
        /// </summary>
        IPosition m_Term2;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a new parallel line.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="from">The reference line.</param>
        internal ParallelUI(IControlContainer cc, IUserAction action, LineFeature from)
            : base(cc, action)
        {
            // Set initial values.
            SetZeroValues();

            // Remember the reference line.
            m_Line = from;
        }
    
        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="updcmd">The update command.</param>
        internal ParallelUI(IControlContainer cc, IUserAction action, UpdateUI updcmd)
            : base(cc, action, updcmd)
        {
            // Just set zero values for everything (we'll pick up stuff in Run).
            SetZeroValues();
        }

        #endregion

        internal LineFeature ReferenceLine { get { return m_Line; } }
        internal IPosition ParallelOne { get { return m_Par1; } }
        internal IPosition ParallelTwo { get { return m_Par2; } }
        internal IPosition TerminalOne { get { return m_Term1; } }
        internal IPosition TerminalTwo { get { return m_Term2; } }

        /// <summary>
        /// Initializes everything with null values. For used by constructors.
        /// </summary>
        void SetZeroValues()
        {
            m_Line = null;
            m_ParDial = null;
            m_Offset = null;
            m_OffsetPoint = null;
            m_IsReversed = false;
            m_TermDial1 = null;
            m_TermLine1 = null;
            m_TermDial2 = null;
            m_TermLine2 = null;
            m_Par1 = null;
            m_Par2 = null;
            m_Term1 = null;
            m_Term2 = null;
        }

        void IDisposable.Dispose()
        {
            KillDialogs();
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            // Don't run more than once.
            if (m_ParDial!=null || m_TermDial1!=null || m_TermDial2!=null)
                throw new Exception("ParallelUI.Run - Command is already running.");

            // If we're doing an update, get the reference line from the original op.
            ParallelOperation op = null;
            UpdateUI up = this.Update;

            if (up!=null)
            {
                op = (up.GetOp() as ParallelOperation);
                if (op==null)
                    throw new Exception("ParallelUI.Run - Unexpected edit type.");

                // Pick up the line that acted as the reference line.
                m_Line = op.ReferenceLine;
            }

            // If it wasn't an update, we might be recalling an old op.
            if (op==null)
                op = (this.Recall as ParallelOperation);

            // Get old observations if necessary.
            if (op!=null)
            {
                // Pick up the offset.
                Observation offset = op.Offset;

                // Is it an observed distance?
                Distance dist = (offset as Distance);
                if (dist!=null)
                    m_Offset = new Distance(dist);
                else
                {
                    // The only other thing it could be is an offset point.
                    OffsetPoint offPoint = (offset as OffsetPoint);
                    if (offPoint!=null)
                        m_OffsetPoint = offPoint.Point;
                }

                m_IsReversed = op.IsArcReversed;
                m_TermLine1 = op.Terminal1;
                m_TermLine2 = op.Terminal2;

                // Ensure the reference line has been selected/highlighted (this may end up
                // calling OnSelectLine)
                EditingController.Current.Select(m_Line);

                // Calculate stuff & paint what we've got.
                Calculate();
                Paint(null);
	        }

	        // Create modeless dialog.

	        if (up!=null)
		        m_ParDial = new ParallelControl(up);
	        else
		        m_ParDial = new ParallelControl(this);

            this.Container.Display(m_ParDial);
            return true;
        }

        /// <summary>
        /// Does any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn. Not used.</param>
        internal override void Paint(PointFeature point)
        {
            Draw();
        }

        /// <summary>
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <see cref="Paint"/> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return true; }
        }

        /// <summary>
        /// Draws the current state of the edit
        /// </summary>
        internal void Draw()
        {
            Debug.Assert(m_Line!=null);
            ISpatialDisplay view = ActiveDisplay;

            // Figure out the positions for the ends of the parallel line (if any) ...

            // Assume we already know both terminals.
            IPosition start = m_Term1;
            IPosition end = m_Term2;

            // If either one is undefined, but a dialog for it is active,
            // try to get the terminal from there instead.
            if (m_TermDial1!=null && start==null)
                start = m_TermDial1.TerminalPosition;

            if (m_TermDial2!=null && end==null)
                end = m_TermDial2.TerminalPosition;

            // If they weren't actually defined, use the parallel points instead.
            if (start==null)
                start = m_Par1;

            if (end==null)
                end = m_Par2;

            // If those weren't defined either, try to calculate them now.
            if (end==null && Calculate())
            {
                start = m_Par1;
                end = m_Par2;
            }

            // Any offset point
            if (m_OffsetPoint!=null)
                m_OffsetPoint.Draw(view, Color.Green);

            // Everything else should draw in usual command-style colour.
            IDrawStyle style = EditingController.Current.Style(Color.Magenta);
            IDrawStyle dottedStyle = new DottedStyle();

            // If the reference line is a curve, get the curve info.
            if (m_Line is ArcFeature)
            {
                ArcFeature arc = (m_Line as ArcFeature);
                bool iscw = arc.IsClockwise;

                // Reverse the direction if necessary.
                if (m_IsReversed)
                    iscw = !iscw;

                // Draw the parallel line (the rest of the circle being dotted).
                if (start!=null)
                {
                    CircularArcGeometry parArc = new CircularArcGeometry(arc.Circle, start, end, iscw);
                    style.Render(view, parArc);

                    parArc.IsClockwise = !parArc.IsClockwise;
                    dottedStyle.Render(view, parArc);
                }
            }
            else
            {
                // PARALLEL IS STRAIGHT

                // If we've got something, figure out positions for dotted portion.
                if (start!=null)
                {
                    // What's the max length of a diagonal crossing the entire screen?
                    double maxdiag = this.MaxDiagonal;

                    // What's the bearing from the start to the end of the parallel?
                    double bearing = Geom.BearingInRadians(start, end);

                    // Project to a point before the start end of the parallel, as
                    // well as a point after the end.
                    IPosition before = Geom.Polar(start, bearing+Constants.PI, maxdiag);
                    IPosition after = Geom.Polar(end, bearing, maxdiag);

                    LineSegmentGeometry.Render(before, start, view, dottedStyle);
                    LineSegmentGeometry.Render(start, end, view, style);
                    LineSegmentGeometry.Render(end, after, view, dottedStyle);
                }
            }

            // Draw terminal positions (if defined).

            if (m_Term1!=null)
                style.Render(view, m_Term1);

            if (m_Term2!=null)
                style.Render(view, m_Term2);

            // The terminal lines.

            if (m_TermLine1!=null)
                m_TermLine1.Render(view, style);

            if (m_TermLine2!=null)
                m_TermLine2.Render(view, style);

            // Do the active dialog last so their stuff draws on top.
            if (m_ParDial!=null)
                m_ParDial.Draw();

            if (m_TermDial1!=null)
                m_TermDial1.Draw();

            if (m_TermDial2!=null)
                m_TermDial2.Draw();
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            // Point selection applies only to the offset point dialog.
            if (m_ParDial!=null)
                m_ParDial.SelectPoint(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            if (m_TermDial2!=null)
                m_TermDial2.SelectLine(line);
            else if (m_TermDial1!=null)
                m_TermDial1.SelectLine(line);
        }

        /// <summary>
        /// Reacts to selection of the Cancel button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window. If this matches the dialog that
        /// this command knows about, the dialog will be destroyed and the command
        /// terminates. If it's some other window, it must be a sub-dialog created
        /// by our guy, so let it handle the request.</param>
        internal override void DialAbort(Control wnd)
        {
            // Ensure all dialogs have been toasted.
            KillDialogs();

            // Get the base class to finish up.
            AbortCommand();
        }

        /// <summary>
        /// Destroys any dialogs that are currently displayed.
        /// </summary>
        void KillDialogs()
        {
            this.Container.Clear();

            if (m_ParDial!=null)
            {
                m_ParDial.Dispose();
                m_ParDial = null;
            }

            if (m_TermDial1!=null)
            {
                m_TermDial1.Dispose();
                m_TermDial1 = null;
            }

            if (m_TermDial2!=null)
            {
                m_TermDial2.Dispose();
                m_TermDial2 = null;
            }
        }

        /// <summary>
        /// Reacts to selection of the OK button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window. If this matches the dialog that
        /// this command knows about, the command will be executed (and, on success,
        /// the dialog will be destroyed). If it's some other window, it must
        /// be a sub-dialog created by our guy, so let it handle the request.</param>
        /// <returns>True if command finished ok</returns>
        internal override bool DialFinish(Control wnd)
        {
            // If it's the offset dialog that's just finished, grab info
            // from it, delete it, and go to the dialog for the first
            // terminal line.

            ISpatialDisplay view = ActiveDisplay;
            UpdateUI up = this.Update;

            if (m_ParDial!=null)
            {
                // Get info from dialog (it'll be ONE of the two). The dialog
                // should only call this function after validating that one
                // of them is defined.
                m_OffsetPoint = m_ParDial.OffsetPoint;
                if (m_OffsetPoint==null)
                    m_Offset = m_ParDial.OffsetDistance;

                // Destroy the dialog.
                KillDialogs();

                // Calculate the positions for the parallel points.
                Calculate();

                // Repaint what we know about.
                Draw();

                // And start the dialog for the first terminal line.
                if (up!=null)
                    m_TermDial1 = new TerminalControl(up, false);
                else
                    m_TermDial1 = new TerminalControl(this, false);

                //m_TermDial1.Show();
                this.Container.Display(m_TermDial1);
                return true;
            }

            if (m_TermDial1!=null)
            {
                // Get the first terminal line (if any). And the position.
                m_TermLine1 = m_TermDial1.TerminalLine;
                m_Term1 = m_TermDial1.TerminalPosition;

                // And move on to the 2nd terminal dialog.
                KillDialogs();

                // Repaint what we know about.

                Draw();
                // And start the dialog for the first terminal line.
                if (up!=null)
                    m_TermDial2 = new TerminalControl(up, true);
                else
                    m_TermDial2 = new TerminalControl(this, true);

                this.Container.Display(m_TermDial2);
                //m_TermDial2.Show();
                return true;
            }

            if (m_TermDial2==null)
                throw new Exception("ParallelUI.DialFinish - No dialog!");

            // Get the nortthern terminal line (if any). And the position.
            m_TermLine2 = m_TermDial2.TerminalLine;
            m_Term2 = m_TermDial2.TerminalPosition;

            // Erase everything special that we've drawn.
            ErasePainting();

            // And ensure the view has nothing selected (sometimes the line
            // last selected had been unhighlighted, although it's end points
            // stay highlighted for some reason).
            EditingController.Current.ClearSelection();

            // If we are doing an update, alter the original operation.
            if (up!=null)
            {
                // Get the original operation.
                ParallelOperation op = (ParallelOperation)up.GetOp();
                if (op==null)
                    throw new Exception("ParallelUI.DialFinish - Unexpected edit type.");

                // Make the update.
                if (m_Offset!=null)
                    op.Correct(m_Line, m_Offset, m_TermLine1, m_TermLine2, m_IsReversed);
                else if (m_OffsetPoint!=null)
                    op.Correct(m_Line, new OffsetPoint(m_OffsetPoint), m_TermLine1, m_TermLine2, m_IsReversed);
            }
            else
            {
                // Create the persistent edit (adds to current session)
                ParallelOperation op = new ParallelOperation();
                bool ok = false;
                
                if (m_Offset!=null)
                    ok = op.Execute(m_Line, m_Offset, m_TermLine1, m_TermLine2, m_IsReversed);
                else if (m_OffsetPoint!=null)
                    ok = op.Execute(m_Line, new OffsetPoint(m_OffsetPoint), m_TermLine1, m_TermLine2, m_IsReversed);

                if (!ok)
                    Session.CurrentSession.Remove(op);
            }

            // Destroy the dialog(s).
            KillDialogs();

            // Get the base class to finish up.
            return FinishCommand();
        }

        /// <summary>
        /// Calculates positions that are parallel to a line.
        /// </summary>
        /// <param name="line">The reference line.</param>
        /// <param name="offset">The observed offset (either a <c>Distance</c> or an <c>OffsetPoint</c>).</param>
        /// <param name="sres">The position of the start of the parallel.</param>
        /// <param name="eres">The position of the end of the parallel.</param>
        /// <returns>True if positions calculated ok</returns>
        internal static bool Calculate(LineFeature line, Observation offset, out IPosition sres, out IPosition eres)
        {
            sres = eres = null;

            // Can't calculate if there is insufficient data.
            if (line==null || offset==null)
                return false;

            // Is it an observed distance?
            Distance dist = (offset as Distance);
            if (dist!=null)
                return ParallelUI.Calculate(line, dist, out sres, out eres);

            // The only other thing it could be is an offset point.
            OffsetPoint offPoint = (offset as OffsetPoint);
            if (offPoint!=null)
            {
                PointFeature point = offPoint.Point;
                return ParallelUI.Calculate(line, point, out sres, out eres);
            }

            MessageBox.Show("ParallelUI.Calculate - Unexpected sort of observation");
            return false;
        }

        /// <summary>
        /// Calculates positions that are parallel to a line.
        /// </summary>
        /// <param name="line">The reference line.</param>
        /// <param name="offset">The offset to the parallel, in ground units. Signed to denote
        /// which side (less than zero means it's to the left of the reference line).</param>
        /// <param name="sres">The position of the start of the parallel.</param>
        /// <param name="eres">The position of the end of the parallel.</param>
        /// <returns>True if positions calculated ok</returns>
        static bool Calculate(LineFeature line, Distance offset, out IPosition sres, out IPosition eres)
        {
            // No result positions so far.
            sres = eres = null;

            // Get the ends of the reference line.
            IPosition spos = line.StartPoint;
            IPosition epos = line.EndPoint;

            ICoordinateSystem sys = CadastralMapModel.Current.CoordinateSystem;

            // If the reference line is a circular arc, get the curve info.
            if (line is ArcFeature)
            {
                ArcFeature arc = (line as ArcFeature);
                Circle circle = arc.Circle;
                double radius = circle.Radius;
                IPosition centre = circle.Center;
                bool iscw = arc.IsClockwise;

                // Get the midpoint of the curve. The reduction of the
                // ground distance will be along the line that goes
                // from the centre of the circle & through this position.

                ILength len = line.Length;
                ILength halfLen = new Length(len.Meters * 0.5);
                IPosition middle;
                line.LineGeometry.GetPosition(halfLen, out middle);

                // Get the bearing from the centre to the mid-position
                // and use that to reduce the offset to the mapping plane.
                double bearing = Geom.BearingInRadians(centre, middle);
                double offdist = offset.GetPlanarMetric(middle, bearing, sys);

                // No parallel if the offset exceeds the radius.
                // if ( offdist > radius ) return FALSE;

                // Calculate the parallel points.
                double sbear = Geom.BearingInRadians(centre, spos);
                sres = Geom.Polar(centre, sbear, offdist+radius);

                double ebear = Geom.BearingInRadians(centre, epos);
                eres = Geom.Polar(centre, ebear, offdist+radius);
            }
            else
            {
                // Get the bearing.of the line.
                double bearing = Geom.BearingInRadians(spos, epos);

                // Get the planar distance for a perpendicular line that passes
                // through the midpoint of the reference line. The planar distance
                // will have the same sign as the ground value.

                IPosition middle = Position.CreateMidpoint(spos, epos);
                bearing += Constants.PIDIV2;
                double offdist = offset.GetPlanarMetric(middle, bearing, sys);

                // Calculate the parallel points.
                sres = Geom.Polar(spos, bearing, offdist);
                eres = Geom.Polar(epos, bearing, offdist);
            }

            return true;
        }

        /// <summary>
        /// Calculates positions that are parallel to a line.
        /// </summary>
        /// <param name="line">The reference line.</param>
        /// <param name="offpoint">The point the parallel must pass through.</param>
        /// <param name="sres">The position of the start of the parallel.</param>
        /// <param name="eres">The position of the end of the parallel.</param>
        /// <returns>True if positions calculated ok</returns>
        internal static bool Calculate(LineFeature refline, PointFeature offpoint, out IPosition sres, out IPosition eres)
        {
            // No result positions so far.
            sres = eres = null;

            // Get the ends of the reference line.
            IPosition spos = refline.StartPoint;
            IPosition epos = refline.EndPoint;

            // If the reference line is a circular arc
            if (refline is ArcFeature)
            {
                // Get the curve info
                ArcFeature arc = (refline as ArcFeature);
                Circle circle = arc.Circle;
                double radius = circle.Radius;
                IPosition centre = circle.Center;
                bool iscw = arc.IsClockwise;

                // Get the (planar) distance from the centre of the
                // circle to the offset point.
                double offdist = Geom.Distance(offpoint, centre);

                // Project the BC/EC radially.
                double sbear = Geom.BearingInRadians(centre, spos);
                sres = Geom.Polar(centre, sbear, offdist);

                double ebear = Geom.BearingInRadians(centre, epos);
                eres = Geom.Polar(centre, ebear, offdist);
            }
            else
            {
                double bearing = Geom.BearingInRadians(spos, epos);

                // Get the perpendicular distance (signed) from the offset point
                // to the reference line.
                double offdist = Geom.SignedDistance(spos.X, spos.Y, bearing, offpoint.X, offpoint.Y);

                // Calculate the parallel points.
                bearing += Constants.PIDIV2;
                sres = Geom.Polar(spos, bearing, offdist);
                eres = Geom.Polar(epos, bearing, offdist);
            }

            return true;
        }

        /// <summary>
        /// Tries to calculate the position of the parallel line.
        /// </summary>
        /// <returns>True if parallel positions calculated ok.</returns>
        bool Calculate()
        {
            // Ensure any previously defined end positions have been reset.
            m_Par1 = m_Par2 = null;

            // Can't do nothing if the reference line is undefined.
            if (m_Line==null)
                return false;

            // Calculate the parallel points, depending on what sort of
            // observation we've got.
            IPosition sres = null;
            IPosition eres = null;
            bool ok = false;

            if (m_Offset!=null)
                ok = ParallelUI.Calculate(m_Line, m_Offset, out sres, out eres);
            else if (m_OffsetPoint!=null)
                ok = ParallelUI.Calculate(m_Line, m_OffsetPoint, out sres, out eres);

            // If the calculation succeeded, allocate vertices to hold the results we got.
            if (ok)
            {
                m_Par1 = sres;
                m_Par2 = eres;
            }

            return ok;
        }

        /// <summary>
        /// Returns the offset for the parallel, in units on the mapping plane. In order
        /// for this to work, a prior call to Calculate must be made.
        /// </summary>
        /// <returns>The offset distance on the mapping plane (>= 0).</returns>
        internal double GetPlanarOffset()
        {
            // If the reference line or the parallel points are undefined, there's nothing we can do.
            if (m_Line==null || m_Par1==null || m_Par2==null)
                return 0.0;

            // If the reference line is a curve, get the curve info.
            if (m_Line is ArcFeature)
            {
		        // Get the (planar) radial offset from the circle to one of the parallel positions.
                ArcFeature arc = (m_Line as ArcFeature);
                double radius = arc.Circle.Radius;
                IPosition center = arc.Circle.Center;
                return Math.Abs(Geom.Distance(center, m_Par1) - radius);
            }

            // Get the ends of the reference line.
            IPosition spos = m_Line.StartPoint;
            IPosition epos = m_Line.EndPoint;

            // And its bearing.
            double bearing = Geom.BearingInRadians(spos, epos);

            // Get the perpendicular distance (signed) from one of the
            // parallel points to the reference line.
            double offdist = Geom.SignedDistance(spos.X, spos.Y, bearing, m_Par1.X, m_Par1.Y);
            return Math.Abs(offdist);
        }

        /// <summary>
        /// Returns the intersection of the parallel with a line. A prior call to Calculate is required.
        /// </summary>
        /// <param name="line">The line to intersect with.</param>
        /// <param name="isEndParallel">Is the intersection biased towards the end of the parallel?</param>
        /// <returns>The intersection (if any). In cases where the line intersects the parallel
        /// more than once, you get an arbitrary intersection.</returns>
        internal IPosition GetIntersect(LineFeature line, bool isEndParallel)
        {
            // Make sure the intersection is undefined.
            IPosition result = null;

            // Return if the parallel points are undefined.
            if (m_Par1==null || m_Par2==null)
                return null;

            // If the reference line is a circular arc, get the curve info.
            if (line is ArcFeature)
            {
                ArcFeature arc = (line as ArcFeature);
                Circle circle = arc.Circle;
                double radius = circle.Radius;
                IPointGeometry centre = circle.Center;
                bool iscw = arc.IsClockwise;

                // Construct a circle that passes through
                // the parallel points (assumed to have the same distance
                // with respect to the centre of the circle).
                double parrad = Geom.Distance(centre, m_Par1);

                // Intersect the circle with the line to intersect with.
                IntersectionResult xres = new IntersectionResult(line);
                uint nx = xres.Intersect(centre, parrad);
                if (nx==0)
                    return null;

                // If there is only one intersection, that's what we want.
                if (nx==1)
                    return xres.Intersections[0].P1;

                // Get the intersection that is closest to the parallel point
                // that has the bias.
                if (isEndParallel)
                    xres.GetClosest(m_Par2, out result, 0.0);
                else
                    xres.GetClosest(m_Par1, out result, 0.0);
            }
            else
            {
                // Get the bearing from the start to the end of the parallel.
                double bearing = Geom.BearingInRadians(m_Par1, m_Par2);

                // Get the ground dimension of a line that crosses the
                // extent of the draw window.
                double dist = MaxDiagonal;

                // Project the parallel line to positions that are well
                // beyond the draw extent.
                IPosition start = Geom.Polar(m_Par1, bearing+Constants.PI, dist);
                IPosition end = Geom.Polar(m_Par2, bearing, dist);

        		// Intersect the line segment with the line to intersect with.
                IntersectionResult xres = new IntersectionResult(line);
                IPointGeometry sg = PointGeometry.Create(start);
                IPointGeometry eg = PointGeometry.Create(end);
                uint nx = xres.Intersect(sg, eg);
                if (nx==0)
                    return null;

                // If there is only one intersection, that's what we want.
                if (nx==1)
                    return xres.Intersections[0].P1;

                // Get the intersection that is closest to the parallel point
                // that has the bias.
                if (isEndParallel)
                    xres.GetClosest(m_Par2, out result, 0.0);
                else
                    xres.GetClosest(m_Par1, out result, 0.0);
            }

            return result;
        }

        /// <summary>
        /// Returns the intersection of the parallel with a line.
        /// </summary>
        /// <param name="refline">The reference line.</param>
        /// <param name="parpos">Search position that coincides with the parallel.</param>
        /// <param name="line">The line to intersect with.</param>
        /// <returns>The intersection (if any). In cases where the line intersects the
        /// parallel more than once, you get the intersection that is closest to the
        /// search position.</returns>
        internal static IPosition GetIntersect(LineFeature refline, IPosition parpos, LineFeature line)
        {
        	// Make sure the intersection is undefined.
            IPosition result = null;

            // Return if the parallel point is undefined.
	        if (parpos==null)
                return null;

        	// If the reference line is a circular arc, get the curve info.
            if (refline is ArcFeature)
            {
                ArcFeature arc = (refline as ArcFeature);
                Circle circle = arc.Circle;
                double radius = circle.Radius;
                IPointGeometry centre = circle.Center;
                bool iscw = arc.IsClockwise;

                // Construct a circle that passes through the search position
                double parrad = Geom.Distance(centre, parpos);

                // Intersect the circle with the line to intersect with.
                IntersectionResult xres = new IntersectionResult(line);
                uint nx = xres.Intersect(centre, parrad);
                if (nx==0)
                    return null;

                // If there is only one intersection, that's what we want.
                if (nx==1)
                    return xres.Intersections[0].P1;

                // Get the intersection that is closest to the search position.
                xres.GetClosest(parpos, out result, 0.0);
            }
            else
            {
                // Get the bearing from the start to the end of the reference line.
                IPosition spos = refline.StartPoint;
                IPosition epos = refline.EndPoint;
                double bearing = Geom.BearingInRadians(spos, epos);

                // Project the parallel line to positions that are a
                // long way away (but make sure we don't end up with
                // negative numbers).
                double dist = Math.Min(100000.0, refline.Length.Meters);
                IPosition start = Geom.Polar(parpos, bearing+Constants.PI, dist);
                IPosition end = Geom.Polar(parpos, bearing, dist);

                // Intersect the line segment with the line to intersect with.
                IntersectionResult xres = new IntersectionResult(line);
                IPointGeometry sg = new PointGeometry(start);
                IPointGeometry eg = new PointGeometry(end);
                uint nx = xres.Intersect(sg, eg);
                if (nx==0)
                    return null;

                // If there is only one intersection, that's what we want.
                if (nx==1)
                    return xres.Intersections[0].P1;

                // Get the intersection that is closest to the search position
                xres.GetClosest(parpos, out result, 0.0);
            }

            return result;
        }

        /// <summary>
        /// Reverses the direction of a parallel to a circular arc.
        /// </summary>
        internal void ReverseArc()
        {
            // Erase whatever's currently drawn.
            ErasePainting();

            // Toggle the arc direction.
            m_IsReversed = !m_IsReversed;

            // Redraw.
            Draw();
        }
    }
}
