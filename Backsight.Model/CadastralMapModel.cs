using System.Diagnostics;
using Backsight.Database;
using Backsight.Environment;

namespace Backsight.Model;

public class CadastralMapModel
{
    /// <summary>
    /// Spatial index for the data in this model.
    /// TODO: Move this to IMapEditorModel?
    /// </summary>
    private EditingIndex m_Index;

    /// <summary>
    /// Default rotation angle for text (in radians).
    /// TODO: Should be part of MapSettings
    /// </summary>
    double m_Rotation;

    /// <summary>
    /// The coordinate system.
    /// </summary>
    readonly ISpatialSystem m_CS;

    /// <summary>
    /// Window of all data in the map.
    /// </summary>
    readonly Window m_Window;

    /// <summary>
    /// Editing sessions.
    /// </summary>
    readonly List<Session> m_Sessions;

    /// <summary>
    /// The edits that have been performed in the editing sessions.
    /// </summary>
    readonly Dictionary<InternalIdValue, Operation> m_Edits;

    /// <summary>
    /// Management of user-specified IDs.
    /// </summary>
    IdManager m_IdManager;

    /// <summary>
    /// The session that we are currently appending to.
    /// </summary>
    Session? m_WorkingSession;

    /// <summary>
    /// Spatial features that have been loaded (including features that may
    /// have been deactivated).
    /// </summary>
    readonly Dictionary<InternalIdValue, Feature> m_Features;

    /// <summary>
    /// Index of feature IDs that are based on the Backsight numbering strategy (as opposed
    /// to user-perceived IDs that originate from some foreign source). The key
    /// is the raw ID, the value the created ID object.
    /// </summary>
    readonly Dictionary<uint, NativeId> m_NativeIds;

    /// <summary>
    /// Index of all foreign IDs (typically IDs that get defined via import from some
    /// alien data source).
    /// </summary>
    readonly Dictionary<string, ForeignId> m_ForeignIds;

    /// <summary>
    /// Creates a new empty model
    /// </summary>
    internal CadastralMapModel() : this(EnvironmentRepository.Current)
    {
    }

    /// <summary>
    /// Creates a new empty model
    /// </summary>
    public CadastralMapModel(IEnvironmentRepository envRepo)
    {
        m_Rotation = 0.0;
        m_Window = new Window();
        m_Sessions = new List<Session>();
        m_Edits = new Dictionary<InternalIdValue, Operation>();
        m_Index = new EditingIndex();
        m_IdManager = new IdManager(envRepo);
        m_Features = new Dictionary<InternalIdValue, Feature>(1000);
        m_NativeIds = new Dictionary<uint, NativeId>(1000);
        m_ForeignIds = new Dictionary<string, ForeignId>(1000);

        // TODO: Allow for different coordinate systems
        m_CS = CoordinateSystem.DefaultSystem;
    }

    /// <summary>
    /// The spatial index for this model (in a form that's suitable for typical queries).
    /// </summary>
    public IExtendedMapIndex Index => m_Index;

    /// <summary>
    /// The spatial index for this model.
    /// </summary>
    internal EditingIndex EditingIndex => m_Index;

    /// <summary>
    /// Default rotation angle for text (in radians).
    /// TODO: This looks like a map project setting
    /// </summary>
    internal double DefaultTextRotation
    {
        get => m_Rotation;
        set => m_Rotation = value;
    }

    /// <summary>
    /// The editing sessions that define this model
    /// </summary>
    internal Session[] Sessions => m_Sessions.ToArray();

    public IWindow Extent => m_Window;

    public IMapObject? QueryClosest(IPosition p, ILength radius, SpatialType types)
    {
        return m_Index.QueryClosest(p, radius, types);
    }

    /// <summary>
    /// The sequence number of the last edit in the working session (0 if no edits have
    /// been performed)
    /// </summary>
    internal uint LastOpSequence => m_WorkingSession?.LastOperation?.EditSequence ?? 0;

    /// <summary>
    /// The last editing session in this model (null if this is a freshly created model,
    /// and data is still being loaded)
    /// </summary>
    internal Session LastSession
    {
        get
        {
            int numSession = m_Sessions.Count;
            return numSession==0 ? null : m_Sessions[numSession-1];
        }
    }

    public ISpatialSystem SpatialSystem => m_CS;

    /// <summary>
    /// Creates a new point feature as part of this model.
    /// The caller is responsible for assigning any ID that the new point should have.
    /// </summary>
    /// <param name="p">The position for the point (not null). If it's an instance of
    /// IPointGeometry, it will be re-used as the geometry for the new feature (otherwise
    /// a new instance of PointGeometry will be created).
    /// </param>
    /// <param name="e">The entity type for the point (not null)</param>
    /// <param name="creator">The operation creating the point</param>
    /// <returns>The created point feature.</returns>
    internal PointFeature AddPoint(IPosition p, IEntity e, Operation creator)
    {
        if (m_WorkingSession is null)
            throw new InvalidOperationException("Working session not set");
        
        PointGeometry g = PointGeometry.Create(p);
        InternalIdValue id = m_WorkingSession.AllocateNextId();
        PointFeature f = new PointFeature(creator, id, e, g);
        //m_Window.Union(p);
        //m_Index.Add(f);
        return f;
    }

    /*
    /// <summary>
    /// Ensures a point feature exists at a specific location in this map model.
    /// The caller is responsible for assigning any ID that the new point should have.
    /// </summary>
    /// <param name="p">The position where a point feature is required</param>
    /// <param name="creator">The operation that should be recorded as the creator
    /// of any newly created point</param>
    /// <returns>The point feature at the specified position (may be a new point)</returns>
    /// <remarks>
    /// I think it may be a bad idea to use this method. In the past, edits have used it
    /// to refer to a point that happened to exist at a given position. However, if positional
    /// data changes over time, it's possible that a point coincident at T1 will not be coincident
    /// at T2. If the deserialization logic follows the logic that was used when the edit was
    /// originally performed (which it should), things could go haywire if a previously existing
    /// point was re-used, but has now moved. The guiding rule is that an edit should
    /// <b>always create the same number of features</b> whenever it is executed (either the
    /// initial run, or on deserialation from the database).
    /// </remarks>
    [Obsolete("Problems with changes over time (see remarks)")]
    internal PointFeature EnsurePointExists(IPosition p, Operation creator)
    {
        if (p is PointFeature)
            return (p as PointFeature);

        EditingIndex index = EditingIndex;
        ILength tol = new Length(MathConstants.XYRES);
        PointFeature pf = (index.QueryClosest(p, tol, SpatialType.Point) as PointFeature);
        //PointFeature pf = (index.QueryClosest(p, Length.Zero, SpatialType.Point) as PointFeature);
        if (pf==null)
            pf = AddPoint(p, DefaultPointType, creator);

        return pf;
    }
    */

    /// <summary>
    /// Creates a new line feature that connects two points.
    /// </summary>
    /// <param name="from">The starting point for the new line</param>
    /// <param name="to">The end point for the new line</param>
    /// <param name="e">The entity type for the line (not null)</param>
    /// <param name="creator">The operation creating the line</param>
    /// <returns>The created line segment.</returns>
    internal LineFeature AddLine(PointFeature from, PointFeature to, IEntity e, Operation creator)
    {
        if (m_WorkingSession is null)
            throw new InvalidOperationException("Working session not set");

        InternalIdValue id = m_WorkingSession.AllocateNextId();
        LineFeature f = new LineFeature(creator, id, e, from, to);
        //m_Window.Union(f.Extent);
        //m_Index.Add(f);

        // Ensure polygons in the vicinity have been marked for rebuild
        f.MarkPolygons();

        return f;
    }

    /// <summary>
    /// Ensures that any IDs associated with an array of features have been indexed
    /// as part of this model.
    /// </summary>
    /// <param name="fa">The features that need to be indexed</param>
    internal void AddFeatureIds(Feature[] fa)
    {
        foreach (Feature f in fa)
        {
            FeatureId fid = f.FeatureId;

            if (fid is ForeignId foreignId)
            {
                ForeignId? existingId = FindForeignId(foreignId.FormattedKey);
                if (existingId is null)
                    AddForeignId(foreignId);
                else if (!object.ReferenceEquals(foreignId, existingId))
                    throw new Exception("More than one foreign ID object for: "+foreignId.FormattedKey);
            }
            else if (fid is NativeId nativeId)
            {
                NativeId? existingId = FindNativeId(nativeId.RawId);
                if (existingId is null)
                    m_NativeIds.Add(nativeId.RawId, nativeId);
                else if (!object.ReferenceEquals(nativeId, existingId))
                    throw new Exception("More than one native ID object for: "+nativeId.RawId);
            }
            else
            {
                throw new NotImplementedException("Unsupported ID type: " + fid.GetType().Name);
            }
        }
    }

    /// <summary>
    /// The circles associated with an array of features
    /// </summary>
    /// <param name="fa">The features of interest</param>
    /// <returns>The circles (if any) that are associated with the supplied features</returns>
    static List<Circle> GetCreatedCircles(Feature[] fa)
    {
        if (fa.Length==0)
            return [];

        // The following will be overkill in most cases, but not for things like
        // bulk data imports...

        // The circles found so far will be noted in an index keyed by the
        // internal ID of the point at the center of the circle.
        var dic = new Dictionary<InternalIdValue, List<Circle>>();

        var result = new List<Circle>(100);

        foreach (Feature f in fa)
        {
            if (f is ArcFeature feature)
            {
                Circle c = feature.Circle;

                if (c.Creator == feature.Creator)
                {
                    InternalIdValue centerPointId = c.CenterPoint.InternalId;
                    bool addToResult = false;
                    List<Circle>? circles;

                    if (dic.TryGetValue(centerPointId, out circles))
                    {
                        Debug.Assert(circles is not null);
                        
                        if (circles.IndexOf(c)<0)
                        {
                            circles.Add(c);
                            addToResult = true;
                        }
                    }
                    else
                    {
                        circles = new List<Circle>(1);
                        circles.Add(c);
                        dic.Add(centerPointId, circles);
                        addToResult = true;
                    }

                    if (addToResult)
                        result.Add(c);
                }
            }
        }

        result.TrimExcess();
        return result;
    }

    /// <summary>
    /// Cleans up after each edit
    /// </summary>
    internal void CleanEdit()
    {
        // Do usual cleaning stuff.
        var cq = new CleanupQuery(this);

        // Ensure that all moved stuff has been intersected
        // (and that any trimmed lines have been adjusted appropriately).
        //System.Windows.Forms.MessageBox.Show("starting intersect");
        //Stopwatch sw = Stopwatch.StartNew();
        //if (cq.Moves.Count > 0)
        //{
        Intersect(cq.Moves);
        //    new CleanupQuery(this);
        //}
        //sw.Stop();
        //System.Windows.Forms.MessageBox.Show(sw.Elapsed.ToString());

        //System.Windows.Forms.MessageBox.Show("building topology");
        //sw.Reset();
        //sw.Start();
        BuildPolygons();
        //sw.Stop();
        //System.Windows.Forms.MessageBox.Show(sw.Elapsed.ToString());
    }

    /// <summary>
    /// Builds polygon topology for everything in the active layer of the map.
    /// </summary>
    /// <devnote>
    /// Should really return status to indicate whether an early return has been
    /// made due to unclean topology, or define current topological status as part
    /// of the model.
    /// </devnote><devnote>
    /// If unclean topology is detected, it may be desirable to continue processing
    /// of any other layers which exist in the map. If we are holding topological
    /// status (as alluded to above), we will need to have separate status indicators
    /// for each layer.
    /// </devnote>
    void BuildPolygons()
    {
        new PolygonBuilder(this).Build();
    }

    /// <summary>
    /// Intersects all features that have moved during rollforward.
    /// </summary>
    /// <param name="moves">The moved features.</param>
    void Intersect(List<Feature> moves)
    {
        // Return if nothing has moved
        if (moves.Count==0)
            return;

        Trace.Write("Intersecting "+moves.Count+" lines");
        var trims = new List<LineFeature>();
        int nMove = 0;

        foreach(Feature f in moves)
        {
            f.IsMoved = false;
            if (f is not LineFeature line)
                continue;

            nMove++;
            line.Split(true, trims);

            // If the line needs to be trimmed, add it to our list.
            // We'll do it at the end.

            /*
                // Note that IsTrimmed checks that both FFL_TRIM and
                // FFL_SYSTOPOL are set, which is kind of what you'd
                // expect after calling CeArc::Split (assuming that
                // the line intersects something). However, when intersects
                // are detected, they ignore stuff that is still marked as
                // moved. So we might not create any arc sections until
                // later on. By calling WasTrimmed() as well, we ensure
                // that the moved arc really does make it into the list
                // (WasTrimmed checks that FFL_TRIM is set and that
                // FFL_SYSTOPOL is clear). In the event that the line
                // no longer intersects anything, that will be trapped
                // when we call CeSplit::Trim below. Phew!

                if (line.IsTrimmed || line.WasTrimmed)
                    trims.Append(line);
             */
        }

        // If we moved anything, ensure that any newly marked polygons 
        // have been cleaned, and the spatial index is up to date.
        if (nMove > 0)
        {
            // If we hit any lines that need to be trimmed, do them now.
            // Any trimmed portions will be marked inactive.
            foreach (LineFeature trimLine in trims)
            {
                /*
                Split split = trimLine.Split;
                if (split != null)
                    split.Trim(null);
                 */
            }

            // This is a little bit of overkill
            var cq = new CleanupQuery(this);
        }

        /*
        // Clear out the list.
        moves.Clear();

        // If we moved anything, ensure that any newly marked polygons
        // have been cleaned, and the space is up to date. We don't need
        // to re-clean topological arcs, since splitting does not de-activate
        // them.

        // Oh yes it does! When line A is cut by line B, it produces arc
        // sections A1,A2,B1,B2. If A1 is also cut by line C, A1 will be
        // marked for deletion and replaced with A3,A4 (and C1,C2 will get
        // created).

        // If you don't clean this up, the line primitive for A1 will still
        // be marked active (still in spatial index). At the moment, BuildPolygons
        // will happily try to build topology for this, leading to an assertion as
        // it tries to form network topology. Although we COULD check there, and
        // leave the deleted stuff kicking around till the next CleanEdit. who
        // knows what other weird stuff could happen. So clean arcs too!

        // If this is really an issue (speed wise), we COULD avoid the call to
        // CleanArcs if system-defined arc sections were processed above, but none
        // of them created new arc sections.

        if (nMove>0)
        {
            // If we hit any arcs that need to be trimmed, do them now.
            // Any trimmed portions will be marked inactive.
            foreach(LineFeature trimLine in trims)
            {
                Split split = trimLine.Split;
                if (split!=null)
                    split.Trim(null);
            }

            var nomoves = new List<IMapObject>(); // shouldn't get any MORE moves
            CleanArcs(nomoves);
            Debug.Assert(nomoves.Count==0);
            CleanPolygons();

            // No need to clean text (well, I can't think of any).
            m_Index.Clean(m_UpdateWindow, false);
        }
         */
    }

    /*
     * This is likely relevant only in the ViewModel
     * 
    internal IEntity GetDefaultEntity(SpatialType t)
    {
        return t switch
        {
            SpatialType.Point => DefaultPointType,
            SpatialType.Line => DefaultLineType,
            SpatialType.Polygon => DefaultPolygonType,
            SpatialType.Text => DefaultTextType,
            _ => throw new NotImplementedException(nameof(GetDefaultEntity))
        };
    }

    /// <summary>
    /// Remembers the default entity type that should be assigned to features with
    /// a specific geometric type.
    /// </summary>
    /// <param name="t">The geometric type</param>
    /// <param name="e">The default entity type for any new features that
    /// are created with the specified geometric type (null for the blank type)</param>
    internal void SetDefaultEntity(SpatialType t, IEntity? e)
    {
        SetDefaultEntity(t, e?.Id ?? 0);
    }

    /// <summary>
    /// Remembers the default entity type that should be assigned to features with
    /// a specific geometric type.
    /// </summary>
    /// <param name="t">The geometric type</param>
    /// <param name="entityId">The internal ID of the default entity type for any new features that
    /// are created with the specified geometric type</param>
    internal void SetDefaultEntity(SpatialType t, int entityId)
    {
        ProjectSettings ps = EditingController.Current.Project.Settings;

        if (t == SpatialType.Point)
            ps.DefaultPointType = entityId;
        else if (t == SpatialType.Line)
            ps.DefaultLineType = entityId;
        else if (t == SpatialType.Polygon)
            ps.DefaultPolygonType = entityId;
        else if (t == SpatialType.Text)
            ps.DefaultTextType = entityId;
        else
            throw new NotImplementedException("SetDefaultEntityType");
    }
*/
    /// <summary>
    /// Finds all circles that pass near a specific position.
    /// </summary>
    /// <param name="p">The search position.</param>
    /// <param name="tol">The search tolerance (expected to be quite small).</param>
    /// <returns>The circles close to the search position (may be empty list)</returns>
    internal List<Circle> FindCircles(IPosition p, ILength tol)
    {
        return new FindCirclesQuery(m_Index, p, tol).Result;
    }

    /// <summary>
    /// Adds a circle to the model. If a circle with the specified radius is
    /// already attached to the point at the center of the circle, you get back the
    /// existing circle. If there is no such circle, one will be created, and the
    /// center point will be modified to refer to it.
    /// </summary>
    /// <param name="c">The point at the center.</param>
    /// <param name="radius">The radius (on the ground), in meters</param>
    /// <returns></returns>
    internal Circle AddCircle(PointFeature c, double radius)
    {
        // Try to match the center point with an existing circle with
        // the specified radius.
        Circle circle = c.GetCircle(radius);
        if (circle!=null)
            return circle;

        // Create a new circle.
        circle = new Circle(c, radius);

        // Refer the center point to the circle.
        circle.AddReferences();
        return circle;
    }

    /// <summary>
    /// Add a circular arc to the map
    /// </summary>
    /// <param name="circle">The circle on which the arc lies. This will be modified to refer
    /// to the newly created arc.</param>
    /// <param name="start">The point at the start of arc. This will be modified to refer
    /// to the newly created arc.</param>
    /// <param name="end">The point at the end of arc. This will be modified to refer
    /// to the newly created arc.</param>
    /// <param name="clockwise">True if the arc is clockwise.</param>
    /// <param name="lineEnt">The entity type for the new line.</param>
    /// <param name="creator">The editing operation creating the arc</param>
    /// <returns>The newly created line</returns>
    internal LineFeature AddCircularArc(Circle circle, PointFeature start, PointFeature end,
        bool clockwise, IEntity lineEnt, Operation creator)
    {
        if (m_WorkingSession is null)
            throw new InvalidOperationException("Working session not set");

        InternalIdValue id = m_WorkingSession.AllocateNextId();
        ArcFeature result = new ArcFeature(creator, id, lineEnt, circle, start, end, clockwise);
        //m_Window.Union(result.Extent);
        //m_Index.Add(result);

        // Ensure polygons in the vicinity have been marked for rebuild
        result.MarkPolygons();

        return result;
    }

    /*
    /// <summary>
    /// Rolls back the last operation known to this map. Does not save the map to disk.
    /// </summary>
    /// <param name="cursess">Specify true if the rollback should be restricted
    /// to operations performed during the current session. False if rollback can span
    /// sessions.</param>
    /// <returns>The code identifying the op that was rolled back. Zero if nothing
    /// was rolled back.</returns>
    internal uint Rollback(bool cursess)
    {
        // Go through each session (starting at the last). As soon
        // as we hit a session that can rollback something, we're done.
        int status = 0;

        if (cursess)
        {
            status = Session.WorkingSession.Rollback();
        }
        else
        {
            for (int i=m_Sessions.Count-1; i>=0 && status==0; i--)
            {
                Session s = m_Sessions[i];
                status = s.Rollback();
            }
        }

        // Return >0 only if we successfully rolled back; 0 if there was nothing to rollback,
        // or some error occurred during rollback.
        if (status>0)
        {
            CleanEdit();
            return (uint)status;
        }

        return 0;
    }
    */

    /// <summary>
    /// Remembers an edit as part of this model (should be done when the edit is recorded
    /// as part of the edit session).
    /// </summary>
    /// <param name="edit">The edit to include as part of this model.</param>
    internal void AddEdit(Operation edit)        
    {
        m_Edits.Add(new InternalIdValue(edit.EditSequence), edit);
    }

    /// <summary>
    /// Removes an edit that is part of this model (for use when undoing the last edit).
    /// </summary>
    /// <param name="edit">The edit to remove</param>
    /// <returns>True if the edit was removed, false if the supplied edit did not appear as part
    /// of this model.</returns>
    internal bool RemoveEdit(Operation edit)
    {
        // Ensure any user-perceived IDs have been removed, and clear the index entries
        // for their internal IDs
        foreach (Feature f in edit.Features)
        {
            FeatureId fid = f.FeatureId;
            if (fid != null)
                RemoveId(fid);

            m_Features.Remove(f.InternalId);
        }

        var id = new InternalIdValue(edit.EditSequence);
        return m_Edits.Remove(id);
    }

    /// <summary>
    /// Removes a user-perceived ID from this model (for use when the last edit is
    /// being undone).
    /// </summary>
    /// <param name="id">The ID to remove</param>
    /// <returns>True if a matching ID was removed. False if the supplied ID isn't one
    /// known to this model.</returns>
    internal bool RemoveId(FeatureId id)
    {
        if (id is NativeId)
            return m_NativeIds.Remove((id as NativeId).RawId);
        else
            return m_ForeignIds.Remove(id.FormattedKey);
    }

    /// <summary>
    /// Obtains a terminal at the specified position.
    /// </summary>
    /// <param name="p">The position of interest</param>
    /// <returns>The corresponding terminal (created if a terminal doesn't already exist
    /// at the position of interest)</returns>
    internal ITerminal GetTerminal(IPosition p)
    {
        // Check whether we have an existing point feature or intersection
        PointGeometry pg = PointGeometry.Create(p);
        ITerminal? t = m_Index.FindTerminal(pg);
        if (t is not null)
            return t;

        // Create an intersection and return that
        Intersection x = new Intersection(pg);
        m_Index.AddIntersection(x);
        return x;
    }

    /// <summary>
    /// Is an editing operation currently being saved? This implementation currently
    /// returns false (always), because the notion of a commit-in-progress does not seem
    /// relevant in the new implementation of the Cadastral Editor (though this may need
    /// to be considered again).
    /// </summary>
    internal bool IsCommittingEdit => false;

    /// <summary>
    /// Adds a new miscellaneous text label. 
    /// </summary>
    /// <param name="creator">The editing operation creating the text</param>
    /// <param name="s">The text string.</param>
    /// <param name="ent">The entity type for the string.</param>
    /// <param name="vtx">The position of the top-left corner of the first character of the text.</param>
    /// <param name="height">The height of the text, in meters on the ground.</param>
    /// <param name="width">The width of the text, in meters on the ground.</param>
    /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
    /// <returns>The newly created text</returns>
    internal TextFeature AddMiscText(Operation creator, string s, IEntity ent, IPosition vtx, double height,
        double width, double rotation)
    {
        if (m_WorkingSession is null)
            throw new InvalidOperationException("Working session not set");

        // Create the "geometry"
        PointGeometry topLeft = PointGeometry.Create(vtx);
        MiscTextGeometry text = new MiscTextGeometry(s, topLeft, ent.Font, height, width, (float)rotation);

        // Do standard stuff for adding a label.
        InternalIdValue id = m_WorkingSession.AllocateNextId();
        return new TextFeature(creator, id, ent, text);
    }

    /// <summary>
    /// Adds a label that is based on a reserved ID.
    /// </summary>
    /// <param name="creator">The editing operation creating the text</param>
    /// <param name="polygonId">The reserved ID and entity type for the the label.</param>
    /// <param name="vtx">The reference position of the label (the position of the top-left corner of the first character
    /// of the text).</param>
    /// <param name="height">The height of the text, in meters on the ground.</param>
    /// <param name="width">The width of the text, in meters on the ground.</param>
    /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
    /// <returns>The newly created text</returns>
    internal TextFeature AddKeyLabel(Operation creator, IdHandle polygonId, IPosition vtx,
        double height, double width, double rotation)
    {
        if (m_WorkingSession is null)
            throw new InvalidOperationException("Working session not set");

        // Exit with error if the key is not reserved.
        if (!polygonId.IsReserved)
            throw new ArgumentException("CadastralMapMode.AddKeyLabel - ID is undefined.");

        // Add the label.
        IEntity ent = polygonId.Entity;
        PointGeometry p = PointGeometry.Create(vtx);
        KeyTextGeometry text = new KeyTextGeometry(p, ent.Font, height, width, (float)rotation);
        InternalIdValue id = m_WorkingSession.AllocateNextId();
        TextFeature label = new TextFeature(creator, id, ent, text);

        // Define the label's ID
        polygonId.CreateId(label);
        text.Label = label;

        return label;
    }

    /// <summary>
    /// Adds a label that is based on a key text
    /// </summary>
    /// <param name="creator">The editing operation creating the text</param>
    /// <param name="ent">The entity type for the label.</param>
    /// <param name="vtx">The reference position of the label</param>
    /// <param name="height">The height of the text, in meters on the ground.</param>
    /// <param name="width">The width of the text, in meters on the ground.</param>
    /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
    /// <returns>The newly created text</returns>
    internal TextFeature AddKeyLabel(Operation creator, IEntity ent, IPosition vtx,
        double height, double width, double rotation)
    {
        if (m_WorkingSession is null)
            throw new InvalidOperationException("Working session not set");

        // Create a key-text primitive.
        PointGeometry pos = PointGeometry.Create(vtx);
        KeyTextGeometry text = new KeyTextGeometry(pos, ent.Font, height, width, (float)rotation);

        // Do standard stuff for adding a label
        InternalIdValue id = m_WorkingSession.AllocateNextId();
        TextFeature result = new TextFeature(creator, id, ent, text);
        text.Label = result;
        return result;
    }

    /// <summary>
    /// Adds a label that is based on a row.
    /// </summary>
    /// <param name="creator">The editing operation creating the text</param>
    /// <param name="polygonId">The ID and entity type to assign to the label.</param>
    /// <param name="vtx">The reference position of the label.</param>
    /// <param name="attributes">The database attributes for the label.</param>
    /// <param name="atemplate">The template for the row text.</param>
    /// <param name="height">The height of the text, in meters on the ground.</param>
    /// <param name="width">The width of the text, in meters on the ground.</param>
    /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
    /// <returns>The newly created text</returns>
    internal TextFeature AddRowLabel(Operation creator, IdHandle polygonId, IPosition vtx, AttributeRecord attributes,
        ITemplate atemplate, double height, double width, double rotation)
    {
        if (m_WorkingSession is null)
            throw new InvalidOperationException("Working session not set");
        
        if (!attributes.Table.Templates.Contains(atemplate))
            throw new ArgumentException($"Template {atemplate.Name} is not associated with {attributes.Table.TableName}.{attributes.Id}");

        // Exit with error if the key is not reserved.
        if (!polygonId.IsReserved)
            throw new ArgumentException();

        // Add the label with null geometry for now (chicken and egg -- need Feature in order
        // to create the Row object that's needed for the RowTextGeometry)
        IEntity ent = polygonId.Entity;
        InternalIdValue id = m_WorkingSession.AllocateNextId();
        TextFeature label = new TextFeature(creator, id, ent, null);

        // Define the label's ID and attach the row to it
        FeatureId fid = polygonId.CreateId(label);
        var row = new Row(fid, attributes);

        // Attach the geometry
        PointGeometry p = PointGeometry.Create(vtx);
        RowTextGeometry text = new RowTextGeometry(row, atemplate, p, ent.Font, height, width, (float)rotation);
        label.TextGeometry = text;

        return label;
    }

    /// <summary>
    /// Loads this model from the database
    /// </summary>
    public void Load()
    {
        var ctx = new LoadingContext();

        Trace.Write("Attaching attributes...");
        AttributeData.Load(GetFeatureIds());

        Trace.Write("Calculating geometry...");
        Operation[] edits = GetCalculationSequence();
        foreach (Operation op in edits)
            op.CalculateGeometry(ctx);

        // Create spatial index
        Trace.Write("Indexing...");
        CreateIndex(edits);

        // Ensure all sections have been extracted from the underlying line (aim to improve
        // performance on repetitive stuff, especially sections based on sections based on sections...).
        // It's safe to play around with the geometry objects, since the spatial index only
        // references the feature objects.
        m_Index.QueryWindow(null, SpatialType.Line, delegate(IMapObject item)
        {
            LineFeature line = (LineFeature)item;
            SectionGeometry section = (line.LineGeometry as SectionGeometry);
            if (section != null)
            {
                UnsectionedLineGeometry baseLine = section.SectionBase;
                line.LineGeometry = baseLine.Section(section);
            }

            return true;
        });


        // Intersect topological lines that aren't marked for deletion
        Trace.Write("Intersecting lines");

        // First mark all lines as "moved" (they get ignored by the intersect
        // finder until they have been intersected themselves)
        MarkAllLinesMoved();

        m_Index.QueryWindow(null, SpatialType.Line, delegate (IMapObject item)
        {
            LineFeature line = (LineFeature)item;
            line.IsMoved = false;
            line.Split(false, null);
            return true;
        });

        // Now build the topology for the map
        BuildPolygons();

        // Initialize ID handling. This associates ID allocations with their corresponding ID packet.
        int nDone = 0;
        foreach (NativeId nid in m_NativeIds.Values)
        {
            IdPacket p = nid.IdGroup.FindPacket(nid);
            Debug.Assert(p != null);
            p.SetId(nid);
            nDone++;
        }

        //System.Windows.Forms.MessageBox.Show("Number done = " + nDone);

        /*
        if (m_IdManager != null)
            m_IdManager.Load(this, project, user);

        // Now go through the sessions to notify the ID manager about IDs
        // that have been used

        foreach (Session s in m_Sessions)
        {
            if (s.Job.JobId == job.JobId && s.User.UserId == user.UserId)
                s.LoadUsedIds(m_IdManager);
        }
         */
    }

    /// <summary>
    /// Marks all spatially indexed lines as "moved". Lines marked in this way
    /// will be ignored by intersect detection software until they themselves
    /// have been intersected against the map.
    /// </summary>
    void MarkAllLinesMoved()
    {
        m_Index.QueryWindow(null, SpatialType.Line, delegate(IMapObject item)
        {
            LineFeature line = (LineFeature)item;
            line.IsMoved = true;
            return true;
        });
    }

    /// <summary>
    /// Initializes the number of elements in this model's session list
    /// </summary>
    /// <param name="numSession">The size of the session list.</param>
    internal void SetSessionCapacity(int numSession)
    {
        m_Sessions.Capacity = numSession;
    }

    /// <summary>
    /// Remembers a session as part of this model
    /// </summary>
    /// <param name="s"></param>
    internal void AddSession(Session s)
    {
        m_Sessions.Add(s);
    }

    /// <summary>
    /// The object that manages assignment of user-specified IDs.
    /// </summary>
    internal IdManager IdManager => m_IdManager;

    /// <summary>
    /// The session that we are currently appending to (null if the model is being deserialized).
    /// </summary>
    internal Session? WorkingSession => m_WorkingSession;

    /// <summary>
    /// Defines the current editing session.
    /// </summary>
    /// <param name="s">The session that new edits should be appended to.</param>
    /// <remarks>During deserialization, the model does not have a working session.</remarks>
    internal void SetWorkingSession(Session s)
    {
        m_WorkingSession = s;
    }

    /// <summary>
    /// Creates a spatial index for the supplied edits.
    /// </summary>
    /// <param name="edits">The edits to include in the index</param>
    internal void CreateIndex(Operation[] edits)
    {
        m_Index = new EditingIndex();

        foreach (Operation op in edits)
        {
            Feature[] createdFeatures = op.Features;
            AddToIndex(createdFeatures);
        }
    }

    /// <summary>
    /// Includes features created by an editing operation as part of the editing
    /// index. Also ensures the overal map extent has been expanded (if necessary)
    /// to include the extent of the features.
    /// </summary>
    /// <param name="fa">The features to add to the index</param>
    internal void AddToIndex(Feature[] fa)
    {
        EditingIndex index = this.EditingIndex;

        foreach (Feature f in fa)
        {
            // Ignore if the feature has no extent (this should apply only during 
            // deserialization of TextFeature instances that are associated with
            // RowTextGeometry). It would be nice to do all indexing after all
            // edits have been deserialized. However, I believe that some of the
            // deserialization logic expects previous edits to be indexed.

            // ...on second thoughts, this is just too messy. If deserialization
            // logic does really require access to a spatial index, the relevant
            // edits should be modified to avoid the dependency

            //IWindow x = f.Extent;
            //if (x != null)
            //{
            //    f.AddToIndex(index);
            //    m_Window.Union(f.Extent);
            //}
            //else
            //{
            //    Debug.Assert(f is TextFeature);
            //    TextFeature tf = (TextFeature)f;
            //    Debug.Assert(tf.TextGeometry is RowTextGeometry);
            //}

            if (f.AddToIndex(index))
                m_Window.Union(f.Extent);
        }

        // The extent of circles don't get included in the map extent, because
        // they're regarded only as construction lines (that should be invisible
        // to the user).

        List<Circle> createdCircles = GetCreatedCircles(fa);
        foreach (Circle c in createdCircles)
            c.AddToIndex(index);
    }

    /// <summary>
    /// Records a new feature loaded as part of this model. This indexes the
    /// feature by it's internal ID.
    /// </summary>
    /// <param name="f">The feature that has been loaded (if null, nothing gets done)</param>
    /// <remarks>This should be called only by the <see cref="Feature"/> constructor.</remarks>
    internal void AddFeature(Feature f)
    {
        if (f != null)
        {
            try { m_Features.Add(f.InternalId, f); }
            catch { throw new Exception("Failed to index feature " + f.InternalId); }
        }
    }

    /// <summary>
    /// Attempts to locate an editing operation based on its internal ID.
    /// </summary>
    /// <param name="id">The ID to look for</param>
    /// <returns>The corresponding operation (null if not found)</returns>
    internal Operation FindOperation(InternalIdValue id)
    {
        Operation result;
        if (m_Edits.TryGetValue(id, out result))
            return result;
        else
            return null;
    }

    /// <summary>
    /// Attempts to locate a spatial feature based on its internal ID.
    /// This is an indexed lookup.
    /// </summary>
    /// <param name="id">The ID to look for</param>
    /// <returns>The corresponding feature (null if not found)</returns>
    internal T Find<T>(InternalIdValue id) where T : Feature
    {
        Feature f;
        if (m_Features.TryGetValue(id, out f))
            return (f as T);
        else
            return null;
    }

    /// <summary>
    /// Tries to obtains a native ID for a feature.
    /// </summary>
    /// <param name="rawId">The raw ID to look for</param>
    /// <returns>The ID that corresponds to the supplied key. Null if there is no matching ID - make
    /// a call to <see cref="AddNativeId"/> to register a new ID.</returns>
    internal NativeId? FindNativeId(uint rawId)
    {
        return m_NativeIds.GetValueOrDefault(rawId);
    }

    /// <summary>
    /// Creates a new native ID and remembers it as part of this model. This will fail
    /// if a native key with the same key has already been added - first make a call
    /// to <see cref="FindNativeId"/> to see if the add is needed.
    /// </summary>
    /// <param name="rawId">The raw key for the new ID</param>
    /// <returns>The corresponding ID (usually not null, but could be if the connection
    /// to the ID database can no longer be established)</returns>
    /// <exception cref="ArgumentException">If an ID group that encloses the specified
    /// raw key cannot be found</exception>
    internal NativeId AddNativeId(uint rawId)
    {
        IdGroup group = m_IdManager.FindGroupByRawId(rawId);
        NativeId result = new NativeId(group, rawId);
        m_NativeIds.Add(rawId, result);
        return result;
    }

    /// <summary>
    /// Tries to obtains a foreign ID for a feature.
    /// </summary>
    /// <param name="key">The formatted key to look for</param>
    /// <returns>The ID that corresponds to the supplied key. Null if there is no matching ID - make
    /// a call to <see cref="AddForeignId"/> to register a new ID.</returns>
    internal ForeignId? FindForeignId(string? key)
    {
        if (key is null)
            return null;
        
        return m_ForeignIds.GetValueOrDefault(key);
    }

    /// <summary>
    /// Creates a new foreign ID and remembers it as part of this model. This will fail
    /// if a foreign key with the same key has already been added - first make a call
    /// to <see cref="FindForeignId"/> to see if the add is needed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The new ID object</returns>
    internal ForeignId AddForeignId(string key)
    {
        var result = new ForeignId(key);
        m_ForeignIds.Add(key, result);
        return result;
    }

    /// <summary>
    /// Remembers a foreign ID as part of this model.
    /// </summary>
    /// <param name="fid">The ID to index as part of this model</param>
    internal void AddForeignId(ForeignId fid)
    {
        m_ForeignIds.Add(fid.FormattedKey, fid);
    }

    /// <summary>
    /// Obtains all loaded feature IDs
    /// </summary>
    /// <returns>The native and foreign IDs that have been loaded (may be an empty array)</returns>
    internal FeatureId[] GetFeatureIds()
    {
        int numNative = m_NativeIds.Count;
        int numForeign = m_ForeignIds.Count;
        List<FeatureId> result = new List<FeatureId>(numNative + numForeign);

        foreach (NativeId nid in m_NativeIds.Values)
            result.Add(nid);

        foreach (ForeignId fid in m_ForeignIds.Values)
            result.Add(fid);

        return result.ToArray();
    }

    /// <summary>
    /// Obtains the edits that are dependent on features created by a specific edit.
    /// This is used by dialogs that provide rollforward previews. It returns 
    /// the subsequent operations that could be impacted as a result of the change.
    /// </summary>
    /// <param name="op">The operation in which the change will occur</param>
    /// <returns>The dependent edits (starting with the supplied editing operation)</returns>
    /// <remarks>This logic may need to be re-visited. In the past, edits could be
    /// "corrected" using references to features that were created after the original
    /// edit (provided that there was no dependency). This was ok, because the geometry
    /// for the features was persisted. In Backsight, the geometry needs to be recalculated
    /// when edits are deserialized from the database. The problem is that older edits may
    /// be dependent on something that happens later. As such, it MAY not be sufficient
    /// to look only at the subsequent edits (since earlier edits may have been modified
    /// (in-memory) via updates).
    /// </remarks>
    internal Operation[] Touch(Operation op)
    {
        // If the edit didn't create any spatial data, it can't impact any
        // other edits.
        if (op.FeatureCount == 0)
            return new Operation[] { op };

        // Locate the session containing the edit
        int sessionIndex = m_Sessions.FindIndex(s => object.ReferenceEquals(s, op.Session));
        if (sessionIndex < 0)
            throw new Exception("Cannot locate editing session for edit");

        // Process the session containing the edit
        List<Operation> result = new List<Operation>();
        m_Sessions[sessionIndex].Touch(result, op);

        // Process all subsequent sessions
        for (int i=sessionIndex+1; i<m_Sessions.Count; i++)
            m_Sessions[i].Touch(result, null);

        // Express the edits we found as an array
        return result.ToArray();
    }

    /// <summary>
    /// Retrieves all edits in this model, in the order they were performed.
    /// </summary>
    /// <returns>The editing operations in all sessions, starting with
    /// the earliest edit.</returns>
    internal Operation[] GetAllEdits()
    {
        List<Operation> result = new List<Operation>();

        foreach (Session s in m_Sessions)
            result.AddRange(s.Edits);

        return result.ToArray();
    }

    /// <summary>
    /// Gets the order in which edits should be calculated.
    /// </summary>
    /// <returns>The edits in this model, arranged in the order they should
    /// be calculated.</returns>
    /// <exception cref="ApplicationException">If the sequence cannot be determined (some
    /// sort of circular dependency exists)</exception>
    /// <remarks>
    /// If no updates have been applied, the result should be arranged in the order the edits were
    /// originally performed. Updates can disrupt this ordering because an update may modify an
    /// operation so that it refers to a feature created by a later operation (this is allowed
    /// so long as there is no circular dependency). Technically, this amounts to a
    /// <a href="https://en.wikipedia.org/wiki/Topological_sorting">topological sort</a>.
    /// </remarks>
    internal Operation[] GetCalculationSequence()
    {
        var todo = new List<Operation>(GetAllEdits());
        var next = new List<Operation>();
        var result = new List<Operation>(todo.Count);

        // Start by marking ALL edits for re-calculation
        foreach (Operation edit in todo)
            edit.ToCalculate = true;

        while (todo.Count > 0)
        {
            next.Clear();

            foreach (Operation edit in todo)
            {
                Operation[] requiredEdits = edit.GetRequiredEdits();

                if (Array.Exists<Operation>(requiredEdits, t => t.ToCalculate))
                {
                    next.Add(edit);
                }
                else
                {
                    edit.ToCalculate = false;
                    result.Add(edit);
                }
            }

            // We're in serious shit if the solution hasn't converged (this should
            // have been checked before any update is saved).
            if (next.Count == todo.Count)
                throw new ApplicationException("Unable to determine calculation sequence");

            // Swap the lists
            (todo, next) = (next, todo);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Ensures that all active features have been included in the spatial index.
    /// </summary>
    /// <remarks>When performing editing updates (where features get moved), the spatial index may end
    /// up being incomplete (changes made to support preview may not get reverted in situations where
    /// the user decides to cancel). The lack of a complete index may then make it appear that lines
    /// have disappeared (since the draw is driven through the index). So call this method if there is
    /// any question about the completeness of the index.</remarks>
    internal void EnsureFeaturesAreIndexed()
    {
        foreach (Feature f in m_Features.Values)
        {
            if (!f.IsInactive)
                m_Index.AddFeature(f);
        }
    }
}