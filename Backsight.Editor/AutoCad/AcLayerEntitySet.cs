using System;
using System.Collections.Generic;

using netDxf;
using Backsight.Environment;
using netDxf.Tables;
using System.Windows.Forms;

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
        /// The entity types, indexed by entity type name.
        /// </summary>
        readonly Dictionary<string, IEntity> m_Entities;

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


//	@parm	Variable indicating whether a layer associated with either
//			a topological layer data or an angle or a distance is being
//			asked for.

        /// <summary>
        /// Obtains an AutoCad layer for a spatial feature (creates the
        /// layer if necessary).
        /// </summary>
        /// <param name="f">The feature that's being exported</param>
        /// <param name="lineType">The AutoCad line type</param>
        /// <param name="useOtherLayer"></param>
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

//////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Given a CeEntity, return a pointer to the corresponding
//			AutoCad layer handle. <mf CacadLayerEntitySet::AddLayers>
//			must be called at some point previously (otherwise you'll
//			get back a null handle).
//
//	@parm	The entity type to search for.
//
//	@parm	Variable indicating whether a layer associated with either
//			a topological layer data or an angle or a distance is being
//			asked for.
//
//	@rdesc	Pointer to the layer handle (null if not found).
//
//////////////////////////////////////////////////////////////////////////////

        Layer GetLayer(IEntity entity, LineType lineType, bool useOtherLayer)
        {
            return null;
        }
        /*
AD_OBJHANDLE* CacadLayerEntitySet::GetpLayerHandle	( const CeEntity* const pEntity
													, const AD_OBJHANDLE LineTypeHandle
													, const LOGICAL UseOtherLayer
													) {

	assert (pEntity != 0);

	// Find the layer name
	CString Entity(pEntity->GetName());
	std::map<CString,CString,CompareStr>::iterator EntToLayIter;
	EntToLayIter = m_EntityToLayer.find(Entity);
	CString TheLayer = (*EntToLayIter).second;

	// Next it is checked if this is an output of a polygon ID and if it is then the
	// layer name has the suffix appended to it before the layer handle is accessed
	LOGICAL IsPolygonId = (UseOtherLayer && pEntity->IsPolygon());
	if(IsPolygonId && m_LabelHasSuffix)
		TheLayer += m_CEDPinLayerSuffix;
	else if(UseOtherLayer && pEntity->IsPoint() && m_AngleHasSuffix)
		TheLayer += m_CEDAngleLayerSuffix;
	else if(UseOtherLayer && pEntity->IsLine() && m_DistHasSuffix)
		TheLayer += m_CEDDistLayerSuffix;

	AD_LAY* pTheLayer = 0;
	// Find the actual layer
	std::map<CString,AD_LAY*,CompareStr>::iterator EntToLayPtrIter;
	EntToLayPtrIter = m_LayerToLayerPtr.find(TheLayer);
	if(EntToLayPtrIter == m_LayerToLayerPtr.end())
	{
		// layer not found - not made yet, make it first
		pTheLayer = MakeALayer( LineTypeHandle, pEntity, TheLayer );
	}
	else // layer found: use it
		pTheLayer = (*EntToLayPtrIter).second;

	// Get the layer's handle
	if ( pTheLayer )
		return &(pTheLayer->objhandle);
	else
		return 0;

} // end of GetpLayerHandle
        */

//////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return a pointer to the line type for this layer.
//
//	@rdesc	Pointer to the line type handle (null if no layers
//			have been defined).
//
//	@devnote At the moment, there is only one possibility (since CED
//			 does not handle line styles), so this method is kinda dumb.
//			 It should probably accept a pointer to an entity type.
//
//////////////////////////////////////////////////////////////////////////////

        /*
AD_OBJHANDLE* CacadLayerEntitySet::GetpLineTypeHandle ( void ) {

	std::map<CString,AD_LAY*,CompareStr>::iterator LayIter;

	LayIter = m_LayerToLayerPtr.begin();
	AD_LAY* pLayer = 0;
	if(LayIter != m_LayerToLayerPtr.end()) AD_LAY* pLayer = (*LayIter).second;

	if ( pLayer )
		return &(pLayer->linetypeobjhandle);
	else
		return 0;

} // end of GetpLineTypeHandle
        */

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Load an entity translation file. Note that during
//			exports from CED, any wildcard in the AutoCad layer
//			name will be treated as literals.
//
//	@parm	File spec of the translation file.
//	@parm	The CED map that the translations relate to.
//
//////////////////////////////////////////////////////////////////////

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

        netDxf.Tables.Layer MakeALayer(netDxf.Tables.LineType lineType, IEntity entity)
        {
            return null;
        }
        /*
AD_LAY* CacadLayerEntitySet::MakeALayer( const AD_OBJHANDLE LineTypeHandle,
		const CeEntity* pEntity, const CString TheLayer )
{
	std::list<CString>::iterator LayListIter;
	LOGICAL insertit;

	// go through the list of layers until end is reached or a place is
	// found for the layer for the current entity
	for(m_LayListIter = m_LayerList.begin();
			m_LayListIter != m_LayerList.end()  && *m_LayListIter < TheLayer;
			m_LayListIter++)
		;

	insertit = TRUE;

	if(m_LayerList.size() && m_LayListIter != m_LayerList.end() &&
			*m_LayListIter == TheLayer)
		insertit = FALSE;

	AD_LAY* pThisLayer = 0;

	if(insertit) {
	// a new layer name was found - insert it into the list and put it out to Acad
		if(m_LayerList.empty())
			m_LayerList.push_front(TheLayer);
		else
			m_LayerList.insert(m_LayListIter,TheLayer);

// Create the actual ACAD layer
		pThisLayer = new AD_LAY;

		// Set default layer initializes the blobs etc.
		adSetDefaultLayer(m_DwgHandle,pThisLayer);

		// DON'T overwrite memory if the entity type has a long name!
		INT4 maxname = sizeof(pThisLayer->name)-1;	// 32
		if ( strlen((LPCTSTR)TheLayer) > maxname ) {
			strncpy(pThisLayer->name,(LPCTSTR)TheLayer,maxname);
			pThisLayer->name[maxname] = '\0';
		}
		else
			strcpy(pThisLayer->name, (LPCTSTR)TheLayer);

		COLORREF rgb = pEntity->GetRGB();
		CacadColour Colour(rgb);
		pThisLayer->color = Colour.GetAcadColour();

		// LineType Handle
		adHancpy(pThisLayer->linetypeobjhandle,LineTypeHandle);

		// Generate new object handle
		adGenerateObjhandle(m_DwgHandle,pThisLayer->objhandle);
//

//
// Add an entry to the map associating the layer names to their pointers
// and add it's pointer to the vector of pointers
		m_LayerToLayerPtr[TheLayer] = pThisLayer;
		m_Layers.push_back(pThisLayer);
//
		// add layer via OpenDWG
		if (!adAddLayer(m_DwgHandle,pThisLayer)) {
			CString ErrMsg = "Error adding layer: ";
			ErrMsg += pThisLayer->name;
			ShowMessage((LPCTSTR)ErrMsg);
			return FALSE;
		}
	}

	return pThisLayer;
}
*/
    }
}
