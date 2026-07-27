using Backsight.Forms;

namespace Backsight.Editor;

static class IntersectionFinderEx
{
    extension(IntersectionFinder finder)
    {
        /// <summary>
        /// Draws intersections on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        internal void Render(ISpatialGraphics display, IDrawStyle style)
        {
            foreach (IntersectionResult r in finder.Intersections)
                r.Render(display, style);
        }
    }
}