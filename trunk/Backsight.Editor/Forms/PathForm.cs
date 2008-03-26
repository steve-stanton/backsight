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
using System.Collections.Generic;
using System.Drawing;

using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdPath"/>
    /// <summary>
    /// Dialog for a new connection path.
    /// </summary>
    partial class PathForm : Form
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        readonly PathUI m_Command;

        /// <summary>
        /// The start of the path.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The end of the path.
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The control that last had the focus.
        /// </summary>
        Control m_Focus;

        /// <summary>
        /// True if user pointer to obtain the start of the path.
        /// </summary>
        bool m_FromPointed;

        /// <summary>
        /// True if user pointer to obtain the end of the path.
        /// </summary>
        bool m_ToPointed;

        /// <summary>
        /// Parsed path items
        /// </summary>
        List<PathItem> m_Items;

        /// <summary>
        /// Does the last parsed item signify an omitted point?
        /// </summary>
        bool m_Omit;

        /// <summary>
        /// The path created from the items.
        /// </summary>
        PathData m_PathData;

        /// <summary>
        /// True if the path has been drawn.
        /// </summary>
        bool m_DrawPath;

        /// <summary>
        /// Current dialog for closure info.
        /// </summary>
        AdjustmentForm m_Adjustment;

        /// <summary>
        /// The current data entry units
        /// </summary>
        DistanceUnit m_Units;

        #endregion

        #region Constructors

        internal PathForm(PathUI command, PointFeature from, PointFeature to)
        {
            InitializeComponent();

            m_Command = command;
            m_From = from;
            m_To = to;

            SetZeroValues();
        }
    
        #endregion

        void SetZeroValues()
        {
	        m_Focus			= null;
	        m_FromPointed	= false;
	        m_ToPointed		= false;
	        m_Items			= null;
            m_Omit          = false;
	        m_PathData		= null;
	        m_DrawPath		= false;
	        m_Adjustment	= null;
            m_Units         = null;
        }

        private void PathForm_Shown(object sender, EventArgs e)
        {
            // Display at top left corner of the screen.
            this.Location = new Point(0, 0);

            // If we are recalling an old operation, fill in the
            // data entry string.
            PathOperation op = null;
            if (m_Command != null)
                op = (m_Command.Recall as PathOperation);
            ShowInput(op);

            // Display the current default units.
            m_Units = CadastralMapModel.Current.EntryUnit;
            if (m_Units == null)
                defaultUnitsLabel.Text = String.Empty;
            else
                defaultUnitsLabel.Text = String.Format("{0}... ", m_Units.Abbreviation);

            // Disable the "Preview" & "OK" buttons. They are only valid
            // after the from- and to-points have been entered, and after
            // something has been entered for the path.
            this.NoPreview();

            // If we already have a from and a to point, fill them
            // in and start in the data entry field.
            if (m_From!=null && m_To!=null)
            {
                m_Focus = fromTextBox;
                OnSelectPoint(m_From);

                m_Focus = toTextBox;
                OnSelectPoint(m_To);
            }
        }

        /// <summary>
        /// Saves the path. This method is called from <see cref="AdjustmentForm"/>
        /// when the user clicks the "OK" button.
        /// </summary>
        internal void Save()
        {
            // There SHOULD be a path to save.
            if (m_PathData==null)
            {
                MessageBox.Show("PathForm.Save -- nothing to save");
                return;
            }

            // Ensure adjustment dialog has been destroyed.
            OnDestroyAdj();

            // Execute the edit
            PathOperation op = null;

            try
            {
                op = new PathOperation(m_PathData);
                op.Execute();
                Finish();
            }

            catch (Exception ex)
            {
                Session.CurrentSession.Remove(op);
                MessageBox.Show(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If we are showing adjustment results, get rid of them
            if (m_Adjustment!=null)
            {
                m_Adjustment.Dispose();
                m_Adjustment = null;
            }

            // Tell the command that's running this dialog that we're done
            if (m_Command!=null)
                m_Command.DialAbort(this);
        }

        /// <summary>
        /// Tells the command that's running this dialog that we're finished
        /// </summary>
        void Finish()
        {
            if (m_Command!=null)
                m_Command.DialFinish(this);
        }

        /// <summary>
        /// Performs processing upon selection of a new point (indicated by pointing at the map)
        /// </summary>
        /// <param name="point">The point that has been selected</param>
        internal void OnSelectPoint(PointFeature point)
        {
            // Return if point is not defined.
            if (point==null)
                return;

            SetColor(point, null);

            // Handle the pointing, depending on what field we were last in.
            if (Object.ReferenceEquals(m_Focus, fromTextBox))
            {
                // Ensure that any previously selected backsight reverts
                // to its normal color and remember the new point.
                if (!Object.ReferenceEquals(point, m_From))
                {
                    SetNormalColor(m_From);
                    m_From = point;
                }

                // Remember that the user pointed.
                m_FromPointed = true;

                // Display any point key
                fromTextBox.Text = GetPointString(m_From);

                // See if preview button can be enabled.
                CheckPreview();

                // Move focus to the to-point.
                toTextBox.Focus();
            }
            else if (Object.ReferenceEquals(m_Focus, toTextBox))
            {
                // Save the to point, and remember that the user pointed.
                if (!Object.ReferenceEquals(point, m_To))
                {
                    SetNormalColor(m_To);
                    m_To = point;
                }

                m_ToPointed = true;

                // Display any point key
                toTextBox.Text = GetPointString(m_To);

                // See if preview button can be enabled.
                CheckPreview();

                // Move focus to the data field, making sure that nothing
                // is initially selected.
                GotoPath();
            }
        }

        /// <summary>
        /// Ensures a point is drawn in its normal color.
        /// </summary>
        /// <param name="point">The point that needs to be drawn normally</param>
        void SetNormalColor(PointFeature point)
        {
            // Redraw in idle time
            if (point != null)
                m_Command.ErasePainting();
        }

        /// <summary>
        /// Sets color for a point. 
        /// </summary>
        /// <param name="point">The point to draw.</param>
        /// <param name="c">The field that the point relates to. The default is
        /// the field that currently has the focus.</param>
        void SetColor(PointFeature point, Control c)
        {
            // Return if point not specified.
            if (point==null)
                return;

            ISpatialDisplay display = m_Command.ActiveDisplay;
            Control field = (c == null ? m_Focus : c);

            if (Object.ReferenceEquals(field, fromTextBox))
                point.Draw(display, Color.DarkBlue);
            else if (Object.ReferenceEquals(field, toTextBox))
                point.Draw(display, Color.LightBlue);
        }

        /// <summary>
        /// Does any painting that this dialog does.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal void Render(ISpatialDisplay display)
        {
            // Draw any currently selected points & any direction.
            SetColor(m_From, fromTextBox);
            SetColor(m_To, toTextBox);

            if (m_PathData!=null && m_DrawPath)
                m_PathData.Render(display);
        }

        private void fromTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = fromTextBox;
        }

        private void toTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = toTextBox;
        }

        private void distanceButton_Click(object sender, EventArgs e)
        {
            // Display distance dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.

            DistanceForm dial = new DistanceForm();
            if (dial.ShowDialog() == DialogResult.OK)
                ReplaceSel(dial.Format());

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        private void fromTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the field is now empty, allow the user to type in an ID.
            if (fromTextBox.Text.Trim().Length == 0)
            {
                SetNormalColor(m_From);
                m_FromPointed = false;
                m_From = null;
                NoPreview();
            }
        }

        private void fromTextBox_Leave(object sender, EventArgs e)
        {
            // Just return if the user specified the field by pointing.
            if (m_FromPointed)
                return;

            // Return if the field is empty.
            string str = fromTextBox.Text.Trim();
            if (str.Length == 0)
                return;

            // Parse the ID value.
            uint idnum;
            if (!UInt32.TryParse(str, out idnum))
            {
                MessageBox.Show("Invalid point ID");
                fromTextBox.Focus();
                return;
            }

            // Ask the map to locate the specified point.
            CadastralMapModel map = CadastralMapModel.Current;
            m_From = new FindPointByIdQuery(map.Index, idnum.ToString()).Result;
            if (m_From == null)
            {
                MessageBox.Show("No point with specified key.");
                fromTextBox.Focus();
                return;
            }

            // Display the point in the correct color.
            SetColor(m_From, fromTextBox);
            CheckPreview();
        }

        private void toTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the field is now empty, allow the user to type in an ID.
            if (toTextBox.Text.Trim().Length == 0)
            {
                SetNormalColor(m_To);
                m_To = null;
                m_ToPointed = false;
                NoPreview();
            }
        }

        private void toTextBox_Leave(object sender, EventArgs e)
        {
            // Just return if the user specified the field by pointing.
            if (m_ToPointed)
                return;

            // Return if the field is empty.
            string str = toTextBox.Text.Trim();
            if (str.Length == 0)
                return;

            // Parse the ID value.
            uint idnum;
            if (!UInt32.TryParse(str, out idnum))
            {
                MessageBox.Show("Invalid point ID");
                fromTextBox.Focus();
                return;
            }

            // Ask the map to locate the specified point.
            CadastralMapModel map = CadastralMapModel.Current;
            m_To = new FindPointByIdQuery(map.Index, idnum.ToString()).Result;
            if (m_To == null)
            {
                MessageBox.Show("No point with specified key.");
                toTextBox.Focus();
                return;
            }

            // Display the point in the correct color.
            SetColor(m_To, toTextBox);
            CheckPreview();
        }

        private void angleButton_Click(object sender, EventArgs e)
        {
            // Display angle dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.

            AngleForm dial = new AngleForm();
            if (dial.ShowDialog() == DialogResult.OK)
                ReplaceSel(dial.Format());

            dial.Dispose();

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        private void culDeSacButton_Click(object sender, EventArgs e)
        {
            // Display cul-de-sac dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.
            CulDeSacForm dial = new CulDeSacForm();
            if (dial.ShowDialog() == DialogResult.OK)
                ReplaceSel(dial.Format());

            dial.Dispose();

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        void ReplaceSel(string s)
        {
            // This is meant to do the same as the MFC CEdit::ReplaceSel method...
            Clipboard.SetText(s);
            pathTextBox.Paste();
        }

        private void curveButton_Click(object sender, EventArgs e)
        {
            // Display curve dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.

            ArcForm dial = new ArcForm();
            if (dial.ShowDialog() == DialogResult.OK)
                ReplaceSel(dial.Format());

            dial.Dispose();

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            // If the entered path parses ok, do the adjustment
            if (ParsePath())
                Adjust();
        }

        private void endCurveButton_Click(object sender, EventArgs e)
        {
            // Stick an /EC at the end of the edit box.
            ReplaceSel(") ");
        }

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            // If we previously had an adjustment dialog, cancel it.
            // This should also end up calling this->OnDestroyAdj

            if (m_Adjustment != null)
            {
                m_Adjustment.Dispose();
                m_Adjustment = null;
            }

            // Ignore any path data that remains.
            m_PathData = null;

            // The preview button should be enabled only if there is
            // something defined for the path.
            CheckPreview();
        }

        void CheckPreview()
        {
            previewButton.Enabled = (m_To != null &&
                                     m_From != null &&
                                     pathTextBox.Text.Trim().Length > 0);
        }

        void NoPreview()
        {
            previewButton.Enabled = false;
        }

        /// <summary>
        /// Sets focus to the path field, ensuring that nothing is
        /// selected when the focus gets there.
        /// </summary>
        void GotoPath()
        {
            // Set selection to the character after any current selection.
            int nSel = pathTextBox.SelectionLength;
            if (nSel>0)
            {
                int istart = pathTextBox.SelectionStart;
                pathTextBox.SelectionLength = 0;
                pathTextBox.SelectionStart = istart + nSel;
            }

            // Move the focus.
            pathTextBox.Focus();

            // If there's only one line in the path control, scroll
            // to the end of the line.
            if (pathTextBox.Lines.Length == 1)
            {
                pathTextBox.SelectionLength = 0;
                pathTextBox.SelectionStart = pathTextBox.Lines[0].Length;
            }
        }

        bool ParsePath()
        {
            // How many characters have we got (this may include extra junk
            // on each line of the control).
            int nchars = pathTextBox.Text.Length;
            if (nchars < 0)
                return false;

            // Grab the entered path (without any embedded newlines) - not sure
            // if they're there or not
            string str = pathTextBox.Text;
            str = str.Replace(System.Environment.NewLine, " ").Trim();

            return ParseString(str);
        }

        /// <summary>
        /// Parses the string that represents the path description.
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <returns>True if the supplied string is understandable</returns>
        bool ParseString(string str)
        {
            // Initialize list of path items.
            m_Items = new List<PathItem>();

            // Pick out each successive word (delimited by white space).
            string[] words = str.Split(new char[] { '\t', ' ' });
            foreach (string w in words)
            {
                if (!ParseWord(w))
                    return false;
            }

            // Validate the path items
            return ValidPath();
        }

        bool ParseWord(string str)
        {
            // Return if string is empty (could be empty if this function
            // has been called recursively from below).
            str = str.Trim();
            int nc = str.Length;
            if (nc == 0)
                return true;

            // If we have a new default units specification, make it
            // the default. There should be whitespace after the "..."
            if (str.Contains("..."))
            {
                DistanceUnit unit = GetUnits(str, true);
                PathItem item = new PathItem(PathItemType.Units, unit, 0.0);
                AddItem(item);
                return true;
            }

            // If we have a counter-clockwise indicator, just remember it
            // and parse anything that comes after it.
            if (nc >= 2 && String.Compare(str.Substring(0, 2), "cc", true) == 0)
            {
                AddItem(PathItemType.CounterClockwise);
                return ParseWord(str.Substring(2));
            }

            // If we have a BC, remember it & parse anything that follows.
            if (str[0] == '(')
            {
                AddItem(PathItemType.BC);
                return ParseWord(str.Substring(1));
            }

            // If we have a EC, remember it & parse anything that follows.
            if (str[0] == ')')
            {
                AddItem(PathItemType.EC);
                return ParseWord(str.Substring(1));
            }

            // If we have a single slash character (possibly followed by
            // a digit or a decimal point), record the single slash &
            // parse anything that follows.

            if (str[0] == '/')
            {
                // Check for a free-standing slash, or a slash that is
                // followed by a numeric digit or decimal point.

                if (nc == 1 || Char.IsDigit(str, 1) || str[1] == '.')
                {
                    AddItem(PathItemType.Slash);
                    return ParseWord(str.Substring(1));
                }

                // More than one character, or what follows is not a digit.
                // So we are dealing with either a miss-connect, or an
                // omit-point. In either case, there should be whitespace
                // after that.

                if (nc == 2)
                {
                    if (str[1] == '-')
                    {
                        AddItem(PathItemType.MissConnect);
                        return true;
                    }

                    if (str[1] == '*')
                    {
                        AddItem(PathItemType.OmitPoint);
                        return true;
                    }
                }
                // Allow CADCOR-style data entry
                else if (nc == 3)
                {
                    if (String.Compare(str, "/mc", true) == 0)
                    {
                        AddItem(PathItemType.MissConnect);
                        return true;
                    }

                    if (String.Compare(str, "/op", true) == 0)
                    {
                        AddItem(PathItemType.OmitPoint);
                        return true;
                    }
                }

                string msg = String.Format("Unexpected qualifier '{0}'", str);
                MessageBox.Show(msg);
                return false;
            }

            // If we have a multiplier, it must be immediately followed
            // by a numeric (integer) value.

            if (str[0] == '*')
            {
                if (nc == 1)
                {
                    MessageBox.Show("Unexpected '*' character");
                    return false;
                }

                // Pick up the repeat count (not sure if the digits need to be
                // followed by white space, or whether non-numeric digits are valid,
                // so pick up only the digits).
                string num = GetIntDigits(str.Substring(1));

                // Error if repeat count is less than 2.
                int repeat;
                if (!Int32.TryParse(num, out repeat) || repeat < 2)
                {
                    string msg = String.Format("Unexpected repeat count in '{0}'", str);
                    MessageBox.Show(msg);
                    return false;
                }

                if (repeat < 2)
                {
                    string msg = String.Format("Unexpected repeat count in '{0}'", str);
                    MessageBox.Show(msg);
                    return false;
                }

                // Duplicate the last item using the repeat count.
                AddRepeats(repeat);

                // Continue parsing after the repeat count.
                return ParseWord(str.Substring(1+num.Length));
            }

            // If the string contains an embedded qualifier (a "*" or a "/"
            // character), process the portion of any string prior to the
            // qualifier. Note that we have just handled the cases where
            // the qualifier was at the very start of the string.

            int starIndex = str.IndexOf('*');
            int slashIndex = str.IndexOf('/');

            if (starIndex >= 0 || slashIndex >= 0)
            {
                int qualIndex = starIndex;
                if (qualIndex < 0 || qualIndex > slashIndex)
                    qualIndex = slashIndex;

                // Process the stuff prior to the qualifier.
                string copy = str.Substring(0, qualIndex);
                if (!ParseWord(copy))
                    return false;

                // Process the stuff, starting with the qualifier character
                return ParseWord(str.Substring(qualIndex));
            }

            // Process this string. We should have either a value or an angle.
            if (str.IndexOf('-') >= 0 || IsLastItemBC())
            {
                // If the string contains a "c" character, it's a central
                // angle; process the string only as far as that.
                PathItemType type = PathItemType.Angle;

                int caIndex = str.ToUpper().IndexOf('C');
                if (caIndex>=0)
                {
                    str = str.Substring(0, caIndex);
                    type = PathItemType.CentralAngle;
                }
                else
                {
                    // Check if it's a deflection (if so, strip out the "d").
                    int dIndex = str.ToUpper().IndexOf('D');
                    if (dIndex >= 0)
                    {
                        str = str.Substring(0, dIndex) + str.Substring(dIndex + 1);
                        type = PathItemType.Deflection;
                    }
                }

                // Try to parse an angular value into radians.
                double radval;
                if (RadianValue.TryParse(str, out radval))
                {
                    PathItem item = new PathItem(type, null, radval);
                    AddItem(item);
                    return true;
                }

                // Bad angle.
                string msg = String.Format("Malformed angle '{0}'", str);
                MessageBox.Show(msg);
                return false;
            }
            else
            {
                // Get the current distance units.
                DistanceUnit unit = GetUnits(null, false);

                // Grab characters that look like a floating point number
                string num = GetDoubleDigits(str);
                double val;
                if (!Double.TryParse(num, out val))
                {
                    string msg = String.Format("Malformed value '{0}'", str);
                    MessageBox.Show(msg);
                    return false;
                }

                // If we didn't get right to the end, we may have distance units,
                // or the ")" character indicating an EC.
                if (num.Length < str.Length && str[num.Length] != ')')
                {
                    unit = GetUnits(str.Substring(num.Length), false);
                    if (unit == null)
                    {
                        string msg = String.Format("Malformed value '{0}'", str);
                        MessageBox.Show(msg);
                        return false;
                    }
                }

                PathItem item = new PathItem(PathItemType.Value, unit, val);
                AddItem(item);

                if (str.Length>num.Length && str[num.Length] == ')')
                    return ParseWord(str.Substring(num.Length));
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns the numeric digits (if any) at the start of a string
        /// </summary>
        /// <param name="str">The string that should be starting with some digits (e.g. 1234abc)</param>
        /// <returns>The leading numeric digits (blank if the first character in the supplied
        /// string is not a digit)</returns>
        string GetIntDigits(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsDigit(str, i))
                {
                    if (i == 0)
                        return String.Empty;
                    else
                        return str.Substring(0, i);
                }
            }

            // The entire string consists of numeric digits (or it's empty)
            return str;
        }

        /// <summary>
        /// Returns a portion of a string that contains numeric characters
        /// </summary>
        /// <param name="s">The string that starts with a numeric substring</param>
        /// <returns>The numeric string starting at the supplied index (a
        /// blank string if the character at that position is not a number,
        /// a period, or a minus sign).</returns>
        /// <remarks>This may not handle i18n (e.g. decimal places may
        /// actually be commas).</remarks>
        string GetDoubleDigits(string s)
        {
            int nChar = 0;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                // Allow '-' character only at the start
                if ((c == '-' && i == 0) || c == '.' || Char.IsNumber(c))
                    nChar++;
                else
                    break;
            }

            if (nChar == 0)
                return String.Empty;

            return s.Substring(0, nChar);
        }

        /// <summary>
        /// Repeats the last path item a specific number of times. The thing to
        /// repeat HAS to be of type PAT_VALUE (or possibly a PAT_MC that has been
        /// automatically inserted by <see cref="AddItem"/>).
        /// </summary>
        /// <param name="repeat">The number of times the last item should appear (we
        /// will append n-1 copies of the last item).</param>
        void AddRepeats(int repeat)
        {
            // Confirm that we have something to repeat.
            if (m_Items.Count == 0)
            {
                MessageBox.Show("Nothing to repeat");
                return;
            }

            // If the last item was a PAT_MC, get to the value before that.
            int prev = m_Items.Count - 1;
            PathItemType type = m_Items[prev].ItemType;
            if (type == PathItemType.MissConnect && prev > 0)
            {
                prev--;
                type = m_Items[prev].ItemType;
            }

            // It can only be a PAT_VALUE.
            if (type != PathItemType.Value)
            {
                MessageBox.Show("Unexpected repeat multiplier");
                return;
            }

            // Make copies of the last value.
            for (int i = 1; i < repeat; i++)
            {
                PathItem copy = new PathItem(m_Items[prev]);
                AddItem(copy);
            }
        }

        /// <summary>
        /// Gets or sets the units for distance observations.
        /// </summary>
        /// <param name="str">The string containing the units specification. The
        /// characters up to the first white space character (or "." character) must
        /// match one of the abbreviations for the desired units. Pass in a null
        /// pointer (the default) if you just want to get the current default
        /// units.</param>
        /// <param name="makedef">True if the units obtained should be regarded as
        /// the new default. (default was false).</param>
        /// <returns>The corresponding units.</returns>
        DistanceUnit GetUnits(string str, bool makedef)
        {
            // Get pointer to the enclosing map.
            CadastralMapModel map = CadastralMapModel.Current;

            if (str != null)
            {
                // Pick up characters that represent the abbreviation for
                // the units. Break on any white space or a "." character.

                string abbrev = String.Empty;
                foreach (char c in str)
                {
                    if (Char.IsWhiteSpace(c) || c == '.')
                        break;

                    abbrev += c;
                }

                // Try to match the abbreviation to one of the unit
                // types known to the map.
                DistanceUnit match = MatchUnits(abbrev);

                // Issue message if there was no match.
                if (match == null)
                {
                    string msg = String.Format("No units with abbreviation '{0}'", abbrev);
                    MessageBox.Show(msg);
                }

                // If the units should be made the new default, do it so
                // long as the units were obtained.
                if (makedef && match != null)
                    m_Units = match;

                // Return the units (if any)
                return match;
            }
            else
            {
                // If the default was not previously defined, pick up
                // the map's current default.
                if (m_Units == null)
                    m_Units = map.EntryUnit;

                return m_Units;
            }
        }

        /// <summary>
        /// Converts a string that represents a distance unit abbreviation into
        /// a DistanceUnit reference (to one of the objects known to the enclosing map).
        /// </summary>
        /// <param name="abbrev">The units abbreviation.</param>
        /// <returns>The corresponding units (null if the abbreviation was not found).</returns>
        DistanceUnit MatchUnits(string abbrev)
        {
            CadastralMapModel map = CadastralMapModel.Current;
            return map.GetUnit(abbrev);
        }

        /// <summary>
        /// Holds on to an additional path item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddItem(PathItem item)
        {
            if (m_Items == null)
                m_Items = new List<PathItem>();

            // If no items have been added, ensure omitted flag has been
            // freshly initialized.
            if (m_Items.Count == 0)
                m_Omit = false;

            // Ignore an attempt to add 2 miss-connects in a row (ValidPath
            // will complain).
            PathItemType type = item.ItemType;
            if (m_Items.Count > 0 &&
                type == PathItemType.MissConnect &&
                m_Items[m_Items.Count - 1].ItemType == PathItemType.MissConnect)
                return;

            // Add the supplied item into the list.
            m_Items.Add(item);

            // If we have just appended a PAT_VALUE, append an additional
            // miss-connect item if we previously omitted a point.
            if (m_Omit && type == PathItemType.Value)
            {
                m_Omit = false;
                AddItem(PathItemType.MissConnect);
            }

            // Remember whether we just omitted a point.
            if (type == PathItemType.OmitPoint)
                m_Omit = true;
        }

        /// <summary>
        /// Holds on to an additional path item. Good for items that
        /// do not have an associated value.
        /// </summary>
        /// <param name="type">The type of item to add.</param>
        void AddItem(PathItemType type)
        {
            PathItem item = new PathItem(type, null, 0.0);
            AddItem(item);
        }

        /*
//	@mfunc	Allocate space for a specific number of path items.
//
//	@parm	The number of items to allocate.
//
/////////////////////////////////////////////////////////////////////////////

UINT2 CdPath::SetSize ( const UINT2 newsize ) {

	UINT2 size;			// The size actually set

	if ( newsize==0 )
		size = max(1,m_NumItem);		// always get at least 1
	else if ( newsize>m_NumAlloc )
		size = newsize;					// increase
	else
		size = max(newsize,m_NumItem);	// decrease, but not too low

//	Return if the new size is identical to the current size
	if ( size == m_NumAlloc ) return size;

//	Allocate new array
	m_NumAlloc = size;
	CePathItem* newlist = new CePathItem[m_NumAlloc];

//	Copy what we have, delete the original, and then point
//	to the copy.
	if ( m_Items ) {
	  memcpy ( &newlist[0], &m_Items[0],
					m_NumItem*sizeof(CePathItem) );
	  delete [] m_Items;
	}
	m_Items = newlist;
	return m_NumAlloc;

} // end of SetSize
         */

        /// <summary>
        /// Checks if the last path item is a BC.
        /// </summary>
        /// <returns>True if the last parsed item represents the BC of a circular arc</returns>
        bool IsLastItemBC()
        {
            if (m_Items.Count == 0)
                return false;

            PathItem lastItem = m_Items[m_Items.Count - 1];
            return (lastItem.ItemType == PathItemType.BC);
        }

        /// <summary>
        /// Validates path items. Prior to call, the path should be parsed by
        /// making a series of calls to ParseWord. This generates a set of
        /// items that are generated without consideration to their context.
        /// This function validates the context, elaborating on the meaning
        /// of PAT_ANGLE and PAT_VALUE item codes.
        /// </summary>
        /// <returns></returns>
        bool ValidPath()
        {
            // The path must contain at least one item.
            if (m_Items == null || m_Items.Count == 0)
            {
                MessageBox.Show("Path has not been specified.");
                return false;
            }

            // All PAT_VALUE's outside of a curve definition should have
            // type PAT_DISTANCE. Within curves, it's a bit more complicated.

            int ibc = 0;        // Index of last BC
            bool curve = false;	// Not in a curve to start with

            for (int i = 0; i < m_Items.Count; i++)
            {
                PathItem item = m_Items[i];

                switch (item.ItemType)
                {
                    case PathItemType.BC:
                    {
                        // If we have a BC, confirm that we are not already in
                        // a curve. Also confirm that there are at least 3 items
                        // after the BC (enough for an angle, a radius, and an EC).

                        if (curve)
                        {
                            MessageBox.Show("Nested curve detected");
                            return false;
                        }
                        curve = true;

                        if ((ibc + 4) > m_Items.Count)
                        {
                            MessageBox.Show("BC not followed by angle, radius, and EC");
                            return false;
                        }

                        ibc = i;
                        break;
                    }

                    case PathItemType.EC:
                    {
                        // If we have an EC, confirm that we were in a curve.

                        if (!curve)
                        {
                            MessageBox.Show("EC was not preceded by BC");
                            return false;
                        }
                        curve = false;
                        break;
                    }

                    case PathItemType.Value:
                    {
                        // If not in a curve, change all PAT_VALUE types to
                        // PAT_DISTANCE types. Inside a curve, PAT_VALUES may
                        // actually be angles that need to be converted into
                        // radians.

                        // All values must point to the data entry units.
                        if (item.Units==null)
                        {
                            MessageBox.Show("Value has no unit of measurement");
                            return false;
                        }

                        if (!curve)
                            item.ItemType = PathItemType.Distance;
                        else
                        {
                            // The value immediately after the BC is always an angle.
                            if (i == (ibc + 1))
                            {
                                item.ItemType = PathItemType.BcAngle;
                                item.Value *= MathConstants.DEGTORAD;
                            }
                            else if (i == (ibc + 2))
                            {
                                // Could be an angle, or a radius. If the NEXT
                                // item is a value, we must have an exit angle.

                                if (m_Items[i + 1].ItemType == PathItemType.Value)
                                {
                                    item.ItemType = PathItemType.EcAngle;
                                    item.Value *= MathConstants.DEGTORAD;
                                }
                                else
                                    item.ItemType = PathItemType.Radius;
                            }
                            else if (i == (ibc + 3))
                                item.ItemType = PathItemType.Radius;
                            else
                                item.ItemType = PathItemType.Distance;
                        }

                        break;
                    }

                    case PathItemType.Deflection:
                    case PathItemType.Angle:
                    {
                        // Angles inside curve definitions have to be qualified. If
                        // they appear, they MUST follow immediately after the BC.
                        // For angles NOT in a curve, you can only have one angle
                        // at a time.

                        if (curve)
                        {
                            // Can't have deflections inside a curve.
                            if (item.ItemType == PathItemType.Deflection)
                            {
                                MessageBox.Show("Deflection not allowed within curve definition");
                                return false;
                            }

                            if (i == (ibc + 1))
                                item.ItemType = PathItemType.BcAngle;
                            else if (i == (ibc + 2))
                                item.ItemType = PathItemType.EcAngle;
                            else
                            {
                                MessageBox.Show("Extraneous angle inside curve definition");
                                return false;
                            }
                        }
                        else
                        {
                            if (i > 0 && m_Items[i-1].ItemType == PathItemType.Angle)
                            {
                                MessageBox.Show("More than 1 angle at the end of a straight");
                                return false;
                            }

                            // Also, it makes no sense to have an angle right after an EC.
                            if (i > 0 && m_Items[i - 1].ItemType == PathItemType.EC)
                            {
                                MessageBox.Show("Angle after EC makes no sense");
                                return false;
                            }
                        }

                        break;
                    }

                    case PathItemType.Slash:
                    {
                        // A free-standing slash character is only valid within
                        // a curve definition. It has to appear at a specific
                        // location in the sequence.
                        //
                        // BC -> BCAngle -> Radius -> Slash
                        // BC -> BCAngle -> Radius -> CCMarker -> Slash
                        // BC -> BCAngle -> ECAngle -> Radius -> CCMarker -> Slash
                        //
                        // In other words, it can come at ibc+3 through ibc+5.

                        if (!curve)
                        {
                            MessageBox.Show("Extraneous '/' character");
                            return false;
                        }

                        if (i < ibc + 3 || i > ibc + 5)
                        {
                            MessageBox.Show("Misplaced '/' character");
                            return false;
                        }

                        break;
                    }

                    case PathItemType.CounterClockwise:
                    {
                        // Counter-clockwise indicator. Similar to PAT_SLASH, it has
                        // a specific range of valid positions with respect to the BC.

                        if (!curve)
                        {
                            MessageBox.Show("Counter-clockwise indicator detected outside curve definition");
                            return false;
                        }

                        if (i < ibc + 3 || i > ibc + 4)
                        {
                            MessageBox.Show("Misplaced 'cc' characters");
                            return false;
                        }

                        break;
                    }

                    case PathItemType.CentralAngle:
                    {
                        // A central angle is valid only within a curve definition
                        // and must be immediately after the BC.

                        if (!curve)
                        {
                            MessageBox.Show("Central angle detected outside curve definition");
                            return false;
                        }

                        if (i != ibc + 1)
                        {
                            MessageBox.Show("Central angle does not follow immediately after BC");
                            return false;
                        }

                        break;
                    }

                    case PathItemType.MissConnect:
                    case PathItemType.OmitPoint:
                    {
                        // Miss-connections & omit points must always follow on from a PAT_DISTANCE.

                        if (i == 0 || m_Items[i - 1].ItemType != PathItemType.Distance)
                        {
                            MessageBox.Show("Miss-Connect or Omit-Point is not preceded by a distance");
                            return false;
                        }

                        break;
                    }

                    case PathItemType.Units:
                    {
                        // No checks
                        break;
                    }

                    default:
                    {
                        // All item types generated via ParseWord should have been
                        // listed above, even if there is no check. If any got missed,
                        // drop through to a message, but keep going.

                        string msg = String.Format("PathForm.ValidPath - Unhandled check for {0}", item.ItemType);
                        MessageBox.Show(msg);
                        break;
                    }

                } // end switch

            } // next item

            // Error if we got to the end, and any curve was not closed.
            if (curve)
            {
                MessageBox.Show("Circular arc does not have an EC");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adjusts a validated path.
        /// </summary>
        void Adjust()
        {
            // Confirm that the from-point & to-point are both defined (this
            // should have been checked beforehand).
            if (m_From==null || m_To==null)
            {
                MessageBox.Show("Terminal points have not been defined");
                return;
            }

            // Assign leg numbers to each path item.
            SetLegs();

            // Get rid of any path previously created.
            m_PathData = null;

            try
            {
                // Create new path data
                PathData pd = new PathData(m_From, m_To);
                pd.Create(m_Items.ToArray());
                m_PathData = pd;

                // Adjust the path

                double de;				// Misclosure in eastings
                double dn;				// Misclosure in northings
                double prec;			// Precision
                double length;			// Total observed length
                double rotation;		// Rotation for adjustment
                double sfac;			// Adjustment scaling factor

                m_PathData.Adjust(out dn, out de, out prec, out length, out rotation, out sfac);

                // Draw the path with the adjustment info
                m_DrawPath = true;
                m_PathData.Render(m_Command.ActiveDisplay);

                // Display the numeric results.
                m_Adjustment = new AdjustmentForm(dn, de, prec, length, this);
                m_Adjustment.Show();

                // Disable the preview button (it will be re-enabled when
                // the adjustment dialog is destroyed).
                previewButton.Enabled = false;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Associates each path item with a leg sequence number.
        /// </summary>
        /// <returns>The number of legs found.</returns>
        int SetLegs()
        {
            int nleg = 0;

            // Note that PathItemType.Units may not get a leg number.

            for (int i = 0; i < m_Items.Count; ) // not i++
            {
                PathItemType type = m_Items[i].ItemType;

                if (type == PathItemType.Distance ||
                    type == PathItemType.Angle ||
                    type == PathItemType.Deflection ||
                    type == PathItemType.BC)
                {
                    // If we have a distance or angle item, increment the leg 
                    // sequence number until we hit an angle or a BC. In the case of
                    // an angle, it always comes at the START of a leg.

                    if (type == PathItemType.Distance ||
                        type == PathItemType.Angle ||
                        type == PathItemType.Deflection)
                    {
                        nleg++;
                        m_Items[i].LegNumber = nleg;
                        for (i++; i < m_Items.Count; i++)
                        {
                            if (m_Items[i].ItemType == PathItemType.Angle ||
                                m_Items[i].ItemType == PathItemType.Deflection ||
                                m_Items[i].ItemType == PathItemType.BC)
                                break;

                            m_Items[i].LegNumber = nleg;
                        }
                    }
                    else
                    {
                        // We have a BC, so increment the leg sequence number
                        // & scan until we hit the EC.

                        nleg++;
                        for (; i < m_Items.Count; i++)
                        {
                            m_Items[i].LegNumber = -nleg;	// negated
                            if (m_Items[i].ItemType == PathItemType.EC)
                            {
                                i++;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    m_Items[i].LegNumber = nleg;
                    i++;
                }
            }

            return nleg;
        }

        /// <summary>
        /// Takes action when the adjustment result dialog has been cancelled (user
        /// has clicked the "Reject" button). At the time of call, the dialog
        /// is already closed, though a reference to the instance is still
        /// held by this class.
        /// </summary>
        internal void OnDestroyAdj()
        {
        }
        /*
void CdPath::OnDestroyAdj ( void ) {

//	Remember not to draw the path.
	m_DrawPath = FALSE;

//	Get rid of the dialog object.
	delete m_pAdjustment;
	m_pAdjustment = 0;

//	Re-enable the Preview button.
	TurnOn(GetDlgItem(IDC_PREVIEW));

} // end of OnDestroyAdj
        */

        /// <summary>
        /// Fills the data entry field with the stuff that was entered for
        /// a specific connection path.
        /// </summary>
        /// <param name="op">The operation to show.</param>
        void ShowInput(PathOperation op)
        {
            // Return if the operation has not been specified.
            if (op==null)
                return;

            // Get the data entry string.
            string str = op.GetString();

            // Display the data entry string (this takes care of word wrap).
            pathTextBox.Text = str;
        }

        /// <summary>
        /// Obtains the ID string for a selected point
        /// </summary>
        /// <param name="p">The point of interest</param>
        /// <returns>The ID string to display</returns>
        internal static string GetPointString(PointFeature p)
        {
            string s = p.FormattedKey;
            return (s.Length == 0 ? p.DataId : s);
        }
    }
}