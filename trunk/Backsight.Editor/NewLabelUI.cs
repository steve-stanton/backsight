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
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

using Backsight.Environment;
using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Editor.Properties;
using Backsight.Geometry;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="03-JAN-1999" was="CuiNewLabel"/>
    /// <summary>
    /// User interface for adding new polygon labels
    /// </summary>
    class NewLabelUI : AddLabelUI
    {
        #region Class data

        /// <summary>
        /// The attribute schema for the polygons.
        /// </summary>
        ITable m_Schema;

        /// <summary>
        /// Annotation template (null means use IDs)
        /// </summary>
        ITemplate m_Template;

        /// <summary>
        /// The last row that was added (if any).
        /// </summary>
        DataRow m_LastRow;

        /// <summary>
        /// The polygon enclosing the last mouse position.
        /// </summary>
        Polygon m_Polygon;

        /// <summary>
        /// The ID (and entity type) of the label that is currently being positioned.
        /// </summary>
        IdHandle m_PolygonId;

        /// <summary>
        /// True if labels should be positioned automatically.
        /// </summary>
        bool m_IsAutoPos;

        /// <summary>
        /// The calculated position (defined only if m_IsAutoPos is true and m_Polygon is defined)
        /// </summary>
        IPosition m_AutoPosition;

        /// <summary>
        /// True if orientation should be automatically determined
        /// </summary>
        bool m_IsAutoAngle;

        /// <summary>
        /// The current orientation line
        /// </summary>
        LineFeature m_Orient;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewLabelUI</c>
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        internal NewLabelUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            m_Schema = null;
            m_Template = null;
            m_LastRow = null;
            m_Polygon = null;
            m_PolygonId = new IdHandle();
            m_IsAutoPos = Settings.Default.AutoPosition;
            m_IsAutoAngle = Settings.Default.AutoAngle;
            m_Orient = null;
            m_AutoPosition = null;
        }

        #endregion

        internal override bool IsAutoPosition
        {
            get { return m_IsAutoPos; }
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            // Disallow if labels are not currently drawn.
            if (!IsTextDrawn)
            {
                string msg = String.Empty;
                msg += ("Labels are currently invisible. To add new labels," + System.Environment.NewLine);
                msg += ("you must initially make them visible (see Edit-Preferences).");
                MessageBox.Show(msg);
                DialFinish(null);
                return false;
            }

            // Tell the view to un-highlight (we'll be doing our own).
            Controller.ClearSelection();

            // Display dialog to get info that will be common to all
            // the labels we will be adding.
            NewLabelForm dial = new NewLabelForm();
            if (dial.ShowDialog() != DialogResult.OK)
            {
                DialFinish(null);
                return false;
            }

            // Pick up the entered info.
            m_Schema = dial.Schema;
            m_Template = dial.Template;

            // Confirm that the entity type has been specified.
            IEntity ent = dial.Entity;
            if (ent == null)
            {
                MessageBox.Show("Polygon type must be specified.");
                DialFinish(null);
                return false;
            }

            // Tell the base class about the entity type.
            base.Entity = ent;

            // Get info for the first label.
            return GetLabelInfo();
        }

        /// <summary>
        /// Get the information relating to a single new label. This will be
        /// called after the attribute schema, annotation template and polygon
        /// entity type has been defined. It gets re-called after each label has
        /// been positioned.
        /// </summary>
        /// <returns>True if info supplied. False if command is done.</returns>
        bool GetLabelInfo()
        {
            // Ensure the normal cursor is displayed
            SetNormalCursor();

            // Ask the base class to return the entity type for the labels.
	        IEntity ent = base.Entity;

            // Get the polygon ID ...

            // If we are auto-numbering, just get the next ID. Otherwise
            // ask for it, showing the default.

            if (EditingController.Current.IsAutoNumber)
                m_PolygonId.ReserveId(ent, 0);
            else
            {
                GetIdForm dial = new GetIdForm(ent, m_PolygonId);
                if (dial.ShowDialog() != DialogResult.OK)
                {
                    DialFinish(null);
                    return false;
                }
            }

            // Start by assuming the text is the key.
            string str = m_PolygonId.FormattedKey;

            // Get any attributes.
            if (m_Schema != null)
            {
                if (!GetAttributes() || m_LastRow == null)
                {
                    DialFinish(null);
                    return false;
                }

                // If an annotation template has been specified, use that
                // to get the text from the row (pass in the ID in case the
                // template includes a reference to the key).
                throw new NotImplementedException("NewLabelUI.GetLabelInfo");
                /*
		            if ( m_pTemplate ) {
			            CString text;
			            m_pTemplate->GetText(*m_pLastRow,(LPCTSTR)str,text);
			            str = text;
		            }
                 */
            }

            // Tell the base class.
            if (!SetDimensions(str))
            {
                DialFinish(null);
                return false;
            }

            // Switch on the command cursor (we won't add anything until
            // the user left clicks inside a polygon).
            SetCommandCursor();
            return true;
        }

        /// <summary>
        /// Handles mouse-move.
        /// </summary>
        /// <param name="pos">The new position of the mouse</param>
        internal override void MouseMove(IPosition pos)
        {
            // If we previously drew a text outline, erase it now.
            EraseRect();

            // Find the polygon (if any) that encloses the mouse position.
            CadastralMapModel map = CadastralMapModel.Current;
            ISpatialIndex index = map.Index;
            IPointGeometry pg = PointGeometry.Create(pos);
            Polygon enc = new FindPointContainerQuery(index, pg).Result;

            // If it's different from what we previously had, remember the
            // new enclosing polygon.
            if (!Object.ReferenceEquals(enc, m_Polygon))
            {
                // If we had something before, and we filled it, erase
                // the fill now.
                //DrawPolygon(false);

                // Remember the polygon we're now enclosed by (if any).
                m_Polygon = enc;

                // Draw the new polygon.
                //DrawPolygon(true);

                // Ensure any calculated position has been cleared
                m_AutoPosition = null;

                // See if a new orientation applies
                CheckOrientation(pg);

                // If the enclosing polygon does not have a label, use the
                // standard cursor and fill the polygon. Otherwise use the
                // gray cursor.
                SetCommandCursor();
            }

            // Draw a rectangle representing the outline of the text.
            if (m_IsAutoPos && m_Polygon!=null)
            {
                if (m_AutoPosition==null)
                    m_AutoPosition = m_Polygon.GetLabelPosition(Width, Height);
            }

            if (m_IsAutoPos && m_AutoPosition!=null)
                DrawRect(m_AutoPosition);
            else
                DrawRect(pos);
        }

        /// <summary>
        /// Reacts to action that concludes the command dialog.
        /// </summary>
        /// <param name="wnd">The dialog window where the action originated (not used)</param>
        /// <returns>True if command finished ok. This implementation returns the
        /// result of a call to <see cref="FinishCommand"/>.</returns>
        internal override bool DialFinish(Control wnd)
        {
            // Ensure any text outline has been erased.
            EraseRect();

            // Clear any polygon fill
            DrawPolygon(false);

            // And un-highlight any orientation line
            if (m_Orient!=null)
            {
                ErasePainting();
                m_Orient = null;
            }

            // If the last row we created was not associated with a label, get rid of it now.
            //if (m_pLastRow && !m_pLastRow->GetpId())
            //{
            //    delete m_pLastRow;
            //    m_pLastRow = 0;
            //}

            // Get the base class to finish up.
            return base.DialFinish(wnd);
        }

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="p">The position where the click occurred</param>
        /// <returns>True (always), indicating that something was done.</returns>
        internal override bool LButtonDown(IPosition p)
        {
            // Make sure the enclosing polygon refers to the point we
            // have been supplied.
            MouseMove(p);

            // Exit the command if the user left-clicked in an area that
            // does not have a polygon (perhaps the user doesn't know that
            // you exit via the right-click menu).
            if (m_Polygon==null)
            {
                DialFinish(null);
                return true;
            }

            // Issue error if the enclosing polygon already has a label (this
            // was supposed to be pretty obvious, given that the cursor is
            // grey, and the polygon is not filled in that case).
            if (!IsValidPolygon())
            {
                MessageBox.Show("Polygon already has a label");
                SetCommandCursor();
                return true;
            }

            // Erase any text outline.
            EraseRect();

            // Erase any highlighting.
            DrawPolygon(false);

            if (m_Orient!=null)
            {
                ErasePainting();
                m_Orient = null;
            }

            // Does the polygon actually contain a label that's shared with a base layer?
            TextFeature oldLabel = m_Polygon.Label;

            // Add a new label.
            TextFeature label = null;

            if (m_IsAutoPos)
                label = AddNewLabel(oldLabel);
            else
                label = AddNewLabel(p, oldLabel);

            // Tell the base class (resets the last known position, used
            // to erase labels during dragging). 
            OnLabelAdd();

            // Undefine the enclosing polygon pointer so that the cursor
            // will become gray as the user moves the mouse out of the polygon.
            m_Polygon = null;

            // Draw it. Note that if the new label replaces an old label,
            // the AddNewLabel call explicitly erases it, but something
            // from within a subsequent CleanEdit call re-draws it in
            // red. To get around that, ensure it's erased again here.
            // ...changes made elsewhere may now make this redundant
            //if (pLabel)
            //{
            //    if (pOldLabel)
            //        pOldLabel->Erase();
            //    GetpWnd()->Draw(pLabel, COL_BLACK);
            //}

            // Get the info for the next label.
            return GetLabelInfo();
        }

        /// <summary>
        /// Creates any applicable context menu
        /// </summary>
        /// <returns>The context menu for this command.</returns>
        internal override ContextMenuStrip CreateContextMenu()
        {
            return new NewLabelContextMenu(this);
        }

        /// <summary>
        /// Handles the context menu "Auto-Position" menuitem.
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void ToggleAutoPosition(IUserAction action)
        {
		    // Ensure any text outline has been erased.
		    EraseRect();

		    ResetLastPos();
		    //OnLabelAdd();	// Bit of a hack

		    // Toggle the auto-positioning option.
		    m_IsAutoPos = !m_IsAutoPos;
            Settings.Default.AutoPosition = m_IsAutoPos;
            Settings.Default.Save();

            // If we've just turned the option on, we'll calculate position on mouse move
            m_AutoPosition = null;

		    // Set the appropriate command cursor.
		    SetCommandCursor();
        }

        /// <summary>
        /// Handles the context menu "Auto-Angle" menuitem.
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void ToggleAutoAngle(IUserAction action)
        {
            m_IsAutoAngle= !m_IsAutoAngle;
            Settings.Default.AutoAngle = m_IsAutoAngle;
            Settings.Default.Save();

            if (!m_IsAutoAngle && m_Orient != null)
            {
                ErasePainting();
                m_Orient = null;
            }
        }

        /// <summary>
        /// Is the "Auto-Angle" menuitem checked in this UI's context menu?
        /// </summary>
        internal bool IsAutoAngle
        {
            get { return m_IsAutoAngle; }
        }

        /// <summary>
        /// Handles the context menu "Finish" menuitem.
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void Finish(IUserAction action)
        {
            DialFinish(null);
        }

        /// <summary>
        /// Ensures the command cursor is shown (the reverse arrow cursor).
        /// </summary>
        internal override void SetCommandCursor()
        {
            ActiveDisplay.MapPanel.Cursor = GetCommandCursor();
        }

        Cursor GetCommandCursor()
        {
            if (IsValidPolygon())
            {
                // If the polygon actually contains a label that is
                // drawn on the current editing theme, take this
                // opportunity to draw it in gray.
                TextFeature label = m_Polygon.Label;
                if (label!=null)
                    label.Draw(ActiveDisplay, Color.Gray);

                if (m_IsAutoPos)
                    return EditorResources.WandCursor;
                else
                    return EditorResources.ReverseArrowCursor;
            }
            else
            {
                if (m_IsAutoPos)
                    return EditorResources.GrayWandCursor;
                else
                    return EditorResources.GrayReverseArrowCursor;
            }
        }

        /// <summary>
        /// Draws (or erases) the polygon that the mouse currently sits in.
        /// </summary>
        /// <param name="draw">Draw the polygon? [Default was true]</param>
        /// <returns>True if a polygon was drawn or erased.</returns>
        bool DrawPolygon(bool draw)
        {
            // Return if there is no valid polygon.
            if (!IsValidPolygon())
                return false;

            if (draw)
            {
                m_Polygon.Render(ActiveDisplay, new DrawStyle());
                if (m_Orient != null)
                    m_Orient.Render(ActiveDisplay, new HighlightStyle());
            }
            else
                ErasePainting();

            return true;
        }

        /// <summary>
        /// Do we currently have a valid polygon for adding a new label?
        /// </summary>
        /// <returns>True if <c>m_Polygon</c> is defined and valid.</returns>
        bool IsValidPolygon()
        {
            // Return if there is no polygon to draw.
            if (m_Polygon==null)
                return false;

            // Try to find an existing label inside the polygon.
            TextFeature label = m_Polygon.Label;

            // If we got something, the normal course of action is
            // to disallow addition of another label. However, if the
            // current editing layer is derived from the base layer
            // of the label, we'll allow it ... UNLESS the existing
            // label is key text and the replacement would be key
            // text as well.

            if (label == null)
                return true;

            if (m_Template == null && label.TextGeometry is KeyTextGeometry)
                return false;

            // Allow things if the user is working with a derived theme (the label
            // that's displayed is currently shared with a base layer, but will
            // be superseded with the label we're about to add).
            ILayer baseLayer = label.BaseLayer;
            return (baseLayer!=null && ActiveLayer.ThemeSequence > baseLayer.ThemeSequence);
        }

        /// <summary>
        /// Creates a new polygon label.
        /// </summary>
        /// <param name="oldLabel">An old shared label that is being replaced.</param>
        /// <returns>The text feature that was added.</returns>
        TextFeature AddNewLabel(TextFeature oldLabel)
        {
            Debug.Assert(m_Polygon!=null);

            // Get the dimensions of the text.
            double width = Width;
            double height = Height;

            // Get a position for the annotation.
            IPosition pos = m_Polygon.GetLabelPosition(width, height);
            if (pos==null)
                return null;

            // Add the label (assumes key label for now).
            return AddNewLabel(pos, oldLabel);
        }

        /// <summary>
        /// Creates a new polygon label in the map.
        /// </summary>
        /// <param name="posn">Reference position for the label.</param>
        /// <param name="oldLabel">An old shared label that is being replaced.</param>
        /// <returns>The text feature that was added.</returns>
        internal override TextFeature AddNewLabel(IPosition posn, TextFeature oldLabel)
        {
            Debug.Assert(m_Polygon!=null);
            CadastralMapModel map = CadastralMapModel.Current;
            TextFeature newLabel = null;

            // If we are replacing another label, use that label's ID
            // and ensure that any ID we reserved has been released.
            if (oldLabel!=null)
            {
                // Get the entity type that the ID handle was assigned
                // before freeing the handle.
                IEntity ent = m_PolygonId.Entity;
                m_PolygonId.FreeId();
                Debug.Assert(ent!=null);

                // Execute the edit
                ReplaceTextOperation op = null;

                try
                {
                    op = new ReplaceTextOperation(oldLabel);

                    if (m_Template != null && m_LastRow != null)
                    {
                        op.Execute(posn, ent, m_LastRow, m_Template, m_Polygon, Height, Width, Rotation);

                        // Confirm that the row got cross-referenced to an ID (not
                        // sure what the above ends up doing).
                        //if (m_LastRow.GetpId()==null)
                        //{
                        //    MessageBox.Show("Attributes were not attached to an ID");
                        //    return null;
                        //}

                        // Ensure the row is cross-referenced to the op.
                        //m_LastRow->AddOp(op);
                    }
                    else
                    {
                        op.Execute(posn, ent, m_Polygon, Height, Width, Rotation);
                    }

                    // Pick up the new label.
                    newLabel = op.Text;
                }

                catch (Exception ex)
                {
                    Session.WorkingSession.Remove(op);
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                // Not replacing an existing label, so confirm that
                // the polygon ID has been reserved.
                if (!m_PolygonId.IsReserved)
                {
                    MessageBox.Show("NewLabelUI.AddNewLabel - No polygon ID");
                    return null;
                }

                /*
                // Check whether another label exists in the specified
                // polygon, but on a layer that is derived from the current
                // editing layer. If so, note the entity type and free the
                // supplied ID.
 
                // SS20080421 - This bit of code was used to check whether the polygon
                // was already labelled on a derived layer, in which case the ID would
                // be promoted to the current layer. However, it's difficult to continue
                // with that logic, since the polygon object is no longer shared with
                // any other layer (and won't even exist unless you switch to another
                // editing layer). Even if the structure was unchanged, I'm not convinced
                // that it's good to define the ID on the basis of something that the user
                // can't actually see (if this were to be done, the user should have been
                // told already, since an explicitly specified ID would be disregarded here).

		        CeLabel* pOldLabel = m_pPolygon->GetBaseLabel();
		        const CeEntity* pEnt=0;

		        if ( pOldLabel ) {
			        pEnt = m_PolygonId.GetpEntity();
			        m_PolygonId.FreeId();
		        }
                 */

                // Execute the edit
                NewTextOperation op = null;

                try
                {
                    op = new NewTextOperation();

                    if (m_Template!=null && m_LastRow!=null)
                    {
                        op.Execute(posn, Height, m_PolygonId, m_LastRow, m_Template, m_Polygon);

                        // Confirm that the row got cross-referenced to an ID (not
			            // sure what the above ends up doing).
                        //if (m_LastRow.GetpId()==null)
                        //{
                        //    MessageBox.Show("Attributes were not attached to an ID");
                        //    return null;
                        //}
                    }
                    else
                    {
                        op.Execute(posn, m_PolygonId, m_Polygon, Height, Width, Rotation);
                    }

                    newLabel = op.Text;
                }

                catch (Exception ex)
                {
                    Session.WorkingSession.Remove(op);
                    MessageBox.Show(ex.Message);
                }
            }

            // If a new label has actually been added
            if (newLabel!=null)
            {
                /*
		// If a row was created, but we only created key text,
		// make sure the label's ID has been cross-referenced
		// to the row (the call to CeRow::SetId makes pointers
		// both ways).

		// @devnote This was introduced 20-OCT-99. While this
		// was previously done for CeRowText labels, it was NOT
		// done for CeKeyText labels. Hence, if you added a new
		// label to the Assessment layer (which currently has
		// no row text templates), you'd get key text, but the
		// attributes you entered would have been stored, but
		// would not be referenced by the ID. So, if you went
		// to update the polygon's attributes, it said there
		// were no attributes.

		// Note that key text is NOT referenced to the creating
		// op. This is inconsistent with the way it has always
		// worked for row text ... row-text labels don't REALLY
		// need to be cross-referenced to the op, but it makes
		// sense if you think of the row as being "used" by the
		// row text.

		if ( m_pLastRow ) {
			CeFeatureId* pFid = pNewLabel->GetpId();
			assert(pFid);
			if ( pFid ) m_pLastRow->SetId(*pFid);
		}
                 */
            }

            return newLabel;
        }

        /// <summary>
        /// Gets the attributes for a new label.
        /// </summary>
        /// <returns>True if attributes entered OK.</returns>
        bool GetAttributes()
        {
            // There HAS to be a schema.
            if (m_Schema==null)
                return false;


            MessageBox.Show("Attribute data entry is not currently implemented");
            return false;
        }
        /*
	// Allocate a blob of memory to hold the attribute data.
	CHARS* pAttrData = new CHARS[m_pSchema->GetSchemaMemSize()];

	// If we previously created a row, make a copy of it's data.
	// Otherwise define default attributes.

	if ( m_pLastRow ) {

		m_pLastRow->SaveDataCopy(pAttrData);

		// If it didn't get associated with a label (somehow), get
		// rid of it now.
		if ( !m_pLastRow->GetpId() ) {
			delete m_pLastRow;
			m_pLastRow = 0;
		}
	}
	else
		m_pSchema->MakeDefaultBlob(pAttrData);

	// Create a new row (without an ID).
	m_pLastRow = new ( os_database::of(m_pSchema)
				     , os_ts<CeRow>::get() )
					   CeRow(*m_pSchema,pAttrData);

	// Don't need our copy of the attribute data any more.
	delete [] pAttrData;
	pAttrData = 0;

	// Construct 'container dialog' to hold controls
	CdRowDialog ParentDialog(TRUE,FALSE);

	// Next construct empty dynamic dialog to have controls added to
	CeDynamicDialog DynamicDialog(&ParentDialog,0,14,4000,10,5);
		
	// Set the title for the dialog window
	DynamicDialog.SetWindowTitle("Specify Attributes for Next Label");

	// Set the dynamic dialog in the container dialog so that
	// OnSize messages get handled.
	ParentDialog.SetDynamicDialog(&DynamicDialog);

	// Construct the object for displaying a row of attributes.
	CeRowDisplay DisplayObject(&DynamicDialog,TRUE,m_pLastRow);

	// Set the object used to construct the controls
	ParentDialog.SetDisplayObject(&DisplayObject);

	// Present modal dialog.
	if ( ParentDialog.DoModal()!=IDOK ) return FALSE;
	
	// if the user requested setting of attributes and some were changed, then they
	// are set in the CeRow object

	if ( DynamicDialog.IsChanged() ) {
		m_pLastRow->OnPreChange();
		LOGICAL retval = DisplayObject.UpdateRow(TRUE);
		m_pLastRow->OnPostChange();
		return retval;
	}

	return TRUE;

} // end of GetAttributes
        */

        /// <summary>
        /// Updates a previously added polygon label.
        /// </summary>
        /// <param name="label">The previously added label.</param>
        /// <returns>True if an update was made.</returns>
        internal override bool UpdateLabel(TextFeature label)
        {
            // For the time being, "update" means updating the attributes
            // associated with the label. There's no way to update the
            // entity type, schema, or annotation template (apart from
            // deleting an existing label, and re-adding a new one).

            return false;
        }

        /// <summary>
        /// Sees whether the orientation of the new text should be altered. This
        /// occurs if the auto-angle capability is enabled, and the specified
        /// position is close to any visible line.
        /// </summary>
        /// <param name="refpos">The current mouse position (reference position
        /// for the new label)</param>
        /// <returns>True if the orientation angle got changed.</returns>
        bool CheckOrientation(IPointGeometry refpos)
        {
            // Return if the auto-angle function is disabled.
            if (!m_IsAutoAngle)
                return false;

            // Return if the auto-position function is enabled
            if (m_IsAutoPos)
                return false;

            // Try to get an orientation line
            LineFeature orient = GetOrientation(refpos);
            if (!Object.ReferenceEquals(orient, m_Orient))
                ErasePainting();

            m_Orient = null;
            if (orient == null)
                return false;

            // Locate the closest point on the line (we SHOULD find it,
            // but if we don't, just bail out)
            ISpatialDisplay display = ActiveDisplay;
            ILength tol = new Length(0.002 * display.MapScale);
            IPosition closest = orient.LineGeometry.GetClosest(refpos, tol);
            if (closest == null)
                return false;

            m_Orient = orient;
            if (m_Orient==null)
                return false;

            // Highlight the new orientation line
            m_Orient.Render(display, new HighlightStyle());

            // Get the rotation angle
            IPointGeometry cg = PointGeometry.Create(closest);
            double rot = m_Orient.LineGeometry.GetRotation(cg);
            SetRotation(rot);
            return true;
        }

        /// <summary>
        /// Sees whether the orientation of the new text should be altered. This
        /// occurs if the auto-orient capability is enabled, and the specified
        /// position is close to any visible lne.
        /// </summary>
        /// <param name="posn">The position to use for making the check.</param>
        /// <returns></returns>
        LineFeature GetOrientation(IPointGeometry posn)
        {
            // The ground tolerance is 2mm at the draw scale.
            ISpatialDisplay display = ActiveDisplay;
            double tol = 0.002 * display.MapScale;

            // If we previously selected something, see if the search point
            // lies within tolerance. If so, there's no change.
            if (m_Orient != null)
            {
                double dist = m_Orient.Distance(posn).Meters;
                if (dist < tol)
                    return m_Orient;
            }

            // Get the map to find the closest line
            CadastralMapModel map = CadastralMapModel.Current;
            ISpatialIndex index = map.Index;
            return (index.QueryClosest(posn, new Length(tol), SpatialType.Line) as LineFeature);
        }

        /// <summary>
        /// Do any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn.</param>
        internal override void Paint(PointFeature point)
        {
            HighlightStyle style = new HighlightStyle();

            if (m_Orient != null)
            {
                style.ShowLineEndPoints = false;
                m_Orient.Render(ActiveDisplay, style);
            }

            if (m_IsAutoPos && m_Polygon!=null)
            {
                if (m_AutoPosition==null)
                    m_AutoPosition = m_Polygon.GetLabelPosition(Width, Height);

                DrawRect(m_AutoPosition);
            }
            else
                base.Paint(point);

            if (IsValidPolygon())
                m_Polygon.Render(ActiveDisplay, style);
        }

        /// <summary>
        /// Performs any processing when the text magnification factor has been changed. If
        /// the text position is being calculated, a new position will be calculated to
        /// account for the size change.
        /// </summary>
        internal override void OnSizeFactorChange()
        {
            if (m_IsAutoPos && m_Polygon!=null)
            {
                m_AutoPosition = m_Polygon.GetLabelPosition(Width, Height);
                //DrawRect(m_AutoPosition);
            }
        }
    }
}
