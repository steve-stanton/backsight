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
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using Backsight.Forms;
using Backsight.Environment;
using Backsight.Editor.Forms;
using Backsight.Editor.Observations;
using Backsight.Editor.UI;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="28-NOV-1998" />
    /// <summary>
    /// Base class for any class that handles the user interface for an editing command.
    /// </summary>
    abstract class CommandUI : IDisposable
    {
        #region Class data

        /// <summary>
        /// The container that may be used to display any sort of user dialog (null if no dialog is involved)
        /// </summary>
        private readonly IControlContainer m_Container;

        /// <summary>
        /// The ID of the edit this command deals with.
        /// </summary>
        private readonly EditingActionId m_EditId;

        /// <summary>
        /// The active display at the time the command was started.
        /// </summary>
        private readonly ISpatialDisplay m_Draw;

        /// <summary>
        /// The object (if any) that was selected for update (usually an <c>IFeature</c>)
        /// </summary>
        //private ISpatialObject m_Update;

        /// <summary>
        /// The update command that is currently running (null if not an update).
        /// </summary>
        private UpdateUI m_UpdCmd;

        /// <summary>
        /// An operation that is being recalled (null if this is an update).
        /// </summary>
        private readonly Operation m_Recall;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <c>CommandUI</c> that isn't an update or a command recall.
        /// </summary>
        /// <param name="cmdId">The item used to invoke the command.</param>
        protected CommandUI(IControlContainer cc, IUserAction cmdId)
            : this(cc, cmdId, null, null)
        {
        }

        /// <summary>
        /// Creates new <c>CommandUI</c> that isn't an update.
        /// </summary>
        /// <param name="cc">The container that may be used to display any sort of
        /// user dialog (null if no dialog is involved)</param>
        /// <param name="cmdId">The item used to invoke the command.</param>
        /// <param name="update">The object (if any) that was selected for update</param>
        /// <param name="recall">An operation that is being recalled (null if this is an update).</param>
        protected CommandUI(IControlContainer cc, IUserAction cmdId, ISpatialObject update, Operation recall)
        {
            m_Container = cc;
            m_Draw = EditingController.Current.ActiveDisplay;
            //m_Update = update;
            m_UpdCmd = null;
            m_Recall = recall;

            if (cmdId is EditingAction)
                m_EditId = (cmdId as EditingAction).EditId;
            else
                m_EditId = EditingActionId.Null;

            if (cmdId is RecalledEditingAction)
                m_Recall = (cmdId as RecalledEditingAction).RecalledEdit;

            Debug.Assert(m_Draw!=null);
        }

        /// <summary>
        /// Creates new <c>CommandUI</c> for use during an editing update. This doesn't refer to
        /// the UpdateUI itself, it refers to a command that is the subject of the update.
        /// </summary>
        /// <param name="editId">The ID of the edit this command deals with.</param>
        /// <param name="updcmd">The update command (not null) that is controlling this command.</param>
        protected CommandUI(IControlContainer cc, EditingActionId editId, UpdateUI updcmd)
        {
            if (updcmd==null)
                throw new ArgumentNullException();

            m_Container = cc;
            m_EditId = editId;
            m_UpdCmd = updcmd;
            m_Draw = updcmd.ActiveDisplay;
            //m_Update = updcmd.SelectedObject;
            m_Recall = null;
            /*
            this.Update = update;

            if (update==null)
            {
                m_Draw = EditingController.Current.ActiveDisplay;
                m_Recall = null;
            }
             */

            Debug.Assert(m_Draw!=null);
        }

        #endregion

        abstract internal bool Run();
        abstract internal void Paint(PointFeature point);
        abstract internal void MouseMove(IPosition p);

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="p">The position where the click occurred</param>
        /// <returns>True if the command handled the mouse down. False if it did nothing.</returns>
        abstract internal bool LButtonDown(IPosition p);

        abstract internal void LButtonUp(IPosition p);
        abstract internal void LButtonDblClick(IPosition p);
        abstract internal void DialAbort(Control wnd);
        abstract internal bool DialFinish(Control wnd);

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        abstract internal void OnSelectPoint(PointFeature point);

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        abstract internal void OnSelectLine(LineFeature line);

        /// <summary>
        /// Creates any applicable context menu
        /// </summary>
        /// <returns>The context menu (null if the command does not utilize a context menu).</returns>
        abstract internal ContextMenuStrip CreateContextMenu();

        internal ISpatialDisplay ActiveDisplay
        {
            get { return m_Draw; }
            //protected set { m_Draw = value; }
        }

        internal IControlContainer Container
        {
            get { return m_Container; }
        }

        // Try UpdateUI.GetUpdate()
        //internal ISpatialObject SelectedObject
        //{
        //    get { return m_Update; }
        //}

        public UpdateUI Update
        {
            get { return m_UpdCmd; }

            // For use by UpdateUI constructor
            /*
            protected set
            {
                m_UpdCmd = value;

                if (m_UpdCmd==null)
                {
                    m_Draw = null;
                    m_Update = null;
                }
                else
                {
                    m_Draw = m_UpdCmd.ActiveDisplay;
                    m_Update = m_UpdCmd.SelectedObject;
                }
            }
             */
        }

        /// <summary>
        /// Sets any offset that applies to this command. This is a placeholder
        ///	only. Commands that accept an offset must implement their own version (if
        ///	they don't, this version will throw an exception, to indicate that the
        ///	function is missing).
        /// </summary>
        /// <param name="offset">The applicable offset (may be null)</param>
        internal virtual void SetOffset(Offset offset)
        {
            throw new NotImplementedException("CommandUI.SetOffset needs to be implemented by "+GetType().Name);
        }

        /// <summary>
        /// Converts a position from the parent window's logical space into a screen position.
        /// </summary>
        /// <param name="pt">The position to convert, in logical units.</param>
        /// <returns>The position in screen units.</returns>
        internal Point LPToScreen(Point pt)
        {
            throw new NotImplementedException("LPToScreen");
            // I think this is misplaced
            /*
            // Convert into device (client) units.
            LPToDP(lpt,respt);

            // Convert into screen units.
            m_pDraw->ClientToScreen(&respt);
             */
        }

        /*
        //	@mfunc	Convert a position from the parent window's logical
        //			space into a device (client) position.
        //
        //	@parm	The position to convert, in logical units.
        //	@parm	The position to define, in client units.

        void CuiCommand::LPToDP ( const CPoint& lpt
						        , CPoint& respt ) const {

	        CClientDC dc(m_pDraw);
	        m_pDraw->OnPrepareDC(&dc);
	        respt = lpt;
	        dc.LPtoDP(&respt);
        }
         */

        /// <summary>
        /// Ensures the command cursor (if any) is shown. This version does nothing.
        /// Derived classes may override.
        /// </summary>
        internal virtual void SetCommandCursor()
        {
        }

        /// <summary>
        /// Ensures cursor is the "normal" arrow cursor.
        /// </summary>
        protected void SetNormalCursor()
        {
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Remembers that the map has been changed. 
        /// </summary>
        void SetChanged()
        {
            /*
	        CEditDoc* pEditDoc = (CEditDoc*)m_pDraw->GetDocument();
	        pEditDoc->SetChanged();
             */
        }

        /// <summary>
        /// Gets a registry setting.
        /// </summary>
        /// <param name="name">The name of the setting to get.</param>
        /// <returns>The value in the registry (null if not found).</returns>
        string GetSetting(string name)
        {
            /*
	        value = AfxGetApp()->GetProfileString("Settings",name);
	        return (!value.IsEmpty());
             */
            return null;
        }

        /// <summary>
        /// Makes a registry setting.
        /// </summary>
        /// <param name="name">The name of the setting to set.</param>
        /// <param name="value">The value to set (specify an empty string to delete the setting).</param>
        void SetSetting(string name, string value)
        {
            /*
	        if ( value.IsEmpty() )
		        AfxGetApp()->WriteProfileString( "Settings", name, 0 );
	        else
		        AfxGetApp()->WriteProfileString( "Settings", name, LPCTSTR(value) );
             */
        }

        /// <summary>
        /// Gets a registry setting that was set using SetBooleanSetting.
        /// </summary>
        /// <param name="name">The name of the setting to get.</param>
        /// <returns>True if the setting was set, and has a "y" value. False if no
        /// setting, or the value is not "y".</returns>
        bool GetBooleanSetting(string name)
        {
            /*
	        CString value;
	        return (GetSetting(name,value) && value[0]=='y');
             */
            return false;
        }

        /// <summary>
        /// Makes a registry setting for a logical variable.
        /// </summary>
        /// <param name="name">The name of the setting to set.</param>
        /// <param name="value">The value to set.</param>
        void SetBooleanSetting(string name, bool value)
        {
            /*
	        if ( value ) {
		        CString str("y");
		        SetSetting(name,str);
	        }
	        else {
		        CString str("n");
		        SetSetting(name,str);
	        }
             */
        }

        internal EditingController Controller
        {
            get { return EditingController.Current; }
        }

        /// <summary>
        /// Shortcut to <c>Controller.ActiveLayer</c> (the map layer currently
        /// designated as the editing layer)
        /// </summary>
        internal ILayer ActiveLayer
        {
            get { return Controller.ActiveLayer; }
        }

        /// <summary>
        /// Finishes this command.
        /// </summary>
        /// <returns>True (always).</returns>
        protected bool FinishCommand()
        {
	        // If this command was invoked by an update command, get
	        // the update to clean up. Otherwise tell the controller.

	        if (m_UpdCmd!=null)
		        m_UpdCmd.FinishCommand(this);
	        else
                this.Controller.FinishCommand(this);

	        return true;
        }

        /// <summary>
        /// Aborts this command.
        /// </summary>
        /// <returns>True (always).</returns>
        protected bool AbortCommand()
        {
	        // If this command was invoked by an update command, get
	        // the update to clean up. Otherwise tell the controller.

            if (m_UpdCmd!=null)
                m_UpdCmd.AbortCommand(this);
            else
                this.Controller.AbortCommand(this);

            // Ensure that any reserved IDs have been released
            IdManager idMan = CadastralMapModel.Current.IdManager;
            if (idMan != null)
                idMan.FreeAllReservedIds();

	        return true;
        }

        /// <summary>
        /// Default handling for selection of a line feature during updates. This checks
        /// whether the specified line is dependent on the update that is being made --
        /// if so, a message is issued. If there is no dependency, the selection is passed
        /// to <c>OnSelectArc</c>
        ///
        /// Derived classes may overide in order to do things like substituting an alternate
        /// line in place of the one the user has selected.
        /// </summary>
        /// <param name="line">The line selected by the user</param>
        void OnUpdateSelect(LineFeature line)
        {
            Debug.Assert(m_UpdCmd!=null);

	        // Process the supplied line if it doesn't appear
	        // to depend on the current update

	        if ( m_UpdCmd.IsDependent(line) )
		        m_UpdCmd.DependencyMessage();
	        else
		        OnSelectLine(line);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            if (m_Container!=null)
                m_Container.Clear();
        }

        #endregion

        internal EditingActionId EditId
        {
            get { return m_EditId; }
        }
        /*
	virtual CeObject*		GetUpdate			( void ) const { return m_pUpdate; }
	virtual CuiUpdate*		GetUpdCmd			( void ) const { return m_pUpdCmd; }
         */

        internal Operation Recall
        {
            get { return m_Recall; }
        }

        /// <summary>
        /// The diagonal length of a line that spans the active display when it is
        /// drawn at the overview scale.
        /// </summary>
        /// <returns>The maximum length, in meters on the ground</returns>
        internal double MaxDiagonal
        {
            get
            {
                ISpatialDisplay display = ActiveDisplay;
                IWindow x = display.MaxExtent;
                return Geom.Distance(x.Min, x.Max);
            }
        }

        /// <summary>
        /// Does this command perform any command-specific painting? If so, it's <c>Paint</c>
        /// method will be called during idle time (see <c>EditingController.OnIdle</c>).
        /// This implementation returns false. Derived classes may override.
        /// <para/>
        /// While it is expected that most deribed classes will return a true result, it is
        /// possible that a command only performs painting at certain stages.
        /// </summary>
        internal virtual bool PerformsPainting
        {
            get { return false; }
        }

        /// <summary>
        /// Indicates that any painting previously done by a command should be erased. This
        /// tells the command's active display that it should revert the display buffer to
        /// the way it was at the end of the last draw from the map model. Given that a command
        /// supports painting, it's <c>Paint</c> method will be called during idle time.
        /// </summary>
        internal void ErasePainting()
        {
            m_Draw.RestoreLastDraw();
        }

        /// <summary>
        /// Converts a ground position into screen coordinates.
        /// </summary>
        /// <param name="p">The position to convert</param>
        /// <returns>The corresponding screen position</returns>
        internal Point GetScreenPoint(IPosition p)
        {
            ISpatialDisplay d = ActiveDisplay;
            int x = (int)d.EastingToDisplay(p.X);
            int y = (int)d.NorthingToDisplay(p.Y);
            Control c = d.MapPanel;
            return c.PointToScreen(new Point(x,y));
        }

        ///// <summary>
        ///// The background color of the display that's being used for this command.
        ///// </summary>
        //internal Color DisplayBackColor
        //{
        //    get
        //    {
        //        ISpatialDisplay d = ActiveDisplay;
        //        Control c = d.MapPanel;
        //        return c.BackColor;
        //    }
        //}

        /// <summary>
        /// Reacts to a situation where the user presses the ESC key. This implementation
        /// does nothing. Derived classes may override.
        /// </summary>
        /// <remarks>
        /// Use of the escape key will generally be limited to commands that display some
        /// sort of non-standard cursor, but which do not provide any user dialog. Commands
        /// with dialogs are usually escaped by clicking on a Cancel button.
        /// </remarks>
        internal virtual void Escape()
        {
            // do nothing
        }

        /// <summary>
        /// Highlights the specified line on the command's active display.
        /// </summary>
        /// <param name="line">The line to highlight (ignored if null)</param>
        protected void Highlight(LineFeature line)
        {
            if (line!=null)
            {
                IDrawStyle style = Controller.HighlightStyle;
                ISpatialDisplay display = ActiveDisplay;
                line.Render(display, style);
            }
        }

        /// <summary>
        /// Checks whether points are currently drawn on the active display
        /// </summary>
        /// <returns>True if points are drawn. False if not drawn. The result
        /// depends on the current display scale & the point drawing scale threshold
        /// that's noted as part of the current map model.
        /// </returns>
        internal bool ArePointsDrawn()
        {
            return EditingController.Current.ArePointsDrawn;
        }
    }
}
