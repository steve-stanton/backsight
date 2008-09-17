// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

            // 20080422 - FOR THE TIME BEING, DEFAULT TO USING THE ID (NO ATTRIBUTES)
            noAttributesCheckBox.Checked = true;
        }

        private void entityTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Get the new polygon type.
            m_PolygonType = entityTypeComboBox.SelectedEntityType;

            // Ensure that the schema combo lists everything for the
            // newly selected entity type.
            ListSchemas();
        }

        private void schemaComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Get the new polygon attribute schema
            m_Schema = schemaComboBox.SelectedTable;
            OnDefaultAnnotation();
        }

        private void defaultAnnotationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            OnDefaultAnnotation();
        }

        /// <summary>
        /// Handles event when user clicks on the checkbox that says "Use ID".
        /// </summary>
        void OnDefaultAnnotation()
        {
            // What does the user want to do?
            m_IsUseId = defaultAnnotationCheckBox.Checked;

            // If the user does NOT want to use the ID, confirm that we
            // have a schema!
            if (!m_IsUseId && m_Schema == null)
            {
                string errmsg = String.Empty;
                errmsg += ("The specified polygon type does not have any" + System.Environment.NewLine);
                errmsg += ("annotation templates, so you can only use the ID.");
                MessageBox.Show(errmsg);
                m_IsUseId = true;
            }

            UseId(m_IsUseId);
        }

        private void annotationTemplateComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Get the new annotation template
            m_Template = (ITemplate)annotationTemplateComboBox.SelectedItem;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles event when user clicks on the checkbox that says "List all choices".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allSchemasCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Does the user want to see all schema(s)?
            m_IsAllSchemas = (allSchemasCheckBox.Checked);

            // Display what the user wants to see.
            ListSchemas();

            // If the user wants all choices, force the combo to drop down.            
            if (m_IsAllSchemas)
                schemaComboBox.DroppedDown = true;
        }

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

        /// <summary>
        /// Handles click on the "Don't have attributes" checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noAttributesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_IsNoAttr = noAttributesCheckBox.Checked;

            if (m_IsNoAttr)
            {
                // Clear out the schema and template combos and disable them.
                schemaComboBox.Clear();
                annotationTemplateComboBox.DataSource = null;

                // And disable them
                schemaComboBox.Enabled = false;
                annotationTemplateComboBox.Enabled = false;

                // Also the checkbox that says "List all choices"
                allSchemasCheckBox.Checked = false;
                allSchemasCheckBox.Enabled = false;
                m_IsAllSchemas = false;

                // Null any previous selection.
                m_Schema = null;
                m_Template = null;

                // The ID is the only choice.
                UseId(true);

                // And set the focus to the OK button.
                okButton.Focus();
            }
            else
            {
                // Load the schema and template combos.
                ListSchemas();

                // And enable stuff.
                schemaComboBox.Enabled = true;
                annotationTemplateComboBox.Enabled = true;
                allSchemasCheckBox.Enabled = true;
            }
        }
    }
}