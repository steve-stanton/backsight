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
using System.IO;
using System.Collections.Generic;

using Backsight.Editor.Properties;

namespace Backsight.Editor
{
    /// <summary>
    /// A tool for managing automatic saves and map backups.
    /// </summary>
    class AutoSaver
    {
        #region Statics

        internal static uint MaxBackup
        {
            get { return Settings.Default.MaxBackup; }
            set
            {
                Settings.Default.MaxBackup = value;
                Settings.Default.Save();
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// Is the user currently in update mode?
        /// </summary>
        bool m_IsUpdate;

        /// <summary>
        /// Some numeric value that identifies an edit that is in progress.
        /// </summary>
        int m_EditId;

        /// <summary>
        /// The operation sequence numbers of backups that exist for the current map.
        /// </summary>
        List<uint> m_Backups;

        /// <summary>
        /// The number of edits that have been performed while working with the current map.
        /// </summary>
        uint m_NumEdit;

        /// <summary>
        /// The controller that is making use of this AutoSaver.
        /// </summary>
        readonly CadastralEditController m_Controller;

        #endregion

        internal AutoSaver(CadastralEditController c)
        {
            m_Controller = c;
            m_IsUpdate = false;
            m_EditId = 0;
            m_Backups = new List<uint>();
            m_NumEdit = 0;
        }

        /// <summary>
        /// Reacts to a start-editing event. This function should be called before any changes
        /// are made to the current map as part of an editing operation.
        /// </summary>
        /// <param name="edid">Some unique value that the caller knows the edit by. Should not be zero.
        /// Don't lose it, because the same value must later be supplied to finish the edit.</param>
        internal void StartEdit(CommandUI cmd)
        {
            m_NumEdit++;

            // Remember the edit ID if it's not already defined.
            if (m_EditId==0)
                m_EditId = (int)cmd.EditId;

            // Do nothing if update mode is active.
            if (m_IsUpdate)
                return;

            // If a backup file has not been produced, do it now.
            if (m_NumEdit==1)
                MakeBackup();
        }

        /// <summary>
        /// Reacts to a finish-editing event. This function should be called after an editing
        /// operation has been appended to the current map.
        /// </summary>
        /// <param name="edid">Some unique value that the caller knows the edit by. Should not be zero.</param>
        internal void FinishEdit(CommandUI cmd)
        {
	        // Return if the specified ID is not the one that started things going.
            int edid = (int)cmd.EditId;
	        if ( edid != m_EditId ) return;

	        m_EditId = 0;

	        // Do nothing if update mode is active.
	        if ( m_IsUpdate ) return;

	        // Save the current model and mark it as un-modified.
            SaveModel();
        }

        /// <summary>
        /// Reacts to an abort-editing event. This function should be called after an editing
        /// operation has been cancelled
        /// </summary>
        /// <param name="edid">Some unique value that the caller knows the edit by. Should not be zero.</param>
        internal void AbortEdit(CommandUI cmd)
        {
            int edid = (int)cmd.EditId;
            if (edid == m_EditId)
                m_EditId = 0;
        }

        private CadastralMapModel Model
        {
            get { return (CadastralMapModel)SpatialController.Current.MapModel; }
        }

        private void SaveModel()
        {
            m_Controller.AutoSave();
        }

        /// <summary>
        /// Extended handler for <c>CadastralEditController.Create</c>. This should be called as soon as
        /// the new map has been created, but before ANYTHING else has been done to it.
        /// </summary>
        internal void OnNew()
        {
            // Revert to initial state.
            Reset();

            // Save the model now
            SaveModel();
        }

        /// <summary>
        /// Extended handler for CEditDoc::OnOpenDocument. This should be called as soon as
        /// the map has been opened, and before ANYTHING else has been done to it. This function
        /// will query CEditDoc for the map, and use its file specfication to search for any
        /// backup files that the map already has.
        /// </summary>
        internal void OnOpen()
        {
	        // Revert to initial state.
	        Reset();

            // Get the file spec for the map.
            string spec = this.Model.Name;
            if (String.IsNullOrEmpty(spec))
                return;

            // Collect a list of any backup files.
            string pattern = Path.GetFileName(spec) + "*";
            string [] files = Directory.GetFiles(Path.GetDirectoryName(spec), pattern, SearchOption.TopDirectoryOnly);
            foreach (string f in files)
            {
                // Get the file extension (which includes a leading dot) and strip off the
                // ".4S" part. The remainder should be the sequence number for the backup.
                string ext = Path.GetExtension(f);
                uint num;
                if(UInt32.TryParse(ext.Substring(3), out num))
                    m_Backups.Add(num);
            }
        }

        /// <summary>
        /// Extended handler for CEditDoc::OnCloseDocument. This should be called
        /// immediately BEFORE the map is closed.
        /// </summary>
        internal void OnClose()
        {
        	Reset();
	        m_Controller.AutoSave();
        }

        /// <summary>
        /// Resets everything to initial state.
        /// </summary>
        private void Reset()
        {
            m_Backups = new List<uint>();
            m_NumEdit = 0;
            m_EditId = 0;
            m_IsUpdate = false;
        }

        /// <summary>
        /// Creates a backup file.
        /// </summary>
        private void MakeBackup()
        {
            // Get the model to make a backup file. Return if it didn't actually get created.
            uint iBack = this.Model.MakeBackup();
            if ( iBack==0 ) return;

            // Remember the sequence number that got assigned.
            m_Backups.Add(iBack);

            // Get the max number of backups that should be retained.
            uint maxBack = AutoSaver.MaxBackup;

            // Treat zero as "no limit"
            if ( maxBack==0 ) return;

            // How many backups do we currently have?
            int nBack = m_Backups.Count;

            // If we've got too many, get rid of the earliest one.

            if ( nBack>maxBack )
            {
		        uint minseq = m_Backups[0];
		        int mindex = 0;

		        for ( int i=1; i<nBack; i++ )
                {
			        if ( m_Backups[i]<minseq )
                    {
				        minseq = m_Backups[i];
				        mindex = i;
			        }
    		    }

		        m_Backups.RemoveAt(mindex);
                string spec = String.Format("{0}{1:D10}", this.Model.Name, minseq);
                File.Delete(spec);
	        }
        }

        /// <summary>
        /// Notes the fact that the user has initiated an update to an old editing operation.
        /// Auto-saves will be inhibited while update mode remains in effect.
        /// </summary>
        internal void OnStartUpdate()
        {
            m_IsUpdate = true;
        }

        /// <summary>
        /// Notes the fact that the user has finished making updates to old editing operations.
        /// If auto-save is actually enabled, the map will be saved.
        /// </summary>
        internal void OnFinishUpdate()
        {
            m_IsUpdate = false;
            SaveModel();
        }
    }
}
