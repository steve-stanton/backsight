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
//using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="10-FEB-1998" was="CeControl" />
    /// <summary>
    /// A control point.
    /// </summary>
    class ControlPoint : Position3D, IEquatable<ControlPoint>
    {
        #region Static

        /// <summary>
        /// Creates a <c>ControlPoint</c> based on the supplied string.
        /// </summary>
        /// <param name="s">String read from external control file.</param>
        /// <returns>The created point</returns>
        /// <remarks>Assumes Manitoba control file</remarks>
        internal ControlPoint CreateInstance(string s)
        {
            return null;
            /*
//	Define initial values.
	m_ControlId = 0;
	m_Zone = 0;
	m_Northing = 0.0;
	m_Easting = 0.0;
	m_Elevation = 0.0;

//	The string must contain at least 77 characters.
	UINT4 slen = StrLength(string);
	if ( slen<77 ) return;

	CHARS* eptr;		// Pointer to char that stops the scan.

//	Characters 7:13 (zero-based) must contain a numeric value.
	INT4 xid = strtol(&string[7],&eptr,10);
	if ( xid<=0 || (eptr<=&string[13] && !isspace(*eptr)) ) return;

//	Characters 35:38 contain the UTM zone number.
	INT4 zone = strtol(&string[35],&eptr,10);
	if ( zone<=0 || zone>60 || (eptr<=&string[38] && !isspace(*eptr)) ) return;

//	Characters 51:60 contain the easting.
	FLOAT8 easting = strtod(&string[51],&eptr);
	if ( easting<TINY || (eptr<=&string[60] && !isspace(*eptr)) ) return;

//	Characters 66:76 contain the northing.
	FLOAT8 northing = strtod(&string[66],&eptr);
	if ( northing<TINY || (eptr<=&string[76] && !isspace(*eptr)) ) return;

//	Everything looks ok, so define the object.

	m_ControlId = UINT4(xid);
	m_Zone = UINT1(zone);
	m_Northing = northing;
	m_Easting = easting;
	m_Elevation = 0.0;		// no elevation data
             */
        }

        #endregion

        #region Class data

        /// <summary>
        /// The ID of the control point.
        /// </summary>
        uint m_ControlId;

        /// <summary>
        /// The zone number.
        /// </summary>
        byte m_Zone;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ControlPoint</c>
        /// </summary>
        internal ControlPoint(uint id, double x, double y, double z, byte zone)
            : base(x,y,z)
        {
            m_ControlId = id;
            m_Zone = zone;
        }

        #endregion

        /// <summary>
        /// Is the position of this control point defined?
        /// </summary>
        internal bool IsDefined
        {
            get { return (X > Constants.TINY); }
        }

        /// <summary>
        /// Does this control point equal the supplied control point?
        /// </summary>
        /// <param name="that">The control point to compare with</param>
        /// <returns>True if this control point has the same ID as <paramref name="that"/></returns>
        public bool Equals(ControlPoint that) // IEquatable<ControlPoint>
        {
            return (this.m_ControlId == that.m_ControlId);
        }

        /// <summary>
        /// The ID of the control point (should be unique)
        /// </summary>
        internal uint ControlId
        {
            get { return m_ControlId; }
        }

        /// <summary>
        /// Checks if this control point falls inside a control range.
        /// </summary>
        /// <param name="range">The control range to compare with.</param>
        /// <returns>True if this control point is in the range.</returns>
        internal bool IsInRange(ControlRange range)
        {
            //return range.IsEnclosing(m_ControlId);
            throw new NotImplementedException();
        }

        /*
//	@mfunc	Save this control point in the map.
//
//	@parm	The entity type for the control.
//
//	@rdesc	The point representing the control.
//
//////////////////////////////////////////////////////////////////////

#include "CeIdHandle.h"

CePoint* CeControl::Save ( const CeEntity& ent ) const {

//	Return if the control point is undefined.
	if ( !this->IsDefined() ) return 0;

//	Get the position.
	CeVertex pos(m_Easting,m_Northing);

//	Add to the map.
	CeMap* pMap = CeMap::GetpMap();
	LOGICAL isold;
	CePoint* pPoint = pMap->AddPoint(pos,&ent,isold);

//	Define the feature's ID.
	if ( pPoint ) {
		CeIdHandle idh(pPoint);
		CHARS keystr[16];
		sprintf(keystr,"%d",m_ControlId);
		idh.CreateForeignId(keystr);
	}

	return pPoint;

} // end of Save
*/

        /// <summary>
        /// Draws this control point on the specified display.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            if (IsDefined)
                style.RenderTriangle(display, this);
        }

    }
}
