using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backsight.Database;

namespace Backsight.Model;

/// <summary>
/// An implementation of <c>IMapRepository</c> that stores maps using the local file system.
/// </summary>
public class MapsDirectory : IMapRepository
{
    private static readonly JsonSerializerOptions Options = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// The path for the root folder (should always refer to a folder that exists).
    /// Each map is stored in a sub-folder with a name that matches the map name.
    /// </summary>
    readonly string _mapsFolderPath;
    
    /// <summary>
    /// The database that defines the operating environment.
    /// </summary>
    readonly IEnvironmentRepository _envRepo;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapsDirectory"/> class.
    /// </summary>
    /// <param name="envRepo">The environment repository to use.</param>
    /// <remarks>
    /// The default path is located under <see cref="System.Environment.SpecialFolder.CommonApplicationData"/>, in
    /// a sub-folder called <c>Backsight/maps</c> (on a Windows machine, this is <c>C:\\ProgramData\Backsight\maps</c>).
    /// But if that path does not already exist, it uses a <c>maps</c> folder within the application folder.
    /// </remarks>
    public MapsDirectory(IEnvironmentRepository envRepo)
    {
        _envRepo = envRepo;

        var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
        _mapsFolderPath = Path.Combine(appData, "Backsight", "maps");

        if (!Directory.Exists(_mapsFolderPath))
        {
            var entryFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? throw new ApplicationException());
            Debug.Assert(entryFolder is not null);
            _mapsFolderPath = Path.Combine(entryFolder, "maps");
            Console.WriteLine("Using assembly location as root folder for map data: " + _mapsFolderPath);
            
            // The folder should already exist because the app should have placed sample
            // there as embedded resources. But make sure anyway.
            if (!Directory.Exists(_mapsFolderPath))
            {
                Console.WriteLine("Creating maps folder: " + _mapsFolderPath);
                Directory.CreateDirectory(_mapsFolderPath);
            }
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> FindAllMapNames()
    {
        var result = new List<string>();

        foreach (var dir in  Directory.EnumerateDirectories(_mapsFolderPath).OrderBy(s => s))
            result.Add(Path.GetFileName(dir));
        
        return result.ToArray();
    }

    /// <inheritdoc />
    public void CreateMap(string mapName, MapSettings settings)
    {
        // Confirm that the map name is unique
        var mapFolder = Path.Combine(_mapsFolderPath, mapName);
        if (Directory.Exists(mapFolder))
            throw new ArgumentException("Map already exists");

        // Create the data folder
        Directory.CreateDirectory(mapFolder);

        // Record a map creation event
        var eventData = new NewProjectEvent
        {
            MapId = Guid.NewGuid(),
            LayerId = settings.ActiveLayer,
            DefaultSystem = String.Empty,
            UserName = System.Environment.UserName,
            MachineName = System.Environment.MachineName
        };
        RecordChange(mapName, eventData, 1);
        
        // And save the settings
        settings.SavedItemCount = eventData.EditSequence;
        SaveMapSettings(mapName, settings);
    }
    
    /// <inheritdoc />
    public bool CanOpen(string mapName)
    {
        var mapFolder = Path.Combine(_mapsFolderPath, mapName);
        return Directory.Exists(mapFolder);
    }

    /// <inheritdoc />
    public IMapStore OpenMap(string mapName)
    {
        // Ensure that the map exists and we can load editing/display preferences (this should help
        // to identify any junk folders created under the maps folder).
        var mapFolder = Path.Combine(_mapsFolderPath, mapName);
        if (!Directory.Exists(mapFolder))
            throw new ArgumentException("Map not found: " + mapName);
        
        // Maybe work with a MapFolder (IMapStore) class that is focused on one specific map  

        // Load preferences for the map (creates if not already present)
        var settings = GetMapSettings(mapName);
        
        // Make sure that the active layer is defined
        if (settings.ActiveLayer == 0)
        {
            var layerName = GlobalUserSetting.Read("DefaultLayer");
            if (String.IsNullOrEmpty(layerName))
            {
                layerName = "Survey";
                GlobalUserSetting.Write("DefaultLayer", layerName);
            }
            
            var layer = _envRepo.Layers.FirstOrDefault(x => x.Name == layerName);
            if (layer is null)
                throw new ApplicationException("Default layer not found: " + layerName);

            settings.ActiveLayer = layer.Id;
            settings.GetDefaults(layer);
            SaveMapSettings(mapName, settings);
        }
        
        // Get rid of any undo folder that may be left over from a crashed editing session
        DeleteUndoFolder(mapName);
        
        // Now do the heavy-lifting
        var store = new MapStore(mapName, this, _envRepo, settings);
        LoadEdits(mapName, store);

        GlobalUserSetting.Write("LastMap", mapName);
        GlobalUserSetting.UpdateRecentMaps(mapName);
        
        // Ensure the settings record the SavedItemCount property (if we've just replaced the settings.json
        // file, it will hold a value of 0)
        if (settings.SavedItemCount == 0)
        {
            settings.SavedItemCount = store.ItemCount;
            SaveMapSettings(mapName, settings);
        }
        
        // Record details for a new session
        uint sessionId = ++store.ItemCount;
        Console.WriteLine("New session: " + sessionId);
        var change = new NewSessionEvent(sessionId, System.Environment.UserName, System.Environment.MachineName);
        RecordChange(mapName, change, 1);
        
        // Append a working session that's empty
        var session = new Session(store, change);
        store.Model.AddSession(session);
        store.Model.SetWorkingSession(session);
        
        return store;
    }

    /// <inheritdoc />
    public void CloseMap(IMapStore store)
    {
        var session = store.Model.WorkingSession;
        if (session is null)
            throw new ApplicationException("No working session");
        
        // Remove any undo folder
        DeleteUndoFolder(store.Name);

        // Combine data files for the session (and append an EndSessionEvent)
        CompleteSession(session); 

        // Completion of the session normally increments the store's item count (to refer
        // to the EndSessionEvent), so save the updated settings
        SaveMapSettings(store.Name, store.Settings);
    }

    /// <inheritdoc />
    public uint RemoveChanges(IMapStore store)
    {
        // Pick up the numbers of the files created after the last savepoint
        string mapFolder = Path.Combine(_mapsFolderPath, store.Name);
        uint[] fileNumbers = GetFileNumbers(mapFolder, store.Settings.SavedItemCount + 1);

        foreach (var fileNum in fileNumbers)
        {
            var fileName = Path.Combine(mapFolder, GetDataFileName(fileNum));
            File.Delete(fileName);
        }
        
        store.ItemCount = store.Settings.SavedItemCount;

        return (uint)fileNumbers.Length;
    }
    
    /// <summary>
    /// Completes a session by appending an <see cref="EndSessionEvent"/>, and combines
    /// operation data files into a single file. Any data files that are beyond the last savepoint
    /// will be discarded.
    /// </summary>
    /// <param name="session">The session to be completed.</param>
    private void CompleteSession(Session session)
    {
        var store = session.MapStore;

        // Pick up the numbers of the files that relate to the session
        string mapFolder = Path.Combine(_mapsFolderPath, store.Name);
        uint[] fileNumbers = GetFileNumbers(mapFolder, session.ItemNumber);

        // Do nothing if everything in the session (including the NewSessionEvent) has been removed
        if (fileNumbers.Length == 0)
            return;
        
        // If all we have is the session start event, just discard it
        if (fileNumbers.Length == 1 && fileNumbers[0] == session.ItemNumber)
        {
            DeleteFile(store.Name, fileNumbers[0]);
            return;
        }

        // Create an event for the end of the session
        var endEvent = new EndSessionEvent(++store.ItemCount);
        var endFile = Path.Combine(_mapsFolderPath, store.Name, GetDataFileName(endEvent.EditSequence));

        // Combine the files
        using (StreamWriter sw = File.CreateText(endFile))
        {
            foreach (uint fileNum in fileNumbers)
            {
                var fileName = Path.Combine(mapFolder, GetDataFileName(fileNum));
                var s = File.ReadAllText(fileName);
                sw.Write(s);
            }

            // And finish off with the end event
            var endText = EditSerializer.GetSerializedString(DataField.Edit, endEvent);
            sw.Write(endText);
        }

        // Get rid of the files that we've just combined
        foreach (uint fileNum in fileNumbers)
        {
            string fileName = Path.Combine(mapFolder, GetDataFileName(fileNum));
            File.Delete(fileName);
        }
    }
    
    /// <summary>
    /// Reads map preferences from a settings file (creates it if it doesn't already exist).
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <returns>The map preferences</returns>
    /// <exception cref="SerializationException">An existing settings file could not be deserialized.</exception>
    public MapSettings GetMapSettings(string mapName)
    {
        var fileName = Path.Combine(_mapsFolderPath, mapName, "settings.json");
        if (File.Exists(fileName))
        {
            var text = File.ReadAllText(fileName);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            var result = JsonSerializer.Deserialize<MapSettings>(text, options);
            if (result is not null)
                return result;
            
            throw new SerializationException("Invalid settings file: " + fileName);
        }
        else
        {
            // TODO: If we have an old settings.txt file (containing settings in xml), replace with json
            
            var result = new MapSettings();
            SaveMapSettings(mapName, result);
            return result;
        }
    }

    /// <summary>
    /// Saves a map settings file.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <param name="settings">The preferences to be saved.</param>
    public void SaveMapSettings(string mapName, MapSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, Options);
        var fileName = Path.Combine(_mapsFolderPath, mapName, "settings.json");
        File.WriteAllText(fileName, json);
        settings.IsDirty = false;
    }

    /// <summary>
    /// Ensures any undo folder for a specific map has been removed (usually done when an
    /// editing session is being closed).
    /// </summary>
    private void DeleteUndoFolder(string mapName)
    {
        var undoFolder = Path.Combine(_mapsFolderPath, mapName, "undo");
        if (Directory.Exists(undoFolder))
            Directory.Delete(undoFolder, true);
    }

    /// <summary>
    /// Obtains the path for a map undo folder (creating it if it does not already exist).
    /// </summary>
    /// <returns>The path for the undo folder.</returns>
    private static string GetUndoFolder(string mapFolder)
    {
        string undoFolder = Path.Combine(mapFolder, "undo");
        if (!Directory.Exists(undoFolder))
            Directory.CreateDirectory(undoFolder);

        return undoFolder;
    }

    /// <summary>
    /// Loads a map store with the edits found in a map folder.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <param name="store">The store to load into.</param>
    private void LoadEdits(string mapName, MapStore store)
    {
        // Note the file numbers of the data files to load
        var dataFolder = Path.Combine(_mapsFolderPath, mapName);
        uint[] fileNums = GetFileNumbers(dataFolder, 0);
        if (fileNums.Length == 0)
            throw new ArgumentException("Map doesn't have any data files");

        // Now load the files
        LoadDataFiles(store, dataFolder, fileNums);
        
        // If the last session didn't complete as expected, complete it now and ensure
        // session files have been combined into a single file.
        var lastSession = store.Model.LastSession;
        if (lastSession is not null && lastSession.EndTime is null)
        {
            Console.WriteLine("Last session did not complete as expected, completing now");
            CompleteSession(lastSession);
            SaveMapSettings(store.Name, store.Settings);            
        }
    }
    
    /// <summary>
    /// Gets the numbers of the data files in the map folder.
    /// </summary>
    /// <param name="dataFolder">The folder containing the data files</param>
    /// <param name="startFileNumber">The earliest file number to pick up</param>
    /// <returns>The data file numbers (sorted). An empty array if the map folder does not exist.</returns>
    private uint[] GetFileNumbers(string dataFolder, uint startFileNumber)
    {
        if (!Directory.Exists(dataFolder))
            return [];

        var result = new List<uint>(100);

        foreach (string s in Directory.GetFiles(dataFolder))
        {
            string name = Path.GetFileNameWithoutExtension(s);
            uint n;
            //if (name.Length == 8 && UInt32.TryParse(name, NumberStyles.HexNumber, null, out n))
            if (UInt32.TryParse(name, out n) && n >= startFileNumber)
                result.Add(n);
        }

        // There's a good chance the files will already be sorted, but just in case
        result.Sort();
        return result.ToArray();
    }

    private void LoadDataFiles(MapStore store, string mapFolderName, uint[] fileNums)
    {
        if (fileNums.Length == 0)
            return;
        
        Trace.Write("Reading data...");
        var ed = new EditDeserializer(store);

        foreach (uint fileNum in fileNums)
        {
            string editFile = Path.Combine(mapFolderName, fileNum + ".txt");

            using (TextReader tr = File.OpenText(editFile))
            {
                var er = new TextEditReader(tr);

                // Ignore any empty files altogether
                while (er.HasNext)
                {
                    ed.SetReader(er);
                    store.Load(ed);
                }
            }
        }

        // Apply any forward references
        ed.ApplyForwardRefs();

        // Remember the highest item number used by the project
        store.ItemCount = fileNums[^1];
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">The value for <paramref name="itemCount"/> must be greater than zero.</exception>
    public void RecordChange<T>(string mapName, T change, uint itemCount) where T : Change
    {
        if (itemCount == 0)
            throw new ArgumentException(nameof(itemCount));
        
        var mapFolder = Path.Combine(_mapsFolderPath, mapName);
        var fileNumber = change.EditSequence + itemCount - 1;
        var fileName = Path.Combine(mapFolder, GetDataFileName(fileNumber));
        
        if (File.Exists(fileName))
            throw new ApplicationException("File already exists: " + fileName);
        
        var changeText = EditSerializer.GetSerializedString(DataField.Edit, change);
        File.WriteAllText(fileName, changeText);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">The value for <paramref name="itemCount"/> must be greater than zero.</exception>
    public bool RemoveChange<T>(string mapName, T change, uint itemCount) where T : Change
    {
        if (itemCount == 0)
            throw new ArgumentException(nameof(itemCount));

        var mapFolder = Path.Combine(_mapsFolderPath, mapName);
        var fileNumber = change.EditSequence + itemCount - 1;
        var fileName = Path.Combine(mapFolder, GetDataFileName(fileNumber));
        var srcFileName = Path.Combine(mapFolder, fileName);
        if (!File.Exists(srcFileName))
            return false;
        
        // Move the file to an undo sub-folder
        var undoFolder = GetUndoFolder(mapFolder);
        var dstFileName = Path.Combine(undoFolder, fileName);
        File.Move(srcFileName, dstFileName);
        return true;
    }

    private void DeleteFile(string mapName, uint fileNumber)
    {
        var mapFolder = Path.Combine(_mapsFolderPath, mapName);
        var fileName = Path.Combine(mapFolder, GetDataFileName(fileNumber));
        File.Delete(fileName);
    }

    /// <summary>
    /// Gets the name of the data file that corresponds to a file number.
    /// </summary>
    /// <param name="fileNumber">The file number</param>
    /// <returns>The corresponding file name (without any directory specification).</returns>
    private static string GetDataFileName(uint fileNumber)
    {
        //return String.Format("{0:X8}.txt", fileNumber);
        return $"{fileNumber}.txt";
    }
}