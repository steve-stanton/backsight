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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Backsight.Editor.Observations;
using Backsight.Editor.Operations;
using Backsight.Editor.UI;
using Backsight.Forms;


namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for updating a connection path.
    /// </summary>
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
        /// Working copy of the legs in the connection path.
        /// </summary>
        readonly List<Leg> m_Legs;

        /// <summary>
        /// Index of the current leg (points into m_Legs).
        /// </summary>
        int m_CurLeg;

        /// <summary>
        /// The geometry for the spans on the current leg
        /// </summary>
        ILineGeometry[] m_LegSections;

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
            Owner = EditingController.Current.MainForm;

            if (update == null)
                throw new ArgumentNullException();

            m_UpdCmd = update;
	        m_CurLeg = -1;
	        m_pop = null;

            // Get the object that was selected for update.
            m_pop = (m_UpdCmd.GetOp() as PathOperation);
            if (m_pop == null)
                throw new ArgumentException("Cannot obtain original connection path for update");

            // Get a working copy of the connection path legs
            // TODO - This will not be suitable in a situation where staggered legs have been created
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

            // Only let the user flip annotations if they're currently visible
            flipDistButton.Enabled = EditingController.Current.AreLineAnnotationsDrawn;

            // Display the precision of the connection path.
            PathInfo p = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, m_Legs.ToArray());
            ShowPrecision(p);

            // A feature on the connection path should have been selected - determine which leg it's part of
            Leg leg = null;
            Feature f = m_UpdCmd.GetUpdate();
            if (f != null)
            {
                leg = m_pop.GetLeg(f);
                int legIndex = m_pop.GetLegIndex(leg);
                SetCurrentLeg(legIndex);
            }

            if (m_CurLeg < 0)
                SetCurrentLeg(0);

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

                    Rework();
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

            // Get the selected distance
            Distance dist = GetSel();
            if (dist == null)
            {
                MessageBox.Show("You must first select a distance from the list.");
                return;
            }

            // Are we breaking before or after the currently selected distance?
            bool isBefore = brkBeforeRadioButton.Checked;

            // You can't break at the very beginning or end of the leg.
            int index = leg.GetIndex(dist);
            if (isBefore && index == 0)
            {
                MessageBox.Show("You can't break at the very beginning of the leg.");
                return;
            }

            if (!isBefore && (index + 1) == leg.Count)
            {
                MessageBox.Show("You can't break at the very end of the leg.");
                return;
            }

            // Break the leg.
            if (!isBefore)
                index++;

            Leg newLeg = leg.Break(index);
            if (newLeg == null)
                return;

            // Make the new leg the current one, and select the very first distance.
            int legIndex = m_Legs.IndexOf(leg);
            Debug.Assert(legIndex >= 0);
            m_Legs.Insert(legIndex+1, newLeg);
            SetCurrentLeg(legIndex + 1);
            Refresh(0);
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
                        Rework();
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
                        Rework();
                    }
                }
            }
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            Leg leg = CurrentLeg;
            if (leg == null)
                return;

            Distance dist = GetSel();
            if (dist == null)
            {
                MessageBox.Show("You must first select a distance from the list.");
                return;
            }

            // Are we inserting before or after the currently selected distance?
            bool isBefore = insBeforeRadioButton.Checked;
 
            // Get the user to specify a new distance.
            using (var dial = new DistForm(dist, false))
            {
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    // Insert the new distance into the current leg.
                    Distance newDist = dial.Distance;
                    int index = leg.Insert(newDist, dist, isBefore, true);
                    Rework();
                    Refresh(index);
                }
            }
        }

        /// <summary>
        /// Recalculates the path after some sort of change, and erases painting so that it can
        /// be redrawn in idle time.
        /// </summary>
        void Rework()
        {
            // Rework the geometry for the sections on the current leg
            PathInfo path = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, m_Legs.ToArray());
            m_LegSections = path.GetSections(m_CurLeg);

            ShowPrecision(path);

            m_UpdCmd.ErasePainting();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            SetCurrentLeg(m_CurLeg + 1);
            if (m_CurLeg < NumLeg)
                Refresh(0);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            SetCurrentLeg(m_CurLeg - 1);
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

        /// <summary>
        /// Does any painting that this dialog does.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal void Render(ISpatialDisplay display)
        {
            // Draw the original path (in pale gray)
            IDrawStyle gray = new DrawStyle(Color.LightGray);
            m_pop.Render(display, gray, true);

            // Draw the current path (in magenta).
            PathInfo p = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, m_Legs.ToArray());
            p.Render(display);

            // Highlight the currently selected line.
            int index = distancesListBox.SelectedIndex;
            if (index >= 0 && index < m_LegSections.Length)
            {
                IDrawStyle style = new HighlightStyle();
                ILineGeometry geom = m_LegSections[index];
                if (geom is IClockwiseCircularArcGeometry)
                    style.Render(display, (IClockwiseCircularArcGeometry)geom);
                else
                    style.Render(display, new IPosition[] { geom.Start, geom.End });
            }
        }

        void Refresh(int index)
        {
            if (m_CurLeg < 0 || m_CurLeg >= NumLeg)
                return;

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

            // Indicate whether we're on the second face
            secondFaceLabel.Visible = (leg as ExtraLeg) != null;

            // List the observed distances for the leg.
            distancesListBox.Items.Clear();
            if (leg.Count == 0)
            {
                distancesListBox.Items.Add("see central angle");
                distancesListBox.SelectedIndex = 0;
            }
            else
            {
                Distance[] dists = leg.GetObservedDistances();
                distancesListBox.Items.AddRange(dists);

                // Select the first (or last) item in the list.
                if (index < 0)
                    distancesListBox.SelectedIndex = dists.Length - 1;
                else if (index < dists.Length)
                    distancesListBox.SelectedIndex = index;
            }

            // Always leave the focus in the list of distances.
            distancesListBox.Focus();
        }

        void ShowPrecision(PathInfo p)
        {
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

        private void updateButton_Click(object sender, EventArgs e)
        {
            // Get the selected distance.
            Distance d = GetSel();
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
                    // Change the distance stored in the leg
                    Leg leg = this.CurrentLeg;
                    int spanIndex = distancesListBox.SelectedIndex;
                    SpanInfo spanInfo = leg.GetSpanData(spanIndex);
                    spanInfo.ObservedDistance = dist.Distance;

                    // Change the displayed distance
                    distancesListBox.Items[spanIndex] = spanInfo.ObservedDistance;

                    Rework();
                    Refresh(spanIndex);
                }
            }
        }

        private void flipDistButton_Click(object sender, EventArgs e)
        {
            Leg leg = CurrentLeg;
            if (leg == null)
                return;

            foreach (SpanInfo span in leg.Spans)
            {
                if (span.HasLine)
                    span.ObservedDistance.ToggleIsFlipped();
            }

            m_UpdCmd.ErasePainting();
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

        void SetCurrentLeg(int legIndex)
        {
            // Do nothing if the leg is unchanged
            if (legIndex == m_CurLeg)
                return;

            if (legIndex < 0 || legIndex >= m_Legs.Count)
                throw new IndexOutOfRangeException();

            // Remember the new leg
            m_CurLeg = legIndex;

            // Calculate the rotation and scaling
            PathInfo path = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, m_Legs.ToArray());

            // Obtain the spans on the current leg
            m_LegSections = path.GetSections(legIndex);
        }

        private void newFaceButton_Click(object sender, EventArgs e)
        {
            Leg leg = CurrentLeg;
            if (leg == null)
                return;

            // Get the observed length of the leg (in meters on the ground).
            double len = leg.GetTotal();

            try
            {
                this.WindowState = FormWindowState.Minimized;

                // Present a data entry dialog for the new face.
                using (LegForm dial = new LegForm(len))
                {
                    if (dial.ShowDialog() != DialogResult.OK)
                        return;

                    // Create the new face and insert it after the current leg.

                    // Must be at least two distances
                    Distance[] dists = dial.Distances;
                    if (dists == null || dists.Length < 2)
                    {
                        MessageBox.Show("The new face must have at least two spans");
                        return;
                    }

                    // Default annotations to the flip side
                    foreach (Distance d in dists)
                        d.IsAnnotationFlipped = true;

                    // Insert the new face
                    ExtraLeg newLeg = new ExtraLeg(leg, dists);

                    // Create features for the extra leg.
                    //newLeg.MakeFeatures(this);

                    // Insert the new leg into our array of legs.
                    int legIndex = m_Legs.IndexOf(leg);
                    m_Legs.Insert(legIndex + 1, newLeg);

                    leg.FaceNumber = 1;
                    newLeg.FaceNumber = 2;

                    // Make the new face the current leg, and select the very first distance.
                    SetCurrentLeg(m_CurLeg + 1);
                    Refresh(0);
                }
            }

            finally
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
    }
}