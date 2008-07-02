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
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Environment;
using Backsight.Geometry;

namespace Backsight.Editor.Operations
{
	/// <written by="Steve Stanton" on="01-OCT-1999" was="CeArcParallel"/>
    /// <summary>
    /// Operation to create a parallel line.
    /// </summary>
    class ParallelOperation : Operation
    {
        #region Class data

        // Required input ...

        /// <summary>
        /// The reference line for the parallel.
        /// </summary>
        LineFeature m_RefLine; // m_pRefArc

        /// <summary>
        /// The offset to the parallel (either a <c>Distance</c>, or an <c>OffsetPoint</c>).
        /// </summary>
        Observation m_Offset;        

        // Optional input ...

        /// <summary>
        /// The 1st terminal arc (if any).
        /// </summary>
        LineFeature m_Term1;

        /// <summary>
        /// The 2nd terminal arc (if any). May be the same as <c>m_Term1</c> (e.g. could be a
        /// multi-segment that bends round).
        /// </summary>
        LineFeature m_Term2;
        
        /// <summary>
        /// Flag bits. Currently a value of 0x1 is the only defined value, and means
        /// that the parallel for a circular arc should go in a direction that
        /// is opposite to that of the reference line.
        /// </summary>
        uint m_Flags;

        // Created features ...

        /// <summary>
        /// The parallel that got created.
        /// </summary>
        LineFeature m_ParLine; // m_pParArc

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ParallelOperation()
            : base()
        {
            // Input ...

            m_RefLine = null;
            m_Offset = null;
            m_Term1 = null;
            m_Term2 = null;
            m_Flags = 0;

            // Created features ...

            m_ParLine = null;
        }

        #endregion

        bool CanCorrect
        {
            get { return true; }
        }

        // Needed by ParallelUI for updating ...

        internal LineFeature ReferenceLine { get { return m_RefLine; } }
        internal LineFeature ParallelLine { get { return m_ParLine; } }
        internal Observation Offset { get { return m_Offset; } }
        internal bool IsArcReversed { get { return (m_Flags==1); } }
        internal LineFeature Terminal1 { get { return m_Term1; } }
        internal LineFeature Terminal2 { get { return m_Term2; } }

        internal override Distance GetDistance(LineFeature line)
        {
            return null; // nothing to do
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Parallel"; }
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(7);

                if (m_ParLine!=null)
                {
                    result.Add(m_ParLine);

                    // If the point features at the end of the arc were
                    // created by this op, append them too (inactive
                    // points should be considered).

                    PointFeature ps = CreatedStartPoint;
                    if (ps!=null)
                        result.Add(ps);

                    PointFeature pe = CreatedEndPoint;
                    if (pe!=null)
                        result.Add(pe);
                }

                return result.ToArray();
            }
        }

        internal override bool Undo()
        {
            base.OnRollback();

            // If the offset was an offset point, get rid of reference to this op.
            m_Offset.OnRollback(this);
            m_Offset = null;

            // Cut the reference from the reference line to this op. And
	        // refs from any terminal lines.
            if (m_RefLine!=null)
                m_RefLine.CutOp(this);

            if (m_Term1!=null)
                m_Term1.CutOp(this);

            if (m_Term2!=null)
                m_Term2.CutOp(this);

            // Rollback the points at the end of the parallel (if they
	        // were created by this operation).
	        Rollback(CreatedStartPoint);
	        Rollback(CreatedEndPoint);


            // Mark the parallel line as deleted.
	        Rollback(m_ParLine);
            return true;
        }

        /// <summary>
        /// Rollforward this operation.
        /// </summary>
        /// <returns>True on success</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the end positions of the parallel line. This
            // will throw an exception if the parallel no longer intersects terminating
            // lines (a message is issued to that effect).
            IPosition spar, epar;

            try
            {
                if (!Calculate(m_RefLine, m_Offset, m_Term1, m_Term2, out spar, out epar))
                    return false;
            }

            catch (Exception ex)
            {
                throw new RollforwardException(this, ex.Message);
            }

            // May need the map.
            CadastralMapModel map = CadastralMapModel.Current;

            // Move the end points (in a situation where the end point
            // was not previously created by this operation, and the
            // end does not agree with what we want, add a new point!).
            PointFeature ps = CreatedStartPoint;
            PointFeature pe = CreatedEndPoint;

            if (ps!=null)
                ps.Move(spar);
            else
            {
                // Get the location at the start of the parallel and
                // see if it's already been moved to the position we
                // want. If not, add a point at the desired location,
                // and alter the parallel line so that it starts there
                // instead.

                IPointGeometry loc = m_ParLine.StartPoint;
                IPointGeometry wantLoc = PointGeometry.Create(spar);

                if (!loc.IsCoincident(wantLoc))
                {
                    ps = AddPoint(wantLoc);
                    m_ParLine.IsMoved = true;
                }
            }

            // Same for the end of the parallel
            if (pe!=null)
                pe.Move(epar);
            else
            {
                IPointGeometry loc = m_ParLine.EndPoint;
                IPointGeometry wantLoc = PointGeometry.Create(epar);

                if (!loc.IsCoincident(wantLoc))
                {
                    pe = AddPoint(wantLoc);
                    m_ParLine.IsMoved = true;
                }
            }

            // If the parallel is a circular arc, check if the arc's
            // circle is still suitable. If not, either adjust the circle,
            // or create a new one. We'll adjust the circle if this
            // operation created the circle, and create a new one if it
            // was created by some earlier operation.

            Circle circle = m_ParLine.Circle;
            if (circle!=null)
            {
                // Define geometry based on the circle's centre and the position
                // of the start of the parallel. If the circles don't match, either
                // adjust the circle, or add a replacement one.
                double parrad = Geom.Distance(circle.Center, spar);
                if (Math.Abs(circle.Radius - parrad) > Constants.XYRES)
                {
                    // Ensure the parallel is known to have moved.
                    m_ParLine.IsMoved = true;

                    if (circle.Creator == this)
                    {
                        // Changing the radius of the circle doesn't actually
                        // flag any dependent ops for update. That should
                        // have happened as a consequence of moving the
                        // end points of the parallel (done above). That's
                        // because the curve incident on the end locations
                        // would have been told, which would tell attached
                        // lines, which would tell the dependent ops.
                        circle.ChangeRadius((ArcFeature)m_ParLine, parrad);
                    }
                    else
                    {
                        // Create a new circle.
                        Circle newCircle = map.AddCircle(circle.CenterPoint, parrad);

                        // Transfer the arc to the new circle.
                        ArcFeature arc = (m_ParLine as ArcFeature);
                        arc.Move(newCircle, arc.IsClockwise);
                    }
		        }
            }

            // Ensure that the parallel really does end at the locations where it's expected to be.
            // SS:25-JUN-07... is this still needed?
            // if (m_ParLine.ChangeEnds(ps, pe))
            //     m_ParLine.IsMoved = true;

	        // Rollforward the base class.
	        return base.OnRollforward();
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.Parallel; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation.
        /// </summary>
        public override void AddReferences()
        {
	        if  (m_RefLine!=null)
                m_RefLine.AddOp(this);

	        if (m_Term1!=null)
                m_Term1.AddOp(this);

	        if (m_Term2!=null && m_Term2!=m_Term1)
                m_Term2.AddOp(this);

            m_Offset.AddReferences(this);
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="refLine">The reference line.</param>
        /// <param name="offset">The observed offset (either a <c>Distance</c> or
        /// an <c>OffsetPoint</c>).</param>
        /// <param name="term1">A line that the parallel should start on.</param>
        /// <param name="term2">A line that the parallel should end on.</param>
        /// <param name="isArcReversed">Should circular arc be reversed?</param>
        /// <returns>True if operation executed ok.</returns>
        internal bool Execute( LineFeature refLine
                             , Observation offset
                             , LineFeature term1
                             , LineFeature term2
                             , bool isArcReversed)
        {
            // Calculate the mathematical position of the parallel points.
            IPosition spar, epar;
            if (!Calculate(refLine, offset, term1, term2, out spar, out epar))
                return false;

            // Don't attempt to add a parallel that starts and stops at the same place.
            if (spar.IsAt(epar, Double.Epsilon))
                throw new Exception("Parallel line has the same start and end points.");

            // Remember supplied info.
            m_RefLine = refLine;
            m_Offset = offset;
            m_Term1 = term1;
            m_Term2 = term2;
            m_Flags = (uint)(isArcReversed ? 1 : 0);

            // Add points at the ends of the parallel.
            CadastralMapModel map = CadastralMapModel.Current;
            PointFeature ps = AddPoint(spar);
            PointFeature pe = AddPoint(epar);

            // Add the parallel line to the map (if it's a circular arc,
            // we may need to reverse the direction).

            IEntity ent = EditingController.Current.ActiveLayer.DefaultLineType;

            if (refLine is ArcFeature)
            {
                ArcFeature arc = (refLine as ArcFeature);
                Circle circle = arc.Circle;
                double radius = circle.Radius;
                PointFeature centre = circle.CenterPoint;
                bool iscw = arc.IsClockwise;

                // Need to add a circle first.
                double parRadius = Geom.Distance(centre, spar);
                Circle parCircle = map.AddCircle(centre, parRadius);

                // Use the reverse arc direction if specified.
                if (isArcReversed)
                    iscw = !iscw;

                // Add the circular arc
                m_ParLine = map.AddCircularArc(parCircle, ps, pe, iscw, ent, this);
            }
            else
            {
                m_ParLine = map.AddLine(ps, pe, ent, this);
            }

            // Peform standard completion steps
            Complete();
            return true;
        }

        /// <summary>
        /// Corrects this operation.
        /// </summary>
        /// <param name="refline">The reference line.</param>
        /// <param name="offset">The observed offset (either a <c>Distance</c> or an <c>OffsetPoint</c>).</param>
        /// <param name="term1">A line that the parallel should start on.</param>
        /// <param name="term2">A line that the parallel should end on.</param>
        /// <param name="isArcReversed">Should circular arc be reversed?</param>
        /// <returns>True if operation updated ok.</returns>
        internal bool Correct( LineFeature refline, Observation offset, LineFeature term1, LineFeature term2, bool isArcReversed)
        {
        	// Alter the reference line if necessary.
	        if (m_RefLine != refline )            
            {
                m_RefLine.CutOp(this);
                m_RefLine = refline;
                m_RefLine.AddOp(this);
            }

	        // Cut any references made by the offset. If nothing
	        // has changed, they will be re-inserted when the offset
	        // is re-saved below.
	        if (m_Offset!=null)
                m_Offset.OnRollback(this);

	        // Get rid of the previously defined offset, and replace with
	        // the new one (we can't necessarily change the old ones
	        // because we may have changed the type of observation).
            m_Offset = offset;
            m_Offset.AddReferences(this);

	        // If either terminal line is being changed
	        if (m_Term1!=term1 || m_Term2!=term2)
            {
		        // Remember the new terminal lines (actually splitting them
		        // is the job of Rollforward).

		        if (m_Term1!=null)
                    m_Term1.CutOp(this);

		        if (m_Term2!=null)
                    m_Term2.CutOp(this);

		        m_Term1 = term1;
		        m_Term2 = term2;

        		if (m_Term1!=null)
                    m_Term1.AddOp(this);

		        if (m_Term2!=null)
                    m_Term2.AddOp(this);
	        }

	        // Alter arc direction if necessary.
	        if (isArcReversed)
		        m_Flags = 1;
	        else
		        m_Flags = 0;

	        return true;
        }

        /// <summary>
        /// Adds a termination point (or re-use any point that's already there).
        /// </summary>
        /// <param name="loc">The location for the termination point.</param>
        /// <returns>The point feature at the termination point (may be a previously
        /// existing point).</returns>
        PointFeature AddPoint(IPosition loc)
        {
            CadastralMapModel map = CadastralMapModel.Current;

            // Add the split point (with default entity type). If a
	        // point already exists at the location, you'll get back
	        // that point instead.

	        // We do this in case the user has decided to
	        // terminate on a line connected to the offset point
	        // that defines the offset to the parallel (which the
	        // UI lets the user do).

            PointFeature p = map.EnsurePointExists(loc, this);
            if (p.Creator == this)
                p.SetNextId();

            return p;
        }

        /// <summary>
        /// The point created at the start of the parallel (null if the point
        /// previously existed).
        /// </summary>
        PointFeature CreatedStartPoint
        {
            get
            {
                if (m_ParLine==null)
                    return null;

                PointFeature p = m_ParLine.StartPoint;
                Debug.Assert(p!=null);
                return (p.Creator==this ? p : null);
            }
        }

        /// <summary>
        /// The point created at the end of the parallel (null if the point
        /// previously existed).
        /// </summary>
        PointFeature CreatedEndPoint
        {
            get
            {
                if (m_ParLine==null)
                    return null;

                PointFeature p = m_ParLine.EndPoint;
                Debug.Assert(p!=null);
                return (p.Creator==this ? p : null);
            }
        }

        /// <summary>
        /// Calculates the position of the parallel points
        /// </summary>
        /// <param name="spar">The start of the parallel.</param>
        /// <param name="epar">The end of the parallel.</param>
        /// <returns>True if calculated ok.</returns>
        bool Calculate(out IPosition spar, out IPosition epar)
        {
            return Calculate(m_RefLine, m_Offset, m_Term1, m_Term2, out spar, out epar);
        }

        /// <summary>
        /// Calculates the terminal positions for the parallel.
        /// </summary>
        /// <param name="refLine">The reference line.</param>
        /// <param name="offset">The observed offset (either a <c>Distance</c>
        /// or an <c>OffsetPoint</c>).</param>
        /// <param name="term1">A line that the parallel should start on.</param>
        /// <param name="term2">A line that the parallel should end on.</param>
        /// <param name="spar">The start of the parallel.</param>
        /// <param name="epar">The end of the parallel.</param>
        /// <returns>True if calculated ok.</returns>
        bool Calculate( LineFeature refLine
                      , Observation offset
                      , LineFeature term1
                      , LineFeature term2
                      , out IPosition spar
                      , out IPosition epar )
        {
            spar = epar = null;

	        if (!ParallelUI.Calculate(refLine, offset, out spar, out epar))
                return false;

	        // If the start of the parallel should begin on a specific
	        // line, get the closest intersection.

            IPosition pos = null;
        	if (term1!=null)
            {
                pos = ParallelUI.GetIntersect(refLine, spar, term1);
		        if (pos==null)
                    throw new Exception("Parallel does not intersect terminal line.");
    		}
	    	spar = pos;

        	// And similarly for the end of the parallel.

	        if (term2!=null)
            {
                pos = ParallelUI.GetIntersect(refLine, epar, term2);
                if (pos==null)
                    throw new Exception("Parallel does not intersect terminal line.");
    		}
	    	epar = pos;
	        return true;
	    }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            writer.WriteFeatureReference("RefLine", m_RefLine);
            writer.WriteFeatureReference("Term1", m_Term1);
            writer.WriteFeatureReference("Term2", m_Term2);

            if (IsArcReversed)
                writer.WriteBool("ArcReversed", true);

            writer.WriteElement("Offset", m_Offset);

            // Created features ...
            writer.WriteCalculatedLine("ParLine", m_ParLine);
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);

            m_RefLine = reader.ReadFeatureByReference<LineFeature>("RefLine");
            m_Term1 = reader.ReadFeatureByReference<LineFeature>("Term1");
            m_Term2 = reader.ReadFeatureByReference<LineFeature>("Term2");

            bool isArcReversed = reader.ReadBool("ArcReversed");
            if (isArcReversed)
                m_Flags = 1;

            m_Offset = reader.ReadElement<Observation>("Offset");

            // Calculate the end positions
            IPosition spos, epos;
            if (!Calculate(out spos, out epos))
                throw new Exception("Failed to calculate parallel line positions");

            m_ParLine = reader.ReadCalculatedLine("ParLine", spos, epos);
        }
    }
}
