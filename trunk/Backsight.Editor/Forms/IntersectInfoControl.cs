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

using Backsight.Environment;
using Backsight.Editor.Operations;

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
        /// ID and entity type for the intersection point.
        /// </summary>
        IdHandle m_PointId;

        /// <summary>
        /// The intersection (if any)
        /// </summary>
        IPosition m_Intersect; // was also transient m_pPoint object

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
            m_PointId = null;
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
        /// The displayed location of the intersection (null if nothing is showing)
        /// </summary>
        internal IPosition Intersection
        {
            get { return m_Intersect; }
        }

        /// <summary>
        /// ID and entity type for the intersection point.
        /// </summary>
        internal IdHandle PointId
        {
            get { return m_PointId; }
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
                // Select the entity type previously defined for the 
                // intersection point.
                PointFeature feat = op.IntersectionPoint;
                m_PointId = new IdHandle(feat);

                // Load the entity combo box with a list for point features
                // and disable it.
                pointTypeComboBox.Load(SpatialType.Point, feat.BaseLayer);
                pointTypeComboBox.Enabled = false;

                // Scroll the entity combo to the previously defined
                // entity type for the intersection point.
                IEntity curEnt = feat.EntityType;
                if (curEnt!=null)
                    pointTypeComboBox.SelectEntity(curEnt);

                // Display the point key (if any) and disable it.
                pointIdComboBox.Text = m_PointId.FormattedKey;
                pointIdComboBox.Enabled = false;
            }
        }

        private void pointTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_PointId==null)
                return;

            // Get the new point type.
            IEntity ent = (IEntity)pointTypeComboBox.SelectedItem;

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
            IntersectForm parent = GetIntersectForm();
            if (this.Visible && parent!=null)
            {
                m_Intersect = parent.CalculateIntersect();
                ShowIntersection();
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
    }
}
