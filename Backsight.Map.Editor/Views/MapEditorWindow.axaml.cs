using System;
using Avalonia.Controls;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.ViewModels;

namespace Backsight.Map.Editor.Views;

public partial class MapEditorWindow : Avalonia.Controls.Window
{
    public MapEditorWindow()
    {
        InitializeComponent();
        
        // Start out with a context that's only suitable in design mode (an effective context will get injected)
        DataContext = new MapEditorViewModel(new DesignMapEditorModel());
        DataContextChanged += OnDataContextChanged;

        Opened += async (_, _) =>
        {
            var startupVm = new StartupViewModel();
            var startup = new StartupWindow { DataContext = startupVm };
            var result = await startup.ShowDialog<string>(this);
            
            Console.WriteLine($"Startup result: {result}");
            if (String.IsNullOrEmpty(result))
                Close();
        };
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is IMapEditorViewModel vm)
        {
            Console.WriteLine("MapEditorViewModel attached to view");
            //MapControl.Map = vm.Map;
        }
    }
}