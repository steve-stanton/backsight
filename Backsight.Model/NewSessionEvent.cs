namespace Backsight.Model;

/// <written by="Steve Stanton" on="04-FEB-2012"/>
/// <summary>
/// Event data for a new editing session
/// </summary>
/// <seealso cref="EndSessionEvent"/>
public class NewSessionEvent : Change
{
    /// <summary>
    /// The login name of the user running the session.
    /// </summary>
    internal string UserName { get; set; }

    /// <summary>
    /// The name of the computer where the project was created.
    /// </summary>
    internal string MachineName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewSessionEvent"/> class.
    /// </summary>
    /// <param name="id">The sequence number of this change (greater than zero).</param>
    /// <param name="userName">The login name of the user running the session.</param>
    /// <param name="machineName">The name of the computer where the project was created.</param>
    public NewSessionEvent(uint id, string userName, string machineName)
        : base(id)
    {
        UserName = userName;
        MachineName = machineName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewSessionEvent"/> class.
    /// </summary>
    /// <param name="ed">The mechanism for reading back content.</param>
    internal NewSessionEvent(EditDeserializer ed)
        : base(ed)
    {
        this.UserName = ed.ReadString(DataField.UserName);
        this.MachineName = ed.ReadString(DataField.MachineName);
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="es">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer es)
    {
        base.WriteData(es);

        es.WriteString(DataField.UserName, this.UserName);
        es.WriteString(DataField.MachineName, this.MachineName);
    }
}