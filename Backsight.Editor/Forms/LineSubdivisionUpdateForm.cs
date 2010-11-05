// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
using System.Diagnostics;
using System.Drawing;

using Backsight.Editor.UI;
using Backsight.Editor.Operations;
using Backsight.Editor.Observations;
using Backsight.Forms;


namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdUpdateSub"/>
    /// <summary>
    /// Dialog for specifying changes to a <see cref="LineSubdivisionOperation"/>.
    /// </summary>
    partial class LineSubdivisionUpdateForm : Form
    {
        #region Class data

        /// <summary>
        /// The command that's running things.
        /// </summary>
        readonly UpdateUI m_UpdCmd;

        /// <summary>
        /// The line that is currently selected.
        /// </summary>
        LineFeature m_SelectedLine; // was m_pSelArc

        /// <summary>
        /// The line subdivision involved.
        /// </summary>
        LineSubdivisionOperation m_pop;

        /// <summary>
        /// The displayed distances
        /// </summary>
        MeasuredLineFeature[] m_Dists;

        /// <summary>
        /// Indicators for sections where the line annotation has been flipped (possibly
        /// back to the standard side). All elements in this array are initially <c>false</c>
        /// (even if certain annotations were previously flipped). Has a 1:1 relationship
        /// with <c>m_Dists</c>.
        /// </summary>
        bool[] m_Flips;

        /*
	INT4			m_FaceIndex1;	// The index of the first face
	INT4			m_FaceIndex2;	// The index of the 2nd face (-1
									// if there is only one face)
	INT4			m_CurIndex;		// The index of the face that
									// is currently displayed
         */

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionUpdateForm"/> class.
        /// </summary>
        /// <param name="up">The command that's running things.</param>
        internal LineSubdivisionUpdateForm(UpdateUI up)
        {
            InitializeComponent();

            m_UpdCmd = up;
            m_SelectedLine = null;
            m_pop = null;
            m_Dists = null;
            m_Flips = null;

            this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        private void LineSubdivisionUpdateForm_Shown(object sender, EventArgs e)
        {
            // Get the object that was selected for update.
            Feature feat = m_UpdCmd.GetUpdate();
            if (feat == null)
                throw new InvalidOperationException("Unexpected update object");

            // If it's a line, remember it.
            m_SelectedLine = (feat as LineFeature);
            m_pop = (feat.Creator as LineSubdivisionOperation);
            Debug.Assert(m_pop != null);

            // Work with a copy of the distances
            MeasuredLineFeature[] sections = m_pop.Sections;
            m_Dists = new MeasuredLineFeature[sections.Length];
            m_Flips = new bool[sections.Length];
            for (int i=0; i<sections.Length; i++)
            {
                MeasuredLineFeature mf = sections[i];
                m_Dists[i] = new MeasuredLineFeature(mf.Line, mf.ObservedLength);
                m_Flips[i] = false;
            }

            /*
	// Get the number of editable faces on the current
	// editing theme
	const CeTheme& theme = GetActiveTheme();
	m_FaceIndex1 = m_pop->GetFaceIndex(theme);

	if ( m_FaceIndex1 < 0 )
	{
		ShowMessage("CdUpdateSub::OnInitDialog\nCannot locate primary face");
		EndDialog(FALSE);
		return TRUE;
	}

	m_FaceIndex2 = m_pop->GetFaceIndex(theme,m_FaceIndex1+1);

	// If we have two faces, the "New Face" button means
	// you want to switch to the other face.
	if ( m_FaceIndex2 != -1 )
		GetDlgItem(IDC_NEWFACE)->SetWindowText("&Other Face");

	// If we have a selected arc that is on the second face,
	// make that the initial face
	INT4 firstFaceIndex = m_FaceIndex1;
	if ( m_pSelArc && m_FaceIndex2 > 0 )
	{
		const CeObjectList* const pS = m_pop->GetSectionList(m_FaceIndex2);
		if ( pS->IsReferredTo(m_pSelArc) ) firstFaceIndex = m_FaceIndex2;
	}

	// Get an array of the distances we'll be updating
	GetFace(firstFaceIndex);

             */

            // Disable the option to flip annotation if annotation is currently invisible
            if (!EditingController.Current.AreLineAnnotationsDrawn)
                flipDistButton.Enabled = false;

            // Reload the list and repaint
            RefreshList();
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            updateButton_Click(sender, e);
        }

        private void listBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Get the currently selected line (if any).
            m_SelectedLine = GetSelectedLine();

            // Ensure stuff gets repainted in idle time
            m_UpdCmd.ErasePainting();
        }

        LineFeature GetSelectedLine()
        {
            MeasuredLineFeature mf = (listBox.SelectedItem as MeasuredLineFeature);
            return (mf == null ? null : mf.Line);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            // Get the selected distance.
            MeasuredLineFeature mf = (listBox.SelectedItem as MeasuredLineFeature);
            if (mf == null)
            {
                MessageBox.Show("You must first select a distance from the list.");
                return;
            }

            m_SelectedLine = mf.Line;

            using (DistForm dist = new DistForm(mf.ObservedLength, false))
            {
                if (dist.ShowDialog() == DialogResult.OK)
                {
                    // Change the displayed distance
                    mf.ObservedLength = dist.Distance;
                    RefreshList();
                }
            }

            /*
	CdDist dist(&m_Dists[index],FALSE);

	if ( dist.DoModal()==IDOK ) {

		CeDistance* pDist = m_pop->GetpDistance(m_pSelArc);
		*pDist = dist.GetDistance();

		// As well as our private copy.
		m_Dists[index] = dist.GetDistance();

		// Pretty things up.
		m_pop->GetpParent()->Erase();
		//Paint();
		RefreshList();
	}
             */
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            m_UpdCmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            m_UpdCmd.DialFinish(this);
        }

        /// <summary>
        /// Dialog-specific painting.
        /// </summary>
        internal void Draw()
        {
            // Draw the features created by the op in magenta.
            ISpatialDisplay display = m_UpdCmd.ActiveDisplay;
            IDrawStyle style = m_UpdCmd.Controller.Style(Color.Magenta);
            style.IsFixed = true;
            m_pop.Render(display, style, true);

            // Highlight the currently selected section (if any).
            if (m_SelectedLine != null)
                m_SelectedLine.Render(display, new HighlightStyle());

            // Draw the points (except the last one, which should
            // coincide with the end of the parent line).
            // TODO: Ideally, the original points should be drawn in gray, the
            // adjusted new points in magenta. This was commented out in CEdit,
            // so I'm not going to reintroduce it just now.

            /*
	            // Get a list of positions on the arc we are subdividing.
	            CeArc* pArc = m_pop->GetpParent();
	            CeVertex* pos = new CeVertex[m_NumDist];
	            pArc->GetPositions(m_NumDist,m_Dists,TRUE,pos);

    		    for ( UINT4 i=0; i<(m_NumDist-1); i++ ) {
    			    pDraw->Draw(pos[i],COL_MAGENTA);
    		    }
             */
        }

        void RefreshList()
        {
            // Highlight currently selected line (if any).
            Draw();

            // List the observed distances, relating each distance
            // to the corresponding line.

            listBox.Items.Clear();
            listBox.Items.AddRange(m_Dists);

            // Always leave the focus in the list of distances.
            listBox.Focus();
        }

        /*

void CdUpdateSub::Refresh ( void ) {

	// There may be no arc sections if this is a new face.

	CeFeature** pFeats = new CeFeature*[m_NumDist];
	m_pop->GetpArcs(m_CurIndex,pFeats);
	CListBox* pList = (CListBox*)GetDlgItem(IDC_LIST);
	pList->ResetContent();

	for ( UINT4 i=0; i<m_NumDist; i++ ) {
		INT4 index = pList->AddString(m_Dists[i].Format());
		pList->SetItemDataPtr(index,pFeats[i]);
		if ( pFeats[i] == m_pSelArc ) pList->SetCurSel(index);
	}

	delete [] pFeats;

	// Always leave the focus in the list of distances.
	pList->SetFocus();

} // end of Refresh
         */

        private void flipDistButton_Click(object sender, EventArgs e)
        {
            // Get the selected distance.
            int index = listBox.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("You must first select a distance from the list.");
                return;
            }

            MeasuredLineFeature mf = m_Dists[index];
            LineFeature line = mf.Line;

            // Flip the switch in the line feature so that it will redraw differently, and
            // remember whether a change has occurred (it's possible the user has flipped back
            // to the original side).
            line.IsLineAnnotationFlipped = !line.IsLineAnnotationFlipped;
            m_Flips[index] = !m_Flips[index];

            // Ensure stuff gets redrawn
            m_UpdCmd.ErasePainting();
        }

        private void newFaceButton_Click(object sender, EventArgs e)
        {
            /*
	// If we previously highlighted something, draw it
	// normally (since it cannot exist as part of any other
	// face).

	if ( m_pSelArc )
	{
		//m_pSelArc->UnHighlight();
		UnHighlightArc();
		m_pSelArc = 0;
	}
	
	// If a second face doesn't already exist, get the
	// user to specify the distances and define them as
	// part of the 
	if ( m_FaceIndex2 < 0 )
	{
		// Get the distance observations

		CdLeg dial(GetObservedLength());
		if ( dial.DoModal() != IDOK ) return;

		// Must be at least two distances

		UINT4 nDist = dial.GetNumDist();
		if ( nDist < 2 )
		{
			AfxMessageBox("The new face must have at least two spans");
			return;
		}

		// Create the new face

		const CeDistance* pDist = dial.GetDists();
		m_FaceIndex2 = m_pop->AddFace(nDist,pDist);
		if ( m_FaceIndex2 < 0 ) return;

		GetDlgItem(IDC_NEWFACE)->SetWindowText("&Other Face");
	}

	if ( m_CurIndex == m_FaceIndex1 )
		GetFace(m_FaceIndex2);
	else
		GetFace(m_FaceIndex1);

	RefreshList();
             */
        }

        /*
void CdUpdateSub::GetFace ( const INT4 faceIndex )
{
	if ( m_CurIndex == faceIndex ) return;

	delete [] m_Dists;

	m_CurIndex = faceIndex;
	m_NumDist = m_pop->GetCount(m_CurIndex);
	m_Dists = new CeDistance[m_NumDist];
	m_pop->GetSpans(m_CurIndex,m_Dists);
}

FLOAT8 CdUpdateSub::GetObservedLength ( void ) const
{
	if ( m_Dists == 0 ) return 0.0;

	FLOAT8 length = 0;

	for ( UINT4 i=0; i<m_NumDist; i++ )
	{
		length += m_Dists[i].GetMetric();
	}

	return length;
}
         */

        /// <summary>
        /// Obtains update items for each revised section.
        /// </summary>
        /// <returns>The items representing the change.</returns>
        internal UpdateItemCollection GetUpdateItems()
        {
            MeasuredLineFeature[] sections = m_pop.Sections;
            Debug.Assert(sections.Length == m_Dists.Length);

            UpdateItemCollection result = new UpdateItemCollection();

            for (int i = 0; i < sections.Length; i++)
            {
                MeasuredLineFeature originalSection = sections[i];
                MeasuredLineFeature revisedSection = m_Dists[i];
                LineFeature line = originalSection.Line;
                Debug.Assert(line == revisedSection.Line);

                Distance originalLength = originalSection.ObservedLength;
                Distance revisedLength = revisedSection.ObservedLength;
                if (originalSection.Equals(revisedLength) == false)
                    result.AddObservation<Distance>(line.DataId, originalLength, revisedLength);

                if (m_Flips[i])
                    result.AddItem<bool>("A"+line.DataId, line.IsLineAnnotationFlipped, !line.IsLineAnnotationFlipped);
            }

            return result;
        }

        private void LineSubdivisionUpdateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure any flipped annotations have been temporarily flipped back. To set the
            // status for good, the appropriate update items must be returned by GetUpdateItems.

            for (int i=0; i<m_Flips.Length; i++)
            {
                if (m_Flips[i])
                {
                    LineFeature line = m_Dists[i].Line;
                    line.IsLineAnnotationFlipped = !line.IsLineAnnotationFlipped;
                }
            }
        }
    }
}
