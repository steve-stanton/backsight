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
using System.Windows.Forms;
using System.Drawing;

using Backsight.Environment;
using Backsight.Editor.Operations;
using Backsight.Editor.Observations;
using Backsight.Editor.UI;

namespace Backsight.Editor.Forms
{
    public partial class RadialControl : UserControl
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        private CommandUI m_Cmd;

        /// <summary>
        /// Command being recalled (if any).
        /// </summary>
        private RadialOperation m_Recall;

        // Direction related ...

        /// <summary>
        /// The point at the start of the sideshot.
        /// </summary>
        private PointFeature m_From;

        /// <summary>
        /// The backsight point (if any).
        /// </summary>
        private PointFeature m_Backsight;

        /// <summary>
        /// 1st parallel point (if direction specified that way).
        /// </summary>
        private PointFeature m_Par1;

        /// <summary>
        /// 2nd parallel point.
        /// </summary>
        private PointFeature m_Par2;

        /// <summary>
        /// Direction angle. If m_Par1 is defined, this should be 0.0 &
        /// should not be used. Always>0
        /// </summary>
        private double m_Radians;

        /// <summary>
        /// Is angle clockwise (relevant only if a backsight has been specified).
        /// </summary>
        private bool m_IsClockwise;

        /// <summary>
        /// Is angle a deflection?
        /// </summary>
	    private bool m_IsDeflection; 

    	// Length related ...

        /// <summary>
        /// Explicitly entered length for sideshot. If defined, m_OffLength should be null.
        /// </summary>
        private Distance m_Length;

        /// <summary>
        /// Transient length offset point. If not null, the m_Length value should be undefined.
        /// </summary>
        private OffsetPoint m_LengthOffset; // was m_pLength

    	// Offset related ...

        /// <summary>
        /// Sub-dialog for getting offset.
        /// </summary>
        private GetOffsetForm m_DialOff;

        /// <summary>
        /// Current direction offset (if any).
        /// </summary>
        private Offset m_Offset;

        // Preview related ...

        /// <summary>
        /// Currently drawn direction.
        /// </summary>
        private Direction m_Dir;

        /// <summary>
        /// The position of the sideshot point.
        /// </summary>
        private IPosition m_To;

        // And miscellaneous ...

        /// <summary>
        /// Any circles incident on the from point.
        /// </summary>
        private List<Circle> m_Circles;

        /// <summary>
        /// The field that last had the focus.
        /// </summary>
        private Control m_Focus;

        /// <summary>
        /// True if a line should be added.
        /// </summary>
        private bool m_WantLine;

        /// <summary>
        /// The ID handle (+entity) for the point.
        /// </summary>
        private IdHandle m_PointId;

        /// <summary>
        /// True if the backsight should be the centre of a curve that the from-point
        /// coincides with.
        /// </summary>
        private bool m_WantCentre;

        /// <summary>
        /// True if OnChange handlers should do NOTHING. Set while doing ShowUpdate.
        /// </summary>
        private bool m_IsStatic;

        #endregion

        internal RadialControl(RadialUI cmd, PointFeature from)
        {
            InitializeComponent();
            Zero();
	        m_Cmd = cmd;
	        m_From = from;
            m_Recall = (RadialOperation)cmd.Recall;
        	InitOp(m_Recall);
        }

        internal RadialControl(UpdateUI updcmd)
        {
            InitializeComponent();
            Zero();
	        m_Cmd = updcmd;
	        InitUpdate();
        }

        internal void Draw()
        {
            //MessageBox.Show("start Draw");

            ISpatialDisplay view = m_Cmd.ActiveDisplay;
            DrawPoints();

            if (m_Dir!=null)
                m_Dir.Render(view);

            IDrawStyle style = EditingController.Current.DrawStyle;
            DrawIfDefined(m_To, view, style, Color.Magenta);

            if (m_To!=null && m_WantLine)
            {
                style.LineColor = Color.Magenta;
                style.Render(view, new IPosition[] { m_From, m_To });
            }

            //MessageBox.Show("end Draw");
        }

        RadialOperation UpdateOp
        {
            get
            {
                UpdateUI up = (m_Cmd as UpdateUI);
                return (up==null ? null : (RadialOperation)up.GetOp());
            }
        }

        bool InitUpdate()
        {
	        // Get the creating op.
	        RadialOperation pop = this.UpdateOp;
	        if (!InitOp(pop) )
                return false;

	        // Get the sideshot point and note its entity type and key.
	        PointFeature point = pop.Point;
	        if (point!=null)
            {
		        m_PointId = new IdHandle(point);
		        m_To = new Position(point);
	        }

	        return true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If the offset dialog is up, get rid of it.
            if (m_DialOff!=null)
                m_Cmd.DialFinish(m_DialOff);

            // Release any reserved ID
            m_PointId.DiscardReservedId();

            // Abort the command.
            m_Cmd.DialAbort(this);
        }

        private void Zero()
        {
            m_Cmd = null;
            m_Recall = null;
            m_From = null;
            m_Backsight = null;
            m_Par1 = null;
            m_Par2 = null;
            m_Radians = 0.0;
            m_IsClockwise = true;
            m_IsDeflection = false;
            m_Length = new Distance();
            m_LengthOffset = null;
            m_DialOff = null;
            m_Offset = null;
            m_Dir = null;
            m_To = null;
            m_Circles = new List<Circle>();
            m_Focus = null;
            m_WantLine = false;
            m_WantCentre = false;
            m_IsStatic = false;
            m_PointId = new IdHandle();
        }

        bool InitOp(RadialOperation pop)
        {
        	if (pop==null)
                return false;

	        // Get the point the sideshot was observed from (unless
	        // it is already defined).
        	if (m_From==null)
                m_From = pop.From;

	        // Was a backsight specified?
	        Direction dir = pop.Direction;
            if (dir is AngleDirection)
            {
                AngleDirection angle = (AngleDirection)dir;
                m_Backsight = angle.Backsight;
                m_Radians = angle.ObservationInRadians;
                if (m_Radians < 0.0)
                {
                    m_Radians = Math.Abs(m_Radians);
                    m_IsClockwise = false;
                }
                else
                    m_IsClockwise = true;

                // Is it a deflection?
		        m_IsDeflection = (dir is DeflectionDirection);
            }

	        // Was direction specified using 2 points (not possible
	        // if we've already picked up a backsight).
        	if (m_Backsight==null)
            {            
                ParallelDirection par = (dir as ParallelDirection);
                if (par!=null)
                {
                    m_Par1 = par.Start;
                    m_Par2 = par.End;
                }
	        }
	        else
            {
		        // Does the backsight point correspond to the centre of a
		        // circle that passes through the from-point?

                double radius = Geom.Distance(m_Backsight, m_From);
                if (m_Backsight.GetCircle(radius)!=null)
                    m_WantCentre = true;
        	}

	        // If we don't have a backsight or a parallel point, it must be a bearing.
            if (!(m_Backsight!=null || m_Par1!=null))
                m_Radians = dir.Bearing.Radians;

	        // Did the direction have an offset? If so, make a transient
	        // copy (specifying m_Length since it's transient).
            Offset offset = dir.Offset;
	        if (offset!=null)
                m_Offset = offset;
                //m_Offset = offset.MakeNewCopy(m_Length);

        	// The length was either entered explicitly, or via an offset point.
            Observation length = pop.Length;
	        Distance dist = (length as Distance);

        	if (dist!=null)
            {
		        m_Length = new Distance(dist);
	        }
	        else
            {
		        // It SHOULD be an offset point.
		        OffsetPoint offsetPoint = (OffsetPoint)length;
                if (offsetPoint!=null)
                    m_LengthOffset = new OffsetPoint(offsetPoint);
        	}

	        // Remember whether a line was added.
            if (pop.Line!=null)
                m_WantLine = true;

	        return true;
        }

        internal void OnSelectPoint(PointFeature point)
        {
            // Return if point is not defined.
            if (point==null)
                return;

            // If the offset sub-dialog is displayed, send the point there.
            if (m_DialOff!=null)
            {
                m_DialOff.OnSelectPoint(point);
                return;
            }

            // Return if point is not valid.
            if (!IsPointValid())
                return;

            HandleSelectPoint(point);

            // Ensure the draw reflects what has been selected (if anything).
            DrawPoints();
        }

        void HandleSelectPoint(PointFeature point)
        {
            if (m_Focus == backsightTextBox)
            {
		        // If the user has checked the "use centre" checkbox, how
		        // come the user is trying to select another backsight.

		        // Ensure that any previously selected backsight reverts
		        // to its normal colour.
        		SetNormalColour(m_Backsight);

		        // Save the specified backsight.
		        m_Backsight = point;

		        // Display it (causes a call to OnChangeBacksight).
                backsightTextBox.Text = String.Format("+{0}", point.FormattedKey);

        		// Move focus to the angle field.
                angleTextBox.Focus();

                return;
            }

            if (m_Focus == angleTextBox)
            {
		        // The direction must be getting specified by pointing
		        // to two parallel points.

        		// Disable the backsight and "use centre" fields.
                backsightTextBox.Enabled = false;
                centreOfCurveCheckBox.Enabled = false;

        		// Define either the first or the second parallel point.
		        if (m_Par1!=null)
                {
			        SetNormalColour(m_Par2);
			        m_Par2 = point;
		        }
		        else
                {
		        	SetNormalColour(m_Par1);
			        m_Par1 = point;
		        }

		        // Figure out the window text.

		        if (m_Par1!=null && m_Par2!=null)
                    angleTextBox.Text = String.Format("+{0}->{1}", m_Par1.FormattedKey, m_Par2.FormattedKey);
                else
                    angleTextBox.Text = String.Format("+{0} & ...", m_Par1.FormattedKey);

		        // Move focus to the length field if we have both parallel points.
		        if (m_Par2!=null)
                    lengthTextBox.Focus();

                return;
            }

            if (m_Focus == lengthTextBox)
            {
                // The length is being specified as an offset point.

        		// Hold on to the offset point & ensure distance is null.
		        if (m_LengthOffset!=null)
                    SetNormalColour(m_LengthOffset.Point);

		        m_LengthOffset = new OffsetPoint(point);
		        m_Length = new Distance();

		        // Display the window text.
                lengthTextBox.Text = String.Format("+{0}", point.FormattedKey);

        		// Move focus to the point type.
                entityTypeComboBox.Focus();
                return;
            }
        }

        /// <summary>
        /// Checks whether it is OK to accept a selected point in the field that last
        /// had the input focus.
        /// </summary>
        /// <returns></returns>
        bool IsPointValid()
        {
            if (m_Focus == backsightTextBox)
            {
                // Disallow a backsight if the "use center" checkbox is ticked.
                if (m_WantCentre)
                    return false;

                // Disallow a backsight if the direction has been specified
                // using two (or even just one) parallel points.
                if (m_Par1!=null)
                {
                    MessageBox.Show("You cannot specify a backsight if you intend to define direction using two points.");
                    return false;
                }

                return true;
            }

            if (m_Focus == angleTextBox)
            {
                // The direction must be getting specified by pointing to two parallel points.
                // This is not allowed if a backsight has been specified.
                if (m_Backsight!=null)
                {
                    MessageBox.Show("You cannot specify two points for direction when the direction has a backsight.");
                    return false;
                }

                // Disallow if two parallel points have already been specified.
                if (m_Par1!=null && m_Par2!=null)
                {
                    MessageBox.Show("You have already specified two points for direction.");
                    return false;
                }

                return true;
            }

            if (m_Focus == lengthTextBox)
            {
                // If a length has been typed in, ignore the point (to be able
                // to specify a length offset point after the fact, the user
                // has to explicitly erase the length, which will cause
                // OnChangeLength to reset m_Length).
                if (m_Length.IsDefined)
                    return false;

                return true;
            }

            // If it's none of the above fields, a point is not valid.
	        // Just return quietly, in case the user is just mucking about
	        // pointing at stuff in the map window.
        	return false;
        }

        internal void OnSelectLine(LineFeature line)
        {
            // Return if line is not defined.
            if (line==null)
                return;

            // If the focus is in the backsight field, and the selected
            // line connects to the from-point, define the backsight to
            // be the point at the other end of the line.
            if (m_From==null || m_Focus!=backsightTextBox)
                return;

            PointFeature point = null;

            if (m_From.IsCoincident(line.StartPoint))
                point = line.EndPoint;
            else if (m_From.IsCoincident(line.EndPoint))
                point = line.StartPoint;

            // Return if a connected point cannot be found.
            if (point==null)
            {
                MessageBox.Show("Cannot locate backsight point.");
                return;
            }

            OnSelectPoint(point);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Don't exit if the offset dialog is still up
            if (m_DialOff!=null)
            {
                MessageBox.Show("The offset dialog hasn't been closed yet.");
                return;
            }

            // Both the direction and length must be defined.
            if (m_Dir!=null && this.Length!=null)
            {
                // Finish the command.
                m_Cmd.DialFinish(this);
            }
            else
            {
                if (m_Dir==null)
                    MessageBox.Show("The direction has not been specified.");
                else
                    MessageBox.Show("The length has not been specified.");
            }
        }

        /// <summary>
        /// Sets the normal colour for a point.
        /// </summary>
        /// <param name="point">The point to set the colour for.</param>
        void SetNormalColour(PointFeature point)
        {
	        // Return if point not specified.
	        if (point==null)
                return;

            // Get the view to draw the point in its normal color.
            ISpatialDisplay view = m_Cmd.ActiveDisplay;
            IDrawStyle style = m_Cmd.Controller.DrawStyle;
            point.Render(view, style);
        }

        void DrawPoints()
        {            
            ISpatialDisplay view = m_Cmd.ActiveDisplay;
            IDrawStyle style = m_Cmd.Controller.DrawStyle;

            DrawIfDefined(m_From, view, style, Color.Cyan);
            DrawIfDefined(m_Backsight, view, style, Color.DarkBlue);
            DrawIfDefined(m_Par1, view, style, Color.Yellow);
            DrawIfDefined(m_Par2, view, style, Color.Yellow);

            if (m_LengthOffset!=null)
                DrawIfDefined(m_LengthOffset.Point, view, style, Color.Green);

            if (m_Offset!=null)
                DrawIfDefined(m_Offset.Point, view, style, Color.Gray);
	    }

        void DrawIfDefined(IPosition point, ISpatialDisplay display, IDrawStyle style, Color col)
        {
            if (point!=null)
            {
                Color oldCol = style.FillColor;

                try
                {
                    style.FillColor = col;
                    style.Render(display, point);
                }

                finally
                {
                    style.FillColor = oldCol;
                }
            }
        }

        private void RadialControl_Load(object sender, EventArgs e)
        {
            // The from-point MUST be defined.
            if (m_From==null)
            {
                MessageBox.Show("RadialControl_Load -- Undefined from-point");
                m_Cmd.DialAbort(this);
                return;
            }

            // Get a list of any circles that are incident on the from point.
            // Use a tight tolerance (3 microns on the ground).
            CadastralMapModel map = CadastralMapModel.Current;

            // Sometimes the BC or EC is apparently not EXACT when the data
            // arrives from a foreign source, so allow 1mm on the ground.
            m_Circles = map.FindCircles(m_From, new Length(0.001));

            // If we are updating a feature that was previously created,
            // load the original info.
            if (IsUpdate)
            {
                // Load the entity combo box with a list for point features.
                entityTypeComboBox.Load(SpatialType.Point);

                ShowUpdate();

                // Disable the ID combo.
                pointIdComboBox.Enabled = false;
            }
            else
            {
                // Load the entity combo box with a list for point features.
                IEntity ent = entityTypeComboBox.Load(SpatialType.Point);

                // If the from point does not lie on a curve, make the
                // "use centre" checkbox invisible.
                centreOfCurveCheckBox.Visible = (m_Circles.Count>0);

                // Disable the angle direction radio buttons if not recalling a previous command.
                if (!ShowRecall())
                    EnableAngleDirection(false);

                // Load the ID combo (reserving the first available ID).
                IdHelper.LoadIdCombo(pointIdComboBox, ent, m_PointId, true);

                // If we are auto-numbering, disable the ID combo.
                EditingController controller = m_Cmd.Controller;
                if (controller.IsAutoNumber)
                    pointIdComboBox.Enabled = false;
            }

            // Get the view to de-select the from point. Then redraw it in light blue.
            if (m_From!=null)
                EditingController.Current.ClearSelection();

            //Draw();
            backsightTextBox.Focus();
        }

        private void angleTextBox_TextChanged(object sender, EventArgs e)
        {
            // Return if OnChange handlers have been disabled.
            if (m_IsStatic)
                return;

            if (angleTextBox.Text.Trim().Length==0)
            {
                // Ensure backsight and user centre check box are enabled.
                if (m_Par1!=null)
                {
                    backsightTextBox.Enabled = true;
                    centreOfCurveCheckBox.Enabled = true;
                }

                // If we already had direction info, reset it.
                SetNormalColour(m_Par1);
                SetNormalColour(m_Par2);

		        m_Par1 = null;
		        m_Par2 = null;
		        m_Radians = 0.0;
		        m_IsClockwise = true;
		        m_IsDeflection = false;

		        // Disable angle direction radio buttons.
		        EnableAngleDirection(false);
            }
            else
            {
                // The direction could have been specified by the
                // user, or it could have been set as the result of
                // a pointing operation. In the latter case, m_Par1
                // will be defined.

                if (m_Par1!=null)
                    EnableAngleDirection(false);
                else
                {
                    // Explicitly entered by the user.

                    // Enable ability to specify clockwise/counter-clockwise,
                    // so long as a backsight has been specified.
                    if (m_Backsight!=null)
                        EnableAngleDirection(true);
                    else
                        clockwiseRadioButton.Checked = true;
                        //SetClockwise(true);

                    // Parse the direction.
                    ParseAngle();
                }
            }

            OnChange();
        }

        private void backsightTextBox_TextChanged(object sender, EventArgs e)
        {
            // Return if OnChange handlers have been disabled.
            if (m_IsStatic)
                return;

            // If field is now empty, ensure that backsight is undefined,
            // and see if this impacts any displayed direction.
            if (backsightTextBox.Text.Trim().Length==0)
            {
                SetNormalColour(m_Backsight);
                m_Backsight = null;
                m_IsClockwise = true;
                EnableAngleDirection(false);
            }
            else if (m_Backsight==null)
            {
                MessageBox.Show("You can only specify the backsight by pointing at the map.");
                return;
            }
            else
                EnableAngleDirection(true);

            OnChange();
        }

        private void lengthTextBox_TextChanged(object sender, EventArgs e)
        {
            // Return if OnChange handlers have been disabled.
            if (m_IsStatic)
                return;

            if (lengthTextBox.Text.Trim().Length==0)
            {
                // If we already had length info, reset it.
                if (m_LengthOffset!=null)
                    SetNormalColour(m_LengthOffset.Point);

                m_Length = new Distance();
                m_LengthOffset = null;
            }
            else
            {
                // The length could have been specified by the
                // user, or it could have been set as the result of
                // a pointing operation. In the latter case,
                // m_LengthOffset will be defined.

                if (m_LengthOffset==null)
                    ParseLength();
                else
                {
                    // Confirm that the displayed text is the text that was
                    // added by OnSelectPoint. If not, the user may be trying
                    // to type in a length. Permit fewer chars, in case the
                    // user is erasing.
                    string keystr = String.Format("+{0}", m_LengthOffset.Point.FormattedKey);
                    string curstr = lengthTextBox.Text.Trim();

                    if (curstr.Length > keystr.Length)
                    {
                        string msg = "What are you trying to do? If you want to type in a" + System.Environment.NewLine +
					                 "new length, you must initially delete the point ID" + System.Environment.NewLine +
					                 "that is currently displayed.";

                        MessageBox.Show(msg);
                        return;
                    }
                }
            }

            OnChange();
        }

        private void offsetButton_Click(object sender, EventArgs e)
        {
            // Disallow an attempt to create more than one offset dialog (the
            // button SHOULD have been disabled when we first created it).
            if (m_DialOff!=null)
            {
                MessageBox.Show("The offset dialog is still open.");
                return;
            }

            // Disable the button. It will be re-enabled when the user
            // exits the dialog (see DialFinish && DialAbort).
            offsetButton.Enabled = false;

            // Also the OK button (you will need to initially close the offset sub-dialog).
            okButton.Enabled = false;

            // Create a modeless dialog and display it.
            m_DialOff = new GetOffsetForm(m_Cmd, m_Offset);
            m_DialOff.Show(this);
        }

        private void centreOfCurveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle the status.
            m_WantCentre = !m_WantCentre;

	        // If the user now wants to use the centre point as backsight
            if (m_WantCentre)
            {
                // How many circles were incident on the from-point?
                int ncircle = m_Circles.Count;

                // There SHOULD be at least one (otherwise the checkbox that
                // leads to this function should have been diabled).
                if (ncircle==0)
                {
                    MessageBox.Show("The from-point does not coincide with any circular curves.");
                    centreOfCurveCheckBox.Checked = false;
                    return;
                }

                // Get the circle involved. If there's more than one, we need
                // to ask the user which one.
                Circle circle;

                if (ncircle==1)
                    circle = m_Circles[0];
                else
                {
                    // Ask the user to select a circle.
                    GetCircleForm dial = new GetCircleForm(m_Circles);
                    if (dial.ShowDialog() != DialogResult.OK)
                    {
                        centreOfCurveCheckBox.Checked = false;
                        return;
                    }

                    circle = dial.Circle;
                    dial.Dispose();
                }

                // Get the point at the centre.
                ISpatialModel map = CadastralMapModel.Current;
                ISpatialObject so = map.QueryClosest(circle.Center, Backsight.Length.Zero, SpatialType.Point);
                m_Backsight = (so as PointFeature);

                // Confirm that we got something.
                if (m_Backsight==null)
                {
                    MessageBox.Show("Cannot find the center point.");
                    centreOfCurveCheckBox.Checked = false;
                    return;
                }

                // Display the key of the backsight.
                backsightTextBox.Text = String.Format("+{0}", m_Backsight.FormattedKey);

                // Disable the backsight field.
                backsightTextBox.Enabled = false;

                // Resume in the angle field.
                angleTextBox.Focus();
            }
            else
            {
                // Draw any backsight point normally.
                SetNormalColour(m_Backsight);

                // Reset backsight stuff.
                m_Backsight = null;
                backsightTextBox.Enabled = true;
                backsightTextBox.Text = String.Empty;

        		// Resume in the backsight field.
                backsightTextBox.Focus();
            }

            OnChange();
        }

        private void clockwiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (clockwiseRadioButton.Checked)
            {
                m_IsClockwise = true;
                OnChange();
            }
        }

        private void counterClockwiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (counterClockwiseRadioButton.Checked)
            {
                m_IsClockwise = false;
                OnChange();
            }
        }

        void SetClockwise(bool iscw)
        {
            if (iscw)
                clockwiseRadioButton.Checked = true;
            else
                counterClockwiseRadioButton.Checked = true;
        }

        private void entityTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Get the new point type (it might be null while the control is loading)
            IEntity ent = entityTypeComboBox.SelectedEntityType;
            if (ent==null)
                return;

            // If the current ID does not apply to the new point type,
            // reload the ID combo (reserving a different ID).
            if (!m_PointId.IsValidFor(ent))
                IdHelper.LoadIdCombo(pointIdComboBox, ent, m_PointId, true);
            else
                m_PointId.Entity = ent;
        }

        private void addLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle the want-line status.
            m_WantLine = addLineCheckBox.Checked;

            // Redraw any extension.
            OnChange();
        }

        void EnableAngleDirection(bool enable)
        {
            if (enable)
            {
                clockwiseRadioButton.Enabled = true;
                counterClockwiseRadioButton.Enabled = true;

                // Ensure that the current angle direction is shown.
                if (m_IsClockwise)
                {
                    clockwiseRadioButton.Checked = true;
                    counterClockwiseRadioButton.Checked = false;
                }
                else
                {
                    clockwiseRadioButton.Checked = false;
                    counterClockwiseRadioButton.Checked = true;
                }
            }
            else
            {
                TurnRadioOff(clockwiseRadioButton);
                TurnRadioOff(counterClockwiseRadioButton);
            }
        }

        /// <summary>
        /// Turns off a radio button. This unchecks the button, and deactivates it
        /// (if you just de-activate it, any previous check mark stays visible,
        /// although grayed out).
        /// </summary>
        /// <param name="radio">The radio button to change</param>
        void TurnRadioOff(RadioButton radio)
        {
            radio.Checked = false;
            radio.Enabled = false;
        }

        /// <summary>
        /// Parses an explicitly entered angle. 
        /// </summary>
        /// <returns>True if direction parses ok.</returns>
        bool ParseAngle()
        {
            // Get the entered string.
            string dirstr = angleTextBox.Text.Trim();
            if (dirstr.Length==0)
                return false;

            // If all we have is a "-", disable the ability to specify
            // clockwise direction & return.
            if (dirstr[0] == '-')
            {
                TurnRadioOff(clockwiseRadioButton);
                counterClockwiseRadioButton.Checked = true;
                if (dirstr.Length==1)
                    return false;
            }

            // If the entered angle contains a "d" (anywhere), treat it
            // as a deflection (and strip it out).
            dirstr = dirstr.ToUpper();
            int dindex = dirstr.IndexOf('D');
            if (dindex>=0)
            {
                dirstr = dirstr.Substring(0, dindex) + dirstr.Substring(dindex+1);
                m_IsDeflection = true;
            }
            else
                m_IsDeflection = false;

            // Validate entered angle.
            double srad = 0.0;
            if (dirstr.Length>0)
            {
                if (!RadianValue.TryParse(dirstr, out srad))
                {
                    MessageBox.Show("Invalid angle.");
                    angleTextBox.Focus();
                    return false;
                }
            }

            // If we have signed radians, it HAS to be a counter-clockwise
            // angle. Otherwise make sure we preserve the directional sense.
            // which may have been previously defined.
            if (srad<0.0)
                m_IsClockwise = false;

            m_Radians = Math.Abs(srad);

            // Ensure the radio button reflects any sign we found.
            SetClockwise(m_IsClockwise);
            return true;
        }

        /// <summary>
        /// Parses an explicitly entered sideshot length.
        /// </summary>
        /// <returns>True if length parses ok.</returns>
        bool ParseLength()
        {
            // Get the entered string.
            string str = lengthTextBox.Text.Trim();

            // Try to parse it.
            Distance length = new Distance(str);
            if (!length.IsDefined)
            {
                MessageBox.Show("Invalid length");
                return false;
            }

            // Hold on to the parsed length (and ensure that any previously
            // defined offset point is null -- should really be null already).
            m_Length = length;
            m_LengthOffset = null;

            return true;
        }

        /// <summary>
        /// Checks whether the current data is enough to construct a direction. If so,
        /// draw it. Take care to erase any previously drawn direction.
        /// </summary>
        void OnChange()
        {
            Direction dir=null;	            // Constructed direction.
            AngleDirection angle;			// Angle from a backsight.
            DeflectionDirection deflect;	// Deflection angle.
            BearingDirection bearing;		// Bearing from north.
            ParallelDirection par;			// Parallel to 2 points.
            double srad;			        // Signed radian value.

            // Apply sign to any angle we have.
            if (m_IsClockwise)
                srad = m_Radians;
            else
                srad = -m_Radians;

            if (m_Backsight!=null)
            {
                // If we have a backsight, we could either have a regular
                // angle or a deflection. To construct either, we need a
                // from-point as well.

                // Note that an angle of zero (passing through the backsight
                // or foresight) is fine.

                if (m_From!=null)
                {
                    IAngle obsv = new RadianValue(srad);

                    if (m_IsDeflection)
                    {
                        deflect = new DeflectionDirection(m_Backsight, m_From, obsv);
                        dir = deflect;
                    }
                    else
                    {
                        angle = new AngleDirection(m_Backsight, m_From, obsv);
                        dir = angle;
                    }
                }
            }
            else if (m_From!=null)
            {
                // No backsight, so we could have either a bearing,
                // or a direction defined using 2 parallel points.
                // Since a bearing of zero is quite valid, we check
                // the dialog field to see if this is an entered value,
                // or just the initial value.

                if (m_Par1!=null && m_Par2!=null)
                {
                    par = new ParallelDirection(m_From, m_Par1, m_Par2);
                    dir = par;
                }
                else
                {
                    if (m_Radians>Constants.TINY || angleTextBox.Text.Trim().Length==0)
                    {
                        bearing = new BearingDirection(m_From, new RadianValue(srad));
                        dir = bearing;
                    }
                }
            }

            // If we have formed a direction, apply any offset.
            if (dir!=null)
                dir.Offset = m_Offset;

            // Try to calulate the position of the sideshot.
            IPosition to = RadialOperation.Calculate(dir, this.Length);

            // Return if we calculated a position that is identical to the old one.
            //if (to!=null && to.IsAt(m_To, Double.Epsilon))
            //    return;

            m_Dir = dir;
            m_To = to;

            m_Cmd.ErasePainting();
        }

        internal Direction Direction
        {
            get { return m_Dir; }
        }

        internal bool WantLine
        {
            get { return m_WantLine; }
        }

        internal IdHandle PointId
        {
            get { return m_PointId; }
        }

        internal Observation Length
        {
            get
            {
                if (m_LengthOffset!=null)
                    return m_LengthOffset;
                else if (m_Length.IsDefined)
                    return m_Length;
                else
                    return null;
            }
        }

        /// <summary>
        /// Finishes a sub-dialog (i.e. the offset dialog).
        /// </summary>
        /// <param name="wnd">The dialog window.</param>
        /// <returns></returns>
        internal bool DialFinish(Control wnd)
        {
	        if (m_DialOff==null)
            {
		        MessageBox.Show("RadialControl.DialFinish - No sub-dialog!");
		        return false;
	        }

	        // The new offset is already known via calls that the
	        // sub-dialog made to SetOffset.

        	// Destroy the sub-dialog.
            m_DialOff.Dispose();
            m_DialOff = null;

        	// Re-enable the offset and OK buttons.
            offsetButton.Enabled = true;
            okButton.Enabled = true;

        	// Resume in the point type field.
            entityTypeComboBox.Focus();

	        // THIS dialog should be in the foreground.
	        //this->SetForegroundWindow();

	        return true;
        }

        /// <summary>
        /// Aborts a sub-dialog (i.e. the offset dialog).
        /// </summary>
        /// <param name="wnd">The dialog window.</param>
        internal void DialAbort(Control wnd)
        {
            if (m_DialOff==wnd)
            {
                m_DialOff.Hide();
                m_DialOff.Dispose();
                m_DialOff = null;

                // Re-enable the offset and OK buttons.
                offsetButton.Enabled = true;
                okButton.Enabled = true;

                // Put this dialog in the foreground.
                //this->SetForegroundWindow();
            }
            else
            {
                MessageBox.Show("RadialControl.DialAbort - Unexpected dialog.");
            }
        }

        /// <summary>
        /// Accepts a new direction offset.
        /// </summary>
        /// <param name="offset">The new offset (if any).</param>
        internal void SetOffset(Offset offset)
        {
            // If we previously had an offset point, make sure it's
            // drawn in it's normal colour.
            if (m_Offset!=null)
            {
                SetNormalColour(m_Offset.Point);
                m_Offset = null;
            }

            // If it's an offset point, draw the point in green.
            m_Offset = offset;
            if (offset!=null && offset.Point!=null)
                DrawPoints();

            // See if that changes anything.
            OnChange();
        }

        void ShowUpdate()
        {
            RadialOperation op = UpdateOp;
            if (op==null)
                return;

            // Ensure OnChange handlers do nothing when we set the
            // text in various edit boxes.
            m_IsStatic = true;

            //SetWindowText("Update Sideshot");

            // Display the key of the backsight (if any).
            if (m_Backsight!=null)
                backsightTextBox.Text = String.Format("+{0}", m_Backsight.FormattedKey);

            // If the angle was specified using two parallel points,
            // display the two keys. Otherwise display the angle and,
            // if it's got a backsight, check the radio button to say
            // whether it's clockwise or not.

            if (m_Par1!=null && m_Par2!=null)
            {
                angleTextBox.Text = String.Format("+{0}->{1}", m_Par1.FormattedKey, m_Par2.FormattedKey);
                backsightTextBox.Enabled = false;
                centreOfCurveCheckBox.Enabled = false;
                EnableAngleDirection(false);
            }
            else
            {
                string dirstr = RadianValue.AsShortString(m_Radians);
                if (m_IsDeflection)
                    dirstr += "d";
                angleTextBox.Text = dirstr;

                if (m_Backsight!=null)
                    EnableAngleDirection(true);
                else
                    EnableAngleDirection(false);
            }

            // If the length was specified as an offset point, display
            // the key of the point. Otherwise display the length.
            if (m_LengthOffset!=null)
            {
                PointFeature offPoint = m_LengthOffset.Point;
                lengthTextBox.Text = String.Format("+{0}", offPoint.FormattedKey);
            }
            else
                lengthTextBox.Text = m_Length.Format();

            // Scroll combo to the entity type of the sideshot point that was created.
            // Then disable the combo.
            IEntity ent = op.Point.EntityType;
            if (ent!=null)
                entityTypeComboBox.SelectEntity(ent);
            entityTypeComboBox.Enabled = false;

            // Display the point key (if any) and disable it.
            pointIdComboBox.Text = m_PointId.FormattedKey;
            pointIdComboBox.Enabled = false;

            // Set the check box that says whether a line was added.
            addLineCheckBox.Checked = m_WantLine;

            // And disable it too.
            addLineCheckBox.Enabled = false;

            // If the from point lies on at least one circle, show the "use centre" checkbox.
            centreOfCurveCheckBox.Visible = (m_Circles.Count>0);

            // If a backsight corresponds to the centre of a circle,
            // show "use-centre" checkbox, with backsight disabled.
            if (m_WantCentre)
            {
                centreOfCurveCheckBox.Checked = true;
                backsightTextBox.Enabled = false;
            }

            // Re-enable OnChange handlers.
            m_IsStatic = false;

            // Highlight direction line etc. To FORCE a draw, the current
            // position of the sideshot has to be removed.
            m_To = null;
            OnChange();
        }

        bool ShowRecall()
        {
        	if ( m_Recall==null)
                return false;

            // Ensure OnChange handlers do nothing when we set the
	        // text in various edit boxes.
	        m_IsStatic = true;

            // Display the key of the backsight (if any).
        	if (m_Backsight!=null)
                backsightTextBox.Text = String.Format("+{0}", m_Backsight.FormattedKey);

            // If the angle was specified using two parallel points,
	        // display the two keys. Otherwise display the angle and,
	        // if it's got a backsight, check the radio button to say
	        // whether it's clockwise or not.

	        if (m_Par1!=null && m_Par2!=null)
            {
                angleTextBox.Text = String.Format("+{0}->{1}", m_Par1.FormattedKey, m_Par2.FormattedKey);
                backsightTextBox.Enabled = false;
                centreOfCurveCheckBox.Enabled = false;
		        EnableAngleDirection(false);
	        }
	        else
            {
                string dirstr = RadianValue.AsShortString(m_Radians);
		        if (m_IsDeflection)
                    dirstr += "d";
                angleTextBox.Text = dirstr;

		        if (m_Backsight!=null)
			        EnableAngleDirection(true);
		        else
			        EnableAngleDirection(false);
	        }

            // If the length was specified as an offset point, display
	        // the key of the point. Otherwise display the length.
	        if (m_LengthOffset!=null)
            {
                PointFeature offPoint = m_LengthOffset.Point;
                lengthTextBox.Text = String.Format("+{0}", offPoint.FormattedKey);
	        }
	        else
                lengthTextBox.Text = m_Length.Format();

            // Scroll combo to the entity type of the sideshot point that was created.
            IEntity ent = m_Recall.Point.EntityType;
            if (ent!=null)
                entityTypeComboBox.SelectEntity(ent);

            // Set the check box that says whether a line was added.
            addLineCheckBox.Checked = m_WantLine;

	        // If a backsight corresponds to the centre of a circle,
	        // show "use-centre" checkbox, with backsight disabled.
            if (m_WantCentre)
            {
                centreOfCurveCheckBox.Checked = true;
                backsightTextBox.Enabled = false;
            }

            // Re-enable OnChange handlers.
	        m_IsStatic = false;

            // Highlight direction line etc. To FORCE a draw, the current
	        // position of the sideshot has to be removed.
            m_To = null;
            OnChange();
            return true;
        }

        private void pointIdComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            IdHelper.OnChangeSelectedId(pointIdComboBox, m_PointId);
        }

        bool IsUpdate
        {
            get { return (m_Cmd is UpdateUI); }
        }

        private void backsightTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = backsightTextBox;
        }

        private void angleTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = angleTextBox;
        }

        private void lengthTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = lengthTextBox;
        }
    }
}
