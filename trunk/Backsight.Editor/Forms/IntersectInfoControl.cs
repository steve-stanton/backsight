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

using Backsight.Environment;
using Backsight.Editor.Operations;
using System.Drawing;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdIntersectOne" />
    /// <summary>
    /// Information about an intersection point.
    /// </summary>
    partial class IntersectInfoControl : UserControl
    {
        #region Class data

        /// <summary>
        /// ID and entity type for the intersection point (should never be null)
        /// </summary>
        IdHandle m_PointId;

        /// <summary>
        /// The intersection (if any)
        /// </summary>
        IPosition m_Intersect; // was also transient m_pPoint object

        /// <summary>
        /// Does the intersection correspond to the default position? Has no meaning
        /// if the <see cref="CanHaveTwoIntersections"/> property is false.
        /// </summary>
        bool m_IsDefault;

        /// <summary>
        /// The point nearest to the intersection. Has no meaning
        /// if the <see cref="CanHaveClosestPoint"/> property is false.
        /// </summary>
        PointFeature m_CloseTo;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor sets everything to null.
        /// </summary>
        public IntersectInfoControl()
        {
            InitializeComponent();

            // No point feature at the intersection.
            m_Intersect = null;
            m_PointId = new IdHandle();
            m_IsDefault = true;
            m_CloseTo = null;
        }

        #endregion

        /// <summary>
        /// Is the "Other Intersection" button visible? Should be set true when dealing
        /// with edits where there is a possibility of two intersections (e.g. as in
        /// a distance-distance intersection).
        /// </summary>
        public bool CanHaveTwoIntersections
        {
            get { return otherButton.Visible; }
            set { otherButton.Visible = value; }
        }

        /// <summary>
        /// Is the <see cref="ClosestPoint"/> property relevant? Should be set true when dealing
        /// with intersects involving line features.
        /// </summary>
        public bool CanHaveClosestPoint
        {
            get { return closestPointValueLabel.Visible; }
            set
            {
                closestPointTrimLabel.Visible =
                closestPointValueLabel.Visible =
                closestPointExplanationLabel.Visible = value;
            }
        }

        /// <summary>
        /// The displayed location of the intersection (null if nothing is showing)
        /// </summary>
        internal IPosition Intersection
        {
            get { return m_Intersect; }
        }

        /// <summary>
        /// ID and entity type for the intersection point (should never be null)
        /// </summary>
        internal IdHandle PointId
        {
            get { return m_PointId; }
        }

        /// <summary>
        /// Does the intersection correspond to the default position? Has no meaning
        /// if the <see cref="CanHaveTwoIntersections"/> property is false.
        /// </summary>
        internal bool IsDefault
        {
            get { return m_IsDefault; }
        }

        /// <summary>
        /// The point nearest to the intersection. Has no meaning
        /// if the <see cref="CanHaveClosestPoint"/> property is false.
        /// </summary>
        internal PointFeature ClosestPoint
        {
            get { return m_CloseTo; }
        }

        internal void InitializeControl(IntersectForm parent)
        {
            // Ask the enclosing property sheet whether we are updating
            // an existing feature or not.

            // If we are updating a previously existing point, select
            // the previously defined entity type.
            IntersectOperation op = parent.GetUpdateOp();
            if (op==null)
            {
                // Load the entity combo box with a list for point features.
                IEntity ent = pointTypeComboBox.Load(SpatialType.Point);

                // Load the ID combo (reserving the first available ID).
                IdHelper.LoadIdCombo(pointIdComboBox, ent, m_PointId, true);

                // If we are auto-numbering, disable the ID combo.
                EditingController controller = EditingController.Current;
                if (controller.IsAutoNumber)
                    pointIdComboBox.Enabled = false;
            }
            else
            {
                pointTypeComboBox.Enabled = false;
                pointIdComboBox.Enabled = false;

                // Select the entity type previously defined for the 
                // intersection point.
                PointFeature feat = op.IntersectionPoint;
                m_PointId = new IdHandle(feat);

                // Load the entity combo box with a list for point features
                // and disable it.
                pointTypeComboBox.Load(SpatialType.Point, feat.BaseLayer);

                // Scroll the entity combo to the previously defined
                // entity type for the intersection point.
                IEntity curEnt = feat.EntityType;
                if (curEnt!=null)
                    pointTypeComboBox.SelectEntity(curEnt);

                // Display the point key (if any) and disable it.
                pointIdComboBox.Text = m_PointId.FormattedKey;

                // Intersects involving line features...
                m_CloseTo = op.ClosePoint;
                ShowCloseTo();
            }
        }

        private void pointTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // Just return if the ID combo is disabled (means we're doing an update)
            if (!pointIdComboBox.Enabled)
                return;

            // Get the new point type.
            IEntity ent = pointTypeComboBox.SelectedEntityType;

            // If the current ID does not apply to the new point type,
            // reload the ID combo (reserving a different ID).
            if (!m_PointId.IsValidFor(ent))
                IdHelper.LoadIdCombo(pointIdComboBox, ent, m_PointId, true);
            else
                m_PointId.Entity = ent;
        }

        private void pointIdComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            IdHelper.OnChangeSelectedId(pointIdComboBox, m_PointId);
        }

        /// <summary>
        /// Attempts to locate the dialog containing this control.
        /// </summary>
        /// <returns>The form containing this control (may be null during initialization)</returns>
        IntersectForm GetIntersectForm()
        {
            return (this.ParentForm as IntersectForm);
        }

        private void IntersectInfoControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (m_CloseTo==null)
                    SetDefaultClosestPoint();

                RecalculateIntersection();
            }
        }

        void RecalculateIntersection()
        {
            IntersectForm parent = GetIntersectForm();
            if (parent!=null)
            {
                m_Intersect = parent.CalculateIntersect();
                ShowIntersection();
            }
        }

        void SetDefaultClosestPoint()
        {
            IntersectForm parent = GetIntersectForm();
            if (parent!=null)
            {
                m_CloseTo = parent.GetDefaultClosestPoint();
                ShowCloseTo();
            }
        }

        void ShowIntersection()
        {
            if (m_Intersect == null)
            {
                northingLabel.Text = eastingLabel.Text = "<undefined>";
            }
            else
            {
                northingLabel.Text = String.Format("{0:0.0000}", m_Intersect.Y);
                eastingLabel.Text = String.Format("{0:0.0000}", m_Intersect.X);
            }
        }

        private void otherButton_Click(object sender, EventArgs e)
        {
            bool oldDefault = m_IsDefault;

            try
            {
                IntersectForm parent = GetIntersectForm();
                m_IsDefault = !m_IsDefault;
                IPosition other = parent.CalculateIntersect();
                if (other==null || other.IsAt(m_Intersect, Constants.TINY))
                    throw new Exception("There isn't another intersection");

                m_Intersect = other;
                ShowIntersection();
                EditingController.Current.ActiveDisplay.RestoreLastDraw();
            }

            catch (Exception ex)
            {
                m_IsDefault = oldDefault;
                MessageBox.Show(ex.Message);
            }
        }

        ISpatialDisplay ActiveDisplay
        {
            get { return EditingController.Current.ActiveDisplay; }
        }

        /// <summary>
        /// Handles any redrawing.
        /// </summary>
        internal void OnDraw()
        {
            ISpatialDisplay display = ActiveDisplay;

            // If the point the intersection needs to be close to is defined, draw it.
            if (m_CloseTo!=null)
            {
                IDrawStyle style = EditingController.Current.Style(Color.Red);
                m_CloseTo.Render(display, style);
            }

            // Draw the intersection point in magenta
            if (m_Intersect!=null)
            {
                IDrawStyle style = EditingController.Current.Style(Color.Magenta);
                style.Render(display, m_Intersect);
            }
        }

        /// <summary>
        /// Reacts to the selection of a line feature (does nothing).
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal void OnSelectLine(LineFeature line)
        {
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal void OnSelectPoint(PointFeature point)
        {
            // Return if point is not defined, or closest point isn't relevant
            if (point==null || !CanHaveClosestPoint)
                return;

            // Save the specified point.
            m_CloseTo = point;

            // Display the point's key
            ShowCloseTo();

            // Rework any intersection.
            RecalculateIntersection();
        }

        void ShowCloseTo()
        {
            // If the close-to point is undefined, ensure the field is blank.
            if (m_CloseTo==null)
                closestPointValueLabel.Text = String.Empty;
            else
            {
                // Display the key of the point
                closestPointValueLabel.Text = m_CloseTo.FormattedKey;

                // Display the point in an appropriate colour.
                OnDraw();
            }
        }
    }
}
