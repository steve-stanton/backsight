using Mapsui.Layers;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// Extension methods for <see cref="FetchInfo"/>.
/// </summary>
static class FetchInfoEx
{
    extension(FetchInfo fetchInfo)
    {
        /// <summary>
        /// Determines the map scale for a fetch.
        /// </summary>
        /// <param name="dpi">The number of pixels per inch for the display.</param>
        /// <returns>The corresponding map scale denominator.</returns>
        internal double GetMapScale(double dpi = 96.0)
        {
            double pixelsToMeters = 0.0254 / dpi;
            var width = fetchInfo.Section.ScreenWidth * pixelsToMeters;
            return fetchInfo.Extent.Width / width;
        }
        
        /// <summary>
        /// The window for the fetch.
        /// </summary>
        internal IWindow Window => new Window(
            new Position(fetchInfo.Extent.MinX, fetchInfo.Extent.MinY),
            new Position(fetchInfo.Extent.MaxX, fetchInfo.Extent.MaxY));
    }
    
}