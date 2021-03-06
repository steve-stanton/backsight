// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
using System.Diagnostics;
using System.Windows.Forms;

using Backsight;


namespace CadastralViewer
{
	/// <written by="Steve Stanton" on="01-MAR-2010" />
    /// <summary>
    /// Controller for the CadastralViewer application.
    /// </summary>
    class ViewController : SpatialController
    {
        #region Class data

        /// <summary>
        /// The main window of the application.
        /// </summary>
        readonly MainForm m_Main;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ViewController</c>
        /// </summary>
        /// <param name="main">The main window for the application (not null)</param>
        internal ViewController(MainForm main)
        {
            if (main==null)
                throw new ArgumentNullException();

            m_Main = main;
        }
        
        #endregion
        
        /// <summary>
        /// Override calls <c>SpatialController.MouseMove</c>, and additionally tells
        /// the application's main window (the main window may also need to display the
        /// current mouse position).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="p"></param>
        /// <param name="b"></param>
        public override void MouseMove(ISpatialDisplay sender, IPosition p, MouseButtons b)
        {
            m_Main.OnMouseMove(sender, p, b);
            base.MouseMove(sender, p, b);
        }

        /// <summary>
        /// Override does the same as <c>SpatialController.SetSelection</c>, and additionally
        /// tells the application's main window (the main window goes on to display a property
        /// grid for the current selection).
        /// </summary>
        public override bool SetSelection(ISpatialSelection newSel)
        {
            if (!base.SetSelection(newSel))
                return false;

            if (m_Main!=null)
            {
                Debug.Assert(newSel!=null);
                ISpatialObject so = newSel.Item;
                m_Main.SetSelection(so);
            }

            return true;
        }

        /// <summary>
        /// Creates a context menu by getting the application's main window to display a
        /// context menu that is appropriate for the current selection (if any).
        /// This overrides the do-nothing implementation provided by <c>SpatialController</c>.
        /// </summary>
        /// <param name="where">The display where the context menu should appear</param>
        /// <param name="p">The preferred position for the menu (may not be honoured if
        /// the context menu would be obscured)</param>
        public override void ShowContextMenu(ISpatialDisplay where, IPosition p)
        {
            ContextMenuStrip menu = m_Main.CreateContextMenu(SpatialSelection);
            where.ShowContextMenu(p, menu);
        }
    }
}
