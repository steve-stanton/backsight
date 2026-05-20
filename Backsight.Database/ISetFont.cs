using Backsight.Environment;

namespace Backsight.Database;

/// <summary>
/// Mutable version of <see cref="IFont"/>.
/// </summary>
public interface ISetFont : ISetter
{
    /// <inheritdoc cref="IFont.TypeFace"/>
    string TypeFace { set; }

    /// <inheritdoc cref="IFont.PointSize"/>
    float PointSize { set; }
    
    /// <inheritdoc cref="IFont.Underline"/>
    bool Underline { set; }
    
    /// <inheritdoc cref="IFont.Italic"/>
    bool Italic { set; }
    
    /// <inheritdoc cref="IFont.Bold"/>
    bool Bold { set; }
}