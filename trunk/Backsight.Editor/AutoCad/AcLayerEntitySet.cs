using System;
using System.Collections.Generic;
using System.IO;
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
        internal AcLayerEntitySet()
        {
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
        /// <remarks>During exports from Backsight, any wildcard in the AutoCad layer
        /// name will be treated as a literal.</remarks>
        internal void TranslateLayerNames(string tranfile)
        {
            // Load the translation file
            string[] tranRecs = File.ReadAllLines(tranfile);

            foreach (string s in tranRecs)
            {
                // Only process lines that contain two strings, seperated by
                // an "=" character.
                string[] sa = s.Split('=');
                if (sa.Length != 2)
                    continue;

                // Ensure there is no leading or trailing white space.
                string layName = sa[0].Trim();
                string entName = sa[1].Trim();

                // Translate the entity name into an object (exit if the translation
                // file refers to an unknown entity type).
                IEntity ent;
                if (!m_Entities.TryGetValue(entName, out ent))
                    throw new InvalidDataException(
                        String.Format("Cannot find entity type called '{0}'", entName));

                // Store translation of entity to layer (so long as it's there - the
                // translation file may contain entries that relate to a different
                // Backsight editing layer).
                if (m_EntityToLayer.ContainsKey(entName))
                    m_EntityToLayer[entName] = layName;
            }
        }

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
