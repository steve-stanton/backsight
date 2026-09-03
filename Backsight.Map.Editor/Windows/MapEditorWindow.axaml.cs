using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Mapsui.Extensions;

namespace Backsight.Map.Editor.Windows;

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

    /// <summary>
    /// Any keys that are currently pressed.
    /// </summary>
    private KeySelection _keySelection = KeySelection.None;

    public MapEditorWindow()
    {
        InitializeComponent();

        // Start out with a context that's only suitable in design mode (an effective context will get injected)
        //DataContext = new MapEditorViewModel(new DesignMapEditorModel());
        DataContextChanged += (_, _) =>
        {
            if (DataContext is IMapEditorViewModel vm)
            {
                //Console.WriteLine("MapEditorViewModel attached to view");
                MapDisplay.Map = vm.MapData;
            }
        };

        Opened += async (_, _) =>
        {
            if (DataContext is MapEditorViewModel vm)
            {
                var mapSupplied = await vm.Startup();
                if (!mapSupplied)
                    Close();
            }
        };

        Closing += async (_, _) =>
        {
            ClearDockPanel();

            if (DataContext is IMapEditorViewModel { CurrentMapName: not null } vm)
                await vm.CloseMap();
        };

        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;

        MapDisplay.PointerPressed += OnPointerPressed;
        MapDisplay.PointerMoved += OnPointerMoved;
        MapDisplay.PointerReleased += OnPointerReleased;
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
        var insertAt = FileMenu.Items.IndexOf(RecentMapsSection) + 1;
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
        if (DataContext is not MapEditorViewModel vm)
            return;

        // c.f. EditingController.MouseDown

        var mb = e.Properties.MouseButton;

        if (mb == MouseButton.Right)
        {
            SetContextMenu(vm.GetContextMenuItems());
        }
        else
        {
            var p = GetWorldPosition(e);
            vm.OnMouseDown(p, mb, _keySelection);
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
            vm.OnMouseMove(p, e.Properties.MouseButton);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (DataContext is MapEditorViewModel vm)
        {
            var p = GetWorldPosition(e);
            vm.OnMouseUp(p, e.Properties.MouseButton);
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not MapEditorViewModel vm)
            return;

        //Console.WriteLine($"MapEditorWindow KeyDown={e.Key} {e.KeyModifiers}");

        // The KeyEventArgs don't tell you which key just went down, it tells you all the keys that are now down
        _keySelection = e.KeySelection;

        if (e.Key == Key.Escape)
            vm.Escape();
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        // The KeyEventArgs don't tell you which key just went up, it tells you what keys are still down
        _keySelection = e.KeySelection;
    }

    /// <summary>
    /// Sets the content of the dock panel to the specified dialog.
    /// </summary>
    /// <param name="dialog">The dialog to display.</param>
    /// <exception cref="InvalidOperationException">DockPanel already has content.</exception>
    /// <exception cref="ArgumentException">Dialog content must be a control.</exception>
    internal void SetDockPanel(DialogWindow dialog)
    {
        ArgumentNullException.ThrowIfNull(dialog);
        
        if (DockContainer.Content is not null)
            throw new InvalidOperationException("DockPanel already has content.");
        
        if (dialog.Content is not Control content)
            throw new ArgumentException("Dialog content must be a control.");

        // The content can only have one visual parent, so clear before we transfer it to the dock panel
        dialog.Content = null;

        // Pull over the data context
        if (content.DataContext is null && dialog.DataContext is not null)
            content.DataContext = dialog.DataContext;

        DockContainer.Width = dialog.Width; // might be NaN
        if (dialog.Background is not null)
            DockContainer.Background = dialog.Background;

        DockContainer.Content = content;
        DockContainer.IsVisible = true;
    }

    /// <summary>
    /// Clears anything currently in the dock panel.
    /// This should be called when the dialog is being closed.
    /// </summary>
    internal void ClearDockPanel()
    {
        DockContainer.Content = null;
        DockContainer.IsVisible = false;
    }

    private void OnPaneGrabBarDragDelta(object? sender, VectorEventArgs e)
    {
        const double minWidth = 150;
        const double maxWidth = 800;
        var newLength = SplitView.OpenPaneLength + e.Vector.X;
        SplitView.OpenPaneLength = Math.Clamp(newLength, minWidth, maxWidth);
    }
}
