namespace Backsight.Map.Editor.Windows;

/// <summary>
/// The way that an instance of <see cref="DialogWindow"/> was closed.
/// </summary>
/// <remarks>
/// Dialogs will usually update properties of the associated view model, which you may want to
/// access after the dialog has returned an <see cref="OK"/> result.
/// </remarks>
public enum DialogResult
{
    /// <summary>
    /// The dialog was not closed by either the OK or the Cancel button (e.g. the user
    /// may have closed the dialog via the [x] in the window title bar).
    /// </summary>
    /// <remarks>
    /// Probably best to treat this in the same way as <see cref="Cancel"/>.
    /// </remarks>
    None = 0,
    
    /// <summary>
    /// The dialog was completed as expected (typically by pressing an OK button).
    /// </summary>
    OK,
    
    /// <summary>
    /// The dialog was not completed (typically by pressing a Cancel button). In that
    /// case, any data entry properties exposed by the dialog's view model should be disregarded.
    /// </summary>
    Cancel,
    
    /// <summary>
    /// The dialog was completed by pressing a Yes button.
    /// </summary>
    Yes,
    
    /// <summary>
    /// The dialog was completed by pressing a No button.
    /// </summary>
    No
}