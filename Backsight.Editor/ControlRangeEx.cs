using Backsight.Forms;

namespace Backsight.Editor;

static class ControlRangeEx
{
    extension(ControlRange r)
    {
        /// <summary>
        /// Draws this control range.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal void Render(ISpatialGraphics display, IDrawStyle style)
        {
            foreach (ControlPoint cp in r.GetDefinedPoints())
                cp.Render(display, style);
        }
    }
}