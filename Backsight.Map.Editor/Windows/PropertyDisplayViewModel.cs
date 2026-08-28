using System;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// View model for the <see cref="PropertyDisplay"/> control.
/// </summary>
public partial class PropertyDisplayViewModel : ViewModelBase
{
    [ObservableProperty]
    private TabItem[] _tabs;
    
    public PropertyDisplayViewModel()
    {
        _tabs = [];
    }

    internal void Update(IMapSelection selection)
    {
        Console.WriteLine("Update view model");
        object? item = selection.Items.SingleOrDefault();

        if (item is null)
            Tabs = [];
        else
        {
            // TODO: Create additional tabs for each associated row
            var tab = CreateTabItem(item);
            Tabs = [tab];
        }
    }

    private TabItem CreateTabItem(object content)
    {
        return new TabItem
        {
            Header = "Properties",
            Content = CreatePropertyGrid(content)
        };
    }
    
    private SimplePropertyGrid CreatePropertyGrid(object o)
    {
        return new SimplePropertyGrid
        {
            DataContext = new SimplePropertyGridViewModel(o)
        };
    }
}