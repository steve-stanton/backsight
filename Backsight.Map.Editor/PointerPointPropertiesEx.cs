using Avalonia.Input;

namespace Backsight.Map.Editor;

/// <summary>
/// Extension methods for <see cref="PointerPressedEventArgs"/>.
/// </summary>
internal static class PointerPointPropertiesEx
{
    extension(PointerPointProperties p)
    {
        internal MouseButton MouseButton => PointerPointPropertiesEx.GetMouseButton(p);
        
        private static MouseButton GetMouseButton(PointerPointProperties properties)
        {
            if (properties.IsLeftButtonPressed)
                return MouseButton.Left;

            if (properties.IsRightButtonPressed)
                return MouseButton.Right;

            if (properties.IsMiddleButtonPressed)
                return MouseButton.Middle;

            return MouseButton.None;
        }
    }
}