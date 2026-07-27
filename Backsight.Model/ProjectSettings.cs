using System.Diagnostics;
using System.Xml.Serialization;
using Backsight.Environment;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="29-APR-2008" />
/// <summary>
/// Current settings for a Backsight project.
/// <para/>
/// This class hold transient properties relating to the Cadastral Editor application -
/// things like the position for the last draw, as well as editing defaults that the user has
/// the ability to respecify. The important thing to remember is that only the most recent project
/// settings are persisted. Thus, no edit should rely implicitly on these properties (the persistent
/// version of each edit must be able to stand alone).
/// </summary>
[XmlRoot]
public class ProjectSettings
{
    /// <summary>
    /// Have changes been made to the values stored in this instance?
    /// Set to <c>true</c> on a call to <see cref="Set"/>. Set to <c>false</c>
    /// on a call to <see cref="WriteXML"/>.
    /// </summary>
    bool m_IsChanged;

    /// <summary>
    /// Information about the area that was last drawn.
    /// TODO: Where should this live? It relates to the view that will be displayed the
    /// next time the map gets opened. But does that mean it belongs in the map model?
    /// Perhaps it would fit better as part of the model if it had a name like WorkingArea.
    /// Or if it was called WorkSettings, it could also contain things like the current
    /// display units etc. Maybe EditPreferences?
    /// </summary>
    WorkingArea _mWorkingArea;

    /// <summary>
    /// Current display units
    /// </summary>
    DistanceUnitType m_DisplayUnit;

    /// <summary>
    /// Current data entry units
    /// </summary>
    DistanceUnitType m_EntryUnit;

    /// <summary>
    /// Should feature IDs be assigned automatically? (false if the user must specify).
    /// </summary>
    bool m_AutoNumber;

    /// <summary>
    /// Scale denominator at which labels (text) will start to be drawn.
    /// </summary>
    double m_ShowLabelScale;

    /// <summary>
    /// Scale denominator at which points will start to be drawn.
    /// </summary>
    double m_ShowPointScale;

    /// <summary>
    /// Height of point symbols, in meters on the ground.
    /// </summary>
    double m_PointHeight;

    /// <summary>
    /// Should intersection points be drawn? Relevant only if points
    /// are drawn at the current display scale (see the <see cref="ShowPointScale"/>
    /// property).
    /// </summary>
    bool m_AreIntersectionsDrawn;

    /// <summary>
    /// The nominal map scale, for use in converting the size of fonts.
    /// </summary>
    uint m_MapScale;

    /// <summary>
    /// The style for annotating lines with distances (and angles)
    /// </summary>
    LineAnnotationStyle m_Annotation;

    /// <summary>
    /// The ID of the default entity type for points (0 if undefined)
    /// When a map gets created, this will default to the value defined via the ILayer.
    /// But the user can subsequently change this on a map-by-map basis.
    /// So what happens if the user decided to switch to a different ILayer? Perhaps the
    /// entity type doesn't relate at all to that layer?
    /// So does that mean that the defaults need to be qualified with the layer?
    /// From what I can tell, an entity type can be associated with 0:1 layers. If IEntity.LayerId == 0,
    /// it applies to all layers. But if non-zero, it's meant only for that layer. Looking in the db,
    /// I see only the "Assessment Parcel" entity type has a defined LayerId. So it wouldn't be seen
    /// at all if you switched to the survey layer.
    /// </summary>
    int m_DefaultPointType;

    /// <summary>
    /// The ID of the default entity type for lines (0 if undefined)
    /// </summary>
    int m_DefaultLineType;

    /// <summary>
    /// The ID of the default entity type for polygon labels (0 if undefined)
    /// </summary>
    int m_DefaultPolygonType;

    /// <summary>
    /// The ID of the default entity type for text (0 if undefined)
    /// </summary>
    int m_DefaultTextType;

    /// <summary>
    /// Default constructor (for serialization mechanism)
    /// </summary>
    public ProjectSettings()
    {
        // Display
        _mWorkingArea = new WorkingArea(0.0, 0.0, 0.0);
        m_DisplayUnit = DistanceUnitType.AsEntered;
        m_ShowLabelScale = 2000.0;
        m_ShowPointScale = 2000.0;
        m_PointHeight = 2.0;
        m_AreIntersectionsDrawn = false;
        m_MapScale = 2000;
        m_Annotation = new LineAnnotationStyle();
        
        // Data entry
        m_EntryUnit = DistanceUnitType.Meters;
        m_AutoNumber = true;
        m_DefaultPointType = 0;
        m_DefaultLineType = 0;
        m_DefaultPolygonType = 0;
        m_DefaultTextType = 0;

        // State
        m_IsChanged = false;
    }

    /// <summary>
    /// Method called whenever values of this class are changed. This just ensures
    /// that <see cref="m_IsChanged"/> gets set.
    /// </summary>
    /// <typeparam name="T">The type of value that's being changed</typeparam>
    /// <param name="value">The value to assign</param>
    /// <returns>The supplied value</returns>
    T Set<T>(T value)
    {
        m_IsChanged = true;
        return value;
    }

    /// <summary>
    /// Reads project settings from an XML file.
    /// </summary>
    /// <param name="fileName">The file spec for the input data</param>
    /// <returns>The data read from the input file</returns>
    public static ProjectSettings CreateInstance(string fileName)
    {
        // If the file doesn't already exist, create something. The file won't have any defaults for entity types, because
        // we don't know the map layer here - they'll get defined when the map layer is picked up by Project.LoadDataFiles.
        if (!File.Exists(fileName))
            new ProjectSettings().WriteXML(fileName);

        XmlSerializer xs = new XmlSerializer(typeof(ProjectSettings));
        using (TextReader reader = new StreamReader(fileName))
        {
            ProjectSettings result = (ProjectSettings)xs.Deserialize(reader);
            result.m_IsChanged = false;
            return result;
        }
    }

    /// <summary>
    /// Writes project information to an XML file.
    /// </summary>
    /// <param name="fileName">The output file (to create)</param>
    public void WriteXML(string fileName)
    {
        // Create the directory if it doesn't already exist
        string dir = Path.GetDirectoryName(fileName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        XmlSerializer xs = new XmlSerializer(typeof(ProjectSettings));
        using (TextWriter writer = new StreamWriter(fileName))
        {
            xs.Serialize(writer, this);
            m_IsChanged = false;
        }
    }

    /// <summary>
    /// Information about the area that was last drawn.
    /// </summary>
    [XmlElement]
    public WorkingArea LastDraw
    {
        get => _mWorkingArea;
        set => _mWorkingArea = Set<WorkingArea>(value);
    }

    /// <summary>
    /// Current display units
    /// </summary>
    [XmlElement("DisplayUnit")]
    public DistanceUnitType DisplayUnitType
    {
        get => m_DisplayUnit;
        set => m_DisplayUnit = Set<DistanceUnitType>(value);
    }

    /// <summary>
    /// Current data entry units
    /// </summary>
    [XmlElement("EntryUnit")]
    public DistanceUnitType EntryUnitType
    {
        get => m_EntryUnit;
        set => m_EntryUnit = Set<DistanceUnitType>(value);
    }

    /// <summary>
    /// Should feature IDs be assigned automatically? (false if the user must specify).
    /// </summary>
    [XmlElement("AutoNumber")]
    public bool IsAutoNumber
    {
        get => m_AutoNumber;
        set => m_AutoNumber = Set<bool>(value);
    }

    /// <summary>
    /// Scale denominator at which labels (text) will start to be drawn.
    /// </summary>
    [XmlElement("LabelScale")]
    public double ShowLabelScale
    {
        get => m_ShowLabelScale;
        set => m_ShowLabelScale = Set<double>(value);
    }

    /// <summary>
    /// Scale denominator at which points will start to be drawn.
    /// </summary>
    [XmlElement("PointScale")]
    public double ShowPointScale
    {
        get => m_ShowPointScale;
        set => m_ShowPointScale = Set<double>(value);
    }

    /// <summary>
    /// Height of point symbols, in meters on the ground.
    /// </summary>
    [XmlElement]
    public double PointHeight
    {
        get => m_PointHeight;
        set => m_PointHeight = Set<double>(value);
    }

    /// <summary>
    /// Should intersection points be drawn? Relevant only if points
    /// are drawn at the current display scale (see the <see cref="ShowPointScale"/> property).
    /// </summary>
    [XmlElement("IntersectionsDrawn")]
    public bool AreIntersectionsDrawn
    {
        get => m_AreIntersectionsDrawn;
        set => m_AreIntersectionsDrawn = Set<bool>(value);
    }

    /// <summary>
    /// The nominal map scale, for use in converting the size of fonts.
    /// </summary>
    [XmlElement]
    public uint NominalMapScale
    {
        get => m_MapScale;
        set => m_MapScale = Set<uint>(value);
    }

    /// <summary>
    /// The style for annotating lines with distances (and angles)
    /// </summary>
    [XmlElement]
    public LineAnnotationStyle LineAnnotation
    {
        get => m_Annotation;
        set => m_Annotation = Set<LineAnnotationStyle>(value);
    }

    /// <summary>
    /// The ID of the default entity type for points (0 if undefined)
    /// </summary>
    [XmlElement]
    public int DefaultPointType
    {
        get => m_DefaultPointType;
        set => m_DefaultPointType = Set<int>(value);
    }

    /// <summary>
    /// The ID of the default entity type for lines (0 if undefined)
    /// </summary>
    [XmlElement]
    public int DefaultLineType
    {
        get => m_DefaultLineType;
        set => m_DefaultLineType = Set<int>(value);
    }

    /// <summary>
    /// The ID of the default entity type for polygons (0 if undefined)
    /// </summary>
    [XmlElement]
    public int DefaultPolygonType
    {
        get => m_DefaultPolygonType;
        set => m_DefaultPolygonType = Set<int>(value);
    }

    /// <summary>
    /// The ID of the default entity type for text (0 if undefined)
    /// </summary>
    [XmlElement]
    public int DefaultTextType
    {
        get => m_DefaultTextType;
        set => m_DefaultTextType = Set<int>(value);
    }

    /// <summary>
    /// Has the information recorded in this instance been saved to disk?
    /// </summary>
    internal bool IsSaved => !m_IsChanged;

    /// <summary>
    /// Ensures default entity types have been defined.
    /// </summary>
    /// <param name="layer">The map layer for the project</param>
    internal void SetEntityTypeDefaults(ILayer layer)
    {
        if (layer == null)
        {
            Trace.WriteLine("ProjectSettings.SetEntityTypeDefaults: Undefined layer");
            return;
        }

        if (DefaultPointType == 0)
            DefaultPointType = layer.DefaultPointType.Id;

        if (DefaultLineType == 0)
            DefaultLineType = layer.DefaultLineType.Id;

        if (DefaultPolygonType == 0)
            DefaultPolygonType = layer.DefaultPolygonType.Id;

        if (DefaultTextType == 0)
            DefaultTextType = layer.DefaultTextType.Id;
    }
}