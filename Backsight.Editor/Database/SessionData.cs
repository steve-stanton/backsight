// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;

using Backsight.Data;
using Backsight.Environment;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Information about an editing session
    /// </summary>
    class SessionData
    {
        #region Class data

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        readonly uint m_SessionId;

        /// <summary>
        /// The ID of the job the session is part of
        /// </summary>
        readonly uint m_JobId;

        /// <summary>
        /// The ID of the user running the session
        /// </summary>
        readonly uint m_UserId;

        /// <summary>
        /// When was session started? 
        /// </summary>
        readonly DateTime m_Start;

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        DateTime m_End;

        /// <summary>
        /// The number of items (objects) created by the session
        /// </summary>
        uint m_NumItem;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SessionData</c> using information selected from the database
        /// </summary>
        /// <param name="sessionId">Unique ID for the session.</param>
        /// <param name="jobId">The ID of the job the session is part of</param>
        /// <param name="userId">The ID of the user running the session</param>
        /// <param name="start">When was session started? </param>
        /// <param name="end">When was the last edit performed?</param>
        /// <param name="numItem">TThe number of items (objects) created by the session</param>
        internal SessionData(uint sessionId, uint jobId, uint userId, DateTime start, DateTime end, uint numItem)
        {
            m_SessionId = sessionId;
            m_JobId = jobId;
            m_UserId = userId;
            m_Start = start;
            m_End = end;
            m_NumItem = numItem;
        }

        #endregion

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        internal uint Id
        {
            get { return m_SessionId; }
        }

        /// <summary>
        /// The ID of the user running the session
        /// </summary>
        internal uint UserId
        {
            get { return m_UserId; }
        }

        /// <summary>
        /// The ID of the job the session is part of
        /// </summary>
        internal uint JobId
        {
            get { return m_JobId; }
        }

        /// <summary>
        /// When was session started? 
        /// </summary>
        internal DateTime StartTime
        {
            get { return m_Start; }
        }

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        internal DateTime EndTime
        {
            get { return m_End; }
        }

        /// <summary>
        /// The number of items (objects) created by the session
        /// </summary>
        internal uint NumItem
        {
            get { return m_NumItem; }
            set { m_NumItem = value; }
        }

        /// <summary>
        /// Deletes this session from the database. This will succeed only
        /// if no edits have been performed as part of the session.
        /// </summary>
        internal void Delete()
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                string sql = String.Format("DELETE FROM [ced].[Sessions] WHERE [SessionId]={0}",
                                                m_SessionId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates the end-time associated with this session. This should be
        /// called at the end of each editing operation.
        /// </summary>
        internal void UpdateEndTime()
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                DateTime now = DateTime.Now;
                string nowString = DbUtil.GetDateTimeString(now);
                string sql = String.Format("UPDATE [ced].[Sessions] SET [EndTime]={0}, [NumItem]={1}" +
                                            " WHERE [SessionId]={2}", nowString, m_NumItem, m_SessionId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
                m_End = now;
            }
        }

        /// <summary>
        /// Reserves an item number for use with this session. This should only
        /// be called for the currently active session. It is a lightweight
        /// request, because it just increments a counter. The database will not
        /// get updated until <see cref="UpdateEndTime"/> is called.
        /// </summary>
        /// <returns>The reserved item number</returns>
        internal uint ReserveNextItem()
        {
            Debug.Assert(m_SessionId == Session.WorkingSession.Id);
            m_NumItem++;
            return m_NumItem;
        }

        /// <summary>
        /// Gets rid of edits that the user has not explicitly saved. This gets rid of the edits,
        /// but does not revert the item count of update time that is stored in the <c>Sessions</c>
        /// table (that isn't considered to be bad).
        /// </summary>
        /// <param name="lastItemToKeep">The item number of the last thing that needs
        /// to be kept</param>
        internal void DiscardEdits(uint lastItemToKeep)
        {
            Transaction.Execute(delegate
            {
                // Get rid of the edits
                string sql = String.Format("DELETE FROM [ced].[Edits] WHERE [SessionId]={0} AND [EditSequence]>{1}",
                                                m_SessionId, lastItemToKeep);
                SqlCommand cmd = new SqlCommand(sql, Transaction.Connection.Value);
                cmd.ExecuteNonQuery();

                // Go back to the old item count (and update session time)
                m_NumItem = lastItemToKeep;
                UpdateEndTime();
            });
        }
    }
}
