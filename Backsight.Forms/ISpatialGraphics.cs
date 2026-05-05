using System.Drawing;
using System.Windows.Forms;

namespace Backsight.Forms;

public interface ISpatialGraphics : ISpatialDisplay
{
    /// <summary>
    /// The surface on which to draw
    /// </summary>
    Graphics Graphics { get; }

    /// <summary>
    /// Displays a context menu
    /// </summary>
    /// <param name="p">The position where the menu should appear</param>
    /// <param name="menu">The menu to display</param>
    void ShowContextMenu(IPosition p, ContextMenuStrip menu);

    /// <summary>
    /// The panel holding the display. You might need this to do low-level things
    /// like working out screen positions.
    /// </summary>
    Control MapPanel { get; }
}