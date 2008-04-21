/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Data;

using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="20-OCT-1999" was="CeNewLabelEx"/>
    /// <summary>
    /// A new polygon label that is created on a derived layer so as to
    /// replace an existing label that was originally shared with the base layers.
    /// </summary>
    [Serializable]
    class ReplaceTextOperation : NewTextOperation
    {
        #region Class data

        /// <summary>
        /// The text label that was superseded as a consequence of adding
        /// an extra label to a derived layer.
        /// </summary>
        readonly TextFeature m_OldText;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ReplaceTextOperation</c> that refers to the text that's
        /// being replaced, but which doesn't yet refer to new text.
        /// </summary>
        internal ReplaceTextOperation(TextFeature oldText)
            : base()
        {
            m_OldText = oldText;
        }

        #endregion

        /// <summary>
        /// Executes the new label operation. This replaces an old label on a base
        /// theme that is being superseded by the new label (the old label should
        /// not be key text, since what gets created here will also be key text).
        /// </summary>
        /// <param name="vtx">The position of the new label.</param>
        /// <param name="ght">The height for the new label, in meters on the ground.</param>
        /// <param name="ent">The entity type for the new label.</param>
        /// <param name="pol">The polygon that the label falls inside.</param>
        internal void Execute(IPosition vtx, double ght, IEntity ent, Polygon pol)
        {
            // Confirm the old label has an ID.
            FeatureId fid = m_OldText.Id;
            if (fid==null)
                throw new Exception("ReplaceTextOperation.Execute - ID is not available.");

            // De-activate the old label.
            SetOldLabel();

            // Add the new label on the current editing layer.
            CreateLabel(vtx, ght, ent, fid, m_OldText.IsForeignId, pol);
        }

        /// <summary>
        /// Creates a key-text label.
        /// </summary>
        /// <param name="vtx">The reference position of the label.</param>
        /// <param name="ght">The height for the new label, in meters on the ground.</param>
        /// <param name="ent">The entity type for the new label.</param>
        /// <param name="id">The ID for the new label.</param>
        /// <param name="isForeign">Is the ID foreign?</param>
        /// <param name="pol">The polygon that the label relates to.</param>
        void CreateLabel(IPosition vtx, double ght, IEntity ent, FeatureId id, bool isForeign, Polygon pol)
        {
        }

        /*
LOGICAL CeNewLabelEx::CreateLabel ( const CeVertex& vtx
								  , const FLOAT8 ght
								  , const CeEntity& ent
								  , CeFeatureId& id
								  , const LOGICAL isForeign
								  , CePolygon& pol ) {

	// Get the map to add a new label to the current editing
	// theme (without any ID).
	CeMap* pMap = CeMap::GetpMap();
	CeLabel* pLabel = pMap->AddKeyLabel(ent,vtx,0,ght);
	if ( !pLabel ) return FALSE;

	// Relate the new label to the specified ID and vice versa.
	pLabel->SetpId(&id,isForeign);
	id.AddReference(*pLabel);

	// Spatially index it.
	pMap->GetSpace().Add(*pLabel);

	// The label MUST be topological, so make sure it's
	// marked as such.
	pLabel->SetTopology(TRUE);

	// Relate the label to the specified polygon & vice versa.
	pol.ClaimLabel(*pLabel);

	// Get the base class to hold the address of the new label.
	SetpLabel(pLabel);

	pMap->CleanEdit();
	return TRUE;

} // end of CreateLabel
        */

        /// <summary>
        /// Executes the new label operation.
        /// </summary>
        /// <param name="vtx">The position of the new label.</param>
        /// <param name="ght">The height for the new label, in meters on the ground.</param>
        /// <param name="ent">The entity type for the new label.</param>
        /// <param name="row">The transient row to use for creating a row for the new label.</param>
        /// <param name="atemplate">The template to use in creating the RowText for the new label.</param>
        /// <param name="pol">The polygon that the label falls inside.</param>
        internal void Execute(IPosition vtx, double ght, IEntity ent, DataRow row,
                                ITemplate atemplate, Polygon pol)
        {
            // Confirm the old label has an ID.
            FeatureId fid = m_OldText.Id;
            if (fid == null)
                throw new Exception("ReplaceTextOperation.Execute - ID is not available.");

            // De-activate the old label.
            SetOldLabel();

            // Add the new label on the current editing layer.
            CreateLabel(vtx, ght, ent, row, atemplate, fid, m_OldText.IsForeignId, pol);
        }

        /// <summary>
        /// Creates a row-text label.
        /// </summary>
        /// <param name="vtx">The reference position of the label.</param>
        /// <param name="ght">The height for the new label, in meters on the ground.</param>
        /// <param name="ent">The entity type for the new label.</param>
        /// <param name="row">The transient row to use for creating a row for the new label.</param>
        /// <param name="atemplate">The template to use in creating the RowText for the new label.</param>
        /// <param name="id">The ID for the new label.</param>
        /// <param name="isForeign">Is the ID foreign?</param>
        /// <param name="pol">The polygon that the label relates to.</param>
        void CreateLabel(IPosition vtx, double ght, IEntity ent, DataRow row, ITemplate atemplate,
                            FeatureId id, bool isForeign, Polygon pol)
        {
        }
        /*
	// Get the map to add a new label to the current editing
	// theme (without any ID).
	CeMap* pMap = CeMap::GetpMap();
	CeLabel* pLabel = pMap->AddRowLabel(ent,vtx,&row,&atemplate,0
										,(FLOAT4)ght);
	if ( !pLabel ) return FALSE;

	// Relate the new label to the specified ID and vice versa.
	pLabel->SetpId(&id,isForeign);
	id.AddReference(*pLabel);

	// Relate the row to the ID and vice versa.
	row.SetId(id);

	// The label MUST be topological, so make sure it's
	// marked as such.
	pLabel->SetTopology(TRUE);

	// Relate the label to the specified polygon & vice versa.
	pol.ClaimLabel(*pLabel);

	// Get the base class to hold the address of the new label.
	SetpLabel(pLabel);

	pMap->CleanEdit();
	return TRUE;

} // end of CreateLabel
*/

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Restore the original label.
            m_OldText.Restore();

            return true;
        }

        /// <summary>
        /// De-activates the label that is being replaced.
        /// </summary>
        void SetOldLabel()
        {
            // De-activate the old label (given that the active
            // editing layer is something like the Assessment layer,
            // but the label is shared with the Plan and Ownership
            // layers, this will also create a label that belongs
            // exclusively to Plan+Ownership).

            m_OldText.IsInactive = true;
        }
    }
}
