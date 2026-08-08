using Backsight.Geometry;
using Backsight.Model;
using Mapsui;
using Mapsui.Extensions;
using SkiaSharp;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// Extension methods for <see cref="Viewport"/>.
/// </summary>
static class ViewportEx
{
    extension(Viewport viewport)
    {
        /// <summary>
        /// Converts a ground position to a screen position.
        /// </summary>
        /// <param name="p">The ground position to be converted.</param>
        /// <returns>The corresponding screen position in the viewport.</returns>
        internal SKPoint ToScreenPoint(IPosition p)
        {
            var (sx, sy) = viewport.WorldToScreenXY(p.X, p.Y);
            return new SKPoint((float)sx, (float)sy);
        }

        /// <summary>
        /// Gets the screen position of a rectangle that encloses a circle. 
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <returns>The screen position of a rectangle that encloses the circle.</returns>
        internal SKRect ToScreenRect(ICircleGeometry circle)
        {
            var cx = CircleGeometry.GetExtent(circle);
            var (left, bottom) = viewport.WorldToScreenXY(cx.Min.X, cx.Min.Y);
            var (right, top) = viewport.WorldToScreenXY(cx.Max.X, cx.Max.Y);
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }        
    }
}