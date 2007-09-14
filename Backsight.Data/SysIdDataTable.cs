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

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class SysIdDataTable
        {
            internal SysIdRow AddEmptyRow()
            {
                SysIdRow result = NewSysIdRow();
                result.LastId = 0;
                AddSysIdRow(result);
                return result;
            }

            /*
            /// <summary>
            /// Reserves the next available ID
            /// </summary>
            /// <returns>The reserved ID</returns>
            public int ReserveId()
            {
                return ReserveIds(1).LowestId;
            }

            /// <summary>
            /// Reserves a packet of IDs
            /// </summary>
            /// <param name="numId">The number of IDs to reserve (>0)</param>
            /// <returns>The reserved packet</returns>
            /// <exception cref="ArgumentException">If the number of IDs to reserve is &lt;=0</exception>
            /// <exception cref="InvalidOperationException">If the SysId table doesn't
            /// appear to contain any rows (it should always hold exactly one row)</exception>
            /// <exception cref="Exception">If the attempted database update didn't alter
            /// exactly one row (which might be the case if another process has requested
            /// IDs).</exception>
            public IdPacket ReserveIds(int numId)
            {
                if (numId<=0)
                    throw new ArgumentException("Number of IDs to reserve must be greater than 0");

                SysIdTableAdapter a = new SysIdTableAdapter();
                if (this.Rows.Count==0 && a.Fill(this)==0)
                    throw new InvalidOperationException("No rows in SysId table");

                int oldValue = this[0].LastId;
                int newValue = oldValue+numId;

                // Attempt to update the database using the last ID that's cached as
                // part of this table. If that fails to update anything, refresh this
                // table and try again. If the second attempt fails, we likely have some
                // real problem (the possibility of a simultaneous request for an ID is
                // assumed to be too remote to be worth handling here).

                int nRows = a.UpdateQuery(newValue, oldValue);
                if (nRows==0)
                {
                    a.ClearBeforeFill = true;
                    a.Fill(this);
                    oldValue = this[0].LastId;
                    newValue = oldValue+numId;

                    nRows = a.UpdateQuery(newValue, oldValue);
                    if (nRows==0)
                        throw new Exception("Failed to reserve ID");
                }

                IdPacket result = new IdPacket(oldValue+1, newValue);
                this[0].LastId = result.HighestId;
                return result;
            }

            /// <summary>
            /// Attempts to release an ID that was previously obtained using <c>ReserveId</c>
            /// </summary>
            /// <param name="id">The ID to release</param>
            /// <returns>True if the ID was release. False if it doesn't match the last
            /// allocated ID.</returns>
            public bool ReleaseId(int id)
            {
                return ReleaseIds(new IdPacket(id, id));
            }

            /// <summary>
            /// Attempts to release the unused portion of an ID packet previously obtained
            /// using <c>ReserveIds</c>.
            /// </summary>
            /// <param name="p">The packet to release (the initial IDs in the packet may have
            /// been used)</param>
            /// <returns>True if the unused portion of the packet was released. False if
            /// the highest ID in the packet doesn't match the last allocated ID held in
            /// the database.</returns>
            /// <exception cref="InvalidOperationException">If the SysId table doesn't
            /// appear to contain any rows (it should always hold exactly one row)</exception>
            public bool ReleaseIds(IdPacket p)
            {
                try
                {
                    SysIdTableAdapter a = new SysIdTableAdapter();
                    if (this.Rows.Count==0 && a.Fill(this)==0)
                        throw new InvalidOperationException("No rows in SysId table");

                    int oldValue = p.HighestId;
                    int newValue = (p.LastNextId==0 ? p.LowestId-1 : p.LastNextId);

                    int nRows = a.UpdateQuery(newValue, oldValue);
                    if (nRows==1)
                    {
                        this[0].LastId = newValue;
                        return true;
                    }

                    return false;
                }

                catch { }
                return false;
            }
             */
        }
    }
}
