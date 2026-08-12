using Avalonia.Input;

namespace Backsight.Map.Editor;

/// <summary>
/// Extension methods for <see cref="KeyEventArgs"/>.
/// </summary>
internal static class KeyEventArgsEx
{
    extension(KeyEventArgs e)
    {
        internal KeySelection KeySelection
        {
            get
            {
                var result = KeySelection.None;

                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                    result |= KeySelection.Ctrl;
                
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    result |= KeySelection.Shift;
                
                if (e.KeyModifiers.HasFlag(KeyModifiers.Alt))
                    result |= KeySelection.Alt;
                
                if (e.Key == Key.F && e.KeyModifiers.HasFlag(KeyModifiers.Control))
                    result |= KeySelection.Find;
                
                if (e.Key == Key.Delete)
                    result |= KeySelection.Delete;
                
                if (e.Key == Key.Escape)
                    result |= KeySelection.Escape;
                
                return result;
            }
        }
    }
}