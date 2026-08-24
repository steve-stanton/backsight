using System.Runtime.CompilerServices;

namespace Backsight.Map.Editor.Windows;

public partial class MessageBoxWindow : DialogWindow<MessageBoxViewModel>
{
    public MessageBoxWindow() : this("Design-time message", "Message")
    {
    }

    public MessageBoxWindow(string message, string heading) : base(new MessageBoxViewModel(message, heading))
    {
        InitializeComponent();
    }
}