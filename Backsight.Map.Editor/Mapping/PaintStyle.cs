using SkiaSharp;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// Parameters that may be used to create an instance of <see cref="SKPaint"/>.
/// </summary>
/// <param name="Color"></param>
/// <param name="Style"></param>
/// <param name="StrokeWidth"></param>
internal readonly record struct PaintStyle(
    SKColor Color,
    SKPaintStyle Style = SKPaintStyle.Stroke,
    float StrokeWidth = 1f
)
{
    /// <summary>
    /// Converts the style parameters to an instance of <see cref="SKPaint"/>.
    /// </summary>
    /// <returns>The paint definition used for rendering to a Skia canvas.</returns>
    /// <remarks>
    /// The caller is responsible for disposing of the returned instance at an appropriate time.
    /// <para/>
    /// See also <a href="https://skia.org/docs/user/api/skpaint_overview/"/>. 
    /// </remarks>
    internal SKPaint ToPaint() => new()
    {
        Color = Color,
        Style = Style,
        StrokeWidth = StrokeWidth,
        IsAntialias = true
    };
}