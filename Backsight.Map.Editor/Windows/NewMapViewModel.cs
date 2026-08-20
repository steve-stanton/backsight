using System;
using System.Linq;
using Backsight.Environment;
using Backsight.Map.Editor.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class NewMapViewModel : ViewModelBase
{
    private readonly IMapEditorModel _model;
    
    [ObservableProperty]
    ILayer[] _layers;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkButtonCommand))]
    private ILayer? _selectedLayer;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkButtonCommand))]
    string _mapName = "";
    
    public event Action? RequestClose;
    
    public NewMapViewModel(IMapEditorModel model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _layers = model.Environment.Layers.Where(x => x.Id > 0).ToArray();
    }

    [RelayCommand(CanExecute=nameof(CanExecuteOkButton))]
    private void OkButton()
    {
        if (SelectedLayer is null)
            throw new InvalidOperationException("No layer selected");
        
        try
        {
            _model.CreateMap(MapName, SelectedLayer);
            RequestClose?.Invoke();
        }
        catch (Exception e)
        {
            // TODO: MessageBox
            Console.WriteLine(e);
        }
    }
    
    bool CanExecuteOkButton()
    {
        return SelectedLayer is not null && !String.IsNullOrWhiteSpace(MapName);
    }

    [RelayCommand]
    void CancelButton()
    {
        MapName = String.Empty;
        RequestClose?.Invoke();
    }
}