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
using System.Collections.Generic;

using Backsight.Environment;

namespace Backsight.Editor
{
    class IdHelper
    {
        /// <summary>
        /// Load an ID combo box with all the available IDs for a specific entity type & map layer.
        /// </summary>
        /// <param name="box">The combo box (not null)</param>
        /// <param name="ent">The entity type that the combo is for (not null)</param>
        /// <param name="canAllocate">Allocate IDs if nothing available?</param>
        /// <returns>The number of IDs that were loaded into the combo (if any). A value of
        /// zero could mean that the entity type isn't meant to have IDs (so the caller should
        /// probably go on to disable the ID combo).</returns>
        /*
        internal static int LoadIdCombo(ComboBox box, IEntity ent, bool canAllocate)
        {
            if (box==null || ent==null)
                throw new ArgumentNullException();

            // Clear out anything that was in the combo before.
            box.Items.Clear();

            // Get a list of all the available IDs for the specified entity type...

            // Get the ID group for the specified entity type.
            IdManager idMan = IdManager.Current;
            IdGroup group = idMan.GetGroup(ent);
            if (group==null)
                return 0;

            // Get the ID ranges for the group
            List<IdRange> ranges = group.IdRanges;
            List<uint> avail = new List<uint>(128);

            foreach (IdRange range in ranges)
            {
                avail.Clear();
                if (range.GetAvailIds(avail, group)==0)
                    continue;

                DisplayId[] ids = new DisplayId[avail.Count];
                for (int i=0; i<ids.Length; i++)
                    ids[i] = new DisplayId(range, avail[i]);

                box.Items.AddRange(ids);
            }

            // If we didn't find anything, try it again! Don't try to
            // allocate again, just in case we go into an infinite loop.

            int nid = box.Items.Count;
            if (nid==0  && canAllocate)
            {
                idMan.GetAllocation(group, true); // with announcement
                nid = IdHelper.LoadIdCombo(box, ent, false);
            }

            return nid;
        }
         */

        /// <summary>
        /// Load an ID combo box with all the available IDs for a specific entity type.
        /// </summary>
        /// <param name="box">The combo box (not null)</param>
        /// <param name="ent">The entity type that the combo is for (if null, the combo will
        /// be empty)</param>
        /// <param name="handle">The ID handle that should be defined to correspond with the
        /// first available ID (may be null). If there are no available IDs for the specified
        /// entity type, any ID previously reserved will be released.</param>
        /// <param name="canAllocate">Allocate IDs if nothing available? Default=TRUE. For
        /// this to have any effect, an ID handle must be supplied.</param>
        /// <returns>The number of IDs that were loaded into the combo (if any)</returns>
        internal static int LoadIdCombo(ComboBox box, IEntity ent, IdHandle handle, bool canAllocate)
        {
            if (box==null)
                throw new ArgumentNullException();

            // Clear out anything that was in the combo before.
            box.Items.Clear();

            if (ent==null)
                return 0;

            // Get a list of all the available IDs for the specified entity type...

            IdManager idMan = CadastralMapModel.Current.IdManager;
            IdGroup group = idMan.GetGroup(ent);
            if (group==null)
                return 0;

            // Get the ID packets for the group
            IdPacket[] packets = group.IdPackets;
            List<uint> avail = new List<uint>(1000);

            uint resid=0;		// The ID that has been reserved (if any).

            foreach (IdPacket packet in packets)
            {
                avail.Clear();
                if (packet.GetAvailIds(avail) == 0)
                    continue;

                DisplayId[] ids = new DisplayId[avail.Count];
                for (int i=0; i<ids.Length; i++)
                {
                    // If this is the very first available ID, and an ID
                    // handle was supplied, reserve the ID.
                    if (resid==0 && handle!=null)
                    {
                        resid = avail[i];
                        handle.ReserveId(packet, ent, resid);
                    }

                    ids[i] = new DisplayId(packet, avail[i]);
                }

                box.Items.AddRange(ids);
            }

            // If an ID handle has been supplied but we did NOT find any
            // available IDs, free any ID previously reserved. (?)
            if (handle!=null && resid==0)
                handle.FreeId();

            // If we did reserve something, make it selected (assumes it's at the top).
            if (resid!=0)
                box.SelectedItem = box.Items[0];
 
            // If we didn't find anything, try it again! Don't try to
            // allocate again, just in case we go into an infinite loop.

            int nid = box.Items.Count;
            if (nid==0 && canAllocate)
            {
                idMan.GetAllocation(group, true); // with announcement
                nid = IdHelper.LoadIdCombo(box, ent, handle, false);
            }

            return nid;
        }

        /// <summary>
        /// Handles a selection change made to an ID combo. This reserves the ID
        /// by associating it with the supplied ID handle (and discards any previous
        /// reserve).
        /// </summary>
        /// <param name="comboBox">The combo box where the selection has changed.</param>
        /// <param name="handle">The handle for the selected ID</param>
        internal static void OnChangeSelectedId(ComboBox comboBox, IdHandle handle)
        {
            DisplayId id = (DisplayId)comboBox.SelectedItem;
            handle.ReserveId(handle.Entity, id.RawId);
        }
    }
}
