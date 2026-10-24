using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// View model for <see cref="GetCbntrolWindow"/>.
/// </summary>
internal partial class GetControlViewModel : DialogViewModel
{
    private readonly MapEditorViewModel _vm;

    /// <summary>
    /// All known control points.
    /// </summary>
    private readonly ControlPoint[] _controlPoints;

    /// <summary>
    /// The names of the areas associated with all the control points.
    /// </summary>
    internal string[] Areas { get; }
    
    /// <summary>
    /// The name of the currently selected area.
    /// </summary>
    [ObservableProperty]
    private string? _selectedArea;
    
    /// <summary>
    /// The subset of control points for the currently selected area, possibly filtered by ID.
    /// This will be empty if no area is selected.
    /// </summary>
    [ObservableProperty]
    private ControlPoint[] _areaPoints = [];

    /// <summary>
    /// Any filter on the displayed IDs of the control points in the current area (blank
    /// if all control points in the currently selected area should be displayed).
    /// </summary>
    [ObservableProperty]
    private string _filter = "";
    
    /// <summary>
    /// The control points in <see cref="AreaPoints"/> that have been selected.
    /// This can be a subset of <see cref="SelectedPoints"/>.
    /// </summary>
    public ObservableCollection<ControlPoint> SelectedAreaPoints { get; } = [];
    
    /// <summary>
    /// Is an ID filter currently being applied?
    /// </summary>
    private bool _isFiltering;

    /// <summary>
    /// The control points that have been selected.
    /// </summary>
    private HashSet<ControlPoint> SelectedPoints { get; } = [];
    
    /// <summary>
    /// The text to display for the number of selected control points.
    /// </summary>
    [ObservableProperty]
    private string? _selectionMessage = "0 selected";
    
    internal GetControlViewModel(MapEditorViewModel vm)
    {
        _vm = vm;

        // Load control points from application content
        // TODO: Should allow through config
        var folder = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? throw new ApplicationException());
        var controlPath = Path.Combine(folder, "ManitobaControl.json");
        var controlText = File.ReadAllText(controlPath);
        _controlPoints = JsonSerializer.Deserialize<ControlPoint[]>(controlText) ?? Array.Empty<ControlPoint>();
        
        Areas = _controlPoints.Select(cp => cp.AreaName).OrderBy(x => x).Distinct().ToArray();

        SelectedAreaPoints.CollectionChanged += (sender, args) =>
        {
            if (_isFiltering)
                return;

            // Ensure that selected control points are included in the overall result
            if (args.NewItems is not null)
            {
                foreach (ControlPoint cp in args.NewItems)
                    SelectedPoints.Add(cp);
            }

            // Remove the control points that have been deselected
            if (args.OldItems is not null)
            {
                foreach (ControlPoint cp in args.OldItems)
                    SelectedPoints.Remove(cp);
            }
            
            SelectionMessage = $"{SelectedPoints.Count} selected";
            OkCommand.NotifyCanExecuteChanged();
        };
    }

    partial void OnSelectedAreaChanged(string? value)
    {
        ApplyFilters();
    }
    
    partial void OnFilterChanged(string? value)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        _isFiltering = true;
        try
        {
            var points = _controlPoints.Where(cp => cp.AreaName == SelectedArea);

            if (!String.IsNullOrWhiteSpace(Filter))
                points = points.Where(cp => cp.ToString().Contains(Filter, StringComparison.OrdinalIgnoreCase));

            AreaPoints = points.OrderBy(x => x.Id).ToArray();
            
            // Restore visible selections from the master set
            SelectedAreaPoints.Clear();
            foreach (var point in AreaPoints.Where(x => SelectedPoints.Contains(x)))
                SelectedAreaPoints.Add(point);
        }
        finally
        {
            _isFiltering = false;
        }
    }

    protected override bool CanExecuteOk()
    {
        return SelectedPoints.Count > 0;
    }
}