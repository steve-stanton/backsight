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
        /// The line that is currently selected (if defined, it should be one of the transient
        /// lines in m_CurrentFace).
        /// </summary>
        LineFeature m_SelectedLine;

        /// <summary>
        /// The line subdivision involved (always relates to the primary face)
        /// </summary>
        LineSubdivisionOperation m_pop;

        /// <summary>
        /// The working copy of the primary face
        /// </summary>
        LineSubdivisionFace m_Face1;

        /// <summary>
        /// The working copy of the alternate face (if there is one).
        /// </summary>
        LineSubdivisionFace m_Face2;

        /// <summary>
        /// The currently listed face (m_Face1 or m_Face2). Should never be null.
        /// </summary>
        LineSubdivisionFace m_CurrentFace;

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

            // Get the edit that created the primary face
            m_pop = (feat.Creator as LineSubdivisionOperation);
            Debug.Assert(m_pop != null);

            if (!m_pop.IsPrimaryFace)
            {
                m_pop = m_pop.OtherSide;
                Debug.Assert(m_pop != null);
                Debug.Assert(m_pop.IsPrimaryFace);
            }

            // Grab something we throw away if the user decides to cancel
            m_Face1 = CreateWorkingFace(m_pop.Face);
            m_Face2 = (m_pop.OtherSide == null ? null : CreateWorkingFace(m_pop.OtherSide.Face));

            // If we have two faces, the "New Face" button means you want to switch to the other face.
            if (m_Face2 != null)
                newFaceButton.Text = "&Other Face";

            // Default to the face containing the initially selected feature
            m_CurrentFace = (feat.Creator == m_pop ? m_Face1 : m_Face2);

            // If a line was selected, remember where it is in our working copy (and if it's actually on
            // the alternate face, make that the initial face for editing).
            m_SelectedLine = null;
            LineFeature selectedLine = (feat as LineFeature);
            if (selectedLine != null)
            {
                LineFeature[] sections = (m_CurrentFace == m_Face1 ? m_pop.Face.Sections : m_pop.OtherSide.Face.Sections);
                int lineIndex = Array.FindIndex<LineFeature>(sections, t => t == selectedLine);

                if (lineIndex >= 0)
                    m_SelectedLine = m_CurrentFace.Sections[lineIndex];
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

            return m_CurrentFace.Sections[listIndex];
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

            Distance dCopy = new Distance(d);

            using (DistForm dist = new DistForm(dCopy, false))
            {
                if (dist.ShowDialog(this) == DialogResult.OK)
                {
                    // Change the displayed distance
                    m_CurrentFace.ObservedLengths[listBox.SelectedIndex] = dist.Distance;
                    m_CurrentFace.Sections[listBox.SelectedIndex].ObservedLength = dist.Distance;
                    m_UpdCmd.ErasePainting();
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

            // Ensure the current face has up-to-date geometry and draw that using magenta draw style
            m_CurrentFace.CalculateGeometry(m_pop.Parent, null);
            style = m_UpdCmd.Controller.Style(Color.Magenta);
            style.IsFixed = true;
            foreach (LineFeature line in m_CurrentFace.Sections)
                line.Render(display, style);

            // Highlight the currently selected section (if any).
            if (m_SelectedLine != null)
                m_SelectedLine.Render(display, new HighlightStyle());
        }

        void RefreshList()
        {
            // Highlight currently selected line (if any).
            Draw();

            // List the observed distances on the current face
            listBox.Items.Clear();
            listBox.Items.AddRange(m_CurrentFace.ObservedLengths);

            // If a line is currently selected, select the corresponding distance in the listbox
            if (m_SelectedLine != null)
            {
                int lineIndex = Array.FindIndex<LineFeature>(m_CurrentFace.Sections, t => t == m_SelectedLine);
                if (lineIndex >= 0)
                    listBox.SelectedIndex = lineIndex;
            }

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

            Distance faceDist = m_CurrentFace.ObservedLengths[index];
            faceDist.ToggleIsFlipped();

            // Ensure stuff gets redrawn
            m_UpdCmd.ErasePainting();
        }

        private void newFaceButton_Click(object sender, EventArgs e)
        {
            // If we previously highlighted something, draw it normally (since it cannot exist as part of any other face).
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
                        m_Face2 = new LineSubdivisionFace(dists, m_Face1.IsEntryFromEnd);
                        InitializeWorkingFace(m_Face2);

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
        /// Creates a working copy of an existing face.
        /// </summary>
        /// <param name="face">The face to copy</param>
        /// <returns>The working copy</returns>
        LineSubdivisionFace CreateWorkingFace(LineSubdivisionFace face)
        {
            if (face == null)
                return null;

            // Make a copy of the distance observations from the source face
            Distance[] distances = CopyDistances(face);
            LineSubdivisionFace result = new LineSubdivisionFace(distances, face.IsEntryFromEnd);

            // Create sections and calculate geometry
            InitializeWorkingFace(result);
            return result;
        }

        void InitializeWorkingFace(LineSubdivisionFace face)
        {
            // Create throwaway line sections (without any feature IDs, not associated with any session,
            // not in spatial index).
            FeatureFactory ff = new ThrowawayFeatureFactory(m_pop);
            ff.LineType = m_pop.Parent.EntityType;
            face.CreateSections(m_pop.Parent, ff, false);

            // And calculate initial geometry
            face.CalculateGeometry(m_pop.Parent, null);
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

            foreach (Distance d in m_CurrentFace.ObservedLengths)
                length += d.Meters;

            return length;
        }

        /// <summary>
        /// The working copy of the primary face
        /// </summary>
        internal LineSubdivisionFace WorkingFace1
        {
            get { return m_Face1; }
        }

        /// <summary>
        /// The working copy of the alternate face (if there is one).
        /// </summary>
        internal LineSubdivisionFace WorkingFace2
        {
            get { return m_Face2; }
        }
    }
}
