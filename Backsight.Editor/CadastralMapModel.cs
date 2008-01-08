/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

using Backsight.Editor.Operations;
using Backsight.Index;
using Backsight.Environment;
using Backsight.Editor.Properties;
using Backsight.Geometry;

namespace Backsight.Editor
{
    [Serializable]
    class CadastralMapModel : ISpatialModel, ISpatialData, IInternalIdFactory
    {
        #region Static Methods

        /// <summary>
        /// The current map model. When the application is running, this should be
        /// not null. It will be null at design time.
        /// </summary>
        internal static CadastralMapModel Current
        {
            get
            {
                EditingController cec = EditingController.Current;
                return (cec==null ? null : (cec.MapModel as CadastralMapModel));
            }
        }

        internal static CadastralMapModel Create()
        {
            return new CadastralMapModel();
        }

        /// <summary>
        /// Opens a map model (creates if it didn't previously exist)
        /// </summary>
        /// <param name="s">The name of the file holding the model</param>
        /// <returns>The map model</returns>
        internal static CadastralMapModel Open(string s)
        {
            CadastralMapModel cmm = LoadFile(s);
            cmm.m_ModelFileName = new ModelFileName(s);
            cmm.OnOpen();
            Settings.Default.LastMap = s;
            Settings.Default.Save();
            return cmm;
        }

        private static CadastralMapModel LoadFile(string inputFileName)
        {
            using (FileStream fs = new FileStream(inputFileName, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                object o = formatter.Deserialize(fs);
                return (CadastralMapModel)o;
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// The name of the file containing the current map model (including full path).
        /// </summary>
        [NonSerialized]
        ModelFileName m_ModelFileName;

        /// <summary>
        /// Spatial index for the data in this model.
        /// </summary>
        [NonSerialized]
        EditingIndex m_Index;

        /// <summary>
        /// Current format ID for the entire cadastral editor class structure.
        /// </summary>
        uint m_Format;

        /// <summary>
        /// An ID indicating that this map model has been registered as part of some
        /// greater data processing job. Zero means this model is unknown to the wider world.
        /// </summary>
        uint m_JobRegistrationId;

        /// <summary>
        /// The number of internal IDs that have been generated throughout the lifetime of
        /// this model (via calls to the <see cref="CreateNextInternalId"/> method).
        /// </summary>
        uint m_NumInternalIds;

        // Acceptable units of measurement ...
        readonly DistanceUnit m_Meters;
        readonly DistanceUnit m_Feet;
        readonly DistanceUnit m_Chains;
        readonly DistanceUnit m_AsEntered;

        /// <summary>
        /// Current display units
        /// </summary>
        DistanceUnit m_DisplayUnit;

        /// <summary>
        /// Current data entry units
        /// </summary>
        DistanceUnit m_EntryUnit;

        /// <summary>
        /// Should feature IDs be assigned automatically? (false if the user must specify).
        /// </summary>
        bool m_AutoNumber;

        /// <summary>
        /// Scale denominator at which labels will start to be drawn.
        /// </summary>
        double m_ShowLabelScale;

        /// <summary>
        /// Scale denominator at which points will start to be drawn.
        /// </summary>
        double m_ShowPointScale;

        /// <summary>
        /// Height of point symbols.
        /// </summary>
        ILength m_PointHeight;

        /// <summary>
        /// Should intersection points be drawn?
        /// </summary>
        bool m_AreIntersectionsDrawn;

        /// <summary>
        /// Should polygon topology be maintained during edits? (may get turned off
        /// temporarily during bulk loads).
        /// </summary>
        bool m_MaintainTopology;

        /// <summary>
        /// The style for annotating lines with distances (and angles)
        /// </summary>
        LineAnnotationStyle m_Annotation;

        /// <summary>
        /// Default rotation angle for text (in radians).
        /// </summary>
        double m_Rotation;

        /// <summary>
        /// The nominal map scale, for use in converting the size of fonts.
        /// </summary>
        uint m_MapScale;

        /// <summary>
        /// The highest sequence number assigned to any operation.
        /// </summary>
        uint m_OpSequence;

        /// <summary>
        /// The coordinate system.
        /// </summary>
        readonly CoordinateSystem m_CoordSystem;

        /// <summary>
        /// Window of all data in the map
        /// </summary>
        IEditWindow m_Window;

        /// <summary>
        /// The last draw window (if any).
        /// </summary>
        IWindow m_DrawWindow;

        /// <summary>
        /// People who have accessed the map model for read/write
        /// </summary>
        List<Person> m_People;

        /// <summary>
        /// Editing sessions.
        /// </summary>
        List<Session> m_Sessions;

        /// <summary>
        /// Entity types that have been utilized by the map (stubs that front information
        /// obtained from the environment database).
        /// </summary>
        List<MapEntity> m_Entities;

        /// <summary>
        /// Layers that have been utilized by the map (stubs that front information
        /// obtained from the environment database).
        /// </summary>
        List<LayerFacade> m_Layers;

        /// <summary>
        /// The ID ranges associated with this map.
        /// </summary>
        readonly List<IdRange> m_IdRanges;

        /// <summary>
        /// The currently active editing layer.
        /// </summary>
        LayerFacade m_ActiveLayer;

        // May want to change the following, since we previously had the ability to define
        // defaults for each layer... (the entity type implied the layer)

        /// <summary>
        /// The default entity type for points (may be null)
        /// </summary>
        MapEntity m_DefaultPointType;

        /// <summary>
        /// The default entity type for lines (may be null)
        /// </summary>
        MapEntity m_DefaultLineType;

        /// <summary>
        /// The default entity type for polygon labels (may be null)
        /// </summary>
        MapEntity m_DefaultPolygonType;

        /// <summary>
        /// The default entity type for text (may be null)
        /// </summary>
        MapEntity m_DefaultTextType;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new empty model
        /// </summary>
        CadastralMapModel()
        {
            m_Format = 1;

            m_ModelFileName = null;
            m_Meters = new DistanceUnit(DistanceUnitType.Meters);
            m_Feet = new DistanceUnit(DistanceUnitType.Feet);
            m_Chains = new DistanceUnit(DistanceUnitType.Chains);
            m_AsEntered = new DistanceUnit(DistanceUnitType.AsEntered);
            m_DisplayUnit = m_AsEntered;
            m_EntryUnit = m_Meters;
            m_AutoNumber = true;
            m_ShowLabelScale = 2000.0;
            m_ShowPointScale = 2000.0;
            m_PointHeight = new Length(2.0);
            m_AreIntersectionsDrawn = false;
            m_MaintainTopology = true;
            m_Annotation = new LineAnnotationStyle();
            m_Rotation = 0.0;
            m_MapScale = 2000; // arbitrary nominal scale
            m_OpSequence = 0;
            m_CoordSystem = new CoordinateSystem();
            m_Window = new Window();
            m_DrawWindow = new Window();
            m_People = new List<Person>();
            m_Sessions = new List<Session>();
            m_Entities = new List<MapEntity>();
            m_Layers = new List<LayerFacade>();
            m_IdRanges = new List<IdRange>();
            m_DefaultPointType = null;
            m_DefaultLineType = null;
            m_DefaultPolygonType = null;
            m_DefaultTextType = null;
            //m_Index = new EditingIndex();
        }

        #endregion

        public override string ToString()
        {
            string name = Name;
            return (String.IsNullOrEmpty(name) ? "(Untitled)" : name);
        }

        /// <summary>
        /// The user-name of the model (including the path).
        /// Blank if the model is undefined, or it's got a temporary system-generated name.
        /// </summary>
        public string Name
        {
            get
            {
                if (m_ModelFileName==null || m_ModelFileName.IsTempName)
                    return String.Empty;
                else
                    return m_ModelFileName.Name;
            }

            internal set
            {
                if (m_ModelFileName==null)
                    m_ModelFileName = new ModelFileName(value);
                else
                    m_ModelFileName.Name = value;
            }
        }

        /// <summary>
        /// The spatial index for this model (in a form that's suitable for typical queries).
        /// </summary>
        /// <remarks>The result may be null, since some older software may rely on
        /// a null result to determine the course of events. Where appropriate, use the
        /// <see cref="EditingIndex"/> property instead.</remarks>
        internal ISpatialIndex Index
        {
            get
            {
                /*
                // The index may be null if the model has just been deserialized
                if (m_Index==null)
                    m_Index = new EditingIndex();
                */
                return m_Index;
            }
        }

        /// <summary>
        /// The spatial index for this model (in a form that's suitable for editing).
        /// If it doesn't already exist, it will be created.
        /// </summary>
        internal EditingIndex EditingIndex
        {
            get
            {
                // The index may be null if the model has just been deserialized
                if (m_Index==null)
                    m_Index = new EditingIndex();

                return m_Index;
            }
        }

        internal double ShowPointScale
        {
            get { return m_ShowPointScale; }
            set { m_ShowPointScale = value; }
        }

        internal ILength PointHeight
        {
            get { return m_PointHeight; }
            set { m_PointHeight = value; }
        }

        internal bool AreIntersectionsDrawn
        {
            get { return m_AreIntersectionsDrawn; }
            set { m_AreIntersectionsDrawn = value; }
        }

        internal double ShowLabelScale
        {
            get { return m_ShowLabelScale; }
            set { m_ShowLabelScale = value; }
        }

        internal double DefaultTextRotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }

        internal uint NominalMapScale
        {
            get { return m_MapScale; }
            set { m_MapScale = value; }
        }

        internal LineAnnotationStyle Annotation
        {
            get { return m_Annotation; }
        }

        internal ReadOnlyCollection<Session> Sessions
        {
            get { return m_Sessions.AsReadOnly(); }
        }

        internal IList<Person> People { get { return m_People; } }

        internal DistanceUnit DisplayUnit
        {
            get { return m_DisplayUnit; }
            set { m_DisplayUnit = GetUnits(value.UnitType); }
        }

        internal DistanceUnit EntryUnit
        {
            get { return m_EntryUnit; }
            set { m_EntryUnit = GetUnits(value.UnitType); }
        }

        internal DistanceUnit GetUnits(DistanceUnitType unitType)
        {
            switch (unitType)
            {
                case DistanceUnitType.Meters:
                    return m_Meters;
                case DistanceUnitType.Feet:
                    return m_Feet;
                case DistanceUnitType.Chains:
                    return m_Chains;
                case DistanceUnitType.AsEntered:
                    return m_AsEntered;
            }

            throw new ArgumentException("Unexpected unit type");
        }

        /// <summary>
        /// Converts a string that represents a distance unit abbreviation into one
        /// of the <c>DistanceUnit</c> instances known to the map.
        /// </summary>
        /// <param name="abbr">The abbreviation to look for (not case-sensitive)</param>
        /// <returns>The corresponding unit (null if the unit cannot be determined)</returns>
        internal DistanceUnit GetUnit(string abbrev)
        {
            string a = abbrev.ToUpper().Trim();
            if (a.Length==0)
                return null;

            if (m_Meters.Abbreviation.ToUpper().StartsWith(a))
                return m_Meters;

            if (m_Feet.Abbreviation.ToUpper().StartsWith(a))
                return m_Feet;

            if (m_Chains.Abbreviation.ToUpper().StartsWith(a))
                return m_Chains;

            return null;
        }

        #region ISpatialModel Members

        public bool IsEmpty
        {
            get { return m_Window.IsEmpty; }
        }

        public IWindow Extent
        {
            get { return m_Window; }
        }

        public ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types)
        {
            return m_Index.QueryClosest(p, radius, types);
        }

        #endregion

        #region ISpatialData Members

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // Do nothing if the index hasn't been created yet
            if (m_Index==null)
                return;

            // Default to draw all defined feature types
            SpatialType types = SpatialType.Feature;

            // Suppress text if the display scale is too small
            if (display.MapScale > m_ShowLabelScale)
                types ^= SpatialType.Text;

            // Suppress points if the display scale is too small
            if (display.MapScale > m_ShowPointScale)
                types ^= SpatialType.Point;

            new DrawQuery(m_Index, display, style, types);

            // Draw intersections if necessary
            if (m_AreIntersectionsDrawn && (types & SpatialType.Point)!=0)
                (m_Index as EditingIndex).DrawIntersections(display);

            //(m_Index as SpatialIndex).Draw(display); // for testing
        }

        #endregion

        /// <summary>
        /// Adds a new session to this model.
        /// </summary>
        /// <returns>The newly created session.</returns>
        internal Session AddSession()
        {
            Person p = AddPerson();
            Session s = new Session(this, (LayerFacade)ActiveLayer);
            m_Sessions.Add(s);
            s.Start(p);
            return s;
        }

        /// <summary>
        /// Updates the end time of the current editing session (if there is one).
        /// </summary>
        internal void UpdateSession()
        {
            Session s = Session.CurrentSession;
            if (s!=null)
                s.UpdateEndTime();
        }

        /// <summary>
        /// Adds a person to this model. If the current user is already known to the
        /// model, you just get back a reference to that user. A person is only added
        /// if they were previously unknown.
        /// </summary>
        /// <returns>The current user</returns>
        private Person AddPerson()
        {
            Person p = Person.FindCurrentUser(m_People);
            if (p==null)
            {
                p = new Person();
                m_People.Add(p);
            }
            return p;
        }

        internal uint MakeBackup()
        {
            return 0;
            /*
            IDataController dc = DataRoot.Controller;
            if (dc==null)
                return 0;

            // If it's a new file that has NEVER been saved, it'll
            // have a temporary file name. So in that case, just
            // get the user to do a regular Save-As.
            if ( !m_IsSaved ) {
	            OnFileSaveAs();
	            return 0;
            }

            // Get the highest operation sequence number from the map.
            UINT4 maxop = m_pMap->GetMaxOp();

            // Get the file spec for the backup file.
            CString spec;
            spec.Format("%s%010d",GetMapSpec(),maxop);

            // Return if we've already made the backup.
            CFileStatus fstatus;
            if ( CFile::GetStatus((LPCTSTR)spec,fstatus) ) return 0;

            // Display a message to say what's happening.
            CString msg;
            msg.Format("Saving backup %s",spec);
            CdMessage* pMess = new CdMessage(msg);
            pMess->Create(CdMessage::IDD);

            // Copy the PSE database.
            m_osdb->copy((LPCTSTR)spec,0664,0);

            // Sleep for 1 second so the user can see the message
            // when dealing with small files.
            Sleep(1000);
            delete pMess;

            // If it got copied ok, return the sequence number.
            if ( CFile::GetStatus((LPCTSTR)spec,fstatus) ) return maxop;

            // Tell the user something screwed up.
            msg.Format("Failed to create backup file %s",spec);
            AfxMessageBox(msg);
            return 0;
             */
        }

        LayerFacade GetRegisteredLayer(ILayer layer)
        {
            LayerFacade f = m_Layers.Find(delegate(LayerFacade d) { return d.Id==layer.Id; });
            if (f==null)
            {
                f = new LayerFacade(layer);
                m_Layers.Add(f);
            }
            return f;
        }

        internal MapEntity GetRegisteredEntityType(IEntity entity)
        {
            if (entity==null)
                return null;

            if ((entity is MapEntity) && (entity as MapEntity).MapModel==this)
                return (entity as MapEntity);

            MapEntity me = m_Entities.Find(delegate(MapEntity e) { return e.Id==entity.Id; });
            if (me==null)
            {
                me = new MapEntity(this, entity);
                m_Entities.Add(me);
            }

            return me;
        }

        internal void Write(string fileName)
        {
            if (m_ModelFileName==null)
                m_ModelFileName = new ModelFileName(fileName);
            else
                m_ModelFileName.Name = fileName;

            Write();
        }

        internal void Write()
        {
            if (m_ModelFileName==null)
                m_ModelFileName = new ModelFileName();

            using (FileStream fs = new FileStream(m_ModelFileName.Name, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, this);
            }
        }

        public void Close()
        {
            Session s = Session.CurrentSession;
            if (s!=null)
            {
                s.End();

                if (s.IsEmpty)
                    m_Sessions.Remove(s);
            }
        }

        /// <summary>
        /// Performs initialization whenever a specific map model is opened.
        /// </summary>
        private void OnOpen()
        {
            // Ensure all entity type wrappers know that they're part of this model.
            foreach (MapEntity me in m_Entities)
                me.MapModel = this;

            // Define information relating to the environment
            AssignData(EnvironmentContainer.Current);

            // Initialize operation sequencing
            m_OpSequence = 0;

            // Notify all sessions
            foreach (Session s in m_Sessions)
            {
                s.OnLoad(this);

                // Ensure the session refers to the same layer data as this model
                /*
                LayerFacade layerFacade = s.LayerFacade;
                if (layerFacade==null)
                    throw new Exception("Session is not associated with an editing layer");

                LayerFacade modelFacade = FindLayerById(layerFacade.Id);
                 */
            }

            // Generate a spatial index
            foreach (Session s in m_Sessions)
                s.AddToIndex();
        }

        /*
        private void CreateIndex()
        {
            EditingIndex index = new EditingIndex();

            foreach (Session s in m_Sessions)
                s.AddToIndex(index);

            m_Index = index;
        }
         */

        /// <summary>
        /// Reserves the next operation sequence number. This should be obtained each time
        /// a new editing operation is appended to this model. It also gets used when a map
        /// model is opened (since the op sequence number is not persisted).
        /// </summary>
        /// <returns>The next available operation sequence number.</returns>
        internal uint ReserveNextOpSequence()
        {
            m_OpSequence++;
            return m_OpSequence;
        }

        /// <summary>
        /// The last operation sequence number returned by <see cref="ReserveNextOpSequence"/>
        /// </summary>
        internal uint LastOpSequence
        {
            get { return m_OpSequence; }
        }

        /// <summary>
        /// Associates environment-related facades with information obtained from
        /// the supplied environment container.
        /// </summary>
        /// <param name="ec">The container holding environment data (not null)</param>
        private void AssignData(IEnvironmentContainer ec)
        {
            if (ec==null)
                throw new ArgumentNullException();

            foreach (MapEntity f in m_Entities)
                f.Data = EnvironmentItemFacade<IEntity>.FindById(ec.EntityTypes, f.Id);

            foreach (LayerFacade f in m_Layers)
                f.Data = EnvironmentItemFacade<ILayer>.FindById(ec.Layers, f.Id);
        }

        public ISpatialSystem SpatialSystem
        {
            get { return m_CoordSystem; }
        }

        internal ICoordinateSystem CoordinateSystem
        {
            get { return m_CoordSystem; }
        }

        /// <summary>
        /// Remembers that rollforward is about to begin.
        /// </summary>
        /// <devnote>This should probably move to EditingController</devnote>
        internal void StartRollforward()
        {
            // @devnote There was a potential memory leak here.
            // If CeMap::Rollforward did not eventually complete for
            // any reason, memory for CeLabelShift objects was not
            // deleted. DO NOT do it here, because the memory may have
            // been allocated in a prior session. I tried to fix this
            // via CeMap::BeforeHook, but that causes a recursive fault.
            // As it stands now, FinishRollforward is supposed to also
            // remove (and delete) any leftover label-shifts. So the
            // removal that happens here is only expected to cover
            // those cases where old files have spurious pointers.

            //m_LabelShifts.RemoveAll();
        }

        internal List<IdRange> IdRanges
        {
            get { return m_IdRanges; }
        }

        internal ILayer ActiveLayer
        {
            get { return m_ActiveLayer; }

            set
            {
                if (value==null)
                    throw new ArgumentNullException();

                // Ensure the layer we save is one of the instances known to this model
                m_ActiveLayer = GetRegisteredLayer(value);

                Session s = Session.CurrentSession;
                if (s==null)
                    AddSession();
                else if (s.ActiveLayer.Id!=m_ActiveLayer.Id)
                {
                    s.End();
                    AddSession();
                }
            }
        }

        /// <summary>
        /// Creates a new point feature as part of this model. This expands the extent of
        /// this model, and updates the spatial index. The caller is responsible for assigning
        /// any ID that the new point should have.
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
            PointFeature f = PointFeature.Create(p, e, ActiveLayer, creator);
            //m_Window.Union(p);
            //m_Index.Add(f);
            return f;
        }

        /// <summary>
        /// Ensures a point feature exists at a specific location in this map model.
        /// </summary>
        /// <param name="p">The position where a point feature is required</param>
        /// <param name="creator">The operation that should be recorded as the creator
        /// of any newly created point</param>
        /// <returns>The point feature at the specified position (may be a new point)</returns>
        internal PointFeature EnsurePointExists(IPosition p, Operation creator)
        {
            if (p is PointFeature)
                return (p as PointFeature);

            EditingIndex index = EditingIndex;
            PointFeature pf = (index.QueryClosest(p, Length.Zero, SpatialType.Point) as PointFeature);
            if (pf==null)
                pf = AddPoint(p, DefaultPointType, creator);

            return pf;
        }

        /// <summary>
        /// Creates a new line feature that connects two points.
        /// </summary>
        /// <param name="from">The starting point for the new line</param>
        /// <param name="to">The end point for the new line</param>
        /// <param name="e">The entity type for the line (not null)</param>
        /// <param name="creator">The operation creating the line</param>
        /// <returns>The created line feature.</returns>
        internal LineFeature AddLine(PointFeature from, PointFeature to, IEntity e, Operation creator)
        {
            LineFeature f = new LineFeature(e, creator, from, to);
            //m_Window.Union(f.Extent);
            //m_Index.Add(f);

            // If topology needs to be maintained, ensure polygons in the vicinity have
            // been marked for rebuild
            if (m_MaintainTopology)
                f.MarkPolygons();

            return f;
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
                f.AddToIndex(index);
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
        /// The circles associated with an array of features
        /// </summary>
        /// <param name="fa">The features of interest</param>
        /// <returns>The circles (if any) that are associated with the supplied features</returns>
        static List<Circle> GetCreatedCircles(Feature[] fa)
        {
            if (fa.Length==0)
                return new List<Circle>();

            // The following will be overkill in most cases, but not for things like
            // bulk data imports...

            // The circles found so far will be noted in an index that's keyed by the
            // internal ID of the point at the center of the circle.
            Dictionary<string, List<Circle>> dic = new Dictionary<string, List<Circle>>();

            List<Circle> result = new List<Circle>(100);

            foreach (Feature f in fa)
            {
                if (f is ArcFeature)
                {
                    Circle c = (f as ArcFeature).Circle;

                    if (c.Creator == f.Creator)
                    {
                        string centerPointId = c.CenterPoint.DataId;
                        bool addToResult = false;
                        List<Circle> circles;

                        if (dic.TryGetValue(centerPointId, out circles))
                        {
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
            CleanupQuery cq = new CleanupQuery(this);

            if (m_MaintainTopology)
            {
                // If maintaining topology, ensure that all moved stuff has been intersected
                // (and that any trimmed lines have been adjusted appropriately).
                //System.Windows.Forms.MessageBox.Show("starting intersect");
                //Stopwatch sw = Stopwatch.StartNew();
                Intersect(cq.Moves);
                //sw.Stop();
                //System.Windows.Forms.MessageBox.Show(sw.Elapsed.ToString());

                //System.Windows.Forms.MessageBox.Show("building topology");
                //sw.Reset();
                //sw.Start();
                BuildPolygons();
                //sw.Stop();
                //System.Windows.Forms.MessageBox.Show(sw.Elapsed.ToString());
            }
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
            Trace.Write("Building polygons");
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
            List<LineFeature> trims = new List<LineFeature>();
            int nMove = 0;

            foreach(Feature f in moves)
            {
                f.IsMoved = false;
                LineFeature line = (f as LineFeature);
                if (line==null)
                    continue;

                nMove++;
                line.Split(trims);

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
                CleanupQuery cq = new CleanupQuery(this);
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

                List<ISpatialObject> nomoves = new List<ISpatialObject>(); // shouldn't get any MORE moves
		        CleanArcs(nomoves);
                Debug.Assert(nomoves.Count==0);
		        CleanPolygons();

		        // No need to clean text (well, I can't think of any).
		        m_Index.Clean(m_UpdateWindow, false);
            }
             */
        }

        internal bool IsAutoNumber
        {
            get { return m_AutoNumber; }
        }

        /// <summary>
        /// Deletes the file that contains this model.
        /// </summary>
        internal void Discard()
        {
            if (m_ModelFileName!=null)
            {
                File.Delete(m_ModelFileName.Name);
                m_ModelFileName = null;
            }
        }

        internal void SetDefaultEntity(SpatialType t, IEntity e)
        {
            if (t == SpatialType.Point)
                DefaultPointType = e;
            else if (t == SpatialType.Line)
                DefaultLineType = e;
            else if (t == SpatialType.Polygon)
                DefaultPolygonType = e;
            else if (t == SpatialType.Text)
                DefaultTextType = e;
            else
                throw new NotImplementedException("SetDefaultEntity");
        }

        internal IEntity DefaultPointType
        {
            get { return m_DefaultPointType; }
            set { m_DefaultPointType = GetRegisteredEntityType(value); }
        }

        internal IEntity DefaultLineType
        {
            get { return m_DefaultLineType; }
            set { m_DefaultLineType = GetRegisteredEntityType(value); }
        }

        internal IEntity DefaultPolygonType
        {
            get { return m_DefaultPolygonType; }
            set { m_DefaultPolygonType = GetRegisteredEntityType(value); }
        }

        internal IEntity DefaultTextType
        {
            get { return m_DefaultTextType; }
            set { m_DefaultTextType = GetRegisteredEntityType(value); }
        }

        internal IWindow DrawExtent
        {
            get { return m_DrawWindow; }
            set { m_DrawWindow = value; }
        }

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
        /// already attached to the point at the centre of the circle, you get back the
        /// existing circle. If there is no such circle, one will be created, and the
        /// centre point will be modified to refer to it.
        /// </summary>
        /// <param name="c">The point at the centre.</param>
        /// <param name="radius">The radius (on the ground)</param>
        /// <returns></returns>
        internal Circle AddCircle(PointFeature c, ILength radius)
        {
            // Try to match the centre point with an existing circle with
            // the specified radius.
	        Circle circle = c.GetCircle(radius);
	        if (circle!=null)
                return circle;

            // Create a new circle.
            circle = new Circle(c, radius);

            // Refer the centre point to the circle.
            circle.AddReferences();
            return circle;
        }


        /// <summary>
        /// Adds a circular arc that corresponds to a complete circle.
        /// </summary>
        /// <param name="center">The point at the center of the circle.</param>
        /// <param name="radius">The radius of the circle (on the ground).</param>
        /// <param name="start">The point (if any) that should be regarded as the
        /// "start" of the circle. If null, an arbitrary start point at the top of
        /// the circle will be used.</param>
        /// <returns>The newly created line</returns>
        internal ArcFeature AddCompleteCircularArc(PointFeature center, ILength radius, PointFeature start)
        {
            throw new NotImplementedException("CadastralMapModel.AddCompleteCircularArc");
        }
        /*
	// Do we already have a suitable circle object?
	CeCircle* pCircle = centre.GetCircle(radius);

	// If not, create one.
	if ( !pCircle ) {
		pCircle = new ( os_database::of(&centre)
					  , os_ts<CeCircle>::get() ) CeCircle(centre,radius);
		if ( !pCircle ) {
			ShowMessage("CeMap::AddCircle\nCannot create circle");
			return 0;
		}
	}

	// Create a curve primitive ...

	// If an explicit start point has been supplied, that location
	// will be treated as the BC (and EC) of the new curve. Otherwise
	// we'll define an arbitrary location at the top of the circle.

	CeLocation* pBC = 0;
	if ( pStart )
		pBC = (CeLocation*)pStart->GetpVertex();
	else {
		CeVertex bc(centre.GetEasting(),centre.GetNorthing()+radius);
		pBC = (CeLocation*)this->AddLocation(bc);
	}

	// Create a clockwise curve.

	CeCurve* pCurve = new ( os_database::of(pCircle)
						  , os_ts<CeCurve>::get() )
							CeCurve(*pCircle,*pBC,*pBC,TRUE);

	// Refer the circle to the curve.
	pCircle->AddObject(*pCurve);

	// Refer the start (=end) location to the curve.
	pBC->AddObject(*pCurve);

	// Append the curve to the spatial index.
	m_Space.Add(pCurve);

	// Add the user-perceived line as well (without any entity type).
	CeArc* pArc = this->AddArc(pCurve,0); 

	// The line is NEVER topological.
	pArc->SetTopology(FALSE);

	// Get the window of the circle, and ensure the map's window
	// is big enough to enclose it. The only other place this is
	// done is in CeMap::AddLocation. However, since a circle may
	// extend beyond any defined location, we need to do it here
	// too.
	CeWindow win(*pCurve);
	m_Window.Expand(win);

	return pArc;

} // end of AddCircle
         */

        /// <summary>
        /// Add a circular arc to the map, along with a line on the currently active editing layer,
        /// along with terminal points.
        /// </summary>
        /// <param name="circle">The circle on which the arc lies.</param>
        /// <param name="start">The point at the start of arc.</param>
        /// <param name="end">The point at the end of arc.</param>
        /// <param name="clockwise">True if the arc is clockwise.</param>
        /// <param name="lineEnt">The entity type for the new line. Null is valid, referring
        /// to circle construction lines.</param>
        /// <param name="creator">The editing operation creating the arc</param>
        /// <returns>The newly created line</returns>
        internal LineFeature AddCircularArc (Circle circle, PointFeature start, PointFeature end,
            bool clockwise, IEntity lineEnt, Operation creator)
        {
            ArcFeature result = new ArcFeature(lineEnt, creator, circle, start, end, clockwise);
            //m_Window.Union(result.Extent);
            //m_Index.Add(result);

            // If topology needs to be maintained, ensure polygons in the vicinity have
            // been marked for rebuild
            if (m_MaintainTopology)
                result.MarkPolygons();

            return result;
        }

        /// <summary>
        /// Creates a new <c>InternalIdValue</c> that can be used to uniquely identify
        /// objects associated with this model.
        /// </summary>
        /// <returns>The created ID value</returns>
        public InternalIdValue CreateNextInternalId()
        {
            m_NumInternalIds++;
            return new InternalIdValue(m_JobRegistrationId, m_NumInternalIds);
        }

        /// <summary>
        /// The number of internal IDs that have been generated for this model.
        /// </summary>
        internal uint InternalIdCount
        {
            get { return m_NumInternalIds; }
        }

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
                status = Session.CurrentSession.Rollback();
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

        /// <summary>
        /// Should polygon topology be maintained during edits? (may get turned off
        /// temporarily during bulk loads).
        /// </summary>
        internal bool IsMaintainingTopology
        {
            get { return m_MaintainTopology; }
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
            ITerminal t = m_Index.FindTerminal(pg);
            if (t!=null)
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
        internal bool IsCommittingEdit
        {
            get { return false; }
        }
    }
}
