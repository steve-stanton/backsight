using System;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;

namespace Backsight.Map.Editor.Tools;

internal abstract class CommandTool : IDisposable
{
    private readonly MapEditorViewModel _viewModel;
    private readonly EditingActionId _editId;
    private readonly IMapStore _mapStore;
    private readonly Session _session;

    protected CommandTool(MapEditorViewModel viewModel, EditingActionId editId)
    {
        _viewModel = viewModel;
        _mapStore = viewModel.Store ?? throw new InvalidOperationException("Cannot create a command tool without an active map.");
        _session = _mapStore.Model.WorkingSession ?? throw new InvalidOperationException("Cannot create a command tool without an active session.");
        _editId = editId;
    }

    internal MapEditorViewModel ViewModel => _viewModel;
    
    internal Session WorkingSession => _session;
    
    /// <summary>
    /// The map that this command is operating on.
    /// </summary>
    internal IMapStore Store => _mapStore;

    /// <summary>
    /// Runs this command tool.
    /// </summary>
    /// <returns>True if the tool was launched as expected. False if it did not.</returns>
    internal abstract bool Run();

    internal void FinishDataEntry<T>(T viewModel) where T : class
    {
        Finish();
    }

    internal void CancelDataEntry<T>(T viewModel) where T : class
    {
        Abort();
    }

    /// <summary>
    /// Reacts to a situation where the user presses the ESC key, by aborting this command.
    /// </summary>
    internal void Escape()
    {
        Abort();
    }
    
    /// <summary>
    /// Aborts this command.
    /// </summary>
    /// <returns>True (always).</returns>
    protected virtual bool Abort()
    {
        ViewModel.AbortCommand(this);
        /*
        // If this command was invoked by an update command, get
        // the update to clean up. Otherwise tell the controller.

        if (m_UpdCmd is not null)
            m_UpdCmd.AbortCommand(this);
        else
            this.Controller.AbortCommand(this);

        // Ensure that any reserved IDs have been released
        IdManager idMan = CadastralMapModel.Current.IdManager;
        if (idMan != null)
            idMan.FreeAllReservedIds();
            */

        return true;
    }
    
    /// <summary>
    /// Finishes this command.
    /// </summary>
    /// <returns>True (always).</returns>
    protected virtual bool Finish()
    {
        /*
        // If this command was invoked by an update command, get
        // the update to clean up. Otherwise tell the controller.

        if (m_UpdCmd is not null)
            m_UpdCmd.FinishCommand(this);
        else
            Controller.FinishCommand(this);
*/
        ViewModel.FinishCommand(this);
        return true;
    }
    
    /// <summary>
    /// Handles a mouse down event.
    /// </summary>
    /// <param name="p">The position (in ground units) where the mouse down event occurred.</param>
    /// <param name="b">What mouse button was pressed.</param>
    /// <returns>This implementation always returns false, indicating that the tool does
    /// not handle mouse down events. Derived classes that do handle mouse down events
    /// should return true (regardless of the outcome).</returns>
    internal virtual bool MouseDown(IPosition p, MouseButton b)
    {
        return false;
    }

    internal virtual void MouseMove(IPosition p, MouseButton b)
    {
    }

    public virtual void Dispose()
    {
    }

    /// <summary>
    /// Renders anything special relating to this command.
    /// </summary>
    /// <param name="canvas">The canvas to render to.</param>
    /// <remarks>
    /// This will be called after the map renderer has drawn the map and any selected features.
    /// </remarks>
    internal virtual void Render(MapCanvas canvas)
    {
        // Do nothing
    }

    /// <summary>
    /// Refreshes the map display to reflect the current state of the data entry command.
    /// </summary>
    internal void RefreshMapDisplay()
    {
        ViewModel.RefreshMapDisplay();
    }
}