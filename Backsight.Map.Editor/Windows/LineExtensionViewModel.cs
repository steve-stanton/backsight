using System;
using System.Diagnostics;
using System.Linq;
using Backsight.Environment;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

// The ViewModel should only be responsible for what's needed by LineExtensionWindow.
// LineExtensionTool should know about the MapEditorViewModel so that it can call FinishCommand or AbortCommand
// when required. And it will hold a reference to LineExtensionWindow/ViewModel.
// The question is how to notify the tool when ViewModel.HandleCloseRequested() is called - should the
// tool be passed into the ViewModel?
// 
// In the old system:
// - LineExtensionUI == LineExtensionTool
// - The UI created a LineExtensionControl (wrapped in a container form) == LineExtensionWindow. And it
//   passed in a reference to itself => this says that the "tool" should be known to the ViewModel.
//   
// When the user closed the LineExtensionControl by clicking the OK button, it called the UI.DialFinish
// method. The UI class grabbed the entered data from the dialog and created the LineExtensionOperation.

// Would it be better to make it so that the ViewModel knows nothing about any CommandTool?...
// The CommandTool could register for the VM.CloseRequested event
public partial class LineExtensionViewModel : DialogViewModel
{
    private readonly LineExtensionTool _tool;
    
    /// <summary>
    /// The line that is being extended.
    /// </summary>
    private readonly LineFeature _extendLine;

    /// <summary>
    /// True if extending from the end of the line. False from the start.
    /// </summary>
    [ObservableProperty]
    private bool _isExtendFromEnd = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private decimal? _length;

    /// <summary>
    /// True if a line should be added too.
    /// </summary>
    [ObservableProperty]
    private bool _wantLine = true;

    [ObservableProperty]
    private IEntity _selectedPointType;
    
    [ObservableProperty]
    private DisplayId[] _pointIds;
    
    [ObservableProperty]
    private DisplayId? _selectedPointId;
    
    /// <summary>
    /// The units to display for entry of the extension length.
    /// </summary>
    internal string Units { get; }
    
    /// <summary>
    /// The entity types that can be used for extension points.
    /// </summary>
    internal IEntity[] PointTypes { get; }
    
    /// <summary>
    /// True if the user can select an ID for the extension point.
    /// </summary>
    internal bool AllowIdSelection { get; }

    internal LineExtensionViewModel(LineExtensionTool tool, LineFeature extendLine)
    {
        _tool = tool;
        _extendLine = extendLine;

        // Get the units hint for the extension length
        var entryUnit = tool.Store.Settings.EntryUnit;
        Units = DistanceUnit.GetUnit(entryUnit).Abbreviation + " ";

        // Fill the combo with the entity types for the extension point
        PointTypes = tool.ViewModel.Environment.EntityTypes
            .Where(x => x.IsPointValid && x.Id != 0)
            .OrderBy(x => x.Name)
            .ToArray();

        _selectedPointType = tool.Store.DefaultPointType;

        // Load the ID combo and select the first available ID
        _pointIds = GetPointIds(tool.Store, _selectedPointType);
        if (_pointIds.Length > 0)
            _selectedPointId = _pointIds[0];
        
        // If we are auto-numbering, disable the combo.
        AllowIdSelection = !tool.Store.Settings.AutoNumber;
    }

    private static DisplayId[] GetPointIds(IMapStore store, IEntity entityType)
    {
        var session = store.Model.WorkingSession;
        Debug.Assert(session is not null);
        
        var idMan = store.Model.IdManager;
        var idGroup = idMan.GetGroup(entityType);
        if (idGroup is null)
            return [];

        // Get the available IDs for the group
        uint[] avail = idGroup.GetAvailIds(); 

        // If we didn't find any, obtain an extra allocation
        if (avail.Length == 0)
        {
            idGroup.GetAllocation(session);
            // TODO: Tell the user that we've made an ID allocation
            
            avail = idGroup.GetAvailIds();
            if (avail.Length == 0)
                throw new ApplicationException("Cannot obtain ID allocation");
        }
        
        return avail.Select(x => new DisplayId(idGroup, x)).ToArray();
    }
    
    internal LineFeature Line => _extendLine;
    
    partial void OnLengthChanged(decimal? value)
    {
        _tool.RefreshMapDisplay();
    }
    
    [RelayCommand]
    private void OtherEnd()
    {
        IsExtendFromEnd = !IsExtendFromEnd;
    }

    partial void OnIsExtendFromEndChanged(bool value)
    {
        _tool.RefreshMapDisplay();
    }

    partial void OnWantLineChanged(bool value)
    {
        _tool.RefreshMapDisplay();
    }

    partial void OnSelectedPointTypeChanged(IEntity? value)
    {
        if (value is null)
        {
            SelectedPointId = null;
            PointIds = [];
        }
        else if (value.IdGroup.Id != _selectedPointId?.Group.Id)
        {
            PointIds = GetPointIds(_tool.Store, value);
            //Console.WriteLine($"Found {PointIds.Length} IDs for entity type {value.Name}");

            if (PointIds.Length == 0)
                SelectedPointId = null;
            else
                SelectedPointId = _pointIds[0];
        }
    }

    internal override bool HandleCloseRequested(DialogResult result)
    {
        Console.WriteLine("Line extension requesting close with result: " + result);

        if (result == DialogResult.OK)
        {
            // return if fails validation
            Debug.Assert(CanExecuteOk());
            
            _tool.FinishDataEntry(this);
        }
        else
        {
            _tool.CancelDataEntry(this);
        }

        return true;
    }
    
    protected override bool CanExecuteOk()
    {
        return Length > 0;
    }

    /// <summary>
    /// Reserves an ID for the extension point.
    /// </summary>
    /// <returns>The handle on the reserved ID (null if the point has an entity type
    /// that does not require any IDs).</returns>
    /*
    internal IdHandle? ReservePointIdH()
    {
        if (SelectedPointId is null)
            return null;

        var result = new IdHandle(_tool.WorkingSession);
        result.Define

    }
    */
}