using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Backsight.Map.Editor.Windows;

public partial class PickButton : UserControl
{
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PickButton, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<PickButton, object?>(nameof(CommandParameter));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public PickButton()
    {
        InitializeComponent();
    }
}