namespace Backsight.Map.Editor.Windows;

public partial class SaveChangesWindow : DialogWindow<SaveChangesViewModel>
{
    public SaveChangesWindow(string message = "Save changes?")
        : base(new SaveChangesViewModel(message))
    {
        InitializeComponent();
    }
}