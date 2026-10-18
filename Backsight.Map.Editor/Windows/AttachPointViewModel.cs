using System;
using System.Diagnostics;
using System.Linq;
using Backsight.Environment;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

public partial class AttachPointViewModel : DialogViewModel
{
    /// <summary>
    /// Key of the global setting indicating whether the command should auto-repeat. The value of
    /// the entry should be 1 or 0 (for true or false).
    /// </summary>
    const string REPEAT_KEY = "AttachPointRepeat";

    /// <summary>
    /// The format of the global setting key for default point type (the substitution refers
    /// to the ID of the relevant map layer, while the value is the ID of the corresponding
    /// entity type).
    /// </summary>
    const string DEFAULT_KEY_FORMAT = "AttachPointType.{0}";
    
    private string DefaultTypeKey { get; }
    
    internal IEntity[] PointTypes { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private IEntity? _selectedPointType;

    [ObservableProperty]
    private bool _isMakeTypeDefault = true;

    [ObservableProperty]
    private bool _isAutoRepeat = true;
    
    internal AttachPointViewModel(MapEditorViewModel vm)
    {
        var settings = vm.Store?.Settings ?? throw new InvalidOperationException("No map store");
        var layer = vm.Environment.FindRequired<ILayer>(settings.ActiveLayer);

        PointTypes = vm.Environment.EntityTypes
            .Where(x => x.Id != 0 && x.IsPointValid && (x.Layer.Id == layer.Id || x.Layer.Id == 0))
            .OrderBy(x => x.Name)
            .ToArray();
        
        // Get the default point type (preferably from a global setting)
        DefaultTypeKey = String.Format(DEFAULT_KEY_FORMAT, settings.ActiveLayer);
        int defaultTypeId = GlobalUserSetting.ReadInt(DefaultTypeKey, 0);
        if (defaultTypeId == 0)
            defaultTypeId = settings.GetDefaults(layer).PointType;

        SelectedPointType = PointTypes.FirstOrDefault(x => x.Id == defaultTypeId);
        
        // Check auto-repeat option (default is to repeat)
        int repeat = GlobalUserSetting.ReadInt(REPEAT_KEY, 1);
        IsAutoRepeat = repeat != 0;
    }
    
    protected override bool CanExecuteOk()
    {
        return SelectedPointType is not null;
    }
    
    protected override void Ok()
    {
        Debug.Assert(SelectedPointType is not null);

        // Ensure the currently selected point type is saved when requested
        if (IsMakeTypeDefault)
            GlobalUserSetting.WriteInt(DefaultTypeKey, SelectedPointType.Id);

        // Ensure the auto-repeat option is saved too
        GlobalUserSetting.WriteInt(REPEAT_KEY, IsAutoRepeat ? 1 : 0);

        base.Ok();
    }
}