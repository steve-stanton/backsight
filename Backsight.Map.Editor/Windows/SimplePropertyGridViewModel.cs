using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Backsight.Map.Editor.Windows;

public record PropertyItem(string Name, object? Value, bool IsExpandable)
{
    private SimplePropertyGridViewModel? _childViewModel;

    public SimplePropertyGridViewModel? ChildViewModel =>
        IsExpandable && Value is not null
            ? _childViewModel ??= new SimplePropertyGridViewModel(Value)
            : null;
}

public class SimplePropertyGridViewModel
{
    private readonly List<PropertyItem> _properties;

    public SimplePropertyGridViewModel(object target)
    {
        var props = TypeDescriptor.GetProperties(target);
        var items = new List<PropertyItem>();
        
        foreach (PropertyDescriptor prop in props.Cast<PropertyDescriptor>().Where(x => x.IsBrowsable))
        {
            var value = prop.GetValue(target);
            items.Add(new PropertyItem(prop.DisplayName, value, value is IExpandablePropertyItem));
        }
        
        _properties = items.OrderBy(x => x.Name).ToList();
    }
    
    public IReadOnlyList<PropertyItem> Properties => _properties;
}