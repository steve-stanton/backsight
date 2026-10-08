using RTools_NTS.Util;

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

    /// <summary>
    /// Performs any validation for the current page.
    /// </summary>
    /// <returns></returns>
    internal virtual bool ValidatePage() => true;
    
    /// <summary>
    /// Performs any initialization that needs to happen when
    /// the page is shown.
    /// </summary>
    internal virtual void OnNavigatedTo() { }
}