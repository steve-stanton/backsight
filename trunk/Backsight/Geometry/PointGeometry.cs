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

namespace Backsight.Geometry
{
	/// <written by="Steve Stanton" on="18-OCT-2006" />
    /// <summary>
    /// Implementation of point geometry that consists of a pair of 64-bit integers,
    /// where position is expressed to the nearest micron on the ground.
    /// </summary>
    /// <remarks>
    /// Integer values are used largely for historical reasons, since various items
    /// of software are coded to accommodate the consequences of roundoff.
    /// </remarks>
    [Serializable]
    public class PointGeometry : IPointGeometry
    {
        #region Class data

        /// <summary>
        /// The easting of the point.
        /// </summary>
        private ILength m_X;

        /// <summary>
        /// The northing of the point.
        /// </summary>
        private ILength m_Y;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PointGeometry</c> from the supplied position (or casts
        /// the supplied position if it's already an instance of <c>PointGeometry</c>).
        /// </summary>
        /// <param name="p">The position the geometry should correspond to</param>
        /// <returns>A newly created <c>PointGeometry</c> instance, or the supplied
        /// position if it's already an instance of <c>PointGeometry</c></returns>
        public static PointGeometry Create(IPosition p)
        {
            if (p is PointGeometry)
                return (p as PointGeometry);
            else
                return new PointGeometry(p);
        }

        /// <summary>
        /// Creates a new <c>PointGeometry</c> at the specified position
        /// (rounded to the nearest micron on the ground).
        /// </summary>
        /// <param name="position">The position the geometry should correspond to</param>
        public PointGeometry(IPosition position)
            : this(position.X, position.Y)
        {
        }

        /// <summary>
        /// Creates a new <c>PointGeometry</c> at the specified position.
        /// (rounded to the nearest micron on the ground).
        /// </summary>
        /// <param name="x">The easting of the point, in meters on the ground.</param>
        /// <param name="y">The northing of the point, in meters on the ground.</param>
        public PointGeometry(double x, double y)
        {
            m_X = new MicronValue(x);
            m_Y = new MicronValue(y);
        }

        /// <summary>
        /// Creates a new <c>PointGeometry</c> at the specified position.
        /// </summary>
        /// <param name="xInMicrons">The easting of the point, in microns on the ground.</param>
        /// <param name="yInMicrons">The northing of the point, in microns on the ground.</param>
        public PointGeometry(long xInMicrons, long yInMicrons)
        {
            m_X = new MicronValue(xInMicrons);
            m_Y = new MicronValue(yInMicrons);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public PointGeometry(PointGeometry copy)
            : this(copy.Easting.Microns, copy.Northing.Microns)
        {
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0:0.0000}N {1:0.0000}E", Y, X);
        }

        public double X
        {
            get { return m_X.Meters; }
        }

        public double Y
        {
            get { return m_Y.Meters; }
        }

        public bool IsAt(IPosition p, double tol)
        {
            return Position.IsCoincident(this, p, tol);
        }

        public ILength Distance(IPosition point)
        {
            return new Length(BasicGeom.Distance(this, point));
        }

        public IWindow Extent
        {
            get { return new Window(this); }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, this);
        }

        /// <summary>
        /// Is this point at the same position as another point.
        /// </summary>
        /// <param name="p">The point to compare with</param>
        /// <returns>True if the positions are identical (to the nearest micron)</returns>
        public bool IsCoincident(IPointGeometry that)
        {
            return (this.Easting.Microns==that.Easting.Microns &&
                    this.Northing.Microns==that.Northing.Microns);
        }

        /// <summary>
        /// Checks if a position is coincident with a line segment
        /// </summary>
        /// <param name="p">The position to test</param>
        /// <param name="start">The start of the segment.</param>
        /// <param name="end">The end of the segment.</param>
        /// <param name="tolsq">The tolerance (squared) to use. Default is XYTOLSQ.</param>
        /// <returns>True if the test position lies somewhere along the segment.</returns>
        public static bool IsCoincidentWith(IPointGeometry p, IPointGeometry start, IPointGeometry end, double tolsq)
        {
            // Check whether there is exact coincidence at either end.
	        if (p.IsCoincident(start) || p.IsCoincident(end))
                return true;

            // Get the distance squared of a perpendicular dropped from
            // the test position to the segment (or the closest end if the
            // perpendicular does not fall ON the segment).
            return (BasicGeom.DistanceSquared(p.X, p.Y, start.X, start.Y, end.X, end.Y) < tolsq);
        }

        public ILength Easting
        {
            get { return m_X; }
        }

        public ILength Northing
        {
            get { return m_Y; }
        }

        /// <summary>
        /// Moves this location to a new position.
        /// 
        /// probably irrelevant... The new position must not correspond to any other
        /// location object, which can be confirmed by making an initial call to
        /// CeMap::FindLocation.
        /// </summary>
        /// <param name="to">The new position.</param>
        /*
        internal void Move(IPointGeometry to)
        {
            if (this.IsCoincident(to))
                return;

            m_X = to.E;
            m_Y = to.N;
         */
            /*
            // Get a list of all the things that this location
            // currently references. It may change when we do the
            // subsequent call to CePrimitive::OnMove. See further
            // explanation below.
             * 
	        CeObjectList* pOrig=0;
	        CeClass* pObjects = this->GetpObjects();

	        if ( pObjects ) {
		        if ( this->IsObjectList() ) {
			        const CeObjectList& list = dynamic_cast<const CeObjectList&>(*pObjects);
			        pOrig = new CeObjectList(list);
		        }
		        else {
			        pOrig = new CeObjectList();
			        pOrig->Append(pObjects);
		        }
	        }

            // Get the base class to do its stuff
	        if ( !this->IsTransient() ) CePrimitive::OnMove(this);

            //	Remember to expand the map window if the new location is
            //	beyond the current limits.

	        const LOGICAL transient = this->IsTransient();
	        CeMap* pMap=0;

	        if ( !transient ) {
		        pMap = CeMap::GetpMap();
		        const CeWindow* const pWin = pMap->GetpWindow();
		        if ( !pWin->IsOverlap(toloc) ) {
			        CeWindow newwin(*pWin);
			        newwin.Expand(toloc);
			        pMap->SetExtent(newwin);
		        }
	        }

    //	If the destination is in the same tile as the original,
    //	all we have to do is move the position.

	    if ( *(this->m_pTileId) == *(toloc.m_pTileId) ) {

    //		Change the position.
		    this->m_X = toloc.m_X;
		    this->m_Y = toloc.m_Y;

	    }
	    else {

    //		Update spatial index to reference this location
    //		from the new tile. If the location is not transient,
    //		THIS will be modified to point to a tile ID that is
    //		part of the spatial index. If the location IS transient,
    //		we copy the pointer to the ID & then null out the original
    //		pointer so that the location destructor will not blow the
    //		ID away.

		    if ( !transient )
			    pMap->GetSpace().Move(this,*(toloc.m_pTileId));
		    else {
			    this->m_pTileId = toloc.m_pTileId;
			    toloc.m_pTileId = 0;
		    }

    //		Copy the new position (but NOT any base class stuff).
		    this->m_X = toloc.m_X;
		    this->m_Y = toloc.m_Y;
	    }

    //	Get the base class to update the spatial index for any
    //	attached lines. 

    //	The location might not reference the same things that
    //	it originally did. We need to pass in the original
    //	list to cover the fact that CePrimitive::OnMove may
    //	have called CeMultiSegment::OnMove. In that case, if
    //	this location is an intermediate vertex, the multi-segment
    //	would have been changed to refer to a new location (at
    //	the original position of this location). Furthermore,
    //	any sections terminating at the old location will now
    //	be pointing to a different location, so this location
    //	will not point back either. By using the original list,
    //	we ensure that everything gets re-indexed.

	    CePrimitive::OnPostMove(pOrig);
	    delete pOrig;
             */
    }
}
