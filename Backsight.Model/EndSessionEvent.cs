namespace Backsight.Model;

/// <written by="Steve Stanton" on="10-FEB-2012"/>
/// <summary>
/// Event data for normal completion of an editing session
/// </summary>
/// <seealso cref="NewSessionEvent"/>
public class EndSessionEvent : Change
{
    // No data (all I really want is the timestamp stored in the base class).

    /// <summary>
    /// Initializes a new instance of the <see cref="EndSessionEvent"/> class.
    /// </summary>
    internal EndSessionEvent(uint id)
        : base(id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndSessionEvent"/> class.
    /// </summary>
    /// <param name="ed">The mechanism for reading back content.</param>
    internal EndSessionEvent(EditDeserializer ed)
        : base(ed)
    {
    }
}