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

namespace Backsight.Editor.Forms
{
    public partial class UpdateForm : Form
    {
        public UpdateForm()
        {
            InitializeComponent();
        }

        internal void Display(Feature feat)
        {
            throw new NotImplementedException("UpdateForm.Display");

            /*
            this.Text = "Update";

	        // Nothing to do if feature was not specified.
	        if (feat==null)
            {
		        CWnd* pInfo = GetDlgItem(IDC_INFO);
		        pInfo->SetWindowText("Nothing selected for update");
		        Enable(false);
		        return;
	        }

	        // Get the creating op and display info about it.
	        IOperation pop = feat.Creator;
	        Show(pop, feat);
             */
        }

        internal void OnAbortUpdate()
        {
            throw new NotImplementedException("UpdateForm");
            //ShowWindow(SW_RESTORE);
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
    }
}
