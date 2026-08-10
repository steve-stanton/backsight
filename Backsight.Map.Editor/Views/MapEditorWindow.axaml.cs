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
        
        KeyDown += OnKeyDown;

        MapDisplay.PointerPressed += OnPointerPressed;
        MapDisplay.PointerMoved += OnPointerMoved;
    }

    private void OnFileMenuOpened(object? sender, RoutedEventArgs e)
    {
        RebuildRecentMapsMenu();
    }

    private void RebuildRecentMapsMenu()
    {
        if (DataContext is not MapEditorViewModel vm)
            return;

        // Clear out anything we previously had
        foreach (var item in _recentMapMenuItems)
            FileMenu.Items.Remove(item);

        _recentMapMenuItems.Clear();

        // Insert recent map names underneath the "Recent Maps" anchor
        var insertAt = FileMenu.Items.IndexOf(RecentMapsAnchor) + 1;
        foreach (var mapName in vm.RecentMaps)
        {
            var item = new MenuItem
            {
                Header = mapName,
                Command = vm.OpenRecentMapCommand,
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
            var p = GetWorldPosition(e);
            vm.OnLeftClick(p);
        }
        
        e.Handled = true;
    }

    private Position GetWorldPosition(PointerEventArgs e)
    {
        var screenPosition = e.GetPosition(MapDisplay);
        var (gx, gy) = MapDisplay.Map.Navigator.Viewport.ScreenToWorldXY(screenPosition.X, screenPosition.Y);
        return new Position(gx, gy);
    }
    

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is MapEditorViewModel vm)
        {
            var p = GetWorldPosition(e);
            vm.OnMouseMove(p);
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        const Key escapeKey = Key.Escape;
        Console.WriteLine($"MapEditorWindow Key={e.Key} {e.KeyModifiers}");

        if (DataContext is not MapEditorViewModel vm)
            return;
        /*
        if (e.KeyModifiers == KeyModifiers.Alt)
        {
            bool redraw = false;
            
            if (e.Key == Key.Left)
                redraw = _drawHistory.SetPrevious();
            else if (e.Key == Key.Right)
                redraw = _drawHistory.SetNext();

            if (redraw)
                DrawExtent();
        }
*/
        if (e.Key == escapeKey)
            vm.Escape();
    }

}
