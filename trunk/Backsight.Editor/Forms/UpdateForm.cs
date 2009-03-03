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
using System.Windows.Forms;
using System.Text;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="30-NOV-1999" was="CdUpdate"/>
    /// <summary>
    /// Dialog for showing summary information about a feature that is currently
    /// the focus of an update.
    /// </summary>
    partial class UpdateForm : Form
    {
        #region Class data

        /// <summary>
        /// The command running this dialog.
        /// </summary>
        readonly UpdateUI m_Cmd;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateForm"/> class.
        /// </summary>
        /// <param name="cmd">The command running this dialog (not null)</param>
        /// <exception cref="ArgumentNullException">If the supplied update command is null</exception>
        internal UpdateForm(UpdateUI cmd)
        {
            InitializeComponent();

            if (cmd == null)
                throw new ArgumentNullException();

            m_Cmd = cmd;
        }

        #endregion

        private void UpdateForm_Shown(object sender, EventArgs e)
        {
            infoLabel.Text = "Nothing selected for update";
            Enable(false);

            // Cancel unless the user explicitly clicks the Finish button
            this.DialogResult = DialogResult.Cancel;
        }

        private void dependenciesButton_Click(object sender, EventArgs e)
        {
            //m_Cmd.Dependencies();
        }

        private void predecessorsButton_Click(object sender, EventArgs e)
        {
            // Get the command to display the predecessors. If a
            // change is ultimately made, another call to Display
            // will be made.
            //m_Cmd.Predecessors();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            //m_Cmd.Cancel();
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            //m_Cmd.Finish();
        }

        internal void Display(Feature feat)
        {
            this.Text = "Update";

            // Nothing to do if feature was not specified.
            if (feat==null)
            {
                infoLabel.Text = "Nothing selected for update";
                Enable(false);
            }
            else
            {
                // Get the creating op and display info about it.
                Operation pop = feat.Creator;
                ShowInfo(pop, feat);
            }
        }

        void ShowInfo(Operation pop, Feature feat)
        {
            if (pop == null)
            {
                infoLabel.Text = "No editing command";
                Enable(false);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                string newLine = System.Environment.NewLine;
                sb.AppendFormat("{0}\t{1}{2}", "Editor command:", pop.Name, newLine);
                sb.AppendFormat("{0}\t{1}{2}", "Edit sequence:", pop.EditSequence, newLine);
                sb.AppendFormat("{0}\t{1}", "Created on:", pop.Session.StartTime.Date);
                infoLabel.Text = sb.ToString();
                Enable(true);

                // Disable the Predecessors button if the selected
                // feature doesn't actually have one.
                //if (!UpdateUI.GetPredecessor(feat))
                //    predecessorsButton.Enabled = false;

                // Disable the Update button if the operation is not updateable.
                //if (pop.CanCorrect)
                //    updateButton.Focus();
                //else
                    updateButton.Enabled = false;
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            //m_Cmd.Update();
        }

        internal void OnFinishUpdate(Operation problem) 
        {
            throw new NotImplementedException("UpdateForm");
            /*
	        if (problem!=null)
            {
		        Show(pProblem,0);
		        this.Text = "Problem";
		        GetDlgItem(IDC_UPDATE)->SetWindowText("Fi&x It");
		        GetDlgItem(IDC_UPDATE)->SetFocus();
		        SetIcon(IDI_STOP_LIGHT);
		        GetDlgItem(IDOK)->EnableWindow(FALSE);
		        GetDlgItem(IDCANCEL)->SetWindowText("Und&o");
	        }
	        else
            {
		        this.Text = "Update";
		        GetDlgItem(IDC_UPDATE)->SetWindowText("&Update");
		        SetIcon(IDI_GO_LIGHT);
		        GetDlgItem(IDOK)->EnableWindow(TRUE);
		        GetDlgItem(IDOK)->SetFocus();
		        GetDlgItem(IDCANCEL)->SetWindowText("Cancel");
	        }

	        ShowWindow(SW_RESTORE);
             */
        }

        /*

void CdUpdate::SetIcon ( const INT4 id ) {

	CStatic* pWnd = (CStatic*)GetDlgItem(IDC_LIGHT);
	HICON icon = AfxGetApp()->LoadIcon(id);
	pWnd->SetIcon(icon);
}
         */

        internal void OnAbortUpdate()
        {
            throw new NotImplementedException("UpdateForm");
            //ShowWindow(SW_RESTORE);
        }

        /// <summary>
        /// Enables (or disables) every button, except Cancel.
        /// </summary>
        /// <param name="isEnable">Should buttons be enabled?</param>
        void Enable(bool isEnable)
        {
            updateButton.Enabled = isEnable;
            predecessorsButton.Enabled = isEnable;
            dependenciesButton.Enabled = isEnable;
            finishButton.Enabled = isEnable;
        }

        /*
void CdUpdate::OnClose() 
{
	m_Cmd.Cancel();
}
         */

        /*

void CdUpdate::SetUpdateCount ( const UINT4 n )
{
	if ( n == 0 )
		GetDlgItem(IDC_NUM_UPDATE)->ShowWindow(SW_HIDE);
	else
	{
		CWnd* pText = GetDlgItem(IDC_NUM_UPDATE);
		CString text;
		text.Format("Update %d",n);
		pText->SetWindowText(text);
		pText->ShowWindow(SW_SHOW);
	}
}
         */
    }
}
