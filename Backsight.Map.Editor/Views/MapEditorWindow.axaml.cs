using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Backsight.Map.Editor.ViewModels;
using Mapsui.Extensions;

namespace Backsight.Map.Editor.Views;

// This should be responsible for:
// 1. holding the MapControl
// 2. renderer/drawing layer
// 3. canvas-specific drawing
public partial class MapEditorWindow : Avalonia.Controls.Window
{
    /// <summary>
    /// Dynamically created menu items for a list of recent maps.
    /// </summary>
    private readonly List<MenuItem> _recentMapMenuItems = [];
    private MapEditorViewModel? _recentMapsViewModel;

    public MapEditorWindow()
    {
        InitializeComponent();

        // Start out with a context that's only suitable in design mode (an effective context will get injected)
        //DataContext = new MapEditorViewModel(new DesignMapEditorModel());
        DataContextChanged += (_, _) =>
        {
            if (DataContext is IMapEditorViewModel vm)
            {
                Console.WriteLine("MapEditorViewModel attached to view");
                MapDisplay.Map = vm.MapData;
            }

            _recentMapsViewModel = DataContext as MapEditorViewModel;
        };

        Opened += async (_, _) =>
        {
            var startupVm = new StartupViewModel();
            var startup = new StartupWindow { DataContext = startupVm };
            var result = await startup.ShowDialog<string>(this);

            Console.WriteLine($"Startup result: {result}");
            if (String.IsNullOrEmpty(result))
            {
                Close();
            }
            else
            {
                Console.WriteLine($"Startup result: {result}");

                if (result == "OpenMap")
                {
                    Console.WriteLine("the startup window should have covered the OpenMap option");
                    Close();
                }
                else
                {
                    if (DataContext is IMapEditorViewModel vm)
                        vm.OpenMap(result);
                }
            }
        };

        Closing += (_, _) =>
        {
            if (DataContext is IMapEditorViewModel { CurrentMapName: not null } vm)
                vm.CloseMap();
        };

        MapDisplay.PointerPressed += OnPointerPressed;
    }

    private void OnFileMenuOpened(object? sender, RoutedEventArgs e)
    {
        _recentMapsViewModel = DataContext as MapEditorViewModel;
        RebuildRecentMapsMenu();
    }

    private void RebuildRecentMapsMenu()
    {
        foreach (var item in _recentMapMenuItems)
            FileMenu.Items.Remove(item);

        _recentMapMenuItems.Clear();

        if (_recentMapsViewModel is null)
            return;

        var insertAt = FileMenu.Items.IndexOf(RecentMapsAnchor) + 1;
        foreach (var mapName in _recentMapsViewModel.RecentMaps)
        {
            var item = new MenuItem
            {
                Header = mapName,
                Command = _recentMapsViewModel.OpenRecentMapCommand,
                CommandParameter = mapName
            };

            FileMenu.Items.Insert(insertAt++, item);
            _recentMapMenuItems.Add(item);
        }
    }

    /// <summary>
    /// Attaches the context menu for the current map selection.
    /// </summary>
    /// <param name="items">The items that the view model wants to display.</param>
    /// <remarks>
    /// The context menu isn't shown here. Avalonia opens the attached <c>ContextMenu</c> when it
    /// raises <see cref="Control.ContextRequested"/> (on release of the right button).
    /// </remarks>
    private void SetContextMenu(IReadOnlyList<ContextMenuItem> items)
    {
        if (items.Count == 0)
        {
            MapDisplay.ContextMenu = null;
            return;
        }

        var menu = new ContextMenu();

        foreach (var item in items)
        {
            menu.Items.Add(item.IsSeparator
                ? new Separator()
                : new MenuItem
                {
                    Header = item.Header,
                    Command = item.Command,
                    IsChecked = item.IsChecked
                });
        }

        MapDisplay.ContextMenu = menu;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var vm = DataContext as IMapEditorViewModel;
        if (vm is null)
            return;
        
        // c.f. EditingController.MouseDown
        
        if (e.Properties.IsRightButtonPressed)
        {
            SetContextMenu(vm.GetContextMenuItems());
        }
        else
        {
            var screenPosition = e.GetPosition(MapDisplay);
            var (gx, gy) = MapDisplay.Map.Navigator.Viewport.ScreenToWorldXY(screenPosition.X, screenPosition.Y);
            var p = new Position(gx, gy);
            vm.OnLeftClick(p);
        }
        
        e.Handled = true;
    }
}
