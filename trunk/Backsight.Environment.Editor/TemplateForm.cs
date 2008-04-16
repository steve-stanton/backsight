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

using Smo = Microsoft.SqlServer.Management.Smo;

using Backsight.SqlServer;
using System.Collections.Generic;


namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Dialog that lets the user define templates for text formatting
    /// </summary>
    public partial class TemplateForm : Form
    {
        readonly IEditTemplate m_Edit;

        /// <summary>
        /// Access to the database
        /// </summary>
        readonly TableFactory m_TableFactory;

        /// <summary>
        /// The table (if any) that refers to the template
        /// </summary>
        ITable m_Table;

        internal TemplateForm()
            : this(null)
        {
        }


        internal TemplateForm(IEditTemplate edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit == null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateTemplate();
            }

            m_TableFactory = new TableFactory();
            m_Table = null;
            m_Edit.BeginEdit();
        }

        private void TemplateForm_Shown(object sender, EventArgs e)
        {
            // Load the schema combo (without the <none> item).
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            ITable[] tables = ec.Tables;
            tableComboBox.DataSource = tables;

            // Attempt to locate a table that refers to the template (while the
            // database structure makes it possible to relate a template to
            // more than one table, the UI works under the assumption that a
            // template will apply to no more than one table - if this proves
            // ok in the longer run, the database structure should be revised
            // to match).

            if (m_Edit.Id != 0)
            {
                m_Table = Array.Find<ITable>(tables, delegate(ITable t)
                {
                    ITemplate[] tableTemplates = t.Templates;
                    ITemplate foundTemplate = Array.Find<ITemplate>(tableTemplates, delegate(ITemplate temp)
                                        { return temp.Id == m_Edit.Id; });
                    return (foundTemplate != null);
                });
            }

            if (m_Table == null)
            {
                fieldsListBox.Enabled = false;
                formatTextBox.Enabled = false;
            }

            // Return if we're creating a new template
            if (m_Edit.IsNew)
            {
                //if (tables.Length>0)
                //    tableComboBox.SelectedItem = tables[0];

                return;
            }

            // Define alternate title for the dialog window
            this.Text = "Update Text Template";

            // Display the name of the item.
            nameTextBox.Text = m_Edit.Name;

            // Display the current format.
            formatTextBox.Text = m_Edit.Format;

            // If the schema is defined (it should be), select it. Then
            // load the list of fields, and select those that the template
            // already uses.
            if (m_Table != null)
            {
                tableComboBox.SelectedItem = m_Table;
                ListFields();
            }
            else
                MessageBox.Show("Template does not have an associated table");

            // Set focus on the OK button.
            okButton.Focus();
        }

        void ListFields()
        {
            // The list should be enabled.
            fieldsListBox.Enabled = (m_Table != null);
            fieldsListBox.DataSource = null;

            if (m_Table != null)
            {
                Smo.Table t = m_TableFactory.FindTableByName(m_Table.TableName);

                if (t != null)
                {
                    // You can only poke the columns into the ListBox via a BindingSource,
                    // don't know why, don't care
                    BindingSource bs = new BindingSource();
                    bs.DataSource = t.Columns;
                    fieldsListBox.DataSource = bs;
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Ensure the template name is defined
            string name = nameTextBox.Text.Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("A template name must be specified");
                nameTextBox.Focus();
                return;
            }

            // Ensure the schema is defined.
            if (m_Table == null)
            {
                MessageBox.Show("The template must be related to a table.");
                tableComboBox.Focus();
                return;
            }

            m_Edit.Name = name;
            //m_Table.Templates.
            m_Edit.FinishEdit();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void fieldsListBox_DoubleClick(object sender, EventArgs e)
        {
            OnSelect();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            OnSelect();
        }

        void OnSelect()
        {
        }

        /*
void CdTemplate::OnSelect() 
{
	// Get the ID of the selected field.
	CListBox* pList = (CListBox*)GetDlgItem(IDC_AVAILABLE);
	int index = pList->GetCurSel();
	if ( index==LB_ERR ) return;
	CbField* pField = (CbField*)pList->GetItemDataPtr(index);

	// Get the sequence of the field within the schema.
	UINT4 sid = m_Edit.GetSchemaId();
	CbSchema* pSchema = m_Data.FindSchema(sid);
	if ( !pSchema ) return;
	INT4 fseq = pSchema->GetSequence(pField);

	// Append the field to the edit control.
	CEdit* pFormat = (CEdit*)GetDlgItem(IDC_FORMAT);
	CString format;
	pFormat->GetWindowText(format);

	// Trim off any trailing white space.
	format.TrimRight();

	// Append a blank and the new field.
	CString fstr;
	fstr.Format(" %%%d",fseq);
	format += fstr;
	pFormat->SetWindowText(format);

	// And append to the template object as well.
	m_Edit.SetFormat(format);
	m_Edit.AttachField(pField->GetId());
}
         */

        private void tableComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Get the selected schema.
            m_Table = (ITable)tableComboBox.SelectedItem;

            // And list the fields
            ListFields();
        }

        /*
    void CdTemplate::OnSelchangeSchemas() 
    {
        CComboBox* pCombo = (CComboBox*)GetDlgItem(IDC_SCHEMAS);
        int index = pCombo->GetCurSel();
        if ( index==LB_ERR ) return;
        CbSchema* pSchema = (CbSchema*)pCombo->GetItemDataPtr(index);
        if ( !pSchema ) return;

        // Display the fields for the selected schema.
        ListFields(*pSchema);

        // Clear out the current format and ensure the control
        // is enabled.
        CString str;
        CEdit* pEdit = (CEdit*)GetDlgItem(IDC_FORMAT);
        pEdit->SetWindowText(str);
        pEdit->EnableWindow(TRUE);

        // Fix the template object to refer to the new schema (with
        // no fields selected so far).
        m_Edit.SetSchemaId(pSchema->GetId());
        m_Edit.RemoveFields();
        m_Edit.SetFormat(str);
    }
         */
    }
}