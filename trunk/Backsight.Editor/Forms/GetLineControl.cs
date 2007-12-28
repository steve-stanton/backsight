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
using System.Drawing;

using Backsight.Environment;
using Backsight.Editor.Operations;
using Backsight.Forms;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdGetLine" />
    /// <summary>
    /// Dialog for getting the user to specify a line (for use with
    /// Intersection edits).
    /// </summary>
    partial class GetLineControl : UserControl
    {
        #region Class data

        /// <summary>
        /// The selected line.
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// Should the line be split at the intersection
        /// (-1 => no split, 0 => not specified, +1 => want split)
        /// </summary>
        int m_WantSplit;

        /// <summary>
        /// The highlight color for the line.
        /// </summary>
        Color m_LineColor;

        #endregion

        #region Constructors

        public GetLineControl()
        {
            InitializeComponent();

            m_Line = null;
            m_WantSplit = 0;
            m_LineColor = Color.Magenta;
        }

        #endregion

        internal LineFeature Line
        {
            get { return m_Line; }
        }

        internal bool WantSplit
        {
            get { return (m_WantSplit==1); }
        }

        /// <summary>
        /// Reacts to selection of a line on the map.
        /// </summary>
        /// <param name="point"></param>
        internal void OnSelectLine(LineFeature line)
        {
            // If we previously had a line selected, draw it normally.
            if (m_Line!=null)
                UnHighlight(m_Line);

            polBoundaryLabel.Visible = false;
            deletedLabel.Visible = false;

            m_Line = line;

            if (m_Line==null)
            {
                gotLineCheckBox.Checked = false;
            }
            else
            {
                gotLineCheckBox.Checked = true;

                if (m_Line.IsTopological)
                    polBoundaryLabel.Visible = true;

                if (m_Line.IsInactive)
                    deletedLabel.Visible = true;
            }

            // Reveal question if a line has been selected.
            questionGroupBox.Visible = gotLineCheckBox.Checked;
            //GetDlgItem(IDC_WANT_SPLIT)->ShowWindow(showq);
            //GetDlgItem(IDC_NO_SPLIT)->ShowWindow(showq);
            //GetDlgItem(IDC_CHECK_YES)->ShowWindow(showq);
            //GetDlgItem(IDC_CHECK_NO)->ShowWindow(showq);

	        // Get the view to de-select the line (if you don't, it might
	        // get redrawn in black if we pick a second line to intersect
	        // with).
	        //GetpView()->UnHighlight(0);

            // Draw the line properly.
	        OnDraw();
        }

        internal void OnDraw()
        {
            if (m_Line!=null)
                Highlight(m_Line);
        }


        internal void InitializeControl(IntersectForm parent, int lineNum)
        {
            //	If we are updating a feature that was previously created,
            //	load the original info. For line-line intersections,
            //	we need to know which page this is, to determine whether we
            //	should display info for the 1st or 2nd line.

            IntersectOperation op = parent.GetUpdateOp();
            ShowUpdate(op, lineNum);

            /*
            // Check the appropriate check boxes for the split stuff.
            if (m_Line!=null)
            {
                if (m_WantSplit==1)
                    yesCheckBox.Checked = true;
                else
                    noCheckBox.Checked = true;

            }

            // Make sure everything is drawn on top.
            OnDraw();
             */
        }

        /// <summary>
        /// Sets the highlight colour for the selected line.
        /// </summary>
        /// <param name="col">The color the line should be drawn in</param>
        internal void SetLineColor(Color col)
        {
            m_LineColor = col;
        }

        void MoveNext()
        {
            IntersectForm dial = (this.ParentForm as IntersectForm);
            if (dial!=null)
                dial.AdvanceToNextPage();
        }

        private void wantSplitButton_Click(object sender, EventArgs e)
        {
            if (m_Line!=null && m_Line.EntityType==null)
            {
                MessageBox.Show("Circle construction lines cannot be split");
                return;
            }

            yesCheckBox.Checked = true;
            noCheckBox.Checked = false;
            m_WantSplit = 1;
            MoveNext();
        }

        private void noSplitButton_Click(object sender, EventArgs e)
        {
            yesCheckBox.Checked = false;
            noCheckBox.Checked = true;
            m_WantSplit = -1;
            MoveNext();
        }

        /// <summary>
        /// Highlights a line.
        /// </summary>
        /// <param name="line">The line to highlight</param>
        void Highlight(LineFeature line)
        {
            ISpatialDisplay display = ActiveDisplay;

            // Create thick pen (we want a line that is 1mm wide, corresponding
            // to the pick aperture).
            DrawStyle style = new DrawStyle(m_LineColor);
            double scale = display.MapScale;
            float pxwid = display.LengthToDisplay(scale*0.001);
            style.Pen.Width = pxwid;

            // Do the draw
            line.Render(display, style);
        }

        /// <summary>
        /// Un-Highlight a line.
        /// </summary>
        /// <param name="line">The line to un-highlight (no longer used)</param>
        void UnHighlight(LineFeature line)
        {
            ErasePainting();
        }

        /// <summary>
        /// Indicates that any painting previously done by a command should be erased. This
        /// tells the command's active display that it should revert the display buffer to
        /// the way it was at the end of the last draw from the map model. Given that a command
        /// supports painting, it's <c>Paint</c> method will be called during idle time.
        /// </summary>
        void ErasePainting()
        {
            ActiveDisplay.RestoreLastDraw();
        }

        ISpatialDisplay ActiveDisplay
        {
            get { return EditingController.Current.ActiveDisplay; }
        }

        /// <summary>
        /// Initialize for an update (or recall)
        /// </summary>
        /// <param name="op">The edit that is being updated or recalled</param>
        /// <param name="lineNum">The sequence number of the distance involved (relevant only for
        /// a <see cref="IntersectTwoLinesOperation"/>)</param>
        internal void ShowUpdate(IntersectOperation op, int lineNum)
        {
            // Return if no update object (and no recall op).
            if (op==null)
                return;

            // Populate the dialog, depending on what sort of operation we have.

            if (op.EditId == EditingActionId.LineIntersect)
            {
                /*
		CeIntersectLine* pOper = dynamic_cast<CeIntersectLine*>(pop);

		if ( linenum==1 )
			this->Show( pOper->GetpArc1()
					  , pOper->IsSplit1() );
		else
			this->Show( pOper->GetpArc2()
					  , pOper->IsSplit2() );
                 */
            }
            else if (op.EditId == EditingActionId.DirLineIntersect)
            {
                /*
		CeIntersectDirLine* pOper = dynamic_cast<CeIntersectDirLine*>(pop);
		this->Show ( pOper->GetpArc()
				   , pOper->IsSplit() );
                 */
            }
            else
            {
                MessageBox.Show("GetLineControl.ShowUpdate - Unexpected editing operation");
            }
        }

        /// <summary>
        /// Displays info for a specific line.
        /// </summary>
        /// <param name="line">The selected line.</param>
        /// <param name="wantsplit">True if the line should be split.</param>
        void ShowLine(LineFeature line, bool wantsplit)
        {
        	// Define the selected line and whether it should be split.
            OnSelectLine(line);

            if (wantsplit)
            {
		        m_WantSplit = 1;
                yesCheckBox.Checked = true;
                noCheckBox.Checked = false;
	        }
	        else
            {
		        m_WantSplit = -1;
                yesCheckBox.Checked = false;
                noCheckBox.Checked = true;
	        }
        }       
    }
}
