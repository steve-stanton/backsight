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

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Editor.UI;

namespace Backsight.Editor.Forms
{
    public partial class NewPointForm : Form
    {
        #region Class data

        /// <summary>
        /// The command running the show.
        /// </summary>
        private readonly CommandUI m_Cmd;

        /// <summary>
        /// Alternate title for dialog
        /// </summary>
        private readonly string m_Title;

        /// <summary>
        /// The position of the point
        /// </summary>
        private Position m_Position;

        /// <summary>
        /// Elevation of the point
        /// </summary>
        private double m_Elevation;

        /// <summary>
        /// The ID and entity type of the point.
        /// </summary>
        private IdHandle m_PointId;

        #endregion

        #region Constructors

        internal NewPointForm(CommandUI cmd, string title, Operation recall)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_Position = new Position(0.0, 0.0);
        	m_Elevation = 0.0;
            m_Title = (title==null ? String.Empty : title);
            m_PointId = new IdHandle();

            NewPointOperation op = (recall as NewPointOperation);
            if (op != null)
            {
                PointFeature pt = op.Point;
                m_Position.X = pt.X;
                m_Position.Y = pt.Y;

                IPointGeometry pg = pt.Geometry;
                if (pg is IPosition3D)
                    m_Elevation = (pg as IPosition3D).Z;
            }
        }

        #endregion

        bool InitUpdate()
        {
	        // Get the point selected for update.
	        PointFeature upt = this.UpdatePoint;
	        if (upt==null)
                return false;

	        // Pick up the previously defined info.
	        m_PointId = new IdHandle((Feature)upt);
            m_Position.X = upt.X;
            m_Position.Y = upt.Y;

            IPointGeometry pg = upt.Geometry;
            if (pg is IPosition3D)
                m_Elevation = (pg as IPosition3D).Z;

	        // Display the position.
            ShowPosition();
            elevationTextBox.Enabled = (Math.Abs(m_Elevation) > Double.Epsilon);

	        // Disable the entity combo box
            entityTypeComboBox.Enabled = false;

        	// Select the defined entity type (if any).
            IEntity curEnt = upt.EntityType;
            if (curEnt!=null)
                entityTypeComboBox.SelectedItem = curEnt.Name; // not sure if this will work

        	// Display the point key (if any) and disable it.
            idComboBox.Text = m_PointId.FormattedKey;
            idComboBox.Enabled = false;

            return true;
        }

        private void NewPointForm_Load(object sender, EventArgs e)
        {
	        // Display alternate window title if one has been supplied.
	        if (!String.IsNullOrEmpty(m_Title))
                this.Text = m_Title;

            ILayer layer = m_Cmd.ActiveLayer;
            IEntity[] entities = EnvironmentContainer.EntityTypes(SpatialType.Point, layer);
            Array.Sort<IEntity>(entities, delegate(IEntity a, IEntity b)
                                    { return a.Name.CompareTo(b.Name); });
            entityTypeComboBox.DataSource = entities;

	        // If it's not an update ...
	        if (!InitUpdate())
            {
                // Pick any default entity type for the currently active map layer (the
                // change handler for the entity type combo will go on to load the
                // ID combo)
                IEntity defEnt = (layer==null ? null : layer.DefaultPointType);
                if (defEnt!=null)
                {
                    entityTypeComboBox.SelectedItem = defEnt;

                    // If we are auto-numbering, disable the ID combo.
                    if (EditingController.Current.IsAutoNumber)
                        idComboBox.Enabled = false;
                }
                else
                    idComboBox.Enabled = false;



		        // If the position is defined (because we're recalling
		        // a previous command), fill in those fields too.

                if (Math.Abs(m_Position.X)>Double.Epsilon && Math.Abs(m_Position.Y)>Double.Epsilon)
                    ShowPosition();
    		}
        }

        private void ShowPosition()
        {
            northingTextBox.Text = String.Format("{0:0.0000}", m_Position.Y);
            eastingTextBox.Text = String.Format("{0:0.0000}", m_Position.X);

            if (Math.Abs(m_Elevation) > Double.Epsilon)
                elevationTextBox.Text = String.Format("{0:0.0000}", m_Elevation);
            else
                elevationTextBox.Text = String.Empty;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Get the northing & easting.
            double y;
            if (!Double.TryParse(northingTextBox.Text, out y))
            {
                MessageBox.Show("Bad northing");
                northingTextBox.Focus();
                return;
            }

            double x;
            if (!Double.TryParse(eastingTextBox.Text, out x))
            {
                MessageBox.Show("Bad easting");
                eastingTextBox.Focus();
                return;
            }

            if (Math.Abs(x)<Double.Epsilon || Math.Abs(y)<Double.Epsilon)
            {
                MessageBox.Show("Position has not been specified.");
                return;
            }

            // See if there is an elevation (get 0.0 if not).
            double z = 0.0;
            if (elevationTextBox.Text.Length>0)
            {
                if (!Double.TryParse(elevationTextBox.Text, out z))
                {
                    MessageBox.Show("Bad elevation");
                    elevationTextBox.Focus();
                    return;
                }
            }

            m_Position = new Position(x, y);
            m_Elevation = z;

            // Check whether the position is on screen. If not, issue a warning
            // message, and let the user cancel if desired.
            ISpatialDisplay display = EditingController.Current.ActiveDisplay;
            IWindow extent = display.Extent;
            if (extent==null || !extent.IsOverlap(m_Position))
            {
                if (MessageBox.Show("Specified position does not overlap current draw window. Continue?",
                                        "Off screen", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            // Are we doing an update?
            PointFeature pupt = this.UpdatePoint;

            // Confirm that there is no selectable point already at the specified position. Allow
            // a tolerance of 1cm on the ground. Only check in 2D.
            // The seatch should be restricted to those points that are currently visible.

            CadastralMapModel map = CadastralMapModel.Current;
            ILength tol = new Length(0.01);
            PointFeature close = (PointFeature)map.QueryClosest(m_Position, tol, SpatialType.Point);

            if (close!=null)
            {
                // Get the ground distance between the existing point & the new one.
                double dist = Geom.Distance(m_Position, close);

                // Confirm if the points are coincident. Unless we're doing an update,
                // and it's the one we're doing.

                if (pupt==null || !Object.ReferenceEquals(close, pupt))
                {
                    if (dist < Constants.TINY)
                    {
                        string msg = String.Format("Specified position coincides with existing point {0}. Continue?",
                                                    close.FormattedKey);
                        if (MessageBox.Show(msg, "Coincident point", MessageBoxButtons.YesNo)== DialogResult.No)
                            return;
                    }
                    else
                    {
                        string msg = String.Format("Specified position is only {0:0.000} metres away from point {1}. Continue?",
                                                    dist, close.FormattedKey);

                        if (MessageBox.Show(msg, "Very near another point", MessageBoxButtons.YesNo)== DialogResult.No)
                            return;

                    }
                }
            }

            // If we're updating a 3D position, update ONLY the elevation
            // now. We must leave the planimetric change to NewPointUI,
            // since it is in charge of controlling rollforward.

            if (pupt!=null)
            {
                // For the time being, we do not provide the ability to change
                // a 2D location into a 3D one. Would need to modify the location,
                // thereby changing its address (or possibly create a duplicate
                // location in XY, although that could cause problems elsewhere).


                if (m_Elevation>Constants.TINY && !(pupt is IEditPosition3D))
                {
                    MessageBox.Show("Cannot convert a 2D point into 3D");
                    return;
                }

                // The point MUST be associated with a creating operation (either
                // a eNewPoint or a GetControl operation).
                Operation creator = (Operation)pupt.Creator;
                if (creator==null)
                {
                    MessageBox.Show("Point cannot be updated because it has no associated edit.");
                    return;
                }

                if (!(creator.EditId == EditingActionId.NewPoint ||
                      creator.EditId == EditingActionId.GetControl))
                {
                    MessageBox.Show("Unexpected editing operation");
                    return;
                }

                // Define new elevation if we have a 3D location.
                if (m_Elevation>Constants.TINY)
                {
                    IEditPosition3D p3D = (pupt as IEditPosition3D);
                    if (p3D==null)
                        MessageBox.Show("Unable to assign elevation");
                    else
                        p3D.Z = m_Elevation;
                }
            }

            m_Cmd.DialFinish(this);
        }

        internal IPosition Position
        {
            get { return m_Position; }
        }

        PointFeature UpdatePoint
        {
            get
            {
                UpdateUI up = (m_Cmd as UpdateUI);
                return (up==null ? null : (PointFeature)up.SelectedObject);
            }
        }

        /// <summary>
        /// Creates a new point, based on the information stored in the
        /// dialog. It is assumed that validation has already been done (see OnOK).
        /// </summary>
        /// <returns></returns>
        internal PointFeature Save()
        {
            // Handle 3D points some other day
            if (Math.Abs(m_Elevation) > Double.Epsilon)
                throw new NotImplementedException("NewPointForm.Save - 3D points not currently supported");

            NewPointOperation op = null;

            try
            {
                op = new NewPointOperation(Session.WorkingSession, 0);

                IEntity ent = entityTypeComboBox.SelectedEntityType;
                m_PointId.Entity = ent;
                DisplayId did = (DisplayId)idComboBox.SelectedItem;
                if (did != null)
                    m_PointId.ReserveId(ent, did.RawId);

                op.Execute(m_Position, m_PointId);
                return op.Point;
            }

            catch (Exception ex)
            {
                //Session.WorkingSession.Remove(op);
                MessageBox.Show(ex.StackTrace, ex.Message);
            }

            return null;
        }

        private void entityTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            IEntity ent = entityTypeComboBox.SelectedEntityType;
            idComboBox.Enabled = (ent!=null && ent.IdGroup!=null);
            if (idComboBox.Enabled)
            {
                if (IdHelper.LoadIdCombo(idComboBox, ent, m_PointId, true)==0)
                {
                    MessageBox.Show("IDs have not been allocated. See Edit - ID Allocations");
                    m_Cmd.DialAbort(this);
                    return;
                }

                idComboBox.SelectedItem = idComboBox.Items[0];

                // If we are auto-numbering, disable the ID combo.
                EditingController controller = m_Cmd.Controller;
                if (controller.IsAutoNumber)
                    idComboBox.Enabled = false;
            }
            else
                idComboBox.Items.Clear();

        }
    }
}
