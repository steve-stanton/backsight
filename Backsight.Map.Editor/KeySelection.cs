using System;

namespace Backsight.Map.Editor;

/// <summary>
/// Key selections that have significance as far as the map editor code-behind is concerned.
/// </summary>
/// <remarks>
/// Additional key combinations may be specified via map editor views (e,g. see the <c>Window.KeyBindings</c>
/// in <c>MapEditorWindow.axaml</c>).
/// </remarks>
[Flags]
internal enum KeySelection
{
    /// <summary>
    /// Nothing selected
    /// </summary>
    None = 0x00,
    
    /// <summary>
    /// ALT key pressed,
    /// </summary>
    Alt = 0x01,
    
    /// <summary>
    /// Ctrl key pressed.
    /// </summary>
    Ctrl = 0x02,
    
    /// <summary>
    /// Shift key pressed.
    /// </summary>
    Shift = 0x04,
    
    /// <summary>
    /// Esc key pressed.
    /// </summary>
    Escape = 0x08,
    
    /// <summary>
    /// Delete key pressed.
    /// </summary>
    Delete = 0x10,
    
    /// <summary>
    /// Pseudo-key for Find pressed (used when the user wants to select something using its ID).
    /// </summary>
    /// <remarks>Recognized if the user presses CTRL+F (in that case, the Ctrl flag will also be set).</remarks>
    Find = 0x20,
}
