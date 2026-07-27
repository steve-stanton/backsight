namespace Backsight.Model;

/// <summary>
/// Defaults when working with a specific map layer.
/// </summary>
/// <param name="LayerId">The ID of the <see cref="Backsight.Environment.ILayer"/> that these defaults relate to.</param>
/// <param name="PointType">The ID of the entity type that should be used for new point features.</param>
/// <param name="LineType">The ID of the entity type that should be used for new line features.</param>
/// <param name="PolygonType">The ID of the entity type that should be used for new polygon labels.</param>
/// <param name="TextType">The ID of the entity type that should be used for new text features.</param>
/// <remarks>
/// When a map is opened for the first time, the default entity types will be assigned using the
/// values defined for the active map layer. The user can subsequently change these defaults if
/// they wish.
/// </remarks>
/// <seealso cref="Backsight.Environment.IEntity"/>
public record LayerDefaults(
    int LayerId,
    int PointType,
    int LineType,
    int PolygonType,
    int TextType);