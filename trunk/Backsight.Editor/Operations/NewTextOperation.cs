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
using System.Data;

using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.Xml;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="11-DEC-1997" was="CeNewLabel"/>
    /// <summary>
    /// Edit to add an item of text (perhaps a polygon label).
    /// </summary>
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
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal NewTextOperation(Session s, NewTextType t)
            : base(s, t)
        {
            m_NewText = (TextFeature)t.Text.LoadFeature(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewTextOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal NewTextOperation(Session s)
            : base(s)
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
        }

        /// <summary>
        /// Defines the text created by this edit
        /// </summary>
        /// <param name="label">The created text (not null)</param>
        protected void SetText(TextFeature label)
        {
            m_NewText = label;
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
        internal override LineFeature GetPredecessor(LineFeature line)
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

        /// <summary>
        /// Executes the new label operation.
        /// </summary>
        /// <param name="vtx">The position of the new label.</param>
        /// <param name="polygonId">The ID and entity type to assign to the new label.</param>
        /// <param name="pol">The polygon that the label falls inside. It should not already refer to a label.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the text, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
        internal void Execute(IPosition vtx, IdHandle polygonId, Polygon pol, 
                                double height, double width, double rotation)
        {
            // Add the label.
            CadastralMapModel map = MapModel;
            m_NewText = map.AddKeyLabel(this, polygonId, vtx, height, width, rotation);

            // Associate the polygon with the label, and vice versa.
            m_NewText.SetTopology(true);
            pol.ClaimLabel(m_NewText);

            Complete();
        }

        /// <summary>
        /// Executes the new label operation.
        /// </summary>
        /// <param name="vtx">The position of the new label.</param>
        /// <param name="polygonId">The ID and entity type to assign to the new label.</param>
        /// <param name="row">The data to use for creating a row for the new label.</param>
        /// <param name="atemplate">The template to use in creating the RowTextGeometry
        /// for the new label.</param>
        /// <param name="pol">The polygon that the label falls inside. It should not already
        /// refer to a label. Not null.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the new label, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
        internal void Execute(IPosition vtx, IdHandle polygonId, DataRow row, ITemplate atemplate, Polygon pol,
                                double height, double width, double rotation)
        {
            if (pol == null)
                throw new ArgumentNullException();

            // Add the label.
            CadastralMapModel map = MapModel;
            m_NewText = map.AddRowLabel(this, polygonId, vtx, row, atemplate, height, width, rotation);

            // Associate the polygon with the label, and vice versa.
            m_NewText.SetTopology(true);
            pol.ClaimLabel(m_NewText);

            Complete();
        }

        /// <summary>
        /// Executes this operation. This version is suitable for adding miscellaneous
        /// non-topological trim.
        /// </summary>
        /// <param name="trim">The text of the label.</param>
        /// <param name="ent">The entity type to assign to the new label (default was null)</param>
        /// <param name="position">The reference position for the label.</param>
        /// <param name="ght">The height of the new label, in meters on the ground.</param>
        /// <param name="gwd">The width of the new label, in meters on the ground.</param>
        /// <param name="rot">The clockwise rotation of the text, in radians from the horizontal.</param>
        internal void Execute(string trim, IEntity ent, IPosition position, double ght, double gwd, double rot)
        {
            // Add the label.
            CadastralMapModel cmm = MapModel;
            m_NewText = cmm.AddMiscText(this, trim, ent, position, ght, gwd, rot);

            // The trim is always non-topological.
            m_NewText.SetTopology(false);

            Complete();
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

        /// <summary>
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// <returns>The serializable version of this edit</returns>
        internal override OperationType GetSerializableEdit()
        {
            NewTextType t = new NewTextType();
            SetSerializableEdit(t);
            return t;
        }

        /// <summary>
        /// Defines the XML attributes and elements that are common to a serialized version
        /// of a derived instance.
        /// </summary>
        /// <param name="t">The serializable version of this edit</param>
        protected void SetSerializableEdit(NewTextType t)
        {
            t.Id = this.DataId;
            t.Text = m_NewText.GetSerializableText();
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            // Nothing to do
        }
    }
}
