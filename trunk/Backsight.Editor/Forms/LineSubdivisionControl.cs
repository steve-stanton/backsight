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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    public partial class LineSubdivisionControl : UserControl
    {
        #region Class data

        /// <summary>
        /// The command running this dialog.
        /// </summary>
        readonly LineSubdivisionUI m_Cmd;

        /// <summary>
        /// Line length
        /// </summary>
        double m_GroundLength;

        /// <summary>
        /// The line being subdivided
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// The entered distances
        /// </summary>
        List<Distance> m_Distances;

        /// <summary>
        /// True if distances are with respect to the start of the line.
        /// </summary>
        bool m_FromStart;

        #endregion

        #region Constructors

        internal LineSubdivisionControl(LineSubdivisionUI cmd, LineFeature line, Operation recall)
        {
            InitializeComponent();

            // Remember the command.
            m_Cmd = cmd;

            // Remember the line involved & its length (in meters)
            m_Line = line;
            m_GroundLength = line.GroundLength.Meters;

            // No distances so far.
            m_Distances = null;
            m_FromStart = true;

            // If we are recalling a previous operation, grab the observed distances from there (only the primary face)
            LineSubdivisionOperation op = (recall as LineSubdivisionOperation);

            if (op!=null)
            {
                MeasuredLineFeature[] sections = op.FirstFace.Sections;
                m_Distances = new List<Distance>(sections.Length);
                foreach (MeasuredLineFeature s in sections)
                    m_Distances.Add(new Distance(s.ObservedLength));
            }
        }

        #endregion

        private void LineSubdivisionControl_Load(object sender, EventArgs e)
        {
            // Display at top left corner of the screen.
            //SetDesktopLocation(0, 0);

            // Display the length of the arc that is being subdivided (in
            // the current data entry units).
            DistanceUnit dunit = CadastralMapModel.Current.EntryUnit;
            lengthTextBox.Text = dunit.Format(m_GroundLength);

            // Check the radio button saying distances are from the
            // start of the line.
            startRadioButton.Checked = true;
            endRadioButton.Checked = false;

            // If we have recalled distances from some earlier edit, load up the list.
            if (m_Distances!=null)
            {
                string[] ds = new string[m_Distances.Count];
                for (int i=0; i<ds.Length; i++)
                    ds[i] = m_Distances[i].Format();
                distancesTextBox.Lines = ds;
            }

            distancesTextBox.Focus();
            distancesTextBox.ScrollToCaret();
        }

        internal bool Save()
        {
            // Return if we do not have at least 2 distances
            if (m_Distances.Count<2)
            {
                MessageBox.Show("At least two distances must be specified.");
                return false;
            }

            // Reverse the distances if necessary.
            if (!m_FromStart)
            {
                for (int i=0, j=m_Distances.Count-1; i<j; i++, j--)
                {
                    Distance tmp = m_Distances[i];
                    m_Distances[i] = m_Distances[j];
                    m_Distances[j] = tmp;
                }
            }

            // Subdivide the line...

            LineSubdivisionOperation op = null;

            try
            {
                op = new LineSubdivisionOperation(m_Line);
                op.AddDistances(m_Distances);
                op.Execute();
                return true;
            }

            catch (Exception ex)
            {
                Session.CurrentSession.Remove(op);
                MessageBox.Show(ex.Message);
            }

            return false;
        }


        private void okButton_Click(object sender, EventArgs e)
        {
            // Ensure the distances are complete
            GetDistances();
            if (m_Distances.Count < 2)
            {
                MessageBox.Show("You must specify at least 2 distances.");
                return;
            }

            m_Cmd.DialFinish(this);
        }

        /// <summary>
        /// Handles request to reckon distances from the start of the line.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_FromStart = startRadioButton.Checked;
            m_Cmd.ErasePainting();
        }

        /// <summary>
        /// Handles request to reckon distances from the end of the line.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_FromStart = !endRadioButton.Checked;
            m_Cmd.ErasePainting();
        }

        /// <summary>
        /// Parses the distances list box. This defines <see cref="m_Distances"/>.
        /// </summary>
        /// <returns>The total distance entered, in meters.</returns>
        /// <exception cref="Exception">If any parsing error occurred. In that case,
        /// <c>m_Distances</c> will be null.</exception>
        double GetDistances()
        {
            // Delete any distances & positions previously obtained.
            m_Distances = null;

            // Initialize total distance.
            double tot = 0.0;
            List<Distance> ds = new List<Distance>();

            foreach (string t in distancesTextBox.Lines)
            {
                // Skip empty lines
                if (t.Length==0)
                    continue;

                // Hold seperate reference, since may attempt to change foreach iterator variable below
                string s = t;

                // Strip out any repeat count
                int nRepeat = 1;
                int repeat = s.IndexOf('*');
                if (repeat>=0)
                {
                    string rest = s.Substring(repeat+1);

                    // The user may have entered the *, and still has to append something else.
                    // In that case, just default to a repear count of 1.
                    if (rest.Length > 0)
                    {
                        nRepeat = int.Parse(rest);
                        if (nRepeat<=0)
                            throw new Exception("Repeat count cannot be less than or equal to zero");
                    }

                    s = s.Substring(0, repeat);
                }

                // Parse the distance (return on any error)
                Distance d = new Distance(s);
                if (!d.IsDefined)
                    return 0.0;

                // Append distances to results list
                for (int i=0; i<nRepeat; i++)
                    ds.Add(d);

                // Update total length
                tot += (nRepeat * d.Meters);
            }

            m_Distances = ds;
            return tot;
        }

        /// <summary>
        /// Works out positions for this subdivision op and draws them. Before calling
        /// this function, a call to <c>GetDistances</c> is required.
        /// </summary>
        List<IPosition> AddPoints()
        {
            // Return if there is nothing to add (we need at least 2 distances).
            if (m_Distances==null || m_Distances.Count<=1)
                return null;

            // Get adjusted lengths for each section (on the map projection, not the ground)
            LineSubdivisionFace face = new LineSubdivisionFace(m_Distances);
            ILength mapLen = m_Line.Length;
            double[] lens = face.GetAdjustedLengths(mapLen);

            List<IPosition> pts = new List<IPosition>(lens.Length);
            double tot = (m_FromStart ? 0.0 : mapLen.Meters);

            foreach (double d in lens)
            {
                if (m_FromStart)
                    tot += d;
                else
                    tot -= d;

                IPosition p;
                if (m_Line.LineGeometry.GetPosition(new Length(tot), out p))
                    pts.Add(p);
            }

            return pts;
        }

        /// <summary>
        /// Handles the "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        internal void Draw()
        {
            // Highlight the line we are subdividing
            ISpatialDisplay draw = m_Cmd.ActiveDisplay;
            //IDrawStyle style = CadastralEditController.Current.HighlightStyle;
            //m_Line.Render(draw, style);

            // Draw the points (except for the last one, which should
            // correspond with either the start or the end of the line
            // we are subdividing).
            List<IPosition> pts = Calculate();

            if (pts!=null)
            {
                IDrawStyle style = CadastralEditController.Current.Style(Color.Magenta);

                for (int i=0; i<pts.Count-1; i++)
                    style.Render(draw, pts[i]);
            }

            DistanceUnit entryUnit = CadastralMapModel.Current.EntryUnit;
            double tot = GetTotalDistance();
            string s = (tot < Double.Epsilon ? String.Empty : entryUnit.Format(tot));
            totalEnteredTextBox.Text = s;

            s = (tot < Double.Epsilon ? String.Empty : entryUnit.Format(m_GroundLength-tot));
            lengthLeftTextBox.Text = s;
        }

        /// <summary>
        /// Obtains the total entered distance, in meters.
        /// </summary>
        /// <returns>The total entered distance, in meters (zero if distances cannot
        /// be parsed at the current time)</returns>
        double GetTotalDistance()
        {
            if (m_Distances==null)
                return 0.0;

            double tot = 0.0;
            foreach (Distance d in m_Distances)
                tot += d.Meters;

            return tot;
        }

        /// <summary>
        /// Attempts to calculate the outcome of this dialog. This re-defines the
        /// content of <see cref="m_Distances"/>. If any problem arises parsing distances,
        /// it gets set to null.
        /// </summary>
        /// <returns>The calculated positions (null if something went wrong)</returns>
        List<IPosition> Calculate()
        {
            try
            {
                GetDistances();
                return AddPoints();
            }

            catch { }
            return null;
        }

        private void distancesTextBox_TextChanged(object sender, EventArgs e)
        {
            m_Cmd.ErasePainting();
        }
    }
}
