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

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="17-NOV-1999" was="CdCheck" />
    /// <summary>
    /// Dialog for reviewing the results of a file check
    /// </summary>
    partial class CheckReviewForm : Form
    {
        #region Class data

        /// <summary>
        /// The object running the overall check.
        /// </summary>
        readonly FileCheckUI m_Cmd;

        /// <summary>
        /// The index of the current item (-1 prior to first item).
        /// Should always be less than <c>m_nTotal</c>.
        /// </summary>
        int m_nCurrent;

        /// <summary>
        /// The total number of items.
        /// </summary>
        int m_nTotal;

        /// <summary>
        /// Have edits been made?
        /// </summary>
	    bool m_IsEdited;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>CheckReviewForm</c> for the specified file checker.
        /// </summary>
        /// <param name="cmd">The file check that needs to be reviewed</param>
        internal CheckReviewForm(FileCheckUI cmd)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_nTotal = 0;
            m_nCurrent = -1;
            m_IsEdited = false;
        }

        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            /*
	// If the index of the next item is beyond the last one,
	// that's us done.
	m_nCurrent++;
	if ( AtFinish() ) return;

	// Get the next item to check.
	INT4 nShow = ShowCheck(m_nCurrent,TRUE);

	// If we auto-advanced, repeat what we did above.
	if ( nShow != m_nCurrent ) {
		m_nCurrent = nShow;
		AtFinish();
	}
             */
        }

        /*
private LOGICAL CdCheck::AtFinish ( void ) {

	// If the next item to show is beyond the last item
	if ( m_nCurrent >= m_nTotal ) {

		// If anything was edited during the review, get
		// the command to recheck. Otherwise get the command
		// to wrap things up.

		if ( m_IsEdited ) {
			m_Cmd.ReCheck();
		}
		else {
			OnResetCheck();
			m_Cmd.Finish();
			return TRUE;
		}
	}

	return FALSE;
}
        */

        int ShowCheck(int index, bool advanceOnFix)
        {
            // Get the check object.
            CheckItem check = m_Cmd.GetResult(index);
            if (check==null)
                return -1;

            // If auto-advance is allowed, and the check has been fixed, keep going until
            // we find something that hasn't been fixed.
            int nShow = index;
            if (advanceOnFix)
            {
                while (check.Types==CheckType.Null)
                {
                    nShow++;
                    if (nShow >= m_nTotal)
                        return nShow;

                    check = m_Cmd.GetResult(nShow);
                    if (check==null)
                        return -1;
                }
            }

            // If there is even more, ensure the OK button says "Next".
            // Otherwise display "Finish" or "ReCheck" depending on
            // whether any edits have been performed.
            if ((nShow+1) == m_nTotal)
            {
                if (m_IsEdited)
                    okButton.Text = "Re-Chec&k";
                else
                    okButton.Text = "&Finish";
            }
            else
                okButton.Text = "&Next";

            // Enable the "Back" button if we're now beyond the 1st item.
            previousButton.Enabled = (nShow>0);

            // Display current sequence
            statusGroupBox.Text = String.Format("{0} of {1}", nShow+1, m_nTotal);

            // Get the check to provide an explanation.
            string msg = (check==null ? "sync lost" : check.Explanation);

            // Get a position for the check. If we can't get one, append a note to the explanation.
            IPosition pos = check.Position;
            if (pos==null)
                msg += " (cannot re-locate the problem)";

            explanationLabel.Text = msg;

            // Return if we did not get a position for the check, ensuring that this
            // focus is with THIS dialog.
            if (pos==null)
            {
                this.Focus();
                return nShow;
            }

            // Get the current draw window, and shrink it by 10% all the way round.
            ISpatialDisplay display = CadastralEditController.Current.ActiveDisplay;
            Window win = new Window(display.Extent);
            win.Expand(-0.1);

            // If necessary, re-center the draw on the position we've got.
            if (!win.IsOverlap(pos))
                display.Center = pos;

            // Select the object involved (this highlights the object
            // and shifts the focus to the main map window).
            check.Select();

            // Return the index of the check that was actually shown.
            return nShow;
        }

        private void CheckReviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Cmd.Finish();        
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            if (m_nCurrent>0)
            {
            	m_nCurrent--;
	            ShowCheck(m_nCurrent, false);
            }
        }

        internal void ShowProgress (int nDone)
        {
	        // Only display status every 100 done.
	        if ((nDone%100) == 0)
            {
                explanationLabel.Text = String.Format("{0} features checked", nDone);
                explanationLabel.Refresh();
            }
        }

        private void CheckReviewForm_Shown(object sender, EventArgs e)
        {
	        // The buttons should be invisible to start with.
	        ShowButtons(false);

            // Ensure focus is on the OK(/First) button.
            okButton.Focus();
        }

        internal void OnFinishCheck(int nCheck, int nProblem)
        {
            // Display summary message.
            if (nProblem > 0)
            {
                statusGroupBox.Text = String.Format("0 of {0}", nProblem);

                string msg = String.Empty;
                msg += String.Format("{0} features checked.", nCheck);
                msg += System.Environment.NewLine;
                msg += String.Format("{0} possible problem", nProblem);
                if (nProblem > 1)
                    msg += "s";
                explanationLabel.Text = msg;

                okButton.Text = "&First";
            }
            else
            {
                statusGroupBox.Text = String.Empty;
                explanationLabel.Text = "No problems found.";
                okButton.Text = "OK";
            }

            // Disable the Back button (you have to click
            // on OK to initially proceed to the first problem).
            previousButton.Enabled = false;

            // Reveal the buttons.
            ShowButtons(true);

            // Ensure the focus us on the ok ("First") button.
            // ... doesn't seem to do it?
            okButton.Focus();

            // Remember where we are.
            m_nTotal = nProblem;
            m_nCurrent = -1;
        }

        void ShowButtons(bool show)
        {
            previousButton.Visible = okButton.Visible = show;
        }

        internal void OnResetCheck()
        {
            this.Text = "File Check Review";
            m_nTotal = 0;
            m_nCurrent = -1;
            m_IsEdited = false;
            ShowButtons(false);
        }

        void Paint(ISpatialDisplay display)
        {
            // If a specific check is active, paint it.
            // This currently only does stuff for CeSplitCheck objects.

            if (m_nCurrent < 0 || m_nCurrent >= m_nTotal)
                return;

            CheckItem checkResult = m_Cmd.GetResult(m_nCurrent);
            if (checkResult != null)
                checkResult.Paint(display);
        }

        /// <summary>
        /// Do stuff when a user has just completed an edit.
        /// </summary>
        /// <param name="op">The operation that has just been completed.</param>
        void OnFinishOp(Operation op)
        {
            // Remember that an edit has occurred.
            if (!m_IsEdited)
            {
                this.Text = "File Check Review (edits made)";
                m_IsEdited = true;
            }

            // Redisplay the info for the current edit (if any).
            if (m_nCurrent < m_nTotal)
                ShowCheck(m_nCurrent, false);

            okButton.Focus();
        }
    }
}