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
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Editor.UI;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="10-FEB-1998" was="CdGetControl" />
    /// <summary>
    /// Dialog for getting a list of control points
    /// </summary>
    partial class GetControlForm : Form
    {
        #region Class data

        /// <summary>
        /// The command driving this dialog.
        /// </summary>
        readonly GetControlUI m_Cmd;

        /// <summary>
        /// Control ranges (one for each line in the dialog).
        /// </summary>
        readonly List<ControlRange> m_Ranges;

        /// <summary>
        /// True if the map initially has an undefined extent.
        /// </summary>
        bool m_NewMap;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>GetControlForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog</param>
        internal GetControlForm(GetControlUI cmd)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_Ranges = new List<ControlRange>();
            m_NewMap = false;
        }

        #endregion

        private void GetControlForm_Shown(object sender, EventArgs e)
        {
            // Remember whether the map starts out empty. We do this here because
            // CadastralMapModel.IsEmpty works by checking whether the map's
            // window is defined. Since we may also set the extent, we could not
            // subsequently get a correct answer as to whether the map already
            // contains data.
            m_NewMap = CadastralMapModel.Current.IsEmpty;

            // Initialize the file spec of the control file, based on the corresponding registry entry.
            // (formerly environment variable CED$ControlFile)
            string cfile = GlobalUserSetting.Read("ControlFile");
            if (!String.IsNullOrEmpty(cfile) && File.Exists(cfile))
            {
                controlFileTextBox.Text = cfile;
                controlTextBox.Focus();
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dial = new OpenFileDialog();
            dial.Filter = "Control files (*.utm)|*.utm|Text files (*.txt)|*.txt|All Files (*.*)|*.*";
            dial.DefaultExt = "utm";
            dial.Title = "Pick Control File";

            // If the user picked a file, display it, and set focus
            // to the list of control points.
            if (dial.ShowDialog() == DialogResult.OK)
            {
                controlFileTextBox.Text = dial.FileName;
                controlTextBox.Focus();
                GlobalUserSetting.Write("ControlFile", dial.FileName);
            }
        }

        private void getDataButton_Click(object sender, EventArgs e)
        {
            // Get rid of any control list previously loaded.
            m_Ranges.Clear();

            // Go through each line in the edit box, to confirm that the control ranges
            // are valid. While at it, get a count of the number of ranges.
            int nrange = 0;
            int nc = 0;

            foreach (string line in controlTextBox.Lines)
            {

                // Skip empty lines
                if (line.Length==0)
                    continue;

                // Confirm the text is valid. If not, select the current line,
                // issue message, & return.

                uint minid, maxid;
                if (GetRange(line, out minid, out maxid)==0)
                {
                    controlTextBox.SelectionStart = nc;
                    controlTextBox.SelectionLength = line.Length;
                    string msg = String.Format("Bad range '{0}'", line);
                    MessageBox.Show(msg);
                    return;
                }

                // Increment the number of valid ranges.
                nrange++;

                // Update the number of characters processed (in case we later need to
                // select problem text)
                nc += (line.Length + System.Environment.NewLine.Length);
            }

            // If no control has been specified, see if we can do things
            // using the current display window.
            if (nrange==0)
            {
                if (m_NewMap)
                    MessageBox.Show("No control points have been specified.");
                else
                    GetInsideWindow();

                return;
            }

            // Define each range (same sort of loop as above).
            m_Ranges.Capacity = nrange;
            foreach (string line in controlTextBox.Lines)
            {
                // Skip empty lines
                if (line.Length==0)
                    continue;

                uint minid, maxid;
                GetRange(line, out minid, out maxid);
                ControlRange r = new ControlRange(minid, maxid);
                m_Ranges.Add(r);
            }

            // Load array of control data.
            IWindow win = LoadControl();
            if (win==null || win.IsEmpty)
                return;

            // Display the results.
            ShowRanges(win);
        }

        /// <summary>
        /// Displays all ranges.
        /// </summary>
        /// <param name="win"></param>
        void ShowRanges(IWindow win)
        {
            // Erase whatever's currently in the list
            controlTextBox.Text = String.Empty;

            // Return if there are no ranges.
            if (m_Ranges.Count == 0)
                return;

            // If the map doesn't have a defined extent, we'll need to initialize
            // the display with the window we've got
            if (m_NewMap)
            {
                ISpatialDisplay display = m_Cmd.ActiveDisplay;
                display.ReplaceMapModel(win);

                // Tell the user the draw scale that has been defined, and ensure points are drawn
                // at that scale.
                double scale = GetSensibleScale(display.MapScale);
                display.MapScale = scale;
                if (!m_Cmd.ArePointsDrawn())
                {
                    EditingController.Current.JobFile.Data.ShowPointScale = (scale+1);
                    Debug.Assert(m_Cmd.ArePointsDrawn());
                }

                // Ensure the point size isn't TOO small (2mm at the display scale should be fine)
                EditingController.Current.JobFile.Data.PointHeight = 0.002 * scale;

                string scalemsg = String.Format("Draw scale has been set to 1:{0}", (uint)scale);
                MessageBox.Show(scalemsg);
            }

            // Only show the first 100 ranges (any more, and the string
            // might get too long to display).
            StringBuilder sb = new StringBuilder(1000);
            for (int i=0; i < Math.Min(100, m_Ranges.Count); i++)
            {
                ControlRange r = m_Ranges[i];

                // Get the range
                uint minkey = r.Min;
                uint maxkey = r.Max;

                // Form status string
                string status;
                int nfound = r.NumDefined;
                int ncontr = r.NumControl;

                if (nfound == ncontr)
                {
                    if (nfound == 1)
                        status = "found";
                    else
                        status = "all found";
                }
                else
                {
                    status = String.Format("found {0} out of {1}", nfound, ncontr);
                }

                string output;
                if (minkey == maxkey)
                    output = String.Format("{0} ({1})", minkey, status);
                else
                    output = String.Format("{0}-{1} ({2})", minkey, maxkey, status);

                sb.Append(output);
                sb.Append(System.Environment.NewLine);
            }

            controlTextBox.Text = sb.ToString();

            // Message if all the ranges could not be shown.
            if (m_Ranges.Count > 100)
            {
                string msg = String.Format("Only the first 100 ranges (of {0}) have been listed.",
                                                m_Ranges.Count);
                MessageBox.Show(msg);
            }
        }

        double GetSensibleScale(double scale)
        {
            // Pick a scale (1:25 is the largest scale we'll go to).

            // 25, 50, 75, 100
            // 150 200 250 .... 1000
            // 1500 2000 2500 .... 10000
            // 15000 20000 25000 .... 50000
            // 60000 70000 ....

            double mult;

            if (scale < 100.0)
                mult = 25.0;
            else if (scale < 1000.0)
                mult = 50.0;
            else if (scale < 10000.0)
                mult = 500.0;
            else if (scale < 50000.0)
                mult = 5000.0;
            else
                mult = 10000.0;

        	uint nmult = (uint)(scale/mult) + 1;
	        return (double)nmult * mult;
        }

        /// <summary>
        /// Parses a string that defines a range of IDs.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="minid"></param>
        /// <param name="maxid"></param>
        /// <returns>The number of IDs in the range (0 if range is invalid).</returns>
        uint GetRange(string str, out uint minid, out uint maxid)
        {
            minid = maxid = 0;
            string s = str.Trim();
            if (s.Length==0)
                return 0;

            // Do we have a range, or just a single value?
            int dash = s.IndexOf('-');

            // Invalid if the dash is at the start.
            if (dash==0)
                return 0;

            // If no dash, we should have just one integer value. Otherwise
            // we should have two integer values.
            if (dash<0)
            {
                if (!UInt32.TryParse(s, out minid) || minid==0)
                    return 0;

                // The end of range is the same as the start.
                maxid = minid;
                return 1;
            }

            // Parse the first value. It should be the dash that
            // stops the scan (or possibly white space prior to the dash).
            string t = s.Substring(0, dash).Trim();
            if (!UInt32.TryParse(t, out minid) || minid==0)
                return 0;

            t = s.Substring(dash+1).Trim();
            if (!UInt32.TryParse(t, out maxid) || maxid==0)
                return 0;

            // The max ID must be greater than the min. If not, assume
            // the user has done something like 7436-43, meaning 7436-7443.
            if (maxid < minid)
            {
                int nc = t.Length;
                if (nc==1)
                    maxid += (minid/10 * 10);
                else
                {
                    uint factor=1;
                    for (int i=0; i<(nc-1); factor*=10, i++);
                    maxid += (minid/factor * factor);
                }
            }

            if (maxid < minid)
                return 0;

            return (maxid-minid+1);
        }

        private void addToMapButton_Click(object sender, EventArgs e)
        {
            // Return if there is nothing to add.
            if (m_Ranges.Count==0)
            {
                MessageBox.Show("There is nothing to add.");
                return;
            }

            try
            {
                // Hide this dialog. If you don't, the entity type dialog that's about to get
                // displayed may be obscured, and there's no way to close it.
                this.Hide();

                // Do we have an entity type for control points?
                // This was formerly obtained via the environment variable called CED$ControlEntity
                int entId = GlobalUserSetting.ReadInt("ControlEntityTypeId", 0);

                // Get the desired entity type.
                GetEntityForm dial = new GetEntityForm(m_Cmd.ActiveLayer, SpatialType.Point, entId);
                dial.ShowDialog();
                IEntity ent = dial.SelectedEntity;
                if (ent==null)
                    throw new Exception("An entity type must be specified");

                // Remember the ID of the selected entity type
                GlobalUserSetting.WriteInt("ControlEntityTypeId", ent.Id);

                // Save the control.
                Save(ent);

                // Issue a warning message if points are not currently displayed
                if (!m_Cmd.ArePointsDrawn())
                    MessageBox.Show("Points will not be drawn at the current scale.");

                m_Cmd.DialFinish(this);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Show();
            }
        }

        /// <summary>
        /// Saves loaded control points in the map. This creates the editing operation
        /// and executes it.
        /// </summary>
        /// <param name="ent">The entity type to assign to control points</param>
        /// <returns>The number of points added to the map</returns>
        int Save(IEntity ent)
        {
            GetControlOperation save = null;

            try
            {
                m_Cmd.ActiveDisplay.MapPanel.Cursor = Cursors.WaitCursor;

                // Create import operation.
                CadastralMapModel map = CadastralMapModel.Current;
                save = new GetControlOperation();

                // Tell each range to add itself to the map.
                foreach (ControlRange r in m_Ranges)
                    r.Save(save, ent);

                // Execute the op
                save.Execute();
                return save.Count;
            }

            catch (Exception ex)
            {
                Session.WorkingSession.Remove(save);
                MessageBox.Show(ex.Message);
                return -1;
            }

            finally
            {
                m_Cmd.ActiveDisplay.MapPanel.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Loads control data from external file (based on the ranges currently defined
        /// in <see cref="m_Ranges"/>)
        /// </summary>
        /// <returns>The window of the loaded data (null if an error is reported)</returns>
        IWindow LoadControl()
        {
            // Get the name of the control file.
            string fspec = controlFileTextBox.Text;

            // Open the control file and scan through it. For each line, try to form
            // a control object. If successful, scan the array of control ranges we have
            // to try to find a match.

            try
            {
                // Initialize the window of loaded data
                Window win = new Window();

                using (StreamReader sr = File.OpenText(fspec))
                {
                    string str;
                    while ((str = sr.ReadLine()) != null)
                    {
                        ControlPoint control;
                        if (ControlPoint.TryParse(str, out control))
                        {
                            foreach (ControlRange r in m_Ranges)
                            {
                                if (control.IsInRange(r))
                                {
                                    r.Insert(control);
                                    win.Union(control);
                                    break;
                                }
                            }
                        }
                    }
                }

                return win;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Redraws any control points.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            foreach (ControlRange r in m_Ranges)
                r.Render(display, style);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If the map initially had an undefined extent, set it back that way.
            //if (m_NewMap)
            //    CadastralMapModel.Current.Extent = 

            m_Cmd.DialAbort(this);
        }

        /// <summary>
        /// Asks the user if they want to load control inside current draw window. If so,
        /// the control file is scanned to obtain all points inside the window.
        /// </summary>
        /// <returns>True if process ran to completion. False if the user indicated that
        /// they don't want to load points inside the current window, or some error
        /// was reported during the scan.</returns>
        bool GetInsideWindow()
        {
            // Ask the user whether they want the data inside the current draw window.
            if (MessageBox.Show("Load control inside current map window?", "No range specified",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return false;

            // Get the current display window.
            IWindow drawin = EditingController.Current.ActiveDisplay.Extent;

            try
            {
                // Scan the control file. For each line, try to form a control object. If
                // successful, see if it falls within the current draw window.

                string fspec = controlFileTextBox.Text;
                ControlRange range = null;
                ControlPoint control;
                int nfound = 0;

                using (StreamReader sr = File.OpenText(fspec))
                {
                    string str;
                    while ((str = sr.ReadLine()) != null)
                    {
                        if (ControlPoint.TryParse(str, out control) && drawin.IsOverlap(control))
                        {
                            nfound++;

                            // If a control range is currently defined, but the
                            // control point cannot be appended, close the range.
                            if (range != null && !range.CanAppend(control))
                                range = null;

                            // If there is no control range, create a new one.
                            if (range == null)
                                range = AddRange();

                            // Append the control point to the current range.
                            range.Append(control);
                        }
                    }
                }

                // Show the results
                ShowRanges(drawin);
                if (nfound==0)
                    MessageBox.Show("No control in current window");

                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Gets another control range
        /// </summary>
        /// <returns>The newly created range (with everything set to null)</returns>
        ControlRange AddRange()
        {
            ControlRange r = new ControlRange();
            m_Ranges.Add(r);
            return r;
        }
    }
}