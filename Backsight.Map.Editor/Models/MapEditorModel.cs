using Backsight.Database;

namespace Backsight.Map.Editor.Models;

public interface IMapEditorModel
{
    string MapName => "none";
}

public sealed class DesignMapEditorModel : IMapEditorModel
{
}

public class MapEditorModel : IMapEditorModel
{
    private readonly IEnvironmentRepository _repo;
    
    public MapEditorModel(IEnvironmentRepository repo)
    {
        _repo = repo;
        _repo.Load();
    }

    public string MapName => _repo.Name;
}