// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Drawing;
using System.Diagnostics;

using Backsight.Editor.Operations;
using Backsight.Geometry;
using Backsight.Editor.Observations;
using Backsight.Editor.UI;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for the <see cref="NewCircleUI"/>
    /// </summary>
    partial class NewCircleForm : Form
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        readonly CommandUI m_Cmd;

        /// <summary>
        /// The circle (if any) that is being updated.
        /// </summary>
        //LineFeature m_Update;

        /// <summary>
        /// The control that has the focus.
        /// </summary>
        Control m_Focus;

        /// <summary>
        /// A previous operation that was recalled (always null if doing
        /// an update).
        /// </summary>
        NewCircleOperation m_Recall;

        // For the operation ...

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        PointFeature m_Center;

        /// <summary>
        /// Observed distance (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/>).
        /// </summary>
        Observation m_Radius; // was m_pRadius

        // Preview related ...

        /// <summary>
        /// Currently displayed circle.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Point used to specify distance.
        /// </summary>
        PointFeature m_RadiusPoint;

        /// <summary>
        /// Entered distance value (if specified that way).
        /// </summary>
        Distance m_RadiusDistance; // was m_Radius

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>NewCircleForm</c> for a brand new circle.
        /// </summary>
        /// <param name="cmd">The command creating this dialog</param>
        internal NewCircleForm(CommandUI cmd)
            : this(cmd, null)
        {
        }

        /// <summary>
        /// Creates a <c>NewCircleForm</c> for a circle, based on a previously
        /// defined circle.
        /// </summary>
        /// <param name="cmd">The command creating this dialog</param>
        /// <param name="recall">The editing operation that's being recalled (null
        /// if not doing a recall)</param>
        internal NewCircleForm(CommandUI cmd, Operation recall)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_Recall = (NewCircleOperation)recall;

            m_Center = null;
            m_Radius = null;
            m_Circle = null;
            m_RadiusPoint = null;
            m_RadiusDistance = null;
            m_Focus = null;
        }

        #endregion

        private void NewCircleForm_Shown(object sender, EventArgs e)
        {
            // If we are updating a circle that was previously created,
            // load the original info.

            if (m_Recall == null)
                ShowUpdate();
            else
                InitOp(m_Recall, false); // not an update
        }

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        internal PointFeature Center
        {
            get { return m_Center; }
        }

        /// <summary>
        /// Observed distance (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/>).
        /// </summary>
        internal Observation Radius
        {
            get { return m_Radius; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (m_Center != null && m_Radius != null)
                m_Cmd.DialFinish(this);
            else
            {
                if (m_Center==null)
                    MessageBox.Show("Center point has not been specified.");
                else
                    MessageBox.Show("Radius has not been specified.");
            }
        }

        private void centerTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = centerTextBox;
        }

        private void centerTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the field is now empty, ensure that the center point is undefined.
            if (centerTextBox.Text.Trim().Length==0)
            {
                SetNormalColor(m_Center);
                m_Center = null;
            }

            OnChange();
        }

        private void centerTextBox_Leave(object sender, EventArgs e)
        {
            // See if a new circle should be drawn.
            OnChange();	
        }

        private void radiusTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = radiusTextBox;
        }

        private void radiusTextBox_TextChanged(object sender, EventArgs e)
        {
            string str = radiusTextBox.Text.Trim();

            if (str.Length==0)
            {
                // If we already had radius info, reset it.
                SetNormalColor(m_RadiusPoint);
                m_RadiusPoint = null;
                m_RadiusDistance = null;
                OnChange();
            }
            else
            {
                // If the first character is a "+" character, it means the text was set
                // via a pointing operation. So if the first char is NOT a "+", treat it
                // as an entered radius.
                if (str[0]!='+')
                {
                    SetNormalColor(m_RadiusPoint);
                    m_RadiusPoint = null;
                    ParseRadius();
                }
            }
        }

        private void radiusTextBox_Leave(object sender, EventArgs e)
        {
            // No validation if the radius is being specified by pointing (in
            // that case, losing the focus is ok.
            if (m_RadiusPoint!=null)
            {
                OnChange();
                return;
            }

            // Return if the field is empty.
            string str = radiusTextBox.Text.Trim();
            if (str.Length==0)
            {
                OnChange();
                return;
            }

            // If the field starts with a "+" character, it must be
            // an explicitly entered point ID.
            if (str[0]=='+')
            {
                m_RadiusPoint = GetPoint(radiusTextBox);
                if (m_RadiusPoint==null)
                {
                    radiusTextBox.Focus();
                    return;
                }
            }

            // Parse the radius.
            ParseRadius();
        }

        /// <summary>
        /// Checks whether the current data is enough to construct a circle.
        /// If so, draws it (erasing any previously drawn circle).
        /// </summary>
        void OnChange()
        {
            // Try to construct a circle based on what's been entered
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
                if (m_Circle!=null && CircleGeometry.IsCoincident(m_Circle, c, Constants.XYRES))
                    return;

                // Draw the new circle
                m_Circle = c;
                ErasePainting();
            }
        }

        /// <summary>
        /// Uses the currently displayed information to try to construct a
        /// circle object.
        /// </summary>
        /// <returns>The constructed circle (null if a circle cannot be created
        /// based on the information that's currently displayed)</returns>
        Circle GetCurrentCircle()
        {
            Circle result = null;

            // Get rid of any previous radius observation.
            m_Radius = null;

            if (m_Center!=null && (m_RadiusPoint!=null || m_RadiusDistance!=null))
            {
                double radius;

                // If we have an offset point, get the radius.
                if (m_RadiusPoint!=null)
                    radius = Geom.Distance(m_Center, m_RadiusPoint);
                else
                    radius = m_RadiusDistance.Meters;

                // Create the circle
                result = new Circle(m_Center, radius);

                // Create the appropriate distance observation.
                // (not sure why these were needed, it certainly looks out of place as
                // part of this method)
                if (m_RadiusPoint!=null)
                    m_Radius = new OffsetPoint(m_RadiusPoint);
                else
                    m_Radius = new Distance(m_RadiusDistance);
            }

            return result;
        }

        /// <summary>
        /// Parses an explicitly entered radius. This sets <c>m_RadiusDistance</c>
        /// </summary>
        /// <returns>True if distance parses ok (may be null).</returns>
        bool ParseRadius()
        {
            // Get the entered string.
            string str = radiusTextBox.Text.Trim();

            // No radius if empty string
            if (str.Length==0)
            {
                m_RadiusDistance = null;
                return true;
            }

            // Parse the radius
            Distance d;
            if (Distance.TryParse(str, out d))
            {
                m_RadiusDistance = d;
                OnChange();
                return true;
            }

            MessageBox.Show("Invalid radius.");
            radiusTextBox.Focus();
            return false;
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

            // Handle the pointing, depending on what field we were last in.
            if (m_Focus == centerTextBox)
            {
        		// Draw any previously selected center point normally.
                SetNormalColor(m_Center);

                // Grab the new centre point.
                m_Center = point;

                // Draw the point in appropriate color.
                m_Center.Draw(ActiveDisplay, Color.LightBlue);

                // Display its key (causes a call to OnChangeCentre).
                centerTextBox.Text = String.Format("+{0}", m_Center.FormattedKey);

                // Move focus to the radius field.
                radiusTextBox.Focus();
            }
            else if (m_Focus == radiusTextBox)
            {
                // The radius must be getting specified by pointing at an offset point.

                // Ensure that any previously selected offset point is
                // drawn in its normal colout.
                SetNormalColor(m_RadiusPoint);

                // Grab the new offset point.
                m_RadiusPoint = point;

                // Draw the point in appropriate colour.
                m_RadiusPoint.Draw(ActiveDisplay, Color.Yellow);

                // Display the point number.
                radiusTextBox.Text = String.Format("+{0}", m_RadiusPoint.FormattedKey);

                // Ensure any radius circle has been refreshed.
                OnChange();

                // Move focus to the OK button.
                okButton.Focus();
            }
        }

        ISpatialDisplay ActiveDisplay
        {
            get { return EditingController.Current.ActiveDisplay; }
        }

        internal void Draw(PointFeature point)
        {
            if (point==null)
                PaintAll();
            else
            {
                if (Object.ReferenceEquals(point, m_Center))
                    point.Draw(ActiveDisplay, Color.LightBlue);
                else if (Object.ReferenceEquals(point, m_RadiusPoint))
                    point.Draw(ActiveDisplay, Color.Yellow);
            }
        }

        /// <summary>
        /// Handles any redrawing. This just ensures that points are drawn in the right
        /// color, and that any distance circle shown is still there.
        /// </summary>
        void PaintAll()
        {
            ISpatialDisplay display = ActiveDisplay;

            if (m_Circle!=null)
                m_Circle.Render(display, new DottedStyle());

            if (m_Center!=null)
                m_Center.Draw(display, Color.LightBlue);

            if (m_RadiusPoint!=null)
                m_RadiusPoint.Draw(display, Color.Yellow);
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

        PointFeature GetPoint(TextBox textBox)
        {
            PointFeature point = null;

            // Return if the field is empty.
            string str = textBox.Text.Trim();
            if (str.Length==0)
                return null;

            // Parse the ID value.
            uint idnum;
            if (!UInt32.TryParse(str, out idnum))
            {
                MessageBox.Show("Invalid point ID");
                textBox.Focus();
                return null;
            }

            // Ask the map to locate the address of the specified point.
            ISpatialIndex index = CadastralMapModel.Current.Index;
            point = new FindPointByIdQuery(index, str).Result;
            if (point==null)
            {
                MessageBox.Show("No point with specified key.");
                textBox.Focus();
                return null;
            }

            // Ensure the text is preceded with a "+" character.
            textBox.Text = String.Format("+{0}", point.FormattedKey);
            return point;
        }

        void ShowUpdate()
        {
            // Get the operation that created the update object (if
            // we're doing an update).
            NewCircleOperation creator = GetUpdateOp();
            if (creator != null)
                InitOp(creator, true);
        }

        void InitOp(NewCircleOperation op, bool isUpdate)
        {
            // Get the center point and display its ID, preceded by a "+" char.
            m_Center = op.Center;
            centerTextBox.Text = String.Format("+{0}", m_Center.FormattedKey);

            // Get the observation that was used to specify the radius.
            Observation radius = op.Radius;

            // Make a copy of the relevant info, depending on whether the
            // radius was entered, or specified as an offset point.
            if (radius is OffsetPoint)
            {
                // Radius was specified as an offset point.
                OffsetPoint offset = (radius as OffsetPoint);
                m_RadiusPoint = offset.Point;
                m_RadiusDistance = null;

                // Display the ID of the offset point, preceded with a "+" char.
                radiusTextBox.Text = String.Format("+{0}", m_RadiusPoint.FormattedKey);

                if (isUpdate)
                {
                    // If there are any incident arcs that were added using
                    // a NewLineOperation (on the same circle), disallow the
                    // ability to change the offset point.

                    // @devnote Long story. In short, if the offset point
                    // gets changed, the user could move it anywhere, so quite
                    // a sophisticated UI could be needed to re-define where
                    // the curves should go (failing that, if you let the user
                    // change things, one end of the curve moves, but not the 
                    // end that met the offset point => looks bent). This is a
                    // problem even if the curves have subsequently been
                    // de-activated.

                    LineFeature line = op.Line;
                    Circle circle = line.Circle;
                    Debug.Assert(circle!=null);
                    if (circle.HasArcsAt(m_RadiusPoint))
                        radiusTextBox.Enabled = false;
                }
            }
            else
            {
                // Radius is (or should be) an entered distance.
                m_RadiusPoint = null;
                Distance dist = (radius as Distance);
                if (dist==null)
                {
                    MessageBox.Show("NewCircleForm.InitOp - Unexpected radius observation.");
                    return;
                }
                m_RadiusDistance = new Distance(dist);

                // Display the radius (will call OnChangeRadius).
                radiusTextBox.Text = m_RadiusDistance.Format();
            }

            // Ensure points are drawn ok.
            PaintAll();
        }

        NewCircleOperation GetUpdateOp()
        {
            UpdateUI up = (m_Cmd as UpdateUI);
            return (up==null ? null : (NewCircleOperation)up.GetOp());
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
    }
}