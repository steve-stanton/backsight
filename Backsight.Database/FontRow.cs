using System.ComponentModel.DataAnnotations.Schema;
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
    public string FontFile { get; set; } = "";
}

// Additional properties to satisfy the interfaces.
internal partial class FontRow : Row, IFont, ISetFont
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
    
    [NotMapped]
    public int Id
    {
        get => FontId;
        set => FontId = value;
    }

    public bool Bold
    {
        get => IsBold == YES;
        set => IsBold = AsString(value);
    }

    public bool Italic
    {
        get => IsItalic == YES;
        set => IsItalic = AsString(value);
    }

    public bool Underline
    {
        get => IsUnderline == YES;
        set => IsUnderline = AsString(value);
    }
}