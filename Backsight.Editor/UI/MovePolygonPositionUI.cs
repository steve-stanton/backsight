// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Editor.Operations;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="20-JAN-2009"/>
    /// <summary>
    /// User interface for moving the reference position of a polygon label
    /// </summary>
    class MovePolygonPositionUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The polygon label that's being modified (not null)
        /// </summary>
        readonly TextFeature m_Text;

        /// <summary>
        /// The last polygon that the polygon position was inside (could be null)
        /// </summary>
        Polygon m_LastPolygon;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>MovePolygonPositionUI</c>
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="text">The text to move (not null)</param>
        internal MovePolygonPositionUI(IControlContainer cc, IUserAction action, TextFeature text)
            : base(cc, action)
        {
            if (text == null)
                throw new ArgumentNullException();

            m_Text = text;
            m_LastPolygon = m_Text.Container;
        }

        #endregion

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns>True (always)</returns>
        internal override bool Run()
        {
            SetCommandCursor();
            return true;
        }

        /// <summary>
        /// Ensures the command cursor (if any) is shown.
        /// </summary>
        internal override void SetCommandCursor()
        {
            ActiveDisplay.MapPanel.Cursor = EditorResources.AttachPointCursor;
        }

        /// <summary>
        /// Handles mouse-move.
        /// </summary>
        /// <param name="p">The new position of the mouse</param>
        internal override void MouseMove(IPosition p)
        {
            if (m_LastPolygon == null || !m_LastPolygon.IsEnclosing(p))
            {
                IPointGeometry pg = PointGeometry.Create(p);
                ISpatialIndex index = CadastralMapModel.Current.Index;
                Polygon pol = new FindPointContainerQuery(index, pg).Result;

                if (pol != null || (m_LastPolygon != null && pol == null))
                {
                    Controller.ActiveDisplay.RestoreLastDraw();
                    m_LastPolygon = pol;
                }
            }
        }

        /// <summary>
        /// Do any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn.
        /// Not used.</param>
        internal override void Paint(PointFeature point)
        {
            ISpatialDisplay display = ActiveDisplay;
            IDrawStyle highlighter = Controller.HighlightStyle;
            m_Text.Render(display, highlighter);

            if (m_LastPolygon != null)
                m_LastPolygon.Render(display, highlighter);
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
        /// Handles a mouse down event
        /// </summary>
        /// <param name="p">The position where the click occurred</param>
        /// <returns>True, indicating that the text was moved.</returns>
        internal override bool LButtonDown(IPosition p)
        {
            MovePolygonPositionOperation op = null;

            try
            {
                op = new MovePolygonPositionOperation(m_Text);
                op.Execute(PointGeometry.Create(p));
                FinishCommand();
                return true;
            }

            catch (Exception ex)
            {
                //Session.CurrentSession.Remove(op);
                MessageBox.Show(ex.StackTrace, ex.Message);
            }

            return false;
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
