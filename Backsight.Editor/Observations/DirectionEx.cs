using Backsight.Forms;

namespace Backsight.Editor.Observations;

static class DirectionEx
{
    extension(Direction d)
    {
        /// <summary>
        /// Renders this direction as a dotted magenta line.
        /// </summary>
        /// <param name="display"></param>
        internal void Render(ISpatialGraphics display)
        {
            // Figure out where the direction line is
            IPosition from = d.StartPosition;
            double len = d.GetMaxDiagonal(display);
            IPosition to = Geom.Polar(from, d.Bearing.Radians, len);

            new DottedStyle().Render(display, from, to);
        }

        /// <summary>
        /// The diagonal length of a line that spans the display when it is
        /// drawn at the overview scale.
        /// </summary>
        /// <param name="display"></param>
        /// <returns></returns>
        private double GetMaxDiagonal(ISpatialDisplay display)
        {
            IWindow x = display.MaxExtent;
            return BasicGeom.Distance(x.Min, x.Max);
        }

        /// <summary>
        /// Gets the maximum length of the direction line (for use when calculating intersections).
        /// </summary>
        /// <param name="defaultLength">The default length to use if the map extent is undefined.</param>
        /// <returns>The maximum length to use for the direction line (in meters on the ground).</returns>
        internal double GetMaxLength(double defaultLength = 100000.0)
        {
            // Define the length of the direction line as the length
            // of a diagonal that crosses the map's extent.
            IWindow mapWin = EditingController.Current.MapModel.Extent;

            // If the window is currently undefined (e.g. during deserialization), just use a really big distance.
            // TODO: This is a hack, might need to persist this when calculating an intersection during deserialization
            return mapWin.IsEmpty ? defaultLength : BasicGeom.Distance(mapWin.Min, mapWin.Max);
        }
    }
}