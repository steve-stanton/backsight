using System;
using System.Diagnostics;
using System.Linq;
using Backsight.Environment;
using Backsight.Model;

namespace Backsight.Map.Editor.Models;

/// <summary>
/// Extension methods for <see cref="IMapStore"/>.
/// </summary>
internal static class IMapStoreEx
{
    extension(IMapStore store)
    {
        /// <summary>
        /// Gets the available IDs for a given entity type (obtains an extra allocation if necessary).
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The available IDs for the given entity type.</returns>
        /// <exception cref="ApplicationException">An attempt to obtain an extra allocation
        /// did not yield any IDs.</exception>
        internal DisplayId[] GetPointIds(IEntity entityType)
        {
            var session = store.Model.WorkingSession;
            Debug.Assert(session is not null);
        
            var idMan = store.Model.IdManager;
            var idGroup = idMan.GetGroup(entityType);
            if (idGroup is null)
                return [];

            // Get the available IDs for the group
            uint[] avail = idGroup.GetAvailIds(); 

            // If we didn't find any, obtain an extra allocation
            if (avail.Length == 0)
            {
                idGroup.GetAllocation(session);
                // TODO: Tell the user that we've made an ID allocation
            
                avail = idGroup.GetAvailIds();
                if (avail.Length == 0)
                    throw new ApplicationException("Cannot obtain ID allocation");
            }
        
            return avail.Select(x => new DisplayId(idGroup, x)).ToArray();
        }
    }
}