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
using System.IO;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-OCT-2007" />
    /// <summary>
    /// Topology for a line. Base class for <see cref="LineTopology"/> and
    /// <see cref="SectionTopologyList"/>
    /// </summary>
    abstract class Topology : IEnumerable<IDivider>
    {
        #region Statics

        /// <summary>
        /// Creates new topology for the specified line.
        /// </summary>
        /// <param name="line">The line the topology is associated with</param>
        /// <returns>New topology for the line (with undefined topological relationships)</returns>
        internal static Topology CreateTopology(LineFeature line)
        {
            // Return topology that's suitable for representing the complete line. This
            // may need to be converted into a SectionTopologyList once we get around to
            // calculating intersections.
            return new LineDivider(line);
        }

        /// <summary>
        /// Have polygons been defined for both sides of the supplied divider?
        /// </summary>
        /// <param name="d">The divider to check</param>
        /// <returns>True if both left and right polygons are not null</returns>
        internal static bool IsBuilt(IDivider d)
        {
            return (d.Left!=null && d.Right!=null);
        }

        /// <summary>
        /// Marks polygons adjacent to a divider for deletion.
        /// </summary>
        /// <param name="d">The divider to process</param>
        internal static void MarkPolygons(IDivider d)
        {
            if (d!=null)
            {
                MarkLeft(d);
                MarkRight(d);
            }
        }

        /// <summary>
        /// Goes through each divider that is incident on the supplied terminal, marking adjacent
        /// polygons for deletion.
        /// </summary>
        /// <param name="t">The terminal position</param>
        internal static void MarkPolygons(ITerminal t)
        {
            IDivider[] divs = t.IncidentDividers();

            if (divs!=null)
            {
                foreach (IDivider d in divs)
                    MarkPolygons(d);
            }
        }

        /// <summary>
        /// Marks the left of a divider for deletion.
        /// </summary>
        /// <param name="d">The divider to process</param>
        internal static void MarkLeft(IDivider d)
        {
            Ring left = d.Left;
            if (left!=null)
                left.IsDeleted = true;
        }

        /// <summary>
        /// Marks the right of a divider for deletion.
        /// </summary>
        /// <param name="d">The divider to process</param>
        internal static void MarkRight(IDivider d)
        {
            Ring right = d.Right;
            if (right!=null)
                right.IsDeleted = true;
        }

        /// <summary>
        /// Determines which side of a divider a horizontal line segment lies on.
        /// Used in point in polygon.
        /// </summary>
        /// <param name="id">The divider to process</param>
        /// <param name="s">Start of horizontal segment.</param>
        /// <param name="e">End of segment.</param>
        /// <param name="od">The divider the side refers to. This usually refers to
        /// the supplied divider, but may refer to another divider in a situation where the
        /// horizontal segment passes directly through one end of <paramref name="d"/>.</param>
        /// <returns>Side.Left if the line is to the left of the divider, Side.Right
        /// if line to the right, Side.On if the end of the line is coincident with
        /// the divider, Side.Unknown if an error arose.</returns>
        /// <remarks>
        /// Rather than passing in 2 PointGeometry objects, it would be better to pass
        /// in a HorizontalRay object, since 2 arbitrary positions are not guaranteed
        /// to be horizontal.
        /// </remarks>
        internal static Side GetSide(IDivider id, IPointGeometry s, IPointGeometry e, out IDivider od)
        {
            // If the end of the horizontal segment hits the start or
            // the end of the specified divider, we have a situation where the divider
            // may not be the divider which is adjacent to the segment. In
            // that case, use special code to determine the side code.
            // Otherwise just convert the supplied divider into a line, and get the
            // side code by looking for a vertex which has a different
            // northing from that of the horizontal segment.

            Debug.Assert(s.Easting.Microns <= e.Easting.Microns);
            double d = e.Easting.Meters - s.Easting.Meters;
            Debug.Assert(d>=0.0);
            HorizontalRay hseg = new HorizontalRay(s, d);

            if (e.IsCoincident(id.From) || e.IsCoincident(id.To)) // e both times!
                return hseg.GetSide(id, e.IsCoincident(id.From), out od);
            else
            {
                od = id;
                return od.LineGeometry.GetSide(hseg);
            }
        }

        /// <summary>
        /// Initializes a divider upon loading of the model that contains it. This clears
        /// the indexing flag that is defined as part of any neighbouring polygons.
        /// </summary>
        /// <param name="d">The divider to process</param>
        internal static void OnLoad(IDivider d)
        {
            if (d.Left!=null)
                d.Left.IsIndexed = false;

            if (d.Right!=null)
                d.Right.IsIndexed = false;

            OnLoadTerminal(d.From);
            OnLoadTerminal(d.To);
        }

        /// <summary>
        /// Initializes a terminal at one end of a divider. If the terminal is an instance
        /// of <see cref="Intersection"/>, the flag bit indicating whether the intersection is
        /// indexed will be cleared.
        /// </summary>
        /// <param name="t"></param>
        static void OnLoadTerminal(ITerminal t)
        {
            if (t is Intersection)
                (t as Intersection).IsIndexed = false;
        }

        /// <summary>
        /// Inserts neighbouring polygons into the supplied index (if they are not already
        /// marked as indexed). This should be called shortly after a model is opened (after
        /// a prior call to <c>OnLoad</c>).
        /// </summary>
        /// <param name="d">The divider to process</param>
        /// <param name="index">The spatial index to add to</param>
        internal static void AddToIndex(IDivider d, EditingIndex index)
        {
            if (d.Left!=null)
                d.Left.AddToIndex(index);

            if (d.Right!=null)
                d.Right.AddToIndex(index);

            AddToIndex(d.From, index);
            AddToIndex(d.To, index);
        }

        /// <summary>
        /// Ensures the terminal at one end of a divider has been indexed.
        /// </summary>
        /// <param name="t">The terminal at the start or end of a divider</param>
        /// <param name="cx">The spatial index to add to</param>
        static void AddToIndex(ITerminal t, EditingIndex cx)
        {
            Intersection x = (t as Intersection);
            if (x!=null && !x.IsIndexed)
                cx.AddIntersection(x);
        }

        /// <summary>
        /// Does one end of a divider dangle?
        /// </summary>
        /// <param name="d">The divider of interest</param>
        /// <param name="loc">The end to check (either the start or end of the supplied divider)</param>
        /// <returns>True if end dangles</returns>
        internal static bool IsDangle(IDivider d, ITerminal loc)
        {
            Debug.Assert(!d.IsOverlap);
            Debug.Assert(d.From == loc || d.To == loc);

            // Get the adjacent polygons
            Ring pL = d.Left;
            Ring pR = d.Right;

            // If either polygon is undefined, treat it as a dangle.
            if (pL==null || pR==null)
                return true;

            // If the divider points to 2 different polygons on each side, it can't be dangling.
            if (pL!=pR)
                return false;

            // That should cover most cases. The only other case is a situation where the divider is
            // a bridging line that connects two islands. Get the end location to see if that's the case.
            IDivider[] divs = loc.IncidentDividers();
            Debug.Assert(Array.IndexOf(divs, d)>=0);
            return (divs.Length==1);
        }

        #endregion

        #region Class data

        /// <summary>
        /// The line the topology relates to (not null).
        /// </summary>
        readonly LineFeature m_Line;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Topology</c> that relates to the specified line.
        /// </summary>
        /// <param name="line">The line the topology relates to (not null)</param>
        internal Topology(LineFeature line)
        {
            if (line==null)
                throw new ArgumentNullException();

            m_Line = line;
        }

        #endregion

        #region IEnumerable<IDivider> Members

        /// <summary>
        /// Provides support for foreach loops
        /// </summary>
        /// <returns>Enumerator on the divider(s) that correspond to this topology</returns>
        abstract public IEnumerator<IDivider> GetEnumerator();

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // not this one, the other one
        }

        #endregion

        public LineFeature Line // IDivider
        {
            get { return m_Line; }
        }

        /// <summary>
        /// The divider at the start of the associated line.
        /// </summary>
        abstract internal IDivider FirstDivider { get; }

        /// <summary>
        /// The divider at the end of the associated line.
        /// </summary>
        abstract internal IDivider LastDivider { get; }

        /// <summary>
        /// Appends dividers that terminate at the specified point. Excludes
        /// overlaps.
        /// </summary>
        /// <param name="result">The list to append to</param>
        /// <param name="t">The point the dividers must terminate at</param>
        internal void AddIncidentDividers(List<IDivider> result, ITerminal t)
        {
            // We could probably exit after appending two dividers, but let's
            // not try to be too smart unless there's a performance issue.

            foreach (IDivider d in this)
            {
                if ((d.From.IsCoincident(t) || d.To.IsCoincident(t)) && !d.IsOverlap)
                    result.Add(d);
            }
        }

        /// <summary>
        /// Ensures that the polygon topology for this line has been completely defined.
        /// This implementation is suitable only for overlapping dividers, since it does nothing.
        /// </summary>
        /// <param name="bwin">The window of any new polygons that got created. This
        /// window is not initialized here. It just gets expanded.</param>
        /// <param name="index">The spatial index to include any newly built polygons.</param>
        internal void BuildPolygons(Window bwin, EditingIndex index)
        {
            foreach (IDivider d in this)
            {
                if (!d.IsOverlap)
                {
                    if (d.Left==null)
                        d.Left = BuildSide(d, bwin, index, true);

                    if (d.Right==null)
                        d.Right = BuildSide(d, bwin, index, false);
                }
            }
        }

        Ring BuildSide(IDivider d, Window bwin, EditingIndex index, bool isLeft)
        {
            List<Face> edge = GetPolygonFaces(d, isLeft);
            Ring result = Ring.Create(edge);
            bwin.Union(result.Extent);
            result.AddToIndex(index);
            return result;
        }

        /// <summary>
        /// Get the dividers defining a new polygon ring, starting with the specified divider.
        /// </summary>
        /// <param name="start">The first divider for the new ring</param>
        /// <param name="isPolLeft">Is the polygon ring to the left of the divider?</param>
        /// <returns>The dividers making up the new ring</returns>
        List<Face> GetPolygonFaces(IDivider start, bool isPolLeft)
        {
            List<Face> result = new List<Face>(10);

            bool isLeft = isPolLeft;
            IDivider d = start;
            uint nface = 0;

            // Cycle the polygon until we return to the first divider.
            for (; ; )
            {
                // Define the next face
                Face face = new Face(d, isLeft);
                nface++;

                // If we've created quite a few faces (more than you'd expect), confirm there are no duplicate faces.
                if ((nface % 256)==0 && GetDuplicateFace(result)!=null)
                {
                    IDivider bad = GetDuplicateFace(result).Divider;
                    string msg = String.Format("Topology.GetPolygonDividers - Duplicate face from ({0}) to ({1}) [Line {2}]",
                                                bad.From.ToString(), bad.To.ToString(), bad.Line.InternalId);
                    DumpFaces(msg, result);
                    throw new Exception(msg);
                }

                result.Add(face);

                // Get the next divider in the cycle.
                ConnectionFinder connect = new ConnectionFinder();
                if (!connect.Create(d, isLeft))
                    throw new Exception("Cannot find connecting boundary");

                // If we connected to the start of another divider, the polygon is
                // to the right. The end of the next divider means the polygon is
                // to the left.
                isLeft = !connect.IsStart;
                d = connect.Next;

                // Break out of loop if we have come back to the first divider.
                if (d==start && isLeft==isPolLeft)
                    break;
            }

            return result;
        }

        /// <summary>
        /// Scans a list of faces to see if there are any duplicates.
        /// This is an O(n-squared) check, so be sparing with calls to this function.
        /// </summary>
        /// <param name="faces">The faces to check.</param>
        /// <returns>The first duplicate face (if any). Null if there aren't any duplicates.</returns>
        Face GetDuplicateFace(List<Face> faces)
        {
            for (int i=0; i<faces.Count; i++)
            {
                // Get the face to check.
                Face iface = faces[i];

                // Loop through all faces, looking for a match (but
                // ignoring the face that we're checking).
                for (int j=0; j<faces.Count; j++)
                {
                    Face jface = faces[j];
                    if (i!=j && iface.Equals(jface))
                        return iface;
                }

            }

            return null;
        }

        /// <summary>
        /// Dumps a list of faces to a text file called <c>C:\Temp\Debug.txt</c>
        /// </summary>
        /// <param name="msg">An error message to write to the dump file.</param>
        /// <param name="faces">The faces to write out.</param>
        void DumpFaces(string msg, List<Face> faces)
        {
            using (StreamWriter sw = File.CreateText(@"C:\Temp\Debug.txt"))
            {
                sw.WriteLine(msg);

                for (int i=0; i<faces.Count; i++)
                {
                    Face face = faces[i];
                    string side = (face.IsLeft ? "L" : "R");
                    string line = String.Format("[{0}] {1} {2}", i, side, face.Divider.ToString());
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Performs any processing when the line associated with this topology
        /// is being de-activated.  This should mark adjacent polygons for deletion, and
        /// remove line references from any intersections.
        /// </summary>
        abstract internal void OnLineDeactivation();

        /*

	// If the specified arc is not the parent arc, do an insert.
	if ( m_pArc != &arc ) return InsertSplit(arc,*pX);

	if ( m_pResult ) {
		ShowMessage("CeSplit::Create\nLine already has splits");
		return 0;
	}

	// Get the end locations.
	CeLocation& sloc = *(m_pArc->GetpStart());
	CeLocation& eloc = *(m_pArc->GetpEnd());

	// Make sure we don't add a null section.
	if ( pX==&sloc || pX==&eloc ) {

//		CString msg;
//		msg.Format("%s\n%s\n%.3lfN %.3lfE"
//			, "CeSplit::Create"
//			, "Attempt to add null section"
//			, sloc.GetNorthing(), sloc.GetEasting() );
//
//		ShowMessage(msg);

		// Only add valid sections.
		CeArc* pArc1 = 0;
		CeArc* pArc2 = 0;
		if ( pX!=&sloc ) pArc1 = AddSection(*m_pArc,sloc,*pX,0);
		if ( pX!=&eloc ) pArc2 = AddSection(*m_pArc,*pX,eloc,0);

		// Mark the parent arc as having system-defined topological sections.
		m_pArc->SetSysTopology(TRUE);

		// Return the arc AFTER the intersection.
		if ( pArc2 )
			return pArc2;
		else
			return pArc1;

	}
	else {

		// Add two sections.
		CeArc* pArc1 = AddSection(*m_pArc,sloc,*pX,0);
		CeArc* pArc2 = AddSection(*m_pArc,*pX,eloc,0);

		// Mark the parent arc as having system-defined topological sections.
		m_pArc->SetSysTopology(TRUE);

		// Return the arc AFTER the intersection.
		return pArc2;
	}

} // end of Create
         */

        /// <summary>
        /// Merges divider sections that are incident on the specified intersection. This gets
        /// called when one of the lines causing an intersection is being removed (deactivated).
        /// </summary>
        /// <param name="x">An intersection that is being removed</param>
        /// <returns>The number of sections after the merge (always 0)</returns>
        internal virtual int MergeSections(Intersection x)
        {
            // This implementation does nothing. The SectionTopologyList class should override.
            return 0;
        }
    }
}
