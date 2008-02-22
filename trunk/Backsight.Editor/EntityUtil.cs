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

using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="05-FEB-1999" was="CeEntityUtil"/>
    /// <summary>
    /// Utility functions relating to entity types. This sort of object
    /// act as a conduit to methods provided by the <see cref="EntityFile"/> and
    /// <see cref="StyleFile"/> classes.
    /// </summary>
    class EntityUtil
    {
        #region Static

        /// <summary>
        /// Information about symbology
        /// </summary>
        static StyleFile s_StyleFile;

        /// <summary>
        /// Information about derived entity types
        /// </summary>
        static EntityFile s_EntityFile;

        #endregion

        #region Class data

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>EntityUtil</c>
        /// </summary>
        internal EntityUtil()
        {
        }

        #endregion

        /*
public:
						CeEntityUtil		( void );
	virtual				~CeEntityUtil		( void );
	virtual	LOGICAL		Open				( void );
	virtual	void		Close				( void );

	static LPCTSTR			GetDerivedType		( const CeArc& line
												, const CeTheme& theme );
	static LPCTSTR			GetUnderivedType	( const CeEntity* const pEnt );
	static const COLORREF	GetColour			( const CeEntity* const pEnt );
	static LOGICAL			IsStyleCustomized	( void );
	static const CeStyle*	GetStyle			( const CeEntity* const pEnt );
	static const CeStyle*	GetStyle			( const CeArc& line
												, const CeTheme& theme );
	static const CeStyle*	GetStyle			( const CeFeature& f );

private:

	virtual CeEntityFile*	OpenEntityFile	( void );
	virtual CeStyleFile*	OpenStyleFile	( void );

	static LOGICAL			GetStyleFile	( CString& spec );
         */
    }
}
