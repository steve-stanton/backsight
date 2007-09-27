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

using Backsight.Forms;
using Backsight.Geometry;
using System.Windows.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="26-SEP-2007" />
    /// <summary>
    /// User interface for subdividing a polygon.
    /// </summary>
    class PolygonSubdivisionUI : SimpleCommandUI
    {
        #region Class data

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates a new <c>PolygonSubdivisionUI</c>
        /// </summary>
        internal PolygonSubdivisionUI(IUserAction action)
            : base(action)
        {
        }

        #endregion

        internal override bool Run()
        {
            SetCommandCursor();
            return true;
        }

        internal override void SetCommandCursor()
        {
            ActiveDisplay.MapPanel.Cursor = EditorResources.PolygonSubdivisionCursor;
        }

        internal override void LButtonDown(IPosition p)
        {
            // Find out what polygon we need to subdivide
            IPointGeometry pg = PointGeometry.Create(p);
            ISpatialIndex index = CadastralMapModel.Current.Index;
            Polygon pol = new FindPointContainerQuery(index, pg).Result;
            if (pol==null)
            {
                MessageBox.Show("Specified position does not fall inside any polygon.");
                return;
            }


            try
            {
                // Form the links. Return if we didn't find any links.
                PolygonSub sub = new PolygonSub(pol);
                PointFeature start, end;
                if (!sub.GetLink(0, out start, out end))
                {
                    MessageBox.Show("Cannot locate any points to connect.");
                    return;
                }

                MessageBox.Show("ok");
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            /*

	StartEdit((INT4)pPolygon);

//	Do the subdivision
	if ( !pPolygon->Subdivide() )
		AfxMessageBox ( "Cannot locate any points to connect." );

	FinishEdit((INT4)pPolygon);

	return TRUE;
             */
            FinishCommand();
        }
        /// <summary>
        /// Reacts to a situation where the user presses the ESC key, by aborting this command.
        /// </summary>
        internal override void Escape()
        {
            AbortCommand();
        }
    }
}
