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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="05-JUL-2007" />
    /// <summary>
    /// A boundary between two polygons.
    /// </summary>
    [Serializable]
    class Boundary : IFeatureDependent, IPossibleList<Boundary>
    {
        #region Class data

        /// <summary>
        /// The line this boundary coincides with (the boundary may coincide with the
        /// complete line, or just a portion of it (<see cref="BoundarySection"/>)).
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// The polygon ring to the left of this boundary.
        /// </summary>
        Ring m_Left;

        /// <summary>
        /// The polygon ring to the right of this boundary.
        /// </summary>
        Ring m_Right;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new polygon <c>Boundary</c> that coincides with the specified line.
        /// </summary>
        /// <param name="line">The line the boundary coincides with.</param>
        internal Boundary(LineFeature line)
        {
            m_Line = line;
            m_Left = null;
            m_Right = null;
        }

        #endregion

        /// <summary>
        /// The line feature this boundary coincides with.
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
        }

        /// <summary>
        /// The position of the start of this boundary (coincides with the start
        /// of the associated line)
        /// </summary>
        internal virtual ITerminal Start
        {
            get { return m_Line.StartPoint; }
        }

        /// <summary>
        /// The position of the end of this boundary (coincides with the end
        /// of the associated line)
        /// </summary>
        internal virtual ITerminal End
        {
            get { return m_Line.EndPoint; }
        }

        /// <summary>
        /// Gets the orientation point for this boundary. This is utilized to form
        /// network topology at the ends of a boundary.
        /// </summary>
        /// <param name="fromStart">True if the orientation from the start of the boundary is
        /// required. False to get the end orientation.</param>
        /// <param name="crvDist">Orientation distance for circular arcs (irrelevant if
        /// the boundary isn't associated with a circular arc). Default=0.0</param>
        /// <returns>The orientation point.</returns>
        internal IPosition GetOrient(bool fromStart, double crvDist)
        {
            return GetLineGeometry().GetOrient(fromStart, crvDist);
        }

        public void OnPreMove(Feature f)
        {
            // This method should never get called. When the referenced line is notified
            // that a move is about to take place, it should remove references to all
            // associated boundaries (and mark itself as moved).
            throw new InvalidOperationException();
        }

        public void OnPostMove(Feature f)
        {
            // This method should never get called. See comment in OnPreMove.
            throw new InvalidOperationException();
        }

        public void AddReferences()
        {
            m_Line.AddReference(this);
        }

        /// <summary>
        /// Does this boundary fall on any layers that overlap the supplied layer list.
        /// </summary>
        /// <param name="layers">The layers of interest</param>
        /// <returns>True if this boundary falls on at least one of the layers in the
        /// supplied layer list.</returns>
        /*
        internal bool IsOverlap(LayerList layers)
        {
            return true;
            //throw new NotImplementedException();
        }
        */

        /// <summary>
        /// Ensures that the polygon topology for this line has been completely defined.
        /// </summary>
        /// <param name="bwin">The window of any new polygons that got created. This
        /// window is not initialized here. It just gets expanded.</param>
        /// <param name="index">The spatial index to include any newly built polygons.</param>
        internal void BuildPolygons(Window bwin, IEditSpatialIndex index)
        {
            if (m_Left==null)
                m_Left = BuildSide(bwin, index, true);

            if (m_Right==null)
                m_Right = BuildSide(bwin, index, false);
        }

        Ring BuildSide(Window bwin, IEditSpatialIndex index, bool isLeft)
        {
            List<BoundaryFace> edge = GetPolygonBoundaries(isLeft);
            Ring result = Ring.Create(edge);
            bwin.Union(result.Extent);
            result.AddToIndex(index);
            return result;
        }

        /// <summary>
        /// Get the boundaries defining a new polygon ring, starting with this boundary.
        /// </summary>
        /// <param name="isPolLeft">Is the polygon ring to the left of this boundary?</param>
        /// <returns>The boundaries making up the new ring</returns>
        List<BoundaryFace> GetPolygonBoundaries(bool isPolLeft)
        {
            List<BoundaryFace> result = new List<BoundaryFace>(10);

            bool isLeft = isPolLeft;
            Boundary b = this;
            uint nface = 0;

            // Cycle the polygon until we return to the first boundary.
            for (; ; )
            {
                // Define the next boundary face
                BoundaryFace face = new BoundaryFace(b, isLeft);
                nface++;

                // If we've created quite a few faces (more than you'd expect), confirm there are no duplicate faces.
                if ((nface % 256)==0 && GetDuplicateFace(result)!=null)
                {
                    Boundary bad = GetDuplicateFace(result).Boundary;
                    string msg = String.Format("Boundary.GetPolygonBoundaries - Duplicate face from ({0}) to ({1}) [Line {2}]",
                                                bad.Start.ToString(), bad.End.ToString(), bad.Line.DataId);
                    DumpFaces(msg, result);
                    throw new Exception(msg);
                }

                result.Add(face);

                // Get the next boundary in the cycle.
                ConnectionFinder connect = new ConnectionFinder();
                if (!connect.Create(b, isLeft))
                    throw new Exception("Cannot find connecting boundary");

		        // If we connected to the start of another boundary, the polygon is
		        // to the right. The end of the next boundary means the polygon is
		        // to the left.
		        isLeft = !connect.IsStart;
                b = connect.Next;

        		// Break out of loop if we have come back to the first boundary.
		        if (b==this && isLeft==isPolLeft)
                    break;

                /*
		        // If the layers for the arc we found are a subset of the
		        // previous arc, start using the subset. Note that the subset
		        // may have gaps in the layer hierarchy (e.g. 1-2-3 overlaying
		        // something that exists only in layer 2 will produce 1-3).
		        const CeLayerList curlayers(*pArc);
		        layers.Overlay(curlayers);
                 */
            }

            return result;
        }

        /// <summary>
        /// Scans a list of faces to see if there are any duplicates.
        /// This is an O(n-squared) check, so be sparing with calls to this function.
        /// </summary>
        /// <param name="faces">The faces to check.</param>
        /// <returns>The first duplicate face (if any). Null if there aren't any duplicates.</returns>
        BoundaryFace GetDuplicateFace(List<BoundaryFace> faces)
        {
            for (int i=0; i<faces.Count; i++)
            {
                // Get the face to check.
                BoundaryFace iface = faces[i];

                // Loop through all faces, looking for a match (but
                // ignoring the face that we're checking).
                for (int j=0; j<faces.Count; j++)
                {
                    BoundaryFace jface = faces[j];
                    if (i!=j && iface.Equals(jface))
                        return iface;
                }

            }

            return null;
        }

        /// <summary>
        /// Dumps a list of boundary faces to a text file called C:\Temp\Debug.txt.
        /// </summary>
        /// <param name="msg">An error message to write to the dump file.</param>
        /// <param name="faces">The faces to write out.</param>
        void DumpFaces(string msg, List<BoundaryFace> faces)
        {
            using (StreamWriter sw = File.CreateText(@"C:\Temp\Debug.txt"))
            {
                sw.WriteLine(msg);

                for (int i=0; i<faces.Count; i++)
                {
                    BoundaryFace face = faces[i];
                    string line = String.Format("[{0}] {1} {2}", i, face.Boundary.Line.DataId, face.IsLeft.ToString());
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Have polygons been defined for both sides of this boundary?
        /// </summary>
        internal bool IsBuilt
        {
            get { return (m_Left!=null && m_Right!=null); }
        }

        /// <summary>
        /// The polygon to the left of this boundary.
        /// </summary>
        internal Ring Left
        {
            get { return m_Left; }
            set { m_Left = value; }
        }

        /// <summary>
        /// The polygon to the right of this boundary.
        /// </summary>
        internal Ring Right
        {
            get { return m_Right; }
            set { m_Right = value; }
        }

        /// <summary>
        /// Gets geometric info for this boundary. For use during the formation
        /// of <c>Polygon</c> objects.
        /// </summary>
        /// <todo>The BoundarySection class needs to override.</todo>
        /// <param name="window">The window of the boundary</param>
        /// <param name="area">The area (in square meters) between the boundary and the Y-axis.</param>
        /// <param name="length">The length of the boundary (in meters on the (projected) ground).</param>
        internal void GetGeometry(out IWindow window, out double area, out double length)
        {
            GetLineGeometry().GetGeometry(out window, out area, out length);
        }

        /// <summary>
        /// Calculates the distance from the start of this boundary to a specific position (on the map projection)
        /// </summary>
        /// <param name="asFarAs">Position on the boundary that you want the length to. Specify
        /// null for the length of the whole boundary.</param>
        /// <returns>The length. Less than zero if a position was specified and it is
        /// not on the boundary.</returns>
        internal ILength GetLength(IPosition asFarAs)
        {
            return GetLineGeometry().GetLength(asFarAs);
        }

        /// <summary>
        /// Gets the most easterly position for this boundary. If more than one position has the
        /// same easting, one of them will be picked arbitrarily.
        /// </summary>
        /// <returns>The most easterly position</returns>
        internal IPosition GetEastPoint()
        {
            return GetLineGeometry().GetEastPoint();
        }

        /// <summary>
        /// Determines which side of a boundary a horizontal line segment lies on.
        /// Used in point in polygon.
        /// </summary>
        /// <param name="s">Start of horizontal segment.</param>
        /// <param name="e">End of segment.</param>
        /// <param name="b">The boundary the side refers to. This usually refers to
        /// this instance, but may refer to another boundary in a situation where the
        /// horizontal segment passes directly through one end of this boundary.</param>
        /// <returns>Side.Left if the line is to the left of the boundary, Side.Right
        /// if line to the right, Side.On if the end of the line is coincident with
        /// the boundary, Side.Unknown if an error arose.</returns>
        /// <remarks>
        /// Rather than passing in 2 PointGeometry objects, it would be better to pass
        /// in a HorizontalRay object, since 2 arbitrary positions are not guaranteed
        /// to be horizontal.
        /// </remarks>
        internal Side GetSide(IPointGeometry s, IPointGeometry e, out Boundary b)
        {
            // If the end of the horizontal segment hits the start or
            // the end of this boundary, we have a situation where the boundary
            // may not be the boundary which is adjacent to the segment. In
            // that case, use special code to determine the side code.
            // Otherwise just convert this boundary into a line, and get the
            // side code by looking for a vertex which has a different
            // northing from that of the horizontal segment.

            Debug.Assert(s.Easting.Microns <= e.Easting.Microns);
            double d = e.Easting.Meters - s.Easting.Meters;
            Debug.Assert(d>=0.0);
            HorizontalRay hseg = new HorizontalRay(s, d);

            if (e.IsCoincident(Start) || e.IsCoincident(End)) // e both times!
                return hseg.GetSide(this, e.IsCoincident(Start), out b);
            else
            {
                b = this;
                return GetLineGeometry().GetSide(hseg);
            }
        }

        /// <summary>
        /// Cuts back a horizontal line segment to the closest intersection with this boundary.
        /// Used in point in polygon.
        /// </summary>
        /// <param name="s">Start of horizontal segment.</param>
        /// <param name="e">End of segment (will be modified if segment intersects this boundary)</param>
        /// <param name="status">Return code indicating whether an error has arisen (returned
        /// as 0 if no error).</param>
        /// <returns>True if the horizontal line was cut back.</returns>
        internal bool GetCloser(IPointGeometry s, ref PointGeometry e, out uint status)
        {
            return GetLineGeometry().GetCloser(s, ref e, out status);
        }

        /// <summary>
        /// The geometry for this boundary. This implementation returns the geometry for the
        /// complete line. The <see cref="BoundarySection"/> class overrides.
        /// </summary>
        /// <returns>The geometry for this boundary</returns>
        internal virtual LineGeometry GetLineGeometry()
        {
            // The line geometry may actually be an instance of SectionGeometry.
            return m_Line.LineGeometry;
        }

        /// <summary>
        /// Initializes this boundary upon loading of the model that contains it. This clears
        /// the indexing flag that is defined as part of any neighbouring polygons.
        /// </summary>
        internal void OnLoad()
        {
            if (m_Left!=null)
                m_Left.IsIndexed = false;

            if (m_Right!=null)
                m_Right.IsIndexed = false;
        }

        /// <summary>
        /// Inserts neighbouring polygons into the supplied index (if they are not already
        /// marked as indexed). This should be called shortly after a model is opened (after
        /// a prior call to <c>OnLoad</c>).
        /// </summary>
        /// <param name="index">The spatial index to add to</param>
        internal void AddToIndex(IEditSpatialIndex index)
        {
            if (m_Left!=null)
                m_Left.AddToIndex(index);

            if (m_Right!=null)
                m_Right.AddToIndex(index);
        }

        public override string ToString()
        {
            return String.Format("L={0} R={1}", (m_Left==null ? "n/a" : m_Left.ToString())
                                              , (m_Right==null ? "n/a" : m_Right.ToString()));
        }

        #region IPossibleList support

        public int Count
        {
            get { return 1; }
        }

        public Boundary this[int index]
        {
            get
            {
                if (index!=0)
                    throw new ArgumentOutOfRangeException();

                return this;
            }
        }

        public IPossibleList<Boundary> Add(Boundary thing)
        {
            return new BasicList<Boundary>(this, thing);
        }

        public IPossibleList<Boundary> Remove(Boundary thing)
        {
            if (!Object.ReferenceEquals(this, thing))
                throw new ArgumentException();

            return null;
        }

        public IEnumerator<Boundary> GetEnumerator()
        {
            yield return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // not this one, the other one
        }

        #endregion

        /// <summary>
        /// Performs processing when the line associated with this boundary is about to be de-activated
        /// as part of a <see cref="DeletionOperation"/>. This marks adjacent polygons for deletion.
        /// </summary>
        internal void OnDeleteLine()
        {
            if (m_Left!=null)
                m_Left.IsDeleted = true;

            if (m_Right!=null)
                m_Right.IsDeleted = true;
        }
    }
}
