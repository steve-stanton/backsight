namespace Backsight.Map.Editor.Windows;

public partial class SaveChangesViewModel : DialogViewModel
{
    protected override DialogResult PositiveResult => DialogResult.Yes;
    
    protected override DialogResult NegativeResult => DialogResult.No;
    
    public string Message { get; }

    internal SaveChangesViewModel(string message)
    {
        Message = message;
    }
}