using System.Drawing;
using System.Drawing.Drawing2D;
using Backsight.Forms;

namespace Backsight.Editor;

static class FeatureEx
{
    extension(Feature f)
    {
        /// <summary>
        /// Draws this feature on a specific map display. Not intended for bulk draws, since
        /// it creates a drawing style object on each call.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="col">The colour to use for the draw</param>
        internal void Draw(ISpatialGraphics display, Color col)
        {
            var style = EditingController.Current.DrawStyle;
            style.LineColor = style.FillColor = col;
            f.Render(display, style);
        }

        internal void Draw(ISpatialGraphics display, HatchStyle hs, Color foreColor)
        {
            var style = EditingController.Current.DrawStyle;
            style.Fill = new Fill(hs, foreColor, display.MapPanel.BackColor);
            f.Render(display, style);
        }
        
    }
}