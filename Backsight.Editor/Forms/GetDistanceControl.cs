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
using System.Diagnostics;
using System.Drawing;

using Backsight.Environment;
using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdGetDist" />
    /// <summary>
    /// Dialog for getting the user to specify a distance (for use with
    /// Intersection edits).
    /// </summary>
    partial class GetDistanceControl : UserControl
    {
        #region Class data

        // Data for operation ...

        /// <summary>
        /// The point the distance was observed from.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// Observed distance (either m_Distance or m_OffsetPoint).
        /// </summary>
        Observation m_ObservedDistance; // was m_pDistance

        /// <summary>
        /// Type of line (if any).
        /// </summary>
        IEntity m_LineType;

        // View-related ...

        /// <summary>
        /// Currently displayed circle.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Point used to specify distance.
        /// </summary>
        PointFeature m_DistancePoint;

        /// <summary>
        /// Entered distance value (if specified that way).
        /// </summary>
        Distance m_Distance;

        /// <summary>
        /// The offset point (if specified that way).
        /// </summary>
        OffsetPoint m_OffsetPoint;

        /// <summary>
        /// The field that last had the focus.
        /// </summary>
        Control m_Focus;

        #endregion

        #region Constructors

        public GetDistanceControl()
        {
            InitializeComponent();

            m_From = null;
            m_ObservedDistance = null;
            m_LineType = null;
            m_Circle = null;
            m_DistancePoint = null;
            m_Distance = null;
            m_OffsetPoint = null;
            m_Focus = null;
        }

        #endregion

        /// <summary>
        /// The title that appears around the UI elements dealing with the entered distance
        /// </summary>
        public string DistanceTitle
        {
            get { return distanceGroupBox.Text; }
            set { distanceGroupBox.Text = value; }
        }

        /// <summary>
        /// Observed distance (either a <see cref="Distance"/> or an <see cref="OffsetPoint"/>).
        /// </summary>
        internal Observation ObservedDistance // was GetpDist
        {
            get { return m_ObservedDistance; }
        }

        /// <summary>
        /// The point the distance was observed from.
        /// </summary>
        internal PointFeature From
        {
            get { return m_From; }
        }

        /// <summary>
        /// Type of line (if any).
        /// </summary>
        internal IEntity LineType
        {
            get { return m_LineType; }
        }

        internal void InitializeControl(IntersectForm parent, int distNum)
        {
            // Initialize combo box with a list of all line entity types
            // for the currently active editing layer.
            lineTypeComboBox.Load(SpatialType.Line);

            // If we are updating a feature that was previously created,
            // load the original info. For distance-distance intersections,
            // we need to know which page this is, to determine whether we
            // should display info for the 1st or 2nd distance.
            IntersectOperation op = parent.GetUpdateOp();
            ShowUpdate(op, distNum);

            // Go to the first text box
            fromPointTextBox.Focus();
        }

        private void distanceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (distanceTextBox.Text.Trim().Length==0)
            {
                // If we already had direction info, reset it.
                SetNormalColor(m_DistancePoint);
                m_DistancePoint = null;
                m_Distance = null;
                OnNewDistance();
            }
            else
            {
                // The distance could have been specified by the user, or it could have been
                // set as the result of a pointing operation. In the latter case, m_DistancePoint
                // will be defined.

                if (m_DistancePoint==null)
                {
                    // Explicitly entered by the user, so parse the distance.
                    ParseDistance();
                }
            }
        }

        private void distanceTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = distanceTextBox;
        }

        private void distanceTextBox_Leave(object sender, EventArgs e)
        {
            // No validation if the distance is being specified by pointing (in
            // that case, losing the focus is ok.
            if (m_DistancePoint!=null)
            {
                OnNewDistance();
                return;
            }

            // Return if the field is empty.
            if (distanceTextBox.Text.Trim().Length==0)
            {
                OnNewDistance();
                return;
            }

            // Parse the distance.
            ParseDistance();
        }

        private void fromPointTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the field is now empty, ensure that the from point
            // is undefined.
            if (fromPointTextBox.Text.Trim().Length==0)
            {
                SetNormalColor(m_From);
                m_From = null;
                OnNewDistance();
            }
            else if (m_From==null)
            {
                MessageBox.Show("You can only specify the from-point by pointing at the map.");
            }
        }

        private void fromPointTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = fromPointTextBox;
        }

        private void fromPointTextBox_Leave(object sender, EventArgs e)
        {
            // See if a new circle should be drawn.
	        OnNewDistance();	
        }

        private void lineTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Get the new selection (if any)
            m_LineType = lineTypeComboBox.SelectedEntityType;

            // If we have everything we need, move directly to the
            // next page. Otherwise move to the first field we need.
            if (m_From==null)
                fromPointTextBox.Focus();
            else if (m_Distance==null)
                distanceTextBox.Focus();
            else
            {
                IntersectForm dial = (this.ParentForm as IntersectForm);
                dial.AdvanceToNextPage();
            }
        }

        /// <summary>
        /// Reacts to selection of a point on the map.
        /// </summary>
        /// <param name="point"></param>
        internal void OnSelectPoint(PointFeature point)
        {
            // Return if point is not defined.
            if (point==null)
                return;

            // Return if point is not valid.
            if (!IsPointValid())
                return;

            // Draw the point with appropriate color
            SetColor(point, m_Focus);

            // Handle the pointing, depending on what field we were last in.
            if (m_Focus == fromPointTextBox)
            {
                // Save the from point.
                SetNormalColor(m_From);
                m_From = point;

                // Display it (causes a call to OnChangeFromPoint).
                ShowKey(fromPointTextBox, m_From);

                // Move focus to the distance field.
                distanceTextBox.Focus();
            }
            else if (m_Focus == distanceTextBox)
            {
                // The distance must be getting specified by pointing
                // at an offset point.

                SetNormalColor(m_DistancePoint);
                m_DistancePoint = point;

                // Display the point number.
                distanceTextBox.Text = m_DistancePoint.FormattedKey;

                // Move focus to the line type.
                lineTypeComboBox.Focus();
            }
        }

        /// <summary>
        /// Checks if it is OK to accept a selected point in the field that last
        /// had the input focus.
        /// </summary>
        /// <returns></returns>
        bool IsPointValid()
        {
            if (m_Focus == fromPointTextBox)
            {
                // If a from point is already defined, allow the point only
                // if the user wants to change it. If the from point is not
                // already defined, the from point is always valid.

                if (m_From!=null)
                {
                    string msg = String.Empty;
                    msg += ("You have already specified the from-point." + System.Environment.NewLine);
                    msg += ("If you want to change it, erase it first.");
                    MessageBox.Show(msg);
                    return false;
                }

                return true;
            }

            if (m_Focus == distanceTextBox)
            {
                // The distance must be getting specified by pointing at an offset point.
                // Disallow if offset point has already been specified.
                if (m_DistancePoint!=null)
                {
                    MessageBox.Show("You have already specified a distance offset point.");
                    return false;
                }

                return true;
            }

            // If it's none of the above fields, a point is not valid.
            // Just return quietly, in case the user is just mucking about
            // pointing at stuff in the map window.

            return false;
        }

        /// <summary>
        /// Checks whether the current data is enough to construct a distance
        /// circle. If so, draw it.
        /// </summary>
        void OnNewDistance()
        {
            // Try to construct circle based on what's been entered
            Circle c = GetCurrentCircle();

            if (c==null)
            {
                // If we didn't get anything, but we previously had a circle, ensure
                // it gets erased.
                if (m_Circle!=null)
                {
                    m_Circle = null;
                    ErasePainting();
                }
            }
            else
            {
                // Just return if current circle matches what we've already got
                if (m_Circle!=null && CirclesMatch(m_Circle, c))
                    return;

                // Draw the new circle
                m_Circle = c;
                ErasePainting();
            }
        }

        /// <summary>
        /// Checks whether two circles match
        /// </summary>
        /// <param name="c1">The first circle to check (not null)</param>
        /// <param name="c2">The second circle to check (not null)</param>
        /// <returns>True if the circles refer to the same center point, and have the same radius</returns>
        bool CirclesMatch(Circle c1, Circle c2)
        {
            return (c1.CenterPoint == c2.CenterPoint &&
                    Math.Abs(c1.Radius - c2.Radius) < Double.Epsilon);
        }

        /// <summary>
        /// Uses the currently displayed information to try to construct a
        /// circle representing the distance.
        /// </summary>
        /// <returns>The constructed circle (null if a circle cannot be created
        /// based on the information that's currently displayed)</returns>
        Circle GetCurrentCircle()
        {
            Circle result = null;

            // Undefine the address of the relevant distance observation.
            m_ObservedDistance = null;

            if (m_From==null)
                return null;

            if (m_DistancePoint!=null || (m_Distance!=null && m_Distance.Meters>Constants.TINY))
            {
                double radius;

                // If we have an offset point, get the radius.
                if (m_DistancePoint!=null)
                    radius = Geom.Distance(m_From, m_DistancePoint);
                else
                    radius = m_Distance.Meters;

                // Construct the circle
                result = new Circle(m_From, radius);

                // Create the appropriate distance observation (this is what
                // gets picked up when we actually go to work out the
                // intersection on the last page of the intersect dialog.

                if (m_DistancePoint!=null)
                {
                    m_OffsetPoint = new OffsetPoint(m_DistancePoint);
                    m_ObservedDistance = m_OffsetPoint;
                }
                else
                    m_ObservedDistance = m_Distance;
            }

            return result;
        }

        /// <summary>
        /// Parses an explicitly entered distance. 
        /// </summary>
        /// <returns>True if distance parses ok (may be null for a blank string).</returns>
        bool ParseDistance()
        {
            // Get the entered string.
            string str = distanceTextBox.Text.Trim();

            // No distance if empty string
            if (str.Length==0)
            {
                m_Distance = null;
                return true;
            }

            // Parse the distance.
            if (!Distance.TryParse(str, out m_Distance))
            {
                MessageBox.Show("Invalid distance.");
                distanceTextBox.Focus();
                return false;
            }

            OnNewDistance();
            return true;
        }

        /// <summary>
        /// Draws the supplied point with a color that's consistent with the
        /// meaning of a control appearing on this dialog.
        /// </summary>
        /// <param name="point">The point to draw</param>
        /// <param name="field">The control the point relates to
        /// (default was the field that currently has the focus)
        /// </param>
        void SetColor(PointFeature point, Control field)
        {
            // Return if point not specified.
            if (point==null)
                return;

            // Determine the color.
            Color col;

            if (field == fromPointTextBox)
                col = Color.LightBlue;
            else if (field == distanceTextBox)
                col = Color.Yellow;
            else
                return;

            // Draw the point in the proper color.
            point.Draw(ActiveDisplay, col);
        }

        /// <summary>
        /// Ensure that a point is drawn with it's "normal" color.
        /// </summary>
        /// <param name="point">The point to set the color for.</param>
        void SetNormalColor(PointFeature point)
        {
            // Redraw in idle time
            if (point!=null)
                ErasePainting();
        }

        internal void OnDraw(PointFeature point)
        {
            if (point==null)
                OnDrawAll();
            else
            {
                if (point==m_From)
                    SetColor(m_From, fromPointTextBox);

                if (point==m_DistancePoint)
                    SetColor(m_DistancePoint, distanceTextBox);
            }
        }

        /// <summary>
        /// Handles any redrawing. This just ensures that points are drawn in the right
        /// color, and that any distance circle shown is still there.
        /// </summary>
        internal void OnDrawAll()
        {
            // Draw any currently selected points
            SetColor(m_From, fromPointTextBox);
            SetColor(m_DistancePoint, distanceTextBox);

            // Draw any current circle
            if (m_Circle!=null)
                m_Circle.Render(ActiveDisplay, new DottedStyle());
        }

        ISpatialDisplay ActiveDisplay
        {
            get { return EditingController.Current.ActiveDisplay; }
        }

        /// <summary>
        /// Initialize for an update (or recall)
        /// </summary>
        /// <param name="op">The edit that is being updated or recalled</param>
        /// <param name="distNum">The sequence number of the distance involved (relevant only for
        /// a <see cref="IntersectTwoDistancesOperation"/>)</param>
        internal void ShowUpdate(IntersectOperation op, int distNum)
        {
            // Return if no update object (and no recall op).
            if (op==null)
                return;

            // Populate the dialog, depending on what sort of operation we have.
            if (op.EditId == EditingActionId.DistIntersect)
            {
                Debug.Assert(distNum==1 || distNum==2);
                IntersectTwoDistancesOperation oper = (IntersectTwoDistancesOperation)op;

                if (distNum==1)
                    ShowDistance(oper.Distance1FromPoint, oper.Distance1, oper.CreatedLine1);
                else
                    ShowDistance(oper.Distance2FromPoint, oper.Distance2, oper.CreatedLine2);
            }
            else if (op.EditId == EditingActionId.DirDistIntersect)
            {
                IntersectDirectionAndDistanceOperation oper = (IntersectDirectionAndDistanceOperation)op;
                ShowDistance(oper.DistanceFromPoint, oper.Distance, oper.CreatedDistanceLine);
            }
            else
            {
                MessageBox.Show("GetDistanceControl.ShowUpdate - Unexpected editing operation");
            }
        }

        /// <summary>
        /// Displays info for a specific distance observation.
        /// </summary>
        /// <param name="from">The point the distance was observed from.</param>
        /// <param name="obsv">The distance observation (either a <see cref="Distance"/>,
        /// or an <see cref="OffsetPoint"/>)</param>
        /// <param name="line">The line to show (null if no line).</param>
        void ShowDistance(PointFeature from, Observation obsv, LineFeature line)
        {
            // If we have an arc, define its entity type and scroll the
            // entity combo box to that type. Note that when the string
            // is selected, it is important that m_From and m_Distance
            // are null; otherwise <mf CdGetDist::OnSelChangeLineType>
            // may automatically move on to the next page of the wizard
            // dialog.

            m_From = null;
            m_Distance = null;
            m_LineType = null;

            if (line!=null)
            {
                m_LineType = line.EntityType;
                lineTypeComboBox.SelectEntity(m_LineType);
            }

            // Define the from-point of the distance
            m_From = from;
            fromPointTextBox.Text = m_From.FormattedKey;

            // What sort of distance observation do we have?
            m_ObservedDistance = null; // m_pDistance
            m_DistancePoint = null;
            m_Distance = null;

            if (obsv is Distance)
            {
                // Display the distance in the original data entry units.
                Distance dist = (obsv as Distance);
                m_Distance = new Distance(dist);
                distanceTextBox.Text = m_Distance.Format();

                // Create the appropriate distance observation (this is what
                // gets picked up when we actually go to work out the intersection.
                // (was on the last page of the property sheet -- see CdIntersectTwo)
                m_ObservedDistance = m_Distance;
            }
            else
            {
                // It SHOULD be an offset point.
                OffsetPoint off = (obsv as OffsetPoint);
                if (off==null)
                    throw new Exception("GetDistanceControl.ShowDistance - Unexpected type of distance.");

                // Get the point involved & display it's ID.
                m_DistancePoint = off.Point;
                distanceTextBox.Text = m_DistancePoint.FormattedKey;

                // Create the appropriate distance observation
                m_OffsetPoint.Point = m_DistancePoint;
                m_ObservedDistance = m_OffsetPoint;
            }
        }

        /// <summary>
        /// Indicates that any painting previously done by a command should be erased. This
        /// tells the command's active display that it should revert the display buffer to
        /// the way it was at the end of the last draw from the map model. Given that a command
        /// supports painting, it's <c>Paint</c> method will be called during idle time.
        /// </summary>
        internal void ErasePainting()
        {
            EditingController.Current.ActiveDisplay.RestoreLastDraw();
        }

        IntersectOperation GetUpdateOp()
        {
            IntersectForm parent = (this.ParentForm as IntersectForm);
            Debug.Assert(parent!=null);
            return parent.GetUpdateOp();
        }

        void ShowKey(TextBox tb, PointFeature point)
        {
            string keystr = String.Empty;
            if (point!=null)
            {
                keystr = point.FormattedKey;
                if (keystr.Length==0)
                    keystr = "+";
            }

            tb.Text = keystr;
        }
    }
}
