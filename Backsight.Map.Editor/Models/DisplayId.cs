using System;
using Backsight.Model;

namespace Backsight.Map.Editor.Models;

/// <written by="Steve Stanton" on="05-APR-2007" />
/// <summary>
/// A numeric ID that is potentially decorated according to some display format (as
/// defined through an associated <see cref="IdGroup"/>
/// </summary>
public class DisplayId
{
    /// <summary>
    /// The raw ID (undecorated with stuff like check digits)
    /// </summary>
    uint m_Id;

    /// <summary>
    /// The group the ID is part of
    /// </summary>
    IdGroup m_Group;

    /// <summary>
    /// Creates a new <c>DisplayId</c>
    /// </summary>
    /// <param name="group">The ID group containing the raw ID</param>
    /// <param name="rawId">The raw ID (undecorated with stuff like check digits)</param>
    internal DisplayId(IdGroup group, uint rawId)
    {
        m_Group = group ?? throw new ArgumentNullException();
        m_Id = rawId;
    }

    /// <summary>
    /// The formatted version of the raw ID.
    /// </summary>
    /// <returns>The result of a call to <c>IdGroup.FormatId(RawId)</c></returns>
    public override string ToString()
    {
        return m_Group.FormatId(m_Id);
    }

    /// <summary>
    /// The raw ID (undecorated with stuff like check digits)
    /// </summary>
    internal uint RawId => m_Id;
    
    /// <summary>
    /// The group the ID is part of
    /// </summary>
    internal IdGroup Group => m_Group;

    /// <summary>
    /// Creates a new feature ID that doesn't reference anything (and does not add it to the map model).
    /// </summary>
    /// <returns>The created feature ID.</returns>
    internal FeatureId CreateId()
    {
        IdPacket? p = m_Group.FindPacket(m_Id);
        if (p is null)
            throw new ApplicationException("Cannot locate packet for ID: " + m_Id);
        
        p.ReserveId(m_Id);
        return p.CreateId(m_Id);
    }
    
    internal IdPacket Packet => m_Group.FindPacket(m_Id) ?? throw new ApplicationException("Cannot locate packet for ID: " + m_Id);
}