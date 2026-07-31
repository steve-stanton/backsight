using Mapsui;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// Extension methods for <see cref="IWindow"/>.
/// </summary>
static class IWindowEx
{
    extension(IWindow window)
    {
        internal MRect ToMRect()
        {
            return new MRect(window.Min.X, window.Min.Y, window.Max.X, window.Max.Y);
        }
    }
}