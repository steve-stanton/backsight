using Backsight.Model.Operations;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="23-JAN-2012"/>
/// <summary>
/// Any sort of change
/// </summary>
public class Change : IPersistent
{
    /// <summary>
    /// Loads the content of an editing event.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    /// <returns>The created editing event object</returns>
    internal static Change Deserialize(EditDeserializer editDeserializer)
    {
        Change result = editDeserializer.ReadPersistent<Change>(DataField.Edit);

        // Note that calculated geometry is NOT defined at this stage. That happens
        // when the model is asked to index the data.

        // Associate referenced features with the edit
        result.AddReferences();

        // If we're dealing with an update, exchange update items. Do this NOW (rather than at it's natural
        // spot in the editing sequence) so as to avoid repeated calculation loops during loading. When an
        // update is applied during regular editing work, we have to rework the calculation sequence and
        // rollforward all the subsequent edits. So we'd have to do that for every update. By applying changes
        // now, we'll end up doing a single calculation loop.

        if (result is UpdateOperation upo)
            upo.ApplyChanges();

        return result;
    }

    /// <summary>
    /// The sequence number of this change (starts at 1 for a new project, always increasing).
    /// </summary>
    readonly uint m_Sequence;

    /// <summary>
    /// The time (UTC) when the change occurred.
    /// </summary>
    readonly DateTime m_When;

    /// <summary>
    /// Initializes a new instance of the <see cref="Change"/> class when creating
    /// a new item during an editing session.
    /// </summary>
    /// <param name="mapStore">The map store that the change relates to.</param>
    protected Change(IMapStore mapStore)
    {
        m_Sequence = ++mapStore.ItemCount;
        m_When = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Change"/> class.
    /// </summary>
    /// <param name="editSequence">The sequence number of this change (greater than zero).</param>
    /// <exception cref="ArgumentException">If the supplied sequence number is zero.</exception>
    protected Change(uint editSequence)
    {
        if (editSequence == 0)
            throw new ArgumentException();

        m_Sequence = editSequence;
        m_When = DateTime.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Change"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="ed">The mechanism for reading back content.</param>
    protected Change(EditDeserializer ed)
    {
        m_Sequence = ed.ReadUInt32(DataField.Id);
        m_When = ed.ReadDateTime(DataField.When);

        // Handle old files
        //if (ed.IsNextField(DataField.When))
        //    m_When = ed.ReadDateTime(DataField.When);
        //else
        //    m_When = DateTime.Now; // could perhaps grab the session start time
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public virtual void WriteData(EditSerializer editSerializer)
    {
        editSerializer.WriteUInt32(DataField.Id, m_Sequence);
        editSerializer.WriteDateTime(DataField.When, m_When);
    }

    /// <summary>
    /// Change sequence number.
    /// </summary>
    internal uint EditSequence => m_Sequence;

    /// <summary>
    /// Adds references to existing features referenced by an editing event.
    /// <para/>
    /// Applies only to instances of <see cref="Operation"/>.
    /// </summary>
    private protected virtual void AddReferences()
    {
        // Do nothing
    }

    /// <summary>
    /// Does this edit come after the supplied edit?
    /// </summary>
    /// <param name="that">The edit to compare with</param>
    /// <returns>True if this edit was performed after the supplied edit</returns>
    internal bool IsAfter(Change that)
    {
        return this.m_Sequence > that.m_Sequence;
    }

    /// <summary>
    /// The time when the change occurred.
    /// </summary>
    internal DateTime When => m_When;
}