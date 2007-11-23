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
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Editor.Properties;
using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="17-NOV-1999" was="CuiFileCheck" />
    /// <summary>
    /// User interface for checking a file. This initially presents the user with a dialog
    /// for indicating the types of checks that need to be performed. After dismissing that
    /// initial dialog, the file is checked and a smaller review dialog appears. The user can
    /// then step through each potential problem to see what's going on. The user can also
    /// make edits while the check review dialog is on screen (the user decides to fix
    /// something). That being the case, the checking needs to take care of the fact that
    /// things may have changed since the check was originally made.
    /// </summary>
    /// <remarks>Unlike other UI classes, this one does NOT inherit from <c>CommandUI</c></remarks>
    class FileCheckUI : IDisposable
    {
        #region Class data

        /// <summary>
        /// Modeless dialog that describes the current item.
        /// </summary>
        CheckReviewForm m_Status;

        /// <summary>
        /// Flag bits indicating the checks that are being performed.
        /// </summary>
        CheckType m_Options;

        /// <summary>
        /// Results of the check.
        /// </summary>
        List<CheckItem> m_Results;

        /// <summary>
        /// The sequence of the last edit that was performed prior to the check.
        /// </summary>
        uint m_OpSequence;

        /// <summary>
        /// The icons used to highlight problems.
        /// </summary>
        //ImageList m_Icons;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal FileCheckUI()
        {
            m_Status = null;
            m_OpSequence = CadastralMapModel.Current.LastOpSequence;
            m_Options = CheckType.Null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // Get rid of the dialog that shows the current problem.
            if (m_Status!=null)
            {
                m_Status.Dispose();
                m_Status = null;
            }
        }

        #endregion

        internal bool CanRollback(uint seq)
        {
            return (seq>m_OpSequence);
        }

        /// <summary>
        /// Runs the file check.
        /// </summary>
        /// <returns>True if file check was initiated.</returns>
        internal bool Run()
        {
            // Get the user to specify what needs to be checked.
            FileCheckForm dial = new FileCheckForm();
            if (dial.ShowDialog()!=DialogResult.OK)
            {
                dial.Dispose();
                return false;
            }

            m_Options = dial.Options;
            dial.Dispose();

            // Confirm that at least one type of check has been specified
            if (m_Options==CheckType.Null)
            {
                MessageBox.Show("You must pick something you want to check.");
                return false;
            }

            // Start the item dialog (modeless).
            m_Status = new CheckReviewForm(this);
            m_Status.Show();

            // Make the initial check.
            int nCheck = CheckMap();

            // Let the review dialog know.
            m_Status.OnFinishCheck(nCheck, m_Results.Count);

            // Paint any markers... will be done by controller in idle time
            // Paint(false);

            // The check may have failed if an edit was in progress.
            return (nCheck>=0);
        }

        /// <summary>
        /// Redraws any check icons.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            if (m_Results==null)
                return;

            foreach (CheckItem check in m_Results)
                check.Render(display, style);

            // Tell the status window too.
            if (m_Status!=null)
                m_Status.Render(display, style);
        }

        /// <summary>
        /// Checks the current map.
        /// </summary>
        /// <returns>The number of objects checked (-1 if the map currently has an
        /// active operation in progress).</returns>
        int CheckMap()
        {
            Debug.Assert(m_Status!=null);

            // Reset current set of problems.
            KillResults();

            // Return if the map has an active operation.
            CadastralMapModel map = CadastralMapModel.Current;
            if (map.IsCommittingEdit)
            {
                MessageBox.Show("Cannot make check because an edit appears to be in progress.");
                return -1;
            }

            ISpatialDisplay display = EditingController.Current.ActiveDisplay;
            Control c = display.MapPanel;

            try
            {
                c.Cursor = Cursors.WaitCursor;
                FileCheckQuery check = new FileCheckQuery(map, m_Options, OnCheck);
                m_Results = check.Result;
                return check.NumChecked;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }

            finally
            {
                c.Cursor = Cursors.Default;
            }
        }

        void OnCheck(int numChecked)
        {
            m_Status.ShowProgress(numChecked);
        }

        /// <summary>
        /// Returns the next thing to check (if anything).
        /// </summary>
        /// <param name="index">The index of the check to retrieve.</param>
        /// <returns>The retrieved check object (null if object is out of range).</returns>
        internal CheckItem GetResult(int index)
        {
            if (index>=0 && index<m_Results.Count)
                return m_Results[index];
            else
                return null;
        }

        /// <summary>
        /// Handles request to wrap things up.
        /// </summary>
        internal void Finish()
        {
            // Get rid of modeless dialog.
            if (m_Status!=null)
            {
                m_Status.Dispose();
                m_Status = null;
            }

            // Tell the controller that the check is finished.
            EditingController.Current.OnFinishCheck();
        }

        /// <summary>
        /// Resets check information prior to making a new check.
        /// </summary>
        void KillResults()
        {
            // Let review dialog that any previous results are now irrelevant
            if (m_Status!=null)
                m_Status.OnResetCheck();
        }

        /// <summary>
        /// Do stuff when a user has just completed an edit.
        /// </summary>
        internal void OnFinishOp()
        {
            ISpatialDisplay display = EditingController.Current.ActiveDisplay;
            IDrawStyle style = EditingController.Current.DrawStyle;
            bool doPost = false;    // Need to post-process the list?

            foreach (CheckItem check in m_Results)
            {
                // Note the current result. Skip if has been fixed.
                CheckType oldTypes = check.Types;
                if (oldTypes == CheckType.Null)
                    continue;

                // Get the current state of the check & restrict to those results that
                // the user wants to see
                CheckType newTypes = check.ReCheck();
                newTypes &= m_Options;

                // If a change has arisen, paint out those results that no longer apply,
                // and update the result.

        		if (newTypes != oldTypes)
                {
			        check.PaintOut(display, style, newTypes);
			        check.Types = newTypes;

        			// If we just fixed a polygon with no enclosing polygon, remember to post-process
                    // the results once we're through the loop.
                    doPost = ((oldTypes & CheckType.NotEnclosed)!=0 && (newTypes & CheckType.NotEnclosed)==0);
		        }
            }

            // If we eliminated at least one "no polygon enclosing polygon"
            // problem, do a post check to look for any single occurence of
            // another problem with the same type. If we find one, mark it
            // as fixed too (but don't remove it from the list, since that
            // might screw up the review dialog).
            if (doPost)
                FileCheckQuery.PostCheck(m_Results, false);

            // Tell the status dialog.
            if (m_Status!=null)
                m_Status.OnFinishOp();

            // Ensure screen immediately reflects any changes
            Render(display, style);
        }

        /// <summary>
        /// Re-checks the map. This is invoked by the <see cref="CheckReviewForm"/> sub-dialog
        /// when the user hits the end of the list having made at least one edit.
        /// </summary>
        /// <returns>The number of objects checked (-1 if the map currently has an active
        /// operation in progress).</returns>
        internal int ReCheck()
        {
        	// Update the last operation prior to the check results.
            m_OpSequence = CadastralMapModel.Current.LastOpSequence;

            // Redo the check.
            int nCheck = CheckMap();
            if (nCheck<0)
                return nCheck;

            // Make sure the review dialog knows that the check is done.
            if (m_Status!=null)
                m_Status.OnFinishCheck(nCheck, m_Results.Count);

            return nCheck;
        }
    }
}
