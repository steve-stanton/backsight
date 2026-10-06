namespace Backsight.Map.Editor.Windows;

/// <summary>
/// Base class for view models that represent a page in a <see cref="WizardViewModel"/>.
/// </summary>
public abstract class PageModel : ViewModelBase
{
    /// <summary>
    /// Handles a request to go back to the previous page.
    /// </summary>
    internal virtual void OnBack()
    {
    }

    /// <summary>
    /// Handles a request to go to the next page.
    /// </summary>
    internal virtual void OnNext()
    {
    }
}