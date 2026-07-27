namespace Backsight.Model;

/// <summary>
/// Basic information about a map.
/// </summary>
/// <param name="Name">The user-perceived map name.</param>
public readonly record struct MapInfo(string Name);