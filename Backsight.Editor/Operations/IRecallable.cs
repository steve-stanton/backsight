using System;

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// Tagging interface for editing operations that may be recalled using the
    /// Cadastral Editor's command recall facility (the <c>Edit - Recall</c> command).
    /// </summary>
    /// <remarks>Tagging the editing operation is merely a convenience, arising from
    /// the fact that editing commands are wired up to the user interface via the
    /// <see cref="EditingAction"/> class (which identifies each edit through an editing ID).
    /// However, the editing operation does not need to do anything to accommodate command
    /// recall - rather, it is the UI class that runs the edit that needs to do stuff.
    /// </remarks>
    interface IRecallable
    {
    }
}
