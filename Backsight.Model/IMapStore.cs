namespace Backsight.Model;

public interface IMapStore
{
    /// <summary>
    /// The internal ID of the map.
    /// </summary>
    //Guid Id { get; }
    
    /// <summary>
    /// The user-perceived name of the map.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The most recent map settings. 
    /// </summary>
    MapSettings Settings { get; }
    
    /// <summary>
    /// The total number of items in this store.
    /// </summary>
    uint ItemCount { get; set; }

    /// <summary>
    /// The old map model.
    /// TODO: Refactor as part of the IMapStore implementation.
    /// </summary>
    CadastralMapModel Model { get; }

    /// <summary>
    /// Undoes the last edit in the current working session.
    /// TODO: Would it be better to have the working session as part of the IMapEditorModel? When
    /// the user saves, copy the saved edits into the IMapStore (the "store" is therefore the
    /// place where edits have been committed).
    /// </summary>
    /// <returns>True if an edit was rolled back.</returns>
    bool UndoLastEdit();
    
    /// <summary>
    /// Records a change to the map.
    /// </summary>
    /// <param name="change">The change to record.</param>
    /// <param name="itemCount">The sequence number of the peak item number associated with the change (must be
    /// a value greater than or equal to <see cref="Change.EditSequence"/>).</param>
    /// <typeparam name="T">The type of change.</typeparam>
    /// <remarks>The change will remain pending until it is saved via a call to <see cref="SaveChanges"/>.</remarks>   
    void RecordChange<T>(T change, uint itemCount) where T : Change;
    
    /// <summary>
    /// Saves any changes that have been recorded.
    /// </summary>
    void SaveChanges();
    
    /// <summary>
    /// Have all changes been saved?
    /// </summary>
    bool IsSaved { get; }

    /// <summary>
    /// Searches for map features within a covering rectangle.
    /// </summary>
    /// <param name="window">The search window.</param>
    /// <typeparam name="T">The specific feature type.</typeparam>
    /// <returns>The features of the requested type, and with an extent that overlaps the search window.</returns>
    List<T> Query<T>(IWindow window) where T : Feature;
}