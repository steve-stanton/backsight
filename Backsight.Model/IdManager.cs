using Backsight.Database;
using Backsight.Environment;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="16-DEC-1998" />
/// <summary>
/// Management of ID assignment. One of these objects forms part of the
/// <c>EditingController</c> class. It is responsible for maintaining a
/// connection to the external database that holds ID info. It acts as a
///	server for dishing out IDs.
/// </summary>
public class IdManager
{
    /// <summary>
    /// The ID groups in the database (excluding the empty group).
    /// </summary>
    readonly IdGroup[] m_IdGroups;

    /// <summary>
    /// Index of the ID groups for each entity type. The key is the ID of the entity type,
    /// the values are elements in the <c>m_IdGroups</c> array.
    /// </summary>
    /// <remarks>
    /// This excludes entity types that are associated with the empty group. 
    /// </remarks>
    readonly Dictionary<int, IdGroup> m_EntityGroups;

    /// <summary>
    /// Creates a new instance of the <c>IdManager</c> class.
    /// </summary>
    /// <param name="repo">The database holding details for the operating environment.</param>
    public IdManager(IEnvironmentRepository repo)
    {
        m_IdGroups = repo.IdGroups.Where(x => x.Id != 0).Select(x => new IdGroup(x)).ToArray();
        m_EntityGroups = GetEntityGroups(repo, m_IdGroups);
    }

    /// <summary>
    /// The ID groups in the database.
    /// </summary>
    internal IdGroup[] IdGroups => m_IdGroups;

    /// <summary>
    /// Attempts to find the ID group with a specific ID
    /// </summary>
    /// <param name="groupId">The ID of the group to look for</param>
    /// <returns>The corresponding ID group.</returns>
    internal IdGroup FindGroupById(int groupId)
    {
        return m_IdGroups.First(g => g.Id == groupId);
    }

    /// <summary>
    /// Attempts to find the ID group that encloses a specific raw ID
    /// </summary>
    /// <param name="rawId">The raw ID of interest</param>
    /// <returns>The corresponding ID group</returns>
    internal IdGroup FindGroupByRawId(uint rawId)
    {
        return m_IdGroups.First(g => g.LowestId <= rawId && rawId <= g.HighestId);
    }

    /// <summary>
    /// Reserves the next available ID for a given entity type.
    /// </summary>
    /// <param name="idh">The ID handle to fill in.</param>
    /// <param name="ent">The entity type to search for.</param>
    /// <param name="id">The specific ID to reserve (specify 0 to get the next available ID).</param>
    /// <returns>True if the ID handle was filled in successfully.</returns>
    internal bool ReserveId(IdHandle idh, IEntity ent, uint id)
    {
        // Ensure the ID handle is free.
        idh.FreeReservedId();

        // Get the ID group to make the reservation
        IdGroup? g = GetGroup(ent);
        if (g is null)
            return false;
        else
            return g.ReserveId(idh, id);
    }

    /// <summary>
    /// Generates an index that cross-references each entity type with its corresponding
    /// ID group.
    /// </summary>
    /// <param name="groups">The ID groups to index</param>
    /// <returns>Index of the ID groups for each entity type. The key is the ID of the entity type,
    /// the values are elements in the <paramref name="groups"/> array.</returns>
    private static Dictionary<int, IdGroup> GetEntityGroups(IEnvironmentRepository repo, IdGroup[] groups)
    {
        var result = new Dictionary<int, IdGroup>();

        foreach (IEntity e in repo.EntityTypes)
        {
            // Every entity type must be associated with an ID group, but it may be the
            // "empty" group (with an ID of 0). We only care for entity types associated
            // with the non-empty groups.
            
            IIdGroup idg = e.IdGroup;
            if (idg.Id > 0)
            {
                int gid = idg.Id;
                IdGroup entGroup = groups.First(g => g.Id == gid);
                result.Add(e.Id, entGroup);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets a new allocation for every ID group. This function is used only when the
    /// user is explicitly allocating ID ranges (through the dialog that lists the allocations).
    /// </summary>
    /// <param name="session">The current working session.</param>  
    /// <returns>Information about the allocations made.</returns>
    internal IdPacket[] GetAllocation(Session session)
    {
        var allocs = new List<IdPacket>();

        foreach (IdGroup g in m_IdGroups)
        {
            IdPacket? p = g.GetAllocation(session);
            if (p is not null)
                allocs.Add(p);
        }

        return allocs.ToArray();
    }

    /// <summary>
    /// Returns the ID group that corresponds to a specific entity type.
    /// </summary>
    /// <param name="ent">The entity type to find.</param>
    /// <returns>The matching group (null if no such group)</returns>
    internal IdGroup? GetGroup(IEntity ent)
    {
        if (ent == null || ent.Id == 0)
            return null;

        if (m_EntityGroups.TryGetValue(ent.Id, out var result))
            return result;
        else
            return null;
    }

    /// <summary>
    /// Exhaustive search for the ID packet that refers to a specific ID. This method
    /// should only be called in situations where something has gone astray.
    /// </summary>
    /// <param name="fid">The ID to search for</param>
    /// <returns>The packet that contains the specified object (null if not found)</returns>
    internal IdPacket FindPacket(NativeId nid)
    {
        foreach (IdGroup g in m_IdGroups)
        {
            IdPacket p = g.FindPacket(nid);
            if (p!=null)
                return p;
        }

        return null;
    }

    /// <summary>
    /// Discards any IDs that may have been reserved (but which are no longer needed). This
    /// should be called in situations where a user cancels from a data entry dialog.
    /// </summary>
    internal void FreeAllReservedIds()
    {
        foreach (IdGroup group in m_IdGroups)
            group.FreeAllReservedIds();
    }
}