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
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdNewLabel" />
    /// <summary>
    /// Dialog that lets the user specify a polygon label
    /// </summary>
    partial class NewLabelForm : Form
    {
        #region Class data

        /// <summary>
        /// The schema for the polygon attribute data
        /// </summary>
        ITable m_Schema;

        /// <summary>
        /// The template for the polygon attribute data
        /// </summary>
        ITemplate m_Template;

        /// <summary>
        /// Entity type for the polygons.
        /// </summary>
        IEntity m_PolygonType;

        /// <summary>
        /// True if all applicable schemas are listed.
        /// </summary>
        bool m_IsAllSchemas;

        /// <summary>
        /// True if adding labels that correspond to the polygon ID.
        /// </summary>
        bool m_IsUseId;

        /// <summary>
        /// True if no attributes.
        /// </summary>
        bool m_IsNoAttr;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewLabelForm</c>
        /// </summary>
        internal NewLabelForm()
        {
            InitializeComponent();

            m_Schema = null;
            m_Template = null;
            m_PolygonType = null;
            m_IsAllSchemas = false;
            m_IsUseId = false;
            m_IsNoAttr = false;
        }

        #endregion

        /// <summary>
        /// Entity type for the polygons.
        /// </summary>
        internal IEntity Entity
        {
            get { return m_PolygonType; }
        }

        /// <summary>
        /// The schema for the polygon attribute data
        /// </summary>
        internal ITable Schema
        {
            get { return m_Schema; }
        }

        /// <summary>
        /// The template for the polygon attribute data
        /// </summary>
        internal ITemplate Template
        {
            get { return m_Template; }
        }

        private void NewLabelForm_Shown(object sender, EventArgs e)
        {
            // Load up entity combo box with the default type for polygons.
            m_PolygonType = entityTypeComboBox.Load(SpatialType.Polygon);

            // Load the schema (and annotation template) combo boxes. By
            // default, you get just the default schemas for the entity
            // type we have defined.
            ListSchemas();
        }

        /*
void CdNewLabel::OnSelchangeEntity() 
{
	// Get the new polygon type.
	CComboBox* pBox = (CComboBox*)GetDlgItem(IDC_ENTITY);
	m_pPolygonType = ReadEntityCombo(pBox);

	// Ensure that the schema combo lists everything for the
	// newly selected entity type.
	ListSchemas();
}

void CdNewLabel::OnSelchangeAttributeSchema() 
{
//	Get the new polygon attribute schema
	CComboBox* pSchBox = (CComboBox*)GetDlgItem(IDC_ATTRIBUTE_SCHEMA);
	m_pSchema = ReadSchemaCombo(pSchBox);
	OnDefaultAnnotation();
}

////////////////////////////////////////////////////////////////////////////////////
//
//  @mfunc	Handle event when user clicks on the checkbox that says
//			"Use ID".
//
////////////////////////////////////////////////////////////////////////////////////

void CdNewLabel::OnDefaultAnnotation ( void ) {

	// What does the user want to do?
	CButton* pTmpDef = (CButton*)GetDlgItem(IDC_DEFAULT_ANNOTATION);
	m_IsUseId = (pTmpDef->GetCheck()==1);

	// If the user does NOT want to use the ID, confirm that we
	// have a schema!
	if ( !m_IsUseId && m_pSchema==0 ) {
		CString errmsg;
		errmsg.Format( "%s\n%s"
			, "The specified polygon type does not have any"
			, "annotation templates, so you can only use the ID.");
		AfxMessageBox(errmsg);
		m_IsUseId = TRUE;
	}

	UseId(m_IsUseId);

} // end of OnDefaultAnnotation

void CdNewLabel::OnSelchangeAnnotationTemplate() 
{
//	Get the new annotation template
	CComboBox* pTmpBox = (CComboBox*)GetDlgItem(IDC_ANNOTATION_TEMPLATE);
	m_pTemplate = ReadTemplateCombo(pTmpBox);
}

void CdNewLabel::OnOK() 
{
	CDialog::OnOK();
}

////////////////////////////////////////////////////////////////////////////////////
//
//  @mfunc	Handle event when user clicks on the checkbox that says
//			"List all choices".
//
////////////////////////////////////////////////////////////////////////////////////

void CdNewLabel::OnAllSchemas ( void ) {

	// Does the user want to see all schema(s)?
	CButton* pAllSch = (CButton*)GetDlgItem(IDC_ALL_SCHEMAS);
	m_IsAllSchemas = (pAllSch->GetCheck()==1);

	// Display what the user wants to see.
	ListSchemas();

	// If the user wants all choices, force the combo to drop down.
	if ( m_IsAllSchemas ) {
		CComboBox* pSchBox = (CComboBox*)GetDlgItem(IDC_ATTRIBUTE_SCHEMA);
		pSchBox->ShowDropDown();
	}

} // end of OnAllSchemas
        */

        /// <summary>
        /// Loads the schema combo box (as well as the annotation templates that apply).
        /// </summary>
        void ListSchemas()
        {
            // Return if the user said "no attributes"
            if (m_IsNoAttr)
                return;

            // Clear out anything we previously had.
            schemaComboBox.DataSource = null;

            if (m_IsAllSchemas)
            {
                // Load up everything that relates to the layer that is currently active.
                m_Schema = schemaComboBox.Load(SpatialType.Polygon);
            }
            else
            {
                m_Schema = schemaComboBox.Load(m_PolygonType);
            }

            // List the applicable annotation templates.
            ListTemplates();
        }

        /// <summary>
        /// Loads the annotation template combo box.
        /// </summary>
        void ListTemplates()
        {
            // Return if the user said "no attributes"
            if (m_IsNoAttr)
                return;

            // Clear out anything we previously had.
            annotationTemplateComboBox.DataSource = null;

            // Try to NOT use an ID (if there is no schema, or the schema
            // has no templates, this will force use of the ID).
            UseId(false);
        }

        /// <summary>
        /// Remembers whether key text is being used or not.
        /// </summary>
        /// <param name="isUseId">True to use key text (in that case, <c>m_Template</c> will be
        /// nulled). [Default was true].</param>
        void UseId(bool isUseId)
        {
            // Remember the desired setting
            m_IsUseId = isUseId;

            // If the user does NOT want to use the ID, try to load up the template combo.
            if (!m_IsUseId && m_Schema!=null)
            {
                // Get the templates (if any) that are related to the schema
                ITemplate[] templates = m_Schema.Templates;

                // Don't know which one as yet.
                m_Template = null;

                if (templates.Length>0)
                {
                    annotationTemplateComboBox.DataSource = templates;
                    m_Template = templates[0];

                    // Enable the combo (with the template selected), ensure the "use ID"
                    // checkmark is clear, and return.
                    annotationTemplateComboBox.Enabled = true;
                    annotationTemplateComboBox.SelectedItem = m_Template;
                    defaultAnnotationCheckBox.Checked = false;
                    return;
                }
            }

            // MUST use the ID.
            m_IsUseId = true;

            // Ensure check mark is set.
            defaultAnnotationCheckBox.Checked = true;

            // You can't pick a template
            annotationTemplateComboBox.Enabled = false;
            annotationTemplateComboBox.DataSource = null;
            m_Template = null;
        }

        /*
////////////////////////////////////////////////////////////////////////////////////
//
//  @mfunc	Handle click on the "Don't have attributes" checkbox.
//
////////////////////////////////////////////////////////////////////////////////////

void CdNewLabel::OnNoAttributes ( void ) {

	CButton* pCheckBox = (CButton*)GetDlgItem(IDC_NO_ATTRIBUTES);
	CComboBox* pSchBox = (CComboBox*)GetDlgItem(IDC_ATTRIBUTE_SCHEMA);
	CComboBox* pTmpBox = (CComboBox*)GetDlgItem(IDC_ANNOTATION_TEMPLATE);
	CButton* pAllSch = (CButton*)GetDlgItem(IDC_ALL_SCHEMAS);

	// If the check-box has been set ...
	if ( pCheckBox->GetCheck() ) {

		m_IsNoAttr = TRUE;

		// Clear out the schema and template combos and disable them.
		pSchBox->ResetContent();
		pTmpBox->ResetContent();

		// And disable them
		pSchBox->EnableWindow(FALSE);
		pTmpBox->EnableWindow(FALSE);

		// Also the checkbox that says "List all choices"
		pAllSch->SetCheck(0);
		pAllSch->EnableWindow(FALSE);
		m_IsAllSchemas = FALSE;

		// Null any previous selection.
		m_pSchema = 0;
		m_pTemplate = 0;

		// The ID is the only choice.
		UseId();

		// And set the focus to the OK button.
		GetDlgItem(IDOK)->SetFocus();
	}
	else {

		// Reset flag so that ListSchemas will actually list.
		m_IsNoAttr = FALSE;

		// Load the schema and template combos.
		ListSchemas();

		// And enable stuff.
		pSchBox->EnableWindow(TRUE);
		pTmpBox->EnableWindow(TRUE);
		pAllSch->EnableWindow(TRUE);
	}

} // end of OnNoAttributes
         */
    }
}