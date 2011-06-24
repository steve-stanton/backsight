using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Backsight.Environment;

using netDxf;
using netDxf.Tables;

namespace Backsight.Editor.AutoCad
{
    /// <summary>
    /// AutoCad layer - entity collection.
    /// </summary>
    /// <was>CacadLayerEntitySet</was>
    class AcLayerEntitySet
    {
        #region Class data

        readonly CadastralMapModel m_MapModel;
        readonly DxfDocument m_AcDocument;

        /// <summary>
        /// Relationship of Backsight entity type with AutoCad layer. The
        /// key is the name of the entity type, the value is the AutoCad layer name.
        /// </summary>
        readonly Dictionary<string, string> m_EntityToLayer;

        /// <summary>
        /// The Backsight entity types, indexed by entity type name.
        /// </summary>
        readonly Dictionary<string, IEntity> m_Entities;

        /// <summary>
        /// AutoCad layers, indexed by AC layer name.
        /// </summary>
        readonly Dictionary<string, Layer> m_Layers;

        /// <summary>
        /// Any suffix to append to AC layer names that relate to PINs (property
        /// identifier numbers). Null if there is no suffix.
        /// </summary>
        readonly string m_PinLayerSuffix;

        /// <summary>
        /// Any suffix to append to AC layer names that relate to distance annotations.
        /// Null if there is no suffix.
        /// </summary>
        readonly string m_DistLayerSuffix;

        /// <summary>
        /// Any suffix to append to AC layer names that relate to angle annotations.
        /// Null if there is no suffix.
        /// </summary>
        readonly string m_AngleLayerSuffix;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AcLayerEntitySet"/> class.
        /// </summary>
        /// <param name="acDoc"></param>
        /// <param name="mapModel"></param>
        internal AcLayerEntitySet(DxfDocument acDoc, CadastralMapModel mapModel)
        {
            if (acDoc == null || mapModel == null)
                throw new ArgumentNullException();

            m_AcDocument = acDoc;
            m_MapModel = mapModel;
            m_EntityToLayer = new Dictionary<string, string>();
            m_Entities = new Dictionary<string, IEntity>();
            m_Layers = new Dictionary<string, Layer>();

            // Grab the entity types that relate to the active editing layer
            ILayer mapLayer = EditingController.Current.ActiveLayer;
            IEntity[] entities = EnvironmentContainer.EntityTypes(SpatialType.All, mapLayer);

            // Initialize entity type -> AC layer name (without any translation), as
            // well as entity name -> entity type index.
            foreach (IEntity e in entities)
            {
                m_EntityToLayer[e.Name] = e.Name;
                m_Entities[e.Name] = e;
            }

            // Grab any suffixes to append to AC layer names
            m_PinLayerSuffix = GetLayerSuffix("CED_PIN_LAYER_SUFFIX");
            m_DistLayerSuffix = GetLayerSuffix("CED_DIST_LAYER_SUFFIX");
            m_AngleLayerSuffix = GetLayerSuffix("CED_ANGLE_LAYER_SUFFIX");
        }

        #endregion

        /// <summary>
        /// Obtains an optional suffix that should be appended to AutoCad layer names,
        /// dependent on the type of data involved.
        /// </summary>
        /// <param name="suffixId">An identifier for the type of data</param>
        /// <returns>The suffix for the specified data (null if there's no suffix)</returns>
        static string GetLayerSuffix(string suffixId)
        {
            // The suffix is picked up through an environment variable. Kind of
            // old-fashioned, but leave it that way for the time being.
            string s = System.Environment.GetEnvironmentVariable(suffixId);

            if (s == null)
                return null;
            else
                return "-" + s;
        }

        /// <summary>
        /// Obtains an AutoCad layer for a spatial feature (creates the
        /// layer if necessary).
        /// </summary>
        /// <param name="f">The feature that's being exported</param>
        /// <param name="lineType">The AutoCad line type</param>
        /// <param name="useOtherLayer">Should a layer associated with a topological
        /// layer, or an angle, or a distance be obtained?</param>
        /// <returns>The matching AutoCad layer</returns>
        Layer GetLayer(IFeature f, LineType lineType, bool useOtherLayer)
        {
            // Get the entity type for the feature (possibly derived in
            // the case of a line).
            IEntity ent = GetEntity(f);

            // No layer if there is no entity type (e.g. construction circles).
            if (ent == null)
                return null;

            // Return the layer (if any).
            return GetLayer(ent, lineType, useOtherLayer);
        }

        /// <summary>
        /// Obtains the (possibly derived) entity type for the supplied feature.
        /// </summary>
        /// <param name="f">The feature of interest</param>
        /// <returns>The entity type (could be null)</returns>
        IEntity GetEntity(IFeature f)
        {
            // No entity if no feature!
            if (f == null)
                return null;

            // If it's not a line, the entity type can't be derived
            LineFeature line = (f as LineFeature);
            if (line == null)
                return f.EntityType;

            // Get the line's un-derived entity type (for things like
            // construction circles, the entity type is null)
            IEntity ent = line.EntityType;
            if (ent == null)
                return null;

            // Get the name of the (possibly derived) entity type.
            // It will be blank if the line has been cut up.
            string entName = line.DerivedEntityTypeName;
            if (ent.Name == entName)
                return ent;

            // Locate the corresponding entity type
            IEntity derivedEnt;
            if (!m_Entities.TryGetValue(entName, out derivedEnt))
            {
                string msg = String.Format("Cannot find entity type '{0}'", entName);
                MessageBox.Show(msg);
                return ent;
            }

            return derivedEnt;
        }

        /// <summary>
        /// Obtains an AutoCad layer for a specific entity type (creates the
        /// layer if necessary).
        /// </summary>
        /// <param name="entity">The entity type to translate (not null)</param>
        /// <param name="lineType">The AutoCad line type</param>
        /// <param name="useOtherLayer">Should a layer associated with a topological
        /// layer, or an angle, or a distance be obtained?</param>
        /// <returns></returns>
        Layer GetLayer(IEntity entity, LineType lineType, bool useOtherLayer)
        {
            if (entity == null)
                throw new ArgumentNullException();

            // Find the AC layer name
            string layerName = m_EntityToLayer[entity.Name];

            // If we're dealing with a polygon ID, or an angle annotation, or a distance
            // annotation, append any suffix
            if (useOtherLayer)
            {
                if (entity.IsPolygonValid && m_PinLayerSuffix != null)
                    layerName += m_PinLayerSuffix;
                else if (entity.IsPointValid && m_AngleLayerSuffix != null)
                    layerName += m_AngleLayerSuffix;
                else if (entity.IsLineValid && m_DistLayerSuffix != null)
                    layerName += m_DistLayerSuffix;
            }

            // Return the layer object if it was previously created.
            Layer layer;
            if (m_Layers.TryGetValue(layerName, out layer))
                return layer;

            // Create new layer
            return MakeALayer(lineType, entity, layerName); 
        }

        /// <summary>
        /// Loads an entity translation file.
        /// </summary>
        /// <param name="tranfile">File spec of the translation file.</param>
        /// <param name="cedmap">The CED map that the translations relate to.</param>
        /// <remarks>During exports from Backsight, any wildcard in the AutoCad layer
        /// name will be treated as a literal.</remarks>
        internal void TranslateLayerNames(string tranfile, CadastralMapModel cedmap)
        {
        }
        /*
void CacadLayerEntitySet::TranslateLayerNames ( const CString& tranfile
											  , const CeMap& cedmap ) {

	// We will need a pointer to the attribute database in order
	// to translate entity names into pointers.
	const CeAttributeStructure* const pdb = cedmap.GetpDatabase();

	// Open the entity translation file.
	FILE* fp;
	if ( (fp=fopen((LPCTSTR)tranfile,"r"))==0 ) {
		ShowMessage("Cannot read translation file");
		return;
	}

	UINT4 slen;				// Number of chars in record.
	CHARS* peq;				// Pointer to "=" character.
	CHARS str[256];			// Input buffer.
	CHARS layname[256];		// External name
	CHARS entname[256];		// Entity name
	CHARS msg[512];			// Message for user
	CeEntity* pEnt;			// Entity pointer

	while ( fgets(str,sizeof(str),fp) ) {

		//	Get the number of characters (excluding the newline at
		//	the end, as well as any other white space).
		slen = StrLength(str);
		if ( slen==0 ) continue;	// skip empty records
		str[slen] = '\0';

		//	Locate the position of the "=" character.
		peq = strchr(str,'=');
		if ( !peq || peq==&str[0] || peq==&str[slen-1] ) {
			sprintf ( msg, "Bad record (no = sign): %s\nContinue?", str );
			if ( ShowMessage(msg,MB_YESNO)==IDNO ) break;
		}

		// Pull out the layer name, and the name of the entity type. The
		// layer name always comes first (for both export and import).
		strcpy ( layname, strtok(str,"=") );
		strcpy ( entname, strtok(NULL,"=") );

		// Ensure there is no leading or trailing white space.

		CString laystr(layname);
		laystr.TrimLeft();
		laystr.TrimRight();

		CString entstr(entname);
		entstr.TrimLeft();
		entstr.TrimRight();

		// Translate the entity name into a pointer.
		pEnt = pdb->GetEntityPtr(os_string(entstr));
		if ( !pEnt ) {

			// If we did not find the entity type in the map, there's
			// something wrong with the translation file (refers to
			// an unknown entity type).

			sprintf(msg,"Cannot find entity type called '%s'",entstr);
			ShowMessage(msg);
		}
		else {

			// store translation of entity to layer
			m_EntityToLayer[entstr] = laystr;

			// If we did not find it, just silently ignore it (It is
			// possible that the translation file contains entries that
			// do not relate to the map's active theme. Since THIS
			// constructor does not load entity types that relate to
			// other themes, there may be nothing to find).

		}

	} // end while (fgets)

	// Close the file
	fclose(fp);

} // end of TranslateLayerNames
        */

        /// <summary>
        /// Creates a new AutoCad layer
        /// </summary>
        /// <param name="lineType">The AutoCad line type</param>
        /// <param name="entity">The Backsight entity type</param>
        /// <param name="layerName">The name to assign to the new layer</param>
        /// <returns>The created layer</returns>
        Layer MakeALayer(LineType lineType, IEntity entity, string layerName)
        {
            // TODO: May need to ensure the name isn't TOO long (older versions of AutoCad may
            // have assumed 32 chars max).

            // Return existing layer if it has already been created
            Layer result;
            if (m_Layers.TryGetValue(layerName, out result))
                return result;

            // Create the new layer. There's no need to add to the output DXF document (in
            // fact, there's no method to do so). The layer will be added by netDxf when
            // we call DxfDocument.AddEntity using an object that refers to a previously
            // unseen AutoCad layer.
            result = new Layer(layerName);
            result.Color = new AciColor(EntityUtil.GetColor(entity));
            result.LineType = lineType;

            // Remember the created object for subsequent lookup by this class.
            m_Layers.Add(layerName, result);

            return result;
        }
    }
}
