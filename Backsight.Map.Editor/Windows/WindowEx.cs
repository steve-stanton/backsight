using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Backsight.Map.Editor.Windows;

public static class WindowEx
{
    extension(Avalonia.Controls.Control control)
    {
        /// <summary>
        /// Attempts to focus a control based on the value of its <see cref="Control.TabIndex"/> property.
        /// </summary>
        /// <param name="tabIndex">The tab index value to look for.</param>
        /// <returns>True if a control with the specified index was found and a call to
        /// <see cref="Control.Focus"/> returned true. False if a control with the specified
        /// index was not found, or the call to <see cref="Control.Focus"/> returned false.
        /// </returns>
        public bool FocusByTabIndex(int tabIndex)
        {
            var target = FindControlByTabIndex<Control>(control, tabIndex);
            if (target is null)
                return false;
            
            target.Focus();
            return true;
        }

        /// <summary>
        /// Recursively searches the visual tree for a control with the given TabIndex.
        /// </summary>
        private static T? FindControlByTabIndex<T>(Visual root, int tabIndex) where T : Control
        {
            if (root is T ctrl && ctrl.TabIndex == tabIndex)
                return ctrl;

            foreach (var child in root.GetVisualChildren())
            {
                var result = FindControlByTabIndex<T>(child, tabIndex);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}