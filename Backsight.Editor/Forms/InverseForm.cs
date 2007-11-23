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
using System.Drawing;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Base class for dialogs dealing with inverse calculations.
    /// </summary>
    partial class InverseForm : Form
    {
        #region Class data

        /// <summary>
        /// The current display unit for distances.
        /// </summary>
        DistanceUnit m_Unit;

        /// <summary>
        /// The last distance returned by the <see cref="Format"/> function.
        /// </summary>
        string m_Distance;

        #endregion

        #region Constructors

        protected InverseForm()
        {
            InitializeComponent();
        }

        #endregion

        #region To be implemented by derived classes

        // The following methods should ideally be declared as abstract methods.
        // To do that, however, you would have to make this class abstract, which
        // the Visual Studio IDE doesn't like (it says it can't instantiate an
        // instance of an abstract class).

        internal virtual void OnSelectPoint(PointFeature point)
        {
            throw new NotImplementedException();
        }

        internal virtual void InitializeUnits(DistanceUnit units)
        {
            throw new NotImplementedException();
        }

        internal virtual void Draw()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// The three standard colors used by derived dialogs.
        /// </summary>
        protected Color[] InverseColors
        {
            get
            {
                return new Color[] { Color.FromArgb(255, 0, 128), 
                                     Color.FromArgb(255, 128, 255),
                                     Color.FromArgb(255, 200, 255) };
            }
        }

        private void InverseForm_Shown(object sender, EventArgs e)
        {
            // Display at top left corner of the screen.
            //this->SetWindowPos( &wndTopMost, 0,0, 0,0, SWP_NOSIZE );

            // Display the adjustment results in the current display
            // units. If the display units are "as entered", use
            // the current data entry units instead.

            CadastralMapModel map = CadastralMapModel.Current;
            if (map!=null)
            {
                DistanceUnit display = map.DisplayUnit;
                m_Unit = (display.UnitType == DistanceUnitType.AsEntered ? map.EntryUnit : display);
                InitializeUnits(m_Unit);
            }
        }

        protected void SetMeters()
        {
            SetUnit(DistanceUnitType.Meters);
        }

        protected void SetFeet()
        {
            SetUnit(DistanceUnitType.Feet);
        }

        protected void SetChains()
        {
            SetUnit(DistanceUnitType.Chains);
        }

        void SetUnit (DistanceUnitType type)
        {
            CadastralMapModel map = CadastralMapModel.Current;
            m_Unit = map.GetUnits(type);
        }

        /// <summary>
        /// Returns a formatted distance string.
        /// </summary>
        /// <param name="metric">The distance on the mapping plane (in meters).</param>
        /// <param name="from">The start position.</param>
        /// <param name="to">The end position.</param>
        /// <returns>The formatted distance</returns>
        protected string Format(double metric, IPosition from, IPosition to)
        {
            if (m_Unit==null)
            {
                m_Distance = String.Empty;
            }
            else
            {
                // Get the fixed number of significant digits to show.
                int prec = -1;

                if (m_Unit.UnitType == DistanceUnitType.Meters)
                    prec = 3;
                else if (m_Unit.UnitType == DistanceUnitType.Feet)
                    prec = 2;
                else if (m_Unit.UnitType == DistanceUnitType.Chains)
                    prec = 4;

                // Get string for the planar distance.
                string pstring = m_Unit.Format(metric, true, prec);

                // Get string for the ground distance (don't bother with
                // units abbreviation).
                ICoordinateSystem sys = CadastralMapModel.Current.CoordinateSystem;
                double sfac = sys.GetLineScaleFactor(from, to);
                double gmetric = metric/sfac;
                string gstring = m_Unit.Format(gmetric, false, prec);

                // Format the complete string.
                m_Distance = String.Format("{0} ({1} on ground)", pstring, gstring);
            }

            return m_Distance;
        }

        protected string GetPointText(PointFeature p)
        {
            if (p==null)
                return String.Empty;

            string result = p.FormattedKey;
            return (result.Length > 0 ? result : p.DataId);
        }

        /// <summary>
        /// Ensures that any special painting on the map display has been cleared.
        /// </summary>
        protected void ErasePainting()
        {
            ISpatialDisplay display = EditingController.Current.ActiveDisplay;
            display.RestoreLastDraw();
        }
    }
}