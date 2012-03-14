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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Backsight.Editor.Observations;
using Backsight.Editor.Operations;
using Backsight.Editor.UI;
using Backsight.Forms;


namespace Backsight.Editor.Forms
{
    partial class UpdatePathForm : Form
    {
        #region Class data

        /// <summary>
        /// The update command that's driving things (not null).
        /// </summary>
        readonly UpdateUI m_UpdCmd;

        /// <summary>
        /// The connection path involved.
        /// </summary>
        readonly PathOperation m_pop;

        /// <summary>
        /// The total number of legs.
        /// </summary>
        //int m_NumLeg; -- use NumLeg property

        /// <summary>
        /// Index of the current leg.
        /// </summary>
        int m_CurLeg;

        /// <summary>
        /// Working copy of the legs in the connection path.
        /// </summary>
        readonly List<Leg> m_Legs;

        /// <summary>
        /// The line that is currently selected.
        /// </summary>
        LineFeature m_SelectedLine; // was m_pSelArc

        // Relating to the adjusted path ...

        /// <summary>
        /// Current rotation.
        /// </summary>
        double m_Rotation;

        /// <summary>
        /// Scale factor
        /// </summary>
        double m_ScaleFac;

        /// <summary>
        /// The current precision
        /// </summary>
	    uint m_Precision;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePathForm"/> class.
        /// </summary>
        /// <param name="update">The update command that's driving things (not null).</param>
        /// <exception cref="ArgumentNullException">If the supplied update command is null.</exception>
        internal UpdatePathForm(UpdateUI update)
        {
            InitializeComponent();

            if (update == null)
                throw new ArgumentNullException();

            m_UpdCmd = update;
	        m_CurLeg = -1;
	        m_pop = null;
	        m_SelectedLine = null;
	        m_ScaleFac = 0.0;
	        m_Rotation = 0.0;
	        m_Precision = 0;

            // Get the object that was selected for update.
            m_pop = (m_UpdCmd.GetOp() as PathOperation);
            if (m_pop == null)
                throw new ArgumentException("Cannot obtain original connection path for update");

            // Get a working copy of the connection path legs
            // TODO - This may not be suitable in a situation where staggered legs have been created
            Leg[] legs = PathParser.CreateLegs(m_pop.EntryString, m_pop.EntryUnit);
            m_Legs = new List<Leg>(legs);
        }

        #endregion

        /// <summary>
        /// The number of legs in the (possibly revised) connection path.
        /// </summary>
        int NumLeg
        {
            get { return m_Legs.Count; }
        }

        private void UpdatePathForm_Shown(object sender, EventArgs e)
        {
            // Initialize radio buttons.
            insBeforeRadioButton.Checked = true;
            brkBeforeRadioButton.Checked = true;

            // Display the precision of the connection path.
            ShowPrecision();

            // A feature on the connection path should have been selected - determine which leg it's part of
            Leg leg = null;
            Feature f = m_UpdCmd.GetUpdate();
            if (f != null)
            {
                leg = m_pop.GetLeg(f);
                m_CurLeg = m_pop.GetLegIndex(leg);
            }

            if (m_CurLeg < 0)
                m_CurLeg = 0;

            if (leg == null)
                Refresh(-1);
            else
                Refresh(leg.GetIndex(f));
        }

        private void angleButton_Click(object sender, EventArgs e)
        {
            StraightLeg leg = (CurrentLeg as StraightLeg);
            if (leg == null)
                return;

            using (AngleForm dial = new AngleForm(leg))
            {
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    if (dial.IsDeflection)
                        leg.SetDeflection(dial.SignedAngle);
                    else
                        leg.StartAngle = dial.SignedAngle;

                    ShowPrecision();
                    m_UpdCmd.ErasePainting();
                }
            }
        }

        private void breakButton_Click(object sender, EventArgs e)
        {
            StraightLeg leg = (CurrentLeg as StraightLeg);
            if (leg == null)
                return;

            // You can't break a staggered leg (this should have already been trapped by disabling the button).
            if (leg.IsStaggered)
            {
                MessageBox.Show("You cannot break a staggered leg.");
                return;
            }
            /*

	// Get the address of the selected distance.
    CeDistance* pDist = GetSel();
	if ( !pDist ) {
		AfxMessageBox("You must first select a distance from the list.");
		return;
	}

	// Are we breaking before or after the currently selected distance?
	CButton* pButton = (CButton*)GetDlgItem(IDC_BRK_BEFORE);
	const LOGICAL isBefore = (pButton->GetCheck()==1);

	// You can't break at the very beginning or end of the leg.
	INT4 index = pLeg->GetIndex(*pDist);

	if ( isBefore && index==0 ) {
		AfxMessageBox("You can't break at the very beginning of the leg.");
		return;
	}

	if ( !isBefore && (index+1)==pLeg->GetCount() ) {
		AfxMessageBox("You can't break at the very end of the leg.");
		return;
	}

	// Break the leg.
	if ( !isBefore ) index++;
	CeLeg* pNewLeg = pLeg->Break(*m_pop,index);
	if ( !pNewLeg ) return;

	// Make the new leg the current one, and select
	// the very first distance.
	m_NumLeg++;
	m_CurLeg++;
	Refresh(0);
             */
        }

        private void curveButton_Click(object sender, EventArgs e)
        {
            CircularLeg leg = (CurrentLeg as CircularLeg);
            if (leg == null)
                return;

            if (leg.IsCulDeSac)
            {
                using (CulDeSacForm dial = new CulDeSacForm(leg))
                {
                    if (dial.ShowDialog() == DialogResult.OK)
                    {
                        leg.SetCentralAngle(dial.CentralAngle);
                        leg.SetRadius(dial.Radius);
                        leg.IsClockwise = dial.IsClockwise;
                        ShowPrecision();
                        m_UpdCmd.ErasePainting();
                    }
                }
            }
            else
            {
                using (ArcForm dial = new ArcForm(leg))
                {
                    if (dial.ShowDialog() == DialogResult.OK)
                    {
                        leg.SetEntryAngle(dial.EntryAngle);
                        leg.SetExitAngle(dial.ExitAngle);
                        leg.SetRadius(dial.Radius);
                        leg.IsClockwise = dial.IsClockwise;
                        ShowPrecision();
                        m_UpdCmd.ErasePainting();
                    }
                }
            }
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            Leg leg = CurrentLeg;
            if (leg == null)
                return;
            /*
	// Get the address of the selected distance.
	CeDistance* pDist = GetSel();
	if ( !pDist ) {
		AfxMessageBox("You must first select a distance from the list.");
		return;
	}

	// Are we inserting before or after the currently selected distance?
	CButton* pButton = (CButton*)GetDlgItem(IDC_INS_BEFORE);
	const LOGICAL isBefore = (pButton->GetCheck()==1);

	// Display dialog for a new distance.
	CdDist dial(0,FALSE);
	if ( dial.DoModal()==IDOK ) {

		// Erase the path.
		m_pop->Erase(m_Rotation,m_ScaleFac);

		// Insert the new distance into the path.
		//const LOGICAL wantLine = dial.WantLine();
		const LOGICAL wantLine = TRUE;
		const CeDistance& newdist = dial.GetDistance();
		INT4 index = pLeg->Insert(newdist,*pDist,isBefore,wantLine);

		// Show the precision and draw the path.
		ShowPrecision();
		Paint();

		// Fix up the list of distances in this dialog.
		Refresh(index);

	}
             */
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            m_CurLeg++;
            if (m_CurLeg < NumLeg)
                Refresh(0);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            m_CurLeg--;
            if (m_CurLeg >= 0)
                Refresh(-1);
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

        void Paint()
        {
            ISpatialDisplay display = m_UpdCmd.ActiveDisplay;
            Render(display);
        }

        /// <summary>
        /// Does any painting that this dialog does.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal void Render(ISpatialDisplay display)
        {
            // Draw the current path (in magenta).
            PathInfo p = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, m_Legs.ToArray());
            p.Render(display);

            // Highlight the currently selected line.
            if (m_SelectedLine != null)
                m_SelectedLine.Render(display, new HighlightStyle());
        }

        void Refresh(int index)
        {
            if (m_CurLeg < 0 || m_CurLeg >= NumLeg)
                return;

            // Erase anything that is currently highlighted.
            //if (m_pSelArc) m_pSelArc->UnHighlight();

            this.Text = String.Format("Leg {0} of {1}", m_CurLeg + 1, NumLeg);

            // Enable the back/next buttons, depending on what leg we're on.
            previousButton.Enabled = (m_CurLeg > 0);
            nextButton.Enabled = (m_CurLeg + 1 < NumLeg);

            // Get the corresponding leg.
            Leg leg = CurrentLeg;

            // If it's a curve, enable the circular arc button and disable the angle & break buttons.
            if (leg is CircularLeg)
            {
                curveButton.Enabled = true;
                angleButton.Enabled = false;
                breakButton.Enabled = false;
            }
            else
            {
                curveButton.Enabled = false;

                // You can break a straight leg so long as it has not been staggered.
                breakButton.Enabled = (leg.IsStaggered == false);

                // Enable the angle button so long as the preceding leg exists, and is also a straight leg.
                bool isPrevStraight = false;
                if (m_CurLeg > 0 && m_Legs[m_CurLeg - 1] is StraightLeg)
                    isPrevStraight = true;

                angleButton.Enabled = isPrevStraight;
            }

            // You can't create a new face if the leg is already staggered.
            newFaceButton.Enabled = (leg.IsStaggered == false);

            /*
	CeExtraLeg* pExtra = dynamic_cast<CeExtraLeg*>(pLeg);
	if ( pExtra )
		GetDlgItem(IDC_SECOND_FACE)->ShowWindow(SW_SHOW);
	else
		GetDlgItem(IDC_SECOND_FACE)->ShowWindow(SW_HIDE);
             */

            // List the observed distances for the leg.
            distancesListBox.Items.Clear();
            if (leg.Count == 0)
            {
                distancesListBox.Items.Add("see central angle");
            }
            else
            {
                List<Distance> dists = new List<Distance>();
                leg.GetSpans(dists);
                distancesListBox.Items.AddRange(dists.ToArray());
            }

            /*
	// Select the first (or last) item in the list.
	if ( !nList ) {
		pList->SetCurSel(-1);
		OnSelchangeList();
		return;
	}

	if ( index < 0 )
		pList->SetCurSel(pList->GetCount()-1);
	else if ( index < pList->GetCount() )
		pList->SetCurSel(index);

	OnSelchangeList();
             */

            // Always leave the focus in the list of distances.
            distancesListBox.Focus();
        }

        void ShowPrecision()
        {
            PathInfo p = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, m_Legs.ToArray());

            // If it's REALLY good, show 1 billion.
            double prec = p.Precision;
            uint iPrec = (prec < Constants.TINY ? 1000000000 : (uint)prec);
            precisionLabel.Text = "Precision " + iPrec;
        }

        private void distancesListBox_DoubleClick(object sender, EventArgs e)
        {
            updateButton_Click(sender, e);
        }

        Distance GetSel()
        {
            return (distancesListBox.SelectedItem as Distance);
        }

        private void distancesListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_CurLeg < 0 || m_CurLeg >= NumLeg)
                return;

            // Ensure stuff gets repainted in idle time
            m_UpdCmd.ErasePainting();
        }

        /*
void CdUpdatePath::OnSelchangeList() 
{
	// What leg are we on?
	CeLeg* pLeg = m_pop->GetpLeg(m_CurLeg);
	assert(pLeg);

	// If it doesn't have any observed distances, it must
	// be defined via the central angle, so just draw the
	// arc.
	if ( pLeg->GetCount() == 0 ) {
		m_pSelArc = dynamic_cast<CeArc*>(pLeg->GetpFeature(0));
		if ( m_pSelArc ) m_pSelArc->Highlight();
		return;
	}

	// Get the currently selected distance.
	CeDistance* pDist = GetSel();
	if ( !pDist ) return;

	// Use the index of the span to get the corresponding
	// feature and draw it. If there isn't one, draw a
	// dotted line instead.
	INT4 index = pLeg->GetIndex(*pDist);
	if ( index<0 ) return;

	m_pSelArc = dynamic_cast<CeArc*>(pLeg->GetpFeature(index));
	if ( m_pSelArc ) m_pSelArc->Highlight();
}
         */

        private void updateButton_Click(object sender, EventArgs e)
        {
            // Get the selected distance.
            Distance d = (distancesListBox.SelectedItem as Distance);
            if (d == null)
            {
                MessageBox.Show("You must first select a distance from the list.");
                return;
            }

            Distance dCopy = new Distance(d);

            using (DistForm dist = new DistForm(dCopy, false))
            {
                if (dist.ShowDialog(this) == DialogResult.OK)
                {
                    // Change the displayed distance
                    /*
                     * from LineSubdivisionUpdateForm
                    m_CurrentFace.ObservedLengths[listBox.SelectedIndex] = dist.Distance;
                    m_CurrentFace.Sections[listBox.SelectedIndex].ObservedLength = dist.Distance;
                    m_UpdCmd.ErasePainting();
                    RefreshList();
                     */

                    /*was
		m_pop->Erase(m_Rotation,m_ScaleFac);
		*pDist = dist.GetDistance();
		CListBox* pList = (CListBox*)GetDlgItem(IDC_LIST);
		INT4 index = pList->GetCurSel();
		ShowPrecision();
		Paint();
		Refresh(index);
                     */
                }
            }
        }

        private void flipDistButton_Click(object sender, EventArgs e)
        {
            Leg leg = CurrentLeg;
            if (leg == null)
                return;
            /*
	CeObjectList feats;
	pLeg->GetFeatures(*m_pop,feats);

	CeListIter loop(&feats);
	CeFeature* pFeat;

	for ( pFeat = (CeFeature*)loop.GetHead();
		  pFeat;
		  pFeat = (CeFeature*)loop.GetNext() ) {

		CeArc* pArc = dynamic_cast<CeArc*>(pFeat);
		if ( pArc ) {
			CeLine* pLine = pArc->GetpLine();
			pLine->SetDistFlipped(!pLine->IsDistFlipped());
		}
	}

	GetpDraw()->InvalidateRect(0);
             */
        }

        /// <summary>
        /// The current leg (null if <see cref="m_CurLeg"/> refers to an invalid leg)
        /// </summary>
        Leg CurrentLeg
        {
            get
            {
                if (m_CurLeg < 0 || m_CurLeg >= m_Legs.Count)
                    return null;
                else
                    return m_Legs[m_CurLeg];
            }
        }

        private void newFaceButton_Click(object sender, EventArgs e)
        {
            Leg leg = CurrentLeg;
            if (leg == null)
                return;

            // Get the observed length of the leg (in meters on the ground).
            double len = leg.GetTotal();

            // Present a data entry dialog for the new face.
            using (LegForm dial = new LegForm(len))
            {
                if (dial.ShowDialog() != DialogResult.OK)
                    return;

                // Create the new face and insert it after the current leg.
            }
            /*
	UINT4 nDist = dial.GetNumDist();
	CeDistance* dists = dial.GetDists();
	CeLeg* pNewLeg = m_pop->InsertFace(pLeg,nDist,dists);
	if ( !pNewLeg ) return;


	// Get the features that were created and draw them (this
	// is so that distance annotations will show, so that the
	// user can flip them if necessary).

	// Make the new face the current leg, and select the very first distance.
	m_NumLeg++;
	m_CurLeg++;
	Refresh(0);
             */
        }
    }
}