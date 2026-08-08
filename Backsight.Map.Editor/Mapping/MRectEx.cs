using Mapsui;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// Extension methods for <see cref="Mapsui.MRect"/>.
/// </summary>
internal static class MRectEx
{
    extension(MRect rect)
    {
        internal Window ToWindow()
        {
            return new Window(rect.Min.X, rect.Min.Y, rect.Max.X, rect.Max.Y);
        }
    }
}