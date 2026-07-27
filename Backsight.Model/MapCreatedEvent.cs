using System;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="23-JAN-2012"/>
/// <summary>
/// Event data for a new map.
/// </summary>
class MapCreatedEvent : Change
{
    /// <summary>
    /// A unique ID for the map.
    /// </summary>
    internal Guid MapId { get; set; }

    /// <summary>
    /// The ID of the layer the map is associated with.
    /// </summary>
    internal int LayerId { get; set; }

    /// <summary>
    /// The name of the default coordinate system.
    /// </summary>
    internal string DefaultSystem { get; set; }

    /// <summary>
    /// The login name of the user who created the map.
    /// </summary>
    internal string UserName { get; set; }

    /// <summary>
    /// The name of the computer where the map was created.
    /// </summary>
    internal string MachineName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapCreatedEvent"/> class
    /// with default values for all properties.
    /// </summary>
    internal MapCreatedEvent()
        : base(1)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapCreatedEvent"/> class.
    /// </summary>
    /// <param name="ed">The mechanism for reading back content.</param>
    internal MapCreatedEvent(EditDeserializer ed)
        : base(ed)
    {
        MapId = new Guid(ed.ReadString(DataField.ProjectId));
        var projectName = ed.ReadString(DataField.ProjectName);
        LayerId = ed.ReadInt32(DataField.LayerId);
        DefaultSystem = ed.ReadString(DataField.CoordinateSystem);
        UserName = ed.ReadString(DataField.UserName);
        MachineName = ed.ReadString(DataField.MachineName);
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="es">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer es)
    {
        base.WriteData(es);

        es.WriteString(DataField.ProjectId, this.MapId.ToString().ToUpper());
        es.WriteString(DataField.ProjectName, String.Empty);
        es.WriteInt32(DataField.LayerId, this.LayerId);
        es.WriteString(DataField.CoordinateSystem, this.DefaultSystem);
        es.WriteString(DataField.UserName, this.UserName);
        es.WriteString(DataField.MachineName, this.MachineName);
    }
}