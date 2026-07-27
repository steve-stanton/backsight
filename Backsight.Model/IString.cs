namespace Backsight.Model;

public interface IString
{
    /// <summary>
    /// The text string.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Clockwise rotation from horizontal
    /// </summary>
    IAngle Rotation { get; }

    /// <summary>
    /// A reference position for the string (typically the top-left corner).
    /// </summary>
    IPointGeometry Position { get; }

    /// <summary>
    /// Any special layout information for the string (used for specifying special
    /// text alignment options). A null value means default formatting should be used.
    /// </summary>
    //StringFormat Format { get; }

    /// <summary>
    /// A closed outline that surrounds the string (could be null if the implementing
    /// class doesn't care).
    /// </summary>
    IPosition[] Outline { get; }

    /// <summary>
    /// Creates the font used to present the string.
    /// </summary>
    /// <param name="display">The display on which the string will be displayed</param>
    /// <returns>The corresponding font (may be null if the font is too small to be drawn)</returns>
    //Font CreateFont(ISpatialDisplay display);
}
