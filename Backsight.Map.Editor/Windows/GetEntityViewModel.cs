using System.Linq;
using Backsight.Database;
using Backsight.Environment;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// View model for <see cref="GetEntityWindow"/>.
/// </summary>
public partial class GetEntityViewModel : DialogViewModel
{
    /// <summary>
    /// The heading for the window.
    /// </summary>
    internal string Heading { get; }
    
    /// <summary>
    /// The entity types to be displayed.
    /// </summary>
    internal IEntity[] EntityTypes { get; }

    /// <summary>
    /// The entity type (if any) that is currently selected.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private IEntity? _selectedEntityType;
    
    /// <summary>
    /// Does the currently selected entity type that can act as a polygon boundary?
    /// </summary>
    [ObservableProperty]
    private bool _isPolygonBoundary;

    /// <summary>
    /// Do the displayed entity types relate to lines?
    /// </summary>
    internal bool LineEntities { get; }
    
    public GetEntityViewModel(IEnvironmentRepository envRepo, ILayer layer, int defaultEntityId, SpatialType t)
    {
        Heading = t switch
        {
            SpatialType.Point => "Entity type for points",
            SpatialType.Line => "Entity type for lines",
            SpatialType.Polygon => "Entity for area labels",
            SpatialType.Text => "Entity type for text",
            _ => "Specify entity type"
        };

        EntityTypes = envRepo.EntityTypes
            .Where(x => x.Id != 0 && x.IsValid(t) && (x.Layer.Id == layer.Id || x.Layer.Id == 0))
            .OrderBy(x => x.Name)
            .ToArray();
        
        LineEntities = t.HasFlag(SpatialType.Line);
        SelectedEntityType = EntityTypes.FirstOrDefault(x => x.Id == defaultEntityId);
    }
    
    protected override bool CanExecuteOk()
    {
        return SelectedEntityType is not null;
    }

    partial void OnSelectedEntityTypeChanged(IEntity? value)
    {
        if (LineEntities && value is not null)
            IsPolygonBoundary = value.IsPolygonBoundaryValid;
    }
}