using System;
using System.Linq;
using Backsight.Environment;
using Backsight.Map.Editor.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class NewMapViewModel : DialogViewModel
{
    private readonly IMapEditorModel _model;
    
    [ObservableProperty]
    ILayer[] _layers;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateMapCommand))]
    private ILayer? _selectedLayer;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateMapCommand))]
    string _mapName = "";
    
    public NewMapViewModel(IMapEditorModel model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _layers = model.Environment.Layers.Where(x => x.Id > 0).ToArray();
    }

    [RelayCommand(CanExecute=nameof(CanExecuteCreateMap))]
    private void CreateMap()
    {
        if (SelectedLayer is null)
            throw new InvalidOperationException("No layer selected");

        // TODO: Confirm the map doesn't already exist
        
        try
        {
            _model.CreateMap(MapName, SelectedLayer);
            Ok();
        }
        catch (Exception e)
        {
            // TODO: MessageBox
            Console.WriteLine(e);
        }
    }
    
    private bool CanExecuteCreateMap()
    {
        return SelectedLayer is not null && !String.IsNullOrWhiteSpace(MapName);
    }
}