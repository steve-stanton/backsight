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
}