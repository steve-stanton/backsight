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

using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="11-DEC-1997" was="CeNewLabel"/>
    /// <summary>
    /// Edit to add an item of text (perhaps a polygon label).
    /// </summary>
    [Serializable]
    class NewTextOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The text label that was created
        /// </summary>
        TextFeature m_NewText;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewTextOperation</c> that doesn't refer to any new text.
        /// </summary>
        internal NewTextOperation()
        {
            m_NewText = null;
        }

        #endregion

        /// <summary>
        /// The text label that was created
        /// </summary>
        internal TextFeature Text // was GetpLabel
        {
            get { return m_NewText; }
            //set { m_Label = value; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Add new text"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>Null (always), since this operation doesn't create any lines.</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { return new Feature[] { m_NewText }; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.NewText; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            // Do nothing
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Mark all created features
            if (m_NewText!=null)
                m_NewText.Undo();

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do?

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>Null (always), since this operation doesn't create any lines.</returns>
        internal LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// Returns true, to indicate that this edit can be corrected.
        /// </summary>
        bool CanCorrect
        {
            get { return true; }
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>False (always), since this edit doesn't depend on anything</returns>
        bool HasReference(Feature feat)
        {
            return false;
        }

        /*
//	@mfunc	Execute the new label operation.
//
//	@parm	The position of the new label.
//	@parm	The height for the new label, in metres on the ground.
//			Specify 0.0 to get the entity type's default size.
//	@parm	The ID and entity type to assign to the new label.
//	@parm	The polygon that the label falls inside. It should
//			not already refer to a label. Specify 0 if the
//			label is non-topological.
//
//	@rdesc	TRUE if operation executed ok.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeNewLabel::Execute	( const CeVertex& vtx
							, const FLOAT8 ght
 							, const CeIdHandle& polygonId
							, CePolygon* pPol ) {

//	Confirm that any specified polygon is good.
	if ( pPol && pPol->IsIsland() ) {
		ShowMessage ( "CeNewLabel::Execute\nIsland polygon." );
		return FALSE;
	}

	CeMap* pMap = CeMap::GetpMap();

//	Add the label.
	m_pNewLabel = pMap->AddKeyLabel(polygonId,vtx,(FLOAT4)ght);
	if ( !m_pNewLabel ) return FALSE;

//	Associate the polygon with the label, and vice versa.
	if ( pPol ) {
		m_pNewLabel->SetTopology(TRUE);
		pPol->ClaimLabel(*m_pNewLabel);
	}
	else
		m_pNewLabel->SetTopology(FALSE);

	return TRUE;

} // end of Execute

//	@mfunc	Execute the new label operation.
//
//	@parm	The position of the new label.
//	@parm	The height for the new label, in metres on the ground.
//			Specify 0.0 to get the entity type's default size.
//	@parm	The ID and entity type to assign to the new label.
//	@parm	The transient row to use for creating a row for the new label.
//	@parm	The template to use in creating the CeRowText primitive for the
//          new label.
//	@parm	The polygon that the label falls inside. It should
//			not already refer to a label. Specify 0 if the
//			label is non-topological.
//
//	@rdesc	TRUE if operation executed ok.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeNewLabel::Execute	( const CeVertex& vtx
							, const FLOAT8 ght
 							, const CeIdHandle& polygonId
							, CeRow& row
							, const CeTemplate& atemplate
							, CePolygon* pPol ) {

//	Confirm that any specified polygon is good.
	if ( pPol && pPol->IsIsland() ) {
		ShowMessage ( "CeNewLabel::Execute\nIsland polygon." );
		return FALSE;
	}

//	Add the label.
	CeMap* pMap = CeMap::GetpMap();
	m_pNewLabel = pMap->AddRowLabel(polygonId,vtx,&row,&atemplate,(FLOAT4)ght);
	if ( !m_pNewLabel ) return FALSE;

//	Associate the polygon with the label, and vice versa.
	if ( pPol ) {
		m_pNewLabel->SetTopology(TRUE);
		pPol->ClaimLabel(*m_pNewLabel);
	}
	else
		m_pNewLabel->SetTopology(FALSE);

	// Cross-reference the row to this op.
	// 22-OCT-99: This was done so that if the row was subsequently
	// changed, we could navigate to the corresponding CeRowText
	// object to update the spatial index. However, if you rolled
	// back an extra label, CeLabel::SetDeleted did not remove
	// the CeRow object, which would lead to a mem violation
	// shortly thereafter. Although it would have been possible
	// to extend the rollback logic, it's better to handle things
	// by overriding CePrimitive::OnPreChange/OnPostChange in
	// CeRow, and getting it to funnel the request to the
	// row text via the CeFeatureId and CeLabel objects. That
	// way, it will also work in the eventuality that the row
	// is attached via some technique other than a CeNewLabel
	// operation.

	// row.AddOp(*this);

	return TRUE;

} // end of Execute
#endif
*/
        /// <summary>
        /// Executes this operation. This version is suitable for adding miscellaneous
        /// non-topological trim.
        /// </summary>
        /// <param name="trim">The text of the label.</param>
        /// <param name="position">The reference position for the label.</param>
        /// <param name="ght">The height of the new label, in meters on the ground. Specify 0 to
        /// use the default height for the specified entity type.</param>
        /// <param name="ent">The entity type to assign to the new label (default was null)</param>
        internal void Execute(string trim, IPosition position, double ght, IEntity ent)
        {
            // Add the label.
            m_NewText = CadastralMapModel.Current.AddMiscText(trim, ent, position, ght, 0.0, 0.0);

            // The trim is always non-topological.
            m_NewText.SetTopology(false);
        }

/*
#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Execute the new label operation. This version is suitable
//			for adding feature text that refers to polygon. This sort
//			of text is always topological.
//
//	@parm	The type of label.
//	@parm	The reference position for the label.
//	@parm	The height for the new label, in metres on the ground.
//			Specify 0.0 to get the entity type's default size.
//	@parm	The entity type to assign to the new label (default=0)
//	@parm	The polygon the label refers to (default=0)
//
//	@rdesc	TRUE if operation executed ok.
//
//	!new for FieldArea
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeNewLabel::Execute ( const FTX type
							, const CeVertex& position
							, const FLOAT8 ght
							, const CeEntity* const pEnt
							, CePolygon* pPol ) {

//	Confirm that any specified polygon is good.
	if ( pPol && pPol->IsIsland() ) {
		ShowMessage ( "CeNewLabel::Execute\nIsland polygon." );
		return FALSE;
	}

//	Add the label.
	CeMap* pMap = CeMap::GetpMap();
	m_pNewLabel = pMap->AddFeatureLabel(type,position,pEnt,pPol,(FLOAT4)ght);
	if ( !m_pNewLabel ) return FALSE;

	return TRUE;

} // end of Execute

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Execute the new label operation.
//
//	@parm	The position of the new label.
//	@parm	The height for the new label, in metres on the ground.
//			Specify 0.0 to get the entity type's default size.
//	@parm	The entity type for the new label.
//	@parm	The polygon that the label falls inside. It should
//			not already refer to a label. Specify 0 if the
//			label is non-topological.
//	@parm	An old label on a theme derived from the current
//			editing theme that will supply the new label's ID.
//
//	@rdesc	TRUE if operation executed ok.

LOGICAL CeNewLabel::Execute ( const CeVertex& vtx
							, const FLOAT8 ght
 							, const CeEntity& ent
							, CePolygon& pol
							, CeLabel& oldlabel ) {

	// Confirm that the specified polygon is good.
	if ( pol.IsIsland() ) {
		ShowMessage ( "CeNewLabel::Execute\nIsland polygon." );
		return FALSE;
	}

	// Confirm the old label has an ID.
	CeFeatureId* pFid = oldlabel.GetpId();
	if ( !pFid ) {
		ShowMessage("CeNewLabel::Execute\nID is not available." );
		return FALSE;
	}

	// Add the new label with a sub-theme that corresponds to
	// the current editing theme, down to (but excluding) the
	// base layer of the existing label.
	CeSubTheme* pSub = GetSubTheme(oldlabel);
	assert(pSub);

	// Get the map to add a new label with an appropriate
	// sub-theme (without any ID).
	CeMap* pMap = CeMap::GetpMap();
	m_pNewLabel = pMap->AddKeyLabel(ent,vtx,pSub,(FLOAT4)ght);
	if ( !m_pNewLabel ) return FALSE;

	// Relate the new label to the specified ID and vice versa.
	m_pNewLabel->SetpId(pFid,oldlabel.IsForeignId());
	pFid->AddReference(*m_pNewLabel);

	// Spatially index it.
	pMap->GetSpace().Add(*m_pNewLabel);

	// The label MUST be topological, so make sure it's
	// marked as such.
	m_pNewLabel->SetTopology(TRUE);

	// Relate the label to the specified polygon & vice versa.
	pol.ClaimLabel(*m_pNewLabel);

	return TRUE;

} // end of Execute

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Execute the new label operation.
//
//	@parm	The position of the new label.
//	@parm	The height for the new label, in metres on the ground.
//			Specify 0.0 to get the entity type's default size.
//	@parm	The entity type for the new label.
//	@parm	The transient row to use for creating a row for the new label.
//	@parm	The template to use in creating the CeRowText primitive for the
//          new label.
//	@parm	The polygon that the label falls inside.
//	@parm	An old label on a theme derived from the current
//			editing theme that will supply the new label's ID.
//
//	@rdesc	TRUE if label created ok.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeNewLabel::Execute ( const CeVertex& vtx
							, const FLOAT8 ght
 							, const CeEntity& ent
							, CeRow& row
							, const CeTemplate& atemplate
							, CePolygon& pol
							, CeLabel& oldlabel ) {

	// Confirm that the specified polygon is good.
	if ( pol.IsIsland() ) {
		ShowMessage ( "CeNewLabel::Execute\nIsland polygon." );
		return FALSE;
	}

	// Confirm the old label has an ID.
	CeFeatureId* pFid = oldlabel.GetpId();
	if ( !pFid ) {
		ShowMessage("CeNewLabel::Execute\nID is not available." );
		return FALSE;
	}

	// Add the new label with a sub-theme that corresponds to
	// the current editing theme, down to (but excluding) the
	// base layer of the existing label.
	CeSubTheme* pSub = GetSubTheme(oldlabel);
	assert(pSub);

	// Get the map to add a new label to the current editing
	// theme (without any ID).
	CeMap* pMap = CeMap::GetpMap();
	m_pNewLabel = pMap->AddRowLabel(ent,vtx,&row,&atemplate,pSub,(FLOAT4)ght);
	if ( !m_pNewLabel ) return FALSE;

	// Relate the new label to the specified ID and vice versa.
	m_pNewLabel->SetpId(pFid,oldlabel.IsForeignId());
	pFid->AddReference(*m_pNewLabel);

	// Relate the row to the ID and vice versa.
	row.SetId(*pFid);

	// The label MUST be topological, so make sure it's
	// marked as such.
	m_pNewLabel->SetTopology(TRUE);

	// Relate the label to the specified polygon & vice versa.
	pol.ClaimLabel(*m_pNewLabel);

	return TRUE;

} // end of Execute
         */
    }
}
