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
using System.Collections.Generic;
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor
{
    class IdHelper
    {
        /// <summary>
        /// Load an ID combo box with all the available IDs for a specific entity type.
        /// </summary>
        /// <param name="box">The combo box (not null)</param>
        /// <param name="ent">The entity type that the combo is for (if null, the combo will
        /// be empty)</param>
        /// <param name="handle">The ID handle that should be defined to correspond with the
        /// first available ID (may be null). If there are no available IDs for the specified
        /// entity type, any ID previously reserved will be released.</param>
        /// <returns>The number of IDs that were loaded into the combo (if any)</returns>
        internal static int LoadIdCombo(ComboBox box, IEntity ent, IdHandle handle)
        {
            if (box==null)
                throw new ArgumentNullException();

            // Clear out anything that was in the combo before.
            box.Items.Clear();

            if (ent==null)
                return 0;

            // Get a list of all the available IDs for the specified entity type...

            IdManager idMan = CadastralMapModel.Current.IdManager;
            if (idMan == null)
                return 0;

            IdGroup group = idMan.GetGroup(ent);
            if (group==null)
                return 0;

            // Get the available IDs for the group
            uint[] avail = group.GetAvailIds();

            // If we didn't find any, obtain an extra allocation
            if (avail.Length == 0)
            {
                IdPacket newPacket = group.GetAllocation(true); // with announcement
                avail = group.GetAvailIds();
                if (avail.Length == 0)
                    throw new ApplicationException("Cannot obtain ID allocation");
            }

            // Load the combo
            DisplayId[] ids = new DisplayId[avail.Length];
            for (int i = 0; i < ids.Length; i++)
                ids[i] = new DisplayId(group, avail[i]);

            box.Items.AddRange(ids);

            // Reserve the first available ID if a handle was supplied (and select it)
            if (handle != null)
            {
                IdPacket p = group.FindPacket(avail[0]);
                handle.ReserveId(p, ent, avail[0]);
                box.SelectedItem = ids[0];
            } 

            return avail.Length;
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
