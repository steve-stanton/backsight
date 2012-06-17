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
        /// Working copy of the leg faces in the connection path.
        /// </summary>
        readonly List<LegFace> m_Faces;

        /// <summary>
        /// Index of the current leg face (points into m_Faces).
        /// </summary>
        int m_CurFaceIndex;

        /// <summary>
        /// The geometry for the spans on the current leg face
        /// </summary>
        ILineGeometry[] m_FaceSections;

        readonly PathEditor m_Edits;

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
	        m_CurFaceIndex = -1;
	        m_pop = null;

            // Get the object that was selected for update.
            m_pop = (m_UpdCmd.GetOp() as PathOperation);
            if (m_pop == null)
                throw new ArgumentException("Cannot obtain original connection path for update");

            // Get a working copy of the connection path legs
            // TODO - This will not be suitable in a situation where staggered legs have been created
            Leg[] legs = PathParser.CreateLegs(m_pop.EntryString, m_pop.EntryUnit);

            m_Faces = new List<LegFace>();
            foreach (Leg leg in legs)
            {
                m_Faces.Add(leg.PrimaryFace);

                if (leg.AlternateFace != null)
                    m_Faces.Add(leg.AlternateFace);
            }

            m_Edits = new PathEditor(legs);
        }

        #endregion

        /// <summary>
        /// The number of leg faces in the (possibly revised) connection path.
        /// </summary>
        int NumFace
        {
            get { return m_Faces.Count; }
        }

        /// <summary>
        /// Obtains the legs associated with the currently defined faces.
        /// </summary>
        /// <returns>The legs currently defined for the path.</returns>
        Leg[] GetLegs()
        {
            var result = new List<Leg>(m_Faces.Count);

            foreach (LegFace face in m_Faces)
            {
                if (face.FaceNumber != 2)
                    result.Add(face.Leg);
            }

            return result.ToArray();
        }

        private void UpdatePathForm_Shown(object sender, EventArgs e)
        {
            // Initialize radio buttons.
            insBeforeRadioButton.Checked = true;
            brkBeforeRadioButton.Checked = true;

            // Only let the user flip annotations if they're currently visible
            flipDistButton.Enabled = EditingController.Current.AreLineAnnotationsDrawn;

            // Display the precision of the connection path.
            PathInfo p = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, GetLegs());
            ShowPrecision(p);

            // A feature on the connection path should have been selected - determine which leg face it's part of
            LegFace face = null;
            Feature f = m_UpdCmd.GetUpdate();
            if (f != null)
            {
                face = m_pop.GetLegFace(f);
                int faceIndex = GetFaceIndex(face.Sequence);
                SetCurrentFace(faceIndex);
            }

            if (m_CurFaceIndex < 0)
                SetCurrentFace(0);

            if (face == null)
                Refresh(-1);
            else
                Refresh(face.GetIndex(f));
        }

        int GetFaceIndex(InternalIdValue sequence)
        {
            for (int i=0; i<m_Faces.Count; i++)
            {
                if (m_Faces[i].Sequence.Equals(sequence))
                    return i;
            }

            return -1;
        }

        private void angleButton_Click(object sender, EventArgs e)
        {
            StraightLeg leg = (CurrentFace.Leg as StraightLeg);
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

                    m_Edits.SetStartAngle(m_CurFaceIndex, dial.SignedAngle, dial.IsDeflection);
                    Rework();
                }
            }
        }

        private void breakButton_Click(object sender, EventArgs e)
        {
            LegFace face = this.CurrentFace;
            StraightLeg leg = (face.Leg as StraightLeg);
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
            int index = leg.PrimaryFace.GetIndex(dist);
            if (isBefore && index == 0)
            {
                MessageBox.Show("You can't break at the very beginning of the leg.");
                return;
            }

            if (!isBefore && (index + 1) == face.Count)
            {
                MessageBox.Show("You can't break at the very end of the leg.");
                return;
            }

            // Break the leg.
            if (!isBefore)
                index++;

            m_Edits.BreakLeg(m_CurFaceIndex, index);

            StraightLeg newLeg = leg.Break(index);
            if (newLeg == null)
                return;

            // Make the new leg the current one, and select the very first distance.
            int faceIndex = m_Faces.IndexOf(face);
            Debug.Assert(faceIndex >= 0);
            m_Faces.Insert(faceIndex+1, newLeg.PrimaryFace);
            SetCurrentFace(faceIndex + 1);
            Refresh(0);
        }

        private void curveButton_Click(object sender, EventArgs e)
        {
            CircularLeg leg = (CurrentFace.Leg as CircularLeg);
            if (leg == null)
                return;

            CircularLegMetrics metrics = leg.Metrics;

            if (metrics.IsCulDeSac)
            {
                using (CulDeSacForm dial = new CulDeSacForm(leg))
                {
                    if (dial.ShowDialog() == DialogResult.OK)
                    {
                        metrics.SetCentralAngle(dial.CentralAngle);
                        metrics.SetRadius(dial.Radius);
                        metrics.IsClockwise = dial.IsClockwise;
                        var newMetrics = new CircularLegMetrics(dial.Radius, dial.IsClockwise, dial.CentralAngle);
                        m_Edits.SetArcMetrics(m_CurFaceIndex, newMetrics);
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
                        metrics.SetEntryAngle(dial.EntryAngle);
                        metrics.SetExitAngle(dial.ExitAngle);
                        metrics.SetRadius(dial.Radius);
                        metrics.IsClockwise = dial.IsClockwise;
                        var newMetrics = new CircularLegMetrics(dial.Radius, dial.IsClockwise, dial.EntryAngle, dial.ExitAngle);
                        m_Edits.SetArcMetrics(m_CurFaceIndex, newMetrics);
                        Rework();
                    }
                }
            }
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            LegFace face = CurrentFace;
            if (face == null)
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
                    m_Edits.InsertSpan(m_CurFaceIndex, newDist, dist, isBefore);
                    int index = face.Insert(newDist, dist, isBefore, true);
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
            var path = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, GetLegs());
            m_FaceSections = path.GetSections(CurrentFace);

            ShowPrecision(path);
            m_UpdCmd.ErasePainting();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            SetCurrentFace(m_CurFaceIndex + 1);
            if (m_CurFaceIndex < NumFace)
                Refresh(0);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            SetCurrentFace(m_CurFaceIndex - 1);
            if (m_CurFaceIndex >= 0)
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
            PathInfo p = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, GetLegs());
            p.Render(display);

            // Highlight the currently selected line.
            int index = distancesListBox.SelectedIndex;
            if (index >= 0 && index < m_FaceSections.Length)
            {
                IDrawStyle style = new HighlightStyle();
                ILineGeometry geom = m_FaceSections[index];
                if (geom is IClockwiseCircularArcGeometry)
                    style.Render(display, (IClockwiseCircularArcGeometry)geom);
                else
                    style.Render(display, new IPosition[] { geom.Start, geom.End });
            }
        }

        /// <summary>
        /// Refreshes the display upon selection of a specific observed distance.
        /// </summary>
        /// <param name="index">The array index of the currently selected distance (-1 if nothing is
        /// currently selected).</param>
        void Refresh(int index)
        {
            if (m_CurFaceIndex < 0 || m_CurFaceIndex >= NumFace)
                return;

            this.Text = String.Format("Leg {0} of {1}", m_CurFaceIndex + 1, NumFace);

            // Enable the back/next buttons, depending on what leg we're on.
            previousButton.Enabled = (m_CurFaceIndex > 0);
            nextButton.Enabled = (m_CurFaceIndex + 1 < NumFace);

            // Get the corresponding face.
            LegFace face = CurrentFace;
            Leg leg = face.Leg;

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
                if (m_CurFaceIndex > 0 && m_Faces[m_CurFaceIndex - 1].Leg is StraightLeg)
                    isPrevStraight = true;

                angleButton.Enabled = isPrevStraight;
            }

            // You can't create a new face if the leg is already staggered.
            newFaceButton.Enabled = (leg.IsStaggered == false);

            // Indicate whether we're on the second face
            secondFaceLabel.Visible = (face == leg.AlternateFace);

            // List the observed distances for the leg.
            distancesListBox.Items.Clear();
            if (face.Count == 0)
            {
                distancesListBox.Items.Add("see central angle");
                distancesListBox.SelectedIndex = 0;
            }
            else
            {
                Distance[] dists = face.GetObservedDistances();
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
            if (m_CurFaceIndex < 0 || m_CurFaceIndex >= NumFace)
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
                    LegFace face = this.CurrentFace;
                    int spanIndex = distancesListBox.SelectedIndex;
                    SpanInfo spanInfo = face.GetSpanData(spanIndex);
                    spanInfo.ObservedDistance = dist.Distance;

                    // Change the displayed distance
                    distancesListBox.Items[spanIndex] = spanInfo.ObservedDistance;

                    m_Edits.UpdateSpan(m_CurFaceIndex, spanIndex, dist.Distance);
                    Rework();
                    Refresh(spanIndex);
                }
            }
        }

        private void flipDistButton_Click(object sender, EventArgs e)
        {
            LegFace face = CurrentFace;
            if (face == null)
                return;

            m_Edits.FlipLegAnnotations(m_CurFaceIndex);

            foreach (SpanInfo span in face.Spans)
            {
                if (span.HasLine)
                    span.ObservedDistance.ToggleIsFlipped();
            }

            m_UpdCmd.ErasePainting();
        }

        /// <summary>
        /// The current leg face (null if <see cref="m_CurFaceIndex"/> refers to an invalid face)
        /// </summary>
        LegFace CurrentFace
        {
            get
            {
                if (m_CurFaceIndex < 0 || m_CurFaceIndex >= m_Faces.Count)
                    return null;
                else
                    return m_Faces[m_CurFaceIndex];
            }
        }

        void SetCurrentFace(int faceIndex)
        {
            // Do nothing if the leg is unchanged
            if (faceIndex == m_CurFaceIndex)
                return;

            if (faceIndex < 0 || faceIndex >= m_Faces.Count)
                throw new IndexOutOfRangeException();

            // Remember the new leg
            m_CurFaceIndex = faceIndex;

            // Calculate the rotation and scaling
            PathInfo path = new PathInfo(m_pop.StartPoint, m_pop.EndPoint, GetLegs());

            // Obtain the spans on the current face
            m_FaceSections = path.GetSections(CurrentFace);
        }

        private void newFaceButton_Click(object sender, EventArgs e)
        {
            LegFace face = CurrentFace;
            if (face == null)
                return;

            // Get the observed length of the leg (in meters on the ground).
            double len = face.GetTotal();

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

                    // Attach the new face to the leg
                    var newFace = new LegFace(face.Leg, dists);

                    // Insert the new face into our array of faces.
                    int faceIndex = m_Faces.IndexOf(face);
                    m_Faces.Insert(faceIndex + 1, newFace);

                    // Make the new face the current leg, and select the very first distance.
                    SetCurrentFace(m_CurFaceIndex + 1);
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