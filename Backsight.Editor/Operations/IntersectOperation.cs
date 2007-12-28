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

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="08-NOV-1997" was="CeIntersect" />
    /// <summary>
    /// An intersect is a COGO operation used to generate a point where two
    /// lines intersect.
    /// </summary>
    [Serializable]
    abstract class IntersectOperation : Operation
    {
        #region Class data

        // No data

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates a new <c>IntersectionOperation</c>
        /// </summary>
        internal IntersectOperation()
        {
        }

        #endregion

        /// <summary>
        /// The point feature at the intersection created by this edit.
        /// </summary>
        abstract internal PointFeature IntersectionPoint { get; } // was GetpIntersect

        /// <summary>
        /// Was the intersection created at it's default position?
        /// </summary>
        abstract internal bool IsDefault { get; }

        /// <summary>
        /// A point feature that is close to the intersection (for use when relocating
        /// the intersection as part of rollforward processing).
        /// </summary>
        abstract internal PointFeature ClosePoint { get; }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The superseded line that the line of interest was derived from. Null if
        /// this edit did not create the line of interest.</returns>
        internal virtual LineFeature GetPredecessor(LineFeature line)
        {
            // This edit doesn't supersede anything
            return null;
        }

        /// <summary>
        /// Returns true, to indicate that this edit can be corrected.
        /// </summary>
        internal bool CanCorrect
        {
            get { return true; }
        }

        /*
public:
	virtual	LOGICAL		GetReCentre		( const CeWindow& drawin
										, CeVertex& centre ) const;

protected:
	virtual	CePoint*	AddIntersection	( const CeVertex& xsect
										, const CeIdHandle& pointId );
	virtual CePoint*	AddIntersection ( const CeVertex& xsect
										, const CeEntity* const pPointType );
	virtual void		OnRollback		( void );
	virtual LOGICAL		OnRollforward	( void );
	virtual void		SplitPostMove	( CeArc* pSplit1
										, CeArc* pSplit2
										, CeArc& line );
	virtual	LOGICAL		SplitLine		( const CePoint& xsect
										, CeArc& line
										, CeArc*& pNew1
										, CeArc*& pNew2 );

         */

        /// <summary>
        /// Adds a new intersection point to the map.
        /// </summary>
        /// <param name="xsect">Position of the intersection.</param>
        /// <param name="pointId">The ID and entity type for the intersection point.
        /// If null, the default entity type for point features will be used.</param>
        /// <returns>The new point feature</returns>
        protected PointFeature AddIntersection(IPosition xsect, IdHandle pointId)
        {
            CadastralMapModel map = MapModel;
            if (pointId==null)
                return map.AddPoint(xsect, map.DefaultPointType, this);

            PointFeature p = map.AddPoint(xsect, pointId.Entity, this);
            pointId.CreateId(p);
            return p;
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// This implementation always returns null.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// Adds to list of features created by this operation. This appends the
        /// supplied line (if it's not null) to the results, and may also add the
        /// point at the start of the line (this covers a situation where an offset
        /// to the start of the line was specified). Note that the end point of the
        /// line is not checked, since that should correspond to the intersect point.
        /// </summary>
        /// <param name="line">A line created by this operation (may be null)</param>
        /// <param name="result">The list to append to</param>
        /// <remarks>This method may be unecessary - I suspect the UI may block an
        /// attempt to add a line when an offset is involved, need to check.</remarks>
        protected void AddCreatedFeatures(LineFeature line, List<Feature> result)
        {
            if (line!=null)
            {
                result.Add(line);

                PointFeature start = line.StartPoint;
                if (Object.ReferenceEquals(start.Creator, this))
                    result.Add(start);
            }
        }

        /// <summary>
        /// Determines whether a pair of line sections (as created by IntersectTwoLinesOperation or
        /// IntersectDirectionAndLineOperation) are coincident with the intersection point.
        /// </summary>
        /// <param name="split">Line that should be prior to intersection (null if no split)</param>
        /// <returns>True if the specified lines is not null, and the location at the end of
        /// the line matches the location of the intersection point.</returns>
        /// <remarks>I'm not sure why this method exists. A comment in a derived class uselessly
        /// informs me that "Defective logic means the intersection point may not coincide with the
        /// location that's common to split sections". However, the way the result is then used looks
        /// highly suspect.</remarks>
        //protected bool IsSplitAtIntersection(LineFeature split)
        //{
        //    if (split==null)
        //        return false;

        //    // Get the common end location
        //    IPointGeometry pE = split.End;
        //    IPointGeometry pX = IntersectionPoint.PointGeometry;
        //    return pE.IsCoincident(pX);
        //}



        /// <summary>
        /// Split a line at an intersection point. If the intersection is coincident with the
        /// start or end of the line, nothing is done. However, if the intersection falls somewhere
        /// along the line, 2 new user-perceived lines will be created, having the same entity type
        /// (and key) as the original line.
        /// </summary>
        /// <param name="xsect">The point at the intersection.</param>
        /// <param name="line">The line to split.</param>
        /// <param name="new1">The new line prior to the intersection (may be null).</param>
        /// <param name="new2">The new line after the intersection (may be null).</param>
        /// <returns>True if a split was done. False if no split (because the intersection
        /// coincides with the start or end of the original line). If no split done, the new
        /// lines are returned as null references.</returns>
        /// <remarks>This function is used by IntersectTwoLinesOperation and
        /// IntersectDirectionAndLineOperation) to split one of their input lines.</remarks>
        protected bool SplitLine(PointFeature xsect, LineFeature line,
                                    out LineFeature new1, out LineFeature new2)
        {
            // Initialize pointers to the new lines.
            new1 = new2 = null;

            // If the intersection corresponds EXACTLY to the start
            // or end of the line, there's nothing to do.
            IPointGeometry xLoc = xsect.PointGeometry;
            IPointGeometry sLoc = line.Start;
            IPointGeometry eLoc = line.End;

            if (xLoc.IsCoincident(sLoc) || xLoc.IsCoincident(eLoc))
                return false;

            /*
             * how PointOnLineOperation does it...

            // Create two line sections (one of them will be associated with the distance)
            m_NewLine1 = MakeSection(m_Line.StartPoint, m_NewPoint);
            m_NewLine2 = MakeSection(m_NewPoint, m_Line.EndPoint);

            // De-activate the parent line
            m_Line.IsInactive = true;
             */

            throw new NotImplementedException("IntersectOperation.SplitLine");
        }

        /*
	// Create the line primitives.
	CeMap* pMap = CeMap::GetpMap();
	const CeLine* const pLine = line.GetpLine();
	CeSection* pSect1 = pMap->AddLineSection(*pLine,sLoc,xLoc);
	CeSection* pSect2 = pMap->AddLineSection(*pLine,xLoc,eLoc);

	// And the arcs to go with them.
	pNew1 = line.MakeSubSection(*pSect1,*this,pSubTheme);
	pNew2 = line.MakeSubSection(*pSect2,*this,pSubTheme);

	// Make the original line inactive (and delete all system-defined
	// sections that are derived from it).
	line.SetInactive(this);

	return TRUE;

} // end of SplitLine
         */
    }
}
