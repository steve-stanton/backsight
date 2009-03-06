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

using Backsight.Forms;
using Backsight.Editor.Forms;
using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Editor.Observations;

namespace Backsight.Editor
{
    class RadialUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The point the sideshot is from.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The dialog for the command.
        /// </summary>
        private RadialControl m_Dialog;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a fresh sideshot.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="from">The point the sideshot is from.</param>
        internal RadialUI(IControlContainer cc, IUserAction action, PointFeature from)
            : base(cc, action)
        {
            m_Dialog = null;
            m_From = from;
        }
    
        /// <summary>
        /// Constructor for command recall.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="op">The operation that's being recalled.</param>
        /// <param name="from">Alternate from-point. Specify null to use the
        /// same from point as the recalled op.</param>
        internal RadialUI(IControlContainer cc, IUserAction action, Operation op, PointFeature from)
            : base(cc, action, null, op)
        {
            m_Dialog = null;
            m_From = from;
        }
    
        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="updcmd">The update command.</param>
        internal RadialUI(IControlContainer cc, IUserAction action, UpdateUI updcmd)
            : base(cc, action, updcmd)
        {
            m_Dialog = null;
            m_From = null;
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose(); // removes any controls from container

            if (m_Dialog!=null)
            {
                m_Dialog.Dispose();
                m_Dialog = null;
            }
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
	        // Don't run more than once.
	        if (m_Dialog!=null)
                throw new InvalidOperationException("RadialUI.Run - Command is already running.");

            UpdateUI pup = this.Update;

            if (pup!=null)
                m_Dialog = new RadialControl(pup);
            else
            {
                if (m_From==null)
                {
                    RadialOperation recall = (RadialOperation)this.Recall;
                    m_From = recall.From;
                }

                m_Dialog = new RadialControl(this, m_From);
            }

            this.Container.Display(m_Dialog);
            return true;
        }

        /// <summary>
        /// Do any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn.
        /// Not used.</param>
        internal override void Paint(PointFeature point)
        {
            if (m_Dialog!=null)
                m_Dialog.Draw();
        }

        /// <summary>
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <c>Paint</c> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return true; }
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            if (m_Dialog!=null)
                m_Dialog.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            if (m_Dialog!=null)
                m_Dialog.OnSelectLine(line);
        }

        /// <summary>
        /// Reacts to selection of the Cancel button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window. If this matches the dialog that
        /// this command knows about, the dialog will be destroyed and the command
        /// terminates. If it's some other window, it must be a sub-dialog created
        /// by our guy, so let it handle the request.</param>
        internal override void DialAbort(Control wnd)
        {
            if (m_Dialog==wnd) // if it's the command dialog ...
            { 
                // Get the base class to destroy this command.
                AbortCommand();
            }
            else if (m_Dialog!=null)
            {
                // Must be a sub-dialog.
                m_Dialog.DialAbort(wnd);
            }
        }

        /// <summary>
        /// Reacts to selection of the OK button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window. If this matches the dialog that
        /// this command knows about, the command will be executed (and, on success,
        /// the dialog will be destroyed). If it's some other window, it must
        /// be a sub-dialog created by our guy, so let it handle the request.</param>
        /// <returns></returns>
        internal override bool DialFinish(Control wnd)
        {
	        if (m_Dialog==null)
            {
		        MessageBox.Show("RadialUI.DialFinish - No dialog!");
		        return false;
	        }

	        // Handle any sub-dialog request.
	        if (m_Dialog != wnd )
                return m_Dialog.DialFinish(wnd);

	        // If we are doing an update, alter the original operation.
	        UpdateUI pup = this.Update;

	        if (pup!=null)
            {
                RadialOperation pop = (pup.GetOp() as RadialOperation);
        		if (pop==null)
                {
			        MessageBox.Show("RadialUI.DialFinish - Unexpected edit type.");
			        return false;
		        }

		        // Get info from the dialog.
		        Direction dir = m_Dialog.Direction;
		        Observation len = m_Dialog.Length;

        		// The direction and length must both be defined.
		        if (dir==null || len==null)
                {
			        MessageBox.Show("Missing parameters for sideshot update.");
			        return false;
		        }

		        // Make the update.
		        pop.Correct(dir, len);
	        }
	        else
            {
        		// Get info from the dialog.
		        Direction dir = m_Dialog.Direction;
		        Observation len = m_Dialog.Length;
		        IdHandle idh = m_Dialog.PointId;

		        IEntity lineEnt = null;
		        if (m_Dialog.WantLine)
                {
                    EditingController ec = EditingController.Current;
                    lineEnt = ec.ActiveLayer.DefaultLineType;
                    if (lineEnt==null)
                        throw new InvalidOperationException("No default entity type for lines");
                }

		        // The direction, length, and point entity type must all be defined.
		        if (dir==null || len==null || idh.Entity==null)
                {
			        MessageBox.Show("Missing parameters for sideshot creation.");
		        	return false;
		        }

		        // Create empty persistent object (adds to current session)
                RadialOperation pop = new RadialOperation(Session.WorkingSession);

		        // Execute the operation.
		        bool ok = pop.Execute(dir, len, idh, lineEnt );

                if (!ok)
                    Session.WorkingSession.Remove(pop);
        	}

	        // Get the base class to finish up.
	        return FinishCommand();
        }

        /// <summary>
        /// Calculates the position of the sideshot point.
        /// </summary>
        /// <param name="dir">The direction observation (if any).</param>
        /// <param name="len">The length observation (if any). Could be a <c>Distance</c> or an
        /// <c>OffsetPoint</c>.</param>
        /// <returns>The position of the sideshot point (null if there is insufficient data
        /// to calculate a position)</returns>
        internal static IPosition Calculate(Direction dir, Observation len)
        {
            // Return if there is insufficient data.
            if (dir==null || len==null)
                return null;

            // Get the position of the point the sideshot should radiate from.
            PointFeature from = dir.From;

            // Get the position of the start of the direction line (which may be offset).
            IPosition start = dir.StartPosition;

            // Get the bearing of the direction.
            double bearing = dir.Bearing.Radians;

            // Get the length of the sideshot arm.
            double length = len.GetDistance(from).Meters;

            // Calculate the resultant position. Note that the length is the length along the
            // bearing -- if an offset was specified, the actual length of the line from-to =
            // sqrt(offset*offset + length*length)
            IPosition to = Geom.Polar(start, bearing, length);

            // Return if the length is an offset point. In that case, the length we have obtained
            // is already a length on the mapping plane, so no further reduction should be done
            // (although it's debateable).
            if (len is OffsetPoint)
                return to;

            // Using the position we've just got, reduce the length we used to a length on the
            // mapping plane (it's actually a length on the ground).
            ICoordinateSystem sys = CadastralMapModel.Current.CoordinateSystem;
            double sfac = sys.GetLineScaleFactor(start, to);
            return Geom.Polar(start, bearing, length*sfac);
        }

        /// <summary>
        /// Accepts a new offset.
        /// </summary>
        /// <param name="offset"></param>
        internal override void SetOffset(Offset offset)
        {
            if (m_Dialog!=null)
                m_Dialog.SetOffset(offset);
        }
    }
}
