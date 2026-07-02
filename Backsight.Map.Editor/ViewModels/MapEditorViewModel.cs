using Backsight.Map.Editor.Models;

namespace Backsight.Map.Editor.ViewModels;

public interface IMapEditorViewModel
{
}

public partial class MapEditorViewModel : ViewModelBase, IMapEditorViewModel
{
    private readonly IMapEditorModel _model;

    public MapEditorViewModel() : this(new DesignMapEditorModel())
    {
    }

    public MapEditorViewModel(IMapEditorModel model)
    {
        _model = model;
    }
}