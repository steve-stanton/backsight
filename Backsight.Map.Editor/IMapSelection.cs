using System.Collections.Generic;
using Backsight.Model;

namespace Backsight.Map.Editor;

/// <summary>
/// Readonly access to properties of the <see cref="Models.Selection"/> class.
/// </summary>
internal interface IMapSelection
{
    IReadOnlyList<IMapObject> Items { get; }
    
    /// <summary>
    /// The geometry for a specific section of a selected line.
    /// </summary>
    LineGeometry? LineSection { get; }
}