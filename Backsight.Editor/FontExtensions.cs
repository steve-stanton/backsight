using System.Drawing;
using Backsight.Environment;

namespace Backsight.Editor;

static class FontExtensions
{
    extension(IFont f)
    {
        internal FontStyle GetModifiers()
        {
            var result = FontStyle.Regular;
            
            if (f.Bold)
                result |= FontStyle.Bold;

            if (f.Italic)
                result |= FontStyle.Italic;

            if (f.Underline)
                result |= FontStyle.Underline;

            return result;
        }
    }

    extension(IEditFont f)
    {
        internal void SetModifiers(FontStyle style)
        {
            f.Bold = (style & FontStyle.Bold) != 0;
            f.Italic = (style & FontStyle.Italic) != 0;
            f.Underline = (style & FontStyle.Underline) != 0;
        }
    }
}