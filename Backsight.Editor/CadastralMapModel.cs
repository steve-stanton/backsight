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
using Sys=System.Diagnostics;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

using Backsight.Editor.Operations;
using Backsight.Index;
using Backsight.Environment;
using Backsight.Editor.Properties;

namespace Backsight.Editor
{
    [Serializable]
    class CadastralMapModel : IEditSpatialModel, ISpatialData, IInternalIdFactory
    {
        #region Static Methods

        internal static CadastralMapModel Current
        {
            get { return (CadastralEditController.Current.MapModel as CadastralMapModel); }
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
        CadastralIndex m_Index;

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
        /// this model (via calls to the <see "CreateNextInternalId"/> method).
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
        EntityFacade m_DefaultPointType;

        /// <summary>
        /// The default entity type for lines (may be null)
        /// </summary>
        EntityFacade m_DefaultLineType;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new empty model
        /// </summary>
        internal CadastralMapModel()
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
            /*
    m_pTheme = 0;
    m_pFont = 0;
             */
            m_Entities = new List<MapEntity>();
            m_Layers = new List<LayerFacade>();
            m_IdRanges = new List<IdRange>();
            m_DefaultPointType = null;
            m_DefaultLineType = null;
        }

        #endregion

        public override string ToString()
        {
            string name = Name;
            return (String.IsNullOrEmpty(name) ? "(Untitled)" : name);
        }

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

        internal ISpatialIndex Index
        {
            get { return m_Index; }
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

        internal IList<Session> Sessions { get { return m_Sessions; } }
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

        private DistanceUnit GetUnits(DistanceUnitType unitType)
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

        public void Save()
        {
            /*
            using (Stream s = new FileStream(@"C:\Temp\Test.bin"))
            {
                using (BinaryWriter w = new BinaryWriter(s))
                {
                    Write(w);
                }
            }
             */
        }

        /*
        internal void Write(BinaryWriter w)
        {
            w.Write(m_Format);
            w.Write(m_JobRegistrationId);
            w.Write(m_NumInternalIds);

            m_Meters.Write(w);
            m_Feet.Write(w);
            m_Chains.Write(w);
            m_AsEntered.Write(w);

            m_DisplayUnit.Write(w);
        }
         */

        /*
        DistanceUnit m_DisplayUnit;
        DistanceUnit m_EntryUnit;
        bool m_AutoNumber;
        double m_ShowLabelScale;
        double m_ShowPointScale;
        ILength m_PointHeight;
        bool m_AreIntersectionsDrawn;
        bool m_MaintainTopology;
        LineAnnotationStyle m_Annotation;
        double m_Rotation;
        uint m_MapScale;
        uint m_OpSequence;
        readonly CoordinateSystem m_CoordSystem;
        IEditWindow m_Window;
        IWindow m_UpdateWindow;
        IWindow m_DrawWindow;
        List<Person> m_People;
        List<Session> m_Sessions;
        List<MapEntity> m_Entities;
        List<LayerFacade> m_Layers;
        readonly List<IdRange> m_IdRanges;
        LayerFacade m_ActiveLayer;
        EntityFacade m_DefaultPointType;
        EntityFacade m_DefaultLineType;
         */
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

            MapEntity me = new MapEntity(this, entity);
            m_Entities.Add(me);
            return me;
        }

        public void Add(Operation op)
        {
            Feature[] data = op.Features;
            Window extent = new Window();
            foreach (Feature f in data)
            {
                extent.Union(f.Extent);
            }

            m_Window.Union(extent);

            if (m_Index==null)
                CreateIndex();
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
            CreateIndex();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateIndex()
        {
            //System.Windows.Forms.MessageBox.Show("start");
            //DateTime start = DateTime.Now;

            CadastralIndex index = new CadastralIndex();

            foreach (Session s in m_Sessions)
                s.AddToIndex(index);

            m_Index = index;
            index.DumpStats();
            //index.Dump();

            /*
            DateTime end = DateTime.Now;
            TimeSpan ts = end-start;
            System.Windows.Forms.MessageBox.Show(ts.ToString());

            features = this.GetFeatures(SpatialType.Line);
            System.Windows.Forms.MessageBox.Show("line count:"+features.Length);
            features = this.GetFeatures(SpatialType.Text);
            System.Windows.Forms.MessageBox.Show("text count:"+features.Length);
            */
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
        /// <devnote>This should probably move to CadastralEditController</devnote>
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
            IEntity mapEntity = GetRegisteredEntityType(e);
            PointFeature f = PointFeature.Create(p, mapEntity, ActiveLayer, creator);
            m_Window.Union(p);
            m_Index.Add(f);
            return f;
        }

        internal LineFeature AddLine(PointFeature from, PointFeature to, IEntity e, Operation creator)
        {
            LineFeature f = new LineFeature(e, creator, from, to);
            m_Window.Union(f.Extent);
            m_Index.Add(f);
            return f;
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
                Intersect(cq.Moves);

                BuildPolygons();
            }
        }

        /// <summary>
        /// Builds polygon topology for everything in the map. If the
        /// model contains more than one topological layer, they will all be built
        /// as separate layers.
        /// <para>
        /// There is no need to build network topology before calling this function,
        /// because network topology is calculated on-the-fly. However, it is assumed
        /// that the lines do in fact form a clean topological network. If any lines are
        /// detected which are coincident at a node, this function will return without
        /// trying to do any further processing.
        /// </para><para>
        /// Does NOT build labels. Make an explicit call to BuildLabels to do that.
        /// </para>
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
            //System.Windows.Forms.MessageBox.Show("start build");
            //DateTime start = DateTime.Now;
            new PolygonBuilder(this).Build();
            //DateTime end = DateTime.Now;
            //TimeSpan ts = end-start;
            //System.Windows.Forms.MessageBox.Show("done in "+ts.Milliseconds+" millisecs");
        }

        /// <summary>
        /// Intersects all features that have moved during rollforward.
        /// </summary>
        /// <param name="moves">The moved features.</param>
        void Intersect(List<Feature> moves)
        {
            /*
        	// Return if nothing has moved
            if (moves.Count==0)
                return;

            List<LineFeature> trims = new List<LineFeature>();
            int nMove = 0;

            foreach(Feature f in moves)
            {
                LineFeature line = (f as LineFeature);
                if (line==null)
                    continue;

                nMove++;
                line.SetNotMoved();
                line.Split(trims);

		        // If the line needs to be trimmed, add it to our list.
		        // We'll do it at the end.

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
            }

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
        /// <param name="addPoints">Should terminal points be added? Default=TRUE.</param>
        /// <returns>The newly created line</returns>
        internal LineFeature AddCircularArc (Circle circle, PointFeature start, PointFeature end,
            bool clockwise, IEntity lineEnt, Operation creator, bool addPoints)
        {
            ArcFeature result = new ArcFeature(lineEnt, creator, circle, start, end, clockwise);
            m_Window.Union(result.Extent);
            m_Index.Add(result);

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
    }
}
