using System.Collections.Generic;
using Backsight.Model;

namespace Backsight.Map.Editor;

/// <summary>
/// Readonly access to properties of the <see cref="Models.Selection"/> class.
/// </summary>
public interface IMapSelection
{
    IReadOnlyList<IMapObject> Items { get; }
    
    /// <summary>
    /// The geometry for a specific section of a selected line.
    /// </summary>
    /// <remarks>
    /// This will be defined only if the selection refers to a single polygon boundary
    /// line that has been divided into a series of sections.
    /// </remarks>
    LineGeometry? LineSection { get; }
}