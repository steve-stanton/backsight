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
using System.Windows.Forms;

using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="17-NOV-1999" was="CuiFileCheck" />
    /// <summary>
    /// User interface for checking a file. Note that unlike other UI classes, this one does NOT
    //	inherit from <c>CommandUI</c>.
    /// </summary>
    class FileCheckUI
    {
        #region Class data

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates a new <c>FileCheckUI</c>
        /// </summary>
        internal FileCheckUI()
        {
        }

        #endregion

        internal bool CanRollback(uint seq)
        {
            throw new NotImplementedException("FileCheckUI.CanRollback");
            //return (seq>m_OpSequence);
        }

        /// <summary>
        /// Runs the file check.
        /// </summary>
        /// <returns>True if file check was initiated.</returns>
        internal bool Run()
        {
            // Get the user to specify what needs to be checked.
            FileCheckForm dial = new FileCheckForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                m_Options = dial.Options;
            }

            return false;
        }
        /*
LOGICAL CuiFileCheck::Run ( void ) {

	// Get the user to specify what needs to be checked.
	CdFileCheck dial;
	if ( dial.DoModal()!=IDOK ) return FALSE;

	// Pick up the options.
	m_Options = dial.GetOptions();
	if ( m_Options==0 ) {
		ShowMessage("You must pick something you want to check.");
		return FALSE;
	}

	// Start the item dialog (modeless).
	m_pStatus = new CdCheck(*this);
	m_pStatus->Create(IDD_CHECK,GetpView());

	// Make the initial check.
	INT4 nCheck = CheckMap();

	// Let the review dialog know.
	m_pStatus->OnFinishCheck(nCheck,m_Results.GetCount());

	// Paint any markers.
	Paint();

	// The check may have failed if an edit was in progress.
	return (nCheck>=0);

} // end of Run
         */
    }
}
