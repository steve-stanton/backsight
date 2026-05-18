using System.Text;
using Backsight.Environment;
using RepoDb.Attributes;

namespace Backsight.Database;

/// <summary>
/// Database representation for a font definition.
/// </summary>
[Map("Fonts")]
internal partial class FontRow
{
    [Primary] public int FontId { get; set; }
    public string TypeFace { get; set; } = "";
    public float PointSize { get; set; }
    public string IsBold { get; set; } = NO;
    public string IsItalic { get; set; } = NO;
    public string IsUnderline { get; set; } = NO;
}

// Additional properties to satisfy the readonly interface.
internal partial class FontRow : Row, IFont
{
    /// <summary>
    /// A user-perceived title for this font.
    /// </summary>
    /// <returns>The type face (font family name), its points size, and
    /// any modifiers.</returns>
    public override string ToString()
    {
        if (String.IsNullOrEmpty(TypeFace))
            return String.Empty;

        var sb = new StringBuilder(100);

        sb.AppendFormat("{0} - {1}", TypeFace, PointSize);

        if (Bold)
            sb.Append(" Bold");
        if (Italic)
            sb.Append(" Italic");
        if (Underline)
            sb.Append(" Underlined");

        return sb.ToString();
    }

    public int Id => FontId;
    public bool Underline => IsUnderline == YES;
    public bool Italic => IsItalic == YES;
    public bool Bold => IsBold == YES;
}