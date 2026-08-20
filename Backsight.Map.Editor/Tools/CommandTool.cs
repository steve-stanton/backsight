using System;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;

namespace Backsight.Map.Editor.Tools;

internal abstract class CommandTool : IDisposable
{
    private readonly MapEditorViewModel _viewModel;
    private readonly EditingActionId _editId;

    protected CommandTool(MapEditorViewModel viewModel, EditingActionId editId)
    {
        _viewModel = viewModel;
        _editId = editId;
    }

    protected MapEditorViewModel ViewModel => _viewModel;

    internal abstract bool Run();

    /// <summary>
    /// Aborts this command.
    /// </summary>
    /// <returns>True (always).</returns>
    protected bool Abort()
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
    protected bool Finish()
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
}