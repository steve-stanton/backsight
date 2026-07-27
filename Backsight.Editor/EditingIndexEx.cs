namespace Backsight.Editor;

static class EditingIndexEx
{
    extension(EditingIndex index)
    {
        /// <summary>
        /// Draws intersection points
        /// </summary>
        /// <param name="mapDisplay">The display to draw to</param>
        internal void DrawIntersections(IMapDisplay mapDisplay)
        {
            index.ProcessIntersections(mapDisplay.Extent, (ISpatialObject o) =>
            {
                o.Draw(mapDisplay);
                return true;
            });
        }
    }
}