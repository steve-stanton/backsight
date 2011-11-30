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
using System.Text;

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
        Distance[] m_Face1;

        /// <summary>
        /// The distances for the alternate face (if there is one)
        /// </summary>
        Distance[] m_Face2;

        /// <summary>
        /// Was the alternate face created via this dialog?
        /// </summary>
        //bool m_IsFace2New;

        /// <summary>
        /// The currently listed face (m_Face1 or m_Face2). Should never be null.
        /// </summary>
        Distance[] m_CurrentFace;

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
            //m_IsFace2New = false;
            this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        /// <summary>
        /// Obtains the distances for one subdivision face (along with a note as
        /// to whether the annotation has been flipped or not).
        /// </summary>
        /// <param name="face">The face of interest (may be null)</param>
        /// <returns>A copy of the distances along the face (null if the supplied face is null).</returns>
        Distance[] CopyDistances(LineSubdivisionFace face)
        {
            if (face == null)
                return null;

            Distance[] current = face.ObservedLengths;
            Distance[] result = new Distance[current.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = new Distance(current[i]);

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
            m_Face1 = CopyDistances(m_pop.PrimaryFace);
            m_Face2 = CopyDistances(m_pop.AlternateFace);

            // If we have two faces, the "New Face" button means you want to switch to the other face.
            if (m_Face2 != null)
                newFaceButton.Text = "&Other Face";

            // If we have a selected line section that is on the second face, make that the initial face.
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
                return m_pop.PrimaryFace.Sections[listIndex];

            // May have just added new face
            if (m_CurrentFace == m_Face2 && m_pop.AlternateFace != null)
                return m_pop.AlternateFace.Sections[listIndex];

            return null;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            // Get the selected distance.
            Distance d = (listBox.SelectedItem as Distance);
            if (d == null)
            {
                MessageBox.Show("You must first select a distance from the list.");
                return;
            }

            //m_SelectedLine = GetLine(listBox.SelectedIndex);
            Distance dCopy = new Distance(d);

            using (DistForm dist = new DistForm(dCopy, false))
            {
                if (dist.ShowDialog() == DialogResult.OK)
                {
                    // Change the displayed distance
                    m_CurrentFace[listBox.SelectedIndex] = new Distance(dist.Distance);
                    m_CurrentFace[listBox.SelectedIndex].IsAnnotationFlipped = d.IsAnnotationFlipped;
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
            // Draw the features originally created by the op in gray.
            ISpatialDisplay display = m_UpdCmd.ActiveDisplay;
            IDrawStyle style = m_UpdCmd.Controller.Style(Color.Gray);
            style.IsFixed = true;
            m_pop.Render(display, style, true);

            // Highlight the currently selected section (if any).
            if (m_SelectedLine != null)
                m_SelectedLine.Render(display, new HighlightStyle());

            // Draw points on the current face (except the last one, which should
            // coincide with the end of the parent line).

            // Leave for now.... need to also cover possible reverse, and annotations

            // and need annotations only if they're drawn
            //if (!EditingController.Current.AreLineAnnotationsDrawn)

            /*
            double[] offsets = LineSubdivisionFace.GetAdjustedLengths(m_pop.Parent, m_CurrentFace);
            LineGeometry geom = m_pop.Parent.LineGeometry;
            IPosition start = geom.Start;
            IPosition end = null;

            for (int i = 0; i < offsets.Length - 1; i++, start = end)
            {
                geom.GetPosition(new Length(offsets[i]), out end);

            }
            */
        }

        void RefreshList()
        {
            // Highlight currently selected line (if any).
            Draw();

            // List the observed distances, relating each distance to the corresponding line.
            listBox.Items.Clear();
            listBox.Items.AddRange(m_CurrentFace);

            // Always leave the focus in the list of distances.
            listBox.Focus();
        }

        private void flipDistButton_Click(object sender, EventArgs e)
        {
            // Get the selected distance.
            int index = listBox.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("You must first select a distance from the list.");
                return;
            }

            Distance faceDist = m_CurrentFace[index];
            faceDist.ToggleIsFlipped();

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

            // If a second face doesn't already exist, get the user to specify the distances.

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

                        // Default annotations to the flip side
                        foreach (Distance d in dists)
                            d.IsAnnotationFlipped = true;

                        // Create the new face (for use in preview only)
                        //m_pop.AlternateFace = CreateAlternateFace(dists);
                        m_Face2 = dists;
                        //m_IsFace2New = true;

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
        /// Creates a new face for the line (for use in previewing the outcome
        /// of the change).
        /// </summary>
        /// <param name="dists">The lengths of the line sections along the alternate face</param>
        LineSubdivisionFace CreateAlternateFace(Distance[] dists)
        {
            // Distances are assumed to come from the start of the line (the LegForm dialog
            // doesn't let you reverse things).
            LineSubdivisionFace face = new LineSubdivisionFace(dists, false);

            // Create features, but without assigning any feature IDs to anything.
            // The ThrowawayFeatureFactory creates stuff with a session ID of 0 (denoting a temporary
            // feature that shouldn't get pointed to by anything else).
            // - prob better to do stuff entirely in Draw
            /*
            FeatureFactory ff = new ThrowawayFeatureFactory(m_pop);
            ff.LineType = m_pop.Parent.EntityType;
            face.CreateSections(m_pop.Parent, ff);

            // Define geometry
            face.CalculateGeometry(m_pop.Parent, null);

            // Add to spatial index
            //Feature[] feats = ff.CreatedFeatures;
            //m_pop.MapModel.AddToIndex(feats);
            */
            return face;
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

            foreach (Distance d in m_CurrentFace)
                length += d.Meters;

            return length;
        }

        /// <summary>
        /// Obtains update items for each revised section.
        /// </summary>
        /// <returns>The items representing the change.</returns>
        internal UpdateItemCollection GetUpdateItems()
        {
            return m_pop.GetUpdateItems(m_Face1, m_Face2);
        }

        private void LineSubdivisionUpdateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure any flipped annotations have been temporarily flipped back. To set the
            // status for good, the appropriate update items must be returned by GetUpdateItems.

            // ...avoid the complication here, calculate stuff as part of Draw
            /*
            CloseFace(m_Face1, m_pop.PrimaryFace);

            if (m_IsFace2New)
            {
                m_pop.AlternateFace = null;
            }
            else
            {
                CloseFace(m_Face2, m_pop.AlternateFace);
            }
             */
        }

        void CloseFace(Distance[] dists, LineSubdivisionFace face)
        {
            /*
            if (face == null)
                return;

            LineFeature[] sections = face.Sections;
            Debug.Assert(sections.Length == dists.Length);

            for (int i = 0; i < sections.Length; i++)
            {
                if (dists[i].IsAnnotationFlipped)
                {
                    Distance d = sections[i].ObservedLength;
                    d.ToggleIsFlipped();
                }
            }
             */
        }
    }
}
