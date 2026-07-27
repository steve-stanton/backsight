using Backsight.Forms;

namespace Backsight.Editor;

static class ControlPointEx
{
    extension(ControlPoint p)
    {
        /// <summary>
        /// Draws this control point on the specified display.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal void Render(ISpatialGraphics display, IDrawStyle style)
        {
            if (p.IsDefined)
                style.RenderTriangle(display, p);
        }
    }
}