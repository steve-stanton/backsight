using System.Diagnostics;
using Backsight.Database;
using Backsight.Environment;

namespace Backsight.Model;

/// <summary>
/// Model-related functions required by the <see cref="EditDeserializer"/>.
/// </summary>
interface IMapLoader
{
    Operation FindOperation(InternalIdValue id);
    T Find<T>(InternalIdValue id) where T : Feature;
    NativeId AddNativeId(uint rawId);
    ForeignId AddForeignId(string key);
    Session LastSession { get; }
}

public class MapStore : IMapStore
{
    private readonly string _mapName;
    private readonly IMapRepository _mapRepo;
    private readonly IEnvironmentRepository _envRepo;

    /// <summary>
    /// Editing and display preferences for the map.
    /// </summary>
    private readonly MapSettings _settings;
    
    // TODO: pull class data into this class?
    private readonly CadastralMapModel _model;
    
    private uint _maxSequence = 0;

    /// <summary>
    /// The last internal ID value assigned to something in this map.
    /// </summary>
    uint _lastItemId;
    
    internal MapStore(
        string mapName,
        IMapRepository mapRepo,
        IEnvironmentRepository envRepo,
        MapSettings settings)
    {
        _mapName = mapName;
        _mapRepo = mapRepo;
        _envRepo = envRepo;
        _settings = settings;
        _model = new CadastralMapModel(_envRepo);
    }

    internal void Load(EditDeserializer ed)
    {
        Change edit = Change.Deserialize(ed);

        if (edit is NewProjectEvent mapInfo)
        {
            // If the project settings don't have default entity types, initialize them with
            // the layer defaults. This covers a case where the settings file has been lost, and
            // automatically re-created by ProjectSettings.CreateInstance.

            var layer = _envRepo.FindRequired<ILayer>(mapInfo.LayerId);
            _settings.GetDefaults(layer);
        }
        else if (edit is NewSessionEvent newSession)
        {
            var s = new Session(this, newSession);
            _model.AddSession(s);
        }
        else if (edit is EndSessionEvent)
        {
            var lastSession = _model.LastSession;
            Debug.Assert(lastSession is not null);
            lastSession.EndTime = edit.When;
        }
        else if (edit is IdAllocation alloc)
        {
            IdGroup g = _model.IdManager.FindGroupById(alloc.GroupId);
            g.AddIdPacket(alloc);

            // Remember that allocations have been made in the session (bit of a hack
            // to ensure the session isn't later removed if no edits are actually
            // performed).
            // TODO: Consider ID allocations as part of the env repository
            var lastSession = _model.LastSession;
            Debug.Assert(lastSession is not null);
            lastSession.AddAllocation(alloc, false);
        }
        else if (edit is Operation op)
        {
            var lastSession = _model.LastSession;
            Debug.Assert(lastSession is not null);
            lastSession.AddOperation(op);
        }
        else
        {
            throw new NotImplementedException("Unexpected edit type: " + edit.GetType().Name);
        }
    }
    
    /// <inheritdoc/>
    public CadastralMapModel Model => _model;
    
    public string Name => _mapName;
    
    /// <summary>
    /// The last internal ID value assigned to something in this map.
    /// </summary>
    /// <remarks>
    /// On a call to <see cref="SaveChanges"/>, this value will be assigned to <see cref="Settings.SavedItemCount"/>.
    /// </remarks>
    public uint ItemCount
    {
        get => _lastItemId;
        set => _lastItemId = value;
    }

    /// <summary>
    /// Undoes the last edit in the current working session.
    /// </summary>
    /// <returns>True if an edit was rolled back, false if there are no edits in the current session (or
    /// the last edit has already been saved).</returns>
    /// <remarks>
    /// You <b>should</b> be able to undo even if you have saved the edit via a call to <see cref="SaveChanges"/> - in
    /// that scenario, the value for <see cref="ItemCount"/> could just be reduced. The problem is that the map
    /// repository may also include items that are not regarded as "edits" (e.g. instances of <see cref="IdAllocation"/>).
    /// So if we undo a saved edit prior to that, anything recorded after that will also need to be removed.
    /// <para/>
    /// Ideally the map repository should only use the <see cref="ItemCount"/> for edits (i.e. anything that
    /// extends from <see cref="Operation"/>). But to do that, we need some other way to persist non-edits.
    /// In the meantime, the application forces a save following any ID allocation and, while that is also
    /// problematic, it means that we can avoid any issue by only allowing rollback to the last save.
    /// </remarks>
    public bool UndoLastEdit()
    {
        var session = _model.WorkingSession;
        if (session is null)
            throw new InvalidOperationException("Working session not set");

        // Disallow an attempt to undo past the start of the working session
        var lastOp = session.LastOperation;
        if (lastOp is null)
            return false;

        // Disallow an attempt to undo past the last save (see remarks)
        if (lastOp.EditSequence < _settings.SavedItemCount)
            return false;
        
        // Remove the change from the repository
        var itemCount = ItemCount + 1 - lastOp.EditSequence;
        bool isRemoved = _mapRepo.RemoveChange(_mapName, lastOp, itemCount);
        if (!isRemoved)
            throw new ApplicationException($"Could not remove change {ItemCount}");
        
        // Undo the last operation
        if (!session.Rollback())
            return false;
        
        ItemCount = lastOp.EditSequence - 1;
        Model.CleanEdit();
        return true;
    }

    /// <inheritdoc/>
    public void SaveChanges()
    {
        // Changes are recorded as we go. All we need to do now is update the saved item count to match
        // the current item count.
        _settings.SavedItemCount = _lastItemId;
        _mapRepo.SaveMapSettings(_mapName, _settings);
    }

    /// <summary>
    /// Editing and display preferences for the map.
    /// </summary>
    public MapSettings Settings => _settings;

    /// <inheritdoc />
    /// TODO: Could be an IMapStore extension method
    public void RecordChange<T>(T change, uint itemCount) where T : Change
    {
        _mapRepo.RecordChange(_mapName, change, itemCount);
    }

    /// <inheritdoc />
    public bool IsSaved => _lastItemId == _settings.SavedItemCount ||
                           _lastItemId == (Model.WorkingSession?.ItemNumber ?? _lastItemId);

    /// <inheritdoc />
    public List<T> Query<T>(IWindow window) where T : Feature
    {
        var result = new List<T>();
        var type = GetSpatialType<T>();
        
        Model.Index.QueryWindow(window, type, item =>
        {
            if (item is T t)
                result.Add(t);
                
            return true;
        });

        return result;
    }

    private static SpatialType GetSpatialType<T>() where T : Feature
    {
        var typeName = typeof(T).Name;

        return typeName switch
        {
            nameof(PointFeature) => SpatialType.Point,
            nameof(LineFeature) => SpatialType.Line,
            nameof(TextFeature) => SpatialType.Text,
            _ => throw new NotImplementedException(typeName)
        };
    }

    /// <summary>
    /// The default entity type for point features.
    /// </summary>
    public IEntity DefaultPointType => FindEntityById(_settings.Defaults?.PointType ?? 0);
    
    /// <summary>
    /// The default entity type for line features.
    /// </summary>
    public IEntity DefaultLineType => FindEntityById(_settings.Defaults?.LineType ?? 0);

    /// <summary>
    /// The default entity type for polygon labels.
    /// </summary>
    public IEntity DefaultPolygonType => FindEntityById(_settings.Defaults?.PolygonType ?? 0);

    /// <summary>
    /// The default entity type for miscellaneous text features.
    /// </summary>
    public IEntity DefaultTextType => FindEntityById(_settings.Defaults?.TextType ?? 0);
    
    private IEntity FindEntityById(int entityId) => _envRepo.FindRequired<IEntity>(entityId);
}