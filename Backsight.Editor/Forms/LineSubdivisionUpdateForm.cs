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
        LineFeature m_SelectedLine;

        /// <summary>
        /// The line subdivision involved.
        /// </summary>
        LineSubdivisionOperation m_pop;

        /// <summary>
        /// The distances for the primary face
        /// </summary>
        AnnotatedDistance[] m_Face1;

        /// <summary>
        /// The distances for the alternate face (if there is one)
        /// </summary>
        AnnotatedDistance[] m_Face2;

        /// <summary>
        /// The currently listed face (m_Face1 or m_Face2). Should never be null.
        /// </summary>
        AnnotatedDistance[] m_CurrentFace;

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
            this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        /// <summary>
        /// Obtains the distances for one subdivision face (along with a note as
        /// to whether the annotation has been flipped or not).
        /// </summary>
        /// <param name="face">The face of interest (may be null)</param>
        /// <returns>The distances along the face (including information about
        /// placement of annotation). Null if the supplied face is null.</returns>
        AnnotatedDistance[] GetDistances(LineSubdivisionFace face)
        {
            if (face == null)
                return null;

            MeasuredLineFeature[] sections = face.Sections;
            AnnotatedDistance[] result = new AnnotatedDistance[sections.Length];

            for (int i = 0; i < result.Length; i++)
            {
                MeasuredLineFeature mf = sections[i];

                // Don't hold the ACTUAL flip status, just record whether it has been changed
                result[i] = new AnnotatedDistance(mf.ObservedLength, false);
            }

            return result;
        }

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

            // Grab something we throw away if the user decides to cancel
            m_Face1 = GetDistances(m_pop.PrimaryFace);
            m_Face2 = GetDistances(m_pop.AlternateFace);

            // If we have two faces, the "New Face" button means
            // you want to switch to the other face.
            if (m_pop.AlternateFace != null)
                newFaceButton.Text = "&Other Face";

            // If we have a selected line section that is on the second face,
            // make that the initial face.
            m_CurrentFace = m_Face1;
            if (m_SelectedLine != null && m_Face2 != null)
            {
                if (m_pop.AlternateFace.HasSection(m_SelectedLine))
                    m_CurrentFace = m_Face2;
            }

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
            return GetLine(listBox.SelectedIndex);
        }

        /// <summary>
        /// Obtains the line section that corresponds to one of the currently
        /// displayed distances.
        /// </summary>
        /// <param name="listIndex">The array index of the displayed line section</param>
        /// <returns>The corresponding line</returns>
        LineFeature GetLine(int listIndex)
        {
            if (listIndex < 0)
                return null;

            if (m_CurrentFace == m_Face1)
                return m_pop.PrimaryFace.Sections[listIndex].Line;

            // May have just added new face
            if (m_CurrentFace == m_Face2 && m_pop.AlternateFace != null)
                return m_pop.AlternateFace.Sections[listIndex].Line;

            return null;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            // Get the selected distance.
            AnnotatedDistance ad = (listBox.SelectedItem as AnnotatedDistance);
            if (ad == null)
            {
                MessageBox.Show("You must first select a distance from the list.");
                return;
            }

            m_SelectedLine = GetLine(listBox.SelectedIndex);

            using (DistForm dist = new DistForm(ad, false))
            {
                if (dist.ShowDialog() == DialogResult.OK)
                {
                    // Change the displayed distance

                    m_CurrentFace[listBox.SelectedIndex] = new AnnotatedDistance(dist.Distance, ad.IsFlipped);
                    RefreshList();
                }
            }
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

            // If we've specified the values for a new face (but it hasn't yet made
            // it into the edit, draw the annotations and extra points).
            if (m_Face2 != null && m_pop.AlternateFace == null)
            {
                LineFeature line = m_pop.Parent;
                double[] lens = LineSubdivisionFace.GetAdjustedLengths(line, m_Face2);
                double totlen = 0.0;

                foreach (double len in lens)
                {
                    totlen += len;
                    IPosition p;
                    if (line.LineGeometry.GetPosition(new Length(totlen), out p))
                        style.Render(display, p);
                }
            }

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
            listBox.Items.AddRange(m_CurrentFace);

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

            AnnotatedDistance ad = m_CurrentFace[index];
            ad.ToggleIsFlipped();

            // Change the line too, so that it will highlight with the annotation
            // on the other side. Remember that this will need to be switched back
            // when this dialog closes.
            LineFeature line = GetLine(index);
            line.IsLineAnnotationFlipped = !line.IsLineAnnotationFlipped;

            // Ensure stuff gets redrawn
            m_UpdCmd.ErasePainting();
        }

        private void newFaceButton_Click(object sender, EventArgs e)
        {
            // If we previously highlighted something, draw it
            // normally (since it cannot exist as part of any other
            // face).
            if (m_SelectedLine != null)
                m_SelectedLine = null;

            // If a second face doesn't already exist, get the
            // user to specify the distances.

            if (m_Face2 == null)
            {
                try
                {
                    this.WindowState = FormWindowState.Minimized;

                    // Get the distance observations

                    using (LegForm dial = new LegForm(GetObservedLength()))
                    {
                        if (dial.ShowDialog() != DialogResult.OK)
                            return;

                        // Must be at least two distances
                        Distance[] dists = dial.Distances;
                        if (dists == null || dists.Length < 2)
                        {
                            MessageBox.Show("The new face must have at least two spans");
                            return;
                        }

                        // Remember the entered distances for the new face.
                        m_Face2 = new AnnotatedDistance[dists.Length];
                        for (int i = 0; i < dists.Length; i++)
                            m_Face2[i] = new AnnotatedDistance(dists[i], true);

                        newFaceButton.Text = "&Other Face";
                    }
                }

                finally
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }

            // Switch to the other face (possibly the one just added)

            if (m_CurrentFace == m_Face1)
                m_CurrentFace = m_Face2;
            else
                m_CurrentFace = m_Face1;

            RefreshList();
        }

        /// <summary>
        /// Sums the observed lengths for the displayed sections.
        /// </summary>
        /// <returns>The total observed length, in meters.</returns>
        double GetObservedLength()
        {
            if (m_CurrentFace == null)
                return 0.0;

            double length = 0.0;

            foreach (AnnotatedDistance ad in m_CurrentFace)
                length += ad.Meters;

            return length;
        }

        /// <summary>
        /// Obtains update items for each revised section.
        /// </summary>
        /// <returns>The items representing the change.</returns>
        internal UpdateItemCollection GetUpdateItems()
        {
            UpdateItemCollection result = new UpdateItemCollection();
            AddUpdateItems(result, m_pop.PrimaryFace, m_Face1);

            // If an alternate face previously exists, handle it like the primary face.
            // Otherwise record the new face.

            if (m_Face2 != null)
            {
                if (m_pop.AlternateFace == null)
                {
                    // Record new face
                }
                else
                {
                    AddUpdateItems(result, m_pop.AlternateFace, m_Face2);
                }
            }

            return result;
        }

        /// <summary>
        /// Adds update items for a previously created face.
        /// </summary>
        /// <param name="uc">The item collection to append to</param>
        /// <param name="face">The previously created face (not null)</param>
        /// <param name="dists">The distances specified by the user (not null)</param>
        void AddUpdateItems(UpdateItemCollection uc, LineSubdivisionFace face, AnnotatedDistance[] dists)
        {
            Debug.Assert(face != null);
            MeasuredLineFeature[] sections = face.Sections;
            Debug.Assert(sections.Length == dists.Length);

            for (int i = 0; i < sections.Length; i++)
            {
                MeasuredLineFeature originalSection = sections[i];
                LineFeature line = originalSection.Line;
                Distance originalLength = originalSection.ObservedLength;

                AnnotatedDistance revisedSection = dists[i];

                if (originalLength.Equals(revisedSection) == false)
                    uc.AddObservation<Distance>(line.DataId, originalLength, revisedSection);

                if (revisedSection.IsFlipped)
                    uc.AddItem<bool>("A" + line.DataId, !line.IsLineAnnotationFlipped, line.IsLineAnnotationFlipped);
            }
        }

        private void LineSubdivisionUpdateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure any flipped annotations have been temporarily flipped back. To set the
            // status for good, the appropriate update items must be returned by GetUpdateItems.

            CloseFace(m_Face1, m_pop.PrimaryFace);
            CloseFace(m_Face2, m_pop.AlternateFace);
        }

        void CloseFace(AnnotatedDistance[] ads, LineSubdivisionFace face)
        {
            if (face == null)
                return;

            MeasuredLineFeature[] sections = face.Sections;
            Debug.Assert(sections.Length == ads.Length);

            for (int i = 0; i < sections.Length; i++)
            {
                if (ads[i].IsFlipped)
                {
                    LineFeature line = sections[i].Line;
                    line.IsLineAnnotationFlipped = !line.IsLineAnnotationFlipped;
                }
            }
        }
    }
}
